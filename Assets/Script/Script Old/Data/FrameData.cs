using System;
using System.Collections.Generic;
using UnityEngine;

namespace ACP
{
	public class FrameData
	{
		public AreaData parentAreaData;
		public int id;
		public List<WidgetData> widgets;
		
		public FrameData ()
		{
			widgets = new List<WidgetData>();
		}
		
		public static FrameData Create (AreaData areaData, int id, JSONObject pageJson)
		{
			FrameData FrameData = new FrameData ();
			FrameData.parentAreaData = areaData;
			
			JSONObject widgetsJson = pageJson ["widgets"];
			if (widgetsJson != null) {
				for (int i = 0; i < widgetsJson.Count; i++) {
					JSONObject widgetJson = widgetsJson [i];
					
					JSONObject typeObject = widgetJson ["widget_type"];
					if (typeObject == null) {
						continue;
					}
					string type = typeObject.str;
					
					switch (type) {
					case "button_frame":
					case "button_share":
					case "button_link":
					case "button_music":
					case "button_gallery":
					case "button_video":
					case "button_photobooth":
					case "button_map":
						ButtonData buttonData = ButtonData.Create (FrameData, i, widgetJson);
						if (buttonData != null) {
							FrameData.widgets.Add (buttonData);
						}
						break;
					case "ar_gallery": 
						//an ar gallery is an integrated widget with its own behavoir 
						ARGalleryData agData = ARGalleryData.Create (FrameData, i, widgetJson);
						if (agData != null) {
							FrameData.widgets.Add (agData);
						}
						break;

					case "image":
						ImageData imageData = ImageData.Create (FrameData, i, widgetJson);
						if (imageData != null) {
							FrameData.widgets.Add (imageData);
						}
						break;
					case "video":
						VideoData videoData = VideoData.Create (FrameData, i, widgetJson);
						
						if (videoData != null) {
							FrameData.widgets.Add (videoData);
						}
						break;
					case "sound":
						SoundData soundData = SoundData.Create (FrameData, i, widgetJson);
						if (soundData != null) {
							FrameData.widgets.Add (soundData);
						}
						break;
					case "model_obj":
						ModelObjData modelData = ModelObjData.Create (FrameData, i, widgetJson);
						if (modelData != null) {
							FrameData.widgets.Add (modelData);
						}
						break;
					case "asset_bundle":
						AssetBundleData assetBundleData = AssetBundleData.Create (FrameData, i, widgetJson);
						if (assetBundleData != null) {
							FrameData.widgets.Add(assetBundleData);
						}
						break;
					default:
						Debug.Log ("UNKNOWN WIDGET TYPE: " + type);
						break;
					}
				}
			}
					
			return FrameData;
		}
	}
}

