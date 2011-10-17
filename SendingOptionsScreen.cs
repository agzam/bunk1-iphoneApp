using System;
using MonoTouch.Dialog;
using MonoTouch.UIKit;
using MonoTouch.Foundation;
using BunknotesApp.Helpers;

namespace BunknotesApp
{
	public class SendingOptionsScreen : ControllerBase
	{
		private StringElement loggedAsElement = new StringElement ("Logged as", "dmg1");
		private EntryElement sendFromElement = new EntryElement ("Send from", "Me", "");
		private string _currentCamperStr;
		private string _currentCabinStr;
		
		private ConfigurationWorker config = new ConfigurationWorker();

		public SendingOptionsScreen ()
		{
			_currentCamperStr = config.LastCamper;
			_currentCabinStr = config.LastCabin;
			Root = GetRoot ();	
		}
		
		private void ChooseCamper ()
		{
			_restManager.RequestCompleted += (sender, e) => {
				var campers = (e as CampersRequestArgs).campers;
			};
			_restManager.GetCampers();
			
			
			PushController (new ChooseCamperScreen ());
		}
		
		private void ChooseCabin ()
		{
			PushController (new ChooseCamperScreen ());
		}
		
		private RootElement GetRoot ()
		{
			return new RootElement ("Options"){
						new Section ("From:") {
							loggedAsElement,
							sendFromElement
						},
						new Section ("To:") {
							new StyledStringElement (_currentCamperStr, () => ChooseCamper ()) { Accessory = UITableViewCellAccessory.DisclosureIndicator },
							new StyledStringElement (_currentCabinStr, () => ChooseCabin ())   { Accessory = UITableViewCellAccessory.DisclosureIndicator },
						},
				};	
		}
	}
}
