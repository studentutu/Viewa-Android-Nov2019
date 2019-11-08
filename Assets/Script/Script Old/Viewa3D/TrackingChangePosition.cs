using UnityEngine;
using System.Collections;

namespace Viewa3D
{
	public class TrackingChangePosition : MonoBehaviour {
		
		public GameObject Target;
		
		public Vector3 PositionTracking;
		public Vector3 PositionLost;
		
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
			//Debug.Log("TrackingChangePosition - Tracking Found");
			SetPositionTo (PositionTracking);
		}
		
		void TrackingLost(bool sticky)
		{
			//Debug.Log("TrackingChangePosition - Tracking Lost");
			SetPositionTo (PositionLost);
		}
		
		protected void SetPositionTo(Vector3 position)
		{
			//Debug.Log("TrackingChangePosition - Setting position to "+position);
			if (this.Animated)
			{
				Hashtable options = new Hashtable();
				options["position"] = position;
				options["islocal"] = true;
				options["time"] = AnimationDuration;
				iTween.MoveTo(Target, options);
			}
			else
			{
				Target.transform.localPosition = position;
			}
		}
	}
}
