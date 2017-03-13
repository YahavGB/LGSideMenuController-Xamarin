# LGSideMenuController-Xamarin
This repository provides a Xamarin iOS and Xamarin Forms port of the iOS control [LGSideMenuController](https://github.com/Friend-LGA/LGSideMenuController). 
LGSideMenuController is also available via [CocoaPods](https://cocoapods.org).

### This Project Assemble Method
LGSideMenuController was written in Objective-C (and got Swift port too). Therefore, to use it with Xamarin.iOS and Xamarin.Forms we must port it using bindable library.
To make the port, I've relied on an open-source project named [objc-automatic](https://github.com/alexsorokoletov/objc-automatic/tree/master/pods), which automates the creation of portable class library and defining the API frintpoots. It's worth noting that objc-atuomatic make use of Xamarin's [Objective Sharpie](https://developer.xamarin.com/guides/cross-platform/macios/binding/objective-sharpie/) (t

### Getting Started

#### Using the baked NuGet package

The project already contains compiled NuGet packages (compiled against iphoneos10.2 SDK), so its super easy to integrate it.

To get started, just clone this project and add the NuGet repository to Xamarin Studio, or Visual Studio, repositories list. The repository is stored at {project_root}/bindings/packages-good (a typical structure of objc-automatic).
 - **optional**: If you're using Xamarin Forms, see the section below.

#### Compiling the project

In case you wish to compile the project yourself, you may do so by cloning this project and compiling the bindable Xamarin project using objc-automatic. The following commands shall be used:

    $ cd {project_root}/bindings
    $  sudo bash ./LGSideMenuController.build.sh

#### Compiling against different iOS SDK

You may compile LGSideMenuController against different SDK simply by cloning objc-automatic. After objc-automatic creates the necessary  binaries, switch the ApiDefinitions.cs and StructsAndEnums.cs files with those in this project.

See the example below:

    $ git clone https://github.com/alexsorokoletov/objc-automatic
    $ cd objc-automatic
    $ sh bind.sh POD=LGSideMenuController

The above portion of commands will generate you the required libararies, as well as Objective Sharpie-resolved ApiDefinitions.cs and StructsAndEnums.cs files. At this point, you should replace these files with those included in this project.
After you switch the files, you may compile the project as usual:

    $ cd bindings
    $ sudo bash ./LGSideMenuController.build.sh

### Xamarin Forms

In case you're interested in integrating this project in Xamarin.Forms projects - I've got you covered as well :).
This project contains a minimal implementation of a custom `MasterDetailPage` which make use of `LGSideMenuController` in iOS, where it uses the default `MasterDetailPage` in Android.

Although this implementation **does not** inherit `PhoneMasterDetailPageRenderer`, I did used it as a reference - so you may find yourself very familiar with this implementation.

#### Include the necessary files

 - Add a reference to the bindable project to your iOS project (e.g. App.iOS). The project should be called DT.Xamarin.LGSideMenuController.
 - Add the following files to your solution:
	 - Add **XamarinForms/Shared/ExtendedMasterDetailPage.cs** to your shared project.
	 - Add **XamarinForms/iOS/Platform/Renderers/ExtendedMasterDetailPageRenderer.cs** to your iOS project.

You're good to go. 

#### Start using LGSideMenuController in Xamarin Forms

In your `MasterDetailPage`, instead of using the  standard control, you should use **Extended**MasterDetailPage, which is a custom control.

Here is an example XAML:

    <?xml version="1.0" encoding="UTF-8"?>
    <controls:ExtendedMasterDetailPage xmlns="http://xamarin.com/schemas/2014/forms"
		xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
		x:Class="App.Pages.MasterPage"
		xmlns:controls="clr-namespace:Quartz.Forms.Controls;assembly=App"
		xmlns:pages="clr-namespace:App.Pages;assembly=Pages"
		Style="{StaticResource DefaultPage}"
		MenuWidth="250"
		MenuBackgroundImage="{markup:ResolvePath menu.background}"
		MenuPresentationStyle="ScaleFromBig">
	<MasterDetailPage.Master>
		<pages:MainMenuPage x:Name="Menu" />
	</MasterDetailPage.Master>
	<MasterDetailPage.Detail>
		<NavigationPage Style="{StaticResource MainNavigator}">
			<x:Arguments>
				<pages:HomePage />
			</x:Arguments>
		</NavigationPage>
	</MasterDetailPage.Detail>
    </controls:ExtendedMasterDetailPage>

Please note the extra attributes `MenuWidth`, `MenuPresentationStyle` and `MenuBackgroundImages`. These are custom attributes that was declared in `ExtendedMasterDetailPage` and are being used only by `LGSideMenuController`.

By replacing Xamarin default control with this custom control, you've finished to integrate `LGSideViewController` in your project.

#### What should I do if I want to change X?

Well,  You may edit `ExtendedMasterDetailPageRenderer` as you wish and get access to the full API of `LGSideViewController`. This file is not different from any other Xamarin renderer. 

### License

This project is released under the Apache 2.0 license.
Please review the licenses of Xamarin iOS, Xamarin Forms, Objective Sharpie, LGSideMenuController and objc-automatic as well.
