using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class InputfieldFocused : MonoBehaviour {

	InputfieldSlideScreen slideScreen;
	InputField inputField;


	void Start () {
		slideScreen = gameObject.GetComponentInParent<InputfieldSlideScreen>();
		inputField = transform.GetComponent<InputField>();
		#if UNITY_IOS
		inputField.shouldHideMobileInput=true;
		#endif
	}

	#if UNITY_IOS

	void Update () {
		if (inputField.isFocused)
		{
			// Input field focused, let the slide screen script know about it.
			slideScreen.InputFieldActive = true;
			slideScreen.childRectTransform = transform.GetComponent<RectTransform>();
		}
	}
	#endif
}