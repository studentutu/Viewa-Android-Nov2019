using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OTPL.UI;

public class MusicResponsePanel : PanelBase {

	protected override void Awake ()
	{
		base.Awake ();
	}

	protected override void OnEnable ()
	{
		base.OnEnable ();
		CanvasManager.Instnace.ShowPanelManager (ePanelManager.BottomBarManager);

        //tracking
        ACPUnityPlugin.Instnace.trackScreen("MusicResponse");
	}
	protected override void OnUIButtonClicked (UnityEngine.UI.Button a_button)
	{
		base.OnUIButtonClicked (a_button);

		switch (a_button.name) {
	
		case "LeftNavButton":
			Debug.Log ("Button selected -" + a_button.name);
			break;
		case "PauseButton":
			Debug.Log ("Button selected -" + a_button.name);
			break;
		case "RightNavButton":
			Debug.Log ("Button selected -" + a_button.name);
			break;
		}
	}

   
}
