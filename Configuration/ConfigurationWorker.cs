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
		private static Dictionary<string, string> loginsDict;
		private static Dictionary<int, string> usedCampersDict;
		private static Dictionary<int, string> usedCabinsDict;
		public static readonly UIColor DefaultBtnColor = UIColor.FromRGB (242, 201, 136);
		public static readonly UIColor DefaultNavbarTint = UIColor.FromRGB (222, 165, 60);
			
		static ConfigurationWorker ()
		{
			loginsDict = new Dictionary<string, string> ();
			usedCampersDict = new Dictionary<int, string> ();
			usedCabinsDict = new Dictionary<int, string> ();
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
		
		private static void ReadCampersFromConfig ()
		{
			usedCampersDict = new Dictionary<int, string> ();
			NSDictionary dic = NSUserDefaults.StandardUserDefaults.DictionaryForKey (cUsedCampers);
			if (dic == null)
				return;
			for (int i = 0; i < dic.Count; i++) {
				int val = 0;
				if (int.TryParse (dic.Keys [i].ToString (), out val))
					usedCampersDict.Add (val, dic.Values [i].ToString ());
			}
		}
		
		private static void SaveCampersToConfig ()
		{
			var campIds = usedCampersDict.Keys.SelectMany (x => new object[] {x.ToString ()}).ToArray ();
			var camperNames = usedCampersDict.Values.SelectMany (x => new object[] {x}).ToArray ();
			var dict = NSDictionary.FromObjectsAndKeys (camperNames, campIds);
			var key = new NSString (cUsedCampers);
			NSUserDefaults.StandardUserDefaults.SetValueForKey (dict, key);
			NSUserDefaults.StandardUserDefaults.Init ();
		}
		
		private static void ReadCabinsFromConfig ()
		{
			usedCabinsDict = new Dictionary<int, string> ();
			NSDictionary dic = NSUserDefaults.StandardUserDefaults.DictionaryForKey (cUsedCabins);
			if (dic == null) return;
			for (int i = 0; i < dic.Count; i++) {
				int val = 0;
				if (int.TryParse (dic.Keys [i].ToString (), out val))
					usedCabinsDict.Add (val, dic.Values [i].ToString ());
			}
		}
		
		private static void SaveCabinsToConfig ()
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
					ReadCampersFromConfig ();
					var campId = RestManager.AuthenticationResult.CampId;
					if (usedCampersDict.Keys.Contains (campId)) {
						var dicValue = usedCampersDict [campId].Split ('|');
						return new Camper{FirstName = dicValue [0], LastName = dicValue [1]};
					}
				} else {
					return new Camper{FirstName = camper [0], LastName = camper [1]};
				}
				return new Camper ();
			}
			set {
				var campId = RestManager.AuthenticationResult.CampId;
				var val = value.FirstName + "|" + value.LastName;
				if (usedCampersDict.Keys.Contains (campId))
					usedCampersDict.Remove (campId);
				usedCampersDict.Add (campId, val);
				NSUserDefaults.StandardUserDefaults.SetString (val, cLastUsedCamper);
				SaveCampersToConfig ();
			}
		}
		
		public static Cabin LastCabin {
			get {
				var id = NSUserDefaults.StandardUserDefaults.IntForKey (cLastUsedCabinId);
				var name = NSUserDefaults.StandardUserDefaults.StringForKey (cLastUsedCabin);
				if (string.IsNullOrEmpty (name)) {
					ReadCabinsFromConfig ();
					var campId = RestManager.AuthenticationResult.CampId;
					if (usedCabinsDict.Keys.Contains (campId)) {
						var dicValue = usedCabinsDict [campId].Split ('|');
						int cabId = 0;
						if (int.TryParse(dicValue[0], out cabId)) {
							return new Cabin{Id = cabId, Name = dicValue [1]};	
						}
					}	
				}
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
				if (usedCabinsDict.Keys.Contains (campId))
					usedCabinsDict.Remove (campId);
				usedCabinsDict.Add (campId, val);
				SaveCabinsToConfig ();
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

