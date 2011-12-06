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
		private EntryElement password 
			= new EntryElement ("Password", "Your password", "", isPassword:true)
					{KeyboardType = UIKeyboardType.Default, ReturnKeyType= UIReturnKeyType.Go};
		
		public LoginScreen ():base(false)
		{
			userName = new EntryElement ("Username", "Your username", ConfigurationWorker.LastUsedUsername);
			userName.KeyboardType = UIKeyboardType.Default;
			userName.AutocorrectionType = UITextAutocorrectionType.No;
			userName.AutocapitalizationType = UITextAutocapitalizationType.None;
			
			password.AutocorrectionType = UITextAutocorrectionType.No;
			password.AutocapitalizationType = UITextAutocapitalizationType.None;
			password.ShouldReturn += delegate{
				DoLogin();
				return true;
			};
			
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
				ConfigurationWorker.ReInitValues();
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

