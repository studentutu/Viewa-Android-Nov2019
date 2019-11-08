using UnityEngine;
using System.Collections;

namespace Viewa3D
{
	public class RunNamedWidgetOnClick : MonoBehaviour {

		public string widgetName;

		public void OnClick() {
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
