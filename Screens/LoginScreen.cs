using System;
using MonoTouch.Dialog;
using MonoTouch.UIKit;
using MonoTouch.Foundation;
using Bunk1DataAnnotations;
using Bunk1.Helpers;
using System.Linq;

namespace BunknotesApp
{
	public class LoginScreen : ControllerBase
	{
		[Required(ErrorMessage = "Username is required")]
		private EntryElement userName;
		[Required(ErrorMessage = "Password is required")]
		private EntryElement password = new EntryElement ("Password", "Your password", "", isPassword:true);
		
		public LoginScreen ():base(false)
		{
			userName = new EntryElement ("Username", "Your username", ConfigurationWorker.LastUsedUsername);
			NavBarHidden = true;
			Root = GetRoot ();
		}
		
		public override void ViewDidAppear (bool animated)
		{
			base.ViewDidAppear (animated);
			Reachability.ConnectionAvailible();
		}
		
		public override void ViewWillAppear (bool animated)
		{
			NavigationController.SetNavigationBarHidden (hidden:true, animated:false);
			base.ViewWillAppear (animated);
		}
		
		private void DoLogin ()
		{
			if (!Validate ())
				return;
			
			_restManager.Authenticate (userName.Value, password.Value, () => {
				ConfigurationWorker.SaveCurrentLogin (userName.Value, password.Value);	
				ConfigurationWorker.LastUsedUsername = userName.Value;
				ConfigurationWorker.LastUsedPassword = password.Value;
				NavigationController.PushViewController (new SendingOptionsScreen (), animated:true);
			});
		}

		public RootElement GetRoot ()
		{
			return new RootElement ("Login") {
				new Section ("Your Bunk1 account") {
							userName,
					 		password
						},
						new Section () {
							new StyledStringElement ("Login", () => DoLogin ()) 
							{ Alignment = UITextAlignment.Center, BackgroundColor =  ConfigurationWorker.DefaultBtnColor }
						},
			};
		}
		
	}
}

