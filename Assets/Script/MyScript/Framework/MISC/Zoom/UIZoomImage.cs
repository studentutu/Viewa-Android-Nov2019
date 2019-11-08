using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIZoomImage : MonoBehaviour, IScrollHandler
{
	private Vector3 initialScale;

	[SerializeField]
	private float zoomSpeed = 0.5f;
	[SerializeField]
	private float maxZoom = 10f;
	private bool zoomStarted;
	public ScrollRectEx scrollRectEx;

	Vector3 myPosition;

	private void Awake()
	{
		initialScale = transform.localScale;
	}

	void ZoomStart() {

		scrollRectEx.routeToParent = false;
		Debug.Log ("LocalPosition :" + transform.localPosition);
		myPosition = transform.localPosition;
//		transform.localScale = new Vector3(1.05f,1.05f,1.05f);
	}
	void ZoomEnd() {

		scrollRectEx.routeToParent = true;
		transform.localScale = Vector3.one;
		transform.localPosition = myPosition;
	}

	void Update() {

		if (Input.touchCount == 2) {

			Debug.Log ("-------------------------------> Input.touchCount == 2");

//			if (transform.localScale == Vector3.one) {
//				if (zoomStarted) {
//					Debug.Log ("###### Zoom End");
//					zoomStarted = false;
//					ZoomEnd ();
//					return;
//				} else {
//					Debug.Log ("###### Zoom Started");
//					zoomStarted = true;
//					ZoomStart ();
//				} 
//			}

			Debug.Log ("Input.touchCount == 2");
			// Store both touches.
			Touch touchZero = Input.GetTouch (0);
			Touch touchOne = Input.GetTouch (1);

			// Find the position in the previous frame of each touch.
			Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
			Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

			// Find the magnitude of the vector (the distance) between the touches in each frame.
			float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
			float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;

			// Find the difference in the distances between each frame.
			float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;

			if (deltaMagnitudeDiff < 10 && deltaMagnitudeDiff >= 0) {
				return;
			} 

			Debug.Log ("deltaMagnitudeDiff :" + deltaMagnitudeDiff);
			var delta = Vector3.one * (deltaMagnitudeDiff * -zoomSpeed * Time.deltaTime);  //Vector3.one
			Debug.Log ("delta :---->" + delta);
			var desiredScale = transform.localScale + delta;

			desiredScale = ClampDesiredScale (desiredScale);

			transform.localScale = desiredScale;

			if (transform.localScale.x > initialScale.x) {
				if (!zoomStarted) {
					Debug.Log ("###### Zoom Started");
					zoomStarted = true;
					ZoomStart ();
				}
			} else {
				if (zoomStarted) {
					Debug.Log ("###### Zoom End :" + "Pos :" + transform.position);
					zoomStarted = false;
					ZoomEnd ();
				}
			}
		}
	}

//	public void OnDrag (UnityEngine.EventSystems.PointerEventData eventData){
//	
//		scrollRectEx.OnDrag (eventData);
//
//		if (Input.touchCount == 2) {
//			Debug.Log ("-------------------------------> OnScroll");
//
//			if (transform.localScale == Vector3.one) {
//				if (!zoomStarted) {
//					Debug.Log ("Zoom Started");
//					zoomStarted = true;
//					ZoomStart ();
//				} else {
//					Debug.Log ("Zoom End");
//					zoomStarted = false;
//					ZoomEnd ();
//				}
//			}
//
//			var delta = Vector3.one * (eventData.scrollDelta.y * zoomSpeed);
//			var desiredScale = transform.localScale + delta;
//
//			desiredScale = ClampDesiredScale (desiredScale);
//
//			transform.localScale = desiredScale;
//		}
//	}

	public void OnScroll(PointerEventData eventData)
	{
		Debug.Log ("-------------------------------> OnScroll");
	
		if (transform.localScale == Vector3.one) {
			if (!zoomStarted) {
				Debug.Log ("Zoom Started");
				zoomStarted = true;
				ZoomStart ();
			} else {
				Debug.Log ("Zoom End");
				zoomStarted = false;
				ZoomEnd ();
			}
		}

		var delta = Vector3.one * (eventData.scrollDelta.y * zoomSpeed);
		var desiredScale = transform.localScale + delta;

		desiredScale = ClampDesiredScale(desiredScale);

		transform.localScale = desiredScale;
	}

	private Vector3 ClampDesiredScale(Vector3 desiredScale)
	{
		desiredScale = Vector3.Max(initialScale, desiredScale);
		desiredScale = Vector3.Min(initialScale * maxZoom, desiredScale);
		return desiredScale;
	}
}