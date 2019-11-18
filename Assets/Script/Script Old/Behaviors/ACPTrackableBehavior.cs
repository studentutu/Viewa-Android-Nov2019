// if USE_OLD_AREA_SNAPTO_SIZING is defined below, when in snapTo mode it will size to fit the AREA HEIGHT 
// if this is NOT defined (commented out) it will size to fit height and width of all snap-to-mode widgets that fit completely within their area.
//#define USE_OLD_AREA_SNAPTO_SIZING 

using UnityEngine;
using ACP;
using System;
using System.Collections;
using System.Collections.Generic;
using Vuforia;
using UnityEngine.UI;
using System.Linq;
using SimpleJSON;
using OTPL.UI;
using System.IO;
using Firebase.Analytics;

/// <summary>
/// This is the class that controls loading and unloading the tracking data when an image is detected, and then unloading it after it goes away for a while
/// </summary>

public class AreaFrameHistory : object
{
	public AreaBehavior area;
	public int frame;

	public override bool Equals (object other)
	{
		if (other == null || GetType () != other.GetType ()) { 
			return false;
		}
		AreaFrameHistory afh = (AreaFrameHistory)other;
		return (afh.frame == this.frame) && (afh.area == this.area);
	}

	public override int GetHashCode ()
	{
		return this.area.GetHashCode () ^ this.frame;
	}
}

// A custom handler that implements the ITrackableEventHandler interface.
public class ACPTrackableBehavior : MonoBehaviour, ITrackableEventHandler
{
	#region Trackable Data

	public TrackableData data;
	public string triggerId;
	public string targetId;

	#endregion

	private WWW jsonWWW;

	#region Child Game Objects

	public GameObject AugmentationObject;
	private GameObject preloader;
	// Just a widget we can display while our areas preload all their images and what not
	public List<AreaBehavior> areas = new List<AreaBehavior> ();
	// The list of areas defined to display when we see this trackable

	#endregion

	#region AR Tracking stuff

	public ACPTrackingManager manager;
	private TrackableBehaviour mTrackableBehaviour;

	#endregion

	#region Visiblity

	public float TimeoutTime = 3.0f;
	// How long after not seeing this Trackable should we wait before unloading it
	public bool isVisible = false;
	public bool hasLoaded = false;
	public bool isUnloading = false;
	private float disappearTime;
	// Track how long since we disappeared

	#endregion

	#region Sticky

	private Bounds allAreaBounds;
	// how big is everything? Used to calculate the camera distance
	public bool Sticky = false;
	// Are we currently 'sticky' or not?

	#endregion

	private Stack<AreaFrameHistory> areaFrameHistory = new Stack<AreaFrameHistory> ();
	private static Matrix4x4? fullScreenCameraMatrix = null;
	private static float fullScreenCameraPixelHeight = 0;
	private bool trackingTriggerEndSent = false;

	private GameObject webPlayerTriggerImagePlane;

	#region Creation & Destruction

	class TweenInfo
	{
		public Vector3 PositionTo;
		public Vector3 PositionFrom;
		public Quaternion RotationTo;
		public Quaternion RotationFrom;
		public float StartTime;
		public bool Active;
		public bool HasUpdated;
		public Transform Target;
	};

	TweenInfo targetTweenInfo = new TweenInfo ();
	TweenInfo preloaderTweenInfo = new TweenInfo ();

	Text mDebugText;

	void Start ()
	{
        
				if (fullScreenCameraMatrix.HasValue == false) {
					fullScreenCameraMatrix = Camera.main.projectionMatrix;
		//			Debug.Log ("SETTING FULLSCREEN CAMERA MATRIX");
		//			Debug.Log ("Camera Projection Matrix[0,0] xScale = "+Camera.main.projectionMatrix[0,0]);
		//			Debug.Log ("Camera Projection Matrix[1,1] yScale = "+Camera.main.projectionMatrix[1,1]);
		//			Debug.Log ("Camera Projection Matrix[2,2] zScale = "+Camera.main.projectionMatrix[2,2]);
					fullScreenCameraPixelHeight = Camera.main.pixelHeight;
				}

		// Register for tracking events
		mTrackableBehaviour = GetComponent<TrackableBehaviour> ();  //Tracking events - for tracking data
		manager = Camera.main.GetComponent<ACPTrackingManager> ();

		// Create an object to store the augmentation....
		AugmentationObject = new GameObject ();
		AugmentationObject.name = "AugmentationObject";
		AugmentationObject.transform.parent = this.transform;

		if (mTrackableBehaviour) {  //null here no probs
			mTrackableBehaviour.RegisterTrackableEventHandler (this);
		}

		#if UNITY_WEBPLAYER
		WebPlayerStart();
		#else
        if(trackableLength() > 0){
            StartCoroutine(LoadTriggerData());
        }
		#endif

	}

	private void WebPlayerStart ()
	{
		// Create the prelaoder
		if (preloader == null) {
			CreatePreloader ();
		}

		this.LoadAreas ();
		OnTrackingFound ();

		this.transform.localScale = new Vector3 (data.size.x, data.size.x, data.size.x);
		if (data.imageUrl != null) {
			if (webPlayerTriggerImagePlane == null) {
				webPlayerTriggerImagePlane = GameObject.CreatePrimitive (PrimitiveType.Plane);
				webPlayerTriggerImagePlane.transform.localScale = new Vector3 (-data.size.x / 10.0f, 1, -data.size.z / 10.0f);
				webPlayerTriggerImagePlane.transform.parent = this.transform;
				webPlayerTriggerImagePlane.transform.Translate (Vector3.up * -0.5f);
			}
			DownloadCache.Instance.DownloadCompleteTexture += HandleTextureCacheInstanceTextureLoaded;
			DownloadCache.Instance.LoadFile (data.imageUrl, DownloadCache.DownloadType.TextureDownload);

			//MouseOrbit mouseOrbit = Camera.main.GetComponent<MouseOrbit> ();
			//mouseOrbit.distance = data.size.x * 3;
			//mouseOrbit.zoomSpeed = data.size.x / 10.0f;
		}
	}

	void HandleTextureCacheInstanceTextureLoaded (string url, Texture2D texture)
	{
		if (url == data.imageUrl) {
			webPlayerTriggerImagePlane.GetComponent<Renderer> ().material.SetTexture ("_MainTex", texture);
		}
	}

	private IEnumerator LoadTriggerData ()
	{
		Debug.Log("LoadTriggerData");
		isVisible = true;

		//jeetesh - changed here.
		// Create the prelaode

		if (preloader == null) {
			//if (AppManager.Instnace.isDynamicContentFromHub) {
				CreatePreloader ();
			//} else {
				//Debug.Log ("Inside Preloader");
				//preloader = (GameObject)GameObject.Instantiate (manager.preloadPrefab);
				//Debug.Log ("Preloader parent :" + preloader.transform.parent);
				//Debug.Log ("Preloader position before:" + preloader.transform.position);
				//Debug.Log ("AugmentationObject.transform.position : " + AugmentationObject.transform.position);

				//preloader.transform.parent = AugmentationObject.transform;
				//Quaternion rotation = AugmentationObject.transform.rotation;			
				//preloader.transform.localPosition = AugmentationObject.transform.position + (AugmentationObject.transform.forward * -5);
				//Debug.Log ("Preloader position after:" + preloader.transform.position);
				//rotation *= Quaternion.Euler (-120, 0, 0);
				//preloader.transform.rotation = rotation;
			//}
		}

		//jeetesh - added below two lines.

		// Has no trackable.. was called frommatrix history!
		if (mTrackableBehaviour == null) {
			Debug.Log ("mTrackableBehaviour == null");
			AttachPreloaderToCamera ();	
			OnTrackingLost ();
		}

		string url;
		if (manager.testMode) {
			url = String.Format ("{0}/cloud/getResponse.aspx?id={1}&cid={2}&language={3}&userId={4}&regionId={5}&userEmail={6}&catid={7}&devmode=1", AppManager.Instnace.baseURL, this.triggerId, this.targetId, this.manager.language,
				AppManager.Instnace.userId, AppManager.Instnace.regionId, AppManager.Instnace.userEmail, AppManager.Instnace.categoryid);
		} else {
			url = String.Format ("{0}/cloud/getResponse.aspx?id={1}&cid={2}&language={3}&userId={4}&regionId={5}&userEmail={6}&catid={7}", AppManager.Instnace.baseURL, this.triggerId, this.targetId, this.manager.language,
				AppManager.Instnace.userId, AppManager.Instnace.regionId, AppManager.Instnace.userEmail, AppManager.Instnace.categoryid);
		}
		Debug.Log ("ACPTrackableBehavior url: "+ url);
		// record the start time
		float jsonRequestStartTime = Time.time;

		url = String.Format ("{0}&cachePreventer={1}", url, UnityEngine.Random.Range (0, 999999)); //add a randomised paramater to prevent caching - see http://answers.unity3d.com/questions/209078/disable-cache-for-www.html
		Debug.Log ("loading JSON url: " + url);

		// Hide the men ubutton etc
		ACPUnityPlugin.Instnace.setNativeUIVisible (false);

		jsonWWW = new WWW (url);

		yield return jsonWWW;

		ProcessJsonResponse (jsonWWW, jsonRequestStartTime);

        Debug.Log("New break point");

		jsonWWW = null;
	}

	void Update ()
	{
		 //Unload if we aren't visible for a while....
		if (!isVisible) {
			if (this.Sticky) {
				// keep updating the time so we dont get destroyed
				disappearTime = Time.time;
			} else if (Time.time - disappearTime > TimeoutTime) {
				//Debug.Log("Unloading areas for trackable: " + data.id);
				//Warning - Jeetesh - This is geting called and UnLoading the Asset as not Sticky.  Have to check the functionality of this.Sticky.
				// Right now commented out the functionality of this.Sticky.
				//jeetesh - commented the below line so that it will not unload the trackable.
                //--UnLoadAreas ();
				//Warning - Jeetesh - This is geting called and stopping the webView to load.
				//				Debug.Log ("manager.OnCloseButtonTapped ()");
				//manager.OnCloseButtonTapped ();
			}
		}

		//		ProcessTween (targetTweenInfo);

		//		if (this.preloader != null) {
		//			ProcessTween (preloaderTweenInfo);
		//		}

		#if UNITY_EDITOR
		if (Input.GetKeyDown ("space")) {
			if (this.Sticky) {
				OnTrackingFound ();
			} else {
				OnTrackingLost ();
			}
		}
#endif
        //		Jeetesh - Check if below commented text is required as it was commented in old-AcptrackableBehavior.
        //		if (this.Sticky) {
        //			if (Camera.main.projectionMatrix == fullScreenCameraMatrix) {
        //				Debug.Log ("REATTACHING CONTENT TO CAMERA");
        //				AttachContentToCamera (); //re-atatch contents when unpausing to ensure we get the correct camera matrix setting
        //			}
        //		}
        OnDeviceBackUpdate();
	}


	private float easeInQuad (float start, float end, float value)
	{
		end -= start;
		return end * value * value + start;
	}


	private float easeOutQuart (float start, float end, float value)
	{
		value--;
		end -= start;
		return -end * (value * value * value * value - 1) + start;
	}

	private float easeOutQuad (float start, float end, float value)
	{
		end -= start;
		return -end * value * (value - 2) + start;
	}

	private float easeInOutQuad (float start, float end, float value)
	{
		value /= .5f;
		end -= start;
		if (value < 1)
			return end / 2 * value * value + start;
		value--;
		return -end / 2 * (value * (value - 2) - 1) + start;
	}

	private float easeOutExpo (float start, float end, float value)
	{
		end -= start;
		return end * (-Mathf.Pow (2, -10 * value / 1) + 1) + start;
	}

	private void ProcessTween (TweenInfo tweenInfo)
	{
		if (tweenInfo.Active) {
			if (!tweenInfo.HasUpdated) {
				tweenInfo.HasUpdated = true;
				tweenInfo.StartTime = Time.time;
			}
			float time = Mathf.Min (1.0f, (Time.time - tweenInfo.StartTime) * 1.0f);
			if (time < 1.0f) {
				time = easeOutExpo (0.0f, 1.0f, time);
				tweenInfo.Target.position = Vector3.Lerp (tweenInfo.PositionFrom, tweenInfo.PositionTo, time);
				tweenInfo.Target.rotation = Quaternion.Slerp (tweenInfo.RotationFrom, tweenInfo.RotationTo, time);
				//Debug.Log ("Setting Tween for " + tweenInfo.Target.name +" to " +tweenInfo.Target.position + "time = "+time);
			} else {
				tweenInfo.Target.position = tweenInfo.PositionTo;
				tweenInfo.Target.rotation = tweenInfo.RotationTo;
				tweenInfo.Active = false;
				//Debug.Log ("Finished Tweens for " + tweenInfo.Target.name +" to position " +tweenInfo.Target.position);
				//Debug.Log ("Finished Tweens for " + tweenInfo.Target.name +" to rotation " +tweenInfo.Target.rotation);
			}
		}
	}

	void OnDestroy ()
	{
		#if UNITY_WEBPLAYER
		//TextureCache.Instance.TextureLoaded -= TextureLoaded;
		#endif

		if (preloader != null) {
			Destroy (preloader);
		}

		// Unregister for tracking events
		mTrackableBehaviour = GetComponent<TrackableBehaviour> ();
		if (mTrackableBehaviour) {
			mTrackableBehaviour.UnregisterTrackableEventHandler (this);
		} 

		//Debug.Log("ACPTrackableBehavior: On Destroy");
	}

	#endregion

	#region Tracking State Changes

	// Implementation of the ITrackableEventHandler function called when the
	// tracking state changes.
	public void OnTrackableStateChanged (
		TrackableBehaviour.Status previousStatus,
		TrackableBehaviour.Status newStatus)
	{
		if (newStatus == TrackableBehaviour.Status.DETECTED ||
			newStatus == TrackableBehaviour.Status.TRACKED) {
			OnTrackingFound ();
		} else { //if (previousStatus != TrackableBehaviour.Status.UNKNOWN)
			// Check for previous status UNKNOWN so we dont START with a tracking lost first thing...

			OnTrackingLost ();
		}
	}

	protected void EnableRenderingOfChildren ()
	{
		Renderer[] rendererComponents = GetComponentsInChildren<Renderer> ();
		// Enable rendering:
		foreach (Renderer component in rendererComponents) {
			component.enabled = true;
		}
	}

	protected void DetatchContentFromCamera ()
	{
		//Debug.Log ("Detaching Content From Camera");
		manager.setBackButtonVisibility(true);
		//Jeetesh - Check if below commented text is required as it was commented in old-AcptrackableBehavior.
			Camera.main.pixelRect = new Rect (0, 0, Screen.width, Screen.height);
				Camera.main.projectionMatrix = fullScreenCameraMatrix.Value;
		isVisible = true;

		if (data != null && data.sticky) {
			foreach (AreaBehavior area in this.areas) {
				area.Sticky = false;
			}	
		}
		this.Sticky = false;

		//------------
		transform.position = Vector3.zero;
		transform.rotation = Quaternion.identity;
		//------------

		Transform t = this.AugmentationObject.transform;
		t.parent = this.transform;

		Debug.Log ("ACPTrackableBehavior Position:" + this.transform.position);

		targetTweenInfo.Active = false;
		AugmentationObject.transform.position = Vector3.zero;
		AugmentationObject.transform.rotation = Quaternion.identity;


		//		targetTweenInfo.PositionFrom = t.position;
		//		targetTweenInfo.RotationFrom = t.rotation;
		//		targetTweenInfo.PositionTo = Vector3.zero;
		//		targetTweenInfo.RotationTo = Quaternion.identity;
		//		targetTweenInfo.Active = true;
		//		targetTweenInfo.HasUpdated = false;
		//		targetTweenInfo.StartTime = Time.time;
		//		targetTweenInfo.Target = t;

	}

	protected void AttachContentToCamera ()
	{
		manager.setBackButtonVisibility (true);

		Vector3 newPos = Vector3.zero;
		Quaternion rotation = Quaternion.identity;

        if (hasLoaded) {
			//Debug.Log ("Entering SnapTo Mode: Attaching content to camera! (LOADED)");
			foreach (AreaBehavior area in this.areas) {
				area.Sticky = true;
			}	
			this.Sticky = true;

			#if USE_OLD_AREA_SNAPTO_SIZING
			float areaHeight = Mathf.Abs(this.allAreaBounds.center.z - this.allAreaBounds.size.z);
			float areaWidth = Mathf.Abs(this.allAreaBounds.center.x - this.allAreaBounds.size.x);
			#else
			float areaHeight = Mathf.Abs (this.allAreaBounds.size.z);
			float areaWidth = Mathf.Abs (this.allAreaBounds.size.x);

			//add 2% padding
			areaHeight += areaHeight * 0.03f;
			areaWidth += areaWidth * 0.03f;
			#endif
			//Debug.Log ("...areaHeight: " + areaHeight);
			//Debug.Log ("...areaWidth x: " + areaWidth);

			#if USE_OLD_AREA_SNAPTO_SIZING
			#else
			Camera.main.projectionMatrix = fullScreenCameraMatrix.Value;
            /*			Debug.Log("Screen Height = " + Screen.height);
			Debug.Log ("Screen Width = " + Screen.width);
			Debug.Log ("Original Camera Pixel Rect = "+Camera.main.pixelRect);
			Debug.Log ("Original Camera Aspect = "+Camera.main.aspect);
			Debug.Log ("Original Camera FOV = "+Camera.main.fieldOfView);
*/

            float newHeight = fullScreenCameraPixelHeight + ((manager.navBarHeight) + 90);
//#if UNITY_EDITOR //            newHeight = fullScreenCameraPixelHeight - (110 + 110);  //- (manager.navBarHeight ) + 90
//            Camera.main.pixelRect = new Rect(0, 110, Screen.width, newHeight);  

//#elif UNITY_IOS || UNITY_ANDROID  //            if(AppManager.Instnace.isVuforiaOn){  //                newHeight = fullScreenCameraPixelHeight + 197 + 70;  //40 //                Camera.main.pixelRect = new Rect(0, 5, Screen.width, newHeight); //20 //60  //9  //            } else {                 //                newHeight = fullScreenCameraPixelHeight - (110 + 110);  //- (manager.navBarHeight ) + 90 //                Camera.main.pixelRect = new Rect(0, 110, Screen.width, newHeight);   //            } 
//#endif 
            #if UNITY_EDITOR
            newHeight = fullScreenCameraPixelHeight - ((manager.navBarHeight) + 65 + 110);  //- (manager.navBarHeight ) + 90
            Camera.main.pixelRect = new Rect(0, 110, Screen.width, newHeight);  //50 //90  //It calculates from bottom.

#elif UNITY_IOS || UNITY_ANDROID

            if(AppManager.Instnace.isVuforiaOn){
                
                newHeight = fullScreenCameraPixelHeight + ((manager.navBarHeight) + 110 );  //- (manager.navBarHeight ) + 90
                Camera.main.pixelRect = new Rect(0, 5 ,Screen.width, newHeight);  
            } else {
                
                newHeight = fullScreenCameraPixelHeight - ((manager.navBarHeight) + 110);  //- (manager.navBarHeight ) + 90
                Camera.main.pixelRect = new Rect(0, 110, Screen.width, newHeight);  
            }
#endif

            Debug.Log ("----------------------------------------- >>>>>>>>>>> New Height = "+newHeight);
            Debug.Log("----------------------------------------- >>>>>>>>>>>> manager.navBarHeight :" + manager.navBarHeight);

			
			Camera.main.aspect = newHeight / Screen.width;
#endif

            /*			Debug.Log ("New Camera Pixel Rect = "+Camera.main.pixelRect);
			Debug.Log ("New Camera Aspect = "+Camera.main.aspect);
			Debug.Log ("New Camera FOV = "+Camera.main.fieldOfView);

			Debug.Log ("Camera Projection Matrix[0,0] xScale = "+Camera.main.projectionMatrix[0,0]);
			Debug.Log ("Camera Projection Matrix[1,1] yScale = "+Camera.main.projectionMatrix[1,1]);
			Debug.Log ("Camera Projection Matrix[2,2] zScale = "+Camera.main.projectionMatrix[2,2]);
*/
#if USE_OLD_AREA_SNAPTO_SIZING
#else
            float xScale = Camera.main.projectionMatrix [0, 0];
			Matrix4x4 matrix = Camera.main.projectionMatrix;
			matrix [0, 0] = xScale / (Screen.height / newHeight);
			Camera.main.projectionMatrix = matrix;
			#endif

			float yFov = (Mathf.Atan (1.0f / Camera.main.projectionMatrix [1, 1])) / (0.5f * Mathf.PI / 180.0f); // [1,1] is y/height
			//Debug.Log ("...yFov: " + yFov);

			float yDistance = (areaHeight / 2.0f) / Mathf.Tan (yFov * Mathf.PI / 180.0f / 2.0f);
			//Debug.Log ("...yDistance: " + yDistance);

			float xDistance = 0; 

			#if USE_OLD_AREA_SNAPTO_SIZING
			#else
			/*
			 * also take area width into account - https://visualjazz.jira.com/browse/VR-505 & VR-529
			 */
			float xFov = (Mathf.Atan (1.0f / Camera.main.projectionMatrix [0, 0])) / (0.5f * Mathf.PI / 180.0f); // [0,0] is x/width
			//			Debug.Log ("...xFov: " + xFov);
			xDistance = (areaWidth / 2.0f) / Mathf.Tan (xFov * Mathf.PI / 180.0f / 2.0f);
			//Debug.Log ("...xDistance: " + xDistance);
			#endif

			float distance;
			//if((areaWidth > areaHeight) && (Screen.height > Screen.width)){
			//	distance = xDistance;
			//} else {
			//	distance = yDistance;
			//}
			distance = Mathf.Max (xDistance, yDistance);
			//float distance = yDistance;
			//if(areaWidth > areaHeight){
			//	distance = xDistance;
			//}

			//Debug.Log ("...using distance: " + distance);
			#if USE_OLD_AREA_SNAPTO_SIZING
			newPos = Camera.main.transform.position + (Camera.main.transform.forward * distance);
			#else

			//Debug.Log ("...newPos (local): " + newPos);
			//new local coords
			newPos = new Vector3 (0f - this.allAreaBounds.center.x, 0f - this.allAreaBounds.center.z, distance);
			//Debug.Log ("...newPos (local): " + newPos);

			//translate this from local to world
			newPos = Camera.main.transform.TransformPoint (newPos);
			//Debug.Log ("...newPos (world): " + newPos);
			#endif
		}
		else
		{
			// Haven't loaded the areas yet... set a default that seems decent enough :)
			newPos = Camera.main.transform.position + (Camera.main.transform.forward * 1500);
		}

		rotation = Camera.main.transform.rotation;			
		rotation *= Quaternion.Euler (-90, 0, 0);
		Transform t = AugmentationObject.transform;
		t.parent = Camera.main.transform;


		targetTweenInfo.Active = false;
		t.position = newPos;
		t.rotation = rotation;

		//		targetTweenInfo.PositionFrom = t.position;
		//		targetTweenInfo.RotationFrom = t.rotation;
		//		targetTweenInfo.PositionTo = newPos;
		//		targetTweenInfo.RotationTo = rotation;
		//		targetTweenInfo.StartTime = Time.time;
		//		targetTweenInfo.Active = true;
		//		targetTweenInfo.HasUpdated = false;
		//		targetTweenInfo.Target = t;
	}

	private void OnTrackingFound ()
	{
		#if UNITY_EDITOR
		Debug.Log("Trackable " + mTrackableBehaviour.TrackableName + " found");
		#endif
		// Re-attach the content to the camera

		//jeetesh
		if (this.Sticky) {
			//Jeetesh - This is commented so that the content will not move in 3D space and will remain attached to camera.
			DetatchContentFromCamera ();

			// Enable rendering again
			EnableRenderingOfChildren ();

			//Restore the pre-loader hierarchy so it moves in 3D space with the camera again....
			//Jeetesh - This is commented so that the preloader will not move in 3D space and will remain attached to camera.
			DetatchPreloaderFromCamera ();

		}

		Debug.Log ("ACPTrackableBehaviour :" + this.transform.position);
		foreach (AreaBehavior ab in this.areas) {
			ab.TrackingFound (this.Sticky);
		}
	}

	private void OnTrackingLost ()
	{
		#if UNITY_EDITOR
		//		Debug.Log("Trackable " + mTrackableBehaviour.TrackableName + " lost");
		#endif

		// If we are still loading, and we have the data, and we are sticky
		// Then we can 'stick' the content to the center, and  attach the preloader 
		// to the camera so it stays in the center of the screen, but we cant stick the content yet
		//  as it hasn't finished loading

		if (!hasLoaded || (data != null && data.sticky)) {
			this.Sticky = true;
			AttachPreloaderToCamera ();
			AttachContentToCamera ();
		} else {
			// Hide everything as we aren't sticky...
			isVisible = false;
			disappearTime = Time.time;

			// Disable rendering:
			Renderer[] rendererComponents = GetComponentsInChildren<Renderer> ();
			foreach (Renderer component in rendererComponents) {
				component.enabled = false;
			}
			Debug.Log ("STICKY NOT ALLOWED!");
		}

		foreach (AreaBehavior ab in this.areas) {
			ab.TrackingLost (this.Sticky);
		}

	}

	#endregion


	protected void ProcessJsonResponse (WWW www, float jsonRequestStartTime)
	{
		// record the end time
		float jsonRequestEndTime = Time.time;

		//bed - we reserve the first 10% of the progress to getting the json file
		ACPUnityPlugin.Instnace.setDownloadProgress (0.1f);

		Debug.Log ("============================================================= >Got Json Response for :" + www.url + "@@:- " + www.text);

		if (www.error != null || string.IsNullOrEmpty (www.text)) {
			// Don't show the alert for hidden triggers
			hasLoaded = true; // makes stuff clean up better...
			ACPUnityPlugin.Instnace.hideStatusoverlay ();
			StartCoroutine (RemovePreloader ());
			UnLoadAreas ();


			#if UNITY_EDITOR
			BackButtonTapped();
			#elif UNITY_ANDROID || UNITY_IPHONE
			//AppManager.Instnace.messageBoxManager.ShowMessage ("Data Download Error", www.error == null ? "Fail To Download" : www.error, "Ok",BackButtonTapped );
            //AppManager.Instnace.messageBoxManager.ShowMessage("Maintainence", "ProcessJsonResponse.. Error .. Server under maintenance. Please try again soon.", "Ok", BackButtonTapped);
            if(AppManager.Instnace.isLive)
                AppManager.Instnace.messageBoxManager.ShowGenericPopup("Maintainence", "Data Download Error. Please try again soon.", "Ok", BackButtonTapped); 
            else
                AppManager.Instnace.messageBoxManager.ShowGenericPopup("Maintainence", "ProcessJsonResponse.. Error .. Server under maintenance. Please try again soon.", "Ok", BackButtonTapped);
            #endif

			ACPUnityPlugin.Instnace.setBackButtonVisibility (true);
		} else {
			//Debug.Log (www.text);
			JSONObject trackableJson = new JSONObject (www.text);

			// Make sure the json has an id field at least....
			if (trackableJson ["id"] == null) {
				hasLoaded = true; // makes stuff clean up better...

				ACPUnityPlugin.Instnace.hideStatusoverlay ();
				StartCoroutine (RemovePreloader ());
				UnLoadAreas ();
				#if UNITY_EDITOR
				BackButtonTapped();
                Debug.Log("Server error ================================================================");
#elif UNITY_IPHONE || UNITY_ANDROID
                //AppManager.Instnace.messageBoxManager.ShowMessage ("Maintainence Error inside.", www.error == null ? "Fail To Download" : www.error, "Ok", BackButtonTapped);
                //AppManager.Instnace.messageBoxManager.ShowMessage("Maintainence Error inside", www.url + "RESPONSE :- " + www.text, "Ok", null);
                if(AppManager.Instnace.isLive)
                    AppManager.Instnace.messageBoxManager.ShowGenericPopup("Maintainence", "Server error. Please try again soon.", "Ok", null);
                 else
                    AppManager.Instnace.messageBoxManager.ShowGenericPopup("Maintainence", "ProcessJsonResponse...trackableJson[id] == null...Server under maintenance. Please try again soon.", "Ok", null); //BackButtonTapped

#endif
                ACPUnityPlugin.Instnace.setBackButtonVisibility (true);

                return;
			}
			#if UNITY_EDITOR
			Debug.Log ("Got data OK");
#endif

            //Jeetesh - Get question from the cms.
            if (!AppManager.Instnace.CheckTrigger(this.triggerId.ToString()))
            {
                AppManager.Instnace.GetQuestion();
            }
            //AppManager.Instnace.messageBoxManager.ShowMessage("Maintainence Error outside", www.url + "RESPONSE :- " + www.text, "Ok", null);
            FacebookLogin.LogUserViewedContentEvent();

			//the json MAY have a mod_date, if so let the download cache instance know
			JSONObject responseModifiedDate = trackableJson ["mod_date"];
			if (responseModifiedDate != null) {
				//modified date is in UTC
				string dateString = responseModifiedDate.str;//date string is UTC formatted
				DateTime localDate = DateTime.Parse (dateString); //this will convert to local
				//Debug.Log ("Response Last Modified Date Specified As string '" + dateString + "' which is local " + localDate);
				if (DownloadCache.Instance) {
					DownloadCache.Instance.IgnoreCachedFilesEarlierThanDate = localDate;
				} else {
					//	Debug.Log ("DownloadCache.Instance is NULL!");
				}
			} else {
				//Debug.Log ("No Response Last Modified Date Specified");
				DownloadCache.Instance.IgnoreCachedFilesEarlierThanDate = null;
			}

			string version = trackableJson ["min_app_version"] == null ? "0" : trackableJson ["min_app_version"].str;

			// Check the data version is acceptable
			if (manager.CheckAppVersion (int.Parse (version))) {
				this.data = TrackableData.Create (trackableJson);
				this.data.id = ulong.Parse (this.triggerId);

				if (data != null) {

                    //Category detail parameter to capture love details
                    WebService.Instnace.catDetailParameter.IsLove = data.is_love;
                    WebService.Instnace.catDetailParameter.CloudTriggerId = triggerId;

                    ACPUnityPlugin.Instnace.setBackButtonVisibility(true);

					// Record in history
					if (this.data.record_history) {
						// Work around an android crash when we send the full json.. UGH...
						string title = trackableJson.StringOrEmpty ("history_title");
						string description = trackableJson.StringOrEmpty ("history_description");
						string image = trackableJson.StringOrEmpty ("history_image");
						string keywords = trackableJson.StringOrEmpty ("keywords");
						string icon = trackableJson.StringOrEmpty ("history_icon");

						 AppManager.Instnace.popup_keyword = keywords;

						string jsonString = string.Format (
							"{{ \"history_title\":		\"{0}\", " +
							"\"history_description\": 	\"{1}\", " +
							"\"history_image\": 		\"{2}\", " +
							"\"history_icon\":   		\"{3}\", " +
							"\"keywords\": 				\"{4}\" }}", title, description, image, icon, keywords);
                        
						ACPUnityPlugin.Instnace.recordHistory (triggerId, targetId, jsonString);
					}

					// Test mode sends timing information to the server.
					//					if (this.manager.timingEvents) {
					//						float requestTime = jsonRequestEndTime - jsonRequestStartTime;
					//						Debug.Log("JSON took : " + (requestTime));
					//						Debug.Log ("Tracking Timing: " + this.data.id +" url: "+www.url);
					//						ACPUnityPlugin.Instnace.trackingEvent ("Timing", this.data.id, -1, -1, -1, www.url, "JSON", "", "", "", requestTime);
					//					}

					//					Debug.Log("JSON finished");

					gameObject.name = "Trackable: " + data.id;
                    AppManager.Instnace.trackingId = data.tracking_id;
                    AppManager.Instnace.triggerId = triggerId;

                    //Tracking Reporting (Initialize the Start time)
                    ACPUnityPlugin.Instnace.InitializeStartTime();

                    Debug.Log("=================================> CampaignTrackingData called");
                    ACPUnityPlugin.Instnace.CampaignTrackingData(data.tracking_id, data.cloud_id, int.Parse(triggerId));
               
                    Firebase.Analytics.Parameter[] UserInfoParameters = {                             //new Firebase.Analytics.Parameter("FirstName", WebService.Instnace.appUser.FirstName),                             new Firebase.Analytics.Parameter("Email", WebService.Instnace.appUser.Email),                             //new Firebase.Analytics.Parameter("FirstName", WebService.Instnace.appUser.Email),                             //new Firebase.Analytics.Parameter("FirstName", WebService.Instnace.appUser.State)                     };

                    FirebaseAnalytics.LogEvent("HubCount", UserInfoParameters);

                    if (AppManager.Instnace.isVuforiaOn)
                    {
                        ACPUnityPlugin.Instnace.trackEvent("Tid_"+ triggerId , "ScanCount", data.tracking_id, 0);
                        ACPUnityPlugin.Instnace.trackEvent("Campaign_Scan", triggerId ,"Tracking id_" +data.tracking_id +" Cloud id_"+data.cloud_id , 0);
                        ACPUnityPlugin.Instnace.CampaignEventsData("SCAN", "COUNT");
                        FirebaseAnalytics.LogEvent("ScanCount","Tid", triggerId);
                        FirebaseAnalytics.LogEvent("ScanCount", UserInfoParameters);

                        //ACPUnityPlugin.Instnace.SendCampaignReport();
                    } else {

                        if (AppManager.Instnace.triggerHistoryMode == eTriggerHistoryMode.TriggerHistoryModeChannel)
                        {
                            ACPUnityPlugin.Instnace.trackEvent("Tid_" + triggerId, "HubCount", data.tracking_id, 0);
                            ACPUnityPlugin.Instnace.trackEvent("Campaign_Hub", triggerId, "Tracking id_" + data.tracking_id + " Cloud id_" + data.cloud_id, 0);
                            ACPUnityPlugin.Instnace.CampaignEventsData("HUB", "COUNT");
                            FirebaseAnalytics.LogEvent("HubCount","Tid", triggerId);
                            FirebaseAnalytics.LogEvent("HubCount", UserInfoParameters);
                            //ACPUnityPlugin.Instnace.SendCampaignReport();

                        } else if(AppManager.Instnace.triggerHistoryMode == eTriggerHistoryMode.TriggerHistoryModeLoved) {

                            ACPUnityPlugin.Instnace.trackEvent("Tid_" + triggerId, "LovedCount", data.tracking_id, 0);
                            ACPUnityPlugin.Instnace.trackEvent("Campaign_Love", triggerId, "Tracking id_" + data.tracking_id + " Cloud id_" + data.cloud_id, 0);
                            ACPUnityPlugin.Instnace.CampaignEventsData("LOVED", "COUNT");
                            FirebaseAnalytics.LogEvent("LovedCount", "Tid", triggerId);
                            FirebaseAnalytics.LogEvent("LovedCount", UserInfoParameters);
                            //ACPUnityPlugin.Instnace.SendCampaignReport();
                        }
                        //TODO: Jeetesh Need to add for the recent Screen event as well.
                    }


					this.LoadAreas ();

					//Debug.Log("Loaded Areas");

					// If we lost tracking during the json load time, go through that method again to fix up  'sticky' triggers!
					if (!isVisible && (data.sticky)) { // || data.record_history))
						isVisible = true;
						OnTrackingLost ();
					}

				}
			} else {
				// this is a bit of a hack but setting it to 'hasLoaded' will cause it to unload when we look away ;)
				hasLoaded = true;
				Debug.Log ("this.manager.OnCloseButtonTapped");
				this.manager.OnCloseButtonTapped ();
				ACPUnityPlugin.Instnace.setNativeUIVisible (true);
			}
		}
	}

    //Function to go back from Image Gallary.
    public void GoBackFromImageGallery()
    {
        if (AppManager.Instnace.isVuforiaOn)
        {

            ScanPanel scanPanel = (ScanPanel)CanvasManager.Instnace.ReturnPanelManager(ePanelManager.MainMenuPanelManager).ReturnPanel(ePanels.Scan_Panel);
            scanPanel.progressImage.fillAmount = 0;
            AppManager.Instnace.mDownloadProgress = 0;
            scanPanel.scanTextPanel.gameObject.SetActive(true);
            CanvasManager.Instnace.ReturnPanelManager(ePanelManager.MainMenuPanelManager).BackToPanel(ePanels.Scan_Panel);

        }
        else
        {

            BlankPanel blankPanel = (BlankPanel)CanvasManager.Instnace.ReturnPanelManager(ePanelManager.MainMenuPanelManager).ReturnPanel(ePanels.Blank_Panel);
            blankPanel.progressImage.fillAmount = 0;
            AppManager.Instnace.mDownloadProgress = 0;
            CanvasManager.Instnace.ReturnPanelManager(ePanelManager.MainMenuPanelManager).BackToPanel(ePanels.Blank_Panel);
        }
    }


    #region UI Handler

    public void BackButtonTapped()
    {
        if (AppManager.Instnace.webViewPage.webViewIsLoaded){
            AppManager.Instnace.webViewPage.DestroyWebView();
        }
        Debug.Log("areaFrameHistory.Count: " + areaFrameHistory.Count);
        //pop our areaFrameHistoryStack and go back
        if (areaFrameHistory.Count == 1)
        {
            
            if (CanvasManager.Instnace.ReturnPanelManager(ePanelManager.MainMenuPanelManager).ReturnPanelStatus(ePanels.ImageGallary_Panel))
            {
                if (AppManager.Instnace.doesButtonPressed)
                {
                    AppManager.Instnace.doesButtonPressed = false;
                    GoBackFromImageGallery();
                    return;
                }
            }
            else if (AppManager.Instnace.webViewPage.webViewIsLoaded)
            {
                if (AppManager.Instnace.doesButtonPressed)
                {
                    AppManager.Instnace.doesButtonPressed = false;
                    Debug.Log("AppManager.Instnace.webViewPage.m_webview :" + AppManager.Instnace.webViewPage.m_webview);
                    AppManager.Instnace.webViewPage.DestroyWebView();
                    return;
                }
            }
        }

        if (areaFrameHistory.Count > 1)
        {
            //if it is a webview then close it and return.

            Debug.Log("areaFrameHistory:" + areaFrameHistory.Count);
            ACPUnityPlugin.Instnace.setDownloadProgress(0);

            if (AppManager.Instnace.webViewPage.webViewIsLoaded)
            {
                Debug.Log("Inside webViewPage");
                AppManager.Instnace.webViewPage.DestroyWebView();
                return;
            }
            Debug.Log("areaFrameHistory.POP");
            areaFrameHistory.Pop();
            AreaFrameHistory afh = areaFrameHistory.Peek();
            afh.area.CurrentFrameIndex = afh.frame;
        }
        else
        {
            if (!AppManager.Instnace.isDynamicDataLoaded)             {
                manager.DestroyTrackableData();                 manager.OnCloseAndNavigate();             }             else
            {
                manager.DestroyTrackableData();                 ShowSurveyQuestion();             } 
        }
    }

    void ShowSurveyQuestion()
    {
        
        if ( AppManager.Instnace.popUpQuestionsResponse == "[]"
                    || AppManager.Instnace.popUpQuestionsResponse == ""
                    || AppManager.Instnace.popUpQuestionsResponse == null)
        {
            //AppManager.Instnace.messageBoxManager.ShowMessage("ShowSurveyQuestion", "ShowSurveyQuetion is [] ", "OK");
            Debug.Log("popUpQuestionResponse:");
            manager.OnCloseAndNavigate();

        }
        else
        {
            if (AppManager.Instnace.CheckTrigger(WebService.Instnace.catDetailParameter.HubTrigger.ToString()))
            {
                Debug.Log("CheckTrigger");
                ACPUnityPlugin.Instnace.DisableButtons();
                manager.OnCloseAndNavigate();
            }
            else
            {
                //jeetesh - change this here.
                Debug.Log("ShowSurvayPopup");
                AppManager.Instnace.ShowSurvayPopup();
            }
        }
    }

	public void HomeButtonTapped ()
	{
		SetZeroFrameIndex ();
		//		manager.setNavigationButtonsEnabled (false);
	}

	public void CloseButtonTapped ()
	{
		//ACPUnityPlugin.Instnace.DisableButtons ();

        if (AppManager.Instnace.isVuforiaOn)
        {
            manager.m_ScanLine.ShowScanLine(true);
            Debug.Log("CloseButtonTapped Scan: true");
        }
        else
        {
            manager.m_ScanLine.ShowScanLine(false);
            Debug.Log("CloseButtonTapped Scan: false");
        }

		areaFrameHistory.Clear ();
		isVisible = false;
		disappearTime = Time.time;

		Renderer[] rendererComponents = GetComponentsInChildren<Renderer> ();

		// Disable rendering:
		foreach (Renderer component in rendererComponents) {
			component.enabled = false;
		}

		this.UnLoadAreas ();
		if(preloader != null)
			RemovePreloader ();  //Jeetesh - Added this here
		//		manager.setCloseButtonVisibility (false);
		manager.setBackButtonVisibility(false);

		if (jsonWWW != null) {
			jsonWWW.Dispose ();
		}
	}
	#endregion

	#region Areas

	private void LoadAreas ()
	{
        CameraDevice.Instance.SetFocusMode(CameraDevice.FocusMode.FOCUS_MODE_NORMAL);
		// Kick off loading all of our stuff
		#if UNITY_EDITOR
		//Debug.Log("Loading areas for trackable: " + data.id);
		#endif

		// Only start loading once...
		if (hasLoaded)
			return;
		hasLoaded = true;

		// Bit of a hack to get sticky behaviour to kick in after loading...
		StartCoroutine ("LoadAreasData");
	}

	private IEnumerator LoadAreasData ()
	{
		//yield return new WaitForSeconds (1.0f);

		#if UNITY_EDITOR
		//Debug.Log ("Loading " + data.areas.Count + " areas");
		#endif
		ACPUnityPlugin.Instnace.trackingEvent ("TriggerStart", data.id, -1, -1, -1, null, null, null, null, null, -1.0f);
        ACPUnityPlugin.Instnace.trackEvent("Trigger", "TriggerStart", data.id.ToString(), 0);
		trackingTriggerEndSent = false;

		if (data != null) {
			// Tell the Native UI about what the share menu should do!
			if (!string.IsNullOrEmpty (data.share_url)) {
				// use trigger share item if available
				ACPUnityPlugin.Instnace.setShareItem (data.id, data.share_url, data.share_title, data.share_text);
			} else if (!string.IsNullOrEmpty (data.channel_share_link)) {
				// fall back to the channel share item if available
				ACPUnityPlugin.Instnace.setShareItem (data.id, data.channel_share_link, data.channel_share_title, data.channel_share_text);
			} else {
				// no share item
				ACPUnityPlugin.Instnace.setShareItem (data.id, null, null, null);
			}
		}

		// Hide the men ubutton etc
		//ACPUnityPlugin.setNativeUIVisible (false);

		// Create all of our areas!
		for (int i = 0; i < data.areas.Count; i++) {
			GameObject go = new GameObject ();
			//go.transform.localScale = new Vector3 (1, 1, 1);
			//			go.transform.parent = this.AugmentationObject.transform;
			//			go.transform.localPosition = Vector3.zero;

			//Debug Text
			//			Transform textObj = GameObject.FindGameObjectWithTag("Debug").transform.GetChild(0);
			//			textObj.GetComponent<Text> ().text = "x: " + go.transform.position.x + "y: " + go.transform.position.y + "z: " + go.transform.position.z;
			Debug.Log("AreaBehavior behaviour");

			AreaBehavior behaviour = go.AddComponent<AreaBehavior> ();
			behaviour.areaPageUrl = String.Format ("/ar/{0}/{1}/a{2}", data.channel_tracking_id, data.tracking_id, i + 1);
			behaviour.data = data.areas [i];
			go.transform.parent = this.AugmentationObject.transform;
			go.transform.localPosition = Vector3.zero;

			// Subscribe to the loaded event so we know when we can remove the preloader and start doing stuff.
			behaviour.AreaLoaded += AreaLoaded;
			behaviour.AreaLoadProgress += AreaLoadProgress;
			behaviour.ActivateAreaFrame += ActivateAreaFrame; //activate area frame for auto stack nativation handling

			areas.Add (behaviour);
			// keep references to these badboys...
		}

		// Calculate the size of the areas for snap to purposes
		//
		//first get the total area sizes
		//	Debug.Log ("Calculating display size...");
		bool first = true;


		#if USE_OLD_AREA_SNAPTO_SIZING
		/*
		* AREA SIZING - https://visualjazz.jira.com/browse/VR-529
		*/

		foreach (AreaBehavior area in areas){
		Vector3 size = new Vector3(area.data.size.x, 0.0f, area.data.size.z);
		if (first) {
		this.allAreaBounds = new Bounds(area.data.position, size);
		Debug.Log ("..all area bounds (first) " + this.allAreaBounds);
		first = false;
		} else {
		this.allAreaBounds.Encapsulate(new Bounds(area.data.position, size));
		Debug.Log ("..all area bounds (next) " + this.allAreaBounds);
		}
		Debug.Log ("..all AREA bounds (final) " + this.allAreaBounds);
		}

		#else
		/*
		* WIDGET SIZING - https://visualjazz.jira.com/browse/VR-529
		*/

		foreach (AreaBehavior area in areas) {
			//the new way is to get the bounding size of all the WIDGETS in the area - but ONLY if the widget is completely in the area. For widgets fully or partially outside the area we should not include them. Widgets not active in SnapTo mode are also ignored
			Vector3 areaSize = new Vector3 (area.data.size.x, 0.0f, area.data.size.z);
			foreach (FrameData frame in area.data.frames) {
				foreach (WidgetData widget in frame.widgets) {
					//					Debug.LogWarning("** !!! Evaluating Widget  " + widget.name);
					if (widget.snapToMode == WidgetData.SnapToMode.AR) {
						Debug.LogWarning ("   IGNORING  " + widget.name + "(" + widget.GetType () + ") beacuse its not shown in SnapTo Mode");
					} else if (widget.ignoreWidgetInSnapToLayout) {
						Debug.LogWarning ("   IGNORING  " + widget.name + "(" + widget.GetType () + ") beacuse its .ignoreSizeInSnapTo property is true");
					} else {
						Vector3 adjustedPos = new Vector3 (widget.position.x + (areaSize.x / 2) - (widget.size.x / 2), 0, widget.position.z + (areaSize.z / 2) - (widget.size.z / 2)); // widget position is relative to the CENTER of an area - so 0,0 is the center.
						//						Debug.Log ("**   Area: " + area.data.position.x + ", " + area.data.position.y + ", " + area.data.position.z + " Size: " + area.data.size.x + ", " + area.data.size.y + ", " + area.data.size.z + " - "+ widget.name);
						//						Debug.Log ("**   Widget Position: " + widget.position.x + ", " + widget.position.y + ", " + widget.position.z + " Size: " + widget.size.x + ", " + widget.size.y + ", " + widget.size.z + " - "+ widget.name);
						//					Debug.Log ("   Adjusted Position: " + adjustedPos.x + ", " + adjustedPos.y + ", " + adjustedPos.z + " Size: " + widget.size.x + ", " + widget.size.y + ", " + widget.size.z + " - "+ widget.name);
						if ((adjustedPos.x >= -0.01f) && (adjustedPos.z >= -0.01f)) {
							if ((adjustedPos.x + widget.size.x) <= (area.data.size.x + 0.01)) {
								if (((0 - adjustedPos.z) + widget.size.z) <= (area.data.size.z + 0.01)) {
									//Debug.LogWarning("**   ADDING  " + widget.name + " (" + widget.GetType()+")");
									Vector3 position = new Vector3 (area.data.position.x + widget.position.x, 0, area.data.position.z + widget.position.z);
									Bounds bounds = new Bounds (position, widget.size);
									//									Debug.Log ("** ..all widget bounds (BEFORE ADDING " + widget.name + ") " + this.allAreaBounds);
									//									Debug.Log ("** adding bounds for "+widget.name + ": " + bounds);
									if (first) {
										this.allAreaBounds = bounds;
										//Debug.Log ("** ..all widget bounds (first) " + this.allAreaBounds);
										first = false;
									} else {
										this.allAreaBounds.Encapsulate (bounds);
										//										Debug.Log ("** ..all widget bounds (AFTER ADDING " + widget.name + ") " + this.allAreaBounds);
									}
								} else {
									//Debug.LogWarning("**   IGNORING  " + widget.name + " because its HEIGHT makes it fall outside the area" + " - "+ widget.name);
								}
							} else {
								//Debug.LogWarning("**   IGNORING  " + widget.name + " because its WIDTH makes it fall outside the area" + " - "+ widget.name);
								//								Debug.LogWarning( "**   adjustedPosX+ widget.size.x =  " + (adjustedPos.x + widget.size.x) + " - "+ widget.name);
								//								Debug.LogWarning( "**   area.data.size.x =  " + area.data.size.x + " - "+ widget.name);
							}
						} else {
							//Debug.LogWarning("**   IGNORING  " + widget.name + " because its origin is outside the area" + " - "+ widget.name);						
						}
					}
				}
			}
		}
		#endif
		//re-center this 
		//this.allAreaBounds = new Bounds(Vector3.zero, new Vector3(this.allAreaBounds.size.x - this.allAreaBounds.center.x*2, 0 ,this.allAreaBounds.size.z - this.allAreaBounds.center.z*2));
		//this.allAreaBounds = new Bounds(Vector3.zero, this.allAreaBounds.size);

		//Debug.Log ("** ..all widget bounds ADJUSTED " + this.allAreaBounds);
		yield return true;
	}

	private void ActivateAreaFrame (AreaBehavior area, int frame)
	{

		//we now want to ALWAYS have navButtons enabled - but now we only care about area 0
		if (areas.IndexOf (area) == 0) {
			//determine if we're going back at all or going forward
			AreaFrameHistory afh = new AreaFrameHistory ();
			afh.area = area;
			afh.frame = frame;

			if (areaFrameHistory.Contains (afh)) {
				//Debug.Log ("history already contains this area & frame, going back to it");
				//pop back to this one
				//Jeetesh - Major change Commented the below line to make the content removed when it frame 0.
				while (areaFrameHistory.Peek ().Equals (afh) == false) {
				    areaFrameHistory.Pop ();
				}
			} else {
				//add to the stack
				//Debug.Log ("Adding Area Frame to History");
				areaFrameHistory.Push (afh);
			}
			//Debug.Log ("History Stack has " + areaFrameHistory.Count + " items");
			//			manager.setNavigationButtonsVisibility (true);
			//			manager.setNavigationButtonsEnabled (areaFrameHistory.Count > 1);
		}
	}

	private void AreaLoadProgress (AreaBehavior area, float progress)
	{
		//our first 10% is reserved for the json download
		//our last 10% is reserved for when we actually becomem active
		area.preloadPercent = progress;

		float totalPercent = 0;
		int i = 1;
		foreach (AreaBehavior a in areas) {
			totalPercent += a.preloadPercent / areas.Count;
			//Debug.Log ("Area " + i + " has loaded "+a.preloadPercent);
			i++;
		}
		if (totalPercent <= 1.0f) {
			ACPUnityPlugin.Instnace.setDownloadProgress (0.1f + totalPercent * 0.8f);
		} else {
			//Debug.Log ("more than 100%?");
		}

		if (preloader) {
			preloader.GetComponent<Spinner> ().Progress = progress;
		}
	}


	//Jeetesh - Most important function for AR thing.
	private void AreaLoaded (AreaBehavior area)
	{
		AppManager.Instnace.isDynamicDataLoaded = true;
		// When an area has loaded,  check to see if we have any more to go
		// If we have loaded everything, show the first frame for every area
		area.AreaLoaded -= AreaLoaded;

		bool loadedEverything = true;
		foreach (AreaBehavior a in areas) {
			if (!a.PreloadFinished) {
				loadedEverything = false;
				break;
			}
		}

		//	Debug.Log ("AreaLoaded, LoadedEverything: " + loadedEverything);
		if (loadedEverything) {
			SetZeroFrameIndex ();
			ACPUnityPlugin.Instnace.hideStatusoverlay ();

			//Debug.Log("Loaded all areas!");
			//		Debug.Log ("AreaLoaded, sticky: " + this.Sticky);
			if (this.Sticky) {
				area.Sticky = this.Sticky;
				area.setAreaPosition ();
				//Jeetesh - This is commented so that the content will not move in 3D space and will remain attached to camera.
				DetatchContentFromCamera ();
				AttachContentToCamera ();

			}
			// Kill the preloader

			if (preloader != null) {
				StartCoroutine (RemovePreloader ());
			}
		}
	}

	private void UnLoadAreas ()
	{
        CameraDevice.Instance.SetFocusMode(CameraDevice.FocusMode.FOCUS_MODE_CONTINUOUSAUTO);
		/*
		if (!hasLoaded)
			return;
		*/
		hasLoaded = false;

		if (areas != null) {
			for (int i = 0; i < areas.Count; i++) {
				GameObject objectToDestroy = areas [i].gameObject;
				if (objectToDestroy != null) {
					Destroy (objectToDestroy);
				}
			}
			areas.Clear ();
		}

		if (this.preloader != null) {
			Destroy (this.preloader);
		}

		if (manager != null) {
			//			manager.setCloseButtonVisibility (false);
			manager.setBackButtonVisibility(false);
		}
		#if UNITY_WEBPLAYER
		#else
		if(AppManager.Instnace.isVuforiaOn){
			Camera.main.gameObject.GetComponent<CloudRecoBehaviour> ().CloudRecoEnabled = true;
            //Jeetesh - This was changed here.
            if(manager.mtargetFinder != null)
                manager.mtargetFinder.ClearTrackables (true);
		}
		#endif

		// Destroy the augmentation object as it might not be parented if we are snap to...
		if (AugmentationObject.transform.parent != this) {
			Destroy (AugmentationObject);
		}

		SendTrackingTriggerEnd ();

		//		ACPUnityPlugin.Instnace.setNativeUIVisible (true);
		//		ACPUnityPlugin.Instnace.setShareItem (0, null, null, null);
				ACPUnityPlugin.Instnace.trackPageView ("/");

		Destroy (this.gameObject);
	}

	#endregion

	public void SetTrackingTriggerEndSent ()
	{ //native client MAY send the triggerEnd event itself, if so it will call this so we don't send it again below.
		//	Debug.Log ("ACPTrackableBehavoir - SetTrackingTriggerEndSent");
		trackingTriggerEndSent = true;		
	}

	public void SendTrackingTriggerEnd ()
	{
		//	Debug.Log ("ACPTrackableBehavoir - sendTrackingTriggerEnd - entered");
		if ((data != null) && (trackingTriggerEndSent == false)) {
			ACPUnityPlugin.Instnace.trackingEvent ("TriggerEnd", data.id, -1, -1, -1, null, null, null, null, null, -1.0f);
            ACPUnityPlugin.Instnace.trackEvent("Trigger", "TriggerEnd", data.id.ToString(), 0);
			trackingTriggerEndSent = true;		
			//		Debug.Log ("ACPTrackableBehavoir - sendTrackingTriggerEnd - sent");
		} else {
			//		Debug.Log ("ACPTrackableBehavoir - sendTrackingTriggerEnd - not sending");
		}
	}

	protected void SetZeroFrameIndex ()
	{
		foreach (AreaBehavior a in areas) {
			a.CurrentFrameIndex = 0;
		}
	}

	#region Preloader

	protected void CreatePreloader ()
	{
		preloader = (GameObject)GameObject.Instantiate (manager.preloadPrefab);
		preloader.transform.parent = Camera.main.transform; //this.transform;
		preloader.transform.localPosition = Vector3.zero;
		preloader.transform.localScale = new Vector3 (0.1f, 0.1f, 0.1f);  //jeetesh - this was commented
		preloader.GetComponent<Spinner> ().Progress = 0.0f;
	}

	protected void DetatchPreloaderFromCamera ()
	{
		if (this.preloader == null)
			return;

		//Debug.Log("Detatching preloader from camera");

		this.preloader.transform.parent = null;

		//jeetesh - check this preloader here

		this.preloader.transform.parent = this.transform;
		this.preloader.transform.position = Vector3.zero;
		this.preloader.transform.localRotation = Quaternion.identity;
		preloader.transform.localScale = new Vector3 (0.05f, 0.05f, 0.05f);
		preloaderTweenInfo.Active = false;


		//		preloaderTweenInfo.PositionFrom = preloader.transform.position;
		//		preloaderTweenInfo.PositionTo = Vector3.zero;
		//		preloaderTweenInfo.Target = preloader.transform;
		//		preloaderTweenInfo.RotationFrom = preloader.transform.rotation;
		//		preloaderTweenInfo.RotationTo = Quaternion.identity;
		//		preloaderTweenInfo.Active = true;
		//		preloaderTweenInfo.StartTime = Time.time;
	}

	public void AttachPreloaderToCamera ()
	{
		Debug.Log ("AttachPreloader");
		if (this.preloader == null) {
			return;
		}

		Debug.Log ("Attaching preloader to camera");
		this.preloader.transform.parent = Camera.main.transform;
		Vector3 newPos = Camera.main.transform.position + (Camera.main.transform.forward * 1500);
		Quaternion rotation = Camera.main.transform.rotation;
		this.preloader.transform.localScale = new Vector3 (30.0f, 30.0f, 30.0f);
		rotation *= Quaternion.Euler (-90, 0, 0);
		this.preloader.transform.position = newPos;
		this.preloader.transform.rotation = rotation;

//		preloaderTweenInfo.Active = false;

		//		preloaderTweenInfo.PositionFrom = preloader.transform.position;
		//		preloaderTweenInfo.PositionTo = newPos;
		//		preloaderTweenInfo.RotationFrom = preloader.transform.rotation;
		//		preloaderTweenInfo.RotationTo = rotation;
		//		preloaderTweenInfo.StartTime = Time.time;
		//		preloaderTweenInfo.Active = true;
		//		preloaderTweenInfo.Target = preloader.transform;
	}

	public IEnumerator RemovePreloader ()
	{
		if (preloader == null)
			yield break;
		//Debug.Log ("RemovePreloader()");
		Spinner spinner = preloader.GetComponent<Spinner> ();
		ACPUnityPlugin.Instnace.hideStatusoverlay ();

		if (!spinner.isRemoving) {

			Debug.Log ("!spinner.isRemoving");
			while (!spinner.finishedAppearing) {
				yield return new WaitForSeconds (0.25f);
			}

			// If we became sticy while loading the json, and we shouldn't actually be sticky, we need to 'exit'....
			if (this.Sticky && this.data != null && !this.data.sticky) {
				this.disappearTime = Time.time - 30.0f;
				this.Sticky = false;
				this.OnTrackingLost ();
			}

			float delay = spinner.Remove ();
			yield return new WaitForSeconds (delay);

			foreach (AreaBehavior a in areas) {
				a.CurrentFrameIndex = 0;
			}

			// Clear reference to the preloader
			this.preloader = null;
		}

		yield return true;
	}
	#endregion

	#region Slearp
	IEnumerator MoveObject (Transform myTransform, Vector3 startPos,Vector3 endPos,float time) {
		float i = 0;
		float rate = 1.0f/time;
		while (i < 1) {
			i += Time.deltaTime * rate;
			myTransform.position = Vector3.Slerp(startPos, endPos, i);
			yield return new WaitForEndOfFrame(); 
		}
	}
	IEnumerator MovePreloader (Transform myTransform, Vector3 startPos,Vector3 endPos,float time) {
		float i = 0;
		float rate = 1.0f/time;
		while (i < 1) {
			i += Time.deltaTime * rate;
			myTransform.position = Vector3.Slerp(startPos, endPos, i);
			yield return new WaitForEndOfFrame(); 
		}
	}
	#endregion

	#region Popup Detail Panel

	//IEnumerator CheckToShowSurvayPopup(){
	//	//		AppManager.Instnace.messageBoxManager.ShowPreloaderDefault ();
	//	string str = "";
	//	str = AppManager.Instnace.baseURL + "/cloud/PopupSurveyIsViewed.aspx?Tid=" + WebService.Instnace.catDetailParameter.HubTrigger + "&EmailId=" + AppManager.Instnace.userEmail;
	//	Debug.Log ("PopupSurveyIsViewed URL:" + str);

	//	WWW response = new WWW (str);

	//	yield return response;

	//	Debug.Log("Web Service Response: "+response.text);
	//	CallBackSurvayPopupIsViewed (response.text);
	//}

	//void CallBackSurvayPopupIsViewed(string response){

	//	Debug.Log ("PopupSurveyIsViewed Response:" + response);
	//	//		AppManager.Instnace.messageBoxManager.HidePreloader ();
	//	JSONNode test = JSONNode.Parse (response);

	//	if (test != null) {

	//		var res = test ["status"] == null? null:test ["status"].Value;

	//		if (res == "Trigger Not Viewed") {
	//			if (AppManager.Instnace.isVuforiaOn)
	//				AppManager.Instnace.acpTrackingManager.m_ScanLine.ShowScanLine (false);
	//			ShowSurvayPopup ();
	//		} else {
	//			//jeetesh - change this here.
	//			AppManager.Instnace.acpTrackingManager.OnCloseAndNavigate ();
	//		}
	//	}
	//}
	public void ShowSurvayPopup() {
		AppManager.Instnace.isPopUpSurvay = true;
		CanvasManager.Instnace.ShowPanelManager (ePanelManager.PopupSurvayManager);
		CanvasManager.Instnace.ReturnPanelManager (ePanelManager.PopupSurvayManager).NavigateToPanel (ePanels.QAPopup_panel);
	}

	#endregion 
    public int ReturnAreaFrameHistoryCount(){
        return areaFrameHistory.Count();
    }

    private void OnDeviceBackUpdate()
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            if (Input.GetKeyUp(KeyCode.Escape))
            {
                //GameObject[] trackables = GameObject.FindGameObjectsWithTag("Trackable");
                if (trackableLength() > 0)
                {
                    Debug.Log(">>>>>>>>>>>>>>>>>>>>>> OnDeviceBackUpdate >>>>>>>>>>>>>>>>>>>>>>>");
                    manager.OnBackButtonTapped();
                }
            }
        }
    }

    int trackableLength(){

        GameObject[] trackables = GameObject.FindGameObjectsWithTag("Trackable");
        if (trackables.Length > 0) {
            return 1;
        } else {
            return 0;
        }
    }

	private void OnApplicationFocus(bool focus)
	{
        if(focus && ACPUnityPlugin.Instnace.isExternalUrl){
            if (areaFrameHistory.Count() == 1 
                && areaFrameHistory.Peek().area.TotalItemsToLoad == 1.0f) {
                //AppManager.Instnace.messageBoxManager.ShowMessage("Message", "TotalItemsToLoad :" + areaFrameHistory.Peek().area.TotalItemsToLoad, "Ok"); 
                Debug.Log("-----------------> OnApplicationFocus ---------------------->");
                StopAllCoroutines();
                BackButtonTapped();
            }
        }
	}
}


//Jeetesh - Response stretch code backup.
//float newHeight = fullScreenCameraPixelHeight - manager.navBarHeight;  //- (manager.navBarHeight ) + 90
//#if UNITY_EDITOR
//Camera.main.pixelRect = new Rect(0, 0, Screen.width, newHeight);  //90
//#elif UNITY_ANDROID || UNITY_IOS            
//Camera.main.pixelRect = new Rect(0, 90, Screen.width, newHeight);  //90
            


