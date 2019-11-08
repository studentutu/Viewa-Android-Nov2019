using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using OTPL.UI;

public class SignupPopupPanel : PanelBase {


    public Text headingText;
    public Text descText;
    public Text ButtonText;
    public Sprite facebookImage;
    public Sprite GoogleImage;
    public Button socialButton;
    bool isFacebookPopupLogin, isGooglePopupLogin;

	protected override void Awake()
	{
        base.Awake();
	}

	protected override void OnEnable()
	{
        base.OnEnable();

        if (WebService.Instnace.appUser.FacebookId != null && WebService.Instnace.appUser.FacebookId != "")
        {
            isFacebookPopupLogin = true;
            headingText.text = "This email is already registered via Facebook";
            descText.text = "This email account you entered is already registered through Facebook. You can signin with this account below:";
            ButtonText.text = "Sign up with facebook";
            socialButton.image.sprite = facebookImage;
        }
        else if (WebService.Instnace.appUser.GoogleId != null && WebService.Instnace.appUser.GoogleId != "")
        {
            isGooglePopupLogin = true;
            headingText.text = "This email is already registered via Google";
            descText.text = "This email account you entered is already registered through Google. You can signin with this account below:";
            ButtonText.text = "Sign up with google";
            socialButton.image.sprite = GoogleImage;
        }
	}

	protected override void OnDisable()
	{
        base.OnDisable();
	}

	protected override void OnUIButtonClicked(Button a_button)
	{
        base.OnUIButtonClicked(a_button);
        switch (a_button.name)
        {
            case "Button":
                //Go To facebook or Google
                CanvasManager.Instnace.HidePanelManager(ePanelManager.PopupSurvayManager);
                if(isGooglePopupLogin) {
                    AppManager.Instnace.ReturnGoogleCloudLogin().OnSignIn();
                } else if(isFacebookPopupLogin){
                    FacebookLogin.CallFBLogin();
                }
                break;
            case "Close_Button":
                CanvasManager.Instnace.HidePanelManager(ePanelManager.PopupSurvayManager);
                //CanvasManager.Instnace.ReturnPanelManager(ePanelManager.MainMenuPanelManager).NavigateToPanel(ePanels.SignInPassword);
                break;
        }
	}
}
