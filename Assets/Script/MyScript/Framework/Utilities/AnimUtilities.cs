using UnityEngine;
using System.Collections;

namespace OTPL
{
	public enum FadeType
	{
		FadeIn,
		FadeOut }
	;

	public class AnimUtilities
	{
		public static float CrossFadeUp (float weight, float fadeTime)
		{
			return Mathf.Clamp01 (weight + Time.deltaTime / fadeTime);
		}
		
		public static float CrossFadeDown (float weight, float fadeTime)
		{
			return Mathf.Clamp01 (weight - Time.deltaTime / fadeTime);
		}
		
		public static float CrossFadeUpClamp (float weight, float fadeTime, float clamp)
		{
			return Mathf.Clamp (weight + Time.deltaTime / fadeTime, 0f, clamp);
		}
		
		public static float CrossFadeDownClamp (float weight, float fadeTime, float clamp)
		{
			return Mathf.Clamp (weight - Time.deltaTime / fadeTime, clamp, 1f);
		}
		
		public static float CrossFadeUpClampBoth (float weight, float fadeTime, float lowerClamp, float upperClamp)
		{
			return Mathf.Clamp (weight + Time.deltaTime / fadeTime, lowerClamp, upperClamp);
		}
		
		public static float CrossFadeDownClampBoth (float weight, float fadeTime, float lowerClamp, float upperClamp)
		{
			return Mathf.Clamp (weight - Time.deltaTime / fadeTime, lowerClamp, upperClamp);
		}
		
		public static float CrossFadeUpClampBothPercent (float weight, float fadeTime, float lowerClamp, float upperClamp)
		{
			return Mathf.Clamp (weight + ((Time.deltaTime / fadeTime) * weight), lowerClamp, upperClamp);
		}
		
		public static float CrossFadeDownClampBothPercent (float weight, float fadeTime, float lowerClamp, float upperClamp)
		{
			return Mathf.Clamp (weight - ((Time.deltaTime / fadeTime) * weight), lowerClamp, upperClamp);
		}
		
		/// <summary>
		/// Copy the Transforms from the Source to the Destination
		/// </summary>
		/// <param name="src">
		/// A <see cref="Transform"/>
		/// </param>
		/// <param name="dst">
		/// A <see cref="Transform"/>
		/// </param>
		/// <param name="velocity">
		/// A <see cref="Vector3"/>
		/// </param>
		public static void CopyTransformsRecurse (Transform src, Transform dst, Vector3 velocity)
		{
			
			Rigidbody body = dst.GetComponent<Rigidbody> ();
			if (body != null) {
				//body.velocity = velocity;
				body.useGravity = true;
			}
			
			dst.position = src.position;
			dst.rotation = src.rotation;
			
			foreach (Transform child in dst) {
				// Match the transform with the same name
				Transform curSrc = src.Find (child.name);
				if (curSrc)
					CopyTransformsRecurse (curSrc, child, velocity);
			}
		}
	}

}
