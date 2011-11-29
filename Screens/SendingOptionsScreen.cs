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
		
		[Required(ErrorMessage = "You forgot the sender's name")]
		EntryElement sendFromElement = new EntryElement ("Send from:", "", ConfigurationWorker.SentFrom);
		string _currentCamperStr;
		string _currentCabinStr;
		
		[Required(ErrorMessage = "Choose camper")]
		StyledStringElement _chooseCamper; 
		[Required(ErrorMessage = "Choose cabin")]
		StyledStringElement _chooseCabin;
		

		public override void ViewWillAppear (bool animated)
		{
			var logAsStr = 
				String.Format("{0} {1}",string.IsNullOrWhiteSpace(ConfigurationWorker.LastMessage) ? "Welcome" : "Welcome back", ConfigurationWorker.LastUsedUsername);
			loggedAsElement = new StyledMultilineElement(logAsStr);
			loggedAsElement.Font = UIFont.SystemFontOfSize(14);
			loggedAsElement.TextColor = UIColor.DarkGray;
			loggedAsElement.Tapped += delegate{
				NavigationController.PopViewControllerAnimated(animated:true);
			};
			
			_currentCamperStr = ConfigurationWorker.LastCamper.ToString();
			_currentCabinStr = ConfigurationWorker.LastCabin.ToString();
			
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
			ConfigurationWorker.SentFrom = sendFromElement.Value;
			NavigationController.PushViewController (new ComposeMessageScreen(), animated:true);
		}
		
		public override void DidRotate (UIInterfaceOrientation fromInterfaceOrientation)
		{
			base.DidRotate (fromInterfaceOrientation);
			Root = GetRoot();
		}
		
		private RootElement GetRoot ()
		{
			var lastSection = new Section(){
				new StyledStringElement("Next", ()=> ProceedNext() )
							{ Alignment = UITextAlignment.Center, BackgroundColor = ConfigurationWorker.DefaultBtnColor}
			};
			var shift = (this.InterfaceOrientation == UIInterfaceOrientation.Portrait || this.InterfaceOrientation == UIInterfaceOrientation.PortraitUpsideDown) ? 80:0;
			lastSection.HeaderView = new UIView(new RectangleF(0,0,0,shift));
			
			return new RootElement ("Options"){
						new Section (shift > 0 ? "From:":"") {
							loggedAsElement,
							sendFromElement
						},
						new Section (shift > 0 ? "To:":"") {
							_chooseCamper, 
							_chooseCabin
						},
//						new Section(){
//							new StyledStringElement("Next", ()=> ProceedNext() )
//							{ Alignment = UITextAlignment.Center, BackgroundColor = ConfigurationWorker.DefaultBtnColor}
//						},
						lastSection
				};	
		}
	}
}
