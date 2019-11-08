using UnityEngine;
using System.Collections;

namespace Viewa3D
{
	public class playSoundOnColliderTouch : MonoBehaviour {

		public AudioClip audioClip;
		public Collider hitTestCollider;
		public float hitTestDepth = 2000; //how far to cast down for the hit test for rotation. Put 0 to not do a hit test at all
		public float volume = 1f;
		public float pitch = 1f;

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

					if(audioClip != null){
						//Debug.LogError("triggering audio clip " + audioClip);
						NGUITools.PlaySound(audioClip, volume, pitch);

					} else {
						Debug.LogError("audioClip is null!");
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
					if (hit.collider == hitTestCollider) {
						return true;
					}
				}
				return false;
			}
		}
	}
}
