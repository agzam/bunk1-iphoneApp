using System;
using MonoTouch.Dialog;
using MonoTouch.UIKit;
using MonoTouch.Foundation;
using Bunk1.Helpers;
using System.Linq;
using System.Collections.Generic;

namespace BunknotesApp
{
	public class ChooseCabinScreen : ControllerBase
	{
		private List<Cabin> _cabinsList = new List<Cabin> ();

		public ChooseCabinScreen ()
		{
			Style = UITableViewStyle.Plain;
//			EnableSearch = true;
//			SearchPlaceholder = "Find bunk";
//			AutoHideSearch = true;
		}
		
		public override void ViewDidAppear (bool animated)
		{
			base.ViewDidAppear (animated);
			_restManager.GetCabins(c => {
				if (c == null) {
					SessionExpired();
					return;
				}
				_cabinsList = c;
				Root = GetRoot ();
			});
		}
		
		public override void LoadView ()
		{
			base.LoadView ();
			View.Alpha = 0.7f;
		}
		
		public override void Selected (NSIndexPath indexPath)
		{
			base.Selected (indexPath);
			ConfigurationWorker.LastCabin = _cabinsList [indexPath.Row];
			NavigationController.PopViewControllerAnimated(true);
		}
		
		private RootElement GetRoot ()
		{
			return new RootElement ("select bunk"){
				new Section (){
						from c in _cabinsList select (Element)new StyledStringElement (c.ToString())
					}
				};	
		}
	}
}
