using System;
using MonoTouch.Dialog;
using MonoTouch.UIKit;
using MonoTouch.Foundation;
using BunknotesApp.Helpers;
using Bunk1DataAnnotations;

namespace BunknotesApp
{
	public class SendingOptionsScreen : ControllerBase
	{
		private StringElement loggedAsElement = new MultilineElement("Logged as","dmg1");
		private EntryElement sendFromElement = new EntryElement ("Send from", "Me", "");
		private string _currentCamperStr;
		private string _currentCabinStr;
		
		[Required(ErrorMessage = "Choose camper")]
		private StyledStringElement _chooseCamper; 
		[Required(ErrorMessage = "Choose cabin")]
		private StyledStringElement _chooseCabin;
		
		private ConfigurationWorker config = new ConfigurationWorker();

		public override void ViewWillAppear (bool animated)
		{
			_currentCamperStr = config.LastCamper;
			_currentCabinStr = config.LastCabin;
			
			_chooseCamper = new StyledStringElement ("Camper",_currentCamperStr);
			_chooseCamper.Accessory = UITableViewCellAccessory.DisclosureIndicator;
			_chooseCamper.Tapped += () => NavigationController.PushViewController(new ChooseCamperScreen (), true);
			
			_chooseCabin = new StyledStringElement ("Bunk",_currentCabinStr);
			_chooseCabin.Accessory = UITableViewCellAccessory.DisclosureIndicator;
			_chooseCabin.Tapped += () => NavigationController.PushViewController (new ChooseCabinScreen (), true);
			Root = GetRoot ();
			base.ViewWillAppear (animated);
		}
		private void ProceedNext(){
			if (!Validate()) return;	
		}
		
		private RootElement GetRoot ()
		{
					
			return new RootElement ("Options"){
						new Section ("From:") {
							loggedAsElement,
							sendFromElement
						},
						new Section ("To:") {
							_chooseCamper, 
							_chooseCabin
						},
						new Section(""){
							new StyledStringElement("Next", ()=> ProceedNext() )
							{ Alignment = UITextAlignment.Center, BackgroundColor = UIColor.LightGray}
						}
				};	
		}
	}
}
