using System;
using MonoTouch.Dialog;
using MonoTouch.UIKit;
using MonoTouch.Foundation;
using Bunk1.Helpers;
using System.Linq;
using System.Collections.Generic;
using Bunk1DataAnnotations;
using System.Drawing;

namespace BunknotesApp
{
	public class UIBarBtn : UIBarButtonItem
	{
		public UIBarBtn (IntPtr handle):base(handle)
		{
		}

		public UIBarBtn (UIBarButtonSystemItem si, EventHandler handler):base(si,handler)
		{
		}

		public UIBarBtn (UIImage image, UIBarButtonItemStyle style, EventHandler handler):base(image, style, handler)
		{
		}
	}
	
	public class ComposeMessageScreen : ControllerBase
	{
		private bool FromLibrary;
		private UIImage Picture;
		private UITextViewExt messageText = new UITextViewExt ();
		private UIView optionsGroup = new UIView ();
		private bool IsReply = false;
		const string touchHereText = "touch here to start typing your bunknote";

		public ComposeMessageScreen ()
		{
			messageText.Frame = new RectangleF (0, 0, 320, 235);
			messageText.Font = UIFont.SystemFontOfSize (18);
			messageText.Changed += (sender, e) => {
				if (messageText.Text.Length > 1800) {
					messageText.Text = messageText.Text.Substring (0, 1800);
				}
			};
			
			AppDelegate.CurrentApp.AppDidEnterBackground += delegate {
				ConfigurationWorker.LastMessage = messageText.Text;
			};	
		}
		
		void AddOptionsGroup ()
		{
			
		}
		
		void SetUIBarButtons ()
		{
			var pickPhotoBtn = Picture == null ?
				new UIBarBtn (UIBarButtonSystemItem.Camera, CameraTapped) :
				new UIBarBtn (new UIImage ("Images/camIcon.png"), UIBarButtonItemStyle.Bordered, CameraTapped);
			
			var sendBtn = new UIBarBtn (new UIImage ("Images/sendBtn.png"), UIBarButtonItemStyle.Bordered, DoSend);
			var btns = new UIBarButtonItem[]{sendBtn,pickPhotoBtn};
			NavigationItem.SetRightBarButtonItems (btns, false);
		}
		
		public override void ViewDidLoad ()
		{
			TableView.ScrollEnabled = false;
			base.ViewDidLoad ();
			SetUIBarButtons ();
			
			
			if (string.IsNullOrWhiteSpace (ConfigurationWorker.LastMessage) || ConfigurationWorker.LastMessage == touchHereText) {
				messageText.Text = touchHereText;
				messageText.TextColor = UIColor.FromRGB (210, 210, 210);
				messageText.Started += delegate {
					messageText.Text = string.Empty;
					messageText.TextColor = UIColor.Black;
					messageText.Started -= delegate{};
				};
			} else {
				messageText.Text = ConfigurationWorker.LastMessage;	
			}
		}
		
		public override void ViewWillAppear (bool animated)
		{
			View.AddSubviews (new UIView[]{messageText, optionsGroup});
			if (RestManager.AuthenticationResult != null && !RestManager.AuthenticationResult.RepliesOn)
				messageText.BecomeFirstResponder ();
			PlaceElements (this.InterfaceOrientation);
			base.ViewWillAppear (animated);
		}
		
		public override void ViewWillDisappear (bool animated)
		{
			ConfigurationWorker.LastMessage = messageText.Text;
			base.ViewWillDisappear (animated);
		}
		
		public override void WillRotate (UIInterfaceOrientation toInterfaceOrientation, double duration)
		{
			base.WillRotate (toInterfaceOrientation, duration);
			PlaceElements (toInterfaceOrientation);
		}
		
		void PlaceElements (UIInterfaceOrientation orientation)
		{
			RectangleF rec, rec2;
			var portrait = (orientation == UIInterfaceOrientation.Portrait || orientation == UIInterfaceOrientation.PortraitUpsideDown);
			var yPort = 320;
			var yLands = 215;
			rec = portrait ? new RectangleF (0, 0, yPort, 360) : new RectangleF (0, 0, 480, yLands);
			rec2 = portrait ? new RectangleF (0, 360, yPort, 480) : new RectangleF (0, yLands, 480, 320);
			messageText.Frame = rec;
			optionsGroup.Frame = rec2;
			
			UIView back = new UIView (new RectangleF (0, 0, 500, 500));
			back.BackgroundColor = UIColor.LightGray;
			back.Alpha = 0.5f;
			
			optionsGroup.Add (back);
			optionsGroup.SendSubviewToBack (back);
			
			UILabel replyLabel = new UILabel (new RectangleF (15, 15, 205, 20)){Text = "add bunk reply stationary"};
			replyLabel.BackgroundColor = UIColor.Clear;
			
			UISwitch replySwitch = new UISwitch (new RectangleF (225, 15, 10, 10));
			replySwitch.On = IsReply;
			replySwitch.ValueChanged += delegate {
				this.IsReply = replySwitch.On;	
			};
			replySwitch.BackgroundColor = UIColor.Clear;
			
			optionsGroup.AddSubviews (new UIView[] { replyLabel, replySwitch });
		}
		
		void DoSend (object sender, EventArgs args)
		{
			if (string.IsNullOrWhiteSpace (messageText.Text)) { 
				MessageBox.Show ("message body cannot be empty");
				return;
			}
			ConfigurationWorker.LastMessage = messageText.Text;
			_restManager.SendBunkNote (messageText.Text, Picture, this.IsReply, result => {
				string message = "";
				if (result == CreateBunkNoteResult.SentSuccessfully) {
					ConfigurationWorker.LastMessage = string.Empty;
					messageText.Text = string.Empty;
					_restManager.Authenticate (ConfigurationWorker.LastUsedUsername, ConfigurationWorker.LastUsedPassword, "updating user data", () => {
						var nextScreen = RestManager.Authenticated ? typeof(SendingOptionsScreen) : typeof(LoginScreen);
						var alert = new UIAlertView{ Message = "bunknote sent succesfully" };
						alert.AddButton ("OK");
						alert.Clicked += delegate {
							var ls = NavigationController.ViewControllers.FirstOrDefault (x => x.GetType () == nextScreen);
							NavigationController.PopToViewController (ls, animated:true);	
						};
						alert.Show ();
					});
					
					return;
				} 
				if (result == CreateBunkNoteResult.Error)
					message = "bah, an error has occured!";
				if (result == CreateBunkNoteResult.BunkNoteAlreadySent)
					message = "your BunkNote has already been sent.";
				if (result == CreateBunkNoteResult.HasNotBeenSent)
					message = "note has NOT been sent.\nplease purchase\nadditional credits";
				
				MessageBox.Show (message);
			});
		}
		
		void CameraTapped (object sender, EventArgs args)
		{
			if (Picture == null) {
				TakePicture ();
				return;
			}
			
			var sheet = new UIActionSheet ("");
			sheet.AddButton ("discard picture");
			sheet.AddButton ("pick new picture");
			sheet.AddButton ("cancel");
			sheet.CancelButtonIndex = 2;
			if (this.InterfaceOrientation == UIInterfaceOrientation.Portrait) {
				// Dummy buttons to preserve the space for the UIImageView
				for (int i = 0; i < 4; i++) {
					sheet.AddButton ("");
					sheet.Subviews [i + 4].Alpha = 0;
				}
			
				var subView = new UIImageView (Picture);
				subView.ContentMode = UIViewContentMode.ScaleAspectFill;
				subView.Frame = new RectangleF (23, 185, 275, 210);
				subView.Layer.CornerRadius = 10;
				subView.Layer.MasksToBounds = true;
				subView.Layer.BorderColor = UIColor.Black.CGColor;
				sheet.AddSubview (subView);
			}
			
			sheet.Clicked += delegate(object ss, UIButtonEventArgs e) {
				if (e.ButtonIndex == 2)
					return;
				
				if (e.ButtonIndex == 0) {
					Picture = null;
					SetUIBarButtons ();
				} else
					TakePicture ();
			};
			sheet.ShowInView (this.View);
			
		}
		
		void TakePicture ()
		{
			FromLibrary = true;
			if (!UIImagePickerController.IsSourceTypeAvailable (UIImagePickerControllerSourceType.Camera)) {
				Camera.SelectPicture (this, PictureSelected);
				return;
			}
			
			var sheet = new UIActionSheet ("");
			sheet.AddButton ("take a photo");
			sheet.AddButton ("from album");
			sheet.AddButton ("back");
			
			sheet.CancelButtonIndex = 2;
			sheet.Clicked += delegate(object sender, UIButtonEventArgs e) {
				if (e.ButtonIndex == 2)
					return;
				
				if (e.ButtonIndex == 0) {
					FromLibrary = false;
					Camera.TakePicture (this, PictureSelected);
				} else
					Camera.SelectPicture (this, PictureSelected);
			};
			sheet.ShowInView (this.View);
		}
		
		UIImage Scale (UIImage image, SizeF size)
		{
			UIGraphics.BeginImageContext (size);
			image.Draw (new RectangleF (new PointF (0, 0), size));
			var ret = UIGraphics.GetImageFromCurrentImageContext ();
			UIGraphics.EndImageContext ();
			return ret;
		}
		
		void PictureSelected (NSDictionary pictureDict)
		{
			int level = NSUserDefaults.StandardUserDefaults.IntForKey ("sizeCompression");
			
			if ((pictureDict [UIImagePickerController.MediaType] as NSString) == "public.image") {
				Picture = pictureDict [UIImagePickerController.EditedImage] as UIImage;
				if (Picture == null)
					Picture = pictureDict [UIImagePickerController.OriginalImage] as UIImage;
				
				// Save a copy of the original picture
				if (!FromLibrary) {
					Picture.SaveToPhotosAlbum (delegate {
						// ignore errors
					});
				}
				
				var size = Picture.Size;
				float maxWidth;
				switch (level) {
				case 0:
					maxWidth = 640;
					break;
				case 1:
					maxWidth = 1024;
					break;
				default:
					maxWidth = size.Width;
					break;
				}

				//var hud = new LoadingHUDView ("Image", "Compressing");
				//View.AddSubview (hud);
				//hud.StartAnimating ();
				
				// Show the UI, and on a callback, do the scaling, so the user gets an animation
				NSTimer.CreateScheduledTimer (TimeSpan.FromSeconds (0), delegate {
					if (size.Width > maxWidth || size.Height > maxWidth)
						Picture = Scale (Picture, new SizeF (maxWidth, maxWidth * size.Height / size.Width));
					//hud.StopAnimating ();
					//hud.RemoveFromSuperview ();
					//hud = null;
				});
			} else {
				//NSUrl movieUrl = pictureDict [UIImagePickerController.MediaURL] as NSUrl;
				
				// Future use, when we find a video host that does not require your Twitter login/password
			}
			
			pictureDict.Dispose ();
			SetUIBarButtons ();
		}
	}
}
