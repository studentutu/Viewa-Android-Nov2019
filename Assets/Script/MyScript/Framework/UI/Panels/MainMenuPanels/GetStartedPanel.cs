using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OTPL.UI;

public class GetStartedPanel : PanelBase {

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void OnEnable()
    {
        base.OnEnable();

        //tracking
        CanvasManager.Instnace.HidePanelManager(ePanelManager.BottomBarManager);
        ACPUnityPlugin.Instnace.trackScreen("GetStartedScreen");
        CanvasManager.Instnace.ReturnPanelManager(ePanelManager.MainMenuPanelManager).panelStack.Clear();
    }

	protected override void OnDisable()
	{
        Debug.Log("OnDisable called");
        base.OnDisable();
	}
	protected override void OnUIButtonClicked(UnityEngine.UI.Button a_button)
    {
        base.OnUIButtonClicked(a_button);

        switch (a_button.name)
        {

            case "getStartedButton":
                Debug.Log("Button selected -" + a_button.name);
                myManager.AddPanel(ePanels.SignUp_Panel);
                break;
            case "signInButton":
                Debug.Log("Button selected -" + a_button.name);
                myManager.AddPanel(ePanels.SignIn_panel);
                break;
        }
    }
	protected override void Update()
	{
        base.Update();
	}
}
