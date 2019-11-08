using UnityEngine;
using System.Collections;

public class FreezeCloseButton : MonoBehaviour {
	
	//GUITexture texture;
	public float m_NativeRatio = 0.6666666666666666666F;
	// Use this for initialization
	
	void Start ()
	{
		//texture = GetComponent<GUITexture> ();

		float currentRatio = (float)Screen.width / (float)Screen.height;
		Vector3 scale = transform.localScale;
		scale.x *= m_NativeRatio / currentRatio;
		transform.localScale = scale;		
		
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
