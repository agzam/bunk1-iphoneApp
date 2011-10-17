using System;
using MonoTouch.UIKit;

namespace BunknotesApp.Helpers
{
	public class MessageBox
	{
		public static void Show (string message)
		{
			var alert = new UIAlertView{ Message = message };
			alert.AddButton ("OK");
			alert.Show ();
		}
	}
}

