using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class HelpPanel : PanelBase
{

    //Transform LeftButton;

    protected override void Awake()
    {
        base.Awake();

        //LeftButton = transform.Find("NavigationBarPanel/LeftButton");
        //UnityEngine.UI.Image buttonImage = LeftButton.GetChild(0).GetComponent<UnityEngine.UI.Image>();
        //buttonImage.sprite = AppManager.Instnace.spriteAtlas.GetSprite("TopNav_BackArrow_White");
    }

    protected override void OnEnable()
    {
        base.OnEnable();

        //tracking 
        ACPUnityPlugin.Instnace.trackScreen("Help");
    }

    protected override void OnDisable()
    {
        base.OnDisable();
    }

    protected override void OnUIButtonClicked (UnityEngine.UI.Button a_button)
	{
		base.OnUIButtonClicked (a_button);

		switch (a_button.name) {

		case "Run_Button":
			Debug.Log ("Button selected -" + a_button.name);
            FacebookLogin.LogUserCompletedTutorialEvent();
                AppManager.Instnace.messageBoxManager.ShowGenericPopup ("Tutorial", "Viewa tutorial is coming soon.", "OK");
			break;
		case "Troubleshooting_Button":
			Debug.Log ("Button selected -" + a_button.name);
                myManager.AddPanel (OTPL.UI.ePanels.Troubleshooting_Panel);
			break;
		case "Still_Button":
			Debug.Log ("Button selected -" + a_button.name);
                myManager.AddPanel (OTPL.UI.ePanels.StillDoesntWork_Panel);
			break;
		case "Rate_Button":
			Debug.Log ("Button selected -" + a_button.name);
			AppManager.Instnace.messageBoxManager.ShowRateMyApp ();
//			Application.OpenURL("http://viewa.com/");
			break;
		case "feedback_Button":
			Debug.Log ("Button selected -" + a_button.name);
                myManager.AddPanel (OTPL.UI.ePanels.Feedback_Panel);
			break;
		case "Enquiries_Button":
			Debug.Log ("Button selected -" + a_button.name);
                myManager.AddPanel (OTPL.UI.ePanels.Enquiries_Panel);
			break;
		}
	}

	//private void Update()
	//{
 //       if (Application.platform == RuntimePlatform.Android)
 //       {
 //           if (Input.GetKey(KeyCode.Escape))
 //           {
 //               myManager.BackToPanel(myManager.panelStack.Peek());
 //           }
 //       }
	//}

}