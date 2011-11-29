using System;
using MonoTouch.UIKit;

namespace Bunk1.Helpers
{
	public class MessageBox
	{
		public static void Show (string message)
		{
			var alert = new UIAlertView{ Message = message };
			alert.AddButton ("OK");
			alert.Show ();
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
	}
}

