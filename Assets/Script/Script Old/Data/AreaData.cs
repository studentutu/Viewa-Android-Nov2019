using System;
using System.Collections.Generic;
using UnityEngine;

namespace ACP
{
	public class AreaData
	{
		public int id;
		public Vector3 position;
		public Vector3 rotation;
		public Vector3 size;
		
		public List<FrameData> frames;
		
		public TrackableData trackableData;
		
//		public ButtonData activateButton;

		public AreaData ()
		{
			frames = new List<FrameData>();
		}

		public static AreaData Create (JSONObject areaJson, TrackableData parent)
		{
			AreaData area = new AreaData ();
			
			area.trackableData = parent;
			
			JSONObject identifier = areaJson["id"];
			if (identifier == null) return null;
			
			area.id = (int) identifier.n;
			
			JSONObject tmp;
			tmp = areaJson["position"];
			area.position = new Vector3 ((float)tmp[0].n, (float)tmp[1].n, (float)tmp[2].n);
			tmp = areaJson["rotation"];
			area.rotation = new Vector3 ((float)tmp[0].n, (float)tmp[1].n, (float)tmp[2].n);
			
			tmp = areaJson["size"];
			area.size = new Vector3 ((float)tmp[0].n, (float)tmp[1].n, (float)tmp[2].n);
			
			//Debug.Log ("Creating area with id: " + area.id);
			
			// Load the pages...
			JSONObject framesJson = areaJson["frames"];
			if (framesJson != null)
			{
				//Debug.Log("Found " + framesJson.Count + " frames");
				for (int pi = 0; pi < framesJson.Count; pi++)
				{
					FrameData FrameData = FrameData.Create(area, pi, framesJson[pi]);
					area.frames.Add(FrameData);
				}
			}
			
			return area;
			
		}
	}
}

