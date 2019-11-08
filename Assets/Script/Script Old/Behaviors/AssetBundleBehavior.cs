using UnityEngine;
using System.Collections;

namespace ACP {
	
	public class AssetBundleBehavior : WidgetBehavior {
		
		public AssetBundleData data;
		public GameObject assetBundleGameObject;
		private GameObject prefabObject = null;
		private AssetBundle bundle;
		private WWW www;

		public override WidgetData Data { 
			get { return data; }
		}
	
		//downloading the bundle the unity standard way
		IEnumerator Start() {
			transform.localPosition = data.position;
			transform.localRotation = Quaternion.Euler (data.rotation);

			Debug.Log ("AssetBundle: Loading from : " + data.AssetBundleUrl);
			www = WWW.LoadFromCacheOrDownload (data.AssetBundleUrl, 0);
			yield return www;

			if(www.assetBundle){
				bundle = www.assetBundle;

                // Jeetesh - Below code was commented ----
                // Jeetesh - if data.objectName = null or "" give it a appropriate name to avoid null reference error.
                if(data.objectName == null || data.objectName == ""){
                    data.objectName = "PlaceholderName";
                }
				AssetBundleRequest assetBundleRequest = bundle.LoadAssetAsync(data.objectName, typeof(GameObject));
				yield return assetBundleRequest;
				
				if(assetBundleRequest == null){
					Debug.LogError ("AssetBundleRequest is null!");
				}
				//----------------------------------------
				
				Debug.Log ("AssetBundle: Loaded "+data.objectName);
                prefabObject = (GameObject) Instantiate(bundle.mainAsset,new Vector3(0,0,0), Quaternion.identity);
				prefabObject.transform.transform.parent = this.transform;
				if(prefabObject == null){
					Debug.LogError ("prefabObject is null!");
				} else {
					if(isSticky()){
						TrackingLost(true);
					} else {
						TrackingFound(true);
					}
				}
				www.Dispose();
				www = null;
				Resources.UnloadUnusedAssets();
				ACPUnityPlugin.Instnace.setDownloadProgress(1f);
			}
		}

/*
		// Utilising Download Cache - THIS IS LEAKING MEMORY (WebStream)
		IEnumerator Start () {
			byte[] bytes;

			Debug.Log ("AssetBundle: Loading from : " + data.AssetBundleUrl);

			bytes = this.GetFile(data.AssetBundleUrl);

			AssetBundleCreateRequest request = AssetBundle.CreateFromMemory(bytes);
			yield return request;

			bundle = request.assetBundle;
			if(bundle == null){
				Debug.LogError ("AssetBundle is null!");
			}

			AssetBundleRequest assetBundleRequest = bundle.LoadAsync(data.objectName, typeof(GameObject));
			yield return assetBundleRequest;

			if(assetBundleRequest == null){
				Debug.LogError ("AssetBundleRequest is null!");
			}

			Debug.Log ("AssetBundle: Loaded "+data.objectName);
			assetBundleObject = (GameObject) Instantiate(assetBundleRequest.asset);
			assetBundleObject.transform.transform.parent = this.transform;
			if(assetBundleObject == null){
				Debug.LogError ("AssetBundleObject is null!");
			}

			Resources.UnloadUnusedAssets();
		} 
*/

		// Update is called once per frame
		void Update () {		
			if(www != null){
				ACPUnityPlugin.Instnace.setDownloadProgress(www.progress * 0.9f);
			}
			//keep aligned to the area
            if(assetBundleGameObject != null) {
                assetBundleGameObject.transform.localPosition = new Vector3(0 - assetBundleGameObject.transform.parent.localPosition.x, 0, 0 - assetBundleGameObject.transform.parent.localPosition.z);
                assetBundleGameObject.transform.localRotation = Quaternion.identity;
                assetBundleGameObject.transform.localScale = Vector3.one;
            }
		}

		public void OnDestroy ()
		{
			bundle.Unload(true);
			Destroy(bundle);
			Destroy(prefabObject);
			prefabObject = null;
			Destroy(gameObject);
			Resources.UnloadUnusedAssets();
			Caching.ClearCache();
			System.GC.Collect();
			Debug.Log ("Destroyed AssetBundleBehavoir");
		}

		public override void Remove()
		{
			Debug.Log ("Removing AssetBundleBehavoir");
			Destroy(gameObject);

		}

		public override void TrackingFound(bool sticky)
		{
            Debug.Log("AssetBundle TrackingFound");
			prefabObject.SendMessage("TrackingFound", sticky);
		}

		public override void TrackingLost(bool sticky)
		{
			Debug.Log ("AssetBundle TrackingLost");
			if(prefabObject != null){
				prefabObject.SendMessage("TrackingLost", sticky);
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
			AreaBehavior ab = areaObject().GetComponent<AreaBehavior>();
			if(ab){
				Debug.Log ("Area is sticky? " + ab.Sticky);
				return ab.Sticky;
			} else {
				Debug.LogWarning ("Cant find area!");
				return false;
			}
		}
	}

}