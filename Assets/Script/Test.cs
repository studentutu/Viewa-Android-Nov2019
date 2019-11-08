using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour {

	public GameObject AugmentationObject;

	// Use this for initialization
	void Start () {

		this.transform.parent = AugmentationObject.transform;
		Quaternion rotation = AugmentationObject.transform.rotation;			
		this.transform.position = AugmentationObject.transform.position + (AugmentationObject.transform.up * 2);
		rotation *= Quaternion.Euler (-90, 0, 0);
		this.transform.rotation = rotation;
	}

}
