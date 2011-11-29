using System;
using System.Json;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

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
		
		public static bool ParseAuthenticationTokenCheck(string jsonString){
			if (string.IsNullOrWhiteSpace (jsonString))	return false;
			var jsonOb = JsonValue.Parse (jsonString);
			if (jsonOb.Count > 0 && jsonOb [0].ContainsKey ("result")){
				return (jsonOb [0]["result"] == 1);
			}
			return false;
		}
		
		public static List<Camper> ParseCampers (string jsonString)
		{
			if (string.IsNullOrWhiteSpace (jsonString))
				return null;
			var campers = new List<Camper> ();
			var jsonOb = JsonValue.Parse (jsonString);
			foreach (JsonValue item  in jsonOb) {
				if (item.ContainsKey ("CamperBunk") && item.ContainsKey ("CamperIndex") && item.ContainsKey ("CamperName")) {
					if (!item.ContainsKey ("CamperIndex") 
						|| string.IsNullOrWhiteSpace (item ["CamperBunk"].ToString ().Replace ("\"", "")))
						continue;						
					
					var nameStrings = item ["CamperIndex"].ToString ().Split ('|');
					
					campers.Add (new Camper{
						CabinId = item ["CamperBunk"],
						FirstName = nameStrings [0].Replace ("\"", ""),
						LastName = nameStrings [1].Replace ("\"", "")
					});
				}	
			}
			
			return campers;
		}
		
		public static List<Cabin> ParseCabins (string jsonString)
		{
			if (string.IsNullOrWhiteSpace (jsonString))
				return null;
						
			var cabins = new List<Cabin> ();
			var jsonOb = JsonValue.Parse (jsonString);
			foreach (JsonValue item  in jsonOb) {
				if (item.ContainsKey ("Bunk_ID") && item.ContainsKey ("Bunk_Name")) {
					if (!item.ContainsKey ("Bunk_ID") 
						|| string.IsNullOrWhiteSpace (item ["Bunk_Name"].ToString ().Replace ("\"", "")))
						continue;
					
					cabins.Add (new Cabin{
						Id = item ["Bunk_ID"],
						Name = item ["Bunk_Name"],
					});
				}	
			}
			
			return cabins;
		}
		
		public static CreateBunkNoteResult BunkNoteResult(string jsonString){
			var jsonOb = JsonValue.Parse (jsonString);

			if (jsonOb.Count > 0 && jsonOb.ContainsKey ("BnResult")) {
										
				int res;
				int.TryParse(jsonOb["BnResult"].ToString(), out res);
				return (CreateBunkNoteResult) res;
			}
			return CreateBunkNoteResult.Error;
		}
		
		public static ImageUploadResult ImageUploadResult(string xmlString){
			var xmlTree = XElement.Parse(xmlString);
			var result = xmlTree.Element("Result").Value;
			var fileName = xmlTree.Element("filename").Value;
			return new ImageUploadResult(){Success = (result == "2"), Filename = fileName };
		}
	}
}

