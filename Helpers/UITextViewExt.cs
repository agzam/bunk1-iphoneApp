using System;
using MonoTouch.UIKit;
using System.Drawing;
using MonoTouch.Foundation;

namespace BunknotesApp
{
	public class UITextViewExt: UITextView
	{
		int minimuGestureLength = 3;
		int maximumVariance = 5;
		PointF gestureStartPoint;
			
		public UITextViewExt ():base(RectangleF.Empty)
		{
		}
		
		public override void TouchesBegan (NSSet touches, UIEvent evt)
		{
			base.TouchesBegan (touches, evt);
			gestureStartPoint = ((UITouch)touches.AnyObject).LocationInView (this);
		}
		
		public override void TouchesMoved (NSSet touches, UIEvent evt)
		{
			if (RestManager.AuthenticationResult != null && !RestManager.AuthenticationResult.RepliesOn)
				return;
			base.TouchesMoved (touches, evt);
			var touch = ((UITouch)touches.AnyObject);
			PointF currentPosition = touch.LocationInView (this);
			
			float deltaX = currentPosition.X - gestureStartPoint.X;
			float deltaY = currentPosition.Y - gestureStartPoint.Y;
		
			if (deltaY >= minimuGestureLength && deltaX <= maximumVariance) {
				if (this.IsFirstResponder)
					this.ResignFirstResponder ();
			}
		}
	}
}

