using System;
using System.Collections.Generic;

using UnityEngine;

namespace ACP
{
	public abstract class WidgetData
	{
		public FrameData parentFrameData;
		
		public int id;
		public string name;
		public Vector3 size;
		public Vector3 position;
		public Vector3 rotation;
		public bool ignoreWidgetInSnapToLayout = false;

		public List<EffectData> appearEffects = new List<EffectData> ();
		public List<EffectData> disappearEffects = new List<EffectData> ();
		
		public bool autoPlay;
		
		public enum SnapToMode
		{
			Both,
			AR,
			Snap
		}
		public SnapToMode snapToMode;
		
		public WidgetData ()
		{
		}
		
		public static void Create (FrameData frameData, WidgetData widgetData, int id, JSONObject jsonObject)
		{
			widgetData.parentFrameData = frameData;
			widgetData.id = id;
			JSONObject s = jsonObject["widget_name"];
			if(s != null) {
				widgetData.name = s.str;
			}
			s = jsonObject["size"];
			if (s.Count == 2)
			{
				widgetData.size = new Vector3 ((float)s[0].n, 1.0f, (float)s[1].n);
			}
			else
			{
				widgetData.size = new Vector3 ((float)s[0].n, (float)s[1].n, (float)s[2].n);
			}
			
			s = jsonObject["position"];
			widgetData.position = new Vector3 ((float)s[0].n, (float)s[1].n, (float)s[2].n);
			s = jsonObject["rotation"];
			widgetData.rotation = new Vector3 ((float)s[0].n, (float)s[1].n, (float)s[2].n);
			
			s = jsonObject["snap_to"];
			if (s != null)
			{
				widgetData.snapToMode = (SnapToMode) (int) s.n;
			} 
			else
			{
				widgetData.snapToMode = SnapToMode.Both;
			}
			
			s = jsonObject["auto_play"];
			if (s != null)
			{
				widgetData.autoPlay = s.b;
			}
			else
			{
				widgetData.autoPlay = false;
			}
			JSONObject appearEffects = jsonObject["appear_effects"];
			if (appearEffects != null) 
			{
				//Debug.Log ("Found " + appearEffects.Count + " Appear Effects");
				for (int aei = 0; aei < appearEffects.Count; aei++) {
					EffectData effect = EffectData.Create (appearEffects[aei]);
					if (effect != null)
						widgetData.appearEffects.Add (effect);
				}
			}
			
			JSONObject disappearEffects = jsonObject["disappear_effects"];
			if (disappearEffects != null) 
			{
				//Debug.Log ("Found " + disappearEffects.Count + " Disappear Effects");
				for (int dei = 0; dei < disappearEffects.Count; dei++) {
					EffectData effect = EffectData.Create (disappearEffects[dei]);
					if (effect != null)
						widgetData.disappearEffects.Add (effect);
				}
			}

		}
		
		public abstract WidgetBehavior CreateBehavior(AreaBehavior parent, int index);
		
		virtual public string[] TexturesRequired() 
		{
			return new string[] {};
		}
		
		virtual public string[] FilesRequired ()
		{
			return new string[] {};
		}
		
		virtual public string[] AudioClipsRequired ()
		{
			return new string[] {};
		}
		
	}
}

