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
		
		private static Dictionary<string, string> loginsDict;
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
				return camper.Length == 2 ? 
					new Camper{FirstName = camper [0], LastName = camper [1]} :
					new Camper();
			}
			set {
				NSUserDefaults.StandardUserDefaults.SetString (value.FirstName + "|" + value.LastName, cLastUsedCamper);
			}
		}
		
		public static Cabin LastCabin {
			get {
				var id = NSUserDefaults.StandardUserDefaults.IntForKey (cLastUsedCabinId);
				var name = NSUserDefaults.StandardUserDefaults.StringForKey (cLastUsedCabin);
				return new Cabin{Id = id, Name = name};
			}
			set {
				if (value == null) {
					NSUserDefaults.StandardUserDefaults.RemoveObject(cLastUsedCabinId);
					NSUserDefaults.StandardUserDefaults.RemoveObject(cLastUsedCabin);
					return;
				}
				NSUserDefaults.StandardUserDefaults.SetInt (value.Id, cLastUsedCabinId);
				NSUserDefaults.StandardUserDefaults.SetString (value.Name, cLastUsedCabin);
			}
		}
		
		public static string SentFrom{
			get{
				return NSUserDefaults.StandardUserDefaults.StringForKey (cSentFrom);
			}
			set{
				NSUserDefaults.StandardUserDefaults.SetString (value, cSentFrom);
			}
		}
		
		public static string LastMessage{
			get{
				return NSUserDefaults.StandardUserDefaults.StringForKey (cLastMessage);
			}
			set{
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

