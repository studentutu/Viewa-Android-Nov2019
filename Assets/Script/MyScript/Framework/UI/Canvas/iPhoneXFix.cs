using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.iOS;


public class iPhoneXFix: MonoBehaviour {

	RectTransform rectTransform;

	void Start(){

		if ((Screen.width == 1125 && Screen.height == 2436)) {

			rectTransform = gameObject.GetComponent<RectTransform> ();
			rectTransform.offsetMax = new Vector2 (rectTransform.offsetMax.x, (110 + 40f) * -1);	//Top
		}
	}
}