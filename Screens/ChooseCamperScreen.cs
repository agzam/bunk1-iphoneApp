using System;
using MonoTouch.Dialog;
using MonoTouch.UIKit;
using MonoTouch.Foundation;
using Bunk1.Helpers;
using System.Linq;
using System.Collections.Generic;

namespace BunknotesApp
{
	public class ChooseCamperScreen : ControllerBase
	{
		private List<Camper> _campersList = new List<Camper> ();

		public ChooseCamperScreen ()
		{
			Style = UITableViewStyle.Plain;
			EnableSearch = true;
			SearchPlaceholder = "Find camper";
			AutoHideSearch = true;
			
			EventHandler addBtnClickHandler = (s, a) => NavigationController.PushViewController (new AddNewCamperScreen (), animated:true);
			var addBtn = new UIBarButtonItem (UIBarButtonSystemItem.Add, addBtnClickHandler);
			NavigationItem.Title = "Select camper";
			NavigationItem.SetRightBarButtonItem (addBtn, animated:true);
		}
		
		public override void ViewDidAppear (bool animated)
		{
			base.ViewDidAppear (animated);
			
			_restManager.GetCampers (campers => {
				if (campers == null) {
					SessionExpired();
					return;
				}
				_campersList.AddRange (campers);
				Root = GetRoot ();
			});	
		}
		
		public override void Selected (NSIndexPath indexPath)
		{
			base.Selected (indexPath);
			var selected = _campersList [indexPath.Row];
			
			ConfigurationWorker.LastCamper = selected;
			var cabin = _restManager.GetCabinById (selected.CabinId);
			ConfigurationWorker.LastCabin = cabin;
			NavigationController.PopViewControllerAnimated (animated:true);	
		}
		
		private RootElement GetRoot ()
		{
			return new RootElement ("Select camper"){
				new Section (){
						from c in _campersList select (Element)new StyledStringElement (c.ToString ())
					}
				};	
		}
	}
}
