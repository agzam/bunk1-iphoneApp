using System;
using MonoTouch.Dialog;
using MonoTouch.UIKit;
using MonoTouch.Foundation;
using BunknotesApp.Helpers;
using System.Linq;
using System.Collections.Generic;

namespace BunknotesApp
{
	public class ChooseCamperScreen : ControllerBase
	{
		private List<Camper> _campersList = new List<Camper> ();
		private ConfigurationWorker config = new ConfigurationWorker ();

		public ChooseCamperScreen ()
		{
			Style = UITableViewStyle.Plain;
			EnableSearch = true;
			SearchPlaceholder = "Find camper";
			AutoHideSearch = true;
			
			EventHandler addBtnClickHandler = (s, a) => NavigationController.PushViewController (new AddNewCamperScreen (), animated:true);
			var addBtn = new UIBarButtonItem (UIBarButtonSystemItem.Add, addBtnClickHandler);
			NavigationItem.SetRightBarButtonItem (addBtn, animated:true);
			
			_restManager.RequestCompleted += (sender, e) => {
				var campers = ((e as CampersRequestArgs).campers).ToList ();
				_campersList.AddRange (campers);
				Root = GetRoot ();
			};
			_restManager.GetCampers ();	
		}
		
		public override void Selected (NSIndexPath indexPath)
		{
			base.Selected (indexPath);
			var selected = _campersList [indexPath.Row];
			
			config.LastCamper = selected;
			_restManager.RequestCompleted += (sender, e) => {
				var cabin = ((e as CabinsRequestArgs).cabins).FirstOrDefault(c=> c.Id == selected.CabinId);
				if (cabin != null) {
					config.LastCabin = cabin;
				}
				NavigationController.PopViewControllerAnimated (true);	
			};
			_restManager.GetCabins ();
		}
		
		private RootElement GetRoot ()
		{
			return new RootElement ("Select camper"){
				new Section (){
						from c in _campersList select (Element)new StyledStringElement (c.ToString())
					}
				};	
		}
	}
}
