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
		public static AppDelegate CurrentApp{ get{
				return ((AppDelegate)UIApplication.SharedApplication.Delegate);
			}
		}
			
		public override bool FinishedLaunching (UIApplication app, NSDictionary options)
		{
			ConfigurationWorker.ReInitValues();
			
			window.AddSubview (navigation.View);
			
			var loginScreen = new LoginScreen(){ Autorotate = true };
			
			navigation.PushViewController (loginScreen, true);
			
			window.MakeKeyAndVisible ();
			
			return true;
		}
	}
}
