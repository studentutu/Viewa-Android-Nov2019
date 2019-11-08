using UnityEngine;
using System.Collections;

namespace Viewa3D
{
	public class SetTransformOnColliderTouch : MonoBehaviour {

		public Collider hitTestCollider;
		public float hitTestDepth = 2000; //how far to cast down for the hit test for rotation. Put 0 to not do a hit test at all

		public GameObject target;
		public bool animated;
		public float animationDuration = 1.0f;

		public bool setPosition = true;
		public Vector3 position = new Vector3(0,0,0);
		public bool setRotation = true;
		public Vector3 rotation = new Vector3(0,0,0);
		public bool setScale = true;
		public Vector3 scale = new Vector3(0,0,0);


		void Update() {
			
			Vector3? touchedPosition = null;
			
			if (Input.GetMouseButtonUp(0)){ //touches are also sent as left mouse clicks
				touchedPosition = Input.mousePosition;
			}
			
			if(touchedPosition != null){
				if(didHitCollider(touchedPosition.Value)){
					colliderDidTouch();
				}
			}				  
		}
		
		private bool didHitCollider(Vector3 pos)
		{
			if((hitTestDepth == 0) || (hitTestCollider == null)){
				return true;
			} else {
				Ray ray = Camera.main.ScreenPointToRay(pos);
				RaycastHit hit;
				if(Physics.Raycast(ray, out hit, hitTestDepth)){
					//if (hitTestCollider.Raycast (ray, out hit, hitTestDepth)) {
					if (hit.collider == hitTestCollider) {
						return true;
					}
				}
				return false;
			}
		}

		public void colliderDidTouch() 
		{
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
