using System;
using System.Collections.Generic;
using UnityEngine;

namespace ACP
{
	public class ButtonData : WidgetData
	{
		public enum ButtonAction
		{
			Page,
			Share,
			Link,
			Music,
			Gallery,
			Video,
			PhotoBooth,
			Map,
		}

		public ButtonAction action = ButtonData.ButtonAction.Link;
		
		public string imageUrl;
		public string imagePressedUrl;
		public string imageReflectionUrl;

		public string shareText;
		public string shareTitle;
		public string shareImage;
		public string shareUrl;
		
		public string photoboothOverlayImageUrl;
		
		public string linkUrl;
		public bool linkExternal;
		public bool fullscreenView;
		
		public int pageIndex;
		
		// Used for the end of fullscreen videos
		public string endImage;
		public string endUrl;

		//for maps
		public double mapLatitude;
		public double mapLongtitude;
		public string mapTitle;
		public string mapDescription;

		public ButtonData ()
		{
		}

		public static ButtonData Create (FrameData frameData, int id, JSONObject buttonJson)
		{
			ButtonData buttonData = new ButtonData ();
			
			try {
				WidgetData.Create (frameData, buttonData, id, buttonJson);
				
				switch (buttonJson["widget_type"].str) {
				case "button_frame":
					buttonData.action = ButtonAction.Page;
					JSONObject frameObj = buttonJson["frame_index"];
					if (frameObj != null)
						buttonData.pageIndex = (int)frameObj.n;
					buttonData.linkUrl = "na";
					break;
				case "button_share":
					buttonData.action = ButtonAction.Share;
					buttonData.shareTitle = buttonJson["share_title"].str;
					buttonData.shareText = buttonJson["share_text"].str;
					
					JSONObject share_image = buttonJson["share_image"];
					if (share_image != null && share_image.str != "")
						buttonData.shareImage = share_image.str;
					
					buttonData.linkUrl = buttonJson["share_url"].str;
					break;
				case "button_link":
					buttonData.action = ButtonAction.Link;
//					buttonData.linkUrl = buttonJson["link_url"].str;

					JSONObject linkUrl = null;
					#if UNITY_IPHONE
					linkUrl = buttonJson["link_url_ios"];
					#elif UNITY_ANDROID
					linkUrl = buttonJson["link_url_android"];
					#endif
					if (linkUrl != null && linkUrl.str != "")
						buttonData.linkUrl = linkUrl.str;
					else
						buttonData.linkUrl = buttonJson["link_url"].str;
					
					JSONObject linkExternalJson = buttonJson["link_external"];
					if (linkExternalJson != null)
					{
						buttonData.linkExternal = linkExternalJson.b;
					}
					
					JSONObject fullsreenViewJson = buttonJson["fullscreen_view"];
					if (fullsreenViewJson != null)
					{
						buttonData.fullscreenView = fullsreenViewJson.b;
					}
					
					buttonData.shareTitle = buttonJson.StringOrNull ("share_title");
					buttonData.shareText = buttonJson.StringOrNull ("share_text");
					buttonData.shareUrl = buttonJson.StringOrNull ("share_url");

					break;
				case "button_music":
					buttonData.action = ButtonAction.Music;
					buttonData.linkUrl = buttonJson["music_url"].str;
					break;
				case "button_gallery":
					buttonData.action = ButtonAction.Gallery;
					buttonData.linkUrl = buttonJson["gallery_url"].str;
					break;
				case "button_video":
					buttonData.action = ButtonData.ButtonAction.Video;
					buttonData.linkUrl = buttonJson.StringOrEmpty ("video_url");
					
					if (buttonJson["end_url"] != null && buttonJson["end_url"].str != "")
						buttonData.endUrl = buttonJson["end_url"].str;
					
					if (buttonJson["end_image"] != null && buttonJson["end_image"].str != "")
						buttonData.endImage = buttonJson["end_image"].str;
					
					buttonData.shareTitle = buttonJson.StringOrNull ("share_title");
					buttonData.shareText = buttonJson.StringOrNull ("share_text");
					buttonData.shareUrl = buttonJson.StringOrNull ("share_url");
					
					break;
				case "button_photobooth":
					buttonData.action = ButtonData.ButtonAction.PhotoBooth;
					buttonData.photoboothOverlayImageUrl = buttonJson["overlay_image"].str;
					break;
				case "button_map":
					//Debug.Log ("Button Map Created!");
					buttonData.action = ButtonData.ButtonAction.Map;
					buttonData.mapLatitude = buttonJson["latitude"].n;
					buttonData.mapLongtitude = buttonJson["longitude"].n;
					if(buttonJson["title"] != null){
						buttonData.mapTitle = buttonJson["title"].str;
					}
					if(buttonJson["description"] != null){
						buttonData.mapDescription = buttonJson["description"].str;
					}
					break;

				default:
					Debug.LogError ("Unknown action type on button: " + buttonJson["widget_type"].str);
					break;
				}
				
				if (buttonJson["image"] != null && buttonJson["image"].str != "")
					buttonData.imageUrl = buttonJson["image"].str;
				if (buttonJson["image_pressed"] != null && buttonJson["image_pressed"].str != "")
					buttonData.imagePressedUrl = buttonJson["image_pressed"].str;
				if (buttonJson["image_reflection"] != null && buttonJson["image_reflection"].str != "")
					buttonData.imageReflectionUrl = buttonJson["image_reflection"].str;
				
				// If no link - unless we are a photo booth or a map
				if((buttonData.action != ButtonData.ButtonAction.PhotoBooth) && (buttonData.action != ButtonData.ButtonAction.Map)){
					if (buttonData.linkUrl == null || buttonData.linkUrl == "")
					{
						Debug.Log ("Invalid button data");
						return null;
					}
				}

				//bed - 25 June 2014 - allow buttons with no images now (for assetBundles and hooking into hidden widgets
/*				if ((buttonData.imageUrl == null || buttonData.imageUrl == "") && !buttonData.autoPlay)
				{
					Debug.Log ("Button is missing required image");
					return null;
				}
*/

				//bed - 10th Oct 2014 - however - if our imageUrl is null OR ends with 'transparent.png' we will ignore it for snapTo sizing purposes
				if((buttonData.imageUrl == null) || (buttonData.imageUrl.EndsWith("transparent.png",true,null))){
					//the pressed image needs to be the same too
					if((buttonData.imagePressedUrl == null) || (buttonData.imagePressedUrl.EndsWith("transparent.png",true,null))){
						buttonData.ignoreWidgetInSnapToLayout = true;
					}
				}
			}
			catch (Exception ex) {
				Debug.Log ("Exception creating ButtonData: " + ex);
				return null;
			}

			return buttonData;
		}
		
		public override WidgetBehavior CreateBehavior (AreaBehavior parent, int index)
		{
			Debug.Log ("WidgetBehavior CreateBehavior");
			//GameObject button = GameObject.CreatePrimitive(PrimitiveType.Plane);
			GameObject button = new GameObject ();
			if(parent != null){
				button.transform.parent = parent.transform;
			}
			ButtonBehavior bh = button.AddComponent<ButtonBehavior> ();
			bh.data = this;
			bh.index = index;
			if(parent != null){
				bh.ButtonPressed += parent.ButtonClicked;
			}
			bh.plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
			bh.plane.transform.parent = button.transform;
			return bh;
		}
		
		override public string[] TexturesRequired() 
		{
			List<string> images = new List<string>();
			if (imageUrl != null && imageUrl.Length>0) images.Add(imageUrl);
			if (imagePressedUrl != null && imagePressedUrl.Length>0) images.Add(imagePressedUrl);
			if (imageReflectionUrl != null && imageReflectionUrl.Length>0) images.Add(imageReflectionUrl);
			
			return images.ToArray();		
		}

	}
}

