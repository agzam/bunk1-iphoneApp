using System;
using System.Collections.Generic;
using System.Linq;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace BunknotesApp
{
	public static class ConfigurationWorker
	{
		const string cLogins = "Logins";
		const string cLastUsedUserName = "LastUsedUserName";
		const string cLastUsedPass = "LastUsedPass";
		const string cLastUsedCabinId = "LastCabinId";
		const string cLastUsedCabin = "LastCabin";
		const string cLastUsedCamper = "LastCamper";
		const string cSentFrom = "SentFrom";
		const string cLastMessage = "LastMessage";
		const string cUsedCampers = "UsedCampersByCamp";
		const string cUsedCabins = "UsedCabinsByCamp";
		const string cAtLeast1Cabin = "IsThereAtLeastOneCabin";
		private static Dictionary<string, string> loginsDict;
		private static Dictionary<string, string> usedCampersDict = new Dictionary<string, string> ();
		private static Dictionary<string, string> usedCabinsDict = new Dictionary<string, string> ();
		public static readonly UIColor DefaultBtnColor = UIColor.FromRGB (242, 201, 136);
		public static readonly UIColor DefaultNavbarTint = UIColor.FromRGB (222, 165, 60);
			
		static ConfigurationWorker ()
		{
			loginsDict = new Dictionary<string, string> ();
		}
		
		#region private methods
		private static void ReadLoginsFromConfig ()
		{
			loginsDict = new Dictionary<string, string> ();
			NSDictionary dic = NSUserDefaults.StandardUserDefaults.DictionaryForKey (cLogins);
			if (dic == null)
				return;
			for (int i = 0; i < dic.Count; i++) {
				loginsDict.Add (dic.Keys [i].ToString (), dic.Values [i].ToString ());
			}
		}
		
		private static void SaveLoginsToConfig ()
		{
			var userNames = loginsDict.Keys.SelectMany (x => new object[] {x}).ToArray ();
			var passwords = loginsDict.Values.SelectMany (x => new object[] {x}).ToArray ();
			var dict = NSDictionary.FromObjectsAndKeys (userNames, passwords);
			var key = new NSString (cLogins);
			NSUserDefaults.StandardUserDefaults.SetValueForKey (dict, key);
			NSUserDefaults.StandardUserDefaults.Init ();
		}
		
		private static void ReadUsedCampersFromConfig ()
		{
			usedCampersDict.Clear();
			NSDictionary dic = NSUserDefaults.StandardUserDefaults.DictionaryForKey (cUsedCampers);
			if (dic == null)
				return;
			for (int i = 0; i < dic.Count; i++) {
				usedCampersDict.Add (dic.Keys [i].ToString (), dic.Values [i].ToString ());
			}
		}
		
		private static void SaveUsedCampersToConfig ()
		{
			var campIds = usedCampersDict.Keys.SelectMany (x => new object[] {x.ToString ()}).ToArray ();
			var camperNames = usedCampersDict.Values.SelectMany (x => new object[] {x}).ToArray ();
			var dict = NSDictionary.FromObjectsAndKeys (camperNames, campIds);
			var key = new NSString (cUsedCampers);
			NSUserDefaults.StandardUserDefaults.SetValueForKey (dict, key);
			NSUserDefaults.StandardUserDefaults.Init ();
		}
		
		private static void ReadUsedCabinsFromConfig ()
		{
			usedCabinsDict.Clear();
			NSDictionary dic = NSUserDefaults.StandardUserDefaults.DictionaryForKey (cUsedCabins);
			if (dic == null)
				return;
			for (int i = 0; i < dic.Count; i++) {
				usedCabinsDict.Add (dic.Keys [i].ToString (), dic.Values [i].ToString ());
			}
		}
		
		private static void SaveUsedCabinsToConfig ()
		{
			var campIds = usedCabinsDict.Keys.SelectMany (x => new object[] {x.ToString ()}).ToArray ();
			var cabins = usedCabinsDict.Values.SelectMany (x => new object[] {x}).ToArray ();
			var dict = NSDictionary.FromObjectsAndKeys (cabins, campIds);
			var key = new NSString (cUsedCabins);
			NSUserDefaults.StandardUserDefaults.SetValueForKey (dict, key);
			NSUserDefaults.StandardUserDefaults.Init ();
		}
		
		#endregion
		
		#region public members 
		public static string LastUsedUsername {
			get {
				return NSUserDefaults.StandardUserDefaults.StringForKey (cLastUsedUserName);
			}
			set {
				NSUserDefaults.StandardUserDefaults.SetString (value, cLastUsedUserName);
				NSUserDefaults.StandardUserDefaults.Init ();
			}
		}
		
		public static string LastUsedPassword {
			get {
				return NSUserDefaults.StandardUserDefaults.StringForKey (cLastUsedPass);
			}
			set {
				NSUserDefaults.StandardUserDefaults.SetString (value, cLastUsedPass);
				NSUserDefaults.StandardUserDefaults.Init ();
			}
		}
		
		public static Camper LastCamper {
			get {
				var camper = NSUserDefaults.StandardUserDefaults.StringForKey (cLastUsedCamper).Split ('|');
				if (camper.Length < 2) {
					ReadUsedCampersFromConfig ();
					if (usedCampersDict.Keys.Contains (UserIdCampIdString)) {
						var dicValue = usedCampersDict [UserIdCampIdString].Split ('|');
						return new Camper{FirstName = dicValue [0], LastName = dicValue [1]};
					}
				} else {
					return new Camper{FirstName = camper [0], LastName = camper [1]};
				}
				return new Camper ();
			}
			set {
				var val = value.FirstName + "|" + value.LastName;
				if (usedCampersDict.Keys.Contains (UserIdCampIdString))
					usedCampersDict.Remove (UserIdCampIdString);
				usedCampersDict.Add (UserIdCampIdString, val);
				NSUserDefaults.StandardUserDefaults.SetString (val, cLastUsedCamper);
				SaveUsedCampersToConfig ();
			}
		}
		
		static string UserIdCampIdString {
			get {	
				return string.Format ("{0}|{1}",
			        RestManager.AuthenticationResult.UserId.ToString (),
			        RestManager.AuthenticationResult.CampId.ToString ());
			}
		}
		
		public static Cabin LastCabin {
			get {
				var id = NSUserDefaults.StandardUserDefaults.IntForKey (cLastUsedCabinId);
				var name = NSUserDefaults.StandardUserDefaults.StringForKey (cLastUsedCabin);
				if (string.IsNullOrEmpty (name)) {
					ReadUsedCabinsFromConfig ();
					if (usedCabinsDict.Keys.Contains (UserIdCampIdString)) {
						var dicValue = usedCabinsDict [UserIdCampIdString].Split ('|');
						int cabId = 0;
						if (int.TryParse (dicValue [0], out cabId)) {
							NSUserDefaults.StandardUserDefaults.SetBool (true, cAtLeast1Cabin);
							return new Cabin{Id = cabId, Name = dicValue [1]};	
						}
					}	
				}
				NSUserDefaults.StandardUserDefaults.SetBool (true, cAtLeast1Cabin);
				return new Cabin{Id = id, Name = name};
			}
			set {
				if (value == null) {
					NSUserDefaults.StandardUserDefaults.RemoveObject (cLastUsedCabinId);
					NSUserDefaults.StandardUserDefaults.RemoveObject (cLastUsedCabin);
					return;
				}
				NSUserDefaults.StandardUserDefaults.SetInt (value.Id, cLastUsedCabinId);
				NSUserDefaults.StandardUserDefaults.SetString (value.Name, cLastUsedCabin);
				var campId = RestManager.AuthenticationResult.CampId;
				var val = value.Id.ToString () + "|" + value.Name;
				if (usedCabinsDict.Keys.Contains (UserIdCampIdString))
					usedCabinsDict.Remove (UserIdCampIdString);
				usedCabinsDict.Add (UserIdCampIdString, val);
				SaveUsedCabinsToConfig ();
			}
		}
		
		public static string SentFrom {
			get {
				return NSUserDefaults.StandardUserDefaults.StringForKey (cSentFrom);
			}
			set {
				NSUserDefaults.StandardUserDefaults.SetString (value, cSentFrom);
			}
		}
		
		public static bool IsThereAtLeast1Cabin {
			get {
				var res = NSUserDefaults.StandardUserDefaults.BoolForKey (cAtLeast1Cabin);
				return res;
			}
			set {
				NSUserDefaults.StandardUserDefaults.SetBool (value, cAtLeast1Cabin);
			}
		}
		
		public static string LastMessage {
			get {
				return NSUserDefaults.StandardUserDefaults.StringForKey (cLastMessage);
			}
			set {
				NSUserDefaults.StandardUserDefaults.SetString (value, cLastMessage);
			}
		}
		
		public static void ReInitValues ()
		{
			NSUserDefaults.StandardUserDefaults.SetString ("", cLastUsedCamper);
			NSUserDefaults.StandardUserDefaults.SetString ("", cLastUsedCabin);
			NSUserDefaults.StandardUserDefaults.SetBool (false, cAtLeast1Cabin);
		}
		
		public static void SaveCurrentLogin (string userName, string password)
		{
			ReadLoginsFromConfig ();
			if (loginsDict.Keys.Contains (userName))
				loginsDict.Remove (userName);
			loginsDict.Add (userName, password);
			SaveLoginsToConfig ();
		}
		
		#endregion
	}
}

