using System;
using System.Collections.Generic;
using System.Linq;
using MonoTouch.Foundation;

namespace BunknotesApp
{
	public class ConfigurationWorker
	{
		private const string DIC_KEY = "Logins";
		private Dictionary<string, string> loginsDict;
		
		public ConfigurationWorker ()
		{
			loginsDict = new Dictionary<string, string> ();
		}
		
		private void ReadLoginsFromConfig ()
		{
			loginsDict = new Dictionary<string, string>();
			NSDictionary dic = NSUserDefaults.StandardUserDefaults.DictionaryForKey (DIC_KEY);
			if (dic == null) return;
			for (int i = 0; i < dic.Count; i++) {
            	loginsDict.Add(dic.Keys [i].ToString(), dic.Values [i].ToString());
			}
		}
		
		private void SaveLoginsToConfig ()
		{
			var userNames = loginsDict.Keys.SelectMany(x=> new object[] {x}).ToArray();
			var passwords = loginsDict.Values.SelectMany(x=> new object[] {x}).ToArray();
			var dict = NSDictionary.FromObjectsAndKeys (userNames, passwords);
			var key = new NSString (DIC_KEY);
			NSUserDefaults.StandardUserDefaults.SetValueForKey (dict, key);
			NSUserDefaults.StandardUserDefaults.Init();
		}
		
		public void SaveCurrentLogin (string userName, string password)
		{
			ReadLoginsFromConfig();
			if (loginsDict.Keys.Contains(userName)) loginsDict.Remove(userName);
			loginsDict.Add(userName, password);
			SaveLoginsToConfig();
		}
	    
		public string LastUsedUsername{
			get{
				return NSUserDefaults.StandardUserDefaults.StringForKey("LastUsedUsername");
			}
			set {
				NSUserDefaults.StandardUserDefaults.SetString(value, "LastUsedUsername");
				NSUserDefaults.StandardUserDefaults.Init();
			}
		}
		
		public string LastCamper{
			get{
				return "Camper";//NSUserDefaults.StandardUserDefaults.StringForKey("LastCamper");
			}
			set{
				NSUserDefaults.StandardUserDefaults.SetString(value, "LastCamper");
				NSUserDefaults.StandardUserDefaults.Init();
			}
		}
		
		public string LastCabin{
			get{
				return "Cabin";//NSUserDefaults.StandardUserDefaults.StringForKey("LastCabin");
			}
			set{
				NSUserDefaults.StandardUserDefaults.SetString(value, "LastCabin");
				NSUserDefaults.StandardUserDefaults.Init();
			}
		}
	}
}

