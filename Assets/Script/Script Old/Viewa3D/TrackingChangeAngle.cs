using UnityEngine;
using System.Collections;

namespace Viewa3D
{
	public class TrackingChangeAngle : MonoBehaviour {

		public GameObject Target;

		public Vector3 AngleTracking;
		public Vector3 AngleLost;

		public bool Animated;
		public float AnimationDuration;

		// Use this for initialization
		void Start () {
			
		}
		
		// Update is called once per frame
		void Update () {
			
		}

		void TrackingFound(bool sticky)
		{
			SetAngleTo (AngleTracking);
		}

		void TrackingLost(bool sticky)
		{
			SetAngleTo (AngleLost);
		}

		protected void SetAngleTo(Vector3 angle)
		{
			if (this.Animated)
			{

				Hashtable options = new Hashtable();
				options["rotation"] = angle;
				options["islocal"] = true;
				options["time"] = AnimationDuration;
				iTween.RotateTo(Target, options);
			}
			else
			{
				Target.transform.localRotation = Quaternion.Euler(angle);
			}
		}
	}
}
	