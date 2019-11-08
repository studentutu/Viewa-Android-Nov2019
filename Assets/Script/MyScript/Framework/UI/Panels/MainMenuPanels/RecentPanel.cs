using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OTPL.UI;
using UnityEngine.UI;

public class RecentPanel : PanelBase {

	Sprite originalSprite;
	Sprite newSprite;

	protected override void Awake ()
	{
		base.Awake ();
        originalSprite = ACPUnityPlugin.Instnace.originalSprite;
        newSprite = ACPUnityPlugin.Instnace.newSprite; 
	}

	protected override void OnEnable ()
	{
		base.OnEnable ();
		CanvasManager.Instnace.ShowPanelManager (ePanelManager.BottomBarManager);

        //tracking
        ACPUnityPlugin.Instnace.trackScreen("Recent");
	}

    protected override void OnDisable()
    {
        base.OnDisable();
    }

    protected override void OnUIButtonClicked (UnityEngine.UI.Button a_button)
	{
		base.OnUIButtonClicked (a_button);

		switch(a_button.name){
		case "Button1":

			if (a_button.GetComponent<SVGImage>().sprite == newSprite) {
                    a_button.GetComponent<SVGImage> ().sprite = originalSprite;
			} else {
                    a_button.GetComponent<SVGImage> ().sprite = newSprite;
			}
			break;
		case "Button2":

                if (a_button.GetComponent<SVGImage>().sprite == newSprite) {
                    a_button.GetComponent<SVGImage> ().sprite = originalSprite;
			} else {
                    a_button.GetComponent<SVGImage> ().sprite = newSprite;
			}
			break;
		case "Button3":

                if (a_button.GetComponent<SVGImage>().sprite == newSprite) {
                    a_button.GetComponent<SVGImage> ().sprite = originalSprite;
			} else {
                    a_button.GetComponent<SVGImage> ().sprite = newSprite;
			}
			break;
		}
	}

 
}
