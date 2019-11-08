using UnityEngine;
using System.Collections;

namespace Viewa3D
{
	public class RunNamedWidgetOnColliderTouch : MonoBehaviour {

		public string widgetName;
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


					if(widgetName != null){
						//look up the widget name and execute it
						// we just find the first match
						//Debug.Log("Finding Widget named "+widgetName);
						
						GameObject[] trackables = GameObject.FindGameObjectsWithTag ("Trackable");
						foreach (GameObject trackable in trackables)
						{
							ACPTrackableBehavior behavior = trackable.GetComponent<ACPTrackableBehavior> ();
							foreach (AreaBehavior area in behavior.areas){
								foreach(ACP.FrameData frameData in area.data.frames){
									foreach (ACP.WidgetData widgetData in frameData.widgets){
										//Debug.Log ("Checking Named Widget: "+widgetData.name);
										if(widgetData.name == widgetName){
											//Debug.Log("Found Widget named "+widgetName);
											if (widgetData.GetType() == typeof(ACP.ButtonData)){
												ACP.ButtonData buttonData = widgetData as ACP.ButtonData;
												ButtonBehavior bb = buttonData.CreateBehavior(null,0) as ButtonBehavior;
												area.ButtonClicked(bb);
												return;
											} else {
												Debug.LogError(" Widget named "+widgetName+ " is not a Button");
											}
										}
									}
								}
							}
						}
						Debug.LogError("Could not find Widget named "+widgetName);
						
					} else {
						Debug.LogError("WidgetName is null!");
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
