using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OTPL.UI;

public class StillDoesntWorkPanel : PanelBase {

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

        //tracking
        ACPUnityPlugin.Instnace.trackScreen("StillDoesntWork");
	}

    protected override void OnDisable()
    {
        base.OnDisable();
    }
    protected override void OnUIButtonClicked (UnityEngine.UI.Button a_button)
	{
		base.OnUIButtonClicked (a_button);

		switch (a_button.name) {

		case "Email_Button":
			Debug.Log ("Button selected -" + a_button.name);

                SendComposedMail();
                //SendEmail ();
			break;
		}
	}

	//void SendEmail ()
	//{
 //       string email = "info@viewa.com";
	//	string subject = MyEscapeURL("Application Still Dosen't work");
	//	string body = MyEscapeURL("My Body\r\nFull of non-escaped chars");
	//	Application.OpenURL("mailto:" + email + "?subject=" + subject );  //+ "&body=" + body
	//}
	//string MyEscapeURL (string url)
	//{
	//	return WWW.EscapeURL(url).Replace("+","%20");
	//}
 
    void SendComposedMail()
    {
        string From = AppManager.Instnace.userEmail;
        string[] To = {"support@viewa.com" };
        string Subject = "Application Still Dosen't work";
        string Body = "";
        AppManager.Instnace.socialSharingScript.SendPlainTextMail(Subject, Body, To);
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
