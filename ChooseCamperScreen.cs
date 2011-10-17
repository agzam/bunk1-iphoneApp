using System;
using MonoTouch.Dialog;
using MonoTouch.UIKit;
using MonoTouch.Foundation;
using BunknotesApp.Helpers;

namespace BunknotesApp
{
	public class ChooseCamperScreen : ControllerBase
	{
		public ChooseCamperScreen ()
		{
			Style = UITableViewStyle.Plain;
			EnableSearch = true;
			SearchPlaceholder = "Find camper";
			AutoHideSearch = true;
			Root = GetRoot ();	
		}
		public override void Select ()
		{
			base.Select ();
		}
		
		private RootElement GetRoot ()
		{
			var strElement = new StringElement ("Emilio", "Emilio");
			strElement.Tapped += () => {
				MessageBox.Show("");	
			};
			return new RootElement ("Select camper"){
				new Section (){
						strElement
					}
				};	
		}
	}
}
