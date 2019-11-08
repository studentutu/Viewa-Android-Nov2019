﻿using System.Collections;
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

public class SignUpPasswordPanel : PanelBase
{
    [SerializeField] InputField password_InputField;
    [SerializeField] InputField confirmpwd_InputField;

    [SerializeField] GameObject passwordCheckImageGO;
    [SerializeField] GameObject confirmpwdCheckImageGO;

    [SerializeField] Sprite showPwdImage;
    [SerializeField] Sprite hidePwdImage;

    [SerializeField] Sprite crossImage;
    [SerializeField] Sprite tickImage;
    [SerializeField] GameObject validationObj;

    Text validationText;

    bool isShowPwdButton;
    bool isShowConfPwdButton;

    bool wasPwdisFocused;
    bool wasConfirmPwdFocused;

    protected override void Awake()
    {
        base.Awake();

        //crossImage = AppManager.Instnace.spriteAtlas.GetSprite("Form_Invalid");
        //tickImage = AppManager.Instnace.spriteAtlas.GetSprite("Form_Okay");
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
        else if (confirmpwd_InputField.isFocused && !wasConfirmPwdFocused)
        {
            wasConfirmPwdFocused = true;
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
             
                if (password_InputField.text.Length == 0){
                    SetValidationText(true);
                    validationText.text = "Password cannot be empty.";
                    //show the cross image here.
                    passwordCheckImageGO.GetComponent<SVGImage>().sprite = crossImage;
                } else {
                    SetValidationText(false);
                    validationText.text = "";
                    passwordCheckImageGO.GetComponent<SVGImage>().sprite = tickImage;
                }
                wasPwdisFocused = false;
            } 
            else if (!confirmpwd_InputField.isFocused && wasConfirmPwdFocused)
            {
                if (!confirmpwdCheckImageGO.activeSelf)
                    confirmpwdCheckImageGO.SetActive(true);
                
                if (confirmpwd_InputField.text.Length == 0){
                    SetValidationText(true);
                    validationText.text = "Confirm password cannot be empty.";
                    //show the cross image here.
                    confirmpwdCheckImageGO.GetComponent<SVGImage>().sprite = crossImage;
                }
                else if (confirmpwd_InputField.text != password_InputField.text) {
                    SetValidationText(true);
                    validationText.text = "Confirm password is not matching with password.";
                    confirmpwdCheckImageGO.GetComponent<SVGImage>().sprite = crossImage;
                } 
                else {
                    SetValidationText(false);
                    validationText.text = "";
                    confirmpwdCheckImageGO.GetComponent<SVGImage>().sprite = tickImage;
                }
                //call confirm password check
                wasConfirmPwdFocused = false;
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
                    a_button.transform.GetChild(0).GetComponent<Image>().sprite = showPwdImage;
                    password_InputField.contentType = InputField.ContentType.Standard;
                    password_InputField.Select();
                }
                else
                {
                    a_button.transform.GetChild(0).GetComponent<Image>().sprite = hidePwdImage;
                    password_InputField.contentType = InputField.ContentType.Password;
                    password_InputField.Select();
                }
                break;

            case "ShowConfirmPwdButton":

                Debug.Log("Button selected -" + a_button.name);
                confirmpwd_InputField.DeactivateInputField();

                if (confirmpwd_InputField.contentType == InputField.ContentType.Password)
                {
                    a_button.transform.GetChild(0).GetComponent<Image>().sprite = showPwdImage;
                    confirmpwd_InputField.contentType = InputField.ContentType.Standard;
                    confirmpwd_InputField.Select();
                }
                else
                {
                    a_button.transform.GetChild(0).GetComponent<Image>().sprite = hidePwdImage;
                    confirmpwd_InputField.contentType = InputField.ContentType.Password;
                    confirmpwd_InputField.Select();
                }
                break;
            case "nextButton":

                //TODO: Check if the user already exists
                //check all fields are there - there than the validation fields.
                Debug.Log("Button selected -" + a_button.name);
                WebService.Instnace.appUser.Password = password_InputField.text;
                ACPUnityPlugin.Instnace.trackSocial("Basic", "BasicLogin", "BasicUserLoggedIn");

                UserParameter User = new UserParameter();
                User.Address1 = "";
                User.Address2 = "";
                User.Country = AppManager.Instnace.geoLocation.country;
                User.Email = WebService.Instnace.appUser.Email;
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

                if (CheckValidation() > 0)
                {
                    Debug.Log("Came inside CheckValidation - SignUpPassword");

                    AppManager.Instnace.messageBoxManager.ShowPreloaderDefault();
                    WebService.Instnace.isSignUpScreen = true;
                    string jsonHeader;
                    string jsonParameter = JsonUtility.ToJson(User);
                    jsonHeader = WebService.Instnace.headerString;
                    byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonParameter);

                    WebService.Instnace.Post(AppManager.Instnace.baseURL + "/cloud/ViewaUserActions.aspx/RegisterUser", bodyRaw, jsonHeader, WebCallback);
                }
                break;
           
        }
        base.OnUIButtonClicked (a_button);
    }

    #endregion //Button Listners


    int CheckValidation()
    {
        
        if (passwordCheckImageGO.GetComponent<SVGImage>().sprite == crossImage
            || confirmpwdCheckImageGO.GetComponent<SVGImage>().sprite == crossImage)
        {
            return 0;
        } 
        else if (password_InputField.text.Length == 0)
        {
            SetValidationText(true);
            validationText.text = "Password cannot be empty.";
            //show the cross image here.
            passwordCheckImageGO.GetComponent<SVGImage>().sprite = crossImage;
            wasPwdisFocused = false;
            return 0;
        } 
        else if(confirmpwd_InputField.text != password_InputField.text) {
            SetValidationText(true);
            validationText.text = "Confirm password is not matching with password.";
            confirmpwdCheckImageGO.GetComponent<SVGImage>().sprite = crossImage;
            wasConfirmPwdFocused = false;
            return 0;
        }
        else if(confirmpwd_InputField.text.Length == 0)
        {
            SetValidationText(true);
            validationText.text = "Confirm password cannot be empty.";
            //show the cross image here.
            confirmpwdCheckImageGO.GetComponent<SVGImage>().sprite = crossImage;
            wasConfirmPwdFocused = false;
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

        if (response.responseCode == 500)
        {
            AppManager.Instnace.messageBoxManager.HidePreloader();
            //User already exists please select a different email id.
            SetValidationText(true);
            validationText.text = "User already exists please select a different email id.";
            return;
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
            WebService.Instnace.appUser.PostCode = _user.PostCode;
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

            FacebookLogin.LogUserCompletedRegistrationEvent();

            AppManager.Instnace.messageBoxManager.HidePreloader();

            AppManager.Instnace.messageBoxManager.ShowGenericPopup ("Welcome", "Congratulations! You have successfully registered to Viewa", "OK");

            AppManager.Instnace.GoToScanScreen();
        }
    }
    void ClearValidationAndText()
    {
        SetValidationText(false);
        password_InputField.text = "";
        confirmpwd_InputField.text = "";
        validationText.text = "";

        passwordCheckImageGO.SetActive(false);
        confirmpwdCheckImageGO.SetActive(false);

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
