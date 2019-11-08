using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnhideGameObjects : MonoBehaviour 
{

	// Use this for initialization
	[ExecuteInEditMode]
	[ContextMenu ("UnhideHidden")]
	void Start () 
	{

		foreach (GameObject obj in Object.FindObjectsOfType(typeof(GameObject)))
		{
			if (obj.transform.parent == null)
			{
				Traverse(obj);
			}
		}
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void Traverse(GameObject obj)
	{
		if (obj.hideFlags == HideFlags.HideInHierarchy) 
		{
			Debug.LogError (obj.name);
			obj.hideFlags = HideFlags.None;
		}
		
		
		foreach (Transform child in obj.transform)
		{
			Traverse(child.gameObject);
		}


	}
}
