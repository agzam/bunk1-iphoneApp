using System;

namespace BunknotesApp
{
	public class BunknoteModel
	{
		public BunknoteModel ()
		{
		}
		
		public User User { get; set; }

		public string SendFrom { get; set; }
		
		public Camper Camper { get; set; }
		
		public string Message { get; set; }
		
		public string Picture { get; set; }
	}
	
	public class User
	{
		int Id { get; set; }

		int CampId { get; set; }
		
		string Password { get; set; }
	}
	

	

}

