using System;
using MonoTouch.Dialog;
using MonoTouch.UIKit;

namespace Bunk1DataAnnotations
{
	public class ValidationAttribute : Attribute
	{
		public string ErrorMessage { get; set; }
	
		public virtual bool Validate (Object sender)
		{
			throw new NotImplementedException ();
		}
	}
	
	public class Required: ValidationAttribute
	{
		public override bool Validate (Object sender)
		{
			var element = sender as EntryElement;
			if (element == null) return true;
			element.FetchValue ();
			if (!string.IsNullOrWhiteSpace (element.Value)) return true;
			
			var alert = new UIAlertView{ Message = ErrorMessage };
			alert.AddButton ("OK");
			alert.Show ();
			return false;
		}
	}
}