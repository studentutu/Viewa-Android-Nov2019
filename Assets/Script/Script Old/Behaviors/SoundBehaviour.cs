using UnityEngine;
using System.Collections;
using ACP;

/// <summary>
/// Plays a sound effect
/// </summary>
public class SoundBehaviour : WidgetBehavior
{
	
	public AudioClip audioClip;
	public SoundData data;
	public AudioSource audioSource;
	
	public override WidgetData Data { 
		get { return data; }
	}

	// Use this for initialization
	void Start ()
	{
		Debug.Log ("Inside SoundBehaviour - Start()");
		if (data != null) {
			transform.localPosition = data.position;
			transform.localRotation = Quaternion.Euler (data.rotation);

			audioClip = GetAudioClip (data.soundUrl);

            if(audioSource == null){
                audioSource = this.gameObject.AddComponent<AudioSource>();
            }

			if (audioSource.clip != null) {
				Debug.Log ("SoundBehaviour - audioSource.clip :" + audioSource.clip);

				this.name = "Sound";
			
				audioSource.clip = audioClip;

				if (data.autoPlay) {
					audioSource.Play ();
				}
			
				foreach (EffectData effect in data.appearEffects) 
				{
					effect.RunEffect (this.gameObject);
				}
			}
		}
	}
	
	public void OnDestroy ()
	{
	}
	
	// Update is called once per frame
	void Update ()
	{
	}
	
	void FixedUpdate ()
	{
	}

	public override void Remove ()
	{
		if (data.disappearEffects.Count > 0) {
			
			if (audioSource.clip != null)
			{
				foreach (EffectData effect in data.disappearEffects) {
					effect.RunEffect (this.gameObject);
				}
			}
			
			TimedObjectDestructor dest = gameObject.AddComponent<TimedObjectDestructor> ();
			dest.timeOut = EffectData.FindLongestEffect (data.disappearEffects);
		//	Debug.Log ("Destroying in " + dest.timeOut);
		} else {
			Destroy (gameObject);
		}
	}
	
}

