using System;
using System.Json;
using System.Collections.Generic;

namespace BunknotesApp
{
	public class JsonParser
	{
		public static AuthenticationResult ParseAuthenticationString (string jsonString)
		{
			if (string.IsNullOrWhiteSpace (jsonString))
				return null;
			
			var jsonOb = JsonValue.Parse (jsonString);
			if (jsonOb.Count > 0 && jsonOb [0].ContainsKey ("UserID") && jsonOb [0].ContainsKey ("TypeID") && 
					jsonOb [0].ContainsKey ("Result") && jsonOb [0].ContainsKey ("Token")) {
										
				return new AuthenticationResult{
						UserId = jsonOb [0] ["UserID"],
			 			CampId = jsonOb [0] ["CampID"],
				        TypeId = jsonOb [0] ["TypeID"],
						Result = (ResponseResultType)Enum.ToObject (typeof(ResponseResultType), jsonOb [0] ["Result"]),
				       	Token = jsonOb [0] ["Token"].ToString ().Replace ("\"", "")
					};
			}
			return null;
		}
		
		public static List<Camper> ParseCampers (string jsonString)
		{
			if (string.IsNullOrWhiteSpace (jsonString))
				return null;
			var campers = new List<Camper>();
			var jsonOb = JsonValue.Parse (jsonString);
			foreach (JsonValue item  in jsonOb) {
				if (item.ContainsKey("CamperBunk") && item.ContainsKey("CamperIndex") && item.ContainsKey("CamperName")) {
					var nameStrings = item["CamperIndex"].ToString().Split('|');
					campers.Add(new Camper{
						CabinId = item["CamperBunk"],
						FirstName = nameStrings[0].Replace("\"",""),
						LastName = nameStrings[1].Replace("\"","")
					});
				}	
			}
			
			return campers;
		}
	}
}

