using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;
using OTPL.INGAME;

public class OptimalVideoPlayer : MonoBehaviour {

	//Raw Image to Show Video Images [Assign from the Editor]
	private VideoPlayer videoPlayer;
	private VideoSource videoSource;
	//Audio
	private AudioSource audioSource;
	private bool isVideoPlaying;
	private bool isVideoPrepared;
    private VideoBehavior videoBehavior;
    private bool loopVideo;

	void Awake()
	{
		Debug.Log("OptimalVideoPlayer - Awake");
		Application.runInBackground = true;
        videoBehavior = transform.GetComponent<VideoBehavior>();
	}

	void OnEnable() {
		
//		videoPlayer.prepareCompleted += PrepareCompleted;
		//ACPUnityPlugin.Instnace.videoUpdateStatus = ACPUnityPlugin.VideoUpdateStatus.Loading;
	}
	void OnDisable() {
		videoPlayer.loopPointReached -= EndReached;
//		videoPlayer.prepareCompleted -= PrepareCompleted;
	}
	public void SetUpVideo(string videoUrl, GameObject videoPlane, int width, int height, bool loopVideo) {

        this.loopVideo = loopVideo;
		//Add VideoPlayer to the GameObject
		if(videoPlayer == null) {
            videoPlayer = videoPlane.AddComponent<VideoPlayer>();  //gameObject.AddComponent<VideoPlayer>();
			videoPlayer.renderMode = VideoRenderMode.MaterialOverride;
            videoPlayer.loopPointReached += EndReached;

			//Add AudioSource
            audioSource = videoPlane.AddComponent<AudioSource>();
		}
			
		//Disable Play on Awake for both Video and Audio
		videoPlayer.playOnAwake = false;
		audioSource.playOnAwake = false;

				////We want to play from video clip not from url
				//videoPlayer.source = VideoSource.VideoClip;

		//We want to play from url
		videoPlayer.source = VideoSource.Url;
		videoPlayer.url = videoUrl;

//		videoPlayer.url = "http://www.quirksmode.org/html5/videos/big_buck_bunny.mp4";

		//Set Audio Output to AudioSource
		videoPlayer.audioOutputMode = VideoAudioOutputMode.AudioSource;

		//Assign the Audio from Video to AudioSource to be played
		videoPlayer.EnableAudioTrack(0, true);
		videoPlayer.SetTargetAudioSource(0, audioSource);
	}

	public void PlayPause()
	{
		if (videoPlayer.isPlaying) 
		{
			videoPlayer.Pause ();

		} else 
		{
			videoPlayer.Play ();
            audioSource.Play();
		}
	}

	public IEnumerator PreparePlayVideo(Texture2D videoTexture){

        //Set video To Play then prepare Audio to prevent Buffering
        //		videoPlayer.clip = videoToPlay;  //Not needed when play from url 

		videoPlayer.Prepare();

		isVideoPrepared = videoPlayer.isPrepared;

	
		//Wait until video is prepared
		while (!isVideoPrepared)
		{
			if (videoPlayer != null) {
				isVideoPrepared = videoPlayer.isPrepared;
				Debug.Log ("Preparing Video");
				yield return null;
			}
		}

		Debug.Log("Done Preparing Video");

		//Assign the Texture from Video to RawImage to be displayed

        videoPlayer.gameObject.GetComponent<Renderer> ().material.mainTexture = videoTexture; //videoTexture; //videoPlayer.texture

		//Play Video
		videoPlayer.Play();
        videoBehavior.SetVideoUpdateStatus (VideoUpdateStatus.Playing);
   
		//Play Sound
		audioSource.Play();

		isVideoPlaying = videoPlayer.isPlaying;

		Debug.Log("Playing Video");
		while (isVideoPlaying)
		{
			if (videoPlayer != null) {
				isVideoPlaying = videoPlayer.isPlaying;
                //ACPUnityPlugin.Instnace.videoUpdateStatus = ACPUnityPlugin.VideoUpdateStatus.Playing;
				Debug.LogWarning("Video Time: " + Mathf.FloorToInt((float)videoPlayer.time));
			}
			yield return null;
		}

		Debug.Log("Done Playing Video");
	}

	public void PlayVideo(Texture2D videoTexture){

        ACPUnityPlugin.Instnace.trackEvent("Video", "PlayVideo", "Play", 0);
		if (isVideoPrepared) {
			videoPlayer.Play ();
		} else {
			StartCoroutine (PreparePlayVideo (videoTexture));
		}
	}

	public void ShutDownVideo() {

        ACPUnityPlugin.Instnace.trackEvent("Video", "VideoStopped", "Stopped", 0);
        videoPlayer.Stop ();
	}

	public void pauseVideo() {

        ACPUnityPlugin.Instnace.trackEvent("Video", "PauseVideo", "Pause", 0);
        videoPlayer.Pause ();
	}
	void EndReached(VideoPlayer a_videoPlayer) {

        if(this.loopVideo){
            videoPlayer.Play();
        } else {

            isVideoPlaying = false;
            isVideoPrepared = false;
            videoBehavior.SetVideoUpdateStatus(VideoUpdateStatus.Finished);
            //ACPUnityPlugin.Instnace.videoUpdateStatus = ACPUnityPlugin.VideoUpdateStatus.Finished;
        }

	}

}
