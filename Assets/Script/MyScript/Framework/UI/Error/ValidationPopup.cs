using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ValidationPopup : MonoBehaviour {

	[SerializeField] private GameObject prefabValidationPopup;
	private GameObject instantiatedObj;
	RectTransform rectTransform;

	void CreateValidationPopup(InputField inputField, string msg) {

		inputField.transform.SetAsLastSibling ();

		if (instantiatedObj == null) {
			instantiatedObj = GameObject.Instantiate<GameObject> (prefabValidationPopup);

		} else if (!instantiatedObj.activeSelf) {
			
			instantiatedObj.SetActive (true);
		}

		instantiatedObj.transform.SetParent (inputField.transform);
		RectTransform rectTransform = instantiatedObj.GetComponent<RectTransform> ();
		rectTransform.anchoredPosition = new Vector3 (-20, 0, 0);
		rectTransform.anchorMin = new Vector2 (1, 0.5f);
		rectTransform.anchorMax = new Vector2 (1, 0.5f);
		rectTransform.pivot = new Vector2 (1f, 0.5f);

		inputField.selectionColor = Color.red;
		Outline outline = inputField.gameObject.AddComponent<Outline> ();
		outline.effectColor = Color.red;
		outline.effectDistance = new Vector2(1, -1);

		instantiatedObj.transform.SetAsLastSibling ();
		instantiatedObj.GetComponentInChildren<Text> ().text = msg;
		instantiatedObj.gameObject.SetActive (true);
	}

	public void ShowValidationPopup(InputField inputField, string msg) {

		CreateValidationPopup(inputField, msg);
	}

	public void HideValidationPopup (InputField inputField){
	
		if (instantiatedObj != null) {
			inputField.selectionColor = Color.white;
			Destroy (inputField.gameObject.GetComponent<Outline> ());
			instantiatedObj.SetActive (false);
		}
	}
		
}
