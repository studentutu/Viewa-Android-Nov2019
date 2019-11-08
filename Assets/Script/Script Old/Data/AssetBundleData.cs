using System;
using System.Collections.Generic;
using UnityEngine;

namespace ACP
{
	public class AssetBundleData : WidgetData
	{
		public string iosAssetBundleUrl;
		public string androidAssetBundleUrl;
		public string webAssetBundleUrl;
		public string objectName;

		public AssetBundleData ()
		{
			this.ignoreWidgetInSnapToLayout = true; //we dont want to include the size of the asset bundle widget
		}

		public static AssetBundleData Create (FrameData frameData, int id, JSONObject assetBundleJson)
		{
			AssetBundleData assetBundleData = new AssetBundleData ();
			
			try {
				WidgetData.Create (frameData, assetBundleData, id, assetBundleJson);
				assetBundleData.iosAssetBundleUrl = assetBundleJson["asset_bundle_ios_url"].str;
				assetBundleData.androidAssetBundleUrl = assetBundleJson["asset_bundle_android_url"].str;
				assetBundleData.webAssetBundleUrl = assetBundleJson["asset_bundle_web_url"].str;
				assetBundleData.objectName = assetBundleJson["asset_object_name"].str;
			}
			catch (Exception ex) {
				Debug.Log ("Exception creating AssetBundleData: " + ex);
				return null;
			}

			return assetBundleData;
		}
		
		public override WidgetBehavior CreateBehavior (AreaBehavior parent, int index)
		{
			GameObject assetBundleGameObject = new GameObject ();
			assetBundleGameObject.name = "AssetBundleGameObject";
			assetBundleGameObject.transform.parent = parent.transform;

			AssetBundleBehavior bh = assetBundleGameObject.AddComponent<AssetBundleBehavior> ();
			bh.data = this;
			bh.index = index;
			bh.assetBundleGameObject = assetBundleGameObject;
			return bh;
		}
		
/* 
   		//utilising downloadCache to d/l the bundle
		override public string[] FilesRequired()
		{
			List<string> files = new List<string>();
#if UNITY_EDITOR
			if (webAssetBundleUrl != null && webAssetBundleUrl.Length>0) files.Add(webAssetBundleUrl);
#elif UNITY_IOS
			if (iosAssetBundleUrl != null && iosAssetBundleUrl.Length>0) files.Add(iosAssetBundleUrl);
#elif UNITY_ANDROID
			if (androidAssetBundleUrl != null && androidAssetBundleUrl.Length>0) files.Add(androidAssetBundleUrl);
#endif
			return files.ToArray();
		}
*/
		public string AssetBundleUrl
		{
			get { 
#if UNITY_EDITOR
            return androidAssetBundleUrl;
				//return webAssetBundleUrl;
#elif UNITY_ANDROID
				return androidAssetBundleUrl;
#elif UNITY_IOS
				return iosAssetBundleUrl;
#endif
			}
		}
	}
}

