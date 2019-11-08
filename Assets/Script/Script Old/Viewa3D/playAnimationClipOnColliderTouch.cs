using UnityEngine;
using System.Collections;

namespace Viewa3D
{
	public class playAnimationClipOnColliderTouch : MonoBehaviour {

		public Animation AnimationComponent;
		public AnimationClip animationClip;
		public Collider hitTestCollider;
		public float hitTestDepth = 2000; //how far to cast down for the hit test for rotation. Put 0 to not do a hit test at all

		void Update() {

			Vector3? touchedPosition = null;

			//touch handling on mobile device
			if (Input.touchCount > 0) 
			{       
				if(Input.touchCount == 1)
				{ // single finger 
					//touchedPosition = blah
				}
			}

			if (Input.GetMouseButtonUp(0)){
				touchedPosition = Input.mousePosition;
			}

			if(touchedPosition != null){
				if(didHitCollider(touchedPosition.Value)){


					if(animationClip != null){
						Debug.LogError("triggering animation clip " + animationClip);
						AnimationComponent.RemoveClip(animationClip);
						AnimationComponent.AddClip(animationClip,animationClip.name);
						AnimationComponent.Play(animationClip.name);

					} else {
						Debug.LogError("animation is null!");
					}
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

	}
}
