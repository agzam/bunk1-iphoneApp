using System;
using MonoTouch.Dialog;
using MonoTouch.UIKit;
using System.Threading;
using MonoTouch.CoreAnimation;
using System.Drawing;
using MonoTouch.Foundation;
using System.Threading.Tasks;

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
			} else if ((sender as StringElement) != null) {
				val = ((StringElement)sender).Value;					
			}
			
			if (!string.IsNullOrWhiteSpace (val))
				return true;
			
			var activeCell = ((Element)sender).GetActiveCell();
				
			var animation = (CAKeyFrameAnimation)CAKeyFrameAnimation.FromKeyPath ("transform.translation.x");
			animation.TimingFunction = CAMediaTimingFunction.FromName(CAMediaTimingFunction.EaseOut.ToString());
			animation.Duration = 0.3;
			animation.Values = new NSObject[]{
				NSObject.FromObject (20),
				NSObject.FromObject (-20),
				NSObject.FromObject (10),
				NSObject.FromObject (-10),
				NSObject.FromObject (15),
				NSObject.FromObject (-15),
			};
			
			activeCell.Layer.AddAnimation (animation,"bounce");

			return false;
		}
	}
}