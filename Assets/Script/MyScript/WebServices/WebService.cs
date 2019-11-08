/*
 * Web Service
 * 
 * A simple HTTP get / post wrapper with SimpleJSON support.
 * 
 * Max Felker | felkerm@gmail.com
 * 
*/

using System; 
using UnityEngine;
using System.Collections; 
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.Networking;
using MiniJSON;
using SimpleJSON;
using OTPL.modal;
using OTPL.UI;
using VoxelBusters.NativePlugins;

public class WebService : Singleton<WebService> {

	//List for Hub detail panel.
//	public List<CategoryDetailWebService> catDetailList = new List<CategoryDetailWebService>();
	//List for Loved detail panel.
//	public List<CategoryDetailWebService> catLovedDetailList = new List<CategoryDetailWebService>();
	// general web service
	public string Endpoint;
	public Dictionary<string, string> postVariables;
	public JSONNode responseJSON;
	//Static field.
	public UserParameter appUser = new UserParameter();
	public CategoryDetail catDetailParameter = new CategoryDetail();
	public HistoryData historyData = new HistoryData();
	public bool isSocialLogin;
	public string headerString;
	public bool isLoginScreen;
	public bool isSignUpScreen;

	// Set up states
    enum State {Idle,Getting, Posting,Responded};
	State state;
	
	// Set up our call back delegation
	public delegate void PostCallBack(UnityWebRequest response);
	PostCallBack _PostCallBack;

	public delegate void GetCallBack(string response);
	GetCallBack _GetCallBack;

	public bool isRegisterFromFacebook;
	public bool isRegisterFromGoogle;
	public bool isRegisterFromLinkedin;

	[HideInInspector]
	public string error;

	public void Start() {

		Initialise ();
		state = State.Idle;

	}
	void OnEnable ()
	{
		// Register to event
		NetworkConnectivity.NetworkConnectivityChangedEvent	+= NetworkConnectivityChangedEvent;
	}

	void OnDisable ()
	{
		// Deregister to event
		NetworkConnectivity.NetworkConnectivityChangedEvent	-= NetworkConnectivityChangedEvent;
	}
	// GET from the server
	public void Get(string URL, GetCallBack getCallBack ){

		Debug.Log ("Get Url :" + URL);
	    //WWW response = new WWW (URL);
		WWW response = new WWW (URL);
		 
		state = State.Getting;
	 
	    StartCoroutine( 
			GetRequest(response,getCallBack)
		);
    }
 
	// POST to the server
	public void Post(string URL, byte [] parameter, string jsonStr, PostCallBack postCallBack){

//        WWWForm form = new WWWForm();
//		var headers = new Dictionary<string,string>(); //form.headers;
//		byte[] rawData;
//		foreach(KeyValuePair<String,String> post_arg in post) {
//			
//			form.AddField(post_arg.Key, post_arg.Value);
//			
//	    }
//		rawData = form.data;
//		headers["Authorization"] = "Basic " + Convert.ToBase64String(Encoding.UTF8.GetBytes("jeeteshb@gmail.com:jeet@123"));
//		string str = ASCIIEncoding.ASCII.GetString(Encoding.UTF8.GetBytes("jeeteshb@gmail.com:jeet@123"));
//		headers.Add ("Content-Type", "application/json");
//		headers.Add ("Authorization", "Basic " + Convert.ToBase64String (Encoding.UTF8.GetBytes ("jeeteshb@gmail.com:jeet@123")));
//		WWW response = new WWW(URL, rawData, headers);
//
//		state = State.Posting;
//	 
//	    StartCoroutine( 
//		GetRequest(response,callBack)
//		);
		Debug.Log("Post Url :" + URL);
		var response = new UnityWebRequest(URL, "POST");
		//byte[] bodyRaw = Encoding.UTF8.GetBytes(null);
		response.uploadHandler = (UploadHandler) new UploadHandlerRaw(parameter);
		response.downloadHandler = (DownloadHandler) new DownloadHandlerBuffer();
		response.SetRequestHeader("Content-Type", "application/json");
		response.SetRequestHeader("Authorization", jsonStr);
		state = State.Posting;

		StartCoroutine( MakeWebRequest(response,postCallBack));
    }

	// Make the Request & Exectute Call Back function
	IEnumerator GetRequest( WWW response, GetCallBack getCallBack ){
	
        yield return response;

		state = State.Responded; 

//		AppManager.instance.messageBoxManager.HidePreloader();
		Debug.Log("Web Service Response: "+response.text);

		getCallBack (response.text);

		if (getResponseCode(response) == 500) {
            AppManager.Instnace.messageBoxManager.ShowGenericPopup("Warning", "There was an error processing the request. Please check your network.", "Ok");
		}
		if (getResponseCode (response) > 400 && getResponseCode (response) < 450) {
			
		}

		response.Dispose ();
//		//		byte[] bytesToEncode = Encoding.UTF8.GetBytes (inputText);
//		string decodedText = Convert.ToBase64String(response.bytes);
//
//		Debug.Log (decodedText);
//
//		JSONNode responseJSON = JSON.Parse(response.text);
//
//		if (responseJSON.Value != null || responseJSON.Value == "") {
//			string jsonString = responseJSON ["error"].ToString ();
//			Debug.Log (jsonString);
//
//			if (responseJSON ["error"] == null && callBack != null) {
//			
//				callBack (responseJSON);
//	
//			}
//
//			if (responseJSON ["error"] != null) {
//				Debug.LogError ("Web Service Error: " + responseJSON ["error"]);
//			}
//		}
//		state = State.Idle; 
//		AppManager.instance.messageBoxManager.HidePreloader();		
    }         

	#region ErrorCode for WWW

	public static int getResponseCode(WWW request) {
		int ret = 0;
		if (request.responseHeaders == null) {
			Debug.LogError("no response headers.");
		}
		else {
			if (!request.responseHeaders.ContainsKey("STATUS")) {
				Debug.Log("response headers has no STATUS.");
			}
			else {
				ret = parseResponseCode(request.responseHeaders["STATUS"]);
			}
		}
		return ret;
	}
	public static int parseResponseCode(string statusLine) {
		int ret = 0;

		string[] components = statusLine.Split(' ');
		if (components.Length < 3) {
			Debug.LogError("invalid response status: " + statusLine);
		}
		else {
			if (!int.TryParse(components[1], out ret)) {
				Debug.LogError("invalid response code: " + components[1]);
			}
		}

		return ret;
	}
	#endregion

	IEnumerator MakeWebRequest( UnityWebRequest response, PostCallBack postCallBack ){

		yield return response.SendWebRequest();  //yield break

//		AppManager.instance.messageBoxManager.HidePreloader();
		Debug.Log("MakeWebRequest: "+response.downloadHandler.text);

		if (!response.isNetworkError && !response.isHttpError) {
			WebService.Instnace.isLoginScreen = false;
            WebService.Instnace.isSignUpScreen = false;
			if (FacebookLogin.isFacebook) {
				FacebookLogin.isFacebook = false;
				FacebookLogin.UserCheckCallback (response);

            } else if(FacebookLogin.isFacebookRegisterUser){
                FacebookLogin.isFacebookRegisterUser = false;
                FacebookLogin.WebCallbackRegister(response);
            }
            else {
                if (postCallBack != null)
                {
                    postCallBack(response);
                }
			}
		}
		else 
		{

            if (response.responseCode == 408)
            {
                AppManager.Instnace.messageBoxManager.ShowGenericPopup("Warning", "Webservice.cs - Request Timeout. Please try again.", "Ok");
                AppManager.Instnace.messageBoxManager.HidePreloader();
            }
            else if (response.responseCode == 500)
            {
                string strError = response.downloadHandler.text;
                AppManager.Instnace.messageBoxManager.ShowGenericPopup("Warning", "There was an error processing the request", "Ok");
                AppManager.Instnace.messageBoxManager.HidePreloader();
            }
            else if (response.responseCode == 503)
            {
                string strError = response.downloadHandler.text;
                AppManager.Instnace.messageBoxManager.ShowGenericPopup("Maintainence", "503 Service Unavailable...Server under maintenance. Please try again soon.", "Ok");
                AppManager.Instnace.messageBoxManager.HidePreloader();
            }
            else {
				if (FacebookLogin.isFacebook) {
					FacebookLogin.isFacebook = false;
					FacebookLogin.UserCheckCallback (response);
				} else {
                    if (postCallBack != null)
                    {
                        postCallBack(response);
                    }
				}
			}
		}
		response.Dispose ();

	}

	#region API Methods

	private void Initialise ()
	{
//		NPBinding.NetworkConnectivity.Initialise ();			
	}

	public bool IsConnected ()
	{
		return NPBinding.NetworkConnectivity.IsConnected;
	}

	#endregion

	#region API Callback Methods

	private void NetworkConnectivityChangedEvent (bool _isConnected)
	{
	}
		
	#endregion
	public void DeleteAppUser(){

		appUser = null;
		appUser = new UserParameter ();
	}

	public void InternetConnectivity(){

		string onlineStr = "Yeah! Now we are online.";
		string notOnlineStr = "The Network request has timed out, please check your internet connection and try again"; 

		if (!IsConnected ())
		{
            if (AppManager.Instnace.messageBoxManager != null) {
                AppManager.Instnace.messageBoxManager.ShowGenericPopup ("Connectivity", notOnlineStr, "Ok", InternetConnectivity);
			}
		}
	}
}

[System.Serializable]
public class UserParameter
{
	public string Address1;
	public string Address2;
	public string Country;
	public string DateOfBirth;
	public string Email;
	public string FirstName;
	public string LastName;
	public string Gender;
	public string Mobile;
	public string PostCode;
	public string RelationshipStatus;
	public string State;
	public string Suburb;
	public string LinkedInId;
	public string GoogleId;
	public string FacebookId;
	public string Password;
	public string PasswordSalt;
	public long RegionId;
	public string LoginInfo;
	public int isLoggedIn;
	public string interest;
	public string DateCreated;
    public long latitude;
    public long longitude;
    public string versionNumber;
    public string deviceId;
    public string deviceName;
}

//************************************************************
//                  campaign parameters
//************************************************************

[System.Serializable]
public class trackingRoot 
{
    public TrackingData trackingData;

    public trackingRoot(){
        
        trackingData = new TrackingData();
    }
    public string ReturnJsonString()
    {
        return JsonUtility.ToJson(this);
    }
}

[System.Serializable]
public class TrackingData
{
    public ViewaCampaignTracker ViewaCampaignTracker;
    public ViewaCampaignEvents [] ListViewaCampaignEvents;
}

[System.Serializable]
public class ViewaCampaignTracker
{
    public string Screen;
    public string TrackingId;
    public string CloudId;
    public int TriggerId;
    public string ImageUrl;
    public string UserId;
    public string DeviceId;
    public string DeviceType;
    public string DeviceModel;
    public string Description;
    public string IconUrl;
    public long TotalSessionTime;
    public DateTime DateCreated;

    //Added location information
    public string Country;
    public string City;
    public string RegionName;
    public string CountryCode;
    public string Zip;
    public long lat;
    public long lon;
}

[System.Serializable]
public class ViewaCampaignEvents
{
    public string EventType;
    public string EventDescription;
    public DateTime DateCreated;
}


