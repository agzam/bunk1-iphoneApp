using System;
using RestSharp;
using MonoTouch.UIKit;
using System.Json;
using System.Collections.Generic;
using BunknotesApp.Helpers;
using System.Threading;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Linq;

namespace BunknotesApp
{
	public class CampersRequestArgs : EventArgs
	{
		public List<Camper> campers;
	}

	public class CabinsRequestArgs : EventArgs
	{
		public List<Cabin> cabins;
	}
	
	public class RestManager
	{
		const string clientAddress = "http://services.bunk1.com";
		private RestClient _client;
		private static List<Camper> _campersList;
		private static List<Cabin> _cabinsList;
		
		public static bool Authenticated{ get; set; }

		public static AuthenticationResult AuthenticationResult{ get; set; }
		
		private EventHandler _requestCompleted = delegate{};
		public event EventHandler RequestCompleted
		{
			add {
				if (_requestCompleted == null || !_requestCompleted.GetInvocationList ().Contains (value)) {
					_requestCompleted += value;
				}
			}
			remove {
				_requestCompleted -= value;
			}
		}
			
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
					_requestCompleted.Invoke(this,null);
					break;
				default:
					break;
				}
				
			}, parameters, "Connecting to bunk1");
			
		}

		void HandleRequestCompleted (object sender, EventArgs e)
		{
			
		}
		
		public void GetCampers ()
		{
			if (!RestManager.Authenticated)
				throw new InvalidOperationException ();
			var auth = RestManager.AuthenticationResult;
			
			if (_campersList != null)
				_requestCompleted.Invoke (this, new CampersRequestArgs{campers = _campersList});
			
			string httpLink = string.Format ("BunkNotes/GetBnoteUserNameSent/{0}/{1}/{2}", auth.CampId, auth.UserId, auth.Token);
			GetRequest (httpLink, response => {
				_campersList = new List<Camper> ();
				_campersList = JsonParser.ParseCampers (response.Content);
				_requestCompleted.Invoke (this, new CampersRequestArgs{campers = _campersList});
			}, null, "Getting list of campers");
		}
		
		public void GetCabins ()
		{
			if (!RestManager.Authenticated)
				throw new InvalidOperationException ();
			
			var auth = RestManager.AuthenticationResult;
			
			if (_cabinsList != null)
				_requestCompleted.Invoke (this, new CabinsRequestArgs{cabins = _cabinsList});
			
			string httpLink = string.Format ("Cabins/GetMobileCampCabins/{0}/{1}", auth.CampId, auth.Token);
			GetRequest (httpLink, response => {
				_cabinsList = new List<Cabin> ();
				_cabinsList = JsonParser.ParseCabins (response.Content);
				_requestCompleted.Invoke (this, new CabinsRequestArgs{cabins = _cabinsList});
			}, null, "Getting list of campers");
		}
	}
}

