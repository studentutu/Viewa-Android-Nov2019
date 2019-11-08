using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using OTPL.UI;
using System.Linq;
using System.Text;
using OTPL.modal;
using SimpleJSON;
using OTPL.Helper;
using UnityEngine.Networking;
using System;
using Firebase.Analytics;


public class SignInPassword : PanelBase
{
    [SerializeField] InputField password_InputField;
    [SerializeField] InputField email_InputField;

    [SerializeField] GameObject passwordCheckImageGO;
    [SerializeField] GameObject emailCheckImageGO;

    [SerializeField] SVGImage PasswordImage;

    [SerializeField] GameObject validationObj;
    [SerializeField] SVGImage BackButtonImage;

    public Sprite showPwdImage;
    public Sprite hidePwdImage;

    public Sprite crossImage;
    public Sprite tickImage;

    Text validationText;

    bool isShowPwdButton;
    bool isShowConfPwdButton;

    bool wasPwdisFocused;
    bool wasEmailFocused;
    int passwordCount;

    protected override void Awake()
    {
        base.Awake();

        //showPwdImage = AppManager.Instnace.spriteAtlas.GetSprite("Form_ShowPassword");
        //hidePwdImage = AppManager.Instnace.spriteAtlas.GetSprite("Form_HidePassword");
        //crossImage = AppManager.Instnace.spriteAtlas.GetSprite("Form_Invalid");
        //tickImage = AppManager.Instnace.spriteAtlas.GetSprite("Form_Okay");

        PasswordImage.sprite = hidePwdImage;
        BackButtonImage.sprite = AppManager.Instnace.spriteAtlas.GetSprite("TopNav_BackArrow_White");
    }

    protected override void OnEnable()
    {
        base.OnEnable();

        SetValidationText(false);

        ACPUnityPlugin.Instnace.trackSocial("Basic", "BasicLogin", "BasicLoginPressed");
        CanvasManager.Instnace.HidePanelManager(ePanelManager.BottomBarManager);
        AppManager.Instnace.eSocial = eSocialSignUp.Basic;
        WebService.Instnace.appUser.LoginInfo = "Basic";
        validationText.text = "";
        ACPUnityPlugin.Instnace.trackScreen("SignUp");
    }

    protected override void OnDisable()
    {
        base.OnDisable();
    }

	protected override void Update()
	{
        if (password_InputField.isFocused && !wasPwdisFocused)
        {
            wasPwdisFocused = true;
        }
        else if (email_InputField.isFocused && !wasEmailFocused)
        {
            wasEmailFocused = true;
        }

#if UNITY_EDITOR
        if (Input.GetMouseButtonDown(0))
        {
#else
            if(Input.touches.Length > 0) {
#endif
            if (!password_InputField.isFocused && wasPwdisFocused)
            {
                if (!passwordCheckImageGO.activeSelf)
                    passwordCheckImageGO.SetActive(true);
                
                if (password_InputField.text.Length == 0)
                {
                    SetValidationText(true);
                    validationText.text = "Password field cannot be empty.";
                    //show the cross image here.
                    passwordCheckImageGO.GetComponent<SVGImage>().sprite = crossImage;
                }
                else
                {
                    SetValidationText(false);
                    validationText.text = "";
                    passwordCheckImageGO.GetComponent<SVGImage>().sprite = tickImage;
                }
                wasPwdisFocused = false;
            }
            else if (!email_InputField.isFocused && wasEmailFocused)
            {
                email_InputField.text = email_InputField.text.Trim();
                if (!emailCheckImageGO.activeSelf)
                    emailCheckImageGO.SetActive(true);
                
                if (email_InputField.text.Length == 0)
                {
                    SetValidationText(true);
                    validationText.text = "Email id cannot be empty.";
                    //show the cross image here.
                    emailCheckImageGO.GetComponent<SVGImage>().sprite = crossImage;
                }
                else if (!TestEmail.IsEmail(email_InputField.text))
                {
                    SetValidationText(true);
                    validationText.text = "The email you entered is incorrect.";
                    emailCheckImageGO.GetComponent<SVGImage>().sprite = crossImage;
                } 
                else
                {
                    SetValidationText(false);
                    validationText.text = "";
                    emailCheckImageGO.GetComponent<SVGImage>().sprite = tickImage;
                }
                //call confirm password check
                wasEmailFocused = false;
            }
        }

        base.Update();
    }

    #region Button Listners

    protected override void OnUIButtonClicked(Button a_button)
    {
        switch (a_button.name)
        {
            case "LeftButton":
                //clear any stuff before moving back.
                break;
            case "ShowPwdButton":

                Debug.Log("Button selected -" + a_button.name);

                if (password_InputField.contentType == InputField.ContentType.Password)
                {
                    a_button.transform.GetChild(0).GetComponent<SVGImage>().sprite = showPwdImage;
                    password_InputField.contentType = InputField.ContentType.Standard;
                    password_InputField.Select();
                }
                else
                {
                    a_button.transform.GetChild(0).GetComponent<SVGImage>().sprite = hidePwdImage;
                    password_InputField.contentType = InputField.ContentType.Password;
                    password_InputField.Select();
                }
                break;
            case "ForgetPassword":
                Debug.Log("Button selected -" + a_button.name);
                //ClearValidation();
                myManager.AddPanel(ePanels.ForgetPassword_Panel);
                //CanvasManager.Instnace.ShowPanelManager(ePanelManager.PopupSurvayManager);
                //CanvasManager.Instnace.ReturnPanelManager(ePanelManager.PopupSurvayManager).NavigateToPanel(ePanels.PasswordPopup_panel);
                break;
            case "nextButton":

                //TODO: Check if the user already exists
                //check all fields are there - there than the validation fields.
                Debug.Log("Button selected -" + a_button.name);
                WebService.Instnace.appUser.Email = email_InputField.text;
                WebService.Instnace.appUser.Password = password_InputField.text;
                ACPUnityPlugin.Instnace.trackSocial("Basic", "BasicLogin", "BasicUserLoggedIn");

                if (CheckValidation() > 0)
                {
                    UserParameter User = new UserParameter();
                    User.Address1 = "";
                    User.Address2 = "";
                    User.Country = AppManager.Instnace.geoLocation.country;
                    User.Email = email_InputField.text;
                    User.Gender = "";
                    User.FirstName = WebService.Instnace.appUser.FirstName;
                    User.LastName = WebService.Instnace.appUser.LastName;
                    User.Mobile = WebService.Instnace.appUser.Mobile;
                    User.Password = password_InputField.text;
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

                    WebService.Instnace.isLoginScreen = true;
                    AppManager.Instnace.messageBoxManager.ShowPreloaderDefault ();
                    string jsonStr = "Basic " + Convert.ToBase64String(Encoding.UTF8.GetBytes(email_InputField.text + ":" + password_InputField.text));
                    WebService.Instnace.headerString = jsonStr;
                    string jsonParameter = JsonUtility.ToJson(User);
                    byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonParameter);
                    WebService.Instnace.Post(AppManager.Instnace.baseURL + "/cloud/ViewaUserActions.aspx/Login", bodyRaw, jsonStr, WebCallback);
                }
                break;
            case "email_Button":
                InputField email_inputField = a_button.GetComponentInParent<InputField>();
                email_inputField.ActivateInputField();
                break;
        }
        base.OnUIButtonClicked (a_button);
    }

    #endregion //Button Listners


    int CheckValidation()
    {

        if (passwordCheckImageGO.GetComponent<SVGImage>().sprite == crossImage
            || emailCheckImageGO.GetComponent<SVGImage>().sprite == crossImage)
        {

            return 0;
        }

        return 1;
    }

    public void ExitToSignInScreen()
    {
        ClearValidationAndText();
        AppManager.Instnace.eSocial = eSocialSignUp.Basic;
        //--myManager.NavigateToPanel (ePanels.Login_Panel);
    }

 
    void WebCallback(UnityWebRequest response)
    {

        if (response.responseCode == 404)
        {
            AppManager.Instnace.messageBoxManager.HidePreloader();
            //User already exists please select a different email id.
            SetValidationText(true);
            validationText.text = "Email id does not exists.";
            email_InputField.transform.SetAsLastSibling();
            return;
        }
        else if (response.responseCode == 400)
        {
            passwordCount++;
            //User already exists please select a different email id.
            SetValidationText(true);
            validationText.text = "Incorrect Password field.";
            password_InputField.transform.SetAsLastSibling();
            if (passwordCount > 1) {

                UserAlreadyRegistered();
                return;
            } else {
                AppManager.Instnace.messageBoxManager.HidePreloader();
                return;    
            }

        }

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

            ClearValidationAndText();

            Firebase.Analytics.Parameter[] UserInfoParameters = {
                            new Firebase.Analytics.Parameter("FirstName", WebService.Instnace.appUser.FirstName),
                            new Firebase.Analytics.Parameter("Email", WebService.Instnace.appUser.Email),
                            new Firebase.Analytics.Parameter("FirstName", WebService.Instnace.appUser.Email),
                            new Firebase.Analytics.Parameter("FirstName", WebService.Instnace.appUser.State)
                    };


            FirebaseAnalytics.LogEvent(FirebaseAnalytics.EventLogin, FirebaseAnalytics.ParameterItemId, _user.Email);
            FirebaseAnalytics.LogEvent(FirebaseAnalytics.EventLogin, UserInfoParameters);
            FacebookLogin.LogUserCompletedRegistrationEvent();

            //AppManager.Instnace.messageBoxManager.HidePreloader();
#if UNITY_EDITOR
            AppManager.Instnace.GoToScanScreen();
#else
            AppManager.Instnace.GoToScanScreen();
            //AppManager.Instnace.messageBoxManager.ShowMessage ("Welcome", "Congratulations! You have successfully registered to Viewa", "OK", AppManager.Instnace.GoToScanScreen);
#endif
        }
    }


    void UserAlreadyRegistered()
    {

        WebService.Instnace.appUser.Email = email_InputField.text;
        string jsonHeader = "Basic " + Convert.ToBase64String(Encoding.UTF8.GetBytes(WebService.Instnace.appUser.Email));
        WebService.Instnace.headerString = jsonHeader;

        AppManager.Instnace.messageBoxManager.ShowPreloaderDefault();
        string jsonParameter = JsonUtility.ToJson(WebService.Instnace.appUser);
        WebService.Instnace.headerString = jsonHeader;
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonParameter);
        WebService.Instnace.Post(AppManager.Instnace.baseURL + "/cloud/ViewaUserActions.aspx/VerifyUsers", bodyRaw, jsonHeader, WebCallbackVerifyUser);
    }


    void WebCallbackVerifyUser(UnityWebRequest response)
    {
        AppManager.Instnace.messageBoxManager.HidePreloader();

        if (response.responseCode == 200)
        {
            var res = JSON.Parse(response.downloadHandler.text);

            if (res != null)
            {
                RootObject responseModel = JsonUtility.FromJson<RootObject>(response.downloadHandler.text);

                D _d = responseModel.d;
                OTPL.modal.User _user = _d.User;
                Debug.Log(response.downloadHandler.text);

                WebService.Instnace.appUser.FacebookId = _user.FacebookId;
                WebService.Instnace.appUser.GoogleId = _user.GoogleId;
            }

            if((WebService.Instnace.appUser.FacebookId != "" || WebService.Instnace.appUser.GoogleId != "") 
               && (WebService.Instnace.appUser.FacebookId != null || WebService.Instnace.appUser.GoogleId != null)) {

                passwordCount = 0;
                //User already exists please select a different email id.
                CanvasManager.Instnace.ShowPanelManager(ePanelManager.PopupSurvayManager);
                CanvasManager.Instnace.ReturnPanelManager(ePanelManager.PopupSurvayManager).NavigateToPanel(ePanels.SignupPopupPanel);
            } else {
                return;
            }

        }
        if (response.responseCode == 402)
        {
            return;
        }
        return;
    }


    void ClearValidationAndText()
    {
        SetValidationText(false);
        password_InputField.text = "";
        email_InputField.text = "";
        validationText.text = "";

        emailCheckImageGO.SetActive(false);
        passwordCheckImageGO.SetActive(false);
        passwordCount = 0;
        //jeetesh - check if commenting the below will be feasible of not.
        //WebService.Instnace.appUser.Email = "";
        //WebService.Instnace.appUser.FirstName = "";
        //WebService.Instnace.appUser.LastName = "";
        //WebService.Instnace.appUser.Mobile = "";
        //WebService.Instnace.appUser.PostCode = "";
    }
    void SetValidationText(bool value)
    {
        if (validationText == null)
        {
            validationText = validationObj.GetComponentInChildren<Text>();
        }
        validationObj.SetActive(value);
    }

}
