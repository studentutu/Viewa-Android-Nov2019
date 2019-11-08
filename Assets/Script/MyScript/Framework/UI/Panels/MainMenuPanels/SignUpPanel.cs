//----------------------------------------------
//       OTPL - Jeetesh
//       created: 5 Dec 2017
//       Copyright © 2017 OTPL
//       Description: Handles all Login related functionality.
//----------------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using OTPL.UI;
using Vuforia;
using SimpleJSON;
using System.Linq;
using System.Text;
using System;
using OTPL.modal;
using OTPL.Helper;
using UnityEngine.Networking;

public class SignUpPanel : PanelBase
{
    public Text validationText;

    protected override void Awake()
    {
        base.Awake();
        FacebookLogin.FacebookInit();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        CanvasManager.Instnace.HidePanelManager(ePanelManager.BottomBarManager);
        //AppManager.Instnace.eSocial = eSocialSignUp.Basic;
        //AppManager.Instnace.isSocialSignInScreen = false;
        AppManager.Instnace.isLoggedInAndInside = false;

        //tracking
        ACPUnityPlugin.Instnace.trackScreen("SignUpPanel");
    }

    protected override void OnDisable()
    {
        base.OnDisable();
    }

    #region Button Listners

    protected override void OnUIButtonClicked(Button a_button)
    {
        base.OnUIButtonClicked(a_button);

        switch (a_button.name)
        {

            case "fbButton":
                Debug.Log("Button selected -" + a_button.name);
                //ClearValidation();
                FacebookLogin.CallFBLogin();
                break;
            case "gButton":
                Debug.Log("Button selected -" + a_button.name);
                //ClearValidation();
                AppManager.Instnace.ReturnGoogleCloudLogin().OnSignIn();
                break;
            case "signUpButton":
                Debug.Log("Button selected -" + a_button.name);
                //ClearValidation();
                myManager.AddPanel(ePanels.SignUpAccount);
                break;
            case "TermsOfService":
                Debug.Log("Button selected -" + a_button.name);
                myManager.AddPanel(ePanels.TermsOfUse_Panel);
                break;
            case "PrivacyPolicy":
                Debug.Log("Button selected -" + a_button.name);
                myManager.AddPanel(ePanels.PrivacyPolicy_Panel);
                break;
            //case "forgetButton":
                //Debug.Log("Button selected -" + a_button.name);
                ////ClearValidation();
                //myManager.NavigateToPanel(ePanels.ForgetPassword_Panel);
                //break;
        }
    }
    #endregion

    void WebCallback(UnityWebRequest response)
    {

        //if (response.responseCode == 404)
        //{
        //    AppManager.Instnace.messageBoxManager.HidePreloader();
        //    //User already exists please select a different email id.
        //    validationObj.SetActive(true);
        //    validationText.text = "Email id does not exist. kindly create an account.";
        //    AddOutlineToInputField(email_InputField);
        //    email_InputField.transform.SetAsLastSibling();
        //    return;
        //}
        //else if (response.responseCode == 400)
        //{
        //    AppManager.Instnace.messageBoxManager.HidePreloader();
        //    //User already exists please select a different email id.
        //    validationObj.SetActive(true);
        //    validationText.text = "Incorrect Password field.";
        //    AddOutlineToInputField(password_InputField);
        //    password_InputField.transform.SetAsLastSibling();
        //    return;
        //}

        var res = JSON.Parse(response.downloadHandler.text);

        if (res != null)
        {
            var D = res["d"];

            if (D != null)
            {
                RootObject responseModel = JsonUtility.FromJson<RootObject>(response.downloadHandler.text);
                Debug.Log("Login - response.downloadHandler.text:" + response.downloadHandler.text);
                D _d = responseModel.d;
                OTPL.modal.User _user = _d.User;
                Debug.Log(response.downloadHandler.text);

                AppManager.Instnace.userId = _user.Id;
                AppManager.Instnace.regionId = _user.RegionId;
                AppManager.Instnace.userEmail = _user.Email;

                UserDataService userDbService = new UserDataService();

                //if the user id exist then update user otherwise 
                if (userDbService.UserExist(_user.Email) > 0)
                {
                    userDbService.CreateOrReplaceUser(_user.Id, _user.Address1, _user.Address2, _user.Country, _user.DateOfBirth, _user.Email, _user.FirstName, _user.LastName, _user.Gender, _user.Mobile, _user.PostCode,
                        _user.RelationshipStatus, _user.State, _user.Suburb, _user.FacebookId, _user.GoogleId, _user.LinkedinId, _user.Password, _user.PasswordSalt, _user.RegionId, "Basic", 1, _user.DateCreated, _user.interest);
                }
                else
                {
                    userDbService.CreateOrReplaceUser(_user.Id, _user.Address1, _user.Address2, _user.Country, _user.DateOfBirth, _user.Email, _user.FirstName, _user.LastName, _user.Gender, _user.Mobile, _user.PostCode,
                        _user.RelationshipStatus, _user.State, _user.Suburb, _user.FacebookId, _user.GoogleId, _user.LinkedinId, _user.Password, _user.PasswordSalt, _user.RegionId, "Basic", 1, _user.DateCreated, _user.interest);
                }


                AppManager.Instnace.GoToScanScreen();
            }
        }
        else
        {

            AppManager.Instnace.messageBoxManager.HidePreloader();
            validationText.text = "User does not exist.";
        }
    }

}
