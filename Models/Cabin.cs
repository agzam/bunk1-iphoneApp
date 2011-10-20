using System;

namespace BunknotesApp
{
	public class Cabin
	{
		public int Id { get; set; }

		public string Name { get; set; }
		
		public override string ToString ()
		{
			return Name;
		}
	}
}

