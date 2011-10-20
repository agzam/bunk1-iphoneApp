using System;
using MonoTouch.Dialog;
using MonoTouch.UIKit;
using MonoTouch.Foundation;
using BunknotesApp.Helpers;
using System.Linq;
using System.Collections.Generic;

namespace BunknotesApp
{
	public class ChooseCabinScreen : ControllerBase
	{
		private List<Cabin> _cabinsList = new List<Cabin> ();
		private ConfigurationWorker config = new ConfigurationWorker ();

		public ChooseCabinScreen ()
		{
			Style = UITableViewStyle.Plain;
			EnableSearch = true;
			SearchPlaceholder = "Find cabin";
			AutoHideSearch = true;
			
			_restManager.RequestCompleted += (sender, e) => {
				_cabinsList = ((e as CabinsRequestArgs).cabins).ToList ();
				Root = GetRoot ();
			};
			_restManager.GetCabins ();	
		}
		
		public override void Selected (NSIndexPath indexPath)
		{
			base.Selected (indexPath);
			config.LastCabin = _cabinsList [indexPath.Row];
			NavigationController.PopViewControllerAnimated(true);
		}
		
		private RootElement GetRoot ()
		{
			return new RootElement ("Select camper"){
				new Section (){
						from c in _cabinsList select (Element)new StyledStringElement (c.ToString())
					}
				};	
		}
	}
}
