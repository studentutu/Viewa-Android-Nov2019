using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Text;
using ACP;
using OTPL.UI;

/// <summary>
/// An area basically groups a set of tracking responses together, so that one trigger from Qualcomm can contain independent responses
/// </summary>

public class AreaBehavior : MonoBehaviour
{
	public AreaData data; // our data
	
	//private ACPTrackingManager manager;
	
	//
	// Keep track of the total set of files we need, as well was the files on a per-frame basis
	//
	Dictionary<string, int> TexturesRequired = new Dictionary<string, int>(); // This keeps track of how many textures we need to load before we can start
	Dictionary<int, Dictionary<string, int>> TexturesRequiredForFrame = new Dictionary<int, Dictionary<string, int>>();
	Dictionary<int, int> TexturesTotalForFrame = new Dictionary<int, int>();
	public Dictionary<string, Texture2D> Textures = new Dictionary<string, Texture2D>();
	
	Dictionary<string, int> FilesRequired = new Dictionary<string, int>();
	Dictionary<int, Dictionary<string, int>> FilesRequiredForFrame = new Dictionary<int, Dictionary<string, int>>();
	Dictionary<int, int> FilesTotalForFrame = new Dictionary<int, int>();
	public Dictionary<string, byte[]> Files = new Dictionary<string, byte[]>();
	
	Dictionary<string, int> AudioClipsRequired = new Dictionary<string, int>();
	Dictionary<int, Dictionary<string, int>> AudioClipsRequiredForFrame = new Dictionary<int, Dictionary<string, int>>();
	Dictionary<int, int> AudioClipsTotalForFrame = new Dictionary<int, int>();
	public Dictionary<string, AudioClip> AudioClips = new Dictionary<string, AudioClip>();
	
	Dictionary<ModelObjData, int> ModelsRequired = new Dictionary<ModelObjData, int>();
	Dictionary<int, Dictionary<ModelObjData, int>> ModelsRequiredForFrame = new Dictionary<int, Dictionary<ModelObjData, int>>();
	Dictionary<int, int> ModelsTotalForFrame = new Dictionary<int, int>();
	public Dictionary<ModelObjData, GameObject> Models = new Dictionary<ModelObjData, GameObject>();
	
	public const int DownloadRetryCountTotal = 5; // How many times do we want to retry failed textures...
	
	public delegate void AreaLoadedEventHandler(AreaBehavior area); // ACPTrackableBehavior subscribes to this event so it knows when we have finished loading
	public event AreaLoadedEventHandler AreaLoaded;
	
	public delegate void AreaLoadProgressEventhandler(AreaBehavior area, float progress);
	public event AreaLoadProgressEventhandler AreaLoadProgress;
	
	public delegate void ActivateAreaFrameEventhandler(AreaBehavior area, int frame);
	public event ActivateAreaFrameEventhandler ActivateAreaFrame;

	public bool PreloadFinished = false; // Have we finished loading yet?
	private bool currentPageActivated = false;

	public string areaPageUrl;
	
	public float ItemsLoaded;
	public float TotalItemsToLoad;
	public float preloadPercent;

	public Dictionary<int, bool> framesLoaded = new Dictionary<int, bool>();
	
	public List<List<WidgetBehavior>> widgetBehaviours;

	private bool sticky = false;
	public bool Sticky
	{
		get { return sticky; }
		set { 
			sticky = value; 
			//Debug.Log ("Setting sticky to " + value);
			
			if (this.currentFrameIndex < 0)
			{
				return;
			}
			
			for (int i = 0; i < this.widgetBehaviours[this.currentFrameIndex].Count; i++)
			{
				GameObject go = this.widgetBehaviours[this.currentFrameIndex][i].gameObject;
				WidgetBehavior widget = go.GetComponent<WidgetBehavior>();
				
				if (widget != null)
				{
					switch (widget.Data.snapToMode)
					{
					case WidgetData.SnapToMode.AR:
						widget.gameObject.SetActive(!value);
						break;
					case WidgetData.SnapToMode.Both:
						widget.gameObject.SetActive(true);
						break;
					case WidgetData.SnapToMode.Snap:
						widget.gameObject.SetActive(value);
						break;
					}
				}
			}

			/*
			WidgetBehavior[] widgets = this.GetComponentsInChildren<WidgetBehavior>();
			for (int i = 0; i < this.transform.GetChildCount(); i++)
			{
				GameObject go = this.transform.GetChild(i).gameObject;
				WidgetBehavior widget = go.GetComponent<WidgetBehavior>();
				
				if (widget != null)
				{
					switch (widget.Data.snapToMode)
					{
					case WidgetData.SnapToMode.AR:
						widget.gameObject.SetActive(!value);
						break;
					case WidgetData.SnapToMode.Both:
						widget.gameObject.SetActive(true);
						break;
					case WidgetData.SnapToMode.Snap:
						widget.gameObject.SetActive(value);
						break;
					}
				}
			}
			*/
		}
	}
	
	// What is out current frame?
	private int currentFrameIndex = -1;
	public int CurrentFrameIndex {
		get { return currentFrameIndex; }
		set 
		{ 
#if UNITY_EDITOR
			//Debug.Log (string.Format("Setting current frame index to: {0}, current frameIndex: {1}", value, currentFrameIndex));
#endif
			Debug.Log ("CurrentFrameIndex");
			if (currentFrameIndex != value && value >= 0)
			{

				if (currentFrameIndex >= 0 && currentFrameIndex < data.frames.Count)
				{
					DeactivatePage(currentFrameIndex);
				}
				currentFrameIndex = value; 
				

				int downloadsRemaining = DownloadsRemainingForFrame(currentFrameIndex);
				if (downloadsRemaining > 0)
				{
					//Debug.Log (downloadsRemaining + " downloads remaining for frame "+currentFrameIndex);
					currentPageActivated = false;
					//ACPUnityPlugin.logNative("Can't activate frame yet as it hasn't finished loading...");
					
					ACPUnityPlugin.Instnace.setDownloadProgress(0.0f);

					// Start downloading the frame data
					StartDownloadsIfRequiredForFrame(currentFrameIndex);
				}
				else
				{
					//Debug.Log ("activating frame because all downloads are ready");
					ActivatePage(currentFrameIndex);
					currentPageActivated = true;
				}
			}
		}
	}
	
	#region Utility
	static int SortByYHeight(WidgetBehavior w1, WidgetBehavior w2)
	{
		float y1, y2;
		y1 = w1.transform.localPosition.y;
		y2 = w2.transform.localPosition.y;
		
		if (y1 > y2) return -1;
		else if (y1 < y2) return 1;
		else return 0;
	}
	#endregion
	
	#region Creation and Destruction
	// Use this for initialization
	void Start ()
	{
		gameObject.name = "Area: " + data.id;
		
//		manager = Camera.main.GetComponent<ACPTrackingManager> ();

		setAreaPosition();
		transform.localRotation = Quaternion.Euler (data.rotation);
		
		widgetBehaviours = new List<List<WidgetBehavior>>();
		for (int i = 0; i < this.data.frames.Count; i++)
		{
			widgetBehaviours.Add(new List<WidgetBehavior>());
			TexturesRequiredForFrame[i] = new Dictionary<string, int>();
			FilesRequiredForFrame[i] = new Dictionary<string, int>();
			AudioClipsRequiredForFrame[i] = new Dictionary<string, int>();
			ModelsRequiredForFrame[i] = new Dictionary<ModelObjData, int>();
		}
		
		for (int i = 0; i < this.data.frames.Count; i++)
		{
			CalculateDownloadsRequiredForFrameIndex(i);
		}
		
		DownloadCache.Instance.DownloadCompleteFile += DownloadLoaded;
		DownloadCache.Instance.DownloadCompleteTexture += TextureLoaded;
		DownloadCache.Instance.DownloadCompleteAudioClip += AudioClipLoaded;
		DownloadCache.Instance.DownloadFailed += DownloadFailed;

		Debug.Log("StartDownloadsIfRequiredForFrame");
		// Start downloading frame 0!
		StartDownloadsIfRequiredForFrame(0);
	}

	public void setAreaPosition()
	{
		// Set our position from the json data

		//for the new snap to sizing method - https://visualjazz.jira.com/browse/VR-529
		transform.localPosition = data.position;
	}

	protected void StartDownloadsIfRequiredForFrame(int frameIndex)
	{
		//ACPUnityPlugin.logNative(String.Format("StartDownloadsIfRequiredForFrame({0})", frameIndex));
		if (DownloadsRemainingForFrame(frameIndex) == 0) 
		{
			FinishedPreloadingFrame(frameIndex);
		} 
		else 
		{
			//Debug.Log("Prioritising remaining downloads for frame " + frameIndex);
			foreach (string url in TexturesRequiredForFrame[frameIndex].Keys) {
				
				if (!Textures.ContainsKey(url))
				{
					DownloadCache.Instance.LoadFile(url, DownloadCache.DownloadType.TextureDownload);
				}
			}
			foreach (string url in FilesRequiredForFrame[frameIndex].Keys) {
				
				if (!Files.ContainsKey(url))
				{
					DownloadCache.Instance.LoadFile(url, DownloadCache.DownloadType.FileDownload);
				}
			}
			foreach (string url in AudioClipsRequiredForFrame[frameIndex].Keys) {
				
				if (!AudioClips.ContainsKey(url))
				{
					DownloadCache.Instance.LoadFile (url, DownloadCache.DownloadType.AudioClipDownload);
				}
			}
		}
	}
	
	protected void CalculateDownloadsRequiredForFrameIndex(int frameIndex)
	{
		FrameData frameData = this.data.frames[frameIndex];
		//ACPUnityPlugin.logNative(string.Format("CalculateDownloadsRequiredForFrame({0})", frameIndex));
		
		foreach (WidgetData wd in frameData.widgets) {
			foreach (string url in wd.TexturesRequired ()) {
				if (url != null) {
					if (!TexturesRequired.ContainsKey (url)) {
						//ACPUnityPlugin.logNative ("Need to download: " + url);
						TexturesRequired [url] = DownloadRetryCountTotal;
					}
					if (!TexturesRequiredForFrame[frameIndex].ContainsKey(url))
					{
						TexturesRequiredForFrame[frameIndex][url] = DownloadRetryCountTotal;
					}
				}
			}
			foreach (string url in wd.FilesRequired ()) {
				if (url != null) {
					if (!FilesRequired.ContainsKey (url)) {
						//ACPUnityPlugin.logNative ("Need to download: " + url);
						FilesRequired [url] = DownloadRetryCountTotal;
					}
					if (!FilesRequiredForFrame[frameIndex].ContainsKey(url))
					{
						FilesRequiredForFrame[frameIndex][url] = DownloadRetryCountTotal;
					}
				}
			}
			foreach (string url in wd.AudioClipsRequired()) {
				if (url != null) {
					if (!AudioClipsRequired.ContainsKey (url)) {
						//ACPUnityPlugin.logNative ("Need to download: " + url);
						AudioClipsRequired [url] = DownloadRetryCountTotal;
					}
					
					if (!AudioClipsRequiredForFrame[frameIndex].ContainsKey(url))
					{
						AudioClipsRequiredForFrame[frameIndex][url] = DownloadRetryCountTotal;
					}
				}
			}
			
			if (wd.GetType() == typeof(ModelObjData)) {
				ModelObjData modelData = (ModelObjData) wd;
				ModelsRequired.Add (modelData, DownloadRetryCountTotal);
				ModelsRequiredForFrame[frameIndex][modelData] = DownloadRetryCountTotal;
			}
		}
		
		if (frameIndex == 0)
		{
			TotalItemsToLoad = TexturesRequiredForFrame[frameIndex].Keys.Count + FilesRequiredForFrame[frameIndex].Keys.Count + AudioClipsRequiredForFrame[frameIndex].Keys.Count + ModelsRequiredForFrame[frameIndex].Keys.Count;
		}
		
		// Store the totals for each frame so we can calculate frame load progress when we need to
		TexturesTotalForFrame[frameIndex] = TexturesRequiredForFrame[frameIndex].Keys.Count;
		FilesTotalForFrame[frameIndex] = FilesRequiredForFrame[frameIndex].Keys.Count;
		AudioClipsTotalForFrame[frameIndex] = AudioClipsRequiredForFrame[frameIndex].Keys.Count;
		ModelsTotalForFrame[frameIndex] = ModelsRequiredForFrame[frameIndex].Keys.Count;
	}
	
	/*
	protected void DownloadDataForFrame(FrameData frameData)
	{
		foreach (WidgetData wd in frameData.widgets) {
			foreach (string url in wd.TexturesRequired ()) {
				if (url != null) {
					if (!TexturesRequired.ContainsKey (url)) {
						//ACPUnityPlugin.logNative ("Need to download: " + url);
						TexturesRequired [url] = DownloadRetryCountTotal;
					}
					
				}
			}
			foreach (string url in wd.FilesRequired ()) {
				if (url != null) {
					if (!FilesRequired.ContainsKey (url)) {
						//ACPUnityPlugin.logNative ("Need to download: " + url);
						FilesRequired [url] = DownloadRetryCountTotal;
					}
				}
			}
			foreach (string url in wd.AudioClipsRequired()) {
				if (url != null) {
					if (!AudioClipsRequired.ContainsKey (url)) {
						//ACPUnityPlugin.logNative ("Need to download: " + url);
						AudioClipsRequired [url] = DownloadRetryCountTotal;
					}
				}
			}
			
			if (wd.GetType() == typeof(ModelObjData)) {
				ModelObjData modelData = (ModelObjData) wd;
				 ModelsRequired.Add (modelData, 5);
			}
		}
		
		TotalItemsToLoad = DownloadsRemaining;
		ItemsLoaded = 0;
		
		StartDownloadsIfRequired();

	}
	*/

	// Update is called once per frame
	void Update ()
	{
		List<WidgetBehavior> widgets = new List<WidgetBehavior>(GetComponentsInChildren<WidgetBehavior>());
		widgets.Sort(SortByYHeight);
		
		int queue = 2999;
		foreach (WidgetBehavior widget in widgets)
		{
			foreach (Transform t in widget.transform)
			{
				if (t.gameObject.GetComponent<Renderer>())
				{
					t.gameObject.GetComponent<Renderer>().material.renderQueue = queue;
				}
			}
			queue--;
		}
	}
	
	public void OnDestroy ()
	{
		DownloadCache.Instance.DownloadCompleteFile -= DownloadLoaded;
		DownloadCache.Instance.DownloadCompleteAudioClip -= AudioClipLoaded;;
		DownloadCache.Instance.DownloadCompleteTexture -= TextureLoaded;;
		DownloadCache.Instance.DownloadFailed -= DownloadFailed;
		
		foreach (Texture2D texture in Textures.Values) {
			GameObject.DestroyImmediate (texture);
		}
		Textures.Clear ();
		
		foreach (AudioClip audioClip in AudioClips.Values) {
			GameObject.DestroyImmediate (audioClip);
		}
		AudioClips.Clear ();		
	}
	
	#endregion

	public void FinishedPreloadingFrame (int frameIndex)
	{
		Debug.Log("FinishedPreloadingFrame");
		if(framesLoaded.ContainsKey(frameIndex) == false){
			framesLoaded[frameIndex] = true;

			if (frameIndex == 0)
			{
				AreaLoadProgress (this, 1.0f);
				
				// If we finished loading frame 0, we have finished preloading...
				if (!PreloadFinished)
				{
					PreloadFinished = true;
					// Tell people about it..
					if (AreaLoaded != null)
						AreaLoaded (this);
				}
				// Start pre-loading the other frames...
				StartCoroutine (PreloadOtherFrames());
			}
			else
			{
				//Debug.Log ("finished preloading frame " + frameIndex);
				if ((CurrentFrameIndex == frameIndex) && (currentPageActivated == false))
				{		
					// Display the frame
					currentPageActivated = true;
					ActivatePage(CurrentFrameIndex);
				} else {
					//Debug.Log (".. but its not the current frame or the currentPage is already activated");
				}
			}
		}
	}
	
	private IEnumerator PreloadOtherFrames()
	{
		// We delay pre-loading of the other frames by the duration of the longest appear effect, so that loading the images in the background doesnt effect the
		// performance of the appearance affects on the first frame
		
		// Calculate how much to delay loading the other frames
		float delay = 0.0f;
		foreach (WidgetData widgetData in this.data.frames[0].widgets)
		{
			float longest = EffectData.FindLongestEffect(widgetData.appearEffects);
			if (longest > delay)
			{
				delay = longest;
			}
		}
		
		// Delay
		yield return new WaitForSeconds(delay);

		//Debug.Log ("PReload Other frames");
		// Start loading the other frames
		for (int i = 1; i < this.data.frames.Count; i++)
		{
			StartDownloadsIfRequiredForFrame(i);
		}
	}
		
	private void PrintRemainingFiles ()
	{
		return;
/*#if UNITY_EDITOR
		StringBuilder sb = new StringBuilder ();
		sb.AppendFormat ("{0} textures remaining\n", TexturesRequired.Keys.Count);
		sb.Append ("-----------------\n");
		foreach (String url in TexturesRequired.Keys) {
			sb.Append (url);
			sb.Append ("\n");
		}
		sb.Append ("-----------------\n");
		
		sb.AppendFormat ("{0} files remaining\n", FilesRequired.Keys.Count);
		sb.Append ("-----------------\n");
		foreach (String url in FilesRequired.Keys) {
			sb.Append (url);
			sb.Append ("\n");
		}
		sb.Append ("-----------------");
		sb.AppendFormat ("{0} sounds remaining\n", AudioClipsRequired.Keys.Count);
		sb.Append ("-----------------\n");
		foreach (String url in AudioClipsRequired.Keys) {
			sb.Append (url);
			sb.Append ("\n");
		}
		sb.Append ("-----------------");			
		ACPUnityPlugin.logNative (sb.ToString ());
#endif*/
	}
	
	protected void CheckDownloadFinished()
	{
		Debug.Log ("CheckDownloadFinished");
		for (int i = 0; i < this.data.frames.Count; i++)
		{
			if (this.DownloadsRemainingForFrame(i) - this.ModelsRequiredForFrame[i].Keys.Count == 0)
			{
				// we have models we need to start loading
				
				if (ModelsRequiredForFrame[i].Keys.Count > 0)
				{
					foreach (ModelObjData data in this.ModelsRequiredForFrame[i].Keys) {
						GameObject model = new GameObject ();
						model.transform.parent = this.transform;
						OBJ obj = model.AddComponent<OBJ> ();
						obj.data = data;
						obj.materialString = System.Text.ASCIIEncoding.ASCII.GetString (this.Files[data.materialUrl]);
						obj.objString = System.Text.ASCIIEncoding.ASCII.GetString (this.Files[data.modelUrl]);
						obj.diffuseTexture = this.Textures[data.imageUrl];
					}
				}
				else
				{
					FinishedPreloadingFrame(i);
				}
			}
		}
	}
	
	protected void UpdateFrameProgress()
	{
		if((CurrentFrameIndex >= 0) && (currentPageActivated==false)){
			float remaining = (float) (TexturesRequiredForFrame[CurrentFrameIndex].Keys.Count + AudioClipsRequiredForFrame[CurrentFrameIndex].Keys.Count + FilesRequiredForFrame[CurrentFrameIndex].Keys.Count + ModelsRequiredForFrame[CurrentFrameIndex].Keys.Count);
			float total = (float) (TexturesTotalForFrame[CurrentFrameIndex] + FilesTotalForFrame[CurrentFrameIndex] + AudioClipsTotalForFrame[CurrentFrameIndex] + ModelsTotalForFrame[CurrentFrameIndex]);
			float progress = (float)1.0 - (remaining / total);
			ACPUnityPlugin.Instnace.setDownloadProgress(progress * 0.9f); //our last 10% is reserved for when we activate
			//Debug.Log ("update frame progress: "+remaining + " downloads remaining for frame "+currentFrameIndex);
			StartDownloadsIfRequiredForFrame(currentFrameIndex);
		}
	}
	
	public void TextureFailed (string url)
	{
		//ACPUnityPlugin.logNative ("Failed to load texture: " + url);
		PrintRemainingFiles ();

		// If a texture fails, retry until our retry count is 0...
		if (TexturesRequired [url] > 0) {
			TexturesRequired [url] = TexturesRequired [url] - 1;
			DownloadCache.Instance.LoadFile (url, DownloadCache.DownloadType.TextureDownload);
		} else {
			// Shit we are out of retries......... sadface.
			TexturesRequired.Remove (url);
			
			foreach (int i in TexturesRequiredForFrame.Keys)
			{
				Dictionary<string, int> frameRequired = TexturesRequiredForFrame[i];
				frameRequired.Remove(url);
			}
		}
		
		CheckDownloadFinished();
	}
	
	public void AudioClipFailed (string url)
	{
		//ACPUnityPlugin.logNative ("Failed to load audio: " + url);
		PrintRemainingFiles ();

		// If a texture fails, retry until our retry count is 0...
		if (AudioClipsRequired [url] > 0) {
			AudioClipsRequired [url] = AudioClipsRequired [url] - 1;
			DownloadCache.Instance.LoadFile (url, DownloadCache.DownloadType.AudioClipDownload);
		} else {
			// Shit we are out of retries......... sadface.
			AudioClipsRequired.Remove (url);
			
			foreach (int i in AudioClipsRequiredForFrame.Keys)
			{
				Dictionary<string, int> frameRequired = AudioClipsRequiredForFrame[i];
				frameRequired.Remove(url);
			}
		}
		
		CheckDownloadFinished();
	}
	
	public void DownloadFailed (string url, DownloadCache.DownloadType downloadType)
	{
	
		switch (downloadType)
		{
		case DownloadCache.DownloadType.TextureDownload:
			TextureFailed(url);
			break;
		case DownloadCache.DownloadType.AudioClipDownload:
			AudioClipFailed(url);
			break;
		case DownloadCache.DownloadType.FileDownload:
		default:
			FileFailed(url);
			break;
			
		}
	}
	
	public void FileFailed(string url)
	{
		PrintRemainingFiles ();
		
		if (FilesRequired[url] > 0)
		{
			FilesRequired[url] = FilesRequired[url]-1;
			DownloadCache.Instance.LoadFile(url, DownloadCache.DownloadType.FileDownload);
		}
		else
		{
			// Shit we are out of retries...... sadface.
			FilesRequired.Remove(url);
			
			foreach (int i in FilesRequiredForFrame.Keys)
			{
				Dictionary<string, int> frameRequired = FilesRequiredForFrame[i];
				frameRequired.Remove(url);
			}
		}
		
		CheckDownloadFinished();
	}
	
	public void TextureLoaded (string url, Texture2D texture)
	{
		if (TexturesRequired.ContainsKey(url)){
			ItemsLoaded++;
			AreaLoadProgress (this, ItemsLoaded / TotalItemsToLoad);
		}

		// Keep reference to texture
		Textures [url] = texture;

		// Update the required list
		TexturesRequired.Remove (url);
		
		// Update the frame lists
		foreach (int i in TexturesRequiredForFrame.Keys)
		{
			Dictionary<string, int> frameRequired = TexturesRequiredForFrame[i];
			frameRequired.Remove(url);
		}
		
		UpdateFrameProgress();
		PrintRemainingFiles ();
		CheckDownloadFinished();
	}
	
	public void AudioClipLoaded (string url, AudioClip audioClip)
	{
		if (AudioClipsRequired.ContainsKey(url)){
			ItemsLoaded++;
			AreaLoadProgress (this, ItemsLoaded / TotalItemsToLoad);
		}

		// Store reference to the clip
		AudioClips [url] = audioClip;

		// Update the required list
		AudioClipsRequired.Remove (url);
		
		// Update the frame lists
		foreach (int i in AudioClipsRequiredForFrame.Keys)
		{
			Dictionary<string, int> frameRequired = AudioClipsRequiredForFrame[i];
			frameRequired.Remove(url);
		}

		UpdateFrameProgress();
		PrintRemainingFiles ();
		CheckDownloadFinished();
	}
	
	public void DownloadLoaded (string url, byte[] bytes)
	{
		if(FilesRequired.ContainsKey(url)){
			ItemsLoaded++;
			AreaLoadProgress(this, ItemsLoaded / TotalItemsToLoad);
		}

		// Store a reference to the file
		Files[url] = bytes;
		
		// Update the required list
		FilesRequired.Remove (url);
		
		// Update the frame lists
		foreach (int i in FilesRequiredForFrame.Keys)
		{
			Dictionary<string, int> frameRequired = FilesRequiredForFrame[i];
			frameRequired.Remove(url);
		}

		UpdateFrameProgress();
		PrintRemainingFiles ();
		CheckDownloadFinished();
	}
	
	public void ObjLoadFinished (OBJ obj)
	{	
		// Store a reference to the model...
		Models.Add(obj.data, obj.gameObject);
		
		// Update the required list
		ModelsRequired.Remove(obj.data);
		// Update the frame lists
		foreach (int i in ModelsRequiredForFrame.Keys)
		{
			Dictionary<ModelObjData, int> frameRequired = ModelsRequiredForFrame[i];
			frameRequired.Remove(obj.data);
		}
		
		if (ModelsRequired.Keys.Count <= 0) 
		{
			CheckDownloadFinished();
		}
	}

	/* Create different Widgets in a All frames - snapToMode ?? */
	private void ActivatePage (int pageIndex)
	{
		Debug.Log ("ActivatePage");
		//Debug.Log ("Activate page: " + pageIndex + " page count: " + data.frames.Count);
		if (pageIndex < data.frames.Count) {
			FrameData frame = data.frames [pageIndex];
			//Debug.Log ("Frame has " + frame.widgets.Count + " widgets");
			for (int i = 0; i < frame.widgets.Count; i++) {
				WidgetData widget = frame.widgets[i];
				WidgetBehavior widgetB = widget.CreateBehavior (this, i);
				this.widgetBehaviours[pageIndex].Add(widgetB);
				
				switch ( widget.snapToMode )
				{
				case WidgetData.SnapToMode.AR:
					widgetB.gameObject.SetActive(!this.Sticky);
					break;
				case WidgetData.SnapToMode.Both:
					widgetB.gameObject.SetActive(true);
					break;
				case WidgetData.SnapToMode.Snap:
					widgetB.gameObject.SetActive(this.Sticky);
					break;
				}
			}
		}

        if (this.data.trackableData != null)
        {
            ACPUnityPlugin.Instnace.trackPageView(String.Format("{0}/frame-{1}", areaPageUrl, pageIndex + 1));
            ACPUnityPlugin.Instnace.trackingEvent("FrameDisplay", this.data.trackableData.id, this.data.id, pageIndex, -1, null, null, null, null, null, -1.0f);
            ACPUnityPlugin.Instnace.trackEvent("Frame", "FrameDisplay", this.data.trackableData.id + "-" + this.data.id + "PageIndex:" + pageIndex, 0);
        }
        ACPUnityPlugin.Instnace.setDownloadProgress(1.0f);

		setAreaPosition();
		ActivateAreaFrame(this,pageIndex);
	}
	
	private void DeactivatePage(int index)
	{
		//Debug.Log("Deactivate page: " + index + " page count: " + data.frames.Count);
		
		foreach (WidgetBehavior bh in this.widgetBehaviours[index])
		{
			bh.Remove();
		}
		
		this.widgetBehaviours[index].Clear();
	}
	
	public void ButtonClicked (ButtonBehavior sender)
	{
		Debug.Log("ButtonClicked - AreaBehavior");
		// Handle button clicks..
        ACPUnityPlugin.Instnace.setDownloadProgressToZero(); 

		switch (sender.data.action)
		{
		case ButtonData.ButtonAction.Page:
			Debug.Log ("ButtonData.ButtonAction.Page");
			ACPUnityPlugin.Instnace.trackingEvent("ARFrameButtonClick", data.trackableData.id, data.id, this.CurrentFrameIndex, sender.index, null, null, null, sender.data.pageIndex.ToString(), "", -1.0f);
			CurrentFrameIndex = sender.data.pageIndex;
			break;
		case ButtonData.ButtonAction.Link:  //Art & Culture -> Tree Mendour Growth -> WebView
			Debug.Log ("ButtonData.ButtonAction.Link");
			ACPUnityPlugin.Instnace.setCustomVariable (1, "resource_url", sender.data.linkUrl);
			ACPUnityPlugin.Instnace.trackPageView (String.Format ("{0}/frame{1}/LinkButton-w{2}", areaPageUrl, CurrentFrameIndex, sender.index + 1));
			//ACPUnityPlugin.Instnace.trackEvent ("WIDGET", "LINK", "w" + sender.index + 1, 0);
                if (AppManager.Instnace.isVuforiaOn)
                {
                    ACPUnityPlugin.Instnace.trackEvent("SCAN-WIDGET", "LINK", "Tid_" + AppManager.Instnace.triggerId, 0);
                    ACPUnityPlugin.Instnace.CampaignEventsData("SCAN-WIDGET", "LINK");
                } else {

                    if (AppManager.Instnace.triggerHistoryMode == eTriggerHistoryMode.TriggerHistoryModeChannel)
                    {
                        ACPUnityPlugin.Instnace.trackEvent("HUB-WIDGET", "LINK", "Tid_" + AppManager.Instnace.triggerId, 0);
                        ACPUnityPlugin.Instnace.CampaignEventsData("HUB-WIDGET", "LINK");

                    } else if(AppManager.Instnace.triggerHistoryMode == eTriggerHistoryMode.TriggerHistoryModeLoved){
                        ACPUnityPlugin.Instnace.trackEvent("LOVED-WIDGET", "LINK", "Tid_" + AppManager.Instnace.triggerId, 0);
                        ACPUnityPlugin.Instnace.CampaignEventsData("LOVED-WIDGET", "LINK");
                    }
                }
			ACPUnityPlugin.Instnace.trackingEvent("ARLinkButtonClick", data.trackableData.id, data.id, this.CurrentFrameIndex, sender.index, null, null, null, sender.data.linkUrl, null, -1.0f);
			ACPUnityPlugin.Instnace.loadWebUrl (this.data.trackableData.id, this.data.id, this.currentFrameIndex, sender.data.id, sender.data.linkUrl, sender.data.linkExternal, sender.data.fullscreenView, sender.data.shareTitle, sender.data.shareText, sender.data.shareUrl);
			break;
		case ButtonData.ButtonAction.Share:
			Debug.Log("ButtonData.ButtonAction.Share");
			ACPUnityPlugin.Instnace.setCustomVariable (1, "resource_url", sender.data.linkUrl);
			ACPUnityPlugin.Instnace.trackPageView (String.Format ("{0}/frame{1}/ShareButton-w{2}", areaPageUrl, CurrentFrameIndex, sender.index + 1));
			ACPUnityPlugin.Instnace.trackEvent ("WIDGET", "SHARE", "w" + sender.index + 1, 0);
                if (AppManager.Instnace.isVuforiaOn){
                    ACPUnityPlugin.Instnace.trackEvent("SCAN-WIDGET", "SHARE", "Tid_" + AppManager.Instnace.triggerId, 0);
                    ACPUnityPlugin.Instnace.CampaignEventsData("SCAN-WIDGET", "SHARE");
                } else {
                    if (AppManager.Instnace.triggerHistoryMode == eTriggerHistoryMode.TriggerHistoryModeChannel)
                    {
                        ACPUnityPlugin.Instnace.trackEvent("HUB-WIDGET", "SHARE", "Tid_" + AppManager.Instnace.triggerId, 0);
                        ACPUnityPlugin.Instnace.CampaignEventsData("HUB-WIDGET", "SHARE");
                    }
                    else if (AppManager.Instnace.triggerHistoryMode == eTriggerHistoryMode.TriggerHistoryModeLoved)
                    {
                        ACPUnityPlugin.Instnace.trackEvent("LOVED-WIDGET", "SHARE", "Tid_" + AppManager.Instnace.triggerId, 0);
                        ACPUnityPlugin.Instnace.CampaignEventsData("LOVED-WIDGET", "SHARE");
                    }
                }
			ACPUnityPlugin.Instnace.trackingEvent("ARShareButtonClick", data.trackableData.id, data.id, this.CurrentFrameIndex, sender.index, null, null, null, sender.data.linkUrl, null, -1.0f);
			ACPUnityPlugin.Instnace.shareUrl (this.data.trackableData.id, this.data.id, this.currentFrameIndex, sender.data.id, sender.data.linkUrl, sender.data.shareTitle, sender.data.shareText, sender.data.shareImage);
			break;
		case ButtonData.ButtonAction.Music:
			Debug.Log("ButtonData.ButtonAction.Music");
			ACPUnityPlugin.Instnace.setCustomVariable (1, "resource_url", sender.data.linkUrl);
			ACPUnityPlugin.Instnace.trackPageView (String.Format ("{0}/frame{1}/MusicGalleryButton-w{2}", areaPageUrl, CurrentFrameIndex, sender.index + 1));
			ACPUnityPlugin.Instnace.trackEvent ("WIDGET", "MUSICGALLERY", "w" + sender.index + 1, 0);
                if (AppManager.Instnace.isVuforiaOn){ 
                    ACPUnityPlugin.Instnace.trackEvent("SCAN-WIDGET", "MUSICGALLERY", "Tid_" + AppManager.Instnace.triggerId, 0);
                    ACPUnityPlugin.Instnace.CampaignEventsData("SCAN-WIDGET", "LINK");
                } else {
                    if (AppManager.Instnace.triggerHistoryMode == eTriggerHistoryMode.TriggerHistoryModeChannel)
                    {
                        ACPUnityPlugin.Instnace.trackEvent("HUB-WIDGET", "MUSICGALLERY", "Tid_" + AppManager.Instnace.triggerId, 0);
                        ACPUnityPlugin.Instnace.CampaignEventsData("HUB-WIDGET", "MUSICGALLERY");
                    }
                    else if (AppManager.Instnace.triggerHistoryMode == eTriggerHistoryMode.TriggerHistoryModeLoved){

                        ACPUnityPlugin.Instnace.trackEvent("LOVED-WIDGET", "MUSICGALLERY", "Tid_" + AppManager.Instnace.triggerId, 0);
                        ACPUnityPlugin.Instnace.CampaignEventsData("LOVED-WIDGET", "MUSICGALLERY");
                    }
                }
			ACPUnityPlugin.Instnace.trackingEvent("ARMusicGalleryButtonClick", data.trackableData.id, data.id, this.CurrentFrameIndex, sender.index, null, null, null, sender.data.linkUrl, null, -1.0f);
			ACPUnityPlugin.Instnace.musicPlayerStart (this.data.trackableData.id, this.data.id, this.currentFrameIndex, sender.data.id, sender.data.linkUrl);
			break;
		case ButtonData.ButtonAction.Gallery:
			Debug.Log("ButtonData.ButtonAction.Gallery");

                //string[] stringArray = sender.data.linkUrl.Split('/');
                //string newString = "https://cms.viewa.com/" + stringArray[stringArray.Length - 1];
                Caching.ClearCache();
			ACPUnityPlugin.Instnace.setCustomVariable (1, "resource_url", sender.data.linkUrl);
			ACPUnityPlugin.Instnace.trackPageView (String.Format ("{0}/frame{1}/ImageGalleryButton-w{2}", areaPageUrl, CurrentFrameIndex, sender.index + 1));
			ACPUnityPlugin.Instnace.trackEvent ("WIDGET", "IMAGEGALLERY", "w" + sender.index + 1, 0);
                if(AppManager.Instnace.isVuforiaOn){
                    ACPUnityPlugin.Instnace.trackEvent("SCAN-WIDGET", "IMAGEGALLERY", "Tid_" + AppManager.Instnace.triggerId, 0);
                    ACPUnityPlugin.Instnace.CampaignEventsData("SCAN-WIDGET", "IMAGEGALLERY");
                    AppManager.Instnace.isGoingToGalleryFromScan = true;
                    Debug.Log("isGoingToGalleryFromScan - true");
                } else {
                    if (AppManager.Instnace.triggerHistoryMode == eTriggerHistoryMode.TriggerHistoryModeChannel)
                    {
                        ACPUnityPlugin.Instnace.trackEvent("HUB-WIDGET", "IMAGEGALLERY", "Tid_" + AppManager.Instnace.triggerId, 0);
                        ACPUnityPlugin.Instnace.CampaignEventsData("HUB-WIDGET", "IMAGEGALLERY");
                    }
                    else if (AppManager.Instnace.triggerHistoryMode == eTriggerHistoryMode.TriggerHistoryModeLoved)
                    {
                        ACPUnityPlugin.Instnace.trackEvent("LOVED-WIDGET", "IMAGEGALLERY", "Tid_" + AppManager.Instnace.triggerId, 0);
                        ACPUnityPlugin.Instnace.CampaignEventsData("LOVED-WIDGET", "IMAGEGALLERY");
                    }
                    AppManager.Instnace.isGoingToGalleryFromScan = false;
                    Debug.Log("isGoingToGalleryFromScan - false");
                }
                ACPUnityPlugin.Instnace.trackingEvent("ARImageGalleryButtonClick", data.trackableData.id, data.id, this.CurrentFrameIndex, sender.index, null, null, null, sender.data.linkUrl, null, -1.0f);  //sender.data.linkUrl
                ACPUnityPlugin.Instnace.imageGalleryStart (this.data.trackableData.id, this.data.id, this.currentFrameIndex, sender.data.id, sender.data.linkUrl); //sender.data.linkUrl
			break;
		case ButtonData.ButtonAction.Video:
			Debug.Log("ButtonData.ButtonAction.Video");
			ACPUnityPlugin.Instnace.setCustomVariable (1, "resource_url", sender.data.linkUrl);
			ACPUnityPlugin.Instnace.trackPageView (String.Format ("{0}/frame{1}/VideoButton-w{2}", areaPageUrl, CurrentFrameIndex, sender.index + 1));
			ACPUnityPlugin.Instnace.trackEvent ("WIDGET", "VIDEOFULLSCREEN", "w" + sender.index + 1, 0);
			ACPUnityPlugin.Instnace.trackingEvent("ARVideoFullScreenButtonClick", data.trackableData.id, data.id, this.CurrentFrameIndex, sender.index, null, null, null, sender.data.linkUrl, null, -1.0f);
                if (AppManager.Instnace.isVuforiaOn){
                    ACPUnityPlugin.Instnace.trackEvent("SCAN-WIDGET", "VIDEOFULLSCREEN", "Tid_" + AppManager.Instnace.triggerId, 0);
                    ACPUnityPlugin.Instnace.CampaignEventsData("SCAN-WIDGET", "VIDEOFULLSCREEN");
                } else {
                    if (AppManager.Instnace.triggerHistoryMode == eTriggerHistoryMode.TriggerHistoryModeChannel)
                    {
                        ACPUnityPlugin.Instnace.trackEvent("HUB-WIDGET", "VIDEOFULLSCREEN", "Tid_" + AppManager.Instnace.triggerId, 0);
                        ACPUnityPlugin.Instnace.CampaignEventsData("HUB-WIDGET", "VIDEOFULLSCREEN");
                    } 
                    else if (AppManager.Instnace.triggerHistoryMode == eTriggerHistoryMode.TriggerHistoryModeLoved)
                    {
                        ACPUnityPlugin.Instnace.trackEvent("LOVED-WIDGET", "VIDEOFULLSCREEN", "Tid_" + AppManager.Instnace.triggerId, 0);
                        ACPUnityPlugin.Instnace.CampaignEventsData("LOVED-WIDGET", "VIDEOFULLSCREEN");
                    }
                }
//			Screen.orientation = ScreenOrientation.LandscapeLeft;
			StartCoroutine (videoButtonImpl (this.data.trackableData.id, this.data.id, this.currentFrameIndex, sender.data.id, sender.data));
			break;
		case ButtonData.ButtonAction.PhotoBooth:
			Debug.Log("ButtonData.ButtonAction.PhotoBooth");
			ACPUnityPlugin.Instnace.setCustomVariable (1, "resource_url", sender.data.photoboothOverlayImageUrl);
			ACPUnityPlugin.Instnace.trackPageView (String.Format ("{0}/frame{1}/PhotoBoothButton-w{2}", areaPageUrl, CurrentFrameIndex, sender.index + 1));
			ACPUnityPlugin.Instnace.trackEvent ("WIDGET", "PHOTOBOOTH", "w" + sender.index + 1, 0);
                if(AppManager.Instnace.isVuforiaOn){
                    ACPUnityPlugin.Instnace.trackEvent("SCAN-WIDGET", "PHOTOBOOTH", "Tid_" + AppManager.Instnace.triggerId, 0);
                    ACPUnityPlugin.Instnace.CampaignEventsData("SCAN-WIDGET", "PHOTOBOOTH");
                } else {
                    if (AppManager.Instnace.triggerHistoryMode == eTriggerHistoryMode.TriggerHistoryModeChannel)
                    {
                        ACPUnityPlugin.Instnace.trackEvent("HUB-WIDGET", "PHOTOBOOTH", "Tid_" + AppManager.Instnace.triggerId, 0);
                        ACPUnityPlugin.Instnace.CampaignEventsData("HUB-WIDGET", "PHOTOBOOTH");
                    }
                    else if (AppManager.Instnace.triggerHistoryMode == eTriggerHistoryMode.TriggerHistoryModeLoved){

                        ACPUnityPlugin.Instnace.trackEvent("LOVED-WIDGET", "PHOTOBOOTH", "Tid_" + AppManager.Instnace.triggerId, 0);
                        ACPUnityPlugin.Instnace.CampaignEventsData("LOVED-WIDGET", "PHOTOBOOTH");
                    }
                }
			ACPUnityPlugin.Instnace.trackingEvent("ARPhotoBoothButtonClick", data.trackableData.id, data.id, this.CurrentFrameIndex, sender.index, null, null, null, sender.data.photoboothOverlayImageUrl, null, -1.0f);
			ACPUnityPlugin.Instnace.photoBoothStart (this.data.trackableData.id, this.data.id, this.currentFrameIndex, sender.data.id, sender.data.photoboothOverlayImageUrl);
			break;
		case ButtonData.ButtonAction.Map:
			Debug.Log("ButtonData.ButtonAction.Map");
			//ACPUnityPlugin.setCustomVariable (1, "resource_url", sender.data.photoboothOverlayImageUrl);
			ACPUnityPlugin.Instnace.trackPageView (String.Format ("{0}/frame{1}/MapButton-w{2}", areaPageUrl, CurrentFrameIndex, sender.index + 1));
			ACPUnityPlugin.Instnace.trackEvent ("WIDGET", "MAP", "w" + sender.index + 1, 0);
                if(AppManager.Instnace.isVuforiaOn){
                    ACPUnityPlugin.Instnace.trackEvent("SCAN-WIDGET", "MAP", "Tid_" + AppManager.Instnace.triggerId, 0);
                    ACPUnityPlugin.Instnace.CampaignEventsData("SCAN-WIDGET", "MAP");
                }else {
                    if (AppManager.Instnace.triggerHistoryMode == eTriggerHistoryMode.TriggerHistoryModeChannel)
                    {
                        ACPUnityPlugin.Instnace.trackEvent("HUB-WIDGET", "MAP", "Tid_" + AppManager.Instnace.triggerId, 0);
                        ACPUnityPlugin.Instnace.CampaignEventsData("HUB-WIDGET", "MAP");
                    }
                    else if (AppManager.Instnace.triggerHistoryMode == eTriggerHistoryMode.TriggerHistoryModeLoved)
                    { 
                        ACPUnityPlugin.Instnace.trackEvent("LOVED-WIDGET", "MAP", "Tid_" + AppManager.Instnace.triggerId, 0);
                        ACPUnityPlugin.Instnace.CampaignEventsData("LOVED-WIDGET", "MAP");
                    }
                }
			ACPUnityPlugin.Instnace.trackingEvent("ARMapButtonClick", data.trackableData.id, data.id, this.CurrentFrameIndex, sender.index, null, null, null, sender.data.photoboothOverlayImageUrl, null, -1.0f);
			ACPUnityPlugin.Instnace.mapStart (this.data.trackableData.id, this.data.id, this.currentFrameIndex, sender.data.id, sender.data.mapTitle, sender.data.mapDescription, sender.data.mapLatitude, sender.data.mapLongtitude);
			break;
		}
	}
	
	private IEnumerator videoButtonImpl (ulong triggerId, int areaIndex, int frameIndex, int widgetIndex, ButtonData data)
	{
		//Debug.Log ("video buttin impl...");
		if (data.linkUrl.StartsWith ("yt://")) {
			string infoUrl = VideoData.getYoutubeVideoInfoUrl (data.linkUrl);
		//	Debug.Log (infoUrl);
			WWW www = new WWW (infoUrl);
			yield return www;
			data.linkUrl = VideoData.getMp4UrlFromVideoInfo (www.text);
		//	Debug.Log (data.linkUrl);
		}
		
		Debug.Log ("calling loadFullscreenVideo from AreaBehavior");
		ACPUnityPlugin.Instnace.loadFullscreenVideo (triggerId, areaIndex, frameIndex, widgetIndex, data.linkUrl, 0, data.endImage, data.endUrl, data.shareTitle, data.shareText, data.shareUrl);
	}
	
	public int DownloadsRemainingForFrame(int frameIndex)
	{
		//ACPUnityPlugin.logNative(string.Format("DownloadsReminingForFrame({0})", frameIndex));
		return AudioClipsRequiredForFrame[frameIndex].Keys.Count + TexturesRequiredForFrame[frameIndex].Keys.Count + FilesRequiredForFrame[frameIndex].Keys.Count + ModelsRequiredForFrame[frameIndex].Keys.Count;
	}
	

	public void TrackingFound(bool sticky)
	{
		foreach (WidgetBehavior widget in this.widgetBehaviours[this.CurrentFrameIndex])
		{
			widget.TrackingFound(sticky);
		}
		setAreaPosition();
	}

	public void TrackingLost(bool sticky)
	{
//		Debug.Log ("Tracking LOST!");
		if((this.CurrentFrameIndex >= 0) && (this.widgetBehaviours.Count > this.CurrentFrameIndex)){
			foreach (WidgetBehavior widget in this.widgetBehaviours[this.CurrentFrameIndex])
			{
				widget.TrackingLost(sticky);
			}
		}
		setAreaPosition();
	}
}


