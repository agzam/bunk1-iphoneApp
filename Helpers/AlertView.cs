using System;
using MonoTouch.UIKit;
using MonoTouch.Foundation;
using System.Drawing;
using BunknotesApp;

namespace Bunk1.Helpers
{
	[Register("ActivityIndicatorAlertView")]
	public class ActivityIndicatorAlertView : UIAlertView
	{
		public static ActivityIndicatorAlertView Show (string message)
		{
			var alertView = new ActivityIndicatorAlertView{ Message = message};
			alertView.Show ();
			return alertView;
		}
		/// <summary>
		/// our activity indicator
		/// </summary>
		UIActivityIndicatorView _activityIndicator; /// <summary>
		/// the message label in the window
		/// </summary>
		UILabel _lblMessage;
		/// <summary>
		/// The message that appears in the /// </summary>
		public string Message {
			get { return this._message;}
			set { _message = value; }
		}
	
		protected string _message;
		
		#region -= constructors =-
		 
		public ActivityIndicatorAlertView (IntPtr handle):base(handle)
		{
		}
		
		[Export("initWithCoder:")]
		public ActivityIndicatorAlertView (NSCoder coder):base(coder)
		{
		}
		
		public ActivityIndicatorAlertView ()
		{
		}
		
		#endregion
		
		/// <summary
		/// we use this to resize our alert view. doing it at any other time has
		/// weird effects because of th elifecycle
		/// </summary>
		public override void LayoutSubviews ()
		{
			base.LayoutSubviews ();
			//----- resize the control
			this.Frame = new RectangleF (this.Frame.X, this.Frame.Y, this.Frame.Width, 120);
			Theme ();
		}
		
		/// <summary>
		/// this is where we do the meat of creating our alert, which includes adding controls, etc.
		/// </summary>
		public override void Draw (RectangleF rect)
		{
			//--- if the control hasn't been setup yet
			if (this._activityIndicator == null) {
			
				//--- if we have a message
				if (!string.IsNullOrWhiteSpace (this._message)) {
					this._lblMessage = new UILabel (new RectangleF (20, 10, rect.Width - 40, 33));
					this._lblMessage.BackgroundColor = UIColor.Clear;
					this._lblMessage.TextColor = UIColor.LightTextColor;
					this._lblMessage.TextAlignment = UITextAlignment.Center;
					this._lblMessage.Text = this._message;
					this.AddSubview (this._lblMessage);
				}
				
				// --- instantiate a new activity indicator
				this._activityIndicator = new UIActivityIndicatorView (UIActivityIndicatorViewStyle.Gray);
				this._activityIndicator.Frame = new RectangleF
					((rect.Width / 2) - (this._activityIndicator.Frame.Width / 2)
						, 50, this._activityIndicator.Frame.Width
						, this._activityIndicator.Frame.Height);
				this.AddSubview (this._activityIndicator);
				this._activityIndicator.StartAnimating ();
			}
			base.Draw (rect);
		}
		
		/// <summary>
		/// dismisses the alert view. makes sure to call it to the main UI thread in case it's called from a worker thread
		/// </summary>
		public void Hide (bool animated)
		{
			this.InvokeOnMainThread (delegate {
				this.DismissWithClickedButtonIndex (0, animated);
			});
		}
		
		private void Theme ()
		{
			var defBlueback = this.GetSubviewElement<UIImageView> ();
			var label = this.GetSubviewElement<UILabel> ();
			var button = this.GetSubviewElement<UIButton> ();
			if (defBlueback != null)
				defBlueback.Alpha = 0;	
			if (label != null) {
				label.TextColor = UIColor.DarkGray;
				label.ShadowColor = UIColor.Clear;	
			}
			if (button != null) {
				button.BackgroundColor = ConfigurationWorker.DefaultBtnColor;
				button.Layer.CornerRadius = 6;
				button.Layer.MasksToBounds = true;
				var btnLbl = button.GetSubviewElement<UILabel> ();
				if (btnLbl != null) {
					btnLbl.TextColor = UIColor.Brown;
					btnLbl.ShadowColor = UIColor.Clear;	
				}
			}
			var back = new UIImageView (new UIImage ("Images/messageBoxBack.png"));
			back.Frame = new RectangleF (0, 0, this.Bounds.Width, this.Bounds.Height);
			back.Layer.CornerRadius = 15;
			back.Layer.MasksToBounds = true;
			back.Layer.BorderColor = UIColor.FromRGB (193, 183, 154).CGColor;
			back.Layer.BorderWidth = 2f;
			
			this.AddSubview (back);
			this.SendSubviewToBack (back);
		}
	}

}