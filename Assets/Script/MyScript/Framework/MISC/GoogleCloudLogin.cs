using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using OTPL.UI;
using OTPL.modal;
using UnityEngine.Networking;
using SimpleJSON;
using System.Linq;
using OTPL.modal;
using System.Threading.Tasks;
using Google;


public class GoogleCloudLogin : MonoBehaviour
{
	public string webClientId = "<your client id here>";

	private GoogleSignInConfiguration configuration;
	private string profilePicUrl;
	#region Google functions
	// Defer the configuration creation until Awake so the web Client ID
	// Can be set via the property inspector in the Editor.
	void Awake() {
		configuration = new GoogleSignInConfiguration {
			WebClientId = webClientId,
            RequestIdToken = true,
            RequestEmail = true,
            RequestProfile = true
		};
	}

	public void OnSignIn() {

        Debug.Log("OnSignIn ------------------------------------------->");
        //AppManager.Instnace.messageBoxManager.ShowPreloaderDefault();
        Loader.Instnace.ShowSocialLoader();
        if (configuration == null)
        {
            configuration = new GoogleSignInConfiguration
            {
                WebClientId = webClientId,
                RequestIdToken = true,
                RequestEmail = true,
                RequestProfile = true
            };
        }
       
		GoogleSignIn.Configuration = configuration;

        GoogleSignIn.Configuration.UseGameSignIn = false;
        GoogleSignIn.Configuration.RequestIdToken = true;
        GoogleSignIn.Configuration.RequestEmail = true;
        GoogleSignIn.Configuration.RequestProfile = true;

		AddStatusText("Calling SignIn");
		GoogleSignIn.DefaultInstance.SignIn().ContinueWith(
			OnAuthenticationFinished);
	}

	public void OnSignOut() {
		AddStatusText("Calling SignOut");
        if(GoogleSignIn.DefaultInstance != null)
		    GoogleSignIn.DefaultInstance.Disconnect();
        if(configuration != null) {
            if (GoogleSignIn.Configuration.RequestIdToken == true)
                GoogleSignIn.DefaultInstance.SignOut();
        }
	}

	public void OnDisconnect() {
		AddStatusText("Calling Disconnect");
		GoogleSignIn.DefaultInstance.Disconnect();
        Loader.Instnace.HideSocialLoader();
		//AppManager.Instnace.messageBoxManager.HidePreloader ();
	}

	internal void OnAuthenticationFinished(Task<GoogleSignInUser> task) {
		if (task.IsFaulted) {
			using (IEnumerator<System.Exception> enumerator =
				task.Exception.InnerExceptions.GetEnumerator()) {
				if (enumerator.MoveNext()) {
					GoogleSignIn.SignInException error =
						(GoogleSignIn.SignInException)enumerator.Current;
                    Loader.Instnace.HideSocialLoader();
                    //AppManager.Instnace.messageBoxManager.HidePreloader();
					AddStatusText("Got Error: " + error.Status + " " + error.Message);
				} else {
					AddStatusText("Got Unexpected Exception?!?" + task.Exception);
                    Loader.Instnace.HideSocialLoader();
					//AppManager.Instnace.messageBoxManager.HidePreloader ();
				}
			}
		} else if(task.IsCanceled) {
			AddStatusText("Canceled");
            Loader.Instnace.HideSocialLoader();
			//AppManager.Instnace.messageBoxManager.HidePreloader ();
		} else  {
			AddStatusText("Welcome: " + task.Result.DisplayName + "!");
			//AppManager.Instnace.messageBoxManager.ShowPreloaderDefaultWithBackGround ();
			LoginProcess (task);
		}
	}

	public void OnSignInSilently() {
		GoogleSignIn.Configuration = configuration;
		GoogleSignIn.Configuration.UseGameSignIn = false;
		GoogleSignIn.Configuration.RequestIdToken = true;
		AddStatusText("Calling SignIn Silently");

		GoogleSignIn.DefaultInstance.SignInSilently()
			.ContinueWith(OnAuthenticationFinished);
		
	}


	public void OnGamesSignIn() {
		GoogleSignIn.Configuration = configuration;
		GoogleSignIn.Configuration.UseGameSignIn = true;
		GoogleSignIn.Configuration.RequestIdToken = false;

		AddStatusText("Calling Games SignIn");

		GoogleSignIn.DefaultInstance.SignIn().ContinueWith(
			OnAuthenticationFinished);
	}

	private List<string> messages = new List<string>();
	void AddStatusText(string text) {
		if (messages.Count == 5) {
			messages.RemoveAt(0);
		}
		messages.Add(text);
		string txt = "";
		foreach (string s in messages) {
			txt += "\n" + s;
		}
	}
	#endregion


	//public void LoginProcessExecuted()
	//{
 //       Debug.Log("LoginProcess ------------------------------------------->");

	//	WebService.Instnace.appUser.Address1 = "";
	//	WebService.Instnace.appUser.Address2 = "";
 //       WebService.Instnace.appUser.Country = "India";
	//	WebService.Instnace.appUser.DateOfBirth = "";
	//	WebService.Instnace.appUser.Email = "vivek@optimalvirtualemployee.com";
	//	WebService.Instnace.appUser.Gender = "";
	//	WebService.Instnace.appUser.FirstName = "Vivek";
	//	WebService.Instnace.appUser.LastName = "Kumar";
	//	WebService.Instnace.appUser.GoogleId = "abcdef";
	//	WebService.Instnace.appUser.Mobile = "";
	//	WebService.Instnace.appUser.Password = "";
	//	WebService.Instnace.appUser.PostCode = "";
	//	WebService.Instnace.appUser.RegionId = AppManager.Instnace.regionId;
	//	WebService.Instnace.appUser.RelationshipStatus = "";
 //       WebService.Instnace.appUser.State = "Delhi";
 //       WebService.Instnace.appUser.Suburb = "NCR";
	//	WebService.Instnace.appUser.LoginInfo = "Google";
	//	WebService.Instnace.appUser.isLoggedIn = 1;

	//	//profilePicUrl = task.Result.ImageUrl.AbsoluteUri;
	//	AppManager.Instnace.isSocialSignInScreen = true;
	//	AppManager.Instnace.eSocial = eSocialSignUp.Social;
	//	string jsonHeader = "Google " + Convert.ToBase64String (Encoding.UTF8.GetBytes (WebService.Instnace.appUser.Email));
	//	WebService.Instnace.headerString = jsonHeader;
	//	string jsonParameter = JsonUtility.ToJson (WebService.Instnace.appUser);
	//	WebService.Instnace.headerString = jsonHeader;
	//	byte[] bodyRaw = Encoding.UTF8.GetBytes (jsonParameter);

 //       Debug.Log("VerifyUsers ------------------------------------------->");
	//	//****************************************************************************
	//	//						VerifyUsers
	//	//****************************************************************************
	//	WebService.Instnace.Post (AppManager.Instnace.baseURL + "/cloud/ViewaUserActions.aspx/VerifyUsers", bodyRaw, jsonHeader, UserCheckCallback);
	//}

    void LoginProcess(Task<GoogleSignInUser> task)
    {
        Debug.Log("LoginProcess ------------------------------------------->");

        WebService.Instnace.appUser.Address1 = "";
        WebService.Instnace.appUser.Address2 = "";
        WebService.Instnace.appUser.Country = AppManager.Instnace.geoLocation.country;
        WebService.Instnace.appUser.DateOfBirth = "";
        WebService.Instnace.appUser.Email = task.Result.Email;
        WebService.Instnace.appUser.Gender = "";
        WebService.Instnace.appUser.FirstName = task.Result.GivenName;
        WebService.Instnace.appUser.LastName = task.Result.FamilyName;
        WebService.Instnace.appUser.GoogleId = task.Result.UserId;
        WebService.Instnace.appUser.Mobile = "";
        WebService.Instnace.appUser.Password = "";
        WebService.Instnace.appUser.PostCode = "";
        WebService.Instnace.appUser.RegionId = AppManager.Instnace.regionId;
        WebService.Instnace.appUser.RelationshipStatus = "";
        WebService.Instnace.appUser.State = AppManager.Instnace.geoLocation.city;
        WebService.Instnace.appUser.Suburb = AppManager.Instnace.geoLocation.regionName;
        WebService.Instnace.appUser.LoginInfo = "Google";
        WebService.Instnace.appUser.isLoggedIn = 1;

        profilePicUrl = task.Result.ImageUrl.AbsoluteUri;
        AppManager.Instnace.isSocialSignInScreen = true;
        AppManager.Instnace.eSocial = eSocialSignUp.Social;
        string jsonHeader = "Google " + Convert.ToBase64String(Encoding.UTF8.GetBytes(WebService.Instnace.appUser.Email));
        WebService.Instnace.headerString = jsonHeader;
        string jsonParameter = JsonUtility.ToJson(WebService.Instnace.appUser);
        WebService.Instnace.headerString = jsonHeader;
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonParameter);

        Debug.Log("VerifyUsers ------------------------------------------->");
        //****************************************************************************
        //                      VerifyUsers
        //****************************************************************************
        WebService.Instnace.Post(AppManager.Instnace.baseURL + "/cloud/ViewaUserActions.aspx/VerifyUsers", bodyRaw, jsonHeader, UserCheckCallback);
    }

	void UserCheckCallback(UnityWebRequest response ){

        Debug.Log("response.downloadHandler.text -------------------------------------------> :" + response.downloadHandler.text);
		
		//user already exist code = 500
		if (response.responseCode == 200) {
			
            var res = JSON.Parse (response.downloadHandler.text);
            RootObject responseModel = JsonUtility.FromJson<RootObject> (response.downloadHandler.text); 
			if (res != null) {
                Debug.Log("Save User  ---------------------------------------> " + res);
				SaveUser (responseModel);
			} else {
				//Jeetesh - Hack - User already exist but we didn't get user data from server.
				Debug.Log("User present but details didn't come from server ------------------------------------->");
				//AppManager.Instnace.messageBoxManager.HidePreloader ();
			}
		} else //if(response.responseCode == 402) 
        {
            UserNotRegistered();
		}
	}
	void SaveUser(RootObject responseModel) {

		D _d = responseModel.d;
		OTPL.modal.User _user = _d.User;

		AppManager.Instnace.userId = _user.Id;
		AppManager.Instnace.regionId = _user.RegionId;
		AppManager.Instnace.userEmail = _user.Email;

        WebService.Instnace.appUser.Email = _user.Email;
        WebService.Instnace.appUser.RegionId = _user.RegionId;
        WebService.Instnace.appUser.FirstName = _user.FirstName;
        WebService.Instnace.appUser.LastName = _user.LastName;
        WebService.Instnace.appUser.Mobile = _user.Mobile;
        WebService.Instnace.appUser.Country = _user.Country;
        WebService.Instnace.appUser.Gender = _user.Gender;
        WebService.Instnace.appUser.interest = _user.interest;




        UserDataService userDbService = new UserDataService();
		//Save Profile Picture
		StartCoroutine(AppManager.Instnace.SavePictureWithUrlAsPNG(profilePicUrl));
        ACPUnityPlugin.Instnace.trackSocial("Google", "GoogleLogin", "GoogleLoginUserLoggedIn");

		//if the user id exist then update user otherwise 
        userDbService.CreateOrReplaceUser (_user.Id, _user.Address1, _user.Address2, _user.Country, _user.DateOfBirth, _user.Email, _user.FirstName, _user.LastName, _user.Gender, _user.Mobile, _user.PostCode, 
				_user.RelationshipStatus, _user.State, _user.Suburb, _user.FacebookId, _user.GoogleId, _user.LinkedinId, _user.Password, _user.PasswordSalt, _user.RegionId, WebService.Instnace.appUser.LoginInfo, WebService.Instnace.appUser.isLoggedIn, _user.DateCreated, _user.interest);
		
        //AppManager.Instnace.messageBoxManager.HidePreloader();
		AppManager.Instnace.GoToScanScreen ();
	}

    void UserNotRegistered()
    {
        ACPUnityPlugin.Instnace.trackSocial("Google", "GoogleSignUp", "GoogleUserSignUp");
        UserParameter User = new UserParameter();
        User.Address1 = "";
        User.Address2 = "";
        User.Country = AppManager.Instnace.geoLocation.country;
        User.Email = WebService.Instnace.appUser.Email;
        User.Gender = "";
        User.FirstName = WebService.Instnace.appUser.FirstName;
        User.LastName = WebService.Instnace.appUser.LastName;
        User.Mobile = WebService.Instnace.appUser.Mobile;
        User.Password = "";
        User.RegionId = AppManager.Instnace.regionId;
        User.LinkedInId = WebService.Instnace.appUser.LinkedInId;
        User.GoogleId = WebService.Instnace.appUser.GoogleId;
        User.FacebookId = WebService.Instnace.appUser.FacebookId;
        User.RelationshipStatus = "";
        User.State = AppManager.Instnace.geoLocation.city;
        User.Suburb = AppManager.Instnace.geoLocation.regionName;
        User.PostCode = AppManager.Instnace.geoLocation.zip;
        User.latitude = AppManager.Instnace.geoLocation.lat;
        User.longitude = AppManager.Instnace.geoLocation.lon;
        User.deviceId = AppManager.Instnace.geoLocation.deviceId;
        User.deviceName = AppManager.Instnace.geoLocation.deviceName;

        WebService.Instnace.isSignUpScreen = true;
        string jsonHeader;
        string jsonParameter = JsonUtility.ToJson(User);
        jsonHeader = WebService.Instnace.headerString;
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonParameter);
        Debug.Log("UserNotRegister --------------------------------------------->");

        WebService.Instnace.Post(AppManager.Instnace.baseURL + "/cloud/ViewaUserActions.aspx/RegisterUser", bodyRaw, jsonHeader, WebCallbackRegister);
    }

    void WebCallbackRegister(UnityWebRequest response)
    {

        var res = JSON.Parse(response.downloadHandler.text);

        if (res != null)
        {
            RootObject responseModel = JsonUtility.FromJson<RootObject>(response.downloadHandler.text);

            D _d = responseModel.d;
            OTPL.modal.User _user = _d.User;
            Debug.Log(response.downloadHandler.text);

            AppManager.Instnace.userId = _user.Id;
            AppManager.Instnace.regionId = _user.RegionId;
            AppManager.Instnace.userEmail = _user.Email;

            WebService.Instnace.appUser.Email = _user.Email;
            WebService.Instnace.appUser.RegionId = _user.RegionId;
            WebService.Instnace.appUser.FirstName = _user.FirstName;
            WebService.Instnace.appUser.LastName = _user.LastName;
            WebService.Instnace.appUser.Mobile = _user.Mobile;
            WebService.Instnace.appUser.Country = _user.Country;
            WebService.Instnace.appUser.Gender = _user.Gender;
            WebService.Instnace.appUser.interest = _user.interest;
            //          AppManager.Instnace.SavePicture (null);

            UserDataService userDbService = new UserDataService();

            userDbService.DeleteAll();
            //if the user id exist then update user otherwise 
            if (userDbService.UserExist(_user.Email) > 0)
            {
                userDbService.CreateOrReplaceUser(_user.Id, _user.Address1, _user.Address2, _user.Country, _user.DateOfBirth, _user.Email, _user.FirstName, _user.LastName, _user.Gender, _user.Mobile, _user.PostCode,
                    _user.RelationshipStatus, _user.State, _user.Suburb, _user.FacebookId, _user.GoogleId, _user.LinkedinId, _user.Password, _user.PasswordSalt, _user.RegionId, WebService.Instnace.appUser.LoginInfo, WebService.Instnace.appUser.isLoggedIn, _user.DateCreated, _user.interest);
            }
            else
            {
                userDbService.CreateOrReplaceUser(_user.Id, _user.Address1, _user.Address2, _user.Country, _user.DateOfBirth, _user.Email, _user.FirstName, _user.LastName, _user.Gender, _user.Mobile, _user.PostCode,
                    _user.RelationshipStatus, _user.State, _user.Suburb, _user.FacebookId, _user.GoogleId, _user.LinkedinId, _user.Password, _user.PasswordSalt, _user.RegionId, WebService.Instnace.appUser.LoginInfo, WebService.Instnace.appUser.isLoggedIn, _user.DateCreated, _user.interest);
            }

            //AppManager.Instnace.messageBoxManager.HidePreloader();
#if UNITY_EDITOR
            AppManager.Instnace.GoToScanScreen();
#else
            //Debug.Log("GoToScanScreen --------------------------------------------->");
            //AppManager.Instnace.GoToScanScreen();
            AppManager.Instnace.messageBoxManager.ShowGenericPopup ("Welcome", "Congratulations! You have successfully registered to Viewa", "OK", AppManager.Instnace.GoToScanScreen);
#endif
        }
    }
		
}