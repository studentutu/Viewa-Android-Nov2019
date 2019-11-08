using System;
using System.Collections.Generic;
using UnityEngine;

namespace ACP
{
	public class VideoData : WidgetData
	{
		public string imageUrl;
		public string imagePressedUrl;
		public string imageMaskUrl;
		public string imageReflectionUrl;
		public string imageLoadingUrl;
		public bool loopVideo;
		public string endImage;
		public string endUrl;
		
		public string videoUrl;
		
		public bool allowFullscreen;
		
		public bool alphaSBS = false;
		
		public VideoData ()
		{
		}

		public static VideoData Create (FrameData frameData, int id, JSONObject jsonObject)
		{
			VideoData videoData = new VideoData ();
			
			
			if (jsonObject["image"] != null && jsonObject["image"].str != "")
				videoData.imageUrl = jsonObject["image"].str;
			
			if (jsonObject["image_pressed"] != null && jsonObject["image_pressed"].str != "")
				videoData.imagePressedUrl = jsonObject["image_pressed"].str;
			
			if (jsonObject["image_loading"] != null && jsonObject["image_loading"].str != "")
				videoData.imageLoadingUrl = jsonObject["image_loading"].str;
			
			if (jsonObject["image_mask"] != null && jsonObject["image_mask"].str != "")
				videoData.imageMaskUrl = jsonObject["image_mask"].str;
			else if (jsonObject["image_reflection"] != null && jsonObject["image_reflection"].str != "")
				videoData.imageReflectionUrl = jsonObject["image_reflection"].str;
			
			JSONObject loop = jsonObject["loop_video"];
			if (loop != null)
				videoData.loopVideo = loop.b;
			else
				videoData.loopVideo = false;

			videoData.videoUrl = jsonObject["video_url"].str;
			
			if (jsonObject["end_url"] != null && jsonObject["end_url"].str != "")
				videoData.endUrl = jsonObject["end_url"].str;
				
			if (jsonObject["end_image"] != null && jsonObject["end_image"].str != "")
				videoData.endImage = jsonObject["end_image"].str;
			
			JSONObject temp = jsonObject["allow_fullscreen"];
			if (temp != null)
				videoData.allowFullscreen = temp.b;
			else
				videoData.allowFullscreen = false;
			
			JSONObject alphaSBS = jsonObject["transparent_video"];
			if (alphaSBS != null)
				videoData.alphaSBS = alphaSBS.b;
			
			WidgetData.Create (frameData, videoData, id, jsonObject);
			
			if (videoData.videoUrl == null || videoData.videoUrl == "")
				return null;
			
			if ((videoData.imageUrl == null || videoData.imageUrl == "") && !videoData.autoPlay)
			{
				return null;
			}
			return videoData;
		}
		
		public override WidgetBehavior CreateBehavior (AreaBehavior parent, int index)
		{
			GameObject video = new GameObject ();
			video.transform.parent = parent.transform;
			
			VideoBehavior bh = video.AddComponent<VideoBehavior> ();
			bh.data = this;
			bh.index = index;
			// Create plane to hold the video playback
			bh.videoPlane = GameObject.CreatePrimitive(PrimitiveType.Plane);
			bh.videoPlane.transform.parent = video.transform;
			// Create plane to hold the images etc.
			bh.imagePlane = GameObject.CreatePrimitive(PrimitiveType.Plane);
			bh.imagePlane.transform.parent = video.transform;
			return bh;
		}
		
		override public string[] TexturesRequired() 
		{
			List<string> images = new List<string>();
			if (imageUrl != null && imageUrl.Length>0) images.Add(imageUrl);
			if (imagePressedUrl != null && imagePressedUrl.Length>0) images.Add(imagePressedUrl);
			if (imageLoadingUrl != null && imageLoadingUrl.Length>0) images.Add(imageLoadingUrl);
			if (imageMaskUrl != null && imageMaskUrl.Length>0) images.Add(imageMaskUrl);
			else if (imageReflectionUrl != null && imageReflectionUrl.Length>0) images.Add(imageReflectionUrl);
            if (endImage != null && endImage.Length > 0) images.Add (endImage);
			
			return images.ToArray();
		}
		
		public static Dictionary<string, string> ParseQueryString (string queryString)
		{
			string[] parameters = queryString.Split ('&');

			Dictionary<string, string> qsCollection = new Dictionary<string, string> ();
			
			foreach (string param in parameters) {
				
				string[] keyvalue = param.Split ('=');
				
				if (keyvalue.Length == 2)
				{
					qsCollection[keyvalue[0]] = keyvalue[1];
				}
			}
			
			return qsCollection;
		}
		
		public static string getYoutubeVideoInfoUrl (string url)
		{
			string videoIdStr = url.Substring (5, url.Length - 5);
			string infoUrl = string.Format ("http://www.youtube.com/get_video_info?video_id={0}&fmt=6", WWW.EscapeURL (videoIdStr));
			return infoUrl;
		}
		
		public static string getMp4UrlFromVideoInfo (string videoInfo)
		{
			string videoUrl = "";
			Debug.Log (videoInfo);
			//Let's make a request to youtube for the direct URL to the mp4 file.
			//    		System.Net.WebClient wc = new System.Net.WebClient();
			//        	string videoInfo = wc.DownloadString(string.Format("http://www.youtube.com/get_video_info?video_id={0}&fmt=6", videoIdStr));
			if (!string.IsNullOrEmpty (videoInfo)) {
				Dictionary<string, string> qsCollection = ParseQueryString (videoInfo);
				
				if (qsCollection.ContainsKey ("url_encoded_fmt_stream_map")) {
					string videoUrlList = WWW.UnEscapeURL (qsCollection["url_encoded_fmt_stream_map"]);
					
					if (!string.IsNullOrEmpty (videoUrlList)) {
						videoUrlList = videoUrlList.Replace ("url=", "|url=");
						string[] videoUrlArray = videoUrlList.Split ('|');
						
						//Let's loop through these urls and find the mp4 video.
						string mp4Url = "";
						string threegppUrl = "";

						foreach (string v in videoUrlArray) {
							Dictionary<string, string> streamQsCollection = ParseQueryString (v);
							
							if (streamQsCollection.Keys.Count > 0) {
								//Debug.Log ("got " + streamQsCollection.Keys.Count + " keys");
								if (streamQsCollection.ContainsKey ("type") && streamQsCollection.ContainsKey ("quality") && streamQsCollection.ContainsKey ("url")) {
									string type = WWW.UnEscapeURL (streamQsCollection["type"]);
									string quality = WWW.UnEscapeURL (streamQsCollection["quality"]);
									#if UNITY_ANDROID
									string url = WWW.UnEscapeURL (WWW.UnEscapeURL (streamQsCollection["url"]));
									#else
									string url = WWW.UnEscapeURL(WWW.UnEscapeURL (streamQsCollection["url"]));
									#endif
									

									//Debug.Log (string.Format ("type: {0}, quality: {1}, url: {2}", type, quality, url));
									//Now let's look for the "video/mp4" mimetype
									if (!string.IsNullOrEmpty (type) && type.Contains ("video/mp4") 
										&& !string.IsNullOrEmpty (quality) && quality.Contains ("medium") 
										&& !string.IsNullOrEmpty (url)) {
										mp4Url = url;
										//Jeetesh - Uncommented the below 3 lines.
										videoUrl = url;
										Debug.Log ("Got video url: " + videoUrl);
										break;
									} else if (!string.IsNullOrEmpty (type) && type.Contains ("video/3gpp") && !string.IsNullOrEmpty (url)) {
										threegppUrl = url;
									}
								}
							}
						}
						
						if (!string.IsNullOrEmpty (mp4Url)) {
							videoUrl = mp4Url;
						} else if (!string.IsNullOrEmpty (threegppUrl)) {
							videoUrl = threegppUrl;
						}
						Debug.Log ("Got video url: " + videoUrl);

					}
				}
			}
			
			return videoUrl.ToString ();
		}
	}

}

