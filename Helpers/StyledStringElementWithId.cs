using System;
using MonoTouch.Dialog;

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
}

