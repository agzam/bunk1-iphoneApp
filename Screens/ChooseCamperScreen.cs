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
		private List<string> _campersList = new List<string> ();
		private ConfigurationWorker config = new ConfigurationWorker();

		public ChooseCamperScreen ()
		{
			Style = UITableViewStyle.Plain;
			EnableSearch = true;
			SearchPlaceholder = "Find camper";
			AutoHideSearch = true;
			
			_restManager.RequestCompleted += (sender, e) => {
				var campers = ((e as CampersRequestArgs).campers).Select (x => x.FirstName + " " + x.LastName).ToList ();
				_campersList.AddRange (campers);
				Root = GetRoot ();
			};
			_restManager.GetCampers ();	
		}
		
		public override void Selected (NSIndexPath indexPath)
		{
			base.Selected (indexPath);
			config.LastCamper = _campersList[indexPath.Row];
			NavigationController.PopViewControllerAnimated(true);
		}
		
		private RootElement GetRoot ()
		{
			return new RootElement ("Select camper"){
				new Section (){
						from c in _campersList select (Element)new StyledStringElement (c)
					}
				};	
		}
	}
}
