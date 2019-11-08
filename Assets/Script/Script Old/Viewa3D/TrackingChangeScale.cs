using UnityEngine;
using System.Collections;

namespace Viewa3D
{
	public class TrackingChangeScale : MonoBehaviour {
		
		public GameObject Target;
		
		public Vector3 ScaleTracking;
		public Vector3 ScaleLost;
		
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
			SetScaleTo (ScaleTracking);
		}
		
		void TrackingLost(bool sticky)
		{
			SetScaleTo (ScaleLost);
		}
		
		protected void SetScaleTo(Vector3 scale)
		{
			if (this.Animated)
			{
				Hashtable options = new Hashtable();
				options["scale"] = scale;
				options["islocal"] = true;
				options["time"] = AnimationDuration;
				iTween.ScaleTo(Target, options);
			}
			else
			{
				Target.transform.localScale = scale;
			}
		}
	}
}
