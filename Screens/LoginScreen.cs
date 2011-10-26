using System;
using MonoTouch.Dialog;
using MonoTouch.UIKit;
using MonoTouch.Foundation;
using Bunk1DataAnnotations;
using BunknotesApp.Helpers;
using System.Linq;

namespace BunknotesApp
{
	public class LoginScreen : ControllerBase
	{
		[Required(ErrorMessage = "Username is required")]
		private EntryElement userName;
		[Required(ErrorMessage = "Password is required")]
		private EntryElement password = new EntryElement ("Password", "Your password", "", isPassword:true);
		private BooleanElement saveCredentials = new BooleanElement ("Save your credentials", true);
		private ConfigurationWorker config = new ConfigurationWorker ();
		
		public LoginScreen ():base(false)
		{
			userName = new EntryElement ("Username", "Your user name", config.LastUsedUsername);
			Root = GetRoot ();
			NavBarHidden = true;
		}
		
		public override void ViewDidAppear (bool animated)
		{
			base.ViewDidAppear (animated);
		}
		
		public override void ViewWillAppear (bool animated)
		{
			NavigationController.SetNavigationBarHidden(hidden:true,animated:false);
			base.ViewWillAppear (animated);
		}
		
		private void DoLogin ()
		{
			if (!Validate()) return;
			
			_restManager.Authenticate(userName.Value, password.Value, ()=>{
				if (saveCredentials.Value) config.SaveCurrentLogin(userName.Value, password.Value);	
				config.LastUsedUsername = userName.Value;
				NavigationController.PushViewController (new SendingOptionsScreen (), false);
			});

			if (!saveCredentials.Value) return;
			
			config.SaveCurrentLogin(userName.Value, password.Value);
			
			
			//var dv = new MainController (new CamperSelectScreen (), pushing:true);
			//((AppDelegate)UIApplication.SharedApplication.Delegate).navigation.PushViewController (dv, animated: true);
			//(AppDelegate)UIApplication.SharedApplication.Delegate).navigation.NavigationBarHidden = false;
			
		}

		public RootElement GetRoot ()
		{
			return new RootElement ("Login") {
				new Section ("Login") {
							userName,
					 		password
						},
						new Section () {
							saveCredentials,
						},
						new Section () {
							new StyledStringElement ("Login", () => DoLogin ()) 
							{ Alignment = UITextAlignment.Center, BackgroundColor =  ConfigurationWorker.DefaultBtnColor }
						},
			};
		}
		
	}
}

