using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI.Extensions;
using UnityEngine.UI;

public class ImageGalleryController : HorizontalScrollSnap {


	internal override void Start ()
	{
		base.Start ();

        transitionSpeed = 10.5f;
	}

	void OnDisable() {

		for (int i = 0; i < ChildObjects.Length; i++) {
			Destroy (ChildObjects [i]);
		}
	}

	internal override void Update ()
	{
		base.Update ();
	}


}
