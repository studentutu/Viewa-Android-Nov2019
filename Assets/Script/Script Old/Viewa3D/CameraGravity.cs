using UnityEngine;
using System.Collections;

namespace Viewa3D
{
	public class CameraGravity : MonoBehaviour {
	
		public Camera cameraToUpdate;
		public float gravity = 9.8f;
		
		// Use this for initialization
		void Start () {
		
			if (GetComponent<Camera>() == null)
			{
				cameraToUpdate = Camera.main;
			}
		}
		
		// Update is called once per frame
		void Update () {
			Physics.gravity = -(cameraToUpdate.transform.up * gravity);
		}
	}
}
