using System;
using System.Collections.Generic;
using System.Linq;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MonoTouch.Dialog;
using System.Drawing;
using Bunk1.Helpers;

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
		public event EventHandler AppDidEnterBackground = delegate {};
		
		public override void DidEnterBackground (UIApplication application)
		{
			AppDidEnterBackground.Invoke(this, EventArgs.Empty);
		}
		
		public static AppDelegate CurrentApp{ get{
				return ((AppDelegate)UIApplication.SharedApplication.Delegate);
			}
		}
			
		public override bool FinishedLaunching (UIApplication app, NSDictionary options)
		{
			ConfigurationWorker.ReInitValues();
			
			window.AddSubview (navigation.View);
			
			//var loginScreen = new ControllerBase { Autorotate = true };
			var loginScreen = new LoginScreen(){ Autorotate = true };
			//var loginScreen = new ComposeMessageScreen{ Autorotate = true };
			
			navigation.PushViewController (loginScreen, true);
			
			window.MakeKeyAndVisible ();
			
			return true;
		}
	}
}
