using System;
using System.Collections.Generic;
using UnityEngine;

namespace ACP
{
	public class ARGalleryData : WidgetData
	{
		public string galleryUrl; //url to the gallery json

		public ARGalleryData ()
		{
		}

		public static ARGalleryData Create (FrameData frameData, int id, JSONObject argJson)
		{
			ARGalleryData argData = new ARGalleryData ();

			argData.galleryUrl = argJson["gallery_url"].str;

			WidgetData.Create(frameData, argData, id, argJson);

			//Debug.Log ("Creating ARGalleryData! ");

			return argData;
		}

		public override WidgetBehavior CreateBehavior (AreaBehavior parent, int index)
		{
			GameObject arg = new GameObject ();
			arg.transform.parent = parent.transform;

			ARGalleryBehavior argb = arg.AddComponent<ARGalleryBehavior> ();
			argb.data = this;
			argb.index = index;
			argb.imagePlane = GameObject.CreatePrimitive(PrimitiveType.Plane);
			argb.imagePlane.transform.parent = arg.transform;
			argb.thumbBGPlane = GameObject.CreatePrimitive(PrimitiveType.Plane);
			argb.thumbBGPlane.transform.parent = arg.transform;

			argb.scrollView = new GameObject( "ARGallery ScrollView" );
			argb.scrollView.transform.parent = parent.transform;

			return argb;
		}

	}
}

