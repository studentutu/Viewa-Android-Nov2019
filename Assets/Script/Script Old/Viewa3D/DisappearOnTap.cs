using UnityEngine;
using System.Collections;

namespace Viewa3D
{
	public class DisappearOnTap : MonoBehaviour {
	
		public GameObject Target;
		
		// Use this for initialization
		void Start () 
		{
			if (Target == null)
			{
				Target = this.gameObject;
			}
		}
		
		// Update is called once per frame
		void Update () 
		{
			foreach (Touch touch in Input.touches)
			{
				if (touch.phase == TouchPhase.Ended)
				{
					DestroyIfTouchedAt(touch.position);
				}
			}
			
			if (Input.GetMouseButtonUp(0))
			{
				DestroyIfTouchedAt(Input.mousePosition);
			}
			
		}
		
		void DestroyIfTouchedAt(Vector2 position)
		{
			Ray ray = Camera.main.ScreenPointToRay(position);
				
			RaycastHit hitInfo;
			if (GetComponent<Collider>().Raycast(ray, out hitInfo, float.MaxValue))
			{
				Destroy (this.gameObject);
			}
		}
	}
}

