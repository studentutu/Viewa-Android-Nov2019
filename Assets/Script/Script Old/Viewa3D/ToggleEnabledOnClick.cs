using UnityEngine;
using System.Collections;

namespace Viewa3D
{
	public class ToggleEnabledOnClick : MonoBehaviour {
		
		public GameObject Target;

		// Use this for initialization
		void Start () 
		{
		}
		
		// Update is called once per frame
		void Update () 
		{
		}
		
		public void OnClick()
		{
			Target.SetActive(! Target.activeSelf);
		}
	}
}
