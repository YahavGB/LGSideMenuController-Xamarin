using System;
using Xamarin.Forms;

namespace Quartz.Forms.Controls
{
	public class ExtendedMasterDetailPage : MasterDetailPage
	{
		#region Enums

		/// <summary>
		/// Enumerator which describes the available menu directions.
		/// </summary>
		public enum Direction
		{
			Left,
			Right
		};

		/// <summary>
		/// Enum which describes the available menu presentation styles.
		/// </summary>
		public enum PresentationStyle
		{
			SlideAbove,
			SlideBelow,
			ScaleFromBig,
			ScaleFromLittle
		}

		#endregion

		#region Properties

		public static BindableProperty MenuDirectionProperty
			= BindableProperty.Create(nameof(MenuDirection), typeof(Direction), typeof(ExtendedMasterDetailPage), default(Direction));

		public static BindableProperty MenuWidthProperty
			= BindableProperty.Create(nameof(MenuWidth), typeof(float), typeof(ExtendedMasterDetailPage), 250.0f);

		public static BindableProperty SwipeRangeProperty
			= BindableProperty.Create(nameof(SwipeRange), typeof(float), typeof(ExtendedMasterDetailPage), 100.0f);

		public static BindableProperty MenuPresentationStyleProperty
			= BindableProperty.Create(nameof(MenuPresentationStyle), typeof(PresentationStyle), typeof(ExtendedMasterDetailPage), default(PresentationStyle));

		public static BindableProperty MenuBackgroundColorProperty
			= BindableProperty.Create(nameof(MenuBackgroundImage), typeof(Color), typeof(ExtendedMasterDetailPage), default(Color));

		public static BindableProperty MenuBackgroundImageProperty
			= BindableProperty.Create(nameof(MenuBackgroundImage), typeof(string), typeof(ExtendedMasterDetailPage), string.Empty);

		/// <summary>
		/// Gets or sets the menu direction.
		/// </summary>
		/// <value>The menu direction.</value>
		public Direction MenuDirection
		{
			get { return (Direction)GetValue(MenuDirectionProperty); }
			set { SetValue(MenuDirectionProperty, value); }
		}

		/// <summary>
		/// Gets or sets the width of the menu.
		/// </summary>
		/// <value>The width of the menu.</value>
		public float MenuWidth
		{
			get { return (float)GetValue(MenuWidthProperty); }
			set { SetValue(MenuWidthProperty, value); }
		}

		/// <summary>
		/// Gets or sets the swipe range.
		/// </summary>
		/// <value>The swipe range.</value>
		public float SwipeRange
		{
			get { return (float)GetValue(SwipeRangeProperty); }
			set { SetValue(SwipeRangeProperty, value); }
		}

		/// <summary>
		/// Gets or sets the menu presentation style.
		/// </summary>
		/// <value>The menu presentation style.</value>
		public PresentationStyle MenuPresentationStyle
		{
			get { return (PresentationStyle)GetValue(MenuPresentationStyleProperty); }
			set { SetValue(MenuPresentationStyleProperty, value); }
		}

		/// <summary>
		/// Gets or sets the color of the menu background.
		/// </summary>
		/// <value>The color of the menu background.</value>
		public Color MenuBackgroundColor
		{
			get { return (Color)GetValue(MenuBackgroundColorProperty); }
			set { SetValue(MenuBackgroundColorProperty, value); }
		}

		/// <summary>
		/// Gets or sets the menu background image.
		/// </summary>
		/// <value>The menu background image.</value>
		public string MenuBackgroundImage
		{
			get { return (string)GetValue(MenuBackgroundImageProperty); }
			set { SetValue(MenuBackgroundImageProperty, value); }
		}

		#endregion
	}
}
