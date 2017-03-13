using System;
using System.ComponentModel;
using System.Linq;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using Quartz.Forms.Controls;
using DreamTeam.Xamarin.LGSideMenuController;
using CurrentPlatform = Xamarin.Forms.Platform.iOS.Platform;
using PointF = CoreGraphics.CGPoint;

[assembly: ExportRenderer(typeof(ExtendedMasterDetailPage), typeof(Quartz.iOS.Platform.Renderers.ExtendedMasterDetailPageRenderer))]

namespace Quartz.iOS.Platform.Renderers
{
	public class ExtendedMasterDetailPageRenderer : LGSideMenuController, IVisualElementRenderer
	{
		#region iVars

		bool _disposed;
		EventTracker _events = null;
		VisualElementTracker _tracker = null;
		bool _presented = false;

		#endregion

		#region Properties

		/// <summary>
		/// Gets the page controller.
		/// </summary>
		/// <value>The page controller.</value>
		IPageController PageController => Element as IPageController;

		/// <summary>
		/// Gets the master detail page controller.
		/// </summary>
		/// <value>The master detail page controller.</value>
		IMasterDetailPageController MasterDetailPageController => Element as IMasterDetailPageController;

		/// <summary>
		/// Gets the extended master details page controller.
		/// </summary>
		/// <value>The extended master details page controller.</value>
		ExtendedMasterDetailPage ExtendedMasterDetailsPageController => Element as ExtendedMasterDetailPage;

		/// <summary>
		/// Gets the element.
		/// </summary>
		/// <value>The element.</value>
		public VisualElement Element { get; private set; }

		/// <summary>
		/// Gets the menu direction.
		/// </summary>
		/// <value>The menu direction.</value>
		public ExtendedMasterDetailPage.Direction MenuDirection
		{
			get
			{
				return ExtendedMasterDetailsPageController.MenuDirection;
			}
		}

		/// <summary>
		/// Gets the menu view controller.
		/// </summary>
		/// <value>The menu view controller.</value>
		public UIViewController MenuViewController
		{
			get
			{
				return (this.MenuDirection == ExtendedMasterDetailPage.Direction.Left)
							  ? this.LeftViewController
							  : this.RightViewController;
			}
			private set
			{
				if (this.MenuDirection == ExtendedMasterDetailPage.Direction.Left)
				{
					this.LeftViewController = value;
				}
				else
				{
					this.RightViewController = value;
				}
			}
		}

		bool Presented
		{
			get { return _presented; }
			set
			{
				if (_presented == value)
				{
					return;
				}

				if (value)
				{
					if (this.MenuDirection == ExtendedMasterDetailPage.Direction.Left)
					{
						this.ShowLeftViewAnimated(true, null);
					}
					else
					{
						this.ShowRightViewAnimated(true, null);
					}
				}
				else
				{
					if (this.MenuDirection == ExtendedMasterDetailPage.Direction.Left)
					{
						this.HideLeftViewAnimated(true, null);
					}
					else
					{
						this.HideRightViewAnimated(true, null);
					}
				}

				this._presented = value;
				((IElementController)Element).SetValueFromRenderer(MasterDetailPage.IsPresentedProperty, value);
			}
		}


		#endregion

		/// <summary>
		/// Occurs when the element is changed.
		/// </summary>
		public event EventHandler<VisualElementChangedEventArgs> ElementChanged;

		public ExtendedMasterDetailPageRenderer()
			: base()
		{
		}

		#region IVisualElementRenderer

		public SizeRequest GetDesiredSize(double widthConstraint, double heightConstraint)
		{
			return this.NativeView.GetSizeRequest(widthConstraint, heightConstraint);
		}

		public UIView NativeView
		{
			get { return this.View; }
		}

		public void SetElement(VisualElement element)
		{
			//--------------------------------------------
			//	Initialize vars
			//--------------------------------------------
			if (this._tracker == null)
			{
				_tracker = new VisualElementTracker(this);
			}

			if (this._events == null)
			{
				_events = new EventTracker(this);
				_events.LoadEvents(View);
			}

			//--------------------------------------------
			//	Reset the old element 
			//--------------------------------------------
			var oldElement = this.Element;
			if (oldElement != null)
			{
				oldElement.SizeChanged -= PageOnSizeChanged;
				oldElement.PropertyChanged -= HandlePropertyChanged;
			}

			//--------------------------------------------
			//	Assign the new element
			//--------------------------------------------

			this.Element = element;
			this.Element.SizeChanged += PageOnSizeChanged;
			this.Element.PropertyChanged += HandlePropertyChanged;

			this.Presented = ((MasterDetailPage)Element).IsPresented;

			//--------------------------------------------
			//	Setup the root and menu view controller
			//--------------------------------------------

			this.RootViewController = new ChildViewController();
			this.LeftViewController = new ChildViewController();
			this.RightViewController = new ChildViewController();

			//--------------------------------------------
			//	Fire the element changed event
			//--------------------------------------------

			OnElementChanged(new VisualElementChangedEventArgs(oldElement, element));

			//--------------------------------------------
			//	Update the view
			//--------------------------------------------

			/* Master Detail Container */
			UpdateMasterDetailContainers();

			/* Background */
			UpdateBackground();

			/* Menu Background */
			UpdateMenuBackground();

			/* Menu Width */
			UpdateMenuWidth();

			/* Menu Presentation Style */
			UpdateMenuPresentationStyle();

			/* Swipe Range */
			UpdateSwipeRange();

			//--------------------------------------------
			//	Attempt to automatically create a menu click event
			//--------------------------------------------

			var detailRenderer = CurrentPlatform.GetRenderer((element as MasterDetailPage).Detail) as UINavigationController;
			if (detailRenderer != null)
			{
				UIViewController firstPage = detailRenderer?.ViewControllers.FirstOrDefault();
				firstPage.NavigationItem.LeftBarButtonItem.Clicked += HandleMenuIcon_Clicked;
			}

			//EffectUtilities.RegisterEffectControlProvider(this, oldElement, element);

			//if (element != null)
			//	element.SendViewInitialized(NativeView);
		}

		public void SetElementSize(Size size)
		{
			this.Element.Layout(new Rectangle(Element.X, Element.Y, size.Width, size.Height));
		}

		public UIViewController ViewController
		{
			get { return this; }
		}

		#endregion

		public override void ViewDidAppear(bool animated)
		{
			base.ViewDidAppear(animated);
			PageController.SendAppearing();
		}

		public override void ViewDidDisappear(bool animated)
		{
			base.ViewDidDisappear(animated);
			PageController?.SendDisappearing();
		}

		public override void ViewDidLayoutSubviews()
		{
			base.ViewDidLayoutSubviews();

			LayoutChildren();
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && !_disposed)
			{
				Element.SizeChanged -= PageOnSizeChanged;
				Element.PropertyChanged -= HandlePropertyChanged;

				if (_tracker != null)
				{
					_tracker.Dispose();
					_tracker = null;
				}

				if (_events != null)
				{
					_events.Dispose();
					_events = null;
				}

				EmptyContainers();

				PageController.SendDisappearing();

				_disposed = true;
			}

			base.Dispose(disposing);
		}

		protected virtual void OnElementChanged(VisualElementChangedEventArgs e)
		{
			this.ElementChanged?.Invoke(this, e);
		}

		public override UIViewController ChildViewControllerForStatusBarHidden()
		{
			if (((MasterDetailPage)Element).Detail != null)
				return (UIViewController)CurrentPlatform.GetRenderer(((MasterDetailPage)Element).Detail);
			else
				return base.ChildViewControllerForStatusBarHidden();
		}

		#region Event Handlers

		//void HandleMasterPropertyChanged(object sender, PropertyChangedEventArgs e)
		//{
		//	if (e.PropertyName == Page.IconProperty.PropertyName || e.PropertyName == Page.TitleProperty.PropertyName)
		//		UpdateLeftBarButton();
		//}

		void HandlePropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == "Master" || e.PropertyName == "Detail")
			{
				UpdateMasterDetailContainers();
			}
			else if (e.PropertyName == VisualElement.BackgroundColorProperty.PropertyName)
			{
				UpdateBackground();
			}
			else if (e.PropertyName == Page.BackgroundImageProperty.PropertyName)
			{
				UpdateBackground();
			}
			else if (e.PropertyName == MasterDetailPage.IsPresentedProperty.PropertyName)
			{
				this.Presented = ExtendedMasterDetailsPageController.IsPresented; // This will fire up the presentation logic.
			}
			else if (e.PropertyName == ExtendedMasterDetailPage.MenuBackgroundColorProperty.PropertyName)
			{
				UpdateMenuBackground();
			}
			else if (e.PropertyName == ExtendedMasterDetailPage.MenuBackgroundImageProperty.PropertyName)
			{
				UpdateMenuBackground();
			}
			else if (e.PropertyName == ExtendedMasterDetailPage.MenuPresentationStyleProperty.PropertyName)
			{
				UpdateMenuPresentationStyle();
			}
			else if (e.PropertyName == ExtendedMasterDetailPage.MenuWidthProperty.PropertyName)
			{
				UpdateMenuWidth();
			}
			else if (e.PropertyName == ExtendedMasterDetailPage.SwipeRangeProperty.PropertyName)
			{
				UpdateSwipeRange();
			}
			else if (e.PropertyName == ExtendedMasterDetailPage.MenuDirectionProperty.PropertyName)
			{
				/* If we're changing the menu direction, lets update the entire properties */
				UpdateMenuBackground();
				UpdateMenuWidth();
				UpdateMenuPresentationStyle();
				UpdateSwipeRange();
			}

		}

		void HandleMenuIcon_Clicked(object sender, EventArgs e)
		{
			if (this.MenuDirection == ExtendedMasterDetailPage.Direction.Left)
			{
				this.ShowLeftViewAnimated(true, null);
			}
			else
			{
				this.ShowRightViewAnimated(true, null);
			}
		}

		void PageOnSizeChanged(object sender, EventArgs eventArgs)
		{
			LayoutChildren();
		}

		#endregion

		#region Private Helpers

		/// <summary>
		/// Empties the containers.
		/// </summary>
		void EmptyContainers()
		{
			/* Remove views */
			foreach (var child in this.RootViewController.View.Subviews)
				child.RemoveFromSuperview();

			/* Remove VCs */
			foreach (var vc in this.RootViewController.ChildViewControllers)
				vc.RemoveFromParentViewController();
		}

		/// <summary>
		/// Updates the background.
		/// </summary>
		void UpdateBackground()
		{
			if (!string.IsNullOrEmpty(((Page)Element).BackgroundImage))
			{
				View.BackgroundColor = UIColor.FromPatternImage(UIImage.FromBundle(((Page)Element).BackgroundImage));
			}
			else if (Element.BackgroundColor == Color.Default)
			{
				View.BackgroundColor = UIColor.White;
			}
			else
			{
				View.BackgroundColor = Element.BackgroundColor.ToUIColor();
			}
		}

		/// <summary>
		/// Updates the menu background.
		/// </summary>
		void UpdateMenuBackground()
		{
			if (!string.IsNullOrEmpty(ExtendedMasterDetailsPageController.MenuBackgroundImage))
			{
				if (this.MenuDirection == ExtendedMasterDetailPage.Direction.Left)
				{
					this.LeftViewBackgroundColor = UIColor.FromPatternImage(UIImage.FromBundle(ExtendedMasterDetailsPageController.MenuBackgroundImage));
					this.RightViewBackgroundColor = UIColor.Clear;
				}
				else
				{
					this.RightViewBackgroundColor = UIColor.FromPatternImage(UIImage.FromBundle(ExtendedMasterDetailsPageController.MenuBackgroundImage));
					this.LeftViewBackgroundColor = UIColor.Clear;
				}
			}
			else if (ExtendedMasterDetailsPageController.MenuBackgroundColor == Color.Default)
			{
				if (this.MenuDirection == ExtendedMasterDetailPage.Direction.Left)
				{
					this.LeftViewBackgroundColor = UIColor.White;
					this.RightViewBackgroundColor = UIColor.Clear;
				}
				else
				{
					this.RightViewBackgroundColor = UIColor.White;
					this.LeftViewBackgroundColor = UIColor.Clear;
				}
			}
			else
			{
				if (this.MenuDirection == ExtendedMasterDetailPage.Direction.Left)
				{
					this.LeftViewBackgroundColor = ExtendedMasterDetailsPageController.MenuBackgroundColor.ToUIColor();
					this.RightViewBackgroundColor = UIColor.Clear;
				}
				else
				{
					this.RightViewBackgroundColor = ExtendedMasterDetailsPageController.MenuBackgroundColor.ToUIColor();
					this.LeftViewBackgroundColor = UIColor.Clear;
				}
			}
		}

		/// <summary>
		/// Updates the width of the menu.
		/// </summary>
		void UpdateMenuWidth()
		{
			if (this.MenuDirection == ExtendedMasterDetailPage.Direction.Left)
			{
				this.LeftViewWidth = ExtendedMasterDetailsPageController.MenuWidth;
				this.RightViewWidth = 0.0f;
			}
			else
			{
				this.RightViewWidth = ExtendedMasterDetailsPageController.MenuWidth;
				this.LeftViewWidth = 0.0f;
			}
		}

		/// <summary>
		/// Updates the menu presentation style.
		/// </summary>
		void UpdateMenuPresentationStyle()
		{
			var presentationStyle = (int)ExtendedMasterDetailsPageController.MenuPresentationStyle;;
			if (this.MenuDirection == ExtendedMasterDetailPage.Direction.Left)
			{
				this.LeftViewPresentationStyle = (LGSideMenuPresentationStyle)presentationStyle;
				this.RightViewPresentationStyle = default(LGSideMenuPresentationStyle);
			}
			else
			{
				this.RightViewPresentationStyle = (LGSideMenuPresentationStyle)presentationStyle;
				this.LeftViewPresentationStyle = default(LGSideMenuPresentationStyle);
			}
		}

		/// <summary>
		/// Updates the master detail containers.
		/// </summary>
		void UpdateMasterDetailContainers()
		{
			//((MasterDetailPage)Element).Master.PropertyChanged -= HandleMasterPropertyChanged;

			EmptyContainers();

			if (CurrentPlatform.GetRenderer(((MasterDetailPage)Element).Master) == null)
				CurrentPlatform.SetRenderer(((MasterDetailPage)Element).Master, CurrentPlatform.CreateRenderer(((MasterDetailPage)Element).Master));
			if (CurrentPlatform.GetRenderer(((MasterDetailPage)Element).Detail) == null)
				CurrentPlatform.SetRenderer(((MasterDetailPage)Element).Detail, CurrentPlatform.CreateRenderer(((MasterDetailPage)Element).Detail));

			var masterRenderer = CurrentPlatform.GetRenderer(((MasterDetailPage)Element).Master);
			var detailRenderer = CurrentPlatform.GetRenderer(((MasterDetailPage)Element).Detail);

			//((MasterDetailPage)Element).Master.PropertyChanged += HandleMasterPropertyChanged;

			this.MenuViewController.View.AddSubview(masterRenderer.NativeView);
			this.MenuViewController.AddChildViewController(masterRenderer.ViewController);

			this.RootViewController.View.AddSubview(detailRenderer.NativeView);
			this.RootViewController.AddChildViewController(detailRenderer.ViewController);

			SetNeedsStatusBarAppearanceUpdate();
		}

		void UpdateSwipeRange()
		{
			/* Setup the swipe range */
			var swipeRange = new LGSideMenuSwipeGestureRange();
			swipeRange.left = swipeRange.right = UIScreen.MainScreen.Bounds.Width / 2;

			/* Decide which variable we should assign the swipe range to */
			if (this.MenuDirection == ExtendedMasterDetailPage.Direction.Left)
			{
				this.LeftViewSwipeGestureRange = swipeRange;
				this.RightViewSwipeGestureRange = new LGSideMenuSwipeGestureRange();
			}
			else
			{
				this.RightViewSwipeGestureRange = swipeRange;
				this.LeftViewSwipeGestureRange = new LGSideMenuSwipeGestureRange();
			}
		}

		/// <summary>
		/// Updates the left bar button.
		/// </summary>
		//void UpdateLeftBarButton()
		//{
		//	var masterDetailPage = Element as MasterDetailPage;
		//	if (!(masterDetailPage?.Detail is NavigationPage))
		//		return;

		//	var detailRenderer = CurrentPlatform.GetRenderer(masterDetailPage.Detail) as UINavigationController;
		//	UIViewController firstPage = detailRenderer?.ViewControllers.FirstOrDefault();

		//	//if (firstPage != null)
		//	//	NavigationRenderer.SetMasterLeftBarButton(firstPage, masterDetailPage);
		//}

		void LayoutChildren()
		{
			var frame = Element.Bounds.ToRectangleF();
			var masterFrame = frame;
			masterFrame.Width = ExtendedMasterDetailsPageController.MenuWidth; //(int)(Math.Min(masterFrame.Width, masterFrame.Height) * 0.8);

			this.RootViewController.View.Frame = frame;

			MasterDetailPageController.DetailBounds = new Rectangle(0, 0, frame.Width, frame.Height);
			MasterDetailPageController.MasterBounds = new Rectangle(0, 0, masterFrame.Width, masterFrame.Height);
		}

		#endregion

		#region ChildViewController

		class ChildViewController : UIViewController
		{
			public override void ViewDidLayoutSubviews()
			{
				foreach (var vc in ChildViewControllers)
					vc.View.Frame = View.Bounds;
			}
		}

		#endregion
	}
}
