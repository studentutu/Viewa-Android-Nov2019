using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using OTPL.UI;
using OTPL.Helper;
using System.Text;
using SimpleJSON;
using UnityEngine.Networking;

public class ForgetPasswordPanel : PanelBase {

	[SerializeField] InputField email_InputField;
    [SerializeField] GameObject emailCheckImageGO;

    Sprite crossImage;
    Sprite tickImage;

   
    [SerializeField] GameObject validationObj;

    bool wasEmailFocused;
	bool isValidated;
    Text validationText;
    bool isMailSent;
	//public Text text;

	struct EmailParameter {
		public string Email;
	}


	protected override void Awake ()
	{
		base.Awake ();

        crossImage = AppManager.Instnace.spriteAtlas.GetSprite("Form_Invalid");
        tickImage = AppManager.Instnace.spriteAtlas.GetSprite("Form_Okay");
	}
		
	protected override void OnEnable ()
	{
		base.OnEnable ();
        SetValidationText(false);
//		CanvasManager.Instnace.ShowPanelManager (ePanelManager.BottomBarManager);

//		email_InputField.onEndEdit.AddListener(delegate {ValidateInput(email_InputField); });
        ACPUnityPlugin.Instnace.trackScreen("ForgetPassword");
	}

	protected override void OnDisable ()
	{
		base.OnDisable ();
        isMailSent = false;
//		email_InputField.onEndEdit.RemoveListener(delegate {ValidateInput(email_InputField); });
	}

	protected override void Update()
	{
        if (email_InputField.isFocused && !wasEmailFocused)
        {
            wasEmailFocused = true;
        }
        #if UNITY_EDITOR
        if (Input.GetMouseButtonDown(0))
        {
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
                    validationText.text = "Email address cannot be empty";
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

	int CheckValidation() {

        if (emailCheckImageGO.GetComponent<SVGImage>().sprite == crossImage)
        {
            return 0;
        }
        else if (email_InputField.text.Length == 0)
        {
            SetValidationText(true);
            validationText.text = "Email address cannot be empty";
            //show the cross image here.
            emailCheckImageGO.GetComponent<SVGImage>().sprite = crossImage;
            return 0;
        }
        return 1;
	}

	protected override void OnUIButtonClicked (UnityEngine.UI.Button a_button)
	{
		switch (a_button.name) {
    		case "LeftButton":
    				ClearValidation ();
    				break;
            case "email_Button":
                InputField email_inputField = a_button.GetComponentInParent<InputField>();
                email_inputField.ActivateInputField();
                break;
            case "SubmitButton":

                if (CheckValidation () > 0  && isMailSent == false) {

                    isMailSent = true;
                    EmailParameter emailParameter = new EmailParameter();
                    emailParameter.Email = email_InputField.text;

                    string jsonString = JsonUtility.ToJson(emailParameter);

    				byte[] bodyRaw = Encoding.UTF8.GetBytes (jsonString);

                    AppManager.Instnace.messageBoxManager.ShowPreloaderDefault();
    				WebService.Instnace.Post (AppManager.Instnace.baseURL + "/cloud/ViewaUserActions.aspx/ResetPassword", bodyRaw, "", WebCallback);
                }
    			break;
		}
		base.OnUIButtonClicked (a_button);
	}
		
 	void WebCallback(UnityWebRequest response ){

        isMailSent = false;
		AppManager.Instnace.messageBoxManager.HidePreloader ();

		if (response.responseCode == 408) {
			//User already exists please select a different email id.
            AppManager.Instnace.messageBoxManager.ShowGenericPopup ("Warning", "Request Time Out", "Ok");
			return;
		} else if (response.responseCode == 404) {
            AppManager.Instnace.messageBoxManager.ShowGenericPopup ("Warning", "Entered email id is not registered with Viewa.", "Ok");
			return;
		}


        CanvasManager.Instnace.ShowPanelManager(ePanelManager.PopupSurvayManager);
        CanvasManager.Instnace.ReturnPanelManager(ePanelManager.PopupSurvayManager).NavigateToPanel(ePanels.PasswordPopup_panel);
        ClearValidation();

		//AppManager.Instnace.messageBoxManager.ShowMessage ("Reset Password", "You will receive the information on your entered email in few minutes.", "Ok");

		//var res = JSON.Parse(response.downloadHandler.text);

		//if(res != null) {

		//	var D = res ["d"];
		//	var User = D ["User"].Value;
		//	Debug.Log("User :" + User.ToString ());
		//}
	}
	void ClearValidation() {

        SetValidationText(false);
		validationText.text = "";
        email_InputField.text = "";
        emailCheckImageGO.SetActive(false);
	}

    void SetValidationText(bool value) {
        if(validationText == null) {
            validationText = validationObj.GetComponentInChildren<Text>();
        }
        validationObj.SetActive(value);
    }
}
