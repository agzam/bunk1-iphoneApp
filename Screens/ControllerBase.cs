using System;
using MonoTouch.Dialog;
using MonoTouch.UIKit;
using System.Linq;
using Bunk1.Helpers;

namespace BunknotesApp
{
	public class ControllerBase: DialogViewController
	{
		private IValidator _validator;
		protected RestManager _restManager = new RestManager ();
		
		public virtual bool NavBarHidden { get; set; }
		
		public ControllerBase ():this(pushing: true)
		{
		}
		
		public ControllerBase (bool pushing):base(null, pushing)
		{
			_validator = new Validator ();
			Autorotate = true;
		}
		
		public override void LoadView ()
		{
			base.LoadView ();
			TableView.BackgroundColor = UIColor.Clear;
			NavigationController.NavigationBar.TintColor = ConfigurationWorker.DefaultNavbarTint;
			ParentViewController.View.BackgroundColor = UIColor.FromPatternImage (UIImage.FromBundle ("Images/bunk1background.jpg"));
		}
		
		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);
			NavigationController.SetNavigationBarHidden (NavBarHidden, animated);
		}
	
		public virtual bool Validate ()
		{
			return _validator.Validate (this);
		}
		
		protected void SessionExpired ()
		{
			var loginScreen =
						NavigationController.ViewControllers.FirstOrDefault (x => x.GetType () == typeof(LoginScreen));
			if (loginScreen != null)
				NavigationController.PopToViewController (loginScreen, animated:true);
			else
				NavigationController.PushViewController (new LoginScreen (), animated:true);
			
			MessageBox.Show ("Session has expired!", 2000);
		}
	}
}

