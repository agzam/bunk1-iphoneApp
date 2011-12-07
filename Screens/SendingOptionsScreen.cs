using System;
using MonoTouch.Dialog;
using MonoTouch.UIKit;
using MonoTouch.Foundation;
using Bunk1.Helpers;
using Bunk1DataAnnotations;
using System.Drawing;

namespace BunknotesApp
{
	public class SendingOptionsScreen : ControllerBase
	{
		StyledMultilineElement loggedAsElement;
		[Required(ErrorMessage = "you forgot the sender's name")]
		EntryElement sendFromElement 
			= new EntryElement ("send from:", "", ConfigurationWorker.SentFrom){
			KeyboardType = UIKeyboardType.Default, 
			AutocorrectionType = UITextAutocorrectionType.No
		};
		string _currentCamperStr;
		string _currentCabinStr;
		[Required(ErrorMessage = "choose camper")]
		StyledStringElement _chooseCamper;
		[Required(ErrorMessage = "choose cabin")]
		StyledStringElement _chooseCabin;

		public override void ViewWillAppear (bool animated)
		{
			var logAsStr = String.Format ("welcome {0} {1}, you have {2} credits",
							string.IsNullOrWhiteSpace (ConfigurationWorker.LastMessage) ? "" : "back", 
							RestManager.AuthenticationResult.FirstName,
							RestManager.AuthenticationResult.NumberOfCredits);
			loggedAsElement = new StyledMultilineElement (logAsStr);
			loggedAsElement.Font = UIFont.SystemFontOfSize (11);
			loggedAsElement.TextColor = UIColor.DarkGray;
			loggedAsElement.Tapped += delegate{
				NavigationController.PopViewControllerAnimated (animated:true);
			};
			
			_currentCamperStr = ConfigurationWorker.LastCamper.ToString ();
			_currentCabinStr = ConfigurationWorker.LastCabin.ToString ();
			
			_chooseCamper = new StyledStringElement ("camper", _currentCamperStr);
			_chooseCamper.Accessory = UITableViewCellAccessory.DisclosureIndicator;
			_chooseCamper.Tapped += () => NavigationController.PushViewController (new ChooseCamperScreen (), true);
			
			_chooseCabin = new StyledStringElement ("bunk", _currentCabinStr);
			_chooseCabin.Accessory = UITableViewCellAccessory.DisclosureIndicator;
			_chooseCabin.Tapped += () => NavigationController.PushViewController (new ChooseCabinScreen (), true);
			Root = GetRoot ();
			
			base.ViewWillAppear (animated);
		}
		
		private void ProceedNext ()
		{
			if (!Validate ())
				return;
			ConfigurationWorker.SentFrom = sendFromElement.Value;
			NavigationController.PushViewController (new ComposeMessageScreen (), animated:true);
		}
		
		public override void DidRotate (UIInterfaceOrientation fromInterfaceOrientation)
		{
			base.DidRotate (fromInterfaceOrientation);
			Root = GetRoot ();
		}
		
		private RootElement GetRoot ()
		{
			var lastSection = new Section (){
				new StyledStringElement ("next", () => ProceedNext ())
							{ Alignment = UITextAlignment.Center, BackgroundColor = ConfigurationWorker.DefaultBtnColor}
			};
			var shift = (this.InterfaceOrientation == UIInterfaceOrientation.Portrait || this.InterfaceOrientation == UIInterfaceOrientation.PortraitUpsideDown) ? 80 : 0;
			lastSection.HeaderView = new UIView (new RectangleF (0, 0, 0, shift));
			
			var toSection = new Section (shift > 0 ? "to:" : "") {
							_chooseCamper, 
						};
			
			if (ConfigurationWorker.IsThereAtLeast1Cabin) toSection.Add(_chooseCabin);
			
			return new RootElement ("options"){
						new Section (shift > 0 ? "from:" : "") {
							loggedAsElement,
							sendFromElement
						},
						toSection,
						lastSection
				};	
		}
	}
}
