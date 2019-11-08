using System;
using System.Collections.Generic;
using UnityEngine;

namespace ACP
{
	public class ModelObjData : WidgetData
	{
		public string imageUrl;
		public string imageReflectionUrl;
		public string modelUrl;
		public string materialUrl;

		public ModelObjData ()
		{
		}

		public static ModelObjData Create (FrameData frameData, int id, JSONObject modelJson)
		{
			ModelObjData modelData = new ModelObjData ();
			modelData.imageUrl = modelJson["texture_url"].str;
			
			if (modelJson["image_reflection"] != null) 
				modelData.imageReflectionUrl = modelJson["image_reflection"].str;
			
			modelData.modelUrl = modelJson["model_url"].str;
			modelData.materialUrl = modelJson["material_url"].str;
			
			WidgetData.Create(frameData, modelData, id, modelJson);
			
			return modelData;
		}
		
		public override WidgetBehavior CreateBehavior (AreaBehavior parent, int index)
		{
			//			GameObject image = GameObject.CreatePrimitive(PrimitiveType.Plane);
			GameObject model = new GameObject ();
			model.transform.parent = parent.transform;
			
			ModelObjBehavior bh = model.AddComponent<ModelObjBehavior> ();
			bh.data = this;
			bh.index = index;
			return bh;

		}
		
		override public string[] TexturesRequired() 
		{
			List<string> images = new List<string>();
			if (imageUrl != null && imageUrl.Length>0) images.Add(imageUrl);
			if (imageReflectionUrl != null && imageReflectionUrl.Length>0) images.Add(imageReflectionUrl);
				
			return images.ToArray();		
		}
		
		override public string[] FilesRequired()
		{
			List<string> files = new List<string>();
			if (modelUrl != null && modelUrl.Length>0) files.Add(modelUrl);
			if (materialUrl != null && materialUrl.Length>0) files.Add(materialUrl);
			
			return files.ToArray();
		}
	}
}

