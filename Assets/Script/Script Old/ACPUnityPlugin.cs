using UnityEngine;
using System.Collections;
//using System.Runtime.InteropServices;
using OTPL.UI;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Text;
using SimpleJSON;
using System;
using System.Collections.Generic;
using System.IO;

/// <summary>
/// Native plugin - used to tell the Android/iOS app when to do certain things
/// </summary>

//12345678 times.

public class ACPUnityPlugin : Singleton<ACPUnityPlugin>
{


    public static bool NativeLogEnabled = false;
    public string _shareTitle;
    public string _shareText;
    public string _shareUrl;
    public GameObject preloadPrefab;
    public GoogleAnalyticsV4 googleAnalytics;
    public bool isExternalUrl;
    public Sprite originalSprite;
    public Sprite newSprite;
    //= AppManager.Instnace.spriteAtlas.GetSprite("Hub_Love_Active"); 


    BlankPanel blankPanel;
    ScanPanel scanpanel;
    OptimalVideoPlayer ovp;
    GameObject preloader;
    List<ViewaCampaignEvents> _viewaCampaignEventsList = new List<ViewaCampaignEvents>();
    string imagePath;


#if UNITY_IPHONE
	public const string c_libName = "__Internal";
#else
    public const string c_libName = "AcpUnityPlugin";
#endif

    private void Start()
    { }

    //public void Update()
    //{
    //#if UNITY_ANDROID
    //        if (Input.GetKeyUp (KeyCode.Escape)) {
    //			Debug.Log ("Unity back button pressed");
    //	        AndroidJavaClass jc = new AndroidJavaClass ("com.unity3d.player.UnityPlayer"); 
    //	        AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject> ("currentActivity"); 
    //	        jo.Call ("onBackPressed");
    //		}
    //#endif
    //}

    #region "Video"
    //	[DllImport(ACPUnityPlugin.c_libName)]
    //	private static extern void _setupVideo(string videoUrl, int tex_id, int width, int height, bool loopVideo);
    //	
    //	[DllImport(ACPUnityPlugin.c_libName)]
    //	private static extern float _videoDownloadProgress(string videoUrl);
    //
    //	[DllImport(ACPUnityPlugin.c_libName)]
    //	private static extern void _playVideo(int tex_id, float time);
    //	
    //	[DllImport(ACPUnityPlugin.c_libName)]
    //	private static extern void _pauseVideo(int tex_id);
    //
    //	[DllImport(ACPUnityPlugin.c_libName)]
    //	private static extern int _updateVideo(int tex_id, float time);
    //	
    //	[DllImport(ACPUnityPlugin.c_libName)]
    //	private static extern void _shutdownVideo(int tex_id);
    #endregion

    //	[DllImport(ACPUnityPlugin.c_libName)]
    //	private static extern void _loadWebUrl(ulong triggerId,
    //		int areaIndex,
    //		int frameIndex,
    //		int widgetIndex,
    //		string url, bool external, bool fullScreen, string shareTitle, string shareText, string shareUrl);
    //	
    //	[DllImport(ACPUnityPlugin.c_libName)]
    //	private static extern void _shareUrl(ulong triggerId,
    //		int areaIndex,
    //		int frameIndex,
    //		int widgetIndex,
    //		string url, string title, string text, string image);
    //	
    //	[DllImport(ACPUnityPlugin.c_libName)]
    //	private static extern void _musicPlayerStart(		
    //		ulong triggerId,
    //		int areaIndex,
    //		int frameIndex,
    //		int widgetIndex,
    //		string url);
    //	
    //	[DllImport(ACPUnityPlugin.c_libName)]
    //	private static extern void _imageGalleryStart(
    //		ulong triggerId,
    //		int areaIndex,
    //		int frameIndex,
    //		int widgetIndex,
    //		string url);
    //
    //	[DllImport(ACPUnityPlugin.c_libName)]
    //	private static extern void _loadFullscreenVideo(
    //		ulong triggerId,
    //		int areaIndex,
    //		int frameIndex,
    //		int widgetIndex,
    //		string url, float time, string endImage, string endUrl, string shareTitle, string shareText, string shareUrl);
    //	
    //	[DllImport(ACPUnityPlugin.c_libName)]
    //	private static extern void _setNativeUIVisible(bool visibility);
    //
    //	[DllImport(ACPUnityPlugin.c_libName)]
    //	private static extern void _setShareItem(ulong trigger_id, string url, string title, string text);
    //	
    //	[DllImport(ACPUnityPlugin.c_libName)]
    //	private static extern void _trackPageView(string url);
    //		
    //	[DllImport(ACPUnityPlugin.c_libName)]
    //    private static extern void _trackEvent(string category, string action, string label, int value);
    //	
    //	[DllImport(ACPUnityPlugin.c_libName)]
    //	private static extern void _setCustomVariable(uint index, string name, string value);
    //	
    //	[DllImport(ACPUnityPlugin.c_libName)]
    //	private static extern void _dataLoadFinished ();
    //
    //	[DllImport(ACPUnityPlugin.c_libName)]
    //	private static extern void _buttonWasAutoPlayed ();
    //
    //	[DllImport(ACPUnityPlugin.c_libName)]
    //	private static extern void _dataLoadFailed();
    //	
    //	[DllImport(ACPUnityPlugin.c_libName)]
    //	private static extern void _logNative(string message);
    //	
    //	[DllImport(ACPUnityPlugin.c_libName)]
    //	private static extern void _photoBoothStart(
    //		ulong triggerId,
    //		int areaIndex,
    //		int frameIndex,
    //		int widgetIndex,
    //		string overlayImageUrl);
    //
    //	[DllImport(ACPUnityPlugin.c_libName)]
    //	private static extern void _mapStart(
    //		ulong triggerId,
    //		int areaIndex,
    //		int frameIndex,
    //		int widgetIndex,
    //		string title,
    //		string description,
    //		double latitude,
    //		double longtitude);
    //
    //	[DllImport(ACPUnityPlugin.c_libName)]
    //	private static extern void _showAlert(string title, string text, string buttonText);
    //	
    //	[DllImport(ACPUnityPlugin.c_libName)]
    //	private static extern void _showLocalizedAlert (string title, string text, string buttonText);
    //
    //	[DllImport(ACPUnityPlugin.c_libName)]
    //	private static extern void _setDownloadProgress (float progress);
    //
    //	//bed - commented this old version for the iOS build as we can't overload
    //	//[DllImport(ACPUnityPlugin.c_libName)]
    //	//private static extern void _showStatusOverlay (string title, string subtitle);
    //	
    //	[DllImport(ACPUnityPlugin.c_libName)]
    //	private static extern void _showStatusOverlay (string title);
    //	
    //	[DllImport(ACPUnityPlugin.c_libName)]
    //	private static extern void _hideStatusOverlay();
    //	
    //	[DllImport(ACPUnityPlugin.c_libName)]
    //	private static extern void _setCloseButtonVisibility (bool visibility);
    //	
    //	[DllImport(ACPUnityPlugin.c_libName)]
    //	private static extern void _setNavigationButtonsVisibility (bool visibility);
    //
    //	[DllImport(ACPUnityPlugin.c_libName)]
    //	private static extern void _setNavigationButtonsEnabled (bool enabled);
    //
    //	[DllImport(ACPUnityPlugin.c_libName)]
    //	private static extern void _showHUD (string title, string subtitle, string imageName, float duration);
    //	
    //	[DllImport(ACPUnityPlugin.c_libName)]
    //	private static extern void _showAppVersionError (int gotVersion, int minVersion, int maxVersion);
    //	
    //	[DllImport(ACPUnityPlugin.c_libName)]
    //	private static extern void _trackingEvent(
    //		string eventType,
    //		ulong triggerId,
    //		int areaIndex,
    //		int frameIndex,
    //		int widgetIndex,
    //		string resourceUrl, 
    //		string resourceId,
    //		string resourceDetailId,
    //		string targetUrl,
    //		string socialNetwork,
    //		float videoPlayTime);

    //	[DllImport(ACPUnityPlugin.c_libName)]
    //	private static extern void _recordHistory(string triggerId, string targetId, string jsonString);
    //		
    //#if UNITY_ANDROID
    //	[DllImport(ACPUnityPlugin.c_libName)]
    //	private static extern void _startActivityWithName(string name);
    //#endif

    public void setupVideo(string videoUrl, GameObject videoPlane, int width, int height, bool loopVideo) //public void setupVideo(string videoUrl, int tex_id, int width, int height, bool loopVideo)
    {
#if DEBUG_PLUGIN
		Debug.Log("ACPUnityPlugin::setupVideo");
#endif

#if UNITY_IPHONE || UNITY_ANDROID
        if (Application.platform != RuntimePlatform.OSXEditor)
        {
            //			AppManager.Instnace.videoPlayingPage.PlayVideoFromURL(videoUrl);
            //			_setupVideo(videoUrl, tex_id, width, height, loopVideo);
        }
        GameObject videoBehavior = videoPlane.transform.parent.gameObject;

        if (!videoPlane.GetComponentInParent<OptimalVideoPlayer>())
        {
            ovp = videoBehavior.AddComponent<OptimalVideoPlayer>();
            ovp.SetUpVideo(videoUrl, videoPlane, width, height, loopVideo);
        }
        else
        {
            ovp.SetUpVideo(videoUrl, videoPlane, width, height, loopVideo);
            //ovp.PlayPause();
        }

#endif
    }

    public float videoDownloadProgress(string videoUrl)
    {

#if DEBUG_PLUGIN
		Debug.Log("ACPUnityPlugin::videoDownloadPorgress");
#endif

#if UNITY_IPHONE || UNITY_ANDROID
        if (Application.platform != RuntimePlatform.OSXEditor)
        {
            //			return _videoDownloadProgress(videoUrl);
        }
#endif

        return 1.0f;
    }

    public void playVideo(Texture2D planeTexture)  // GameObject videoPlanepublic void playVideo(int tex_id, float time)
    {
        if (ovp != null)
        {
            ovp.PlayVideo(planeTexture);
        }

        //#if DEBUG_PLUGIN
        //		Debug.Log("ACPUnityPlugin::playVideo");
        //#endif
        //
        //#if UNITY_IPHONE || UNITY_ANDROID
        //    	if( Application.platform != RuntimePlatform.OSXEditor )
        //		{
        //			_playVideo(tex_id, time);
        //		}
        //#endif
    }

    public void pauseVideo() //int tex_id, float time
    {

        if (ovp != null)
        {
            ovp.pauseVideo();
        }
    }

    public void pauseVideo(Texture2D videoTexture, float time) //int tex_id, float time
    {

        if (ovp != null)
        {
            ovp.pauseVideo();
        }
        //#if DEBUG_PLUGIN
        //		Debug.Log("ACPUnityPlugin::pauseVideo");
        //#endif
        //
        //#if UNITY_IPHONE || UNITY_ANDROID
        //    	if( Application.platform != RuntimePlatform.OSXEditor )
        //		{
        //			_pauseVideo(tex_id);
        //		}
        //#endif
    }

    public void shutdownVideo() //int tex_id
    {
        if (ovp != null)
        {
            ovp.ShutDownVideo();
        }

        //#if DEBUG_PLUGIN
        //		Debug.Log("ACPUnityPlugin::shutdownVideo");
        //#endif
        //
        //#if UNITY_IPHONE || UNITY_ANDROID
        //    	if( Application.platform != RuntimePlatform.OSXEditor )
        //		{
        //			_shutdownVideo(tex_id);
        //		}
        //#endif
    }

    //   public VideoUpdateStatus updateVideo() //int tex_id
    //{
    //#if DEBUG_PLUGIN
    //		Debug.Log("ACPUnityPlugin::updateVideo");
    //#endif
    //		
    //#if UNITY_IPHONE || UNITY_ANDROID
    //		if( Application.platform != RuntimePlatform.OSXEditor )
    //		{
    //			return (VideoUpdateStatus) _updateVideo(tex_id, time);
    //		}
    //#endif
    //return VideoUpdateStatus.Loading;
    //}


    public void startActivityWithName(string activityName)
    {
        //#if UNITY_ANDROID
        //		_startActivityWithName(activityName);
        //#endif
    }

    public void loadWebUrl(
        ulong triggerId,
        int areaIndex,
        int frameIndex,
        int widgetIndex,
        string url, bool external, bool fullScreen, string shareTitle, string shareText, string shareUrl)
    {
        _shareTitle = shareTitle;
        _shareText = shareText;
        _shareUrl = shareUrl;

        isExternalUrl = external;

#if UNITY_IPHONE || UNITY_ANDROID
        if (Application.platform != RuntimePlatform.OSXEditor && !external) //Application.platform != RuntimePlatform.OSXEditor && !external
        {
            if (url.Contains(".pdf"))
            {
                isExternalUrl = true;
                Application.OpenURL(url);
            }
            else
            {
                Debug.Log("ACPUnityPlugin: LoadWebUrl");
                //          _loadWebUrl(triggerId, areaIndex, frameIndex, widgetIndex, url, external, fullScreen, shareTitle, shareText, shareUrl);
                AppManager.Instnace.webViewPage.LoadWebUrl(triggerId, areaIndex, frameIndex, widgetIndex, url, external, fullScreen, shareTitle, shareText, shareUrl);
            }
        }
        else
        {
            trackEvent("WebViewExternal", "Url:" + url, "Url", 0);
            Application.OpenURL(url);
        }
#endif
    }

    public void shareUrl(ulong triggerId,
        int areaIndex,
        int frameIndex,
        int widgetIndex,
        string url, string title, string text, string image)
    {

        //#if UNITY_IPHONE || UNITY_ANDROID
        //		if( Application.platform != RuntimePlatform.OSXEditor )
        //		{
        //			_shareUrl(triggerId, areaIndex, frameIndex, widgetIndex, url, title, text, image);
        //		}
        //#endif
    }

    public void musicPlayerStart(
        ulong triggerId,
        int areaIndex,
        int frameIndex,
        int widgetIndex,
        string url)
    {
        //#if UNITY_IPHONE || UNITY_ANDROID
        //		if( Application.platform != RuntimePlatform.OSXEditor )
        //		{
        //			_musicPlayerStart(triggerId, areaIndex, frameIndex, widgetIndex, url);
        //		}
        //#endif
    }

    public void imageGalleryStart(
        ulong triggerId,
        int areaIndex,
        int frameIndex,
        int widgetIndex,
        string url)
    {
        ImageGallaryPanel imageGalleryPanel = (ImageGallaryPanel)CanvasManager.Instnace.ReturnPanelManager(ePanelManager.MainMenuPanelManager).ReturnPanel(ePanels.ImageGallary_Panel);
        if (imageGalleryPanel != null)
        {
            imageGalleryPanel.mTriggerId = triggerId;
            imageGalleryPanel.mAreaIndex = areaIndex;
            imageGalleryPanel.mFrameIndex = frameIndex;
            imageGalleryPanel.mWidgetIndex = widgetIndex;
            imageGalleryPanel.mUrl = url;
            CanvasManager.Instnace.ReturnPanelManager(ePanelManager.MainMenuPanelManager).AddPanel(ePanels.ImageGallary_Panel);
            //imageGalleryPanel._imageGalleryStart (triggerId, areaIndex, frameIndex, widgetIndex, url);
        }
        //#if UNITY_IPHONE || UNITY_ANDROID
        //		if( Application.platform != RuntimePlatform.OSXEditor )
        //		{
        //			_imageGalleryStart(triggerId, areaIndex, frameIndex, widgetIndex, url);
        //		}
        //#endif
    }

    public void loadFullscreenVideo(
        ulong triggerId,
        int areaIndex,
        int frameIndex,
        int widgetIndex,
        string url, float time, string endImage, string endUrl, string shareTitle, string shareText, string shareUrl)
    {
        _shareText = shareText;
        _shareTitle = shareTitle;
        _shareUrl = shareUrl;
        Debug.Log("loadFullscreenVideo");
        AppManager.Instnace.videoPlayingPage.LoadFullscreenVideo(triggerId, areaIndex, frameIndex, widgetIndex, url, time, endImage, endUrl, shareTitle, shareText, shareUrl);

#if UNITY_IPHONE || UNITY_ANDROID
        //if( Application.platform != RuntimePlatform.OSXEditor )
        //{
        //_loadFullscreenVideo(triggerId, areaIndex,frameIndex, widgetIndex, url, time, endImage, endUrl, shareTitle, shareText, shareUrl);
        //}
#endif
    }

    public void setNativeUIVisible(bool visibility)
    {
        //#if UNITY_IPHONE || UNITY_ANDROID
        //		if( Application.platform != RuntimePlatform.OSXEditor )
        //		{
        //			_setNativeUIVisible(visibility);
        //		}
        //#endif
    }

    public void setShareItem(ulong trigger_id, string url, string title, string text)
    {
        //#if UNITY_IPHONE || UNITY_ANDROID
        //		if( Application.platform != RuntimePlatform.OSXEditor )
        //		{
        //			_setShareItem(trigger_id, url, title, text);
        //		}
        //#endif
    }

    public void trackPageView(string url)
    {

        Debug.Log("--------------------------------------------- >> PageView :" + url);
        //#if UNITY_IPHONE || UNITY_ANDROID
        //		if( Application.platform != RuntimePlatform.OSXEditor )
        //		{
        //			_trackPageView(url);
        //		}
        //#endif
#if !UNITY_EDITOR
        googleAnalytics.LogScreen(url);
#endif
    }

    public void trackScreen(string screenName)
    {
        //Debug.Log("========================================= :" + screenName);
#if !UNITY_EDITOR
        googleAnalytics.LogScreen(screenName);
#endif
    }

    public void trackEvent(string category, string action, string label, int value)
    {
#if !UNITY_EDITOR
        googleAnalytics.LogEvent(category, action, label, value);
#endif
        //#if UNITY_IPHONE || UNITY_ANDROID
        //		if( Application.platform != RuntimePlatform.OSXEditor )
        //		{
        //			_trackEvent(category, action, label, value);
        //		}
        //#endif
    }

    public void trackSocial(string socialNetwork, string socialAction, string socialTarget)
    {
#if !UNITY_EDITOR
        googleAnalytics.LogSocial(socialNetwork, socialAction, socialTarget);
#endif

    }

    public void setCustomVariable(uint index, string name, string value)
    {
        //#if UNITY_IPHONE || UNITY_ANDROID
        //		if( Application.platform != RuntimePlatform.OSXEditor )
        //		{
        //			_setCustomVariable(index, name, value);
        //		}
        //#endif
    }

    public void buttonWasAutoPlayed()
    {
        //		#if UNITY_IPHONE || UNITY_ANDROID
        //		if (Application.platform != RuntimePlatform.OSXEditor) {
        //			_buttonWasAutoPlayed();
        //		} else {
        //			Debug.Log("BUTTON WAS AUTOPLAYED");
        //		}
        //		#endif
    }
    public void dataLoadFinished()
    {
        //		#if UNITY_IPHONE || UNITY_ANDROID
        //		if (Application.platform != RuntimePlatform.OSXEditor) {
        //			_dataLoadFinished();
        //		}
        //		#endif
    }

    public void dataLoadFailed()
    {
        //		#if UNITY_IPHONE || UNITY_ANDROID
        //		if (Application.platform != RuntimePlatform.OSXEditor) {
        //			_dataLoadFailed();
        //		}
        //		#endif
    }

    public void logNative(string message)
    {
        //		if (NativeLogEnabled)
        //		{
        //			#if UNITY_IPHONE || UNITY_ANDROID
        //			if (Application.platform != RuntimePlatform.OSXEditor) {
        //				_logNative(message);
        //			}
        //			#endif
        //		}
        //		else
        //		{
        //#if UNITY_EDITOR
        //			Debug.Log(message);
        //#endif
        //		}
    }

    public void photoBoothStart(
        ulong triggerId,
        int areaIndex,
        int frameIndex,
        int widgetIndex,
        string overlayImageUrl)
    {
        //		#if UNITY_IPHONE || UNITY_ANDROID
        //		if (Application.platform != RuntimePlatform.OSXEditor) {
        //			_photoBoothStart (triggerId, areaIndex, frameIndex, widgetIndex, overlayImageUrl);
        //		} else {
        //			Debug.Log ("photoBoothStart(" + overlayImageUrl + ")");
        //		}
        //		#endif
    }

    public void mapStart(
        ulong triggerId,
        int areaIndex,
        int frameIndex,
        int widgetIndex,
        string title,
        string description,
        double latitude,
        double longitude)
    {
        //		#if UNITY_IPHONE || UNITY_ANDROID
        //		if (Application.platform != RuntimePlatform.OSXEditor) {
        //			_mapStart (triggerId, areaIndex, frameIndex, widgetIndex, title, description, latitude, longitude);
        //		} else {
        //			Debug.Log ("mapStart(" + title + " - " + description + " "+latitude +","+longitude + ")");
        //		}
        //		#endif
    }

    public void showAlert(string title, string text, string buttonText)
    {
        //		#if UNITY_IPHONE || UNITY_ANDROID
        //		if (Application.platform != RuntimePlatform.OSXEditor) {
        //			_showAlert (title, text, buttonText);
        //		} else {
        //			Debug.Log ("showAlertWithTitle(" + title + ")");
        //		}
        //		#endif
    }

    public void showLocalizedAlert(string title, string text, string buttonText)
    {
        AppManager.Instnace.messageBoxManager.ShowGenericPopup(title, text, buttonText);
        //		#if UNITY_IPHONE || UNITY_ANDROID
        //		if (Application.platform != RuntimePlatform.OSXEditor) {
        //			_showLocalizedAlert (title, text, buttonText);
        //		} else {
        //			Debug.Log ("showAlertWithTitle(" + title + ")");
        //		}
        //		#endif
    }
    public void setDownloadProgressToZero()
    {

        if (blankPanel == null)
            blankPanel = (BlankPanel)CanvasManager.Instnace.ReturnPanelManager(ePanelManager.MainMenuPanelManager).ReturnPanel(ePanels.Blank_Panel);
        if (scanpanel == null)
            scanpanel = (ScanPanel)CanvasManager.Instnace.ReturnPanelManager(ePanelManager.MainMenuPanelManager).ReturnPanel(ePanels.Scan_Panel);

        AppManager.Instnace.mDownloadProgress = 0;
        blankPanel.progressImage.fillAmount = 0;
        scanpanel.progressImage.fillAmount = 0;
    }

    public void setDownloadProgress(float progress)
    {
        AppManager.Instnace.mDownloadProgress = progress;

        if (blankPanel == null)
            blankPanel = (BlankPanel)CanvasManager.Instnace.ReturnPanelManager(ePanelManager.MainMenuPanelManager).ReturnPanel(ePanels.Blank_Panel);
        if (scanpanel == null)
            scanpanel = (ScanPanel)CanvasManager.Instnace.ReturnPanelManager(ePanelManager.MainMenuPanelManager).ReturnPanel(ePanels.Scan_Panel);

        if (blankPanel.gameObject.activeSelf)
        {

            blankPanel.progressImage.fillAmount = progress;

        }
        else if (scanpanel.gameObject.activeSelf)
        {

            scanpanel.progressImage.fillAmount = progress;
        }

        //		Debug.Log ("setDownloadProgress(" + progress + ")");
        //		#if UNITY_IPHONE || UNITY_ANDROID
        //		if (Application.platform != RuntimePlatform.OSXEditor) {
        //			_setDownloadProgress (progress);
        //		}
        //		#endif
    }

    // bed - commented this old version, as iOS can overload methods
    /*public static void showStatusOverlay (string title, string subtitle)
	{
		#if UNITY_IPHONE || UNITY_ANDROID
		if (Application.platform != RuntimePlatform.OSXEditor) {
			_showStatusOverlay (title, subtitle);
		} else {
			Debug.Log ("showStatusOverlay(" + title + ")");
		}
		#endif
	}*/

    public void showStatusOverlay(string title)
    {
        //Load the default localizedstring Data.
        //if (scanpanel == null) {
        //	scanpanel = (ScanPanel)CanvasManager.Instnace.ReturnPanelManager (ePanelManager.MainMenuPanelManager).ReturnPanel (ePanels.Scan_Panel);
        //	if(scanpanel != null)
        //		scanpanel.progressStatusText.text = LocalizeStringManager.ReturnLocalizeString (title);
        //} else {
        //	scanpanel.progressStatusText.text = LocalizeStringManager.ReturnLocalizeString (title);	
        //}

        //scanpanel.scanTextPanel.gameObject.SetActive (true);

        //		#if UNITY_IPHONE || UNITY_ANDROID
        //		if (Application.platform != RuntimePlatform.OSXEditor) {
        //			_showStatusOverlay (title);
        //		} else {
        //			Debug.Log ("showStatusOverlay(" + title + ")");
        //		}
        //		#endif
    }

    public void hideStatusoverlay()
    {
        if (scanpanel == null)
            scanpanel = (ScanPanel)CanvasManager.Instnace.ReturnPanelManager(ePanelManager.MainMenuPanelManager).ReturnPanel(ePanels.Scan_Panel);

        if (scanpanel != null)
            scanpanel.scanTextPanel.gameObject.SetActive(false);

        //		#if UNITY_IPHONE || UNITY_ANDROID
        //		if (Application.platform != RuntimePlatform.OSXEditor) {
        //			_hideStatusOverlay();
        //		} else {
        //			Debug.Log ("hideStatusoverlay()");
        //		}
        //		#endif
    }

    public bool IsBackButtonVisible { get; set;}

	public void setBackButtonVisibility(bool visibility) {

        IsBackButtonVisible = visibility;

		if(blankPanel == null)
			blankPanel = (BlankPanel)CanvasManager.Instnace.ReturnPanelManager (ePanelManager.MainMenuPanelManager).ReturnPanel (ePanels.Blank_Panel);
		if(scanpanel == null)
			scanpanel = (ScanPanel)CanvasManager.Instnace.ReturnPanelManager (ePanelManager.MainMenuPanelManager).ReturnPanel (ePanels.Scan_Panel);

		if (scanpanel.gameObject.activeSelf) {
			//			scanpanel.homeButton.gameObject.SetActive(visibility);
            SetLoveButtonImage(scanpanel.LovedButton);
			scanpanel.backButton.gameObject.SetActive(visibility);
            scanpanel.LovedButton.gameObject.SetActive(visibility);
           
		} else if (blankPanel.gameObject.activeSelf) {
			//			blankPanel.homeButton.gameObject.SetActive(visibility);
			blankPanel.backButton.gameObject.SetActive(visibility);
            blankPanel.LovedButton.gameObject.SetActive(visibility);
		}
	}

    void SetLoveButtonImage(Button a_button) {

        //Sprite originalSprite = AppManager.Instnace.spriteAtlas.GetSprite("BtmNav_Loved_White");
        //Sprite newSprite = AppManager.Instnace.spriteAtlas.GetSprite("Hub_Love_Active"); 

        Transform childTransform = a_button.transform.GetChild(0);
        if (WebService.Instnace.catDetailParameter.IsLove)
        {
            childTransform.GetComponent<SVGImage>().sprite = newSprite;
        }
        else
        {
            childTransform.GetComponent<SVGImage>().sprite = originalSprite;
        }
    }

//Jeetesh - This function is no lognger in Use now.
//	public void setCloseButtonVisibility (bool visibility)
//	{
//		#if UNITY_IPHONE || UNITY_ANDROID
//		if (Application.platform != RuntimePlatform.OSXEditor) {
//			_setCloseButtonVisibility(visibility);
//		} else {
//			Debug.Log ("setCloseButtonVisibility()");
//		}
//		#endif
//		if(blankPanel == null)
//			blankPanel = (BlankPanel)CanvasManager.Instnace.ReturnPanelManager (ePanelManager.MainMenuPanelManager).ReturnPanel (ePanels.Blank_Panel);
//		if(scanpanel == null)
//			scanpanel = (ScanPanel)CanvasManager.Instnace.ReturnPanelManager (ePanelManager.MainMenuPanelManager).ReturnPanel (ePanels.Scan_Panel);
//
//		if (scanpanel.gameObject.activeSelf) {
//			scanpanel.closeButton.gameObject.SetActive (visibility);
//
//		} else if (blankPanel.gameObject.activeSelf) {
//			blankPanel.closeButton.gameObject.SetActive (visibility);
//
//		}
//	}


//	public void setNavigationButtonsEnabled (bool enabled)
//	{
//		if(blankPanel == null)
//			blankPanel = (BlankPanel)CanvasManager.Instnace.ReturnPanelManager (ePanelManager.MainMenuPanelManager).ReturnPanel (ePanels.Blank_Panel);
//		if(scanpanel == null)
//			scanpanel = (ScanPanel)CanvasManager.Instnace.ReturnPanelManager (ePanelManager.MainMenuPanelManager).ReturnPanel (ePanels.Scan_Panel);
//
//		if (scanpanel.gameObject.activeSelf) {
////			scanpanel.homeButton.gameObject.SetActive(enabled);
//			scanpanel.backButton.gameObject.SetActive(enabled);
//		} else if (blankPanel.gameObject.activeSelf) {
////			blankPanel.homeButton.gameObject.SetActive(enabled);
//			blankPanel.backButton.gameObject.SetActive(enabled);
//		}
////		#if UNITY_IPHONE || UNITY_ANDROID
////		if (Application.platform != RuntimePlatform.OSXEditor) {
////			_setNavigationButtonsEnabled(enabled);
////		} else {
////			Debug.Log ("setNavigationButtonsEnabled()");
////		}
////		#endif
//	}

//	public void setNavigationButtonsVisibility (bool visibility)
//	{
//		if(blankPanel == null)
//			blankPanel = (BlankPanel)CanvasManager.Instnace.ReturnPanelManager (ePanelManager.MainMenuPanelManager).ReturnPanel (ePanels.Blank_Panel);
//		if(scanpanel == null)
//			scanpanel = (ScanPanel)CanvasManager.Instnace.ReturnPanelManager (ePanelManager.MainMenuPanelManager).ReturnPanel (ePanels.Scan_Panel);
//		
//		if (scanpanel.gameObject.activeSelf) {
////			scanpanel.homeButton.gameObject.SetActive(visibility);
//			scanpanel.backButton.gameObject.SetActive(visibility);
//		} else if (blankPanel.gameObject.activeSelf) {
////			blankPanel.homeButton.gameObject.SetActive(visibility);
//			blankPanel.backButton.gameObject.SetActive(visibility);
//		}
//
////		#if UNITY_IPHONE || UNITY_ANDROID
////		if (Application.platform != RuntimePlatform.OSXEditor) {
////			_setNavigationButtonsVisibility(visibility);
////		} else {
////			Debug.Log ("setNavigationButtonsVisibility()");
////		}
////		#endif
//	}

	public void showHUD (string title, string subtitle, string imageName, float duration)
	{
//		#if UNITY_IPHONE || UNITY_ANDROID
//		if (Application.platform != RuntimePlatform.OSXEditor) {
//			_showHUD(title, subtitle, imageName, duration);
//		} else {
//			Debug.Log ("setCloseButtonVisibility()");
//		}
//		#endif
	}
	
	
	public void showAppVersionError (int gotVersion, int minVersion, int maxVersion)
	{
//		#if UNITY_IPHONE || UNITY_ANDROID
//		if (Application.platform != RuntimePlatform.OSXEditor) {
//			_showAppVersionError(gotVersion, minVersion, maxVersion);
//		} else {
//		}
//		#endif
	}
	
	public void trackingEvent(string eventType,
		ulong triggerId,
		int areaIndex,
		int frameIndex,
		int widgetIndex,
		string resourceUrl, 
		string resourceId,
		string resourceDetailId,
		string targetUrl,
		string socialNetwork,
		float videoPlayTime)
	{
//		#if UNITY_IPHONE || UNITY_ANDROID
//		if (Application.platform != RuntimePlatform.OSXEditor) {
//			_trackingEvent(eventType, triggerId, areaIndex, frameIndex, widgetIndex, resourceUrl, resourceId, resourceDetailId, targetUrl, socialNetwork, videoPlayTime);
//		} else {
//			Debug.Log(string.Format("TRACKINGEVENT eventType: {0}\n trigger: {1}\nareaIndex: {2}\nframeIndex: {3}\nwidgetIndex: {4}\nresourceUrl:{5}\nresourceId: {6}\ndetailResourceId: {7}\ntargetUrl: {8}\n socialNetwork: {9}\nvideoPlayTime: {10}", eventType, triggerId, areaIndex, frameIndex, widgetIndex, resourceUrl, resourceId, resourceDetailId, targetUrl, socialNetwork, videoPlayTime));
//		}
//		#endif
	}
	
	public void recordHistory(string triggerId, string targetId, string jsonString)
	{
//#if UNITY_IPHONE || UNITY_ANDROID
//		if (Application.platform != RuntimePlatform.OSXEditor) 
//		{
//			_recordHistory(triggerId, targetId, jsonString);
//		}
//		else 
//		{
//			Debug.Log ("recordHistory()");
//		}
//#endif
	}

	public void DisableButtons (){

        WebService.Instnace.historyData.history_Description = "";
        WebService.Instnace.historyData.history_icon = "";
        WebService.Instnace.historyData.history_Image = "";
        WebService.Instnace.historyData.history_Title = "";
        WebService.Instnace.historyData.history_icon = "";

		if(scanpanel == null)
			scanpanel = (ScanPanel)CanvasManager.Instnace.ReturnPanelManager (ePanelManager.MainMenuPanelManager).ReturnPanel (ePanels.Scan_Panel);
		if(blankPanel == null)
			blankPanel = (BlankPanel)CanvasManager.Instnace.ReturnPanelManager (ePanelManager.MainMenuPanelManager).ReturnPanel (ePanels.Blank_Panel);
		
        if (AppManager.Instnace.isVuforiaOn) {
			scanpanel.LovedButton.gameObject.SetActive (false);
			scanpanel.backButton.gameObject.SetActive (false);
			scanpanel.history_Icon.gameObject.SetActive (false);
			scanpanel.history_Title.gameObject.SetActive (false);
		} else {
			blankPanel.LovedButton.gameObject.SetActive (false);
			blankPanel.backButton.gameObject.SetActive (false);
			blankPanel.history_Icon.gameObject.SetActive (false);
			blankPanel.history_Title.gameObject.SetActive (false);
		}
	}
	public void EnableButtons (){

		if(scanpanel == null)
			scanpanel = (ScanPanel)CanvasManager.Instnace.ReturnPanelManager (ePanelManager.MainMenuPanelManager).ReturnPanel (ePanels.Scan_Panel);
		if(blankPanel == null)
			blankPanel = (BlankPanel)CanvasManager.Instnace.ReturnPanelManager (ePanelManager.MainMenuPanelManager).ReturnPanel (ePanels.Blank_Panel);
        if (AppManager.Instnace.isVuforiaOn) {
			scanpanel.LovedButton.gameObject.SetActive (true);
			scanpanel.backButton.gameObject.SetActive (true);
			//scanpanel.history_Icon.gameObject.SetActive(false);
		} else {
			blankPanel.LovedButton.gameObject.SetActive (true);
			blankPanel.backButton.gameObject.SetActive (true);
			//blankPanel.history_Icon.gameObject.SetActive (true);
		}
	}

	public void CreateOldPreloader ()
	{
		if (preloader == null) {
			preloader = (GameObject)GameObject.Instantiate (preloadPrefab);
			//		preloader.transform.localScale = new Vector3 (30.0f, 30.0f, 30.0f);

			this.preloader.transform.parent = this.transform;
			preloader.transform.localPosition = Vector3.zero;
			Vector3 newPos = this.transform.position + (this.transform.forward * 1200);
			Quaternion rotation = this.transform.rotation;
			this.preloader.transform.localScale = new Vector3 (20.0f, 20.0f, 20.0f);
			rotation *= Quaternion.Euler (-90, 0, 0);

			this.preloader.transform.position = newPos;
			this.preloader.transform.rotation = rotation;
			preloader.GetComponent<Spinner> ().Progress = 1.0f;
		}
	}

	public void RemoveOldPreloader() {
		if (preloader != null) {
			Destroy (preloader);
			preloader = null;
		}
	}

    #region Campaign Report
    //**************************************************************************
    //                      Send Campaign Report
    //**************************************************************************

    public void CampaignEventsData(string EventType, string EventDescription){

        ViewaCampaignEvents viewaCampaignEvents = new ViewaCampaignEvents();
        viewaCampaignEvents.EventType = EventType;
        viewaCampaignEvents.EventDescription = EventDescription;
        viewaCampaignEvents.DateCreated = DateTime.Now;
        _viewaCampaignEventsList.Add(viewaCampaignEvents);
        SendCampaignReport();
    }

    string Screen;
    string TrackingId;
    string CloudId;
    int TriggerId;
    string ImageUrl;
    string UserId;
    string DeviceId;
    string DeviceType;
    string DeviceModel;
    string Description;
    string IconUrl;
    DateTime startTime;
    DateTime DateCreated;
    long TotalSessionTime;

    public void CampaignTrackingData( string a_trackingId, string a_cloudId, int a_triggerId){

        string temp = AppManager.Instnace.triggerHistoryMode == eTriggerHistoryMode.TriggerHistoryModeChannel ? "HUB" : "LOVED";

        Screen = AppManager.Instnace.isVuforiaOn ? "SCAN" : temp;
        TrackingId = a_trackingId;
        CloudId = a_cloudId;
        TriggerId = a_triggerId;
        ImageUrl = WebService.Instnace.historyData.history_Image;
        IconUrl = WebService.Instnace.historyData.history_icon;
        Description = WebService.Instnace.historyData.history_Description;
        UserId = AppManager.Instnace.userEmail;
        DeviceId = SystemInfo.deviceUniqueIdentifier;
        DeviceType = SystemInfo.deviceName;
        DeviceModel = SystemInfo.deviceModel;
        //startTime = DateTime.Now;
        DateCreated = DateTime.Now;
    }

    public void SendCampaignReport()
    {
        TimeSpan timeSpan = DateTime.Now - startTime;
        int totalSecondsInHours = timeSpan.Hours * 60 * 60;
        int totalSecondsInMinutes = timeSpan.Minutes * 60;


        trackingRoot trackingRoot = new trackingRoot();

        ViewaCampaignTracker viewaCampaignTracker = new ViewaCampaignTracker();
       
        trackingRoot.trackingData.ViewaCampaignTracker = viewaCampaignTracker;

        if (_viewaCampaignEventsList.Count > 0)
        {
            viewaCampaignTracker.TrackingId = TrackingId;  //--
            viewaCampaignTracker.TriggerId = TriggerId;  //--
            viewaCampaignTracker.UserId = UserId;  //--
            trackingRoot.trackingData.ListViewaCampaignEvents = _viewaCampaignEventsList.ToArray();
        } else {
            TotalSessionTime = totalSecondsInHours + totalSecondsInMinutes + timeSpan.Seconds;
            viewaCampaignTracker.TrackingId = TrackingId;  //--
            viewaCampaignTracker.TriggerId = TriggerId;  //--
            viewaCampaignTracker.UserId = UserId;  //--
            viewaCampaignTracker.Screen = Screen;
            viewaCampaignTracker.CloudId = CloudId;
            viewaCampaignTracker.ImageUrl = ImageUrl;
            viewaCampaignTracker.DeviceId = DeviceId;
            viewaCampaignTracker.DeviceType = DeviceType;
            viewaCampaignTracker.DeviceModel = DeviceModel;
            viewaCampaignTracker.Description = Description;
            viewaCampaignTracker.IconUrl = IconUrl;
            viewaCampaignTracker.TotalSessionTime = TotalSessionTime;
            viewaCampaignTracker.DateCreated = DateCreated;

            //location information added
            viewaCampaignTracker.Country = AppManager.Instnace.geoLocation.country;
            viewaCampaignTracker.City = AppManager.Instnace.geoLocation.city;
            viewaCampaignTracker.RegionName = AppManager.Instnace.geoLocation.regionName;
            viewaCampaignTracker.CountryCode = AppManager.Instnace.geoLocation.countryCode;
            viewaCampaignTracker.Zip = AppManager.Instnace.geoLocation.zip;
            viewaCampaignTracker.lat = AppManager.Instnace.geoLocation.lat;
            viewaCampaignTracker.lon = AppManager.Instnace.geoLocation.lon;

            trackingRoot.trackingData.ListViewaCampaignEvents = _viewaCampaignEventsList.ToArray();
        }

        string jsonString = trackingRoot.ReturnJsonString();
        Debug.Log("Campaign Tracking json :" + jsonString);
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonString);
        _viewaCampaignEventsList.Clear();
        WebService.Instnace.Post(AppManager.Instnace.baseURL + "/cloud/ViewaTrackingDataAction.aspx/SubmitTracking", bodyRaw, "", null);
    }

    public void InitializeStartTime(){

        startTime = DateTime.Now;
    }

    //void GetCampaignResponse(UnityWebRequest response)
    //{
    //    Debug.Log("PopUpSurveyCallBack :" + response.downloadHandler.text);
    //    var res = JSON.Parse(response.downloadHandler.text);
    //    _viewaCampaignEventsList.Clear();
    //}
    #endregion
}
