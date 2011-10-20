using System;
using MonoTouch.Dialog;
using MonoTouch.UIKit;

namespace Bunk1DataAnnotations
{
	public class ValidationAttribute : Attribute
	{
		public string ErrorMessage { get; set; }

		public string DefaultValue { get; set; }
	
		public virtual bool Validate (Object sender)
		{
			throw new NotImplementedException ();
		}
	}
	
	public class Required: ValidationAttribute
	{
		public override bool Validate (Object sender)
		{
			string val = "";
			if ((sender as EntryElement) != null) {
				((EntryElement) sender).FetchValue ();
				val = ((EntryElement)sender).Value;
			}
			else if((sender as StringElement) != null){
				val = ((StringElement)sender).Value;					
			}
			
			if (!string.IsNullOrWhiteSpace(val)) return true;
			
			var alert = new UIAlertView{ Message = ErrorMessage };
			alert.AddButton ("OK");
			alert.Show ();
			return false;
		}
	}
}