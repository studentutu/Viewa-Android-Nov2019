using UnityEngine;
using System.Collections;

public class LowPassFilterAccelerometer : MonoBehaviour {
	
	public float accelerometerUpdateInterval = 1.0f / 30.0f;
	public float lowPassKernelWidthInSeconds = 0.5f;
	
	private Vector3 lowPassValue;
	private float lowPassFilterFactor;
	
	// Use this for initialization
	void Start ()
	{
		lowPassValue = Input.acceleration;
		lowPassFilterFactor = accelerometerUpdateInterval / lowPassKernelWidthInSeconds;
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	
	}
	
	public Vector3 LowPassFilterAccelerometerValue ()
	{
		Vector3 acc = Input.acceleration;
		lowPassValue.x = Mathf.Lerp (lowPassValue.x, acc.x, lowPassFilterFactor);
		lowPassValue.y = Mathf.Lerp (lowPassValue.y, acc.y, lowPassFilterFactor);
		lowPassValue.z = Mathf.Lerp (lowPassValue.z, acc.z, lowPassFilterFactor);
		return lowPassValue;
	}
}
