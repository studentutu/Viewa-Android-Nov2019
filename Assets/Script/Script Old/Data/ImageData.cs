using System;
using System.Collections.Generic;
using UnityEngine;

namespace ACP
{
	public class ImageData : WidgetData
	{
		public string imageUrl;
		public string imageReflectionUrl;

		public ImageData ()
		{
		}

		public static ImageData Create (FrameData frameData, int id, JSONObject imageJson)
		{
			ImageData imageData = new ImageData ();
			
			if (imageJson["image"] != null && imageJson["image"].str != "")
				imageData.imageUrl = imageJson["image"].str;
			
			if (imageJson["image_reflection"] != null && (imageJson["image_reflection"].str != ""))
				imageData.imageReflectionUrl = imageJson["image_reflection"].str;
			
			if (imageData.imageUrl == null || imageData.imageUrl == "")
				return null;
			
			WidgetData.Create(frameData, imageData, id, imageJson);
			
		return imageData;
		}
		
		public override WidgetBehavior CreateBehavior (AreaBehavior parent, int index)
		{
			GameObject image = new GameObject ();
			image.transform.parent = parent.transform;

			ImageBehavior bh = image.AddComponent<ImageBehavior> ();
			bh.data = this;
			bh.index = index;
			bh.plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
			bh.plane.transform.parent = image.transform;
			return bh;

		}
		
		override public string[] TexturesRequired() 
		{
			List<string> images = new List<string>();
			if (imageUrl != null && imageUrl.Length>0) images.Add(imageUrl);
			if (imageReflectionUrl != null && imageReflectionUrl.Length>0) images.Add(imageReflectionUrl);
				
			return images.ToArray();		
		}
	}
}

