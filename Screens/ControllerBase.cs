using System;
using MonoTouch.Dialog;
using MonoTouch.UIKit;
using System.Linq;
using Bunk1.Helpers;
using MonoTouch.CoreGraphics;
using System.Drawing;
using MonoTouch.CoreAnimation;
using MonoTouch.Foundation;
using System.Collections.Generic;

namespace BunknotesApp
{
	public class SunPath
	{
		public int ControllerIndex;
		public int ToX, ToY;
		public bool PortraitMode;
		private static List<SunPath> WaysOSun = CreateWaysOSun ();
		private const int defPortraitX = 67, defPortraitY = 64, defLandscapeX = 242, defLandscapeY = 170;

		public static PointF CurrentSunPosition { get; set; }
		
		static SunPath ()
		{
			CurrentSunPosition = new PointF (0, 0);
		}

		public SunPath (int controllerIndex, int toX, int toY, bool portrait = true)
		{
			ControllerIndex = controllerIndex;
			ToX = toX;
			ToY = toY;
			PortraitMode = portrait;
		}
		
		public static PointF GetPointForControllerIndex (int controllerIndex, bool portrait = true)
		{
			var found = WaysOSun.FirstOrDefault (x => x.ControllerIndex == controllerIndex && x.PortraitMode == portrait);
			if (found != null) return new PointF (found.ToX, found.ToY);
			else return portrait ? new PointF (defPortraitX, defPortraitY) : new PointF(defLandscapeX,defLandscapeY);
		}
	
		public static List<SunPath> CreateWaysOSun ()
		{
			return new List<SunPath>{ 
				new SunPath (1, 287, 427),
				new SunPath (2, 67, 64)
			};
		}
	}
	
	public class ControllerBase: DialogViewController
	{
		private IValidator _validator;
		protected RestManager _restManager = new RestManager ();
		
		public virtual bool NavBarHidden { get; set; }
		
		public ControllerBase ():this(pushing: true)
		{
 
		}

		public ControllerBase (bool pushing):base(null, pushing)
		{
			_validator = new Validator ();
			Autorotate = true;
		}
		
		int GetCurrentControllerViewIndex ()
		{
			for (int i = 0; i < NavigationController.ViewControllers.Length; i++) {
				if (NavigationController.ViewControllers [i] == NavigationController.VisibleViewController)
					return i;
			}
			return -1;
		}
		
		void SetBackground (UIInterfaceOrientation interfaceOrientation)
		{
			var sunImg = new UIImage ("Images/sun.png");
			sunImageView = new UIImageView (sunImg);
			var imageViews = ParentViewController.View.Subviews.Where (v=> v.GetType() == typeof(UIImageView));
			foreach (var view in imageViews){
				if (((UIImageView)view).Image.Size == sunImg.Size){
					view.RemoveFromSuperview();
				}
			}
			ParentViewController.View.AddSubview (sunImageView);
			ParentViewController.View.SendSubviewToBack (sunImageView);	
			
			var alpha = (CAKeyFrameAnimation)CAKeyFrameAnimation.FromKeyPath ("opacity");
			alpha.Values = new NSNumber[]{
								   NSNumber.FromFloat (0),
                                   NSNumber.FromFloat (1),
                                 };
			alpha.Duration = 1;
			
			var rotation = (CAKeyFrameAnimation)CAKeyFrameAnimation.FromKeyPath ("transform.rotation");
			rotation.Duration = 1;
			rotation.TimingFunction = CAMediaTimingFunction.FromName (CAMediaTimingFunction.EaseOut.ToString ()); 
			rotation.Values = new NSNumber[]{
								   NSNumber.FromFloat (-3),
                                   NSNumber.FromFloat (0),
                                 };
			
			
			var posAnim = (CAKeyFrameAnimation)CAKeyFrameAnimation.FromKeyPath ("position");
			var posPath = new CGPath ();
			posPath.MoveToPoint (SunPath.CurrentSunPosition);
			var toPoint = SunPath.GetPointForControllerIndex (GetCurrentControllerViewIndex (), interfaceOrientation == UIInterfaceOrientation.Portrait || interfaceOrientation == UIInterfaceOrientation.PortraitUpsideDown);
			posPath.AddLineToPoint (toPoint);
			
			posAnim.Path = posPath;
			posAnim.Duration = 0.6;
			posAnim.TimingFunction = CAMediaTimingFunction.FromName (CAMediaTimingFunction.EaseOut.ToString ()); 

			posAnim.AnimationStopped += delegate {
				sunImageView.Layer.Position = toPoint;	
				SunPath.CurrentSunPosition = sunImageView.Layer.Position;
				//SetBackgroundGradient(interfaceOrientation);
			};
			sunImageView.Layer.AddAnimation (alpha, "opacity");
			sunImageView.Layer.AddAnimation (rotation, "rotation");
			sunImageView.Layer.AddAnimation (posAnim, "position");
			
		}
		
		void SetBackgroundGradient(UIInterfaceOrientation interfaceOrientation = UIInterfaceOrientation.Portrait){
			var backImage = new UIImage("Images/background.jpg");
			var backImageLandscape = new UIImage("Images/backgroundLandscape.jpg");
			ParentViewController.View.BackgroundColor= UIColor.FromPatternImage (
					(interfaceOrientation == UIInterfaceOrientation.Portrait || interfaceOrientation == UIInterfaceOrientation.PortraitUpsideDown) 
					? backImage : backImageLandscape);
//				new UIImageView((interfaceOrientation == UIInterfaceOrientation.Portrait 
//				                                || interfaceOrientation == UIInterfaceOrientation.PortraitUpsideDown) ? backImage : backImageLandscape);
//			var imageViews = ParentViewController.View.Subviews.Where (v=> v.GetType() == typeof(UIImageView));
//			foreach (var view in imageViews){
//				var size = ((UIImageView)view).Image.Size;
//				if (size == backImage.Size || size == backImageLandscape.Size){
//					view.RemoveFromSuperview();
//				}
//			}
//			ParentViewController.View.AddSubview (background);
//			ParentViewController.View.SendSubviewToBack (background);	
		}
		
		public override bool ShouldAutorotateToInterfaceOrientation (UIInterfaceOrientation toInterfaceOrientation)
		{
			return true;
		}
		
		UIImageView sunImageView;
		public override void LoadView ()
		{
			ParentViewController.View.BackgroundColor= UIColor.FromPatternImage(new UIImage("Images/background.jpg")); 
			base.LoadView ();
			
			TableView.BackgroundColor = UIColor.Clear;
			NavigationController.NavigationBar.TintColor = ConfigurationWorker.DefaultNavbarTint;
			SetBackground (UIInterfaceOrientation.Portrait);
		}
		
		public override void WillAnimateRotation (UIInterfaceOrientation toInterfaceOrientation, double duration)
		{
			base.WillAnimateRotation (toInterfaceOrientation, duration);
			SetBackground (toInterfaceOrientation);
			//SetBackgroundGradient(toInterfaceOrientation);
		}
		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);
			NavigationController.SetNavigationBarHidden (NavBarHidden, animated);
		}
	
		public virtual bool Validate ()
		{
			return _validator.Validate (this);
		}
		
		protected void SessionExpired ()
		{
			var loginScreen =
						NavigationController.ViewControllers.FirstOrDefault (x => x.GetType () == typeof(LoginScreen));
			if (loginScreen != null)
				NavigationController.PopToViewController (loginScreen, animated:true);
			else
				NavigationController.PushViewController (new LoginScreen (), animated:true);
			
			MessageBox.Show ("Session has expired!", 2000);
		}
	}
}

