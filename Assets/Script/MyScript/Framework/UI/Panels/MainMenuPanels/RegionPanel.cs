using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OTPL.UI;

public class RegionPanel : PanelBase {

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
        ACPUnityPlugin.Instnace.trackScreen("Region");
	}

    protected override void OnDisable()
    {
        base.OnDisable();
    }

    protected override void OnUIButtonClicked (UnityEngine.UI.Button a_button)
	{
		base.OnUIButtonClicked (a_button);

		switch (a_button.name) {

		//---Navigation Buttons----------------------------
		case "LeftButton":
			Debug.Log ("Button selected -" + a_button.name);
			break;
		case "RightButton":
			Debug.Log ("Button selected -" + a_button.name);
			break;
			//-------------------------------------------------
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
