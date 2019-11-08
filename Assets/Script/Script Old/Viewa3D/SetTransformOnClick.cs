using UnityEngine;
using System.Collections;

namespace Viewa3D
{
	public class SetTransformOnClick : MonoBehaviour {

		public GameObject target;
		public bool animated;
		public float animationDuration = 1.0f;

		public bool setPosition = true;
		public Vector3 position = new Vector3(0,0,0);
		public bool setRotation = true;
		public Vector3 rotation = new Vector3(0,0,0);
		public bool setScale = true;
		public Vector3 scale = new Vector3(0,0,0);

		public void OnClick() {
			if(target){
				if(animated){
					if(setPosition){
						Hashtable options = new Hashtable();
						options["position"] = position;
						options["islocal"] = true;
						options["time"] = animationDuration;
						iTween.MoveTo(target, options);
					}
					if(setRotation){
						Hashtable options = new Hashtable();
						options["rotation"] = rotation;
						options["islocal"] = true;
						options["time"] = animationDuration;
						iTween.RotateTo(target, options);
					}
					if(setScale){
						Hashtable options = new Hashtable();
						options["scale"] = scale;
						options["islocal"] = true;
						options["time"] = animationDuration;
						iTween.ScaleTo(target, options);
					}
				} else {
					if(setPosition){
						target.transform.localPosition = position;
					}
					if(setRotation){
						target.transform.localRotation = Quaternion.Euler(rotation);
					}
					if(setScale){
						target.transform.localScale = scale;
					}
				}
			}
		}
	}
}
