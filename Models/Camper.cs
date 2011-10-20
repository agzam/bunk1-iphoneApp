namespace BunknotesApp
{
	public class Camper
	{
		public int CabinId { get; set; }
		
		public string LastName { get; set; }

		public string FirstName { get; set; }
		
		public override string ToString ()
		{
			return string.Format ("{0} {1}", FirstName, LastName);
		}
	}
}

