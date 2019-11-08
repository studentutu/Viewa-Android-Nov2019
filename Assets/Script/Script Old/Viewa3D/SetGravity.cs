using UnityEngine;
using System.Collections;

namespace Viewa3D
{
	public class SetGravity : MonoBehaviour {

		public Vector3 Gravity;
		// Use this for initialization
		void Start () {
			Physics.gravity = Gravity;
		}
	}
}