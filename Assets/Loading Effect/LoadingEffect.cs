using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingEffect : MonoBehaviour {

	//Loading effect
	public bool loading;
	public Texture loadingTexture;
	public float size = 70.0f;
	float rotAngle = 0.0f;
	public float rotSpeed = 300.0f;

	void Update () {
		if(loading){
			rotAngle += rotSpeed * Time.deltaTime;
		}	
	}

	void OnGUI() {
		if(loading){
			Vector2 pivot = new Vector2(Screen.width/2, Screen.height/2);
			GUIUtility.RotateAroundPivot(rotAngle%360,pivot);
			GUI.DrawTexture(new Rect ((Screen.width - size)/2 , (Screen.height - size)/2, size, size), loadingTexture); 
		}
	}

}
