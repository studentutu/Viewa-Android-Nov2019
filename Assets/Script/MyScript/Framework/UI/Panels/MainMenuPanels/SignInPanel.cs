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

public class SignInPanel : PanelBase
{
    
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
        ACPUnityPlugin.Instnace.trackScreen("SignInPanel");
    }

    protected override void OnDisable()
    {
        base.OnDisable();
    }
    #region Button Listners

    protected override void OnUIButtonClicked(Button a_button)
    {
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
            case "signInButton":
                Debug.Log("Button selected -" + a_button.name);
                //ClearValidation();
                myManager.AddPanel(ePanels.SignInPassword);
                break;
        }
        base.OnUIButtonClicked(a_button);
    }
    #endregion

}
