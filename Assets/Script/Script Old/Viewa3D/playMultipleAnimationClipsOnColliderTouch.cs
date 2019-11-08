using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Viewa3D
{

	public class playMultipleAnimationClipsOnColliderTouch : MonoBehaviour {

		public enum PlayType{PlayOne, PlayAll};
		public enum PlayOrder{RandomOrder, SequencedOrder};

		public Animation AnimationComponent;
		public AnimationClip[] animationClips;
		public Collider hitTestCollider;
		public float hitTestDepth = 2000; //how far to cast down for the hit test for rotation. Put 0 to not do a hit test at all
		public PlayType playType;
		public PlayOrder playOrder;

		private List<AnimationClip> clipQueue = new List<AnimationClip>();
		private int playIndex = -1;

		void Update() {

			Vector3? touchedPosition = null;

			//touch handling on mobile device
			if (Input.touchCount > 0) 
			{       
				if(Input.touchCount == 1)
				{ // single finger 
					//touchedPosition = blah
				}
			}

			if (Input.GetMouseButtonUp(0)){
				touchedPosition = Input.mousePosition;
			}

			if(touchedPosition != null){
				if(didHitCollider(touchedPosition.Value)){

					int animationCount = 1;
					if(playType == PlayType.PlayAll){
						animationCount = animationClips.Length;
					}

					if(playOrder == PlayOrder.RandomOrder){
						shuffleClips();
						playIndex = 0;
					} else if((playOrder == PlayOrder.SequencedOrder) && (playType == PlayType.PlayOne)){
						//sequenced index, playing one by one
						playIndex++;
						if(playIndex >= animationClips.Length){
							playIndex = 0;
						}
					} else {
						playIndex = 0;
					}

					if(animationClips != null){
						clipQueue = new List<AnimationClip>();
						for(int i = playIndex; i < playIndex + animationCount; i++){
							AnimationClip animationClip = animationClips.GetValue(i) as AnimationClip;						
							if(animationClip != null){
								//Debug.Log("adding clip " + playIndex + " to queue " + animationClip);
								clipQueue.Add(animationClip);
							} else {
								//Debug.Log("animationClip is null - can't add to queue!");
							}
						}
					} else {
						Debug.LogError("animationClips array is null!");
					}

					playNextClip();
				}
			}		

			if((clipQueue.Count > 0) && (AnimationComponent.isPlaying == false)){
				playNextClip();
			}
		}

		void shuffleClips()
		{
			// Knuth shuffle algorithm :: courtesy of Wikipedia :)
			for (int t = 0; t < animationClips.Length; t++ )
			{
				AnimationClip tmp = animationClips[t];
				int r = Random.Range(t, animationClips.Length);
				animationClips[t] = animationClips[r];
				animationClips[r] = tmp;
			}
		}

		private void playNextClip()
		{
			//Debug.Log ("Playing next clip");
			AnimationClip clip = clipQueue[0];
			AnimationComponent.RemoveClip(clip);
			AnimationComponent.AddClip(clip, clip.name);
			AnimationComponent.Play(clip.name);
			clipQueue.RemoveAt(0);
		}

		private bool didHitCollider(Vector3 pos)
		{
			if((hitTestDepth == 0) || (hitTestCollider == null)){
				return true;
			} else {
				Ray ray = Camera.main.ScreenPointToRay(pos);
				RaycastHit hit;
				if(Physics.Raycast(ray, out hit, hitTestDepth)){
				//if (hitTestCollider.Raycast (ray, out hit, hitTestDepth)) {
					if (hit.collider == hitTestCollider) {
						return true;
					}
				}
				return false;
			}
		}

	}
}
