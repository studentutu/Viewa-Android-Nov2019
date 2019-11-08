using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace ACP
{
	public class EffectData
	{
		public delegate void iTweenMethodDelegate (GameObject o, Hashtable args);

		public enum EffectType
		{
			iTween
		}
		public EffectType type;

		public iTweenMethodDelegate iTweenMethod;

		public Hashtable parameters = new Hashtable ();

		public EffectData ()
		{
		}

		public static EffectData Create (JSONObject o)
		{
			EffectData data = new EffectData ();
			
			JSONObject t = o ["type"];
			if (t == null)
				return null;
			
			if (t.str.StartsWith ("t_")) {
				data.type = EffectType.iTween;
				switch (t.str) {
				case "t_FadeFrom":
					data.iTweenMethod = iTween.FadeFrom;
					break;
				case "t_FadeTo":
					data.iTweenMethod = iTween.FadeTo;
					break;
				case "t_MoveFrom":
					data.iTweenMethod = iTween.MoveFrom;
					break;
				case "t_MoveTo":
					data.iTweenMethod = iTween.MoveTo;
					break;
				case "t_ScaleFrom":
					data.iTweenMethod = iTween.ScaleFrom;
					break;
				case "t_ScaleTo":
					data.iTweenMethod = iTween.ScaleTo;
					break;
				case "t_RotateFrom":
					data.iTweenMethod = iTween.RotateFrom;
					break;
				case "t_RotateTo":
					data.iTweenMethod = iTween.RotateTo;
					break;
				case "t_RotateBy":
					data.iTweenMethod = iTween.RotateBy;
					break;
				case "t_AudioTo":
					data.iTweenMethod = iTween.AudioTo;
					break;
				case "t_AudioFrom":
					data.iTweenMethod = iTween.AudioFrom;
					break;
				case "t_AudioUpdate":
					data.iTweenMethod = iTween.AudioUpdate;
					break;
				case "t_Stab":
					data.iTweenMethod = iTween.Stab;
					break;
				default:
					Debug.Log ("UNHANDLED tween Method: " + t.str);
					break;
				}
				
				foreach (string key in o["params"].keys) {
					JSONObject param = o ["params"] [key];
					switch (param.type) {
					case JSONObject.Type.STRING:
						data.parameters.Add (key, param.str);
//					Debug.Log("Adding parameter " + key + ": " + param.str);
						break;
					case JSONObject.Type.NUMBER:
						data.parameters.Add (key, (float)param.n);
//					Debug.Log("Adding parameter " + key + ": " + param.n);
						break;
					case JSONObject.Type.ARRAY:
						if (param.Count == 3) {
							data.parameters.Add (key, new Vector3 ((float)param [0].n, (float)param [1].n, (float)param [2].n));
						} else if (param.Count == 2) {
							data.parameters.Add (key, new Vector2 ((float)param [0].n, (float)param [1].n));
						}
						break;
					case JSONObject.Type.BOOL:
						data.parameters.Add (key, param.b);
						break;
					default:
						break;
					}
				}
			}
			
			return data;
		}

		public void RunEffect (GameObject o)
		{
			switch (type) {
			case EffectType.iTween:
				{
					if (iTweenMethod == iTween.Stab || 
					iTweenMethod == iTween.AudioFrom || 
					iTweenMethod == iTween.AudioTo || 
					iTweenMethod == iTween.AudioUpdate) {
						//Debug.Log ("Setting audio source...");
						parameters ["audiosource"] = o.GetComponent<AudioSource>();
						parameters ["audioclip"] = o.GetComponent<AudioSource>().clip;
					}
					iTweenMethod (o, parameters);
					break;
				}
			default:
				Debug.Log ("Unhandled effect type in RunEffect()");
				break;
			}
		}

		public static float FindLongestEffect (List<EffectData> effects)
		{
			float time = 0;
			foreach (EffectData effect in effects) {
				float thisTime = 0;
				float thisDelay = 0;
				if (effect.parameters.ContainsKey ("time")) {
					thisTime = (float)effect.parameters["time"];
				}
				if (effect.parameters.ContainsKey ("delay")) {
					thisDelay = (float)effect.parameters["delay"];
				}
				if (thisTime + thisDelay > time)
					time = thisTime + thisDelay;
			}
			
			return time;
		}
	}
}

