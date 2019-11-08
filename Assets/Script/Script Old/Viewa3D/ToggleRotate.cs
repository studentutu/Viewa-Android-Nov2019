using UnityEngine;
using System.Collections;

namespace Viewa3D
{
	public class ToggleRotate : MonoBehaviour {
		
		public Transform Target;
		public bool Rotating = false;
		public Vector3 RotationSpeed;

		public Viewa3D.TouchRotateAndZoom touchRotateAndZoomScriptComponent; //when toggling rotation we need to cancel any momuntum rotation in the touchRotateAndZoom

		// Use this for initialization
		void Start () 
		{
		}
		
		// Update is called once per frame
		void Update () 
		{
			if (Rotating)
			{
				Target.transform.Rotate (RotationSpeed * Time.deltaTime);
			}
		}
		
		public void OnClick()
		{
			Rotating = !Rotating;
			if(touchRotateAndZoomScriptComponent != null){
				touchRotateAndZoomScriptComponent.disableRotationMomentum();
			}
		}
	}
}
