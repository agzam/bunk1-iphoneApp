using System;

namespace BunknotesApp
{
	public class AuthenticationResult
	{
		public int UserId { get; set; }

		public int TypeId { get; set; }

		public ResponseResultType Result { get; set; }

		public int CampId { get; set; }

		public string Token { get; set; }
		
		public string FirstName { get; set; }
		
		public string NumberOfCredits{ get; set; }
		
		public bool RepliesOn {get;set;}
	}
}

