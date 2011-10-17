using System;
using MonoTouch.Dialog;
using MonoTouch.UIKit;
using MonoTouch.Foundation;
using Bunk1DataAnnotations;
using BunknotesApp.Helpers;

namespace BunknotesApp
{
	public class LoginScreen : ControllerBase
	{
		[Required(ErrorMessage = "Username is required")]
		private EntryElement userName;
		
		[Required(ErrorMessage = "Password is required")]
		private EntryElement password = new EntryElement ("Password", "Your password", "", isPassword:true);
		private BooleanElement saveCredentials = new BooleanElement ("Save your credentials", true);
		private ConfigurationWorker config = new ConfigurationWorker();
		
		public LoginScreen ()
		{
			userName = new EntryElement ("Username", "Your user name", config.LastUsedUsername);
			Root = GetRoot ();
			NavBarHidden = true;
		}
		
		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);
		}
		
		private void DoLogin ()
		{
			if (!Validate()) return;
			
			_restManager.RequestCompleted += (sender, e) => {
				if (saveCredentials.Value) config.SaveCurrentLogin(userName.Value, password.Value);	
				config.LastUsedUsername = userName.Value;
				PushController (new SendingOptionsScreen ());
			};
			_restManager.Authenticate(userName.Value, password.Value);
			
			
//			if (!saveCredentials.Value) return;
//			
//			config.SaveCurrentLogin(userName.Value, password.Value);
			
			
			//var dv = new MainController (new CamperSelectScreen (), pushing:true);
			//((AppDelegate)UIApplication.SharedApplication.Delegate).navigation.PushViewController (dv, animated: true);
			//(AppDelegate)UIApplication.SharedApplication.Delegate).navigation.NavigationBarHidden = false;
			

//			
//			NSUserDefaults.StandardUserDefaults.SetString ("abrvalg", "username");
//			NSUserDefaults.StandardUserDefaults.Init ();
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
							new StyledStringElement ("Login", () => DoLogin ()) { Accessory = UITableViewCellAccessory.DisclosureIndicator }
						},
			};
		}
		
	}
}

