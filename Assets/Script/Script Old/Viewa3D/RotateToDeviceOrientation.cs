using UnityEngine;
using System.Collections;

// this script will rotate a sprite to its assigned device orientation rotation
namespace Viewa3D
{

	public class RotateToDeviceOrientation : MonoBehaviour {

		public float portraitRotation = 0;
		public float portraitUpsideDownRotation = 180;
		public float landscapeLeftRotation = -90;
		public float landscapeRightRotation = 90;

		// Use this for initialization
		void Start () {
		}
		
		// Update is called once per frame
		void Update () {
		
			DeviceOrientation orientation;
			//if in emulator simulate this
			orientation = Input.deviceOrientation;
			//Debug.Log ("Device Orientation = "+orientation);
			float rotation = portraitRotation; //default to portrait
			switch(orientation){
				case DeviceOrientation.PortraitUpsideDown: rotation = portraitUpsideDownRotation; break;
				case DeviceOrientation.LandscapeLeft: rotation = landscapeLeftRotation; break;
				case DeviceOrientation.LandscapeRight: rotation = landscapeRightRotation; break;
			}
			gameObject.transform.localRotation = Quaternion.identity;
			gameObject.transform.Rotate(new Vector3(0,0,rotation));
			//using iTween here causes issues when switching from tracking/snapTo
		}
	}
}