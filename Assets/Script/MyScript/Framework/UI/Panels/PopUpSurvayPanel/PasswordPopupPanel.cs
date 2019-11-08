using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using OTPL.UI;

public class PasswordPopupPanel : PanelBase {


	protected override void Awake()
	{
        base.Awake();
	}

	protected override void OnEnable()
	{
        base.OnEnable();
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
            case "Okay_Button":
                CanvasManager.Instnace.HidePanelManager(ePanelManager.PopupSurvayManager);
                CanvasManager.Instnace.ReturnPanelManager(ePanelManager.MainMenuPanelManager).NavigateToPanel(ePanels.ForgetPassword_Panel);
                break;
            case "Close_Button":
                CanvasManager.Instnace.HidePanelManager(ePanelManager.PopupSurvayManager);
                CanvasManager.Instnace.ReturnPanelManager(ePanelManager.MainMenuPanelManager).NavigateToPanel(ePanels.ForgetPassword_Panel);
                break;
        }
	}
}
