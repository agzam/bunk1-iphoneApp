using System;
using MonoTouch.Dialog;
using MonoTouch.UIKit;
using MonoTouch.Foundation;
using BunknotesApp.Helpers;
using System.Linq;
using System.Collections.Generic;

namespace BunknotesApp
{
	public class AddNewCamperScreen : ControllerBase
	{
		private List<string> _campersList = new List<string> ();
		private ConfigurationWorker config = new ConfigurationWorker ();
		
		public override void ViewWillAppear (bool animated)
		{
			Root = GetRoot ();
			base.ViewWillAppear (animated);
		}
		
		private RootElement GetRoot ()
		{
			return new RootElement ("Add new camper"){
					new Section ("First name"){
						new EntryElement ("First Name", "Camper's First Name", "")
					},
					new Section ("Last name"){
						new EntryElement ("Last Name", "Enter Camper's Last Name", "")
					},
					new Section () {
							new StyledStringElement ("Save", () => {}) 
							{ Alignment = UITextAlignment.Center, BackgroundColor =  ConfigurationWorker.DefaultBtnColor }
					}
			};	
		}
	}
}
