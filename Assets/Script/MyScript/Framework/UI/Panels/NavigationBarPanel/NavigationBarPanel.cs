using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NavigationBarPanel : PanelBase {

    
	protected override void Awake ()
	{
		base.Awake ();
	}

    #region Button Listners

	protected override void OnUIButtonClicked (Button a_button)
	{
		base.OnUIButtonClicked (a_button);

		switch (a_button.name)
		{

		case "LeftButton":
			Debug.Log("Button selected -" + a_button.name);
			break;

		case "RightButton":
			Debug.Log("Button selected -" + a_button.name);
			//CanvasManager.Instnace.ReturnPanelManager(ePanelManager.MainMenuPanelManager).NavigateToPanel(ePanels.Login_Panel);
			break;
		}
	}
    
    #endregion //Button Listners
}

