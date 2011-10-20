using System;
using MonoTouch.Dialog;
using MonoTouch.UIKit;
using System.Linq;

namespace BunknotesApp
{
	public class ControllerBase: DialogViewController
	{
 		private IValidator _validator;
		protected RestManager _restManager = new RestManager();
		
		public virtual bool NavBarHidden {get;set;}
		
		public ControllerBase():this(pushing: true){}
		
		public ControllerBase(bool pushing):base(null, pushing){
			_validator = new Validator();
			Autorotate = true;
		}
		
		public override void LoadView ()
		{
			base.LoadView ();
			TableView.BackgroundColor = UIColor.Clear;
			ParentViewController.View.BackgroundColor = UIColor.FromPatternImage (UIImage.FromBundle ("Images/bunk1background.jpg"));
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

