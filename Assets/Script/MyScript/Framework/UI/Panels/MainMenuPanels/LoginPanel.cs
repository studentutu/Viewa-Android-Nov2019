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

public class LoginPanel : PanelBase
{
	
	[SerializeField] private InputField email_InputField;
	[SerializeField] private InputField password_InputField;

	public GoogleCloudLogin googleCloudLogin;
	public LinkedinLogin linkedinLogin;
	public Text validationText;
	public GameObject validationObj;

	protected override void Awake ()
	{
		base.Awake ();
		FacebookLogin.FacebookInit();
		googleCloudLogin = gameObject.GetComponentInChildren<GoogleCloudLogin> ();
		//linkedinLogin = gameObject.GetComponentInChildren<LinkedinLogin> ();
		validationObj.SetActive (false);
	}

	protected override void OnEnable ()
	{
		base.OnEnable ();
		CanvasManager.Instnace.HidePanelManager (ePanelManager.BottomBarManager);
		AppManager.Instnace.eSocial = eSocialSignUp.Basic;
		AppManager.Instnace.isSocialSignInScreen = false;
		AppManager.Instnace.isLoggedInAndInside = false;

        //tracking
        ACPUnityPlugin.Instnace.trackScreen("Login");
	}

	protected override void OnDisable ()
	{
		base.OnDisable ();
	}
    #region Button Listners

	protected override void OnUIButtonClicked (Button a_button)
	{
		base.OnUIButtonClicked (a_button);

		switch(a_button.name){

		case "fbButton":
			Debug.Log ("Button selected -" + a_button.name);
			ClearValidation ();
			FacebookLogin.CallFBLogin();
			break;
		case "gButton":
			Debug.Log ("Button selected -" + a_button.name);
			ClearValidation ();
			googleCloudLogin.OnSignIn ();
			break;
		case "linButton":
			Debug.Log ("Button selected -" + a_button.name);
			ClearValidation ();
			linkedinLogin.Login ();
			break;
		case "signInButton":
			//check if the the email and password are correct.
			//Check if the user exists in database 
			//and the username and password entered correctly
			//put email not found , password incorrect validation from server
			Debug.Log ("Button selected -" + a_button.name);

//			CanvasManager.Instnace.ReturnPanelManager (ePanelManager.MainMenuPanelManager).NavigateToPanel (ePanels.Scan_Panel);
//			CanvasManager.Instnace.ShowPanelManager (ePanelManager.BottomBarManager);

			if(CheckValidation () > 0) {
				
				WebService.Instnace.isLoginScreen = true;
				AppManager.Instnace.messageBoxManager.ShowPreloaderDefault ();
				string jsonStr = "Basic " + Convert.ToBase64String (Encoding.UTF8.GetBytes (email_InputField.text+":"+password_InputField.text));
				WebService.Instnace.headerString = jsonStr;
				WebService.Instnace.Post (AppManager.Instnace.baseURL + "/cloud/ViewaUserActions.aspx/Login", null, jsonStr, WebCallback);
			}

			break;
		case "createAcButton":
			Debug.Log ("Button selected -" + a_button.name);
			ClearValidation ();
			myManager.NavigateToPanel(ePanels.SignUp_Panel);
			break;
		case "forgetButton":
			Debug.Log ("Button selected -" + a_button.name);
			ClearValidation ();
			myManager.NavigateToPanel(ePanels.ForgetPassword_Panel);
			break;
		}
	}
    #endregion //Button Listners

	void Update()
	{
		if (email_InputField.isFocused) {
			if (email_InputField.GetComponent<Outline> ()) {
				Destroy (email_InputField.GetComponent<Outline> ());
				validationText.text = "";
				password_InputField.transform.SetAsLastSibling ();
				validationObj.SetActive (false);
                email_InputField.shouldHideMobileInput = true;
			} 
			email_InputField.ActivateInputField ();
		} else if (password_InputField.isFocused) {
			if (password_InputField.GetComponent<Outline> ()) {
				Destroy (password_InputField.GetComponent<Outline> ());
				validationText.text = "";
				email_InputField.transform.SetAsLastSibling ();
				validationObj.SetActive (false);
                password_InputField.shouldHideMobileInput = true;
			}
			password_InputField.ActivateInputField ();
		}
	}


	int CheckValidation() {
		
		if (email_InputField.text.Length == 0 && password_InputField.text.Length == 0) {
			validationText.text = "Email & Password fields cannot be empty";
			AddOutlineToInputField (email_InputField);
			AddOutlineToInputField (password_InputField);
			password_InputField.transform.SetAsLastSibling ();
			validationObj.SetActive (true);
			return 0;

		} else if (email_InputField.text.Length == 0) {
			validationText.text = "Email field cannot be empty";
			AddOutlineToInputField (email_InputField);
			email_InputField.transform.SetAsLastSibling ();
			validationObj.SetActive (true);
			return 0;

		} else if (!TestEmail.IsEmail(email_InputField.text)) {//  !email_InputField.text.Contains ("@") || !email_InputField.text.Contains (".")) {
			
			validationText.text = "Not a valid email. Please enter a valid email";
			AddOutlineToInputField (email_InputField);
			email_InputField.transform.SetAsLastSibling ();
			validationObj.SetActive (true);
			return 0;

		} else if (password_InputField.text.Length == 0) {
			
			validationText.text = "Password field cannot be empty.";
			AddOutlineToInputField (password_InputField);
			password_InputField.transform.SetAsLastSibling ();
			validationObj.SetActive (true);
			return 0;

		} else {
			return 1;
		}
	}

	void AddOutlineToInputField(InputField inputField){

		Color color = Color.white;

		ColorUtility.TryParseHtmlString ("#ff0000", out color); //#ff3b1c

		Outline outline = inputField.GetComponent<Outline> ();

		if (outline != null) {
			outline.effectColor = color;
			outline.effectDistance = new Vector2 (1, -1);
		} else {
			outline = inputField.gameObject.AddComponent<Outline> ();
			outline.effectColor = color;
			outline.effectDistance = new Vector2 (1, -1);
		}
	}

	void WebCallback(UnityWebRequest response ){

		if (response.responseCode == 404) {
			AppManager.Instnace.messageBoxManager.HidePreloader ();
			//User already exists please select a different email id.
			validationObj.SetActive (true);
			validationText.text = "Email id does not exist. kindly create an account.";
			AddOutlineToInputField (email_InputField);
			email_InputField.transform.SetAsLastSibling ();
			return;
		} else if (response.responseCode == 400) {
			AppManager.Instnace.messageBoxManager.HidePreloader ();
			//User already exists please select a different email id.
			validationObj.SetActive (true);
			validationText.text = "Incorrect Password field.";
			AddOutlineToInputField (password_InputField);
			password_InputField.transform.SetAsLastSibling ();
			return;
		}

		var res = JSON.Parse(response.downloadHandler.text);

		if (res != null) {
			var D = res ["d"];

			if (D != null) {
				RootObject responseModel = JsonUtility.FromJson<RootObject> (response.downloadHandler.text);
                Debug.Log("Login - response.downloadHandler.text:" + response.downloadHandler.text);
				D _d = responseModel.d;
				OTPL.modal.User _user = _d.User;
				Debug.Log (response.downloadHandler.text);

				AppManager.Instnace.userId = _user.Id;
				AppManager.Instnace.regionId = _user.RegionId;
				AppManager.Instnace.userEmail = _user.Email;
					
				UserDataService userDbService = new UserDataService ();

				//if the user id exist then update user otherwise 
				if (userDbService.UserExist (_user.Email) > 0) {
					userDbService.CreateOrReplaceUser (_user.Id, _user.Address1, _user.Address2, _user.Country, _user.DateOfBirth, _user.Email, _user.FirstName, _user.LastName, _user.Gender, _user.Mobile, _user.PostCode, 
						_user.RelationshipStatus, _user.State, _user.Suburb, _user.FacebookId, _user.GoogleId, _user.LinkedinId, _user.Password, _user.PasswordSalt, _user.RegionId, "Basic", 1, _user.DateCreated, _user.interest);
				} else {
					userDbService.CreateOrReplaceUser (_user.Id, _user.Address1, _user.Address2, _user.Country, _user.DateOfBirth, _user.Email, _user.FirstName, _user.LastName, _user.Gender, _user.Mobile, _user.PostCode, 
						_user.RelationshipStatus, _user.State, _user.Suburb, _user.FacebookId, _user.GoogleId, _user.LinkedinId, _user.Password, _user.PasswordSalt, _user.RegionId, "Basic", 1, _user.DateCreated, _user.interest);
				}

				//activate scan mode
				//			TrackerManager.Instance.GetTracker<ObjectTracker>().Start();
				ClearValidation();

				AppManager.Instnace.GoToScanScreen ();
			}
		} else {

			AppManager.Instnace.messageBoxManager.HidePreloader ();
			validationText.text = "User not exist";
		}
	}

	void ClearValidation() {
		
		validationText.text = "";
		email_InputField.text = "";
		password_InputField.text = "";
		validationObj.SetActive (false);
		if (email_InputField.GetComponent<Outline> ()) {
			Destroy (email_InputField.GetComponent<Outline> ());
		}
		if (password_InputField.GetComponent<Outline> ()) {
			Destroy (password_InputField.GetComponent<Outline> ());
		}
	}
}
