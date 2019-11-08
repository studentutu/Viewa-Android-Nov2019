using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OTPL.UI;
using UnityEngine.UI;
using System.Linq;
using OTPL.Helper;
using SimpleJSON;
using UnityEngine.Networking;
using OTPL.modal;
using System.Linq;
using System.Text;
using System;
using VoxelBusters.NativePlugins;
using UnityEngine.EventSystems;
using Google;

public class ProfilePanel : PanelBase {

	[SerializeField] Image _profileImage;
	[SerializeField] InputField firstName_InputField;
	[SerializeField] InputField lastName_InputField;
	[SerializeField] InputField email_InputField;
	[SerializeField] InputField mobile_InputField;
	[SerializeField] InputField country_InputField;
	[SerializeField] InputField postcode_InputField;
    [SerializeField] Dropdown m_dropDownGender;


    [SerializeField] GameObject firstNameCheckImageGO;
    [SerializeField] GameObject lastNameCheckImageGO;
    [SerializeField] GameObject mobileCheckImageGO;
    [SerializeField] GameObject countryCheckImageGO;
    [SerializeField] GameObject postcodeCheckImageGO;

    [SerializeField] GameObject interestGO;

    public bool isComingFromMobileNotification;
    Button [] interestArray;
    

    [SerializeField] Sprite crossImage;
    [SerializeField] Sprite tickImage;
    Texture2D profilePic;

	[SerializeField] Sprite _ProfileSprite;

    bool wasEmailFocused;
    bool wasFirstNameFocused;
    bool wasLastNameFocused;
    bool wasMobileFocused;
    bool wasGenderFocused;
    bool wasCountryFocused;
    bool wasPostCodeFocused;


	bool isEdit;
	UserInfo user;
	bool isValidated;
    bool isProfileBack;
	
	public Image ProfileImage {
	get { 
			return _profileImage;
		}
	set {
			_profileImage = value;
		}
	}

	protected override void Awake ()
	{
		base.Awake ();

        //LeftButton = transform.Find("NavigationBarPanel/LeftButton");
        //UnityEngine.UI.Image buttonImage = LeftButton.GetChild(0).GetComponent<UnityEngine.UI.Image>();
        //buttonImage.sprite = AppManager.Instnace.spriteAtlas.GetSprite("TopNav_BackArrow_White");

        //crossImage = AppManager.Instnace.spriteAtlas.GetSprite("Form_Invalid");
        //tickImage = AppManager.Instnace.spriteAtlas.GetSprite("Form_Okay");

        interestArray = interestGO.GetComponentsInChildren<Button>();
	}

    string GetInterestString(){

        string interestString = "";
        for (int i = 0; i < interestArray.Length; i++){

            var color = interestArray[i].GetComponent<Image>().color;

            if(color == ACP.Utility.HexToColor("0093FF")){
                if(interestString.Length > 0) {
                    interestString = interestString + "," + interestArray[i].gameObject.name;
                } else {
                    interestString = interestArray[i].gameObject.name;
                }
            }
        }
        return interestString;
    }

    string[] ReturnButtonName(string a_interestString)
    {

        string[] breakApart = a_interestString.Split('/');
        string str = breakApart[breakApart.Length - 1];
        string[] breakComma = str.Split(',');
        return breakComma;
    }


	protected override void OnEnable ()
	{
		base.OnEnable ();

		CanvasManager.Instnace.ShowPanelManager (ePanelManager.BottomBarManager);

        profilePic = AppManager.Instnace.LoadPicture(AppManager.Instnace.GetManualProfilePicPath());
        if(profilePic != null){
            profilePic = AppManager.Instnace.LoadPicture(AppManager.Instnace.GetManualProfilePicPath());
        } else {
            profilePic = AppManager.Instnace.LoadPicture(AppManager.Instnace.ReturnProfilePicPath());
        }
       
        if (profilePic != null){
            TextureScale.Bilinear(profilePic, 200, 200);
            _profileImage.sprite = Sprite.Create(profilePic, new Rect(0, 0, 200, 200), new Vector2());
        }
		else
			_profileImage.sprite = _ProfileSprite;

        //Initialize all the interest buttons with default grey color
        for (int j = 0; j < interestArray.Length; j++)
        {
            var color = interestArray[j].GetComponent<Image>().color;
            color = ACP.Utility.HexToColor("E6EBF4");
            interestArray[j].GetComponentInChildren<Text>().color = ACP.Utility.HexToColor("000000");
            interestArray[j].GetComponent<Image>().color = color;
        }
        string[] genderOption = { "Other","Female","Male"};
        foreach (string ans in genderOption)
        {
            m_dropDownGender.options.Add(new Dropdown.OptionData(ans));
        }
        m_dropDownGender.RefreshShownValue();
        //Initialize the user data from database to profile screen.
		UserDataService userDbService = new UserDataService();
		user = userDbService.GetUser ();
		if (user != null) {
			WebService.Instnace.headerString = user.LoginInfo + " " + Convert.ToBase64String (Encoding.UTF8.GetBytes (user.Email + ":" + user.Password));
			WebService.Instnace.appUser.LoginInfo = user.LoginInfo;
			firstName_InputField.text = user.FirstName;
			lastName_InputField.text = user.LastName;
			email_InputField.text = user.Email;
            email_InputField.enabled = false;
			mobile_InputField.text = user.Mobile;
            postcode_InputField.text = user.PostCode;
			if (user.Country == "")
                country_InputField.text = AppManager.Instnace.geoLocation.country;
			else
				country_InputField.text = user.Country;
            if (user.Gender != "") {
//              genderOptionList [gender_dropDown.value].text = user.Gender;
                m_dropDownGender.captionText.text = m_dropDownGender.options[GetDropDownText(user.Gender)].text;
                //gender_dropDown.captionText.text = user.Gender;
                m_dropDownGender.RefreshShownValue();
            } else  {
                m_dropDownGender.captionText.text = "Select";
            }

            m_dropDownGender.onValueChanged.AddListener(delegate {
                DropdownValueChanged(m_dropDownGender);
            });

            //Initialize the user interest buttons with blue color
            string[] interestButtonNameArray = ReturnButtonName(user.interest);

            for (int i = 0; i < interestArray.Length; i++)
            {
                for (int k = 0; k < interestButtonNameArray.Length; k++)
                {
                    if (interestButtonNameArray[k] == interestArray[i].name)
                    {
                        var color = interestArray[i].GetComponent<Image>().color;
                        color = ACP.Utility.HexToColor("0093FF");
                        interestArray[i].GetComponent<Image>().color = color;
                        interestArray[i].GetComponentInChildren<Text>().color = ACP.Utility.HexToColor("FFFFFF");
                    }
                }
            }
        } 

        ACPUnityPlugin.Instnace.trackScreen("UserProfile");

        if(isComingFromMobileNotification) {
            isComingFromMobileNotification = false;
            mobile_InputField.ActivateInputField();
        }
	}

    void DropdownValueChanged(Dropdown a_dropDownGender){

        m_dropDownGender.captionText.text = m_dropDownGender.options[a_dropDownGender.value].text;
    }

    public int GetDropDownText(string text)
    {

        int value = 0;
        switch (text)
        {
            case "Other":
                m_dropDownGender.value = 0;
                value = 0;
                break;
            case "Female":
                m_dropDownGender.value = 1;
                value = 1;
                break;
            case "Male":
                m_dropDownGender.value = 2;
                value = 2;
                break;
        }
        return value;
    }

	protected override void OnDisable ()
	{
		base.OnDisable ();
        m_dropDownGender.ClearOptions();
	}

	protected override void Update()
	{
            if (firstName_InputField.isFocused && !wasFirstNameFocused)
            {
                wasFirstNameFocused = true;
            }
            else if (lastName_InputField.isFocused && !wasLastNameFocused)
            {
                wasLastNameFocused = true;
            }
            else if (mobile_InputField.isFocused && !wasMobileFocused)
            {
                wasMobileFocused = true;
            }
            else if (country_InputField.isFocused && !wasCountryFocused)
            {
                wasCountryFocused = true;
            }
            else if (postcode_InputField.isFocused && !wasPostCodeFocused)
            {
                wasPostCodeFocused = true;
            }

#if UNITY_EDITOR
            if (Input.GetMouseButtonDown(0))
            {
#else
            if(Input.touches.Length > 0) {
#endif

                if (!firstName_InputField.isFocused && wasFirstNameFocused)
                {
                    firstName_InputField.GetComponent<InputField>().enabled = false;

                    if (!firstNameCheckImageGO.activeSelf)
                        firstNameCheckImageGO.SetActive(true);

                    if (firstName_InputField.text.Length == 0)
                    {
                    AppManager.Instnace.messageBoxManager.ShowGenericPopup("Alert", "First name cannot be empty.", "Ok");
                        firstNameCheckImageGO.GetComponent<SVGImage>().sprite = crossImage;
                    }
                    else
                    {
                        firstNameCheckImageGO.GetComponent<SVGImage>().sprite = tickImage;
                    }
                    //call confirm password check
                    wasFirstNameFocused = false;
                }
                else if (!lastName_InputField.isFocused && wasLastNameFocused)
                {
                    lastName_InputField.GetComponent<InputField>().enabled = false;

                    if (!lastNameCheckImageGO.activeSelf)
                        lastNameCheckImageGO.SetActive(true);

                    if (lastName_InputField.text.Length == 0)
                    {
                    AppManager.Instnace.messageBoxManager.ShowGenericPopup("Alert", "Last name cannot be empty.", "Ok");
                        lastNameCheckImageGO.GetComponent<SVGImage>().sprite = crossImage;
                    }
                    else
                    {
                        lastNameCheckImageGO.GetComponent<SVGImage>().sprite = tickImage;
                    }
                    //call confirm password check
                    wasLastNameFocused = false;
                }
                else if (!mobile_InputField.isFocused && wasMobileFocused)
                {
                    mobile_InputField.GetComponent<InputField>().enabled = false;
                    
                    if (!mobileCheckImageGO.activeSelf)
                        mobileCheckImageGO.SetActive(true);


                    if (mobile_InputField.text.Length < 10 && mobile_InputField.text.Length > 0)
                    {
                        AppManager.Instnace.messageBoxManager.ShowGenericPopup("Alert", "Phone cannot be less than 10 digit.", "Ok");
                       
                        //show the cross image here.
                        mobileCheckImageGO.GetComponent<SVGImage>().sprite = crossImage;
                    }
                    else
                    {
                        mobileCheckImageGO.GetComponent<SVGImage>().sprite = tickImage;
                    }
                    //call confirm password check
                    wasMobileFocused = false;
                }
                else if (!country_InputField.isFocused && wasCountryFocused)
                {
                    country_InputField.GetComponent<InputField>().enabled = false;

                    if (!countryCheckImageGO.activeSelf)
                        countryCheckImageGO.SetActive(true);

                    if (country_InputField.text.Length == 0)
                    {
                        countryCheckImageGO.GetComponent<SVGImage>().sprite = crossImage;
                    }
                    else
                    {
                        countryCheckImageGO.GetComponent<SVGImage>().sprite = tickImage;
                    }
                    //call confirm password checks
                    wasCountryFocused = false;
                }
                else if (!postcode_InputField.isFocused && wasPostCodeFocused)
                {
                    postcode_InputField.GetComponent<InputField>().enabled = false;    

                    if (!postcodeCheckImageGO.activeSelf)
                        postcodeCheckImageGO.SetActive(true);

                    if (postcode_InputField.text.Length == 0)
                    {
                        postcodeCheckImageGO.GetComponent<SVGImage>().sprite = crossImage;
                    }
                    else
                    {
                        postcodeCheckImageGO.GetComponent<SVGImage>().sprite = tickImage;
                    }
                    //call confirm password check
                    wasPostCodeFocused = false;
                }
            }

        base.Update();

            ////Jeetesh - functionality for Device back button pressed
            //if (Application.platform == RuntimePlatform.Android)
            //{
            //    if (Input.GetKey(KeyCode.Escape))
            //    {
            //        BackNaviagtion();
            //        myManager.BackToPanel(myManager.panelStack.Peek());
            //    }
            //}
    }

	protected override void OnUIButtonClicked (Button a_button)
	{
	
		switch (a_button.name) {

		case "RightButton":

		    SaveUser();

			break;

		case "Logout_Button":
				Logout ();
			break;

        case "Fname_Button":
                //InputField fname_inputField = a_button.GetComponentInChildren<InputField>();
                firstName_InputField.GetComponent<InputField>().enabled = true;
                firstName_InputField.ActivateInputField ();
			break;
        case "Lname_Button":
                //InputField lname_inputField = a_button.GetComponentInChildren<InputField>();
                lastName_InputField.GetComponent<InputField>().enabled = true;
                lastName_InputField.ActivateInputField();
			break;
        case "Email_Button":
                AppManager.Instnace.messageBoxManager.ShowGenericPopup("Alert", "Registered email cannot be updated.", "Ok");
			break;
        case "Mobile_Button":
                //InputField mobileInputField = a_button.GetComponentInChildren<InputField>();
                mobile_InputField.GetComponent<InputField>().enabled = true;
                mobile_InputField.ActivateInputField();
			break;
        case "Country_Button":
                //InputField countryInputField = a_button.GetComponentInChildren<InputField>();
                country_InputField.GetComponent<InputField>().enabled = true;
                country_InputField.ActivateInputField();
			break;
        case "PostCode_Button":
                //InputField postCodeInputField = a_button.GetComponentInChildren<InputField>();
                postcode_InputField.GetComponent<InputField>().enabled = true;
                postcode_InputField.ActivateInputField();
			break;
        case "Gender_Button":
                //InputField genderInputField = a_button.GetComponentInChildren<InputField>();
                //genderInputField.ActivateInputField();
            break;
		
		case "LeftButton":

                BackNaviagtion();

                break;
        case "ResetPassword_Button":
                myManager.AddPanel(ePanels.ResetPasswordPanel);
            break;
        case "Property":
        case "Books":
        case "Art":
        case "Fitness":
        case "Business":
        case "Travel":
        case "Television":
        case "Lifestyle":
        case "News":
        case "Sports":
        case "Food":
        case "Outdoors":
        case "Fashion":
        case "Film":
        case "Technology":
        case "Health":
        case "Politics":
        case "Pop Culture":

                var color = a_button.GetComponent<Image>().color;

                if (color == ACP.Utility.HexToColor("0093FF")){
                    color = ACP.Utility.HexToColor("E6EBF4");
                    a_button.GetComponentInChildren<Text>().color = ACP.Utility.HexToColor("000000");
                    a_button.GetComponent<Image>().color = color;
                } else {
                    color = ACP.Utility.HexToColor("0093FF");
                    a_button.GetComponentInChildren<Text>().color = ACP.Utility.HexToColor("FFFFFF");
                    a_button.GetComponent<Image>().color = color;
                }
                break;
		}

		base.OnUIButtonClicked (a_button);
	}

    void BackNaviagtion(){

        ClearMobileNotificationIcon();

        ClearValidationAndText();
    }

    void ClearMobileNotificationIcon() {

        BottomBarPanel bottomBarPanel = (BottomBarPanel)CanvasManager.Instnace.ReturnPanelManager(ePanelManager.BottomBarManager).ReturnPanel(ePanels.BottomBarPanel);

        if (WebService.Instnace.appUser.Mobile.Length <= 0)
        {
            bottomBarPanel.mobileNotificationIcon.SetActive(true);
        }
        else
        {
            bottomBarPanel.mobileNotificationIcon.SetActive(false);
        }
    }

    void SaveUser() {

        if (CheckValidation() == 1)
        {
            UserParameter User = new UserParameter();
            User.Address1 = "";
            User.Address2 = "";
            User.Country = country_InputField.text;
            User.Email = email_InputField.text;
            User.FirstName = firstName_InputField.text;
            User.LastName = lastName_InputField.text;
            User.Mobile = mobile_InputField.text;
            User.PostCode = postcode_InputField.text;
            User.RegionId = AppManager.Instnace.regionId;
            User.interest = GetInterestString();
            User.Gender = m_dropDownGender.options[m_dropDownGender.value].text;

            AppManager.Instnace.messageBoxManager.ShowPreloaderDefault();
            string jsonString = JsonUtility.ToJson(User);
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonString);
            string jsonStr = WebService.Instnace.appUser.LoginInfo + " " + Convert.ToBase64String(Encoding.UTF8.GetBytes(email_InputField.text + ":" + ""));

            WebService.Instnace.Post(AppManager.Instnace.baseURL + "/cloud/ViewaUserActions.aspx/UpdateUser", bodyRaw, jsonStr, WebCallback);
        }
        else
        {
            AppManager.Instnace.messageBoxManager.ShowGenericPopup("Alert", "Please complete all required fields.", "OK");
            //AppManager.Instnace.messageBoxManager.ShowMessage("Alert", "Please complete all required fields.", "OK");
        }
    }

	int CheckValidation() {

        //check if any of the input fields contains cross image then return 0 otherwise return 1. 
       
        if (firstNameCheckImageGO.GetComponent<SVGImage>().sprite == crossImage
            || lastNameCheckImageGO.GetComponent<SVGImage>().sprite == crossImage
            || mobile_InputField.text.Length < 10 && mobile_InputField.text.Length > 0)
        {

            return 0;
        }

        return 1;
	}


	void WebCallback(UnityWebRequest response ){

		AppManager.Instnace.messageBoxManager.HidePreloader ();
		var res = JSON.Parse(response.downloadHandler.text);

		if (res != null) {
			var D = res ["d"];

			if (D != null) {
				RootObject responseModel = JsonUtility.FromJson<RootObject> (response.downloadHandler.text); 

				D _d = responseModel.d;
				OTPL.modal.User _user = _d.User;
				Debug.Log (response.downloadHandler.text);

                WebService.Instnace.appUser.Email = _user.Email;
                WebService.Instnace.appUser.RegionId = _user.RegionId;
                WebService.Instnace.appUser.FirstName = _user.FirstName;
                WebService.Instnace.appUser.LastName = _user.LastName;
                WebService.Instnace.appUser.Mobile = _user.Mobile;
                WebService.Instnace.appUser.Country = _user.Country;
                WebService.Instnace.appUser.Gender = _user.Gender;
                WebService.Instnace.appUser.interest = _user.interest;


				UserDataService userDbService = new UserDataService ();


				//Need to update the database here instead of Delete and create new.
				if (userDbService.DeleteAll () == 0) {
					userDbService.CreateUser (_user.Id, _user.Address1, _user.Address2, _user.Country, _user.DateOfBirth, _user.Email, _user.FirstName, _user.LastName, _user.Gender, _user.Mobile, _user.PostCode, 
						_user.RelationshipStatus, _user.State, _user.Suburb, _user.FacebookId, _user.GoogleId, _user.LinkedinId, _user.Password, _user.PasswordSalt, _user.RegionId, WebService.Instnace.appUser.LoginInfo, 1, _user.DateCreated, _user.interest);
				}
				firstName_InputField.text = _user.FirstName;
				lastName_InputField.text = _user.LastName;
				email_InputField.text = _user.Email;
				mobile_InputField.text = _user.Mobile;
				country_InputField.text = _user.Country;
				postcode_InputField.text = _user.PostCode;

                ClearMobileNotificationIcon();
			}
			
            AppManager.Instnace.messageBoxManager.ShowGenericPopup("User Saved", "User profile is successfully Updated", "OK");
            //AppManager.Instnace.messageBoxManager.ShowMessage("User Saved", "User profile is successfully Updated", "OK");

		} else {

            AppManager.Instnace.messageBoxManager.ShowGenericPopup("Not Saved", "User not Saved.", "OK");
		}
	}

    void saveProfileCallback() {
        myManager.BackToPanel(ePanels.SideMenuPanel);
    }

	//void ChangeColor(Color acolor) {
		
		//ColorBlock cb = gender_dropDown.colors;
		//cb.normalColor = acolor;
		//gender_dropDown.colors = cb;
		//cb = editProfilePicButton.colors;
		//cb.normalColor = acolor;
		//editProfilePicButton.colors = cb;
		//foreach (InputField inputfield in inputFields) {
		//	if (inputfield.name != "email") {
		//		ColorBlock colorblock = inputfield.colors;
		//		colorblock.normalColor = acolor;
		//		inputfield.colors = colorblock;
		//	}
		//}
	//}

    void ClearValidationAndText()
    {

        email_InputField.text = "";
        firstName_InputField.text = "";
        lastName_InputField.text = "";
        mobile_InputField.text = "";
        postcode_InputField.text = "";
        country_InputField.text = "";

        firstNameCheckImageGO.SetActive(false);
        lastNameCheckImageGO.SetActive(false);
        mobileCheckImageGO.SetActive(false);
        countryCheckImageGO.SetActive(false);
        postcodeCheckImageGO.SetActive(false);

        for (int i = 0; i < interestArray.Length; i++){

            var color = interestArray[i].GetComponent<Image>().color;
            color = ACP.Utility.HexToColor("E6EBF4");
            interestArray[i].GetComponent<Image>().color = color;
        }
    }


	void Logout()	{
        
		UserDataService userDbService = new UserDataService ();
		UserInfo _user = userDbService.GetUser ();

		if (_user != null && WebService.Instnace.appUser.LoginInfo != null) {
			switch (WebService.Instnace.appUser.LoginInfo) {

				case "Google":
                    //if (AppManager.Instnace.ReturnGoogleCloudLogin() != null)
                    //{
                    //    AppManager.Instnace.ReturnGoogleCloudLogin().OnSignOut();
                    //}
					break;
				case "Facebook":
						FacebookLogin.CallFBLogout();
					break;
				case "Linkedin":
					break;
				case "Basic":
					break;
			}
			AppManager.Instnace.DeleteProfilePic (); 
			//AppManager.Instnace.messageBoxManager.ShowPreloaderDefault (2.0f);

            if (userDbService != null)
            {
                userDbService.DeleteAll();
                Debug.Log("Profile - userDbService.DeleteAll();");
            }
		}
        PlayerPrefs.DeleteAll();
		if(WebService.Instnace.appUser != null)
			WebService.Instnace.DeleteAppUser ();
		AppManager.Instnace.eSocial = eSocialSignUp.Basic;
        myManager.NavigateToPanel (ePanels.GetStarted_Panel);
	}

}
