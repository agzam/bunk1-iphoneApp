using System;
using System.Collections.Generic;
using System.Linq;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MonoTouch.Dialog;
using System.Drawing;
using BunknotesApp.Helpers;

namespace BunknotesApp
{
	public class Application
	{
		static void Main (string[] args)
		{
			UIApplication.Main (args);
		}
	}
		
	public partial class AppDelegate:UIApplicationDelegate
	{	
		public override bool FinishedLaunching (UIApplication app, NSDictionary options)
		{
			window.AddSubview (navigation.View);
			
			navigation.PushViewController (new LoginScreen(), true);
			
			window.MakeKeyAndVisible ();
			
			return true;
			
		}
	}
}
