using System;
using MonoTouch.UIKit;
using System.Drawing;
using System.Linq;
using BunknotesApp;

namespace Bunk1.Helpers
{
	public class MessageBox:UIAlertView
	{
		public static void Show (string message)
		{
			var alert = new MessageBox{ Message = message };
			alert.AddButton ("OK");
			alert.Show ();
		}
		
		public override void LayoutSubviews ()
		{
			base.LayoutSubviews ();
			Theme();
		}
		
		public static void Show (string message, int timeout)
		{
			var timer = new System.Timers.Timer (timeout);
			var alert = new UIAlertView{ Message = message };
			
			timer.Elapsed += delegate {
				timer.Stop ();	
				alert.InvokeOnMainThread (delegate {
					alert.DismissWithClickedButtonIndex (0, animated:true);	
				});
			};
			alert.Show ();
			timer.Start ();
		}
		
		private void Theme(){
			var defBlueback = this.GetSubviewElement<UIImageView>();
			var label = this.GetSubviewElement<UILabel>();
			var button = this.GetSubviewElement<UIButton>();
			if (defBlueback != null) defBlueback.Alpha = 0;	
			if (label != null) {
				label.TextColor = UIColor.DarkGray;
				label.ShadowColor = UIColor.Clear;	
			}
			if (button != null) {
				button.BackgroundColor = ConfigurationWorker.DefaultBtnColor;
				button.Layer.CornerRadius = 6;
				button.Layer.MasksToBounds = true;
				var btnLbl = button.GetSubviewElement<UILabel>();
				if (btnLbl != null) {
					btnLbl.TextColor = UIColor.Brown;
					btnLbl.ShadowColor = UIColor.Clear;	
				}
			}
			var back = new UIImageView(new UIImage("Images/messageBoxBack.png"));
			back.Frame = new RectangleF(0,0,this.Bounds.Width, this.Bounds.Height);
			back.Layer.CornerRadius = 15;
			back.Layer.MasksToBounds = true;
			back.Layer.BorderColor = UIColor.FromRGB(193,183,154).CGColor;
			back.Layer.BorderWidth = 2f;
			
			this.AddSubview(back);
			this.SendSubviewToBack(back);
		}
	}
	
	public class MessageBoxNew:UIAlertView{
		public MessageBoxNew(IntPtr handle):base(handle){}
		
		public static void Show(string messageText){
			var mb = new UIAlertView{Message = messageText};
			var back = new UIImageView(new UIImage("Images/messageBoxBack.png"));
			
			mb.Show();
			mb.Subviews[0].Alpha = 0f;
			var label = ((UILabel) mb.Subviews[1]);
			label.TextColor = UIColor.DarkGray;
			label.ShadowColor = UIColor.Clear;
			back.Frame = new RectangleF(0,0,mb.Bounds.Width, mb.Bounds.Height);
			back.Layer.CornerRadius = 15;
			back.Layer.MasksToBounds = true;
			back.Layer.BorderColor = UIColor.FromRGB(193,183,154).CGColor;
			back.Layer.BorderWidth = 2f;
			
			mb.AddSubview(back);
			mb.SendSubviewToBack(back);
		}
	}
}

