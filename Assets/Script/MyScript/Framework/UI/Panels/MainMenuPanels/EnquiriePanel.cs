using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using OTPL.UI;
using System.Net;
using System.Net.Mail;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

public class EnquiriePanel : PanelBase {

	public InputField nameInputField;
	public InputField companyInputField;
	public InputField contactInputField;
	public InputField descriptionInputField;
	[SerializeField] Text validationText;
	[SerializeField] GameObject validationObj;

    //Transform LeftButton;

	protected override void OnEnable ()
	{
		base.OnEnable ();
		CanvasManager.Instnace.ShowPanelManager (ePanelManager.BottomBarManager);
		UserDataService userDbService = new UserDataService ();
		UserInfo _user = userDbService.GetUser ();
		if (_user != null) {
			nameInputField.text = _user.FirstName + " " + _user.LastName;
            nameInputField.readOnly = true;
            if(!string.IsNullOrEmpty(_user.Email)){
                contactInputField.text = _user.Email;
                contactInputField.readOnly = true;
            }
		}

        //tracking
        ACPUnityPlugin.Instnace.trackScreen("Enquiry");

        //LeftButton = transform.Find("NavigationBarPanel/LeftButton");
        //UnityEngine.UI.Image buttonImage = LeftButton.GetChild(0).GetComponent<UnityEngine.UI.Image>();
        //buttonImage.sprite = AppManager.Instnace.spriteAtlas.GetSprite("TopNav_BackArrow_White");
	}
	protected override void OnDisable () {
		base.OnDisable ();
	}
	protected override void OnUIButtonClicked (UnityEngine.UI.Button a_button)
	{
		switch (a_button.name) {
	
		case "Submit_Button":
			Debug.Log ("Button selected -" + a_button.name);
			if (CheckValidation () > 0) {
                    SendComposedMail();
                    //SendEmail();
				//AppManager.Instnace.messageBoxManager.ShowMessage ("Feedback", "Thanks you for your feedback.", "Ok");    
			}
			break;
		case "LeftButton":
			ClearValidation ();
			break;
		}

		base.OnUIButtonClicked (a_button);
	}

    void SendComposedMail()
    {

        string From = AppManager.Instnace.userEmail;
        string[] To = { "support@viewa.com", AppManager.Instnace.userEmail.Trim() };
        string Subject = "Viewa Enquiry";
        string Body = descriptionInputField.text;
        AppManager.Instnace.socialSharingScript.SendPlainTextMail(Subject, Body, To);
    }

	protected override void Update()
	{
        if (descriptionInputField.isFocused)
        {
            if (descriptionInputField.GetComponent<Outline>())
            {
                Destroy(descriptionInputField.GetComponent<Outline>());
                validationObj.SetActive(false);
                validationText.text = "";
            }
        }
        //Jeetesh - functionlity for Device back button pressed.
        //if (Application.platform == RuntimePlatform.Android)
        //{
        //    if (Input.GetKey(KeyCode.Escape))
        //    {
        //        ClearValidation();
        //        myManager.BackToPanel(myManager.panelStack.Peek());
        //    }
        //}
        base.Update();
	}


	int CheckValidation() {
        
		if (descriptionInputField.text.Length == 0) 
		{
			validationObj.SetActive (true);
			validationText.text = "Description field cannot be empty.";

			if(descriptionInputField.text.Length == 0)
				AddOutlineToInputField (descriptionInputField);
			validationObj.SetActive (true);
			return 0;

		} else {

			return 1;
		}
	}
	void ClearValidation() {

		validationText.text = "";
		descriptionInputField.text = "";

		validationObj.SetActive (false);

		if(descriptionInputField.GetComponent<Outline>()){
			Destroy (descriptionInputField.GetComponent<Outline> ());
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
}
