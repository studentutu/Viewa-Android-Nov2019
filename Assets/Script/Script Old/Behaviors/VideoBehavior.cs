using UnityEngine;
using System.Collections;
using System;
using ACP;
using OTPL.INGAME;


/// <summary>
/// Plays a video on a plane, uses the native plugin to tell the host operating system where to draw its video
/// </summary>
public class VideoBehavior : WidgetBehavior
{
	public enum VideoBehaviorState
	{
		Initial,
		Pressed,
		Paused,
		Playing,
        End
	}
	
	public VideoBehaviorState state;
    public VideoUpdateStatus status;
	
	public Texture image;
	public Texture imagePressed;
	public Texture imageMask;
	public Texture imageReflection;
	public Texture imageLoading;
	public Texture endImage;
	
	public GameObject videoPlane;
	public GameObject imagePlane;
	
	public Texture2D videoTexture;
	public int videoTextureWidth = 256;
	public int videoTextureHeight = 256;
	
	private GameObject loadingText;
	private TextMesh loadingTextMesh;

	private bool hoverLastFrame = false;

	public bool Enabled = false;
	
	public float playTime = 0.0f;

	public VideoData data;
	
	public override WidgetData Data { 
		get { return data; }
	}
	
	public bool pausedWithApp = false;
	
    void OnApplicationPause(bool paused)
	{
		Debug.Log ("VideoBehaviour::OnApplicationPaused(" + paused + ")");
		if (paused && state == VideoBehaviorState.Playing)
		{
			Debug.Log ("VideoBehaviour - Pausing the video");
			pausedWithApp = true;
//			ACPUnityPlugin.Instnace.pauseVideo((int)videoTexture.GetNativeTexturePtr (), Time.time);
			ACPUnityPlugin.Instnace.pauseVideo(videoTexture, Time.time);
            UpdateVisiblePlanes(false);
			state = VideoBehaviorState.Paused;
		}
		if (!paused && pausedWithApp)
		{
			Debug.Log ("Video Behaviour - Unpausing the video");
			pausedWithApp = false;
//			ACPUnityPlugin.Instnace.playVideo((int)videoTexture.GetNativeTexturePtr(), Time.time);
			ACPUnityPlugin.Instnace.playVideo(videoTexture);
            UpdateVisiblePlanes(true);
			state= VideoBehaviorState.Playing;
		}
	}

	// Use this for initialization
	void Start ()
	{
        state = VideoBehavior.VideoBehaviorState.Initial;
        //DownloadCache.Instance.DownloadCompleteTexture += TextureLoaded;
		// Make the image plane touchable
		imagePlane.gameObject.layer = 8;
		
		if (data.alphaSBS) 
		{
            videoPlane.GetComponent<Renderer>().material = Camera.main.GetComponent<ACPTrackingManager>().materialAlphaBlended; //materialUIAlphaSBS
		}
		else if (data.imageMaskUrl != null)
		{
			videoPlane.GetComponent<Renderer>().material = Camera.main.GetComponent<ACPTrackingManager> ().materialUIMask;
		}
		else if (data.imageReflectionUrl != null)
		{
			videoPlane.GetComponent<Renderer>().material = Camera.main.GetComponent<ACPTrackingManager> ().materialUIShiny;
		}
		else
		{
			videoPlane.GetComponent<Renderer>().material = Camera.main.GetComponent<ACPTrackingManager> ().materialUITransparent;
		}
		
		imagePlane.GetComponent<Renderer>().material = Camera.main.GetComponent<ACPTrackingManager>().materialUITransparent;
		
		videoTexture = new Texture2D (videoTextureWidth, videoTextureHeight, TextureFormat.RGBA32, false);
		Color color = Color.black;
		for (int x = 0; x < videoTextureWidth; x++)
		{
			for (int y = 0; y < videoTextureHeight; y++)
			{
				videoTexture.SetPixel (x, y, color);
			}
		}
		videoTexture.Apply ();
	
		if (data != null) 
		{
			videoPlane.transform.localScale = new Vector3 (-data.size.x / 10.0f, 1, -data.size.z / 10.0f);
			imagePlane.transform.localScale = new Vector3 (-data.size.x / 10.0f, 1, -data.size.z / 10.0f);
			transform.localPosition = data.position;
			transform.localRotation = Quaternion.Euler (data.rotation);

			if (data.imageUrl != null) 
			{
				image = GetTexture (data.imageUrl);
				image.wrapMode = TextureWrapMode.Clamp;
				imagePlane.GetComponent<Renderer>().material.mainTexture = image;
			}
			if (data.imagePressedUrl != null)
			{
				imagePressed = GetTexture (data.imagePressedUrl);
				imagePressed.wrapMode = TextureWrapMode.Clamp;
			}
			
			if (data.imageLoadingUrl != null)
			{
				imageLoading = GetTexture (data.imageLoadingUrl);
				imageLoading.wrapMode = TextureWrapMode.Clamp;
			}
			
			if (data.imageMaskUrl != null)
			{
				imageMask = GetTexture (data.imageMaskUrl);
				imageMask.wrapMode = TextureWrapMode.Clamp;
				videoPlane.GetComponent<Renderer>().material.SetTexture ("_MaskTex", imageMask);
			}
			else if (data.imageReflectionUrl != null)
			{
				imageReflection = GetTexture (data.imageReflectionUrl);
				videoPlane.GetComponent<Renderer>().material.SetTexture ("_MaskTex", imageReflection);
			}
			
			if (data.endImage != null)
			{
				endImage = GetTexture (data.endImage);
				endImage.wrapMode = TextureWrapMode.Clamp;
			}
			
			
//			float timeUntilEnabled = EffectData.FindLongestEffect (data.appearEffects);
			//Debug.Log ("TODO: Enable in " + timeUntilEnabled + " seconds");
			Enabled = true;
			foreach (EffectData effect in data.appearEffects) 
			{
				effect.RunEffect (this.gameObject);
			}
		}
		
		this.name = "Video-" + videoTexture.GetNativeTexturePtr ();

		StartCoroutine(SetupVideo ());
	}
	
    private void OnEnable()
    {
        if (data != null)
        {
            if (data.autoPlay)
            {
                PlayVideo();
            }
        }
    }

	public IEnumerator SetupVideo ()
	{
		if (this.data.videoUrl.StartsWith ("yt://")) {
			string infoUrl = VideoData.getYoutubeVideoInfoUrl (this.data.videoUrl);
			WWW www = new WWW (infoUrl);
			yield return www;
			data.videoUrl = VideoData.getMp4UrlFromVideoInfo (www.text);
			Debug.Log ("getMp4UrlFromVideoInfo :" + data.videoUrl);
		}
		
		if (ACPTrackingManager.ARVideoEnabled) {
			ACPUnityPlugin.Instnace.setupVideo (data.videoUrl, videoPlane, videoTextureWidth, videoTextureHeight, data.loopVideo);
		}

        StartCoroutine(DelayPlayingVideo());
	}
	
    IEnumerator DelayPlayingVideo()
    {

        yield return new WaitForEndOfFrame();

        if (data.autoPlay)
        {
            PlayVideo();
        }
    }


    public void SetVideoUpdateStatus(VideoUpdateStatus videoUpdateStatus) {

        status = videoUpdateStatus;
    }
	
	public void SetVideoTexturePercentage (string value)
	{
		string[] split = value.Split (',');
		
		float scaleAdjust = 1.0f;
		if (data.alphaSBS)
			scaleAdjust = 0.5f;
		
		if (split.Length == 2)
		{
			float xPercentage = Mathf.Clamp (float.Parse (split[0]), 0f, 1f);
			float yPercentage = Mathf.Clamp (float.Parse (split[1]), 0f, 1f);
			
			videoPlane.GetComponent<Renderer>().material.mainTextureScale = new Vector2 (xPercentage, yPercentage);
			videoPlane.GetComponent<Renderer>().material.mainTextureOffset = new Vector2 (0f, 1f - yPercentage);
			
			videoPlane.GetComponent<Renderer>().material.SetFloat("_AlphaOffsetX", scaleAdjust * xPercentage);
			
			//imagePlane.renderer.material.mainTexture = new Vector2(xPercentage, yPercentage);
			//imagePlane.renderer.material.mainTextureOffset = new Vector2 (0f, 1f - yPercentage);
		}
		else 
		{
			videoPlane.GetComponent<Renderer>().material.mainTextureScale = new Vector2 (1.0f, 1.0f);
			//imagePlane.renderer.material.mainTextureScale = new Vector2 (1.0f, 1.0f);
		}
	}

    //Jeetesh - Not required in this Class
	//public void TextureLoaded (string url, Texture texture)
	//{
	//	//Debug.Log(url);
	//	if (url == data.imageUrl) 
	//	{
	//		image = texture;
	//		image.wrapMode = TextureWrapMode.Clamp;

	//		if (state == VideoBehavior.VideoBehaviorState.Initial)
	//		{
	//			//Debug.Log("VideoBehavior::SettingTexture");
	//			imagePlane.GetComponent<Renderer>().material.mainTexture = image;
	//		}
	//	}
		
	//	if (url == data.imagePressedUrl) 
	//	{
	//		imagePressed = texture;
	//		imagePressed.wrapMode = TextureWrapMode.Clamp;

	//		if (state == VideoBehavior.VideoBehaviorState.Pressed)
	//		{
	//			imagePlane.GetComponent<Renderer>().material.mainTexture = imagePressed;
	//		}
	//	}
		
	//	if (url == data.imageMaskUrl)
	//	{
	//		imageMask = texture;
	//		imageMask.wrapMode = TextureWrapMode.Clamp;

	//		videoPlane.GetComponent<Renderer>().material.SetTexture("_MaskTex", imageMask);
	//	}
	//}

	// Update is called once per frame
	void Update ()
	{
        if (ACPTrackingManager.ARVideoEnabled)
        {
            if (state == VideoBehavior.VideoBehaviorState.Initial || state == VideoBehavior.VideoBehaviorState.Pressed)
            {
                
                if (hoverLastFrame)
                {
                    UpdateVisiblePlanes(false);
                    hoverLastFrame = false;
                }
           
            }
            else if (state == VideoBehaviorState.Playing)
            {
                UpdateVideo();
            }
        }
	}
	
    private void UpdateVideo()
    {
        if (ACPTrackingManager.ARVideoEnabled)
        {
            switch (status)
            {
                case VideoUpdateStatus.Finished:
                    {
                        if (!string.IsNullOrEmpty(data.endImage))
                        {
                            //Debug.Log ("showing end image");
                            state = VideoBehavior.VideoBehaviorState.End;
                            imagePlane.GetComponent<Renderer>().material.mainTexture = endImage;
                        }
                        else if(!string.IsNullOrEmpty(data.endUrl)){
                            Application.OpenURL(data.endUrl);
                        }
                        else
                        {
                            state = VideoBehavior.VideoBehaviorState.Initial;
                        }
                        UpdateVisiblePlanes(false);
                        ACPUnityPlugin.Instnace.shutdownVideo();
                        state = VideoBehavior.VideoBehaviorState.Initial;
                        //ACPUnityPlugin.Instnace.setupVideo(data.videoUrl, videoPlane, videoTextureWidth, videoTextureHeight, data.loopVideo);
                        break;
                    }
                case VideoUpdateStatus.Loading:
                    
                    if (imageLoading != null)
                    {
                        imagePlane.GetComponent<Renderer>().material.mainTexture = imageLoading;
                        //LoadingTextRequire();
                    }

                    break;
                case VideoUpdateStatus.Playing:
                    
                    UpdateVisiblePlanes(true);
                    videoPlane.GetComponent<Renderer>().material.mainTexture = videoTexture;

                    break;
                default:
                    break;
            }
        }
    }


    protected void LoadingTextRequire() {
        
        //UpdateVisiblePlanes(false);
        if (imageLoading != null)
        {
            imagePlane.GetComponent<Renderer>().material.mainTexture = imageLoading;

            Debug.Log("Video is loading - Get this progress from OVP");
            float progress = 1.0f; ACPUnityPlugin.Instnace.videoDownloadProgress(data.videoUrl);

            if (loadingText == null)
            {
                ACPTrackingManager trackingManager = Camera.main.GetComponent<ACPTrackingManager>();

                loadingText = (GameObject)Instantiate(trackingManager.videoLoadingTextPrefab);
                loadingText.transform.parent = this.transform.parent;
                loadingText.transform.localPosition = data.position;
                //loadingText.transform.localRotation = Quaternion.Euler (data.rotation);
                loadingTextMesh = loadingText.GetComponent<TextMesh>();

                float scale = 1;
                if (this.data.size.x > this.data.size.y)
                {
                    scale = this.data.size.x / 20;
                }
                else
                {
                    scale = this.data.size.y / 20;
                }
                loadingText.transform.localScale = new Vector3(scale, scale, scale);

            }

            Debug.Log("Video progress: " + progress);
            loadingTextMesh.text = String.Format("Loading {0}%", Math.Round(progress * 100.0f));
        }
    }

    public void UpdateVisiblePlanes(bool isPlaying)
	{
		imagePlane.SetActive(!isPlaying);
		//videoPlane.SetActive(isPlaying);
	}
	
	
	/*public IEnumerator RemovePreloader()	
	{
		Spinner spinner = preloader.GetComponent<Spinner>();
		
		if (!spinner.isRemoving)
		{
			if (spinner != null)
			{
				while (!spinner.finishedAppearing)
				{
					yield return new WaitForSeconds(0.25f);
				}
				
				//float delay = spinner.Remove();
				Destroy(preloader);
				preloader = null;
			}
		}
		

		yield return true;
	}
*/
	public void OnDestroy ()
	{
		Debug.Log("Destroying video player");

        //DownloadCache.Instance.DownloadCompleteTexture -= TextureLoaded;

		if (videoTexture != null)
		{
			ACPUnityPlugin.Instnace.shutdownVideo ();  //(int)videoTexture.GetNativeTexturePtr ()
		}
		if (playTime > 0) {
			ACPUnityPlugin.Instnace.trackingEvent("ARVideoStop", this.data.parentFrameData.parentAreaData.trackableData.id, this.data.parentFrameData.parentAreaData.id, this.data.parentFrameData.id, this.data.id, this.data.videoUrl, null, null, null, null, playTime);
		}		
		
/*		if (preloader != null)
		{
			Debug.Log("Need to destroy the spinner...");
		}
		DestroyImmediate(preloader);
//		if (preloader != null)
//		{
//			
//			StartCoroutine(RemovePreloader());
//		}
*/
	}

	public void OnHover (bool released)
	{
		if (!Enabled)
		{
            Debug.Log("OnHover - !Enabled");
			return;
		}
		
		hoverLastFrame = true;

        if (state == VideoBehavior.VideoBehaviorState.Playing && released)
        {
            if (data.allowFullscreen)
            {
                Debug.Log("-------------------> OnHover Pausing video");
                //              ACPUnityPlugin.Instnace.pauseVideo ((int)videoTexture.GetNativeTexturePtr (), Time.time);
                //imagePlane.GetComponent<Renderer>().material.mainTexture = image;
                //imagePlane.SetActive(true);
                ACPUnityPlugin.Instnace.pauseVideo(videoTexture, Time.time);
                ACPUnityPlugin.Instnace.loadFullscreenVideo(this.data.parentFrameData.parentAreaData.trackableData.id, this.data.parentFrameData.parentAreaData.id, this.data.parentFrameData.id, this.data.id, data.videoUrl, 0.0f, data.endImage, data.endUrl, null, null, null);
                state = VideoBehaviorState.Paused;
            }
            else
            {
                Debug.Log("-------------------> OnHover Pausing video - not full screen");
                //              ACPUnityPlugin.Instnace.pauseVideo((int)videoTexture.GetNativeTexturePtr(), Time.time);
                //imagePlane.GetComponent<Renderer>().material.mainTexture = image;
                //imagePlane.SetActive(true);
                ACPUnityPlugin.Instnace.pauseVideo(videoTexture, Time.time);
                UpdateVisiblePlanes(false);
                state = VideoBehaviorState.Paused;
            }
        }
        else if (state == VideoBehavior.VideoBehaviorState.Paused && released)
        {
            Debug.Log("--------------------------> OnHover Playing video");
            //          ACPUnityPlugin.Instnace.playVideo((int)videoTexture.GetNativeTexturePtr(), Time.time);
            ACPUnityPlugin.Instnace.playVideo(videoTexture); //imagePlane
                                                         //imagePlane.SetActive(false);
            state = VideoBehaviorState.Playing;
            UpdateVisiblePlanes(true);
        }
        else if (state == VideoBehavior.VideoBehaviorState.Initial && released)  //&& released 
		{
            Debug.Log("-------------------> OnHover state == VideoBehavior.VideoBehaviorState.Initial (PlayVideo)");
			state = VideoBehavior.VideoBehaviorState.Pressed;
			imagePlane.GetComponent<Renderer>().material.mainTexture = imagePressed;
            PlayVideo();
		}
		else if (state == VideoBehavior.VideoBehaviorState.End) 
		{
            Debug.Log("-------------------> OnHover state == VideoBehavior.VideoBehaviorState.End - released;" + released);
            if (!string.IsNullOrEmpty(data.endUrl) && released)
			{
				ACPUnityPlugin.Instnace.loadWebUrl (
					this.data.parentFrameData.parentAreaData.trackableData.id, // trigger
					this.data.parentFrameData.parentAreaData.id, // area
					this.data.parentFrameData.id, // frame
					this.data.id, // widget
					this.data.endUrl, false, false, null, null, null);
            } else {
              
                state = VideoBehavior.VideoBehaviorState.Initial;
                imagePlane.GetComponent<Renderer>().material.mainTexture = image;
                PlayVideo();
            }
		}
		else if ((state == VideoBehavior.VideoBehaviorState.Pressed || state == VideoBehavior.VideoBehaviorState.Initial) 
		         && released) {
            Debug.Log ("OnHover Button click - PlayVideo");
			state = VideoBehavior.VideoBehaviorState.Initial;
			imagePlane.GetComponent<Renderer>().material.mainTexture = image;
			PlayVideo ();
		}
	}

	public override void Remove ()
	{
		Enabled = false;
		if (data.disappearEffects.Count > 0) {
			foreach (EffectData effect in data.disappearEffects) {
				effect.RunEffect (this.gameObject);
			}
			
			TimedObjectDestructor dest = gameObject.AddComponent<TimedObjectDestructor> ();
			dest.timeOut = EffectData.FindLongestEffect (data.disappearEffects);
		} else {
			Destroy (gameObject);
		}
	}



	public void PlayVideo ()
	{
		if (ACPTrackingManager.ARVideoEnabled)
		{
			if (state != VideoBehavior.VideoBehaviorState.Playing) 
			{
				Debug.Log ("Play video!!!");
                //			if (preloader == null)
                //			{
                //				Debug.Log("Create preloader");
                //				ACPTrackingManager trackingManager = Camera.main.GetComponent<ACPTrackingManager>();
                //				preloader = (GameObject) GameObject.Instantiate(trackingManager.preloadPrefab);
                //				preloader.transform.parent = this.transform.parent;
                //				preloader.transform.localPosition = data.position;
                //				preloader.transform.localRotation = Quaternion.Euler (data.rotation);
                //				//preloader.transform.Translate(Vector3.up * 20.0f);
                //				float scale = 1;
                //				if (this.data.size.x > this.data.size.y)
                //				{
                //					scale = this.data.size.x / 20;
                //				}
                //				else
                //				{
                //					scale = this.data.size.y / 20;
                //				}
                //
                //				preloader.transform.localScale = new Vector3(scale, scale, scale);
                //			}

                videoPlane.gameObject.SetActive(true);
                
                //if (videoPlane.GetComponent<Renderer>().material.mainTexture != videoTexture)
                //{
                //    videoPlane.GetComponent<Renderer>().material.mainTexture = videoTexture;
                //}

				//Jeetesh - commented this 
				state = VideoBehavior.VideoBehaviorState.Playing;
                status = VideoUpdateStatus.Loading;
//				ACPUnityPlugin.Instnace.playVideo ((int)videoTexture.GetNativeTexturePtr (), Time.time);

				Debug.Log ("ACPUnityPlugin.Instnace.playVideo");
				ACPUnityPlugin.Instnace.playVideo (videoTexture);

				ACPUnityPlugin.Instnace.trackingEvent("ARVideoStart", this.data.parentFrameData.parentAreaData.trackableData.id, this.data.parentFrameData.parentAreaData.id, this.data.parentFrameData.id, this.data.id, this.data.videoUrl, null, null, null, null, -1.0f);
	
			}
		}
		else
		{
			ACPUnityPlugin.Instnace.loadFullscreenVideo (this.data.parentFrameData.parentAreaData.trackableData.id, this.data.parentFrameData.parentAreaData.id, this.data.parentFrameData.id, this.data.id, data.videoUrl, 0.0f, data.endImage, data.endUrl, null, null, null);
		}

	}	
}