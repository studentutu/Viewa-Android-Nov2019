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

public class SignUpAccountPanel : PanelBase
{

    [SerializeField] InputField email_InputField;
    [SerializeField] InputField firstName_InputField;
    [SerializeField] InputField lastName_InputField;
    [SerializeField] InputField phone_InputField;

    [SerializeField] GameObject emailCheckImageGO;
    [SerializeField] GameObject firstNameCheckImageGO;
    [SerializeField] GameObject lastNameCheckImageGO;
    [SerializeField] GameObject phoneCheckImageGO;
    [SerializeField] GameObject validationObj;

    Sprite crossImage;
    Sprite tickImage;
    Text validationText;

    bool wasEmailFocused;
    bool wasFirstNameFocused;
    bool wasLastNameFocused;
    bool wasPhoneFocused;

    protected override void Awake()
    {
        base.Awake();

        crossImage = AppManager.Instnace.spriteAtlas.GetSprite("Form_Invalid");
        tickImage = AppManager.Instnace.spriteAtlas.GetSprite("Form_Okay");
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
        if (email_InputField.isFocused && !wasEmailFocused)
        {
            wasEmailFocused = true;
        }
        else if (firstName_InputField.isFocused && !wasFirstNameFocused)
        {
            wasFirstNameFocused = true;
        }
        else if (lastName_InputField.isFocused && !wasLastNameFocused)
        {
            wasLastNameFocused = true;
        }
        else if (phone_InputField.isFocused && !wasPhoneFocused)
        {
            wasPhoneFocused = true;
        }

#if UNITY_EDITOR
        if (Input.GetMouseButtonDown(0)) {
#else
            if(Input.touches.Length > 0) {
#endif
            if (!email_InputField.isFocused && wasEmailFocused)
            {
                if (!emailCheckImageGO.activeSelf)
                    emailCheckImageGO.SetActive(true);

                if (email_InputField.text.Length == 0)
                {
                    SetValidationText(true);
                    validationText.text = "Email cannot be empty.";
                    //show the cross image here.
                    emailCheckImageGO.GetComponent<SVGImage>().sprite = crossImage;
                }
                else if (!TestEmail.IsEmail(email_InputField.text))
                {
                    SetValidationText(true);
                    validationText.text = "The email you entered is incorrect.";
                    emailCheckImageGO.GetComponent<SVGImage>().sprite = crossImage;

                } else {

                    SetValidationText(false);
                    validationText.text = "";
                    emailCheckImageGO.GetComponent<SVGImage>().sprite = tickImage;
                }
                wasEmailFocused = false;
            }
            else if (!firstName_InputField.isFocused && wasFirstNameFocused)
            {
                if (!firstNameCheckImageGO.activeSelf)
                    firstNameCheckImageGO.SetActive(true);

                if (firstName_InputField.text.Length == 0)
                {
                    SetValidationText(true);
                    validationText.text = "First name cannot be empty.";
                    //show the cross image here.
                    firstNameCheckImageGO.GetComponent<SVGImage>().sprite = crossImage;
                } else {
                    SetValidationText(false);
                    validationText.text = "";
                    firstNameCheckImageGO.GetComponent<SVGImage>().sprite = tickImage;
                }
                //call confirm password check
                wasFirstNameFocused = false;
            }
            else if (!lastName_InputField.isFocused && wasLastNameFocused)
            {
                if (!lastNameCheckImageGO.activeSelf)
                    lastNameCheckImageGO.SetActive(true);
                
                if (lastName_InputField.text.Length == 0)
                {
                    SetValidationText(true);
                    validationText.text = "Last name cannot be empty.";
                    //show the cross image here.
                    lastNameCheckImageGO.GetComponent<SVGImage>().sprite = crossImage;
                } else {
                    SetValidationText(false);
                    validationText.text = "";
                    lastNameCheckImageGO.GetComponent<SVGImage>().sprite = tickImage;
                }
                //call confirm password check
                wasLastNameFocused = false;
            }
            else if (!phone_InputField.isFocused && wasPhoneFocused)
            {
                if (!phoneCheckImageGO.activeSelf)
                    phoneCheckImageGO.SetActive(true);
                
                if (phone_InputField.text.Length == 0)
                {
                    SetValidationText(false);
                    validationText.text = "";
                    //show the cross image here.
                    phoneCheckImageGO.SetActive(false);
                }
                else if (phone_InputField.text.Length < 10)
                {
                    SetValidationText(true);
                    validationText.text = "Phone cannot be less than 10 digit.";
                    //show the cross image here.
                    phoneCheckImageGO.GetComponent<SVGImage>().sprite = crossImage;
                }
                else {
                    SetValidationText(false);
                    validationText.text = "";
                    phoneCheckImageGO.GetComponent<SVGImage>().sprite = tickImage;
                }
                //call confirm password check
                wasPhoneFocused = false;
            }
        }

        base.Update();
	} 


#region Button Listners

	protected override void OnUIButtonClicked (Button a_button)
	{
		switch (a_button.name) {
		case "LeftButton":
                ClearValidationAndText();
			//AppManager.Instnace.messageBoxManager.ShowMessageWithTwoButtons ("Alert", "Are you sure you want to exit the sign-up process", "Ok", "Cancel", ExitToSignUpScreen);
			
			break;
        case "nextButton":

            if (CheckValidation() > 0)
            {
                //TODO: Check if the user already exists
                //check all fields are there - there than the validation fields.
                Debug.Log("Button selected -" + a_button.name);
                WebService.Instnace.appUser.FirstName = firstName_InputField.text;
                WebService.Instnace.appUser.LastName = lastName_InputField.text;
                WebService.Instnace.appUser.Email = email_InputField.text;
                WebService.Instnace.appUser.Mobile = phone_InputField.text;
                ACPUnityPlugin.Instnace.trackSocial("Basic", "BasicLogin", "BasicUserLoggedIn");

                string jsonHeader = "Basic " + Convert.ToBase64String(Encoding.UTF8.GetBytes(WebService.Instnace.appUser.Email));
                WebService.Instnace.headerString = jsonHeader;

                    AppManager.Instnace.messageBoxManager.ShowPreloaderDefault ();
                string jsonParameter = JsonUtility.ToJson(WebService.Instnace.appUser);
                WebService.Instnace.headerString = jsonHeader;
                byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonParameter);
                //****************************************************************************
                //                      VerifyUsers
                //****************************************************************************
                WebService.Instnace.Post(AppManager.Instnace.baseURL + "/cloud/ViewaUserActions.aspx/VerifyUsers", bodyRaw, jsonHeader, WebCallback);

            }
            break;
		case "fname_Button":
			InputField fname_inputField = a_button.GetComponentInParent<InputField> ();
			fname_inputField.ActivateInputField ();
			break;
		case "lname_Button":
			InputField lname_inputField = a_button.GetComponentInParent<InputField> ();
			lname_inputField.ActivateInputField ();
			break;
		case "email_Button":
			InputField email_inputField = a_button.GetComponentInParent<InputField> ();
			email_inputField.ActivateInputField ();
			break;
		case "phone_Button":
			InputField phone_inputField = a_button.GetComponentInParent<InputField> ();
			phone_inputField.ActivateInputField ();
			break;
		}

		base.OnUIButtonClicked (a_button);
	}
   
#endregion //Button Listners


	int CheckValidation() {
		
        //check if any of the input fields contains cross image then return 0 otherwise return 1. 

        if (emailCheckImageGO.GetComponent<SVGImage>().sprite == crossImage
            || firstNameCheckImageGO.GetComponent<SVGImage>().sprite == crossImage
            || lastNameCheckImageGO.GetComponent<SVGImage>().sprite == crossImage
            || (phoneCheckImageGO.GetComponent<SVGImage>().sprite == crossImage && phoneCheckImageGO.activeSelf))
        {
            
            return 0;
        }
		
		return 1;
	}

	
	void ClearValidationAndText() {

        SetValidationText(false);
		email_InputField.text = "";
		firstName_InputField.text = "";
		lastName_InputField.text = "";
		phone_InputField.text = "";
		validationText.text = "";

        emailCheckImageGO.SetActive(false);
        firstNameCheckImageGO.SetActive(false);
        lastNameCheckImageGO.SetActive(false);
        phoneCheckImageGO.SetActive(false);
	}

	//public void ExitToSignUpScreen(){
	//	ClearValidationAndText ();
 //       myManager.BackToPanel(ePanels.SignUp_Panel);
		
 //       //AppManager.Instnace.eSocial = eSocialSignUp.Basic;
	//	//--myManager.NavigateToPanel (ePanels.Login_Panel);
	//}

    void WebCallback(UnityWebRequest response)
    {
        AppManager.Instnace.messageBoxManager.HidePreloader();

        if (response.responseCode == 200)
        {
            //User already exists please select a different email id.
            SetValidationText(true);
            validationText.text = "User already exists please select a different email id.";
            return;
        } 
        else {

            ClearValidationAndText();
            myManager.AddPanel(ePanels.SignUpPassword);
        }
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
