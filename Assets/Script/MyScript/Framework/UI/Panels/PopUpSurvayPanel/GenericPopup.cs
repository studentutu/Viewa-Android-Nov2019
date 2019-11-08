using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GenericPopup : PanelBase
{
    public Text headingText;
    public Text descText;

    public GameObject Button1;
    public GameObject Button2;

    public delegate void GenericCallBack();
    public GenericCallBack genericCallBack;

    private bool isPopUpVisible;

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

	protected override void Start()
	{
        base.Start();
	}

    public void Show(string heading, string desc, string buttonTxt) {

        if (isPopUpVisible)
            return;

        isPopUpVisible = true;
        //Deactivate the button2
        Button2.SetActive(false);
        headingText.text = heading;
        descText.text = desc;
        Button1.GetComponentInChildren<Text>().text = buttonTxt;
    }

    public void Show(string heading, string desc, string buttonTxt, GenericCallBack a_genericCallBack)
    {
        if (isPopUpVisible)
            return;
        
        isPopUpVisible = true;
        //Deactivate the button2
        Button2.SetActive(false);

        headingText.text = heading;
        descText.text = desc;
        Button1.GetComponentInChildren<Text>().text = buttonTxt;

        genericCallBack = null;
        genericCallBack = a_genericCallBack;
    }

    public void Show(string heading, string desc, string buttonTxt, string buttonTxt2, GenericCallBack a_genericCallBack)
    {
        if (isPopUpVisible)
            return;

        isPopUpVisible = true;
        //Deactivate the button2
        Button2.SetActive(true);

        headingText.text = heading;
        descText.text = desc;
        Button1.GetComponentInChildren<Text>().text = buttonTxt;
        Button2.GetComponentInChildren<Text>().text = buttonTxt2;

        genericCallBack = null;
        genericCallBack = a_genericCallBack;
    }

	protected override void OnUIButtonClicked(Button a_button)
	{
        //base.OnUIButtonClicked(a_button);

        isPopUpVisible = false;
        Debug.Log("GenericPopup - Clicked Button " + a_button.name);

        switch (a_button.name)
        {
            case "Ok_Button":
                //Go To facebook or Google
                if(genericCallBack != null){
                    genericCallBack();
                }
                gameObject.SetActive(false);
                break;
            case "Close_Button":
                gameObject.SetActive(false);
                break;
        }
	}
}
