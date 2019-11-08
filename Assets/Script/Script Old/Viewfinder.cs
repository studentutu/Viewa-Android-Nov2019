using UnityEngine;
using System.Collections;

public class Viewfinder : MonoBehaviour {
	
	LowPassFilterAccelerometer lowPass;
	public Transform center;
	
	// Use this for initialization
	void Start ()
	{
		lowPass = gameObject.GetComponent<LowPassFilterAccelerometer> ();
	}
	
	// Update is called once per frame
	void Update ()
	{
		Vector3 acc = lowPass.LowPassFilterAccelerometerValue ();
		Vector3 position = center.localPosition;
		position.x = -acc.y * 10.0f;
		position.z = -acc.z * 10.0f;
		center.localPosition = position;
	}
}
