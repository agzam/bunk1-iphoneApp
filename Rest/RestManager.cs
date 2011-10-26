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

		public event EventHandler RequestCompleted {
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
	
		private Task<RestResponse> RequestAsync (string address, Dictionary<string, string> parameters = null, string activityIndicatorMessage = "")
		{
			var request = new RestRequest (address, Method.GET);
			
			if (parameters != null) {
				foreach (var param in parameters) {
					request.AddParameter (param.Key, param.Value);
				}	
			}
			
			ActivityIndicatorAlertView activityIndicator = string.IsNullOrWhiteSpace (activityIndicatorMessage) 
				? null : ActivityIndicatorAlertView.Show (activityIndicatorMessage);	
			
			var task = Task<RestResponse>.Factory.StartNew (() => {
				return _client.Execute (request);
			});
			task.ContinueWith (t => {
				activityIndicator.Hide (animated:true);
			});
			
			return task;
		}
		
		public void Authenticate (string username, string password, Action callback)
		{
			var parameters = new Dictionary<string, string>{ {"login",username}, {"pass",password}  };
			RestManager.Authenticated = false;
			RestManager.AuthenticationResult = null;
			
			var request = RequestAsync ("Authentications/CheckBunkNotesMobileAuthentication", parameters, "Connecting to bunk1");
			request.ContinueWith (t => {
				var authResult = JsonParser.ParseAuthenticationString (t.Result.Content);
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
					callback.Invoke ();
					break;
				default:
					break;
				}
			}, TaskScheduler.FromCurrentSynchronizationContext ());
			
		}

		void HandleRequestCompleted (object sender, EventArgs e)
		{
			
		}
		
		public void GetCampers (Action<List<Camper>> callback)
		{
			if (!RestManager.Authenticated)
				throw new InvalidOperationException ();
			var auth = RestManager.AuthenticationResult;
			
			if (_campersList != null)
				_requestCompleted.Invoke (this, new CampersRequestArgs{campers = _campersList});
			
			string httpLink = string.Format ("BunkNotes/GetBnoteUserNameSent/{0}/{1}/{2}", auth.CampId, auth.UserId, auth.Token);
			var request = RequestAsync (httpLink, null, "Getting list of campers");
			request.ContinueWith (t => {
				_campersList = new List<Camper> ();
				_campersList = JsonParser.ParseCampers (t.Result.Content);
				callback.Invoke (_campersList);
			}, TaskScheduler.FromCurrentSynchronizationContext ());
		}
		
		public Task<RestResponse> GetCabins (Action<List<Cabin>> callback)
		{
			if (!RestManager.Authenticated)
				throw new InvalidOperationException ();
			
			var auth = RestManager.AuthenticationResult;
			
			if (_cabinsList != null)
				callback.Invoke (_cabinsList);
			
			string httpLink = string.Format ("Cabins/GetMobileCampCabins/{0}/{1}", auth.CampId, auth.Token);
			var response = RequestAsync (httpLink, null, "Getting list of campers");
			response.ContinueWith (t => {
				_cabinsList = new List<Cabin> ();
				_cabinsList = JsonParser.ParseCabins (t.Result.Content);
				callback.Invoke (_cabinsList);
			}, TaskScheduler.FromCurrentSynchronizationContext ());
			
			return response;
		}
		
		public Cabin GetCabinById (int cabinId)
		{
			Cabin result = null;
			
			if (!RestManager.Authenticated)
				throw new InvalidOperationException ();
			var auth = RestManager.AuthenticationResult;
			
			if (_cabinsList != null)
				result = _cabinsList.FirstOrDefault (x => x.Id == cabinId);
			if (result != null)
				return result;
			
			var request = new RestRequest (string.Format ("Cabins/GetMobileCampCabins/{0}/{1}", auth.CampId, auth.Token), Method.GET);
			
			ActivityIndicatorAlertView activityIndicator;
			var thread = new Thread (() => activityIndicator = ActivityIndicatorAlertView.Show ("Getting associated bunk"));
			thread.Start ();
			var task = Task<RestResponse>.Factory.StartNew (() => {
				return _client.Execute (request);});
			
			task.ContinueWith (t => activityIndicator.Hide (true), TaskScheduler.FromCurrentSynchronizationContext ());
			
			task.Wait ();
			
			_cabinsList = new List<Cabin> ();
			_cabinsList = JsonParser.ParseCabins (task.Result.Content);
			result = _cabinsList.FirstOrDefault (x => x.Id == cabinId);
			
			return result;
		}
	}
}

