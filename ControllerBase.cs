using System;
using MonoTouch.Dialog;
using MonoTouch.UIKit;

namespace BunknotesApp
{
	public class ControllerBase: DialogViewController
	{
 		private IValidator _validator;
		protected RestManager _restManager = new RestManager();
		
		public virtual bool NavBarHidden {get;set;}
		
		public UINavigationController Navigation {
			get { return ((AppDelegate)UIApplication.SharedApplication.Delegate).navigation; }
			private set{}
		}
		public ControllerBase():base(null, pushing:true){
			_validator = new Validator();
			Autorotate = true;
		}
		
		public ControllerBase (RootElement root):base(root)
		{
			Autorotate = true;	
		}
	
		public ControllerBase (RootElement root, bool pushing):base(root, pushing)
		{
			Autorotate = true;	
		}

		public override void LoadView ()
		{
			base.LoadView ();
			TableView.BackgroundColor = UIColor.Clear;
			ParentViewController.View.BackgroundColor = UIColor.FromPatternImage (UIImage.FromBundle ("Images/bunk1background.jpg"));
		}
		
		protected void PushController (ControllerBase controller)
		{
			Navigation.PushViewController (controller, true);
		}
		
		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);
			NavigationController.SetNavigationBarHidden (NavBarHidden, animated);
		}
	
		public virtual bool Validate(){
			return _validator.Validate(this);
		}
	}
}

