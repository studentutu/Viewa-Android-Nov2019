using UnityEngine;
using System.Collections;

namespace Viewa3D
{

	public class ChangeScale : MonoBehaviour {

		public Transform Target;
		public Vector3 Amount;
		public bool Animated;
		public float AnimationDuration;

		public Vector3 MaxScale;
		public Vector3 MinScale;

		// Use this for initialization
		void Start () {
		
		}
		
		// Update is called once per frame
		void Update () {

		}

		public void IncreaseScale()
		{
			ChangeScaleBy(Amount);
		}

		public void DecreaseScale()
		{
			ChangeScaleBy (-Amount);
		}

		public void ChangeScaleBy(Vector3 amount)
		{
			Vector3 TargetScale = ClampScaleVector(Target.localScale + amount);
			if (Animated)
			{
				iTween.ScaleTo (Target.gameObject, TargetScale, AnimationDuration);
			}
			else
			{
				Target.localScale = TargetScale;
			}
		}

		private Vector3 ClampScaleVector(Vector3 scale)
		{
			return new Vector3 (Mathf.Clamp(scale.x, MinScale.x, MaxScale.x), 
			           Mathf.Clamp (scale.y, MinScale.y, MaxScale.y), 
			           Mathf.Clamp (scale.z, MinScale.z, MaxScale.z));
		}
	}
}