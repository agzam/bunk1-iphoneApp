using System;
using MonoTouch.Dialog;
using MonoTouch.UIKit;
using MonoTouch.Foundation;
using Bunk1.Helpers;
using System.Linq;
using System.Collections.Generic;
using Bunk1DataAnnotations;
using System.Drawing;

namespace BunknotesApp
{
	public class AddNewCamperScreen : ControllerBase
	{
		[Required(ErrorMessage = "First name is required")]
		private EntryElement _firstName = new EntryElement (" ", "First Name", "");
		[Required(ErrorMessage = "Last name is required")]
		private EntryElement _lastName = new EntryElement (" ", "Last Name", "");
		private Section imageSection = new Section (){ new SimpleImageElement ("Images/camperIcon.png") };
		
		public override void ViewWillAppear (bool animated)
		{
			Root = GetRoot ();
			base.ViewWillAppear (animated);
		}
		
		private void DoSave ()
		{
			if (!Validate ())
				return;
			
			ConfigurationWorker.LastCamper = new Camper{FirstName = _firstName.Value, LastName = _lastName.Value};
			var so = NavigationController.ViewControllers.FirstOrDefault (x => x.GetType () == typeof(SendingOptionsScreen));
			NavigationController.PopToViewController (so, animated:true);
		}
		
		public override void WillAnimateRotation (UIInterfaceOrientation toInterfaceOrientation, double duration)
		{
			base.WillAnimateRotation (toInterfaceOrientation, duration);
			if (toInterfaceOrientation == UIInterfaceOrientation.Portrait || toInterfaceOrientation == UIInterfaceOrientation.PortraitUpsideDown) {
				imageSection = new Section (){ new SimpleImageElement ("Images/camperIcon.png") };
			} else {
				imageSection = null;
			}
			Root = GetRoot ();
		}
		
		private RootElement GetRoot ()
		{
			return new RootElement ("New camper"){
					new Section (){
						_firstName,
						_lastName
					},
					imageSection,
					new Section () {
							new StyledStringElement ("Save", DoSave) 
							{ Alignment = UITextAlignment.Center, BackgroundColor =  ConfigurationWorker.DefaultBtnColor }
					}
			};	
		}
	}
	
	public class SimpleImageElement : StringElement, IElementSizing
	{
		private string imageUrl;

		public SimpleImageElement (string imageUrl):base("")
		{
			this.imageUrl = imageUrl;
		}
		
		public override UITableViewCell GetCell (UITableView tv)
		{
			var cell = base.GetCell (tv);
			var img = UIImage.FromBundle (imageUrl);
		
			var view = new UIImageView (img.StretchableImage (90, 98));
			
			cell.BackgroundView = view;
			return cell;
		}

		#region IElementSizing implementation
		public float GetHeight (UITableView tableView, NSIndexPath indexPath)
		{
			return 200;
		}
		#endregion
	}
}
