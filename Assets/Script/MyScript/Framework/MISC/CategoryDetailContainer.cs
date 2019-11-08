using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CategoryDetailContainer : MonoBehaviour {

	public long id;
	public string cloudId;
	public bool isLoved;
	public string CategoryId;
	public Image history_image;
	public Image history_icon;
	public Text history_description;
	public Button lovedButton;


	public void OnEnable() {

		history_image.preserveAspect = true;
		history_icon.preserveAspect = true;
	}

}





