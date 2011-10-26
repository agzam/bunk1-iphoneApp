using System;
using System.Collections.Generic;
using System.Linq;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace BunknotesApp
{
	public class ConfigurationWorker
	{
		private const string cLogins = "Logins";
		private const string cLastUsedUserName = "LastUsedUserName";
		private const string cLastUsedCabinId = "LastCabinId";
		private const string cLastUsedCabin = "LastCabin";
		private const string cLastUsedCamper = "LastCamper";
		private Dictionary<string, string> loginsDict;
		public static readonly UIColor DefaultBtnColor = UIColor.FromRGB (237, 230, 178);
		public static readonly UIColor DefaultNavbarTint = UIColor.FromRGB (222, 165, 60);
			
		public ConfigurationWorker ()
		{
			loginsDict = new Dictionary<string, string> ();
		}
		
		#region private methods
		private void ReadLoginsFromConfig ()
		{
			loginsDict = new Dictionary<string, string> ();
			NSDictionary dic = NSUserDefaults.StandardUserDefaults.DictionaryForKey (cLogins);
			if (dic == null)
				return;
			for (int i = 0; i < dic.Count; i++) {
				loginsDict.Add (dic.Keys [i].ToString (), dic.Values [i].ToString ());
			}
		}
		
		private void SaveLoginsToConfig ()
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
		public string LastUsedUsername {
			get {
				return NSUserDefaults.StandardUserDefaults.StringForKey (cLastUsedUserName);
			}
			set {
				NSUserDefaults.StandardUserDefaults.SetString (value, cLastUsedUserName);
				NSUserDefaults.StandardUserDefaults.Init ();
			}
		}
		
		public Camper LastCamper {
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
		
		public Cabin LastCabin {
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
		
		public static void ReInitValues ()
		{
			NSUserDefaults.StandardUserDefaults.SetString ("", cLastUsedCamper);
			NSUserDefaults.StandardUserDefaults.SetString ("", cLastUsedCabin);
		}
		
		public void SaveCurrentLogin (string userName, string password)
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

