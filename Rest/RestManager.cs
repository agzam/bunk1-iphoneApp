using System;
using RestSharp;
using MonoTouch.UIKit;
using System.Json;
using System.Collections.Generic;
using BunknotesApp.Helpers;
using System.Threading;
using System.Threading.Tasks;
using System.ComponentModel;

namespace BunknotesApp
{
	public class CampersRequestArgs : EventArgs
	{
		public List<Camper> campers;
	}
	
	public class RestManager
	{
		const string clientAddress = "http://services.bunk1.com";
		private RestClient _client;
		
		public static bool Authenticated{ get; set; }

		public static AuthenticationResult AuthenticationResult{ get; set; }
		
		public event EventHandler RequestCompleted;
			
		public RestManager ()
		{
			_client = new RestClient (clientAddress);
		}
	
		private void GetRequest (string address, Action<RestResponse> callback, 
			Dictionary<string, string> parameters = null, string activityIndicatorMessage = "")
		{
			if (callback == null)
				throw new ArgumentException ("callback function required");
			
			var request = new RestRequest (address, Method.GET);
			
			if (parameters != null) {
				foreach (var param in parameters) {
					request.AddParameter (param.Key, param.Value);
				}	
			}
			
			ActivityIndicatorAlertView activityIndicator = string.IsNullOrWhiteSpace (activityIndicatorMessage) 
				? null : ActivityIndicatorAlertView.Show (activityIndicatorMessage);
			
			var bw = new BackgroundWorker ();
			bw.DoWork += (sender, e) => {
				e.Result = _client.Execute (request);
			};
			bw.RunWorkerCompleted += (sender, e) => {
				if (activityIndicator != null)
					activityIndicator.Hide (true);
				callback.Invoke (e.Result as RestResponse);
			};
			bw.RunWorkerAsync ();
		}
		
		public void Authenticate (string username, string password)
		{
			var parameters = new Dictionary<string, string>{ {"login",username}, {"pass",password}  };
			RestManager.Authenticated = false;
			RestManager.AuthenticationResult = null;
			
			GetRequest ("Authentications/CheckBunkNotesMobileAuthentication", response => {
				var authResult = JsonParser.ParseAuthenticationString (response.Content);
				switch (authResult.Result) {
				case ResponseResultType.BadRequest: 
					MessageBox.Show ("Invalid Username or Password,\nPlease retry");
					break;
				case ResponseResultType.NoCredits: 
					MessageBox.Show ("We are sorry,\nYour account does not have\nenough credits to send bunknotes");
					break;
				case ResponseResultType.CampBunknoteAccess: 	
					MessageBox.Show ("We are sorry,\nYour camp currently doesn't allow\nsending bunknotes");
					break;
				case ResponseResultType.UserTypeAccess:
					MessageBox.Show ("We are sorry,\nYour account type doesn't support\nsending bunknotes");
					break;
				case ResponseResultType.Successful:
					RestManager.Authenticated = true;
					RestManager.AuthenticationResult = authResult;
					RequestCompleted.Invoke (this, null);
					break;
				default:
					break;
				}
				
			}, parameters, "Connecting to bunk1");
			
		}
		
		public void GetCampers ()
		{
			if (!RestManager.Authenticated)
				throw new InvalidOperationException ();
			var auth = RestManager.AuthenticationResult;
			string httpLink = string.Format ("BunkNotes/GetBnoteUserNameSent/{0}/{1}/{2}", auth.CampId, auth.UserId, auth.Token);
			GetRequest (httpLink, response => {
				var campersResult = JsonParser.ParseCampers (response.Content);
				RequestCompleted.Invoke (this, new CampersRequestArgs{campers = campersResult});
			}, null, "Getting list of campers");
		}
	}
}

