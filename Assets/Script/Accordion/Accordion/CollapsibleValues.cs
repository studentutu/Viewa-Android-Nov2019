using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ACP;

public class CollapsibleValues : MonoBehaviour {

	Text headingText;
	RectTransform arrowRectTransform;
	Toggle m_Toggle;

	void Awake(){
		this.headingText = this.transform.GetChild(0).GetComponent<Text> ();
		arrowRectTransform = this.transform.GetChild (0).transform.GetChild (0).transform as RectTransform;
		this.m_Toggle = this.gameObject.GetComponent<Toggle>();

		if (this.m_Toggle != null)
		{
			this.m_Toggle.onValueChanged.AddListener(OnValueChanged);
		}
	}

	public void OnValueChanged(bool state)
	{
		//change heading text color
		if (m_Toggle.isOn == true) {
			headingText.color = Utility.HexToColor ("0093FF");
			arrowRectTransform.localRotation = Quaternion.Euler (0, 0, 0);
		} else {
            headingText.color = Utility.HexToColor ("95989A");
			arrowRectTransform.localRotation = Quaternion.Euler (0, 0, -270);
		}
	}
}
