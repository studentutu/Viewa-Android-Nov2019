using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OTPL.UI;
using UnityEngine.UI;
using UnityEditor;

public class AboutPanel : PanelBase {

    public Text versionNumber;
    //Transform LeftButton;

	protected override void Awake ()
	{
		base.Awake ();

        if(AppManager.Instnace.isLive){
            versionNumber.text = "Version: " + AppManager.Instnace.AppVersionNumber; //Application.version;
        } else {
            versionNumber.text = "Staging Version: " + AppManager.Instnace.AppVersionNumber; //+ "." + "3.4";
        }
         //PlayerSettings.iOS.buildNumber

        //LeftButton = transform.Find("NavigationBarPanel/LeftButton");
        //UnityEngine.UI.Image buttonImage = LeftButton.GetChild(0).GetComponent<UnityEngine.UI.Image>();
        //buttonImage.sprite = AppManager.Instnace.spriteAtlas.GetSprite("TopNav_BackArrow_White");
	}

	protected override void OnEnable ()
	{
		base.OnEnable ();
		CanvasManager.Instnace.ShowPanelManager(ePanelManager.BottomBarManager);
        ACPUnityPlugin.Instnace.trackScreen("About");
	}

    protected override void OnDisable()
    {
        base.OnDisable();
    }

    protected override void OnUIButtonClicked (UnityEngine.UI.Button a_button)
	{
		base.OnUIButtonClicked (a_button);

		switch (a_button.name) {
		
//		case "Enquiries_Button":
//			Debug.Log ("Button selected -" + a_button.name);
//			myManager.NavigateToPanel (ePanels.Enquiries_Panel);
//			break;
		case "Terms_Button":
			Debug.Log ("Button selected -" + a_button.name);
                myManager.AddPanel (ePanels.TermsOfUse_Panel);
			break;
		case "Privacy_Button":
			Debug.Log ("Button selected -" + a_button.name);
                myManager.AddPanel (ePanels.PrivacyPolicy_Panel);
			break;
		}
	}
    //private void Update()
    //{
    //    if (Application.platform == RuntimePlatform.Android)
    //    {
    //        if (Input.GetKey(KeyCode.Escape))
    //        {
    //            myManager.BackToPanel(myManager.panelStack.Peek());
    //        }
    //    }
    //}
}
