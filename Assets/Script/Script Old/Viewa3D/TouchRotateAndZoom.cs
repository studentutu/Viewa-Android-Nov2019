//#define BUNDLE_TEST_ONLY

using UnityEngine;
using System.Collections;

namespace Viewa3D
{
	public class TouchRotateAndZoom : MonoBehaviour {

		public GameObject target;
		public Collider hitTestCollider; //optional the mesh collider used for the hit test for rotation (pinch and zoom doesn't require a hit test). Recommend a simple Box Collider as a mesh Collider gets insanely slow
	
		public bool zoomEnabled = true; //set to false to disable the user zooming
		public float pinchScaleRate = 2.0f; // zoom rate with pinch
		public Vector3 maxScale;
		public Vector3 minScale;
		public Vector3 fillScale; //what doubletapping sets scale too
		public bool fillShouldResetRotation = true; //should the double-tap fill reset the rotation too

		public bool rotationEnabled = true; //set to false to disable the user rotating
		public float rotationRate = 0.1f; //how fast it rotates compared to touch movement
		public Viewa3D.ToggleRotate toggleRotateScriptComponent; //reference to a toggleRotate script to disable if necessary
		public float rotationFriction = 0.5f; //decay of rotation momuntum - set to 1 for never slowing down, set to 0 for no momuntum.
		public float hitTestDepth = 2000; //how far to cast down for the hit test for rotation. Put 0 to not do a hit test at all

		private bool manualRotation = false;
		private bool rotationMomentumActive = false;
		private Vector3 rotationVelocity;

		// Use this for initialization
		void Start () {
		}
		
		// Update is called once per frame
		void Update () {
			if(rotationMomentumActive){
				rotationVelocity *= Mathf.Pow(rotationFriction, Time.deltaTime);
				DoUserRotate();

				//Debug.Log("x velocity = "+Mathf.Abs(rotationVelocity.x) +", y velocity ="+ Mathf.Abs(rotationVelocity.y));
				if((Mathf.Abs(rotationVelocity.x)<0.01f) && (Mathf.Abs(rotationVelocity.y)<0.01f) && (Mathf.Abs(rotationVelocity.z)<0.01f)){
					rotationMomentumActive = false;
				}
			}
		}

		GameObject areaObject()
		{
			GameObject areaObject = this.gameObject;
			while(areaObject.name.StartsWith("Area:") == false){
				areaObject = areaObject.transform.parent.gameObject;
			}
			return areaObject;
		}

		bool isSticky()
		{
#if BUNDLE_TEST_ONLY
			return true;
#else
			AreaBehavior ab = areaObject().GetComponent<AreaBehavior>();
			if(ab){
				//Debug.Log ("Area is sticky? " + ab.Sticky);
			} else {
				Debug.LogWarning ("Cant find area!");
			}
			return ab.Sticky;
			#endif
		}

		void DoUserRotate()
		{
			if(isSticky()){
				//we sticky we rotate relateive to the camera
				target.transform.RotateAround(target.transform.position,Camera.main.transform.up,(0-rotationVelocity.x));
				target.transform.RotateAround(target.transform.position,Camera.main.transform.right,rotationVelocity.y);
				target.transform.RotateAround(target.transform.position,Camera.main.transform.forward,rotationVelocity.z);
			} else {
				//when in AR mode we always rotate relative to the area plane
				GameObject _areaObject = areaObject();
				target.transform.RotateAround(target.transform.position,_areaObject.transform.forward,(0-rotationVelocity.x));
				target.transform.RotateAround(target.transform.position,_areaObject.transform.right,rotationVelocity.y);
				target.transform.RotateAround(target.transform.position,_areaObject.transform.up,(0-rotationVelocity.z));
			}
		}

		void DoResetPositionAndScale()
		{
			if(fillShouldResetRotation){
				target.transform.localRotation = Quaternion.identity;
			}
			if(zoomEnabled){
				target.transform.localScale = fillScale;
				target.transform.localPosition = Vector3.zero;
			}
		}

		void FixedUpdate() {

			//kb handling for testing in editor
			if(Input.GetKey(KeyCode.LeftShift)){
				if (Input.GetKey ("up")) {
					ChangeScaleBy(new Vector3(10,10,10));
				} else if (Input.GetKey ("down")) {
					ChangeScaleBy(new Vector3(-10,-10,-10));
				} else if (Input.GetKey ("left")){
					rotationVelocity = new Vector3(0,0,1);
					DoUserRotate ();
				} else if (Input.GetKey ("right")){
					rotationVelocity = new Vector3(0,0,-1);
					DoUserRotate ();
				}
			} else {
				if (Input.GetKey ("up")) {
					rotationVelocity = new Vector3(0,1,0);
					DoUserRotate ();
				} else if (Input.GetKey ("down")) {
					rotationVelocity = new Vector3(0,-1,0);
					DoUserRotate ();
				} else if (Input.GetKey ("left")) {
					rotationVelocity = new Vector3(-1,0,0);
					DoUserRotate ();
				} else if (Input.GetKey ("right")) {
					rotationVelocity = new Vector3(1,0,0);
					DoUserRotate ();
				} else if (Input.GetKey ("return")) {
					DoResetPositionAndScale();
				}
			}

			//touch handling on mobile device
			if (Input.touchCount > 0) 
			{       
				if(Input.touchCount == 1)
				{ // single finger 

					Touch theTouch = Input.GetTouch(0);					
					if(rotationEnabled){
						if ((theTouch.phase == TouchPhase.Began) && didHitCollider(theTouch.position)) 
						{
							disableRotationMomentum();
							manualRotation = true;
							if(toggleRotateScriptComponent != null){
								toggleRotateScriptComponent.Rotating = false;
							}
//							Debug.Log("Touches Began Momentum = false");
						}       
						
						if ((theTouch.phase == TouchPhase.Moved) && manualRotation) 
						{	
							disableRotationMomentum();
							rotationVelocity = theTouch.deltaPosition * rotationRate;
							DoUserRotate();
						}       

						if (((theTouch.phase == TouchPhase.Ended) || (theTouch.phase == TouchPhase.Canceled)) && manualRotation) {
							manualRotation = false;
							if(rotationFriction > 0){
								rotationMomentumActive = true;
							}
						}
					}
					if ((theTouch.tapCount == 2) && (didHitCollider(theTouch.position))){
						disableRotationMomentum();
						if ((theTouch.phase == TouchPhase.Ended) || (theTouch.phase == TouchPhase.Canceled)) 
						{
							DoResetPositionAndScale();
						}
					}

				}// touchCount = 1

				if(Input.touchCount == 2)
				{ // pinch zooming
					disableRotationMomentum();
					// Store both touches.
					Touch touchZero = Input.GetTouch(0);
					Touch touchOne = Input.GetTouch(1);
					// Find the position in the previous frame of each touch.
					Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
					Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;					
					// Find the magnitude of the vector (the distance) between the touches in each frame.
					float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
					float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;

					if(zoomEnabled){
						// Find the difference in the distances between each frame.
						float deltaMagnitudeDiff = (touchDeltaMag - prevTouchDeltaMag) * pinchScaleRate;
						ChangeScaleBy(new Vector3(deltaMagnitudeDiff,deltaMagnitudeDiff,deltaMagnitudeDiff));
					}

					//rotate angle changes
					if (touchOne.phase == TouchPhase.Began){
						manualRotation = true;
					} 
					if (((touchZero.phase == TouchPhase.Moved) || (touchOne.phase == TouchPhase.Moved)) && manualRotation) {
						float turnAngle = Angle(touchZero.position, touchOne.position);
						float prevTurn = Angle(touchZero.position - touchZero.deltaPosition,
						                       touchOne.position - touchOne.deltaPosition);
						float turnAngleDelta = Mathf.DeltaAngle(prevTurn, turnAngle);
						rotationVelocity = new Vector3(0,0,turnAngleDelta);
						DoUserRotate();
					}
					if (((touchOne.phase == TouchPhase.Ended) || (touchOne.phase == TouchPhase.Canceled)) && manualRotation) {
						manualRotation = false;
						if(rotationFriction > 0){
							rotationMomentumActive = true;
						}
					}
				} // touchCount = 2
			}
		}

		static private float Angle (Vector2 pos1, Vector2 pos2) {
			Vector2 from = pos2 - pos1;
			Vector2 to = new Vector2(1, 0);
			
			float result = Vector2.Angle( from, to );
			Vector3 cross = Vector3.Cross( from, to );
			
			if (cross.z > 0) {
				result = 360f - result;
			}
			
			return result;
		}

		private bool didHitCollider(Vector3 pos)
		{
			if((hitTestDepth == 0) || (hitTestCollider == null)){
				return true;
			} else {
				Ray ray = Camera.main.ScreenPointToRay(pos);
				RaycastHit hit;
				if (hitTestCollider.Raycast (ray, out hit, hitTestDepth)) {
					if (hit.collider == hitTestCollider) {
						return true;
					}
				}
				return false;
			}
		}

		public void disableRotationMomentum()
		{
			rotationMomentumActive = false;
			rotationVelocity = Vector3.zero;
		}

		private void ChangeScaleBy(Vector3 amount)
		{
			Vector3 TargetScale = ClampScaleVector(target.transform.localScale + amount);
			target.transform.localScale = TargetScale;

			//keep us centered
			target.transform.localPosition = Vector3.zero;

			//			iTween.ScaleTo (target.gameObject, TargetScale, 0.1f);
		}
		
		private Vector3 ClampScaleVector(Vector3 scale)
		{
			return new Vector3 (Mathf.Clamp(scale.x, minScale.x, maxScale.x), 
			                    Mathf.Clamp (scale.y, minScale.y, maxScale.y), 
			                    Mathf.Clamp (scale.z, minScale.z, maxScale.z));
		}

	}
}