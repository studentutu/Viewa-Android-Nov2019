using UnityEngine;
using System.Collections;

namespace Viewa3D
{
	public class BlackHoleCenter : MonoBehaviour {
		public GameObject BlackHoleObject;
		public float BlackHoleRotation = 200;
		public float BlackHoleGravity = 50;
		public Vector3 rotationVector = new Vector3(1,0,0);
		public bool FaceBlackHole = true;

		void FixedUpdate(){
			transform.RotateAround (BlackHoleObject.transform.position, rotationVector, BlackHoleRotation * Time.deltaTime);
			if(FaceBlackHole){
				transform.LookAt(BlackHoleObject.transform.position);
			}
			transform.Translate(Vector3.forward*BlackHoleGravity*Time.deltaTime);
		}
	}
}