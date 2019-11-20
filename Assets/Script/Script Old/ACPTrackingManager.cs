//#define USE_STAGING

using UnityEngine;

using ACP;
using System;
using System.Collections;
using System.IO;
using System.Text;
using Vuforia;
using UnityEngine.UI;
using OTPL.UI;
using SimpleJSON;

using UnityEngine.Networking;
using SQLite4Unity3d;
using System.Linq;
using OTPL.modal;


/// <summary>
/// This is the main object that runs the whole thing.
/// It is attached to the camera and handles loading/unloading both the issue JSON data (IssueData), as well as the QualcommAR Image Marker data (DataSet class)
/// </summary>
/// 
public class ACPTrackingManager : MonoBehaviour, IObjectRecoEventHandler
{
	#region propeties and variables
	// Use this for initialization
	public PackageData package;
	private DataSet dataSet;
	public GameObject preloadPrefab;
	public Material materialUITransparent;
	public Material materialUIShiny;
	public Material materialUIMask;
	public Material materialUIAlphaSBS;
    public Material materialAlphaBlended;
	public GameObject quadPrefab;
	public GameObject videoLoadingTextPrefab;

	private string datasetPath;
	private DateTime datasetModifiedDate;
	public bool testMode = false;
	public bool timingEvents = true;
	private bool scanningEnabledByApp = false;
	
    private CloudRecoBehaviour mCloudRecoBehaviour; //CloudRecoBehaviour reference to avoid lookups
    public ObjectTracker mImageTracker; //ImageTracker reference to avoid lookups
	public CameraDevice.FocusMode mFocusMode;
	public static bool ARVideoEnabled = true;
	
	//public GUIText requestingText;
//	public GUIText statsText;
	
	public bool nativeCloseButton = true;
//	public GUITexture closeButton; //for use inside unity only (or web)
//	public GUITexture backButton; //for use inside unity only (or web)
//	public GUITexture homeButton; //for use inside unity only (or web)

	public float scanStartTime;
	public float scanEndTime;
	public float scanTotalTime = -1f;
	public float requestStartTime;
	public float requestEndTime;
	public float requestTotalTime = -1f;
	
	private bool isRequesting;
	private bool isShowingHoldStill;
	//public string baseUrl = "http://content.viewa.com.au";
	public string language = "en";
	public TrackableData previewTrackableData;
	public int navBarHeight = 0;
	/// <summary>
	/// The scan-line rendered in overlay when Cloud Reco is in scanning mode.
	/// </summary>
	public ScanLine m_ScanLine;
    public TargetFinder mtargetFinder;
   
	//hovering logic
	private WidgetBehavior currentHoverWidget; //if mouse/touch is hovering on a widget, this is the widget. Otherwise null
	private bool isHovering; //true if we're started hovering
	private ScanPanel _scanPanel;
	private bool vuforiaFlag;

	#endregion

	#region Unity functions

	void Awake ()
	{
        //VuforiaRenderer.Instance.SetLegacyRenderingEnabledCondition(() => true);
	}
		
	void Start ()
	{
		m_ScanLine = FindObjectOfType<ScanLine>();
//		if (Application.isEditor) {
//			Invoke ("delayedTestTriggerLoad", 5);
//		}	

        if(AppManager.Instnace.isLive) {
            //Jeetesh - Access key and Secret key of Live.
            SetCloudCredentials ("0b3491e59b2e3d28909001b1178066d3cf2bf5cc|e76eacac21f0dc36ba3aab37f2e8a7196cebe1bf");
        } else {
            //Jeetesh - Access key and Secret key of Staging.
            SetCloudCredentials("00229e34706c137bf139dfab98c6337ab41dc12c|8005935be76221d2c747c8f0a8ad26b86da9943a");
        }

        //SetCloudCredentials("0b3491e59b2e3d28909001b1178066d3cf2bf5cc|e76eacac21f0dc36ba3aab37f2e8a7196cebe1bf");

		//Disable the scanning initially.
		DisableScanning ();			

		SetInitialFocusSetting();

#if UNITY_EDITOR
        navBarHeight = DisplayMetricsUtil.DpToPixel(50); //87;  //DisplayMetricsUtil.DpToPixel(44); //120
#elif UNITY_ANDROID
        navBarHeight = DisplayMetricsUtil.DpToPixel(50);  //60
#elif UNITY_IOS
		navBarHeight = DisplayMetricsUtil.DpToPixel(50); //50
#endif

        if (Application.isEditor) {
            Invoke ("delayedTestTriggerLoad", 5.0f);
        }  

        Camera.main.transparencySortMode = TransparencySortMode.Orthographic;
	}


	private void delayedTestTriggerLoad()
	{
#if USE_STAGING
		//CreateACPTrackable(new GameObject(), "53441", "8111e01fe52544fa9990191f2aead7eb", null); //AR Gallery1 - staging
		//CreateACPTrackable(new GameObject(), "53439", "f0a40464189b41dc95afdc3bed3b5e7a", null); //AR Gallery2 - staging
		//CreateACPTrackable(new GameObject(), "55143", "394db771283c4d26bb47888a437fcb58", null); //AR Gallery3 - staging
		//CreateACPTrackable(new GameObject(), "54992", "e3e6809d830c4a23bf2075406c778a6d", null); //asset bundle - staging
		//CreateACPTrackable(new GameObject(), "53278", "27e7baa207b0480ea74c4b7c7bbb585d", null); //vinceTest page1 - staging
		//CreateACPTrackable(new GameObject(), "53359", "69697400c8f24ed2b49343099b336f96", null); //vinceTest page2 - staging
		//CreateACPTrackable(new GameObject(), "55466", "c23f572eb00948b389ab27df4bb23e99", null); //new Litho Layout - staging - weekly review 2849337

#else


		//CreateACPTrackable(new GameObject(), "157246", "0c301f4919aa4e76a95dc2547da14281", null); //z-index issue

		//Live 3DAssetBundles
		//CreateACPTrackable(new GameObject(), "150532", "838c576c75ec47d9bc7be5758adb23a8", null); // 3D asset bundle - diorshow 3d eyelash brush 
		//CreateACPTrackable(new GameObject(), "140300", "d77bb107a16845b7ae0bcfb98a6bef50", null); // 3D asset bundle - mocom tests - VW mini LIVE
		//CreateACPTrackable(new GameObject(), "162607", "4a0217d22fc04adbbeb18f57c7ab5598", null); // 3D asset bundle example2 - warrior dude- LIVE
		//CreateACPTrackable(new GameObject(), "164113", "843c0293a9544c37b0a63ba4d729cb08", null); // 3D asset bundle cameraEmersion - LIVE
		//CreateACPTrackable(new GameObject(), "171417", "3f9355fff6824d3f830a0e34c2ab0d41", null); // 3D asset bundle multiple animations

		//live HUBS
//		CreateACPTrackable(new GameObject(), "140917", "818e3eb4986e48e48241b6b15ebab529", null); //multi area trigger for navigation testing - LIVE
		//CreateACPTrackable(new GameObject(), "138205", "3a048bea49714011a223451fd29e4db5", null); // crazy parting explosion - LIVE
		//CreateACPTrackable(new GameObject(), "156512", "e7228a4f1acb412397b7c5e78b6fd660", null); // Fresh Herbs size test - LIVE - http://content.viewa.com/australia/ad/gourmet-garden/1409/ad_gourmetgarden_herb_1409_01.aspx
		//CreateACPTrackable(new GameObject(), "155524", "09c75759e5c8437ba976e256e38d71c3", null); // Body Shop size test - LIVE - http://content.viewa.com/australia/ad/bodyshop/1408/ad_bodyshop_bachelor_14xx_1.aspx
		//CreateACPTrackable(new GameObject(), "149694", "d5517d4d7a044c3fbb331a3cbc51ecbd", null); // 20th century new girl hub - LIVE
		//CreateACPTrackable(new GameObject(), "137538", "c7a4fe4f47c34e18877137e59e2cd270", null); // AirNZ HUB - LIVE
		//CreateACPTrackable(new GameObject(), "54194", "9520eaaa73494f0e81a7fb0313a0922a", null); // Anchor Bay - width test - LIVE
		//CreateACPTrackable(new GameObject(), "169593", "519f16fe75504d428b111c0cdb688345", null); // new default snapto layout
		//CreateACPTrackable(new GameObject(), "174107", "bd1e77ab79974dc3ac441d988b0ed5cf", null); // VR-619 - car, lighting
		//CreateACPTrackable(new GameObject(), "174519", "39ee165b598c43e2ba13512f573a2df2", null); //AR Video
        //Jeetesh - testing;
        //CreateACPTrackable(new GameObject(), "295713", "03490cdc6b154b6ba7c5ce7050d3c725", null); //Belle Properties
        //CreateACPTrackable(new GameObject(), "296997", "35efe3c0055f46b0822b65d61f68c330", null); //AfPA vote to support
        //CreateACPTrackable(new GameObject(), "296990", "761e1a644045438fab439c21c3d8e01b", null); //Pure Baby
        //CreateACPTrackable(new GameObject(), "296874", "c391c636cabf4480a2760ba9a26bb06d", null); //Ray White Tas
        //CreateACPTrackable(new GameObject(), "206005", "c6231b3add654223afca14cb1808c937", null); //Beauty old trigger
        //CreateACPTrackable(new GameObject(), "206007", "eb7114f6d9844655ba95c4b0c7360e62", null); //Beauty old trigger
        //CreateACPTrackable(new GameObject(), "295713", "03490cdc6b154b6ba7c5ce7050d3c725", null); //End-image trigger
        //CreateACPTrackable(new GameObject(), "296907", "9ac72a5b3d6949f0baef4600a7b30791", null); //Beauty old trigger
        //CreateACPTrackable(new GameObject(), "296742", "afacd91a13444a7fb4f53d7787fdbe67", null); //Beauty old trigger
        //CreateACPTrackable(new GameObject(), "296742", "afacd91a13444a7fb4f53d7787fdbe67", null); //Asset Bundle trigger
        //CreateACPTrackable(new GameObject(), "297071", "ae3be075d5ea4e4588792258e5312662", null); //Transparent Video Trigger
        #endif
	}
	#endregion

	#region camera setting

    public void SetInitialFocusSetting()
	{
        //StartCoroutine(SetFocusModeToTriggerAuto());
        SetFocusModeToContinuousAuto();
	}
	
	private IEnumerator SetFocusModeToTriggerAuto()
    {
        if (CameraDevice.Instance.SetFocusMode(CameraDevice.FocusMode.FOCUS_MODE_TRIGGERAUTO)) {
              mFocusMode = CameraDevice.FocusMode.FOCUS_MODE_TRIGGERAUTO;
        }
        
        //Debug.Log("Focus Mode Changed To " + mFocusMode);
        
        yield return new WaitForSeconds(1.0f);
        
        SetFocusModeToContinuousAuto();
    }
    
    private void SetFocusModeToContinuousAuto()
    {
        if (!CameraDevice.Instance.SetFocusMode(CameraDevice.FocusMode.FOCUS_MODE_CONTINUOUSAUTO)) {
            mFocusMode = CameraDevice.FocusMode.FOCUS_MODE_CONTINUOUSAUTO;
        }
        
        //Debug.Log("Focus Mode Changed To " + mFocusMode);
    }
    //Jeetesh
#region Commented Text 4
    public IEnumerator ReloadTest()
	{
		yield return new WaitForSeconds(2);
		LoadIssueData (Application.dataPath + "/StreamingAssets/" + "issue");
	}
#endregion


#endregion


    // Update is called once per frame
    void Update ()
	{
		//code for Vuforia On and Off
		if (!AppManager.Instnace.isVuforiaOn && !vuforiaFlag) {
            
            if (VuforiaManager.Instance.Initialized) {
				vuforiaFlag = true;

				DisableScanning();
			}
				
		} else if(AppManager.Instnace.isVuforiaOn && vuforiaFlag) {
			vuforiaFlag = false;

            if (VuforiaManager.Instance.Initialized) {
				
                //Enable scanning
				EnableScanning();
			}

            if (AppManager.Instnace.isDynamicDataLoaded){
                m_ScanLine.ShowScanLine(false);
            } else {
                m_ScanLine.ShowScanLine(true);
            }

            if(AppManager.Instnace.pushNotificationController.isPushNotificationShown){
              
                m_ScanLine.ShowScanLine(false);
            }
            Debug.Log("HasInitialized ScanLine: true");
		}

		if (AppManager.Instnace.isVuforiaOn || AppManager.Instnace.isDynamicContentFromHub) {
            
            if (!AppManager.Instnace.isPopUpSurvay)
            {
                EventsUpdate();
            }
		}
	}
	
	public void DisableScanning ()
	{
        //Remove button to show flash light

        if(mImageTracker != null){
            mImageTracker.Stop();
        }

		scanningEnabledByApp = false;
        m_ScanLine.ShowScanLine(false);

		if (mCloudRecoBehaviour) {

            Debug.Log("------------->  DisableScanning -  Inside mCloudRecoBehaviour block");
			mCloudRecoBehaviour.CloudRecoEnabled = false;
            mCloudRecoBehaviour.enabled = false;
			ACPUnityPlugin.Instnace.hideStatusoverlay ();
			m_ScanLine.ShowScanLine (false);
		}

        DisableVuforia();
	}
	
	public void EnableScanning ()
	{
        //Add button to show flash light
        //CameraDevice.Instance.SetFlashTorchMode(true);

        if(mImageTracker != null) {
            mImageTracker.Start();
        }

        EnableVuforia();

		scanningEnabledByApp = true;

        //SetFocusModeToTriggerAuto();

		if (mCloudRecoBehaviour) {
			//Debug.Log ("Enabling Scanning");
            mCloudRecoBehaviour.enabled = true;
			mCloudRecoBehaviour.CloudRecoEnabled = true;
			ShowStatusOverlay ("ScanOverlayTextDownloading");
		}
	}

	//public void SetBaseURL (string url)
	//{
	//	//Debug.Log ("Setting baseURL to: " + url);
	//	this.baseUrl = url;
	//}
	
	public void EnableTestMode ()
	{
		testMode = true;
		//timingEvents = true;
	}
	
	public void DisableTestMode ()
	{
		testMode = false;
		//timingEvents = false;
	}

	//public void SetNavbarHeight(string stringHeight)
	//{
	//	this.navBarHeight = int.Parse(stringHeight);
	//	//Debug.Log("Explicit NavBarHeight is "+navBarHeight);
	//}

	public void SetLanguage(string language)
	{
		this.language = language;
	}
	
	public void SetCloudCredentials (string credentials)
	{
		//Debug.Log ("Setting credentials to: " + credentials);
		string[] split = credentials.Split ('|');
		
		mCloudRecoBehaviour = GetComponent<CloudRecoBehaviour> ();
		if (mCloudRecoBehaviour == null) 
		{ 
			mCloudRecoBehaviour = this.gameObject.AddComponent<CloudRecoBehaviour> ();
			mCloudRecoBehaviour.RegisterEventHandler (this);
			//mCloudRecoBehaviour.FeaturePointColor = new UnityEngine.Color(1.0f, 1.0f, 1.0f, 1.0f);
			mCloudRecoBehaviour.AccessKey = split [0];
			mCloudRecoBehaviour.SecretKey = split [1];
		} 
		else if ( mCloudRecoBehaviour.AccessKey != split[0] || mCloudRecoBehaviour.SecretKey != split[1])
		{
			mCloudRecoBehaviour.AccessKey = split [0];
			mCloudRecoBehaviour.SecretKey = split [1];
			//mCloudRecoBehaviour.OnInitialized();
		}
	}

	public void SetTrackingTriggerEndSent ()
	{ //native client MAY send the triggerEnd event itself, if so it will call this so we don't send it again below.
		//Debug.Log ("ACPTrackingManager - SetTrackingTriggerEndSent");
		GameObject[] trackables = GameObject.FindGameObjectsWithTag ("Trackable");
		foreach (GameObject trackable in trackables) {
			trackable.SendMessage ("SetTrackingTriggerEndSent");
		}
	}

	public void SendTrackingTriggerEnd ()
	{
		//Debug.Log ("ACPTrackingManager - sendTrackingTriggerEnd");
		GameObject[] trackables = GameObject.FindGameObjectsWithTag ("Trackable");
		foreach (GameObject trackable in trackables) {
			trackable.SendMessage ("SendTrackingTriggerEnd");
		}
	}

	#region OnButtonTap
	//Jeetesh - This call from ACPTrackableBehaviour due to some reason. - Thats why its still present.
	//otherwise the close button is no longer there it is replaced with back button.
	public void OnCloseButtonTapped ()
	{
		DestroyTrackableData ();
		OnCloseAndNavigate ();
	}

    public void TrackableDestroyedInstantly() {

        GameObject[] trackables = GameObject.FindGameObjectsWithTag("Trackable");
        foreach (GameObject trackable in trackables)
        {
            trackable.SendMessage("CloseButtonTapped");
        }
    }

	//This also called and closed from bottom bar.
	public void OnCloseAndNavigate(){

        Debug.Log("Inside OnCloseAndNavigate");
        //If coming from Bottom Bar delete all the trackables 
        //Need to see for the video play as well
        if (AppManager.Instnace.webViewPage.webViewIsLoaded)
            AppManager.Instnace.webViewPage.DestroyWebView();

        Debug.Log("GoToPanel OnCloseAndNavigate");
		GoToPanel ();
	}
	public void DestroyTrackableData() {

		Debug.Log ("ACPTrackingManager - DestroyTrackableData");
		GameObject[] trackables = GameObject.FindGameObjectsWithTag ("Trackable");
        if(trackables.Length > 0) {

            ACPUnityPlugin.Instnace.SendCampaignReport();
            AppManager.Instnace.isDynamicDataLoaded = false;
            AppManager.Instnace.isDynamicContentFromHub = false;
            WebService.Instnace.catDetailParameter.IsLove = false;
            WebService.Instnace.catDetailParameter.CategoryID = "";
            WebService.Instnace.catDetailParameter.CloudTriggerId = "";
            WebService.Instnace.catDetailParameter.HubTrigger = 0;

            foreach (GameObject trackable in trackables)
            {
                trackable.SendMessage("CloseButtonTapped");
            }
        }
	}

	public void GoToPanel(){
		Debug.Log ("GoToPanel");

		if (!AppManager.Instnace.isVuforiaOn) {
            Debug.Log("GoToPanel AppManager.Instnace.isVuforiaOn");
			BlankPanel blankPanel = (BlankPanel)CanvasManager.Instnace.ReturnPanelManager (ePanelManager.MainMenuPanelManager).ReturnPanel (ePanels.Blank_Panel);
			blankPanel.progressImage.fillAmount = 0;
			AppManager.Instnace.mDownloadProgress = 0;
			AppManager.Instnace.isDynamicContentFromHub = false;

			if (AppManager.Instnace.triggerHistoryMode == eTriggerHistoryMode.TriggerHistoryModeChannel
			    || AppManager.Instnace.triggerHistoryMode == eTriggerHistoryMode.TriggerHistoryModeRecent
                || AppManager.Instnace.triggerHistoryMode == eTriggerHistoryMode.TriggerHistoryModeLoved) {
				
                if(blankPanel.myManager.panelStack.Count > 0)
                    CanvasManager.Instnace.ReturnPanelManager (ePanelManager.MainMenuPanelManager).BackToPanel (ePanels.HubDetail_Panel);
				else 
                    CanvasManager.Instnace.ReturnPanelManager (ePanelManager.MainMenuPanelManager).BackToPanel (ePanels.HubDetail_Panel);
			} 
		} else {
            Debug.Log("GoToPanel scanPanel");
            ScanPanel scanPanel = (ScanPanel)CanvasManager.Instnace.ReturnPanelManager (ePanelManager.MainMenuPanelManager).ReturnPanel (ePanels.Scan_Panel);
			scanPanel.progressImage.fillAmount = 0;
            scanPanel.LovedButton.gameObject.SetActive(false);
			AppManager.Instnace.mDownloadProgress = 0;
			scanPanel.scanTextPanel.gameObject.SetActive (true);
			CanvasManager.Instnace.ReturnPanelManager (ePanelManager.MainMenuPanelManager).BackToPanel (ePanels.Scan_Panel);
            EnableScanning();
		}
	}

	public void OnBackButtonTapped ()
	{
		GameObject[] trackables = GameObject.FindGameObjectsWithTag ("Trackable");
        Debug.Log("Inside OnBackButtonTapped :" + trackables.Length);
        if (trackables.Length > 0)
        {
            foreach (GameObject trackable in trackables)
            {
                Debug.Log("trackables.Length > 0");
                trackable.SendMessage("BackButtonTapped");
            }
        } else {
            Debug.Log("OnClose------------------------>");
            DestroyTrackableData();
            OnCloseAndNavigate();
        }
	}

//	public void OnHomeButtonTapped ()
//	{
//		GameObject[] trackables = GameObject.FindGameObjectsWithTag ("Trackable");
//		foreach (GameObject trackable in trackables) {
//			trackable.SendMessage ("HomeButtonTapped");
//		}
//	}

	#endregion
	private void UpdateStatsText ()
	{
		StringBuilder sb = new StringBuilder ();
		if (this.scanTotalTime >= 0) {
			sb.AppendFormat ("Time before request: {0}\n", this.scanTotalTime);
		}
		if (this.requestTotalTime >= 0) {
			sb.AppendFormat ("Request time: {0}\n", this.requestTotalTime);
		}
		
//		this.statsText.text = sb.ToString ();
	}
	
	void PrintChildObjects (GameObject go, string indent)
	{
		//Debug.Log (indent + go.name + ": " + go.transform.position.ToString ());
		for (int i = 0; i < go.transform.childCount; i++) {
			
			Transform t = go.transform.GetChild (i);
			PrintChildObjects (t.gameObject, indent + "    ");
		}
	}
	
	public void PrintDebugInfo ()
	{
		Debug.Log ("Camera is at: " + Camera.main.transform.position);
		
		object[] objs = GameObject.FindObjectsOfType (typeof(GameObject));
		foreach (object o in objs) {
			PrintChildObjects ((GameObject)o, "");
		}
		
 // 	GameObject[] trackables = GameObject.FindGameObjectsWithTag ("Trackable");
 // 	
 // 	foreach (GameObject trackable in trackables) {
 // 		Debug.Log ("Trackable " + trackable.name + " is located at: " + trackable.transform.position);
 // 	}
	}

#region CommentedText 2

    public void LoadPreview (string jsonString)
	{
		Debug.Log ("LoadPreview");
		StartCoroutine (LoadPreviewImpl (jsonString));
	}
	
	public IEnumerator LoadPreviewImpl (string jsonString)
	{
		JSONObject jsonObject = new JSONObject (jsonString);
		
		// Load the trackables data
		this.previewTrackableData = TrackableData.Create (jsonObject);
		Debug.Log ("-LoadPreviewImpl-");
		
		DetatchContentFromTrackables ();
		AttachContentToTrackables (null);
		
		yield return true;
	}
#endregion

    public void EnableCache ()
	{
		//Debug.Log ("Cache is enabled!");
		DownloadCache.Instance.CacheEnabled = true;
	}
	
	public void DisableCache ()
	{
	//	Debug.Log ("Cache is disabled!");
		DownloadCache.Instance.CacheEnabled = false;
	}
	
	public void EnableLogging ()
	{
		//ACPUnityPlugin.NativeLogEnabled = true;
	}
	
	public void DisableLogging ()
	{
		//ACPUnityPlugin.NativeLogEnabled = false;
	}
	
	public void EnableARVideo ()
	{
		ACPTrackingManager.ARVideoEnabled = true;
	}
	
	public void DisableARVideo ()
	{
		ACPTrackingManager.ARVideoEnabled = false;
	}

#region Commented Text 3
    //jeetesh
    public void LoadPreviewUrl (string url)
	{
		Debug.Log ("LoadPreviewUrl");
		StartCoroutine (LoadIssueDataPreviewImpl (url));
	}
	
	public void LoadIssueData (string issue_id)
	{
		Debug.Log ("LoadIssueData");
		StartCoroutine (LoadIssueDataImpl (issue_id, false, false, VuforiaUnity.StorageType.STORAGE_ABSOLUTE));
	}
	
	public void LoadIssueJson (string issue_id)
	{
		Debug.Log ("LoadIssueJson");
		StartCoroutine (LoadIssueDataImpl (issue_id, true, false, VuforiaUnity.StorageType.STORAGE_ABSOLUTE));
	}
#endregion
    public bool CheckAppVersion (int minAppVersion)
	{
		//see https://visualjazz.jira.com/wiki/display/VR/Trigger+Version+Schemas
		const int CURRENT_TRIGGER_VERSION = 9;// this maximum version should be bumped up 
		//Debug.Log ("App version is "+CURRENT_TRIGGER_VERSION + " trigger version is "+minAppVersion);

		if (minAppVersion >= 1 && minAppVersion <= CURRENT_TRIGGER_VERSION) { 
			return true;
		} else {
		//	Debug.Log ("App version incompatible");
			ACPUnityPlugin.Instnace.showAppVersionError (minAppVersion, 1, CURRENT_TRIGGER_VERSION);
			return false;
		}	
	}
	
	// for android application to exit
	public void CloseApplication ()
	{
		
		// This doesn't exist any more? Hope this just isn't required now :P
		/*
		QCARBehaviour qcarBehaviour = gameObject.GetComponent<QCARBehaviour> ();
		if (qcarBehaviour) {
			qcarBehaviour.StopQCAR();
		}
		*/
		Application.Quit ();
	}

    #region CommentedRegion

    private IEnumerator LoadIssueDataPreviewImpl (string url)
    {
    	WWW www = new WWW (url);
    	yield return www;

    	if (www.error != null) {
    		Debug.LogError ("Unable to load issue data: " + www.error);
    	} else {
    		Debug.LogError ("LoadIssueDataPreviewImpl " + www.error);
    		string jsonString = www.text;

    		LoadPreview (jsonString);
    	}
    }


    	private IEnumerator LoadIssueDataImpl (string base_url, bool jsonOnly, bool jar, VuforiaUnity.StorageType storageType)
    	{
    		//ACPUnityPlugin.logNative ("LoadIssueDataImpl() start");
    		Debug.Log("LoadIssueDataImpl() start");
    		string jsonUrl = "file://" + base_url + ".json";
    
    		if (jar)
    		{
    			jsonUrl = "jar:" + jsonUrl;
    		}
    		
    		string datasetPath = base_url + ".xml";
    		
    		//LoadDataSet ("QCAR/StonesAndChips.xml", DataSet.StorageType.STORAGE_APPRESOURCE);
    		if (jar && storageType == VuforiaUnity.StorageType.STORAGE_ABSOLUTE)
    		{
    			datasetPath = datasetPath.Replace ("!", "");
    		}
    
    #if UNITY_WEBPLAYER
    #else
    		DateTime modifiedDate = File.GetLastWriteTime (datasetPath);
    		
    		if (this.datasetPath != null && this.datasetPath == datasetPath && modifiedDate <= datasetModifiedDate) {
    			//ACPUnityPlugin.logNative ("Dataset has not changed.. loading json only");
    			//Debug.Log("Dataset has not changed!");
    			jsonOnly = true;
    		}
    		this.datasetPath = datasetPath;
    		this.datasetModifiedDate = modifiedDate;
    #endif
    		//Debug.Log("JSON URL IS: " + jsonUrl);
    		
    		WWW www = new WWW (jsonUrl);
    		yield return www;
    		
    		if (www.error != null) 
    		{
    			Debug.Log ("Unable to load issue data: " + www.error);
    			//ACPUnityPlugin.logNative ("Unable to load issue data: " + www.error);
    		} else  {
    			//ACPUnityPlugin.logNative ("Loaded package json data");
    			Debug.Log ("Loaded package json data");
    			string jsonString = www.text;
    			//Debug.Log (jsonString);
    			
    			JSONObject jsonObject = new JSONObject (jsonString);
    			
    			package = new PackageData ();
    			package.id = (int)jsonObject["id"].n;
    			
    			JSONObject temp = jsonObject["tracking_id"];
    			if (temp != null)
    				package.tracking_id = temp.str;
    			
    			temp = jsonObject["channel_tracking_id"];
    			if (temp != null)
    				package.channel_tracking_id = temp.str;
    			
    			//ACPUnityPlugin.logNative ("Attempting to load trackables");
    
    			JSONObject trackablesJson = jsonObject["trackables"];
    
    			// Load the trackables data
    			for (int ti = 0; ti < trackablesJson.Count; ti++) {
    				package.trackables.Add (TrackableData.Create (trackablesJson[ti]));
    			}
    			//Debug.Log ("Issue: " + issue.id + "\nDescription: " + issue.description);
    			
    			if (Application.platform == RuntimePlatform.OSXEditor) {
    				LoadDataSet (Application.dataPath + "/StreamingAssets/QCAR/StonesAndChips.xml", storageType);
    			} 
    			else 
    			{
    				if (jsonOnly)
    				{
    					//ACPUnityPlugin.logNative ("Detatching content from trackables");
    					DetatchContentFromTrackables ();
    					//ACPUnityPlugin.logNative ("Attaching content to trackables");
    					AttachContentToTrackables (dataSet);
    					
                        ACPUnityPlugin.Instnace.dataLoadFinished ();
    				}
    				else
    				{
    					LoadDataSet (datasetPath, storageType);
    				}
    			}
    		}
    		
    	}


        //Load and activate a data set at the given path.

    	private IEnumerator LoadDataSetNextFrame(string dataSetPath, VuforiaUnity.StorageType storageType)
    	{
    		yield return new WaitForSeconds(1.5f);
    		LoadDataSet(dataSetPath, storageType);
    	}
    	
    	private bool LoadDataSet (string dataSetPath, VuforiaUnity.StorageType storageType)
    	{
    		//return true;
    		if (dataSet != null) {
    			DestroyDataSet (dataSet);
    			dataSet = null;
    			
    			// This is a hack..
    			// QCAR seems to have a bug where the tracking objects wont have been destroyed yet, and if you try to reload the same dataset immediately, it has 
    			// a massive cry and crashes. This just puts a small delay in so the dataset will be loaded after at least one frame
    			// which seems to make it happy again.
    			StartCoroutine (LoadDataSetNextFrame (dataSetPath, storageType));
    			return true;
    		}
    		else
    		{
    			// Check if the data set exists at the given path.
    			if (!DataSet.Exists (dataSetPath, storageType)) {
    				//ACPUnityPlugin.logNative ("Data set " + dataSetPath + " does not exist.");
    				
    #if UNITY_WEBPLAYER
    				AttachContentToTrackables (dataSet);
    				PrintDebugInfo ();
    #else
    				if (Application.platform == RuntimePlatform.OSXEditor) 
    #endif
    				{
    					AttachContentToTrackables (dataSet);
    					PrintDebugInfo();
    				}
                    ACPUnityPlugin.Instnace.dataLoadFailed();
    
    				return false;
    			}
    			
    			// Request an ImageTracker instance from the TrackerManager.
    			ObjectTracker imageTracker = (ObjectTracker)TrackerManager.Instance.GetTracker<ObjectTracker>();
    			
    			// Create a new empty data set.
    			dataSet = imageTracker.CreateDataSet ();
    			
    			// Load the data set from the given path.
    			if (!dataSet.Load (dataSetPath, storageType)) {
    				//ACPUnityPlugin.logNative ("Failed to load data set " + dataSetPath + ".");
    				File.Delete (dataSetPath);
    	    		File.Delete (dataSetPath.Replace (".xml", ".dat"));
    				//ACPUnityPlugin.dataLoadFailed();
    				return false;
    			}
    			// (Optional) Activate the data set.
    			imageTracker.ActivateDataSet (dataSet);
    			
    			AttachContentToTrackables (dataSet);
    			
    			PrintDebugInfo();
    			
                ACPUnityPlugin.Instnace.dataLoadFinished();
    			
    			return true;
    		}
    	}
    
    	// Destroy a dataset.
    	private bool DestroyDataSet (DataSet dataSet)
    	{
    		
    		// Request an ImageTracker instance from the TrackerManager.
    		ObjectTracker imageTracker = (ObjectTracker)TrackerManager.Instance.GetTracker<ObjectTracker>();
    		
    		// Make sure the data set is not active.
    		imageTracker.DeactivateDataSet (dataSet);
    		
    		// (choose true to also destroy the Trackable objects that belong to the data set).
    		bool success = imageTracker.DestroyDataSet (dataSet, true);
    		
    		//if (success) Debug.Log("Did destroy data set!");
    		//else Debug.Log("Didn't destory data set!");
    		return success;
    	}

  
        // Add Trackable event handler and content (cubes) to the Trackables.
    	private void AttachContentToTrackables (DataSet dataSet)
    	{
    
    		Debug.Log ("TrackingManager AttachContentToTrackables");
    
    #if UNITY_WEBPLAYER
    		Debug.Log ("AttachContentToTrackables");
    		
    		Debug.Log ("Creating fake thing...");
    		GameObject go = (GameObject)Instantiate (Resources.Load ("ACPImageTarget"));
    		go.transform.localScale = new Vector3 (227.0f, 227.0f, 227.0f);
    		
    		ImageTargetBehaviour itb = go.GetComponent<ImageTargetBehaviour> ();
    		
    		CreateTrackableObject (itb, previewTrackableData);
    #else
    		if (Application.platform == RuntimePlatform.OSXEditor) {
    			
    			//Debug.Log ("doing shit...");
    			for (int i = 0; i < package.trackables.Count; i++) {
    				TrackableData data = package.trackables [i];
    				
    			//	Debug.Log ("Creating fake thing...");
    				GameObject go = (GameObject)Instantiate (Resources.Load ("ACPImageTarget"));
    				go.transform.localScale = new Vector3 (227.0f, 227.0f, 227.0f);
    				
    				ImageTargetBehaviour itb = go.GetComponent<ImageTargetBehaviour> ();
    				//itb.TrackableName = data.id;
    				
    				CreateTrackableObject (itb, data);
    			}
    		} 
    #endif
    	}

    	private void DetatchContentFromTrackables ()
    	{
    		Debug.Log ("TrackingManager DetachContentToTrackables");
    
    		GameObject[] trackables = GameObject.FindGameObjectsWithTag ("Trackable");
    		foreach (GameObject trackable in trackables)
    		{
    			// All this crap shoudl really be in a function in ACPTrackableBehavior...
    			foreach (Transform child in trackable.transform)
    			{
    				Destroy (child.gameObject);
    			}
    			
    			ACPTrackableBehavior behavior = trackable.GetComponent<ACPTrackableBehavior> ();
    			behavior.isVisible = false;
    			behavior.hasLoaded = false;
    
    			behavior.areas.Clear ();
    		}
    	}

    	private void CreateTrackableObject (DataSetTrackableBehaviour dstb, TrackableData data)
    	{
            ACPUnityPlugin.Instnace.logNative ("Attaching content for trackable with name: " + data.id);
    		
    		GameObject go = dstb.gameObject;
    		go.tag = "Trackable";
    		//Debug.Log ("Trackable Scale is: " + go.transform.localScale);
    		
    		ACPTrackableBehavior acpTrackable = go.GetComponent<ACPTrackableBehavior> ();
    		
    		if (acpTrackable == null) {
    			acpTrackable = go.AddComponent<ACPTrackableBehavior> ();
    		}
    		
    		acpTrackable.data = data;
    		
    //		GameObject text = (GameObject)Instantiate (Resources.Load ("TrackerElementLabel"));
    //		text.name = "DEBUG LABEL";
    //		text.GetComponent<TextMesh> ().text = data.id;
    //		text.transform.parent = dstb.transform;
    //		text.transform.localScale = new Vector3 (0.1f, 0.1f, 0.1f);
    //		text.transform.localRotation = Quaternion.Euler (new Vector3 (90, 0, 0));
    //		dstb.gameObject.active = true;
    		
    	}

#endregion

    #region Cloud
    public void OnInitialized (Vuforia.TargetFinder targetFinder)
	{
		//Debug.Log ("Vuforia Cloud Initialised!");
		// get a reference to the Image Tracker, remember it
		mImageTracker = TrackerManager.Instance.GetTracker<ObjectTracker>();
        mtargetFinder = targetFinder;
	}

	public void checkScanning ()
	{
		//Debug.Log ("Checking Scanning!");
		if (mCloudRecoBehaviour) {
           
			GameObject[] trackables = GameObject.FindGameObjectsWithTag ("Trackable");
			if(trackables.GetLength(0) > 0){
				//Debug.Log ("...Trackable exists - disabling scanning");
				mCloudRecoBehaviour.CloudRecoEnabled = false;
			} else if(scanningEnabledByApp){
				//Debug.Log ("...No trackable exists");
				mCloudRecoBehaviour.CloudRecoEnabled = true;
			}
		}

	}

	public void SetFocusContinuousAuto()
	{
		Debug.Log ("TrackingManager SetFocusContinuousAuto");
		if (!CameraDevice.Instance.SetFocusMode(CameraDevice.FocusMode.FOCUS_MODE_CONTINUOUSAUTO))
		{
            SetFocusModeToTriggerAuto();
		}
		else
		{
            mFocusMode = CameraDevice.FocusMode.FOCUS_MODE_CONTINUOUSAUTO;
		}
	}
	 
    /// <summary>
    /// called when an error is reported during initialization
    /// </summary>
    public void OnInitError (TargetFinder.InitState initError)
	{
		switch (initError) {
		case TargetFinder.InitState.INIT_ERROR_NO_NETWORK_CONNECTION:
                AppManager.Instnace.messageBoxManager.ShowGenericPopup ("Network Unavailable", "Please check your internet connection and try again.", "OK");
//			ACPUnityPlugin.showLocalizedAlert ("","VuforiaError2","OK");
			break;
		case TargetFinder.InitState.INIT_ERROR_SERVICE_NOT_AVAILABLE:
                AppManager.Instnace.messageBoxManager.ShowGenericPopup ("Service Unavailable", "The service is unavailable, please try again later.", "OK");
//			ACPUnityPlugin.showLocalizedAlert ("","VuforiaError5","OK");
			break;
		}
	}

    /// <summary>
    /// called when an error is reported while updating
    /// </summary>
    public void OnUpdateError (TargetFinder.UpdateState updateError)
	{
		//Debug.Log ("OnUpdateError: " + updateError.ToString ());
		
		switch (updateError) {
        case TargetFinder.UpdateState.UPDATE_ERROR_AUTHORIZATION_FAILED:
			//ACPUnityPlugin.showAlert ("Authorization Error", "The cloud recognition service access keys are incorrect or have expired.", "OK");
            AppManager.Instnace.messageBoxManager.ShowGenericPopup("Authorization Error", "The cloud recognition service access keys are incorrect or have expired.", "OK");
//			ACPUnityPlugin.showLocalizedAlert ("","VuforiaError1","OK");
			break; 
		case TargetFinder.UpdateState.UPDATE_ERROR_BAD_FRAME_QUALITY:
			//ACPUnityPlugin.showAlert ("Poor Camera Image", "The camera does not have enough detail, please try again later", "OK");
//			AppManager.Instnace.messageBoxManager.ShowMessage("Poor Camera Image", "The camera does not have enough detail, please try again later", "OK");
			break;
		case TargetFinder.UpdateState.UPDATE_ERROR_NO_NETWORK_CONNECTION:
//			ACPUnityPlugin.showHUD ("Network Unavailable", "Please check your internet connection and try again.", "ScanErrorNetworkIcon", 3.0f);
            AppManager.Instnace.messageBoxManager.ShowGenericPopup("Network Unavailable", "Please check your internet connection and try again.", "OK");
//			ACPUnityPlugin.showLocalizedAlert ("","VuforiaError2","OK");
			break;
		case TargetFinder.UpdateState.UPDATE_ERROR_PROJECT_SUSPENDED:
//			ACPUnityPlugin.showAlert ("Authorization Error", "The cloud recognition service has been suspended.", "OK");
                AppManager.Instnace.messageBoxManager.ShowGenericPopup("Authorization Error", "The cloud recognition service has been suspended.", "OK");
//			ACPUnityPlugin.showLocalizedAlert ("","VuforiaError3","OK");
			break;
		case TargetFinder.UpdateState.UPDATE_ERROR_REQUEST_TIMEOUT:
//			ACPUnityPlugin.showHUD ("Request Timeout", "The network request has timed out, please check your internet connection and try again.", "ScanErrorTimeoutIcon", 3.0f);
//			AppManager.Instnace.messageBoxManager.ShowMessage("Request Timeout", "The network request has timed out, please check your internet connection and try again.", "OK");
//			ACPUnityPlugin.showLocalizedAlert ("","VuforiaError4","OK");
			break;
		case TargetFinder.UpdateState.UPDATE_ERROR_SERVICE_NOT_AVAILABLE:
//			ACPUnityPlugin.showAlert ("Service Unavailable", "The service is unavailable, please try again later.", "OK");
                AppManager.Instnace.messageBoxManager.ShowGenericPopup("Service Unavailable", "The service is unavailable, please try again later.", "OK");
//			ACPUnityPlugin.showLocalizedAlert ("","VuforiaError5","OK");
			break;
		case TargetFinder.UpdateState.UPDATE_ERROR_TIMESTAMP_OUT_OF_RANGE:
//			ACPUnityPlugin.showAlert ("Clock Sync Error", "Please update the date and time and try again.", "OK");
                AppManager.Instnace.messageBoxManager.ShowGenericPopup("Clock Sync Error", "Please update the date and time and try again.", "OK");
//			ACPUnityPlugin.showLocalizedAlert ("","VuforiaError6","OK");
			break;
		case TargetFinder.UpdateState.UPDATE_ERROR_UPDATE_SDK:
			//ACPUnityPlugin.showAlert ("Unsupported Version", "The application is using an unsupported version of Vuforia.", "OK");
                AppManager.Instnace.messageBoxManager.ShowGenericPopup("Unsupported Version", "The application is using an unsupported version of Vuforia.", "OK");
//			ACPUnityPlugin.showLocalizedAlert ("","VuforiaError7","OK");
			break;
		}
	}

    /// <summary>
    /// called when the CloudRecoBehaviour starts or stops scanning
    /// </summary>
    public void OnStateChanged (bool scanning)
	{
		//Debug.Log ("Vuforia State changed, scanning: " + scanning);
		if (scanning) {
			this.scanStartTime = Time.time;
			this.scanEndTime = -1f;
			this.scanTotalTime = -1f;
			this.requestStartTime = -1f;
			this.requestEndTime = -1f; 
			this.requestTotalTime = -1f;
			this.isShowingHoldStill = false;
		}
		if (!scanning) {
			this.isShowingHoldStill = true;
		}
       
		if (AppManager.Instnace.isVuforiaOn) {
			//This should automatically enable the scanning if it is not On.
			//checkScanning ();
            m_ScanLine.ShowScanLine(scanning);

            if (AppManager.Instnace.pushNotificationController.isPushNotificationShown)
            {
                m_ScanLine.ShowScanLine(false);
            }

		} else {
			m_ScanLine.ShowScanLine (false);
		}
	}

    /// <summary>
	/// called when a new search result is found ( No preloader will attach until the Search is not found)
    /// </summary>
    public void OnNewSearchResult (TargetFinder.TargetSearchResult targetSearchResult)
	{
		Debug.Log ("OnNewSearchResult - New search result: " + targetSearchResult.UniqueTargetId + " - " + targetSearchResult.TargetName);
		
		requestEndTime = Time.time;
		requestTotalTime = requestEndTime - requestStartTime;
        //UpdateStatsText ();

        /* Test Stuff 
		if (targetSearchResult.TargetName == "Mintia_other_db") {
			mCloudRecoBehaviour.AccessKey = "9109be38adc3f5a31e19d4292d5f30fc1468305d";
			mCloudRecoBehaviour.SecretKey = "424484c23d8e3b5799df13c79a1925d108698bfa";
		} else {
			mCloudRecoBehaviour.AccessKey = "5cd34002b78d97211066f5e27b46f3587034005f";
			mCloudRecoBehaviour.SecretKey = "8a40ecc75fcf6314fd90bd3c67f6d19318df72a5";
		}
		
		mCloudRecoBehaviour.Initialize ();
		*/

        TargetFinder.CloudRecoSearchResult cloudRecoSearchResult = (TargetFinder.CloudRecoSearchResult)targetSearchResult;

        Debug.Log ("cloudRecoSearchResult -------------->  " + cloudRecoSearchResult.MetaData);
		
        JSONObject metaJson = new JSONObject (cloudRecoSearchResult.MetaData);
		string triggerId;
		
		try {
			triggerId = metaJson ["triggerId"].str;
		} catch (Exception ex) {
			Debug.Log (ex.ToString());
			return;
		}

        //Jeetesh - Uncomment the below line if you want to show a popup to show trigger id and cloud id after scanning image.
        //AppManager.Instnace.messageBoxManager.ShowMessage("OnNewSearchResult","Trigger id:-"+ triggerId + ", Cloud id:-" + targetSearchResult.UniqueTargetId, "Ok", null);
		
		// Test mode sends timing information to the server.
		if (this.timingEvents)
		{
			//Debug.Log("CLOUD took : " + (requestTotalTime));
			ACPUnityPlugin.Instnace.trackingEvent("Timing", UInt32.Parse(triggerId), -1, -1, -1, "", "CLOUD", "", "", "", requestTotalTime);
            ACPUnityPlugin.Instnace.trackEvent("Scan", "ScannedTime", "Time", (int)Math.Ceiling(requestEndTime));
		}

		// enable the new result with the same ImageTargetBehaviour:
        ImageTargetBehaviour imageTargetBehaviour = (ImageTargetBehaviour) mtargetFinder.EnableTracking (targetSearchResult, "Trackable-" + triggerId);

		if (imageTargetBehaviour != null) {

			//ACPUnityPlugin.logNative ("Attaching content for trackable with name: " + triggerId);

			ACPUnityPlugin.Instnace.trackingEvent("ScannedTrigger", UInt32.Parse(triggerId), -1, -1, -1, "", "", "", "", "", -1);
            ACPUnityPlugin.Instnace.trackEvent("Scan", "ScannedTrigger", "triggerId", Int32.Parse(triggerId));

			CreateACPTrackable(imageTargetBehaviour.gameObject, triggerId, targetSearchResult.UniqueTargetId, null);
            
		} else {
			Debug.Log ("image target behaviour is  null!");
		}

	}

	//This function is called from iOS to load dynamic content from CMS
	public void CreateACPTrackable(string triggerTargetJsonString)
	{
		Debug.Log ("CreateACPTrackable-:");
		string[] split = triggerTargetJsonString.Split('|');
		CreateACPTrackable(new GameObject(), split[0], split[1], split[2]);
	}
	
	private void CreateACPTrackable(GameObject gameObject, string triggerId, string targetId, string json)
	{

		WebService.Instnace.catDetailParameter.HubTrigger = long.Parse(triggerId);
		WebService.Instnace.catDetailParameter.CloudTriggerId = targetId;
		WebService.Instnace.catDetailParameter.EmailId = AppManager.Instnace.userEmail;
		WebService.Instnace.catDetailParameter.RegionId = AppManager.Instnace.regionId;

		Debug.Log ("TrackingManager CreateACPTrackable");
		Debug.Log ("json :-:" + json);

		ACPUnityPlugin.Instnace.EnableButtons ();

        if (json != null && json != "")
        {
            AddHistoryData(json);
        }
		// Don't create a trackable if the old one is still there..
		/*
		GameObject[] trackables = GameObject.FindGameObjectsWithTag ("Trackable");
		if (trackables.Length > 0)
		{
			return;
		}
		*/
		
		mCloudRecoBehaviour.CloudRecoEnabled = false;

		gameObject.tag = "Trackable";

        ACPTrackableBehavior acpTrackable = gameObject.GetComponent<ACPTrackableBehavior> ();
		if (acpTrackable == null) {
			acpTrackable = gameObject.AddComponent<ACPTrackableBehavior> ();
		}
				
		acpTrackable.triggerId = triggerId;
		acpTrackable.targetId = targetId;

		//StartCoroutine ( SetLovedButtonOnScanScreen (triggerId));
	}
	
//	public void setNavigationButtonsVisibility (bool visibility)
//	{
//		ACPUnityPlugin.Instnace.setNavigationButtonsVisibility (visibility);
//	}

//	public void setNavigationButtonsEnabled (bool enabled)
//	{
//		ACPUnityPlugin.Instnace.setNavigationButtonsEnabled (enabled);
//	}

	public void setBackButtonVisibility(bool visibility){
		ACPUnityPlugin.Instnace.setBackButtonVisibility (visibility);
	}

//	public void setCloseButtonVisibility (bool visibility)
//	{
//		ACPUnityPlugin.Instnace.setCloseButtonVisibility (visibility);
//	}
	#endregion

	public void LoadTriggerFromMenu(){
		//CreateACPTrackable(new GameObject(), "140917", "818e3eb4986e48e48241b6b15ebab529", null); //multi area trigger for navigation testing - LIVE
//		CreateACPTrackable(new GameObject(), "138205", "3a048bea49714011a223451fd29e4db5", null); // crazy parting explosion - LIVE
		//CreateACPTrackable(new GameObject(), "156512", "e7228a4f1acb412397b7c5e78b6fd660", null); // Fresh Herbs size test - LIVE - http://content.viewa.com/australia/ad/gourmet-garden/1409/ad_gourmetgarden_herb_1409_01.aspx
		//CreateACPTrackable(new GameObject(), "155524", "09c75759e5c8437ba976e256e38d71c3", null); // Body Shop size test - LIVE - http://content.viewa.com/australia/ad/bodyshop/1408/ad_bodyshop_bachelor_14xx_1.aspx
		//CreateACPTrackable(new GameObject(), "149694", "d5517d4d7a044c3fbb331a3cbc51ecbd", null); // 20th century new girl hub - LIVE
		//CreateACPTrackable(new GameObject(), "137538", "c7a4fe4f47c34e18877137e59e2cd270", null); // AirNZ HUB - LIVE
		//CreateACPTrackable(new GameObject(), "54194", "9520eaaa73494f0e81a7fb0313a0922a", null); // Anchor Bay - width test - LIVE
		//CreateACPTrackable(new GameObject(), "169593", "519f16fe75504d428b111c0cdb688345", null); // new default snapto layout
		//CreateACPTrackable(new GameObject(), "174107", "bd1e77ab79974dc3ac441d988b0ed5cf", null); // VR-619 - car, lighting
		//CreateACPTrackable(new GameObject(), "174519", "39ee165b598c43e2ba13512f573a2df2", null); //AR Video
	}

	void ShowStatusOverlay(string msg){

		//ACPUnityPlugin.Instnace.showStatusOverlay (msg);
	}

	void HideStatusoverlay(){
		Debug.Log ("HideStatusoverlay");
		if (_scanPanel == null) {
			_scanPanel = (ScanPanel)CanvasManager.Instnace.ReturnPanelManager (ePanelManager.MainMenuPanelManager).ReturnPanel (ePanels.Scan_Panel);
			if(_scanPanel != null)
				_scanPanel.progressStatusText.text = "";
		} else {
			_scanPanel.progressStatusText.text = "";	
		}
	}

	//This function is called from Update() function
	void EventsUpdate(){

        // 3D Tap Handling
        bool mouseOver = false;
        bool mouseReleased = false;
        Vector2 screenPoint = Input.mousePosition;

        if (Input.touches.Length > 0)
        {
            mouseOver = true;

            screenPoint = Input.touches[0].position;

            if (Input.touches[0].phase == TouchPhase.Ended)
            {
                mouseReleased = true;
            }
        }

        if (Input.GetMouseButton(0))
        {
            mouseOver = true;
        }

        if (Input.GetMouseButtonUp(0))
        {
            mouseReleased = true;
        }

        if (mouseOver || mouseReleased)
        {
            RaycastHit hitInfo;
            int layerMask = 1 << 8 | 1 << 0;

            if (Physics.Raycast(Camera.main.ScreenPointToRay(screenPoint), out hitInfo, Mathf.Infinity, layerMask))
            {

                //Shan: only process touches below the nav bar on android, otherwise the click event on the nav buttons (back, home, share, close) will go through and trigger the AR buttons behind nav bar
#if UNITY_ANDROID
                if (screenPoint.y < (Screen.height - navBarHeight))
                {
#endif

                    // Hit a button
                    ButtonBehavior button = hitInfo.transform.parent.gameObject.GetComponent<ButtonBehavior>();
                    if (button != null)
                    {
                        if (isHovering == false)
                        {
                            currentHoverWidget = button;
                        }
                        if (currentHoverWidget == button)
                        {
                            button.OnHover(mouseReleased);
                        }
                    }
                    else
                    {
                        VideoBehavior video = hitInfo.transform.parent.gameObject.GetComponent<VideoBehavior>();
                        if (video != null)
                        {
                            if (isHovering == false)
                            {
                                currentHoverWidget = video;
                            }
                            if (currentHoverWidget == video)
                            {
                                video.OnHover(mouseReleased);
                            }
                        }
                    }
                    if (mouseReleased)
                    {
                        currentHoverWidget = null;
                        isHovering = false;
                    }
                    else if (mouseOver)
                    {
                        isHovering = true;
                    }

#if UNITY_ANDROID
                }
#endif
            }
            else if (mouseReleased)
            {
                // Missed buttons - trigger a focus manually
                StartCoroutine(SetFocusModeToTriggerAuto());
                isHovering = false;
            }
            else if (mouseOver)
            {
                isHovering = true;
            }

        }

        if (mCloudRecoBehaviour != null)
        {
            if (mCloudRecoBehaviour.CloudRecoInitialized && (mtargetFinder.IsRequesting()))
            {

                // we are requesting
                if (scanTotalTime < 0)
                {
                    scanEndTime = Time.time;
                    scanTotalTime = scanEndTime - scanStartTime;
                    requestStartTime = scanEndTime;
                }

                if (!isRequesting)
                {
                    isRequesting = true;
                    isShowingHoldStill = false;
                    //                  ACPUnityPlugin.showStatusOverlay ("ScanProcessingOverlayTitle", "ScanProcessingOverlaySubtitle");


                    ACPUnityPlugin.Instnace.showStatusOverlay("ScanOverlayTextDownloading");
                }

                requestTotalTime = Time.time - requestStartTime;
                requestEndTime = Time.time;
                UpdateStatsText();

                //requestingText.enabled = true;
            }
            else
            {
                if (scanTotalTime >= 0)
                {
                    scanTotalTime = -1f;
                    scanStartTime = Time.time;
                    scanEndTime = -1f;

                    requestEndTime = scanStartTime;
                    requestTotalTime = requestEndTime - requestStartTime;
                    UpdateStatsText();
                }

                // hide the status overlay if we need to
                if (isRequesting)
                {
                    isRequesting = false;

                    ACPUnityPlugin.Instnace.hideStatusoverlay();
                }

                // do we need to shwo the hold still message?
                if (!isShowingHoldStill && (Time.time - scanStartTime > 3.0f))
                {
                    //                  ACPUnityPlugin.showStatusOverlay ("ScanScanningOverlayTitle", "ScanScanningOverlaySubtitle");

                    ACPUnityPlugin.Instnace.showStatusOverlay("ScanOverlayTextHoldStill");
                    isShowingHoldStill = true;
                }

                //requestingText.enabled = false;
            }
        }

#if UNITY_EDITOR
        if (Input.GetKeyDown("return"))
        {
            delayedTestTriggerLoad();
        }
#endif
	}

	void AddHistoryData (string json){

		Debug.Log ("json :"+ json);
		JSONNode test = JSONNode.Parse (json);
		if (test != null) {
			WebService.Instnace.historyData.history_Title = test ["history_title"] == null?"":test ["history_title"].Value;
			WebService.Instnace.historyData.history_icon = test ["history_icon"] == null?"":test ["history_icon"].Value;
			WebService.Instnace.historyData.history_Image = test ["history_image"] == null?"":test ["history_image"].Value;
			WebService.Instnace.historyData.history_Description = test ["history_description"] == null?"":test ["history_description"].Value;

			Debug.Log ("History_Title :" + WebService.Instnace.historyData.history_Title);
			Debug.Log ("History_image :" + WebService.Instnace.historyData.history_Image);
		}
	}

	IEnumerator LoadBlankHistoryIcon(string url, BlankPanel ablankPanel) {
		WWW www = new WWW (url);
		yield return www;
		if (string.IsNullOrEmpty (www.error)) {
			ablankPanel.history_Icon.sprite = Sprite.Create (www.texture, new Rect (0, 0, www.texture.width, www.texture.height), new Vector2 (0, 0));
		}
	}

	IEnumerator LoadScanHistoryIcon(string url, ScanPanel aScanPanel) {
		WWW www = new WWW (url);
		yield return www;
		if (string.IsNullOrEmpty (www.error)) {
			aScanPanel.history_Icon.sprite = Sprite.Create (www.texture, new Rect (0, 0, www.texture.width, www.texture.height), new Vector2 (0, 0));
		}
	}

	IEnumerator SetLovedButtonOnScanScreen(string triggerId) {

		yield return new WaitForEndOfFrame ();
		string url = AppManager.Instnace.baseURL + "/cloud/LoveServiceScan.aspx?" + "emailId=" + AppManager.Instnace.userEmail + "&c=10&rid=" +AppManager.Instnace.regionId + "&triggerId="+ triggerId;
		WebService.Instnace.Get (url, ScanLovedCallBack);
	}

	void ScanLovedCallBack(string response) {

		Debug.Log ("Scan Loved Response :" + response);

		ScanPanel scanPanel = (ScanPanel) CanvasManager.Instnace.ReturnPanelManager (ePanelManager.MainMenuPanelManager).ReturnPanel (ePanels.Scan_Panel);

		if (response == "" || response == null || response == "[]") {

			scanPanel.SetLovedButton (false);
		} else {

			var res = JSON.Parse (response);

			Debug.Log ("res :" + res.ToString ());

			if (res != null) {

				JSONNode test = JSONNode.Parse (response);

				CategoryDetailWebService catDetail = new CategoryDetailWebService ();
				catDetail.id = test[0] ["id"] == null ? 0 : test [0]["id"].AsInt;
				catDetail.cloud_id = test[0] ["cloud_id"] == null ? "" : test[0] ["cloud_id"].Value;
				catDetail.history_title = test[0] ["history_title"] == null ? "" : test[0] ["history_title"].Value;
				catDetail.history_description = test[0] ["history_description"] == null ? "" : test[0] ["history_description"].Value;
				catDetail.history_image = test[0] ["history_image"] == null ? "" : test[0] ["history_image"].Value;
				catDetail.history_icon = test[0] ["history_icon"] == null ? "" : test[0] ["history_icon"].Value;
				catDetail.keywords = "";//test["keywords"].Value;
				catDetail.channel_id = test[0] ["channel_id"] == null ? 0 : test[0] ["channel_id"].AsInt;
				catDetail.channel_name = test[0] ["channel_name"] == null ? "" : test[0] ["channel_name"].Value;
                catDetail.is_last = test[0] ["is_last"] == null ? false : test[0] ["is_last"].AsBool;
				catDetail.CategoryId = test[0] ["CategoryId"] == null ? "" : test[0] ["CategoryId"].Value;
				Debug.Log ("Scan is_last :" + catDetail.is_last + "catDetail.id :" + catDetail.id);
				scanPanel.SetLovedButton (catDetail.is_last);
			}
		}
	}

    public void EnableVuforia() {
        VuforiaRuntimeUtilities.SetAllowedFusionProviders(FusionProviderType.VUFORIA_VISION_ONLY);
#if UNITY_ANDROID || UNITY_IOS
        GetComponent<VuforiaBehaviour>().enabled = true;
#endif
        GetComponent<DefaultInitializationErrorHandler>().enabled = true;
    }
    public void DisableVuforia() {

        GetComponent<VuforiaBehaviour>().enabled = false;
        GetComponent<DefaultInitializationErrorHandler>().enabled = false;
    }
}


