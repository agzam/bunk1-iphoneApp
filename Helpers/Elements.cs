using System;
using MonoTouch.Dialog;
using MonoTouch.UIKit;

namespace BunknotesApp
{
	public class StyledStringElementWithId: StyledStringElement
	{
		public int Id { get; set; }
		
		StyledStringElementWithId (string caption, string sValue, int id) : base (caption, sValue)
		{
			Id = id;
		}
	}
	
	public class StringElementExt:StyledMultilineElement{
		
		public StringElementExt(string caption) : base (caption) {	}
		
		public override float GetHeight (MonoTouch.UIKit.UITableView tableView, MonoTouch.Foundation.NSIndexPath indexPath)
		{
			return base.GetHeight (tableView, indexPath);
		} 
	}	
}

