using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vuforia;
using Facebook.Unity;
using OTPL.UI;
using System;
using System.Text;
using OTPL.modal;
using UnityEngine.Networking;
using SimpleJSON;


//FB.API("/me?friends.fields(id,name,first_name,middle_name,last_name,name_format,birthday,age_range,email,gender,installed)", Facebook.HttpMethod.GET, GetFriendsCallback);

public class FacebookLogin
{

    public static string status = "Ready";
    public static string lastResponse = string.Empty;
    static string accessToken;
    public static bool isFacebook;
    public static bool isFacebookRegisterUser;

    public static void FacebookInit()
    {
        FB.Init(OnInitComplete, OnHideUnity);
    }


    private static void OnInitComplete()
    {
        LogAppLaunchEvent();

        status = "Success - Check log for details";
        lastResponse = "Success Response: OnInitComplete Called\n";
        string logMessage = string.Format(
            "OnInitCompleteCalled IsLoggedIn='{0}' IsInitialized='{1}'",
            FB.IsLoggedIn,
            FB.IsInitialized);
        Debug.Log(logMessage);
        if (AccessToken.CurrentAccessToken != null)
        {
            Debug.Log(AccessToken.CurrentAccessToken.ToString());
        }
    }

    private static void OnHideUnity(bool isGameShown)
    {
        status = "Success - Check log for details";
        lastResponse = string.Format("Success Response: OnHideUnity Called {0}\n", isGameShown);
        Debug.Log("Is game shown: " + isGameShown);
    }

    public static void CallFBLogin()
    {
        ACPUnityPlugin.Instnace.trackSocial("Facebook", "FacebookLogin", "FacebookLoginPressed");
        FB.LogInWithReadPermissions(new List<string>() { "public_profile", "email" }, HandleResult);  //"user_friends", "user_birthday"
        Loader.Instnace.ShowSocialLoader();
    }

    public static void CallFBLoginForPublish()
    {
        // It is generally good behavior to split asking for read and publish
        // permissions rather than ask for them all at once.
        //
        // In your own game, consider postponing this call until the moment
        // you actually need it.
        FB.LogInWithPublishPermissions(new List<string>() { "publish_actions" }, HandleResult);
    }

    public static void CallFBLogout()
    {
        if (FB.IsLoggedIn)
        {
            FB.LogOut();
        }
    }

    private static void HandleResult(IResult result)
    {

        if (result == null)
        {
            //AppManager.Instnace.messageBoxManager.HidePreloader();
            Loader.Instnace.HideSocialLoader();
            lastResponse = "Null Response\n";
            //AppManager.Instnace.messageBoxManager.ShowMessage("FB-Login", "HandleResult - Null Response ", "OK");
            Debug.Log(lastResponse);
            return;
        }
        else if (result.ResultDictionary.TryGetValue("access_token", out accessToken))
        {
            Debug.Log("access_token= " + accessToken);
            AppManager.Instnace.accessToken = accessToken;
            //AppManager.Instnace.messageBoxManager.ShowPreloaderDefaultWithBackGround();
        }


        // Some platforms return the empty string instead of null.
        if (!string.IsNullOrEmpty(result.Error))
        {
            //AppManager.Instnace.messageBoxManager.HidePreloader();
            Loader.Instnace.HideSocialLoader();
            status = "Error - Check log for details";
            lastResponse = "Error Response:\n" + result.Error;
            //AppManager.Instnace.messageBoxManager.ShowMessage("FB-Login", "HandleResult - Error Response: " + result.Error, "OK");
        }
        else if (result.Cancelled)
        {
            //AppManager.Instnace.messageBoxManager.HidePreloader();
            Loader.Instnace.HideSocialLoader();
            status = "Cancelled - Check log for details";
            lastResponse = "Cancelled Response:\n" + result.RawResult;
            ACPUnityPlugin.Instnace.trackSocial("Facebook", "FacebookLogin", "FacebookLoginCancelled");
        }
        else if (!string.IsNullOrEmpty(result.RawResult))
        {

            status = "Success";
            lastResponse = "Success Response:\n" + result.RawResult;
            FB.API("me/picture?type=square&height=200&width=200", Facebook.Unity.HttpMethod.GET, pictureCallback);
            //FB.API("/me?fields=id,first_name,last_name,email", HttpMethod.GET, graphCallback);
        }
        else
        {
            lastResponse = "Empty Response\n";
        }

        Debug.Log(result.ToString());
    }

    private static void pictureCallback(IGraphResult result)
    {
        if (result.Error == null)
        {
            AppManager.Instnace.SavePicture(result.Texture, AppManager.Instnace.ReturnProfilePicPath());

        } else {
            //AppManager.Instnace.messageBoxManager.ShowMessage("FB-Login", "pictureCallback - Null Response" + result.Error, "OK");
        }
        //FB.API("/me?fields = birthday, location", HttpMethod.GET, DisplayOtherDetail);
        //FB.API("/me?fields=id,first_name,last_name,gender,email, age_range, address, birthday, link, hometown, education, devices, friends, friendlists", HttpMethod.GET, graphCallback);
        FB.API("/me?fields=id,first_name,last_name,email", HttpMethod.GET, graphCallback);
    }

    private static void DisplayOtherDetail(IGraphResult result)
    {

        if (result.Error == null)
        {

            string birthday;
            string location;

            if (result.ResultDictionary.TryGetValue("birthday", out birthday))
            {
                Debug.Log("birthday= " + birthday.ToString());
            }
            if (result.ResultDictionary.TryGetValue("location", out location))
            {
                Debug.Log("location= " + location.ToString());
            }
        }
    }

    private static void graphCallback(IGraphResult result)
    {
        //		var userInfo = DatabaseService.Instnace.GetUserInfo ();
        Debug.Log("hi= " + result.RawResult);
        string id;
        string firstname;
        string lastname;
        string gender;
        string email;
        string address;
        string birthday;
        string hometown;
        string location;
        string locale;
        string language;
        int minAge;
        Dictionary<string, int> age_range = new Dictionary<string, int>();

        if (result.ResultDictionary.TryGetValue("id", out id))
        {
            Debug.Log("UserId :" + id);
        }
        if (result.ResultDictionary.TryGetValue("first_name", out firstname))
        {
            Debug.Log("firstname= " + firstname.ToString());
        }
        if (result.ResultDictionary.TryGetValue("last_name", out lastname))
        {
            Debug.Log("lastname= " + lastname.ToString());
        }
        if (result.ResultDictionary.TryGetValue("gender", out gender))
        {
            Debug.Log("gender= " + gender.ToString());
        }
        if (result.ResultDictionary.TryGetValue("email", out email))
        {
            Debug.Log("email= " + email.ToString());
        }
        if (result.ResultDictionary.TryGetValue("address", out address))
        {
            Debug.Log("address= " + address.ToString());
        }
        if (result.ResultDictionary.TryGetValue("birthday", out birthday))
        {
            Debug.Log("birthday= " + birthday.ToString());
        }
        if (result.ResultDictionary.TryGetValue("hometown", out hometown))
        {
            Debug.Log("hometown= " + address.ToString());
        }
        if (result.ResultDictionary.TryGetValue("location", out location))
        {
            Debug.Log("location= " + location.ToString());
        }
        if (result.ResultDictionary.TryGetValue("locale", out locale))
        {
            Debug.Log("locale= " + locale.ToString());
        }
        if (result.ResultDictionary.TryGetValue("language", out language))
        {
            Debug.Log("language= " + language.ToString());
        }
        if (result.ResultDictionary.TryGetValue("age_range", out age_range))
        {

            if (age_range.TryGetValue("min", out minAge))
            {
                Debug.Log("min = " + minAge);
            }
        }

        WebService.Instnace.appUser.Address1 = "";
        WebService.Instnace.appUser.Address2 = "";
        WebService.Instnace.appUser.Country = AppManager.Instnace.geoLocation.country;
        WebService.Instnace.appUser.DateOfBirth = "";
        WebService.Instnace.appUser.Email = email;
        WebService.Instnace.appUser.Gender = gender;
        WebService.Instnace.appUser.FirstName = firstname;
        WebService.Instnace.appUser.LastName = lastname;
        WebService.Instnace.appUser.FacebookId = id;
        WebService.Instnace.appUser.Mobile = "";
        WebService.Instnace.appUser.Password = "";
        WebService.Instnace.appUser.PostCode = "";
        WebService.Instnace.appUser.RegionId = AppManager.Instnace.regionId;
        WebService.Instnace.appUser.RelationshipStatus = "";
        WebService.Instnace.appUser.State = AppManager.Instnace.geoLocation.city;
        WebService.Instnace.appUser.Suburb = AppManager.Instnace.geoLocation.regionName;
        WebService.Instnace.appUser.LoginInfo = "Facebook";
        WebService.Instnace.appUser.isLoggedIn = 1;


        AppManager.Instnace.isSocialSignInScreen = true;
        AppManager.Instnace.eSocial = eSocialSignUp.Social;
        string jsonHeader = "Facebook " + Convert.ToBase64String(Encoding.UTF8.GetBytes(WebService.Instnace.appUser.Email));
        WebService.Instnace.headerString = jsonHeader;
        //		AppManager.Instnace.messageBoxManager.ShowPreloaderDefaultWithBackGround ();
        string jsonParameter = JsonUtility.ToJson(WebService.Instnace.appUser);
        WebService.Instnace.headerString = jsonHeader;
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonParameter);
        isFacebook = true;
        //****************************************************************************
        //						VerifyUsers
        //****************************************************************************
        WebService.Instnace.Post(AppManager.Instnace.baseURL + "/cloud/ViewaUserActions.aspx/VerifyUsers", bodyRaw, jsonHeader, null);
    }

    //	private static void DisplayOtherDetail(IGraphResult result){
    //		Image profilePic;
    //		Dictionary<string, int> age_range_dict = new Dictionary<string, int> ();	
    //
    //		if (result.Error == null) {
    //			Debug.Log("birthday :" + result.ResultDictionary ["birthday"]);
    //			Debug.Log("location :" + result.ResultDictionary ["location"]);
    //			//Debug.Log("Age Range :" + result.ResultDictionary ["age_range"]);
    //		}
    //	
    //	}

    //	private static void DisplayOtherDetail(IGraphResult result){
    //		Image profilePic;
    //		Text bday = DialogBirthday;
    //		Text loc = DialogLocation;
    //		Text age = DialogAge;
    //		if (result.Error == null) {
    //			bday.text = "" + result.ResultDictionary ["user_birthday"];
    //			bday.text = "" + result.ResultDictionary ["location"];
    //			bday.text = "" + result.ResultDictionary ["age"];
    //		}
    //	
    //	}

    //	private void graphCallback(IGraphResult result)
    //	{
    //		Debug.Log ("hi= " + result.RawResult);
    //		string id;
    //		string firstname;
    //		string lastname;
    //		string gender;
    //		string email;
    //		if (result.ResultDictionary.TryGetValue ("id", out id)) {
    //			id_base.text = "ID = " + id.ToString ();
    //		}
    //		if (result.ResultDictionary.TryGetValue ("first_name", out firstname)) {
    //			fname_base.text = "firstname= " + firstname.ToString ();
    //		}
    //		if (result.ResultDictionary.TryGetValue ("last_name", out lastname)) {
    //			lname_base.text = "lastname= " + lastname.ToString ();
    //		}
    //		if (result.ResultDictionary.TryGetValue ("gender", out gender)) {
    //			gender_base.text = "gender= " + gender.ToString ();
    //		}
    //		if (result.ResultDictionary.TryGetValue ("email", out email)) {
    //			email_base.text = "email= " + email.ToString ();
    //		}
    //	}CallFBLogin

    public static void UserCheckCallback(UnityWebRequest response)
    {
        

        //user already exist code = 500
        if (response.responseCode == 200)
        {
            var res = JSON.Parse(response.downloadHandler.text);
            RootObject responseModel = JsonUtility.FromJson<RootObject>(response.downloadHandler.text);

            if (res != null)
            {
                //AppManager.Instnace.messageBoxManager.ShowMessage("User already exist", response.downloadHandler.text , "OK");
                SaveUser(responseModel);
            }
            else
            {
                //AppManager.Instnace.messageBoxManager.ShowMessage("FB", "else responseCode 200", "ok");
                //Jeetesh - Hack - User already exist but we didn't get user data from server.
                //AppManager.Instnace.messageBoxManager.HidePreloader();
                Loader.Instnace.HideSocialLoader();
                Debug.Log("User present but details didn't come from server");
                //AppManager.Instnace.messageBoxManager.ShowMessage("FB-Login", "UserCheckCallback - Null Response", "OK");
            }
        }
        else //if (response.responseCode == 402)
        {
            //AppManager.Instnace.messageBoxManager.ShowMessage("FB", "UserNotRegistered - responseCode else", "ok");
            //AppManager.Instnace.messageBoxManager.ShowMessage("User Not registered", response.downloadHandler.text, "OK");
            UserNotRegistered();
        }

    }
    static void SaveUser(RootObject responseModel)
    {

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
        ACPUnityPlugin.Instnace.trackSocial("Facebook", "FacebookLogin", "FacebookUserLoggedIn");

        //if the user id exist then update user otherwise 

        userDbService.CreateOrReplaceUser(_user.Id, _user.Address1, _user.Address2, _user.Country, _user.DateOfBirth, _user.Email, _user.FirstName, _user.LastName, _user.Gender, _user.Mobile, _user.PostCode,
                _user.RelationshipStatus, _user.State, _user.Suburb, _user.FacebookId, _user.GoogleId, _user.LinkedinId, _user.Password, _user.PasswordSalt, _user.RegionId, WebService.Instnace.appUser.LoginInfo, WebService.Instnace.appUser.isLoggedIn, _user.DateCreated, _user.interest);

        AppManager.Instnace.messageBoxManager.ShowPreloaderDefault();
        AppManager.Instnace.GoToScanScreen();
    }
    static void UserNotRegistered()
    {
        //AppManager.Instnace.messageBoxManager.ShowMessage("FB", "UserNotRegistered", "ok");
        isFacebookRegisterUser = true;
        ACPUnityPlugin.Instnace.trackSocial("Facebook", "FacebookSignUp", "FacebookUserSignUp");
        UserParameter User = new UserParameter();
        User.Address1 = "";
        User.Address2 = "";
        User.Country = AppManager.Instnace.geoLocation.country;
        User.Email = WebService.Instnace.appUser.Email;
        User.Gender = WebService.Instnace.appUser.Gender;
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

        Debug.Log("Came inside CheckValidation - SignUpPassword");
        WebService.Instnace.isSignUpScreen = true;
        string jsonHeader;
        string jsonParameter = JsonUtility.ToJson(User);
        jsonHeader = WebService.Instnace.headerString;
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonParameter);

        WebService.Instnace.Post(AppManager.Instnace.baseURL + "/cloud/ViewaUserActions.aspx/RegisterUser", bodyRaw, jsonHeader, null);
    }

    public static void WebCallbackRegister(UnityWebRequest response)
    {
        //AppManager.Instnace.messageBoxManager.ShowMessage("FB", "WebCallbackRegister", "ok");
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

            //          AppManager.Instnace.SavePicture (null);
            WebService.Instnace.appUser.Email = _user.Email;
            WebService.Instnace.appUser.RegionId = _user.RegionId;
            WebService.Instnace.appUser.FirstName = _user.FirstName;
            WebService.Instnace.appUser.LastName = _user.LastName;
            WebService.Instnace.appUser.Mobile = _user.Mobile;
            WebService.Instnace.appUser.Country = _user.Country;
            WebService.Instnace.appUser.Gender = _user.Gender;
            WebService.Instnace.appUser.interest = _user.interest;

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

            LogUserCompletedRegistrationEvent();

            //AppManager.Instnace.messageBoxManager.HidePreloader();

            AppManager.Instnace.messageBoxManager.ShowGenericPopup("Welcome", "Congratulations! You have successfully registered to Viewa", "OK");

            AppManager.Instnace.GoToScanScreen();
        } else {
            //AppManager.Instnace.messageBoxManager.ShowMessage("FB-Login", "WebCallbackRegister - Null Response", "OK");
        }
    }

    #region Custom Event Code 

    /**
 * Include the Facebook namespace via the following code:
 * using Facebook.Unity;
 *
 * For more details, please take a look at:
 * developers.facebook.com/docs/unity/reference/current/FB.LogAppEvent
 */

    public static void LogAppLaunchEvent()
    {
        FB.LogAppEvent(
            "FBSDKAppEventNameActivatedApp"
        );
    }
    public static void LogUserCompletedRegistrationEvent()
    {
        var userDescription = new Dictionary<string, object>();
        userDescription["email_id"] = WebService.Instnace.appUser.Email;
        userDescription["Name"] = WebService.Instnace.appUser.FirstName + "" + WebService.Instnace.appUser.LastName;

        FB.LogAppEvent(
            "FBSDKAppEventNameCompletedRegistration"
        );
    }

    public static void LogUserCompletedTutorialEvent()
    {
        FB.LogAppEvent(
            "FBSDKAppEventNameCompletedTutorial"
        );
    }

    public static void LogUserViewedContentEvent() {

        var contentDescription = new Dictionary<string, object>();
        contentDescription["trigger_id"] = AppManager.Instnace.triggerId;
        contentDescription["tracking_id"] = AppManager.Instnace.trackingId;

        FB.LogAppEvent(
            "FBSDKAppEventNameViewedContent", 0.0f, contentDescription
        );
    }
    #endregion
}
	