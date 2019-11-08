using System;
using System.Collections.Generic;
using UnityEngine;

namespace ACP
{
	public class SoundData : WidgetData
	{
		public string soundUrl;

		public SoundData ()
		{
		}

		public static SoundData Create (FrameData frameData, int id, JSONObject soundJson)
		{
			SoundData soundData = new SoundData ();
			
			if (soundJson ["sound_url"] != null && soundJson ["sound_url"].str != "")
				soundData.soundUrl = soundJson ["sound_url"].str;
			
			
			WidgetData.Create (frameData, soundData, id, soundJson);
			
			return soundData;
		}
		
		public override WidgetBehavior CreateBehavior (AreaBehavior parent, int index)
		{
			GameObject go = new GameObject ();
			go.transform.parent = parent.transform;

			SoundBehaviour bh = go.AddComponent<SoundBehaviour> ();
			bh.data = this;
			bh.index = index;
			return bh;

		
		}
		
		override public string[] AudioClipsRequired ()
		{
			List<string> sounds = new List<string> ();
			if (soundUrl != null && soundUrl.Length > 0)
				sounds.Add (soundUrl);
			
			return sounds.ToArray ();		
		}
		

	}
}

