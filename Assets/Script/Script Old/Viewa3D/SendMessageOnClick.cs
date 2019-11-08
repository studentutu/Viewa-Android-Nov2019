using UnityEngine;
using System.Collections;

namespace Viewa3D {

	// Sends a message to an array of targets, or array of target names, on click
	public class SendMessageOnClick : MonoBehaviour {
		
		public GameObject[] Targets;
		public string[] TargetNames;
		
		public string Message = "OnClick";
		
		// Use this for initialization
		void Start () {
			
		}
		
		// Update is called once per frame
		void Update () {
			
			foreach (Touch touch in Input.touches)
			{
				if (touch.phase == TouchPhase.Ended)
				{
					CheckForClick (touch.position);
				}
			}
			
			if (Input.GetMouseButtonUp(0))
			{
				CheckForClick (Input.mousePosition);
			}
			
		}

		void CheckForClick(Vector2 position)
		{
			Ray ray = Camera.main.ScreenPointToRay(position);
			
			RaycastHit hitInfo;
			if (GetComponent<Collider>().Raycast(ray, out hitInfo, float.MaxValue))
			{
				foreach (GameObject target in Targets)
				{
					target.SendMessage(Message);
				}
				
				foreach (string targetName in TargetNames)
				{
					GameObject target = GameObject.Find (targetName);
					target.SendMessage(Message);
				}
			}
		}
	}
}
	