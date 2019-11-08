using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OTPL.UI;
using UnityEngine.UI;
using OTPL.Helper;
using System.Net;
using System.Net.Mail;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;


public class FeedbackPanel : PanelBase {

	[SerializeField] InputField fname_InputField;
	[SerializeField] InputField lname_InputField;
	[SerializeField] InputField email_InputField;
	[SerializeField] InputField brief_InputField;
    [SerializeField] InputField area_InputField;
	[SerializeField] Dropdown feedback_Dropdown;
	[SerializeField] Text validationText;
	[SerializeField] GameObject validationObj;

    //Transform LeftButton;

	protected override void Awake ()
	{
		base.Awake ();

        //LeftButton = transform.Find("NavigationBarPanel/LeftButton");
        //UnityEngine.UI.Image buttonImage = LeftButton.GetChild(0).GetComponent<UnityEngine.UI.Image>();
        //buttonImage.sprite = AppManager.Instnace.spriteAtlas.GetSprite("TopNav_BackArrow_White");
	}

	protected override void OnEnable ()
	{
		base.OnEnable ();
		CanvasManager.Instnace.ShowPanelManager (ePanelManager.BottomBarManager);

		UserDataService userDbService = new UserDataService ();
		UserInfo _user = userDbService.GetUser ();
		if (_user != null) {
			fname_InputField.text = _user.FirstName;
			lname_InputField.text = _user.LastName;
			email_InputField.text = _user.Email;
            fname_InputField.readOnly = true;
            lname_InputField.readOnly = true;
            email_InputField.readOnly = true;
            area_InputField.readOnly = true;

            if (!string.IsNullOrEmpty(_user.State) && !string.IsNullOrEmpty(_user.Country))
            {
                area_InputField.text = _user.State + ", " + _user.Country;
            } 
            else if(!string.IsNullOrEmpty(_user.Country)){
                
                area_InputField.text = _user.Country;
            }
		}

        //tracking
        ACPUnityPlugin.Instnace.trackScreen("Feedback");
	}

	protected override void OnDisable ()
	{
        ClearValidation();
		base.OnDisable ();

	}

	protected override void OnUIButtonClicked (UnityEngine.UI.Button a_button)
	{
		switch (a_button.name) {
		case "LeftButton":
			ClearValidation ();
			break;
		case "Submit_Button":
			Debug.Log ("Button selected -" + a_button.name);
			if (CheckValidation () > 0) {
				//SendEmail();
                    SendComposedMail();
				//AppManager.Instnace.messageBoxManager.ShowMessage ("Feedback", "Thanks you for your feedback.", "Ok", ExitFromFeedBack);
			}
			break;
		}
		base.OnUIButtonClicked (a_button);
	}

	void ExitFromFeedBack() {
		this.myManager.BackToPanel (myManager.panelStack.Peek ());
	}

	protected override void Update()
	{
        if (brief_InputField.isFocused)
        {
            if (brief_InputField.GetComponent<Outline>())
            {
                Destroy(brief_InputField.GetComponent<Outline>());
                validationObj.SetActive(false);
                validationText.text = "";
            }
        }

        ////Jeetesh - functionality to go back from Device back button.
        //if (Application.platform == RuntimePlatform.Android)
        //{
        //    if (Input.GetKey(KeyCode.Escape))
        //    {
        //        myManager.BackToPanel(myManager.panelStack.Peek());
        //    }
        //}

        base.Update();
	}

	int CheckValidation() {
		
		if (brief_InputField.text.Length == 0) 
		{
			validationObj.SetActive (true);
			validationText.text = "Description field cannot be empty";
			
			AddOutlineToInputField (brief_InputField);
			validationObj.SetActive (true);
			return 0;
		} else {

			return 1;
		}
	}
	void ClearValidation() {

		validationText.text = "";
		brief_InputField.text = "";

		validationObj.SetActive (false);
		if(brief_InputField.GetComponent<Outline>()){
			Destroy (brief_InputField.GetComponent<Outline> ());
		}
	}
	void AddOutlineToInputField(InputField inputField){

		Color color = Color.white;

		ColorUtility.TryParseHtmlString ("#ff0000", out color);

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

    void SendComposedMail() {

        string From = AppManager.Instnace.userEmail;
        string[] To = {"support@viewa.com",AppManager.Instnace.userEmail.Trim()};
        string Subject = "Viewa Feedback";
        string Body = brief_InputField.text;
        AppManager.Instnace.socialSharingScript.SendPlainTextMail(Subject, Body, To);
    }
}
