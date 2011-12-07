using System;
using RestSharp;
using MonoTouch.UIKit;
using System.Json;
using System.Collections.Generic;
using Bunk1.Helpers;
using System.Threading;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Linq;
using MonoTouch.Foundation;

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
		//const string clientAddress = "http://192.168.1.31/Bunk1.RestServices/";
		
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
				Thread.Sleep(5000);
				return _client.Execute (request);
			});
			task.ContinueWith (t => {
				activityIndicator.Hide (animated:true);
			});
			
			return task;
		}
		
		public void Authenticate (string username, string password, string message, Action callback)
		{
			if(!Reachability.ConnectionAvailible()) return;
				
			var parameters = new Dictionary<string, string>{ {"login",username}, {"pass",password}  };
			RestManager.Authenticated = false;
			RestManager.AuthenticationResult = null;
			
			var request = RequestAsync ("Authentications/CheckBunkNotesMobileAuthentication", parameters, message);
			request.ContinueWith (t => {
				var authResult = JsonParser.ParseAuthenticationString (t.Result.Content);
				if(authResult == null){
					MessageBox.Show("authentication error.\nplease try again.\nif error repeats several times\ncontact bunk1 tech support");
					return;
				}
				switch (authResult.Result) {
				case ResponseResultType.BadRequest: 
					MessageBox.Show ("invalid username or password,\nplease retry");
					break;
				case ResponseResultType.NoCredits: 
					MessageBox.Show ("we are sorry,\nyou do not have\nenough credits to send bunknotes");
					break;
				case ResponseResultType.CampBunknoteAccess: 	
					MessageBox.Show ("we are sorry,\nyour camp currently doesn't allow\nsending bunknotes");
					break;
				case ResponseResultType.UserTypeAccess:
					MessageBox.Show ("we are sorry,\nyour account type doesn't support\nsending bunknotes");
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
		
		private void CheckAuthenticationToken(){
			
			var address = string.Format("Authentications/CheckAuthenticationbyToken/{0}",AuthenticationResult.Token);
			var request = new RestRequest (address, Method.GET);
			var result = JsonParser.ParseAuthenticationTokenCheck(_client.Execute (request).Content);
			if (!result) {
				Authenticate(ConfigurationWorker.LastUsedUsername, ConfigurationWorker.LastUsedPassword, "", null);
			}
		}
		
		public void GetCampers (Action<List<Camper>> callback)
		{
			CheckAuthenticationToken();
			var auth = RestManager.AuthenticationResult;
			
			if (_campersList != null)
				_requestCompleted.Invoke (this, new CampersRequestArgs{campers = _campersList});
			
			string httpLink = string.Format ("BunkNotes/GetBnoteUserNameSent/{0}/{1}/{2}", auth.CampId, auth.UserId, auth.Token);
			var request = RequestAsync (httpLink, null, "getting list of campers");
			request.ContinueWith (t => {
				_campersList = JsonParser.ParseCampers (t.Result.Content);
				callback.Invoke (_campersList);
			}, TaskScheduler.FromCurrentSynchronizationContext ());
		}
		
		public Task<RestResponse> GetCabins (Action<List<Cabin>> callback)
		{
			CheckAuthenticationToken();
			
			var auth = RestManager.AuthenticationResult;
			
			if (_cabinsList != null)
				callback.Invoke (_cabinsList);
			
			string httpLink = string.Format ("Cabins/GetMobileCampCabins/{0}/{1}", auth.CampId, auth.Token);
			var response = RequestAsync (httpLink, null, "getting list of cabins");
			response.ContinueWith (t => {
				_cabinsList = new List<Cabin> ();
				_cabinsList = JsonParser.ParseCabins (t.Result.Content);
				ConfigurationWorker.IsThereAtLeast1Cabin = _cabinsList.Count() > 0;
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
			
			if (_cabinsList != null){
				result = _cabinsList.FirstOrDefault (x => x.Id == cabinId);
				if (result != null){
					ConfigurationWorker.IsThereAtLeast1Cabin = true;
					return result;
				}
			}
			var request = new RestRequest (string.Format ("Cabins/GetMobileCampCabins/{0}/{1}", auth.CampId, auth.Token), Method.GET);
			
			ActivityIndicatorAlertView activityIndicator;
			var thread = new Thread (() => activityIndicator = ActivityIndicatorAlertView.Show ("getting associated cabin"));
			thread.Start ();
			var task = Task<RestResponse>.Factory.StartNew (() => {
				return _client.Execute (request);});
			
			task.ContinueWith (t => activityIndicator.Hide (true), TaskScheduler.FromCurrentSynchronizationContext ());
			
			task.Wait ();
			
			_cabinsList = new List<Cabin> ();
			_cabinsList = JsonParser.ParseCabins (task.Result.Content);
			ConfigurationWorker.IsThereAtLeast1Cabin = _cabinsList.Count > 0;
			result = _cabinsList.FirstOrDefault (x => x.Id == cabinId);
			
			return result;
		}
		
		private ImageUploadResult SendImage (UIImage image)
		{
			var address = "BunkNotes/InsertBNoteImage?CampId={CampId}&Token={Token}";
			var request = new RestRequest (address, Method.POST);
			request.AddUrlSegment("CampId",RestManager.AuthenticationResult.CampId.ToString());
			request.AddUrlSegment("Token",RestManager.AuthenticationResult.Token);
			
			ActivityIndicatorAlertView activityIndicator;
			var thread = new Thread (() => activityIndicator = ActivityIndicatorAlertView.Show ("sending the image"));
			thread.Start ();			
			ImageUploadResult result = null;
			
			using (NSData imageData = image.AsJPEG()) {
				Byte[] imgByteArray = new Byte[imageData.Length];
				System.Runtime.InteropServices.Marshal.Copy (imageData.Bytes, imgByteArray, 0, Convert.ToInt32 (imageData.Length));
				request.AddFile ("DataString", imgByteArray, "filename");
			}
			
			var task = Task<RestResponse>.Factory.StartNew (() => {
				return _client.Execute (request);
			});
			task.ContinueWith (t => {
				activityIndicator.Hide (animated:true);
			}, TaskScheduler.FromCurrentSynchronizationContext ());
			task.Wait();
			result = JsonParser.ImageUploadResult (task.Result.Content);
			return result;
		}
		
		public void SendBunkNote (string messageText, UIImage image, bool isReply, Action<CreateBunkNoteResult> callback)
		{
			if(!Reachability.ConnectionAvailible()) return;
			CheckAuthenticationToken();
			var imageServerFilename = "";
			
			if (image != null) {
				var sendImgResult = SendImage (image);
				if (sendImgResult != null && sendImgResult.Success ) {
					imageServerFilename = sendImgResult.Filename;	
				}
			}
			
			var auth = RestManager.AuthenticationResult;
			
			var address = "BunkNotes/InsertBunkNotes";
			var request = new RestRequest (address, Method.POST);
			request.RequestFormat = DataFormat.Json;
			request.AddBody (new {
				BnGuid = "686DB329-20FA-4CE9-88CA-7139E1F6FC0E", 
				UserID = auth.UserId,
				CampID = auth.CampId,
				BunkID = 24595,
				FName = ConfigurationWorker.LastCamper.FirstName,
				LName = ConfigurationWorker.LastCamper.LastName,
				BnFrom = ConfigurationWorker.SentFrom,
				BnText = messageText,
				BnBorder = 0,
				BnReply = isReply ? 1:0,
				BnImageFilename = imageServerFilename,
				Token = auth.Token
			});
			
			ActivityIndicatorAlertView activityIndicator = ActivityIndicatorAlertView.Show ("sending bunknote");	
			
			var task = Task<RestResponse>.Factory.StartNew (() => {
				return _client.Execute (request);
			});
			task.ContinueWith (t => {
				activityIndicator.Hide (animated:true);
				callback.Invoke (JsonParser.BunkNoteResult (t.Result.Content));
			}, TaskScheduler.FromCurrentSynchronizationContext ());
		}
	}
}

