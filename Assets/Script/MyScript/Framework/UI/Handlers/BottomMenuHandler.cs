//----------------------------------------------
//       OTPL - Jeetesh
//       created: 5 Dec 2017
//       Copyright © 2017 OTPL
//       Description: Handles the BottomMenu bar.
//----------------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BottomMenuHandler : MonoBehaviour {

    Button[] buttons;

    private void Awake()
    {
        buttons = gameObject.GetComponentsInChildren<Button>();
    }

    /// <summary>
    /// Enable the button delegate OnEnable
    /// </summary>
    private void OnEnable()
    {
        foreach (Button button in buttons)
        {
            button.onClick.AddListener(() => OnBottomMenuButtonClicked(button));
        }
    }

    /// <summary>
    /// Disable the button delegates OnDisable
    /// </summary>
    private void OnDisable()
    {
        foreach (Button button in buttons)
        {
            button.onClick.RemoveAllListeners();
        }
    }

    /// <summary>
    /// bottom menu button clicked function.
    /// </summary>
    /// <param name="a_button">A button.</param>
    void OnBottomMenuButtonClicked(Button a_button) {

        switch (a_button.name)
        {

            case "HubButton":
                Debug.Log("Button selected -" + a_button.name);
                break;
            case "LovedButton":
                Debug.Log("Button selected -" + a_button.name);
                break;
            case "ScanButton":
                Debug.Log("Button selected -" + a_button.name);
                break;
            case "SettingButton":
                Debug.Log("Button selected -" + a_button.name);
                break;
            case "SideMenuButton":
                Debug.Log("Button selected -" + a_button.name);
                break;
        }
    }

    /// <summary>
    /// Enables the bottom menu.
    /// </summary>
    public void EnableBottomMenu(){
        gameObject.SetActive(true);
    }

    /// <summary>
    /// Disables the bottom menu.
    /// </summary>
    public void DisableBottomMenu(){
        gameObject.SetActive(false);
    }
}
