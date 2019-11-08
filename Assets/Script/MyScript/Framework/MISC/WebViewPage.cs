using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using VoxelBusters.Utility.UnityGUI.MENU;
using VoxelBusters.Utility;
using VoxelBusters.NativePlugins;
using VoxelBusters.NativePlugins.Demo;
using OTPL.UI;
using System.IO;

public class WebViewPage : MonoBehaviour {

	#region Properties
	[SerializeField]
	private 	string 		m_url;
	//[SerializeField, Multiline(6)]
	//private 	string		m_HTMLString;
	//[SerializeField]
	//private 	string		m_javaScript;
	//[SerializeField]
	//private 	string		m_evalString;
	//[SerializeField]
	private 	string[]	m_schemeList	= new string[] {
		"unity",
		"mailto",
		"tel"
	};
	public bool webViewIsLoaded;
	public GameObject m_webViewPrefab;
    [HideInInspector]
	public WebView	m_webview;
	private float footer;
	//GameObject aWebView;
	int navHeight;
	ACPTrackableBehavior acpTrackable;

	void Awake() {
	
        if (m_webViewPrefab == null)
			Debug.LogWarning ("WebView prefab is not present.");
	}

	void Start ()
	{
		m_webview = gameObject.GetComponentInChildren<WebView> ();
        m_webview.AutoShowOnLoadFinish = false;
        m_webview.CanBounce = true;
        //m_webview.ShowSpinnerOnLoad = true;
        m_webview.ScalesPageToFit = false;
        m_webview.AutoShowOnLoadFinish = false;
		// Unset enable feature text
		//WebView works differently when compared to other features supported by our plugin. Here Native WebView is represented as a GameObject.
		//So this means you can have multiple instances of WebView in a single view and customise its functionalities as per your preference.");
		//Either you can create a GameObject with WebView component attached to it. Or else you can make use of WebView prefab provided along with this plugin (Path: Assets/VoxelBusters/NativePlugins/Prefab). "
	}

	void OnEnable ()
	{
		// Set frame
		// Registering callbacks
		WebView.DidShowEvent						+= DidShowEvent;
		WebView.DidHideEvent						+= DidHideEvent;
		WebView.DidDestroyEvent						+= DidDestroyEvent;
		WebView.DidStartLoadEvent					+= DidStartLoadEvent;
		WebView.DidFinishLoadEvent					+= DidFinishLoadEvent;
		WebView.DidFailLoadWithErrorEvent			+= DidFailLoadWithErrorEvent;
		WebView.DidFinishEvaluatingJavaScriptEvent	+= DidFinishEvaluatingJavaScriptEvent;
		WebView.DidReceiveMessageEvent				+= DidReceiveMessageEvent;
	}

	void OnDisable ()
	{
		// Deregistering callbacks
		WebView.DidShowEvent						-= DidShowEvent;
		WebView.DidHideEvent						-= DidHideEvent;
		WebView.DidDestroyEvent						-= DidDestroyEvent;
		WebView.DidStartLoadEvent					-= DidStartLoadEvent;
		WebView.DidFinishLoadEvent					-= DidFinishLoadEvent;
		WebView.DidFailLoadWithErrorEvent			-= DidFailLoadWithErrorEvent;
		WebView.DidFinishEvaluatingJavaScriptEvent	-= DidFinishEvaluatingJavaScriptEvent;
		WebView.DidReceiveMessageEvent				-= DidReceiveMessageEvent;
	}

	#endregion

	#region GUI Methods

//	void DisplayFeatureFunctionalities ()
//	{
//
//		if (m_webview == null)
//		{
//			GUILayout.Label("Create WebView", kSubTitleStyle);
//
//			if (GUILayout.Button("Create"))
//			{
//				GameObject _newWebviewGO	= new GameObject("WebView");
//				m_webview					= _newWebviewGO.AddComponent<WebView>();
//
//				AddNewResult("Successfully created new WebView.");
//			}
//
//			return;
//		}
//
//		DrawLoadAPI();
//		DrawLifeCycleAPI();
//		DrawControlTypes();
//		DrawPropertiesAPI();
//
//		// Misc
//		GUILayout.Label("Misc.", kSubTitleStyle);
//
//		if (GUILayout.Button("Add New URL Scheme Name"))
//		{
//			AddNewURLSchemeName();
//		}
//
//		GUILayout.Box ("[NOTE] You will receive DidReceiveMessageEvent, when webview tries to load URL which starts with currently watched URL Scheme's.");
//
//		if (GUILayout.Button("Clear Cache"))
//		{		
//			ClearCache();
//		}
//	}
//
//	private void DrawLoadAPI ()
//	{
//		GUILayout.Label("Load URL", kSubTitleStyle);
//
//		m_url = GUILayout.TextField(m_url);
//
//		if (GUILayout.Button("Load"))
//		{
//			LoadRequest();
//		}
//
//		GUILayout.Label("Other Load Operations", kSubTitleStyle);
//
//		if (GUILayout.Button("Load HTML String"))
//		{
//			LoadHTMLString();
//		}
//
//		if (GUILayout.Button("Load HTML String With JavaScript"))
//		{
//			LoadHTMLStringWithJavaScript();
//		}
//
//		if (GUILayout.Button("Load File"))
//		{
//			LoadFile();
//		}
//
//		GUILayout.Box ("[NOTE] You will receive DidStartLoadEvent, when webview starts loading page.");
//		GUILayout.Box ("[NOTE] You will receive DidFinishLoadEvent, when webview finishes loading page.");
//		GUILayout.Box ("[NOTE] You will receive DidFailLoadWithErrorEvent, when webview fails to load page.");
//
//		if (GUILayout.Button("Evaluate JavaScript"))
//		{
//			EvaluateJavaScriptFromString();
//		}
//
//		GUILayout.Box ("[NOTE] You will receive DidFinishEvaluatingJavaScriptEvent, when webview finishes evaluating javascript expression.");
//	}
//
//	private void DrawLifeCycleAPI ()
//	{
//		GUILayout.Label("Lifecycle", kSubTitleStyle);
//
//		if (GUILayout.Button("Show"))
//		{		
//			ShowWebView();
//		}
//
//		GUILayout.Box ("[NOTE] DidShowEvent is fired, when webview appears on the screen.");
//
//		if (GUILayout.Button("Hide"))
//		{		
//			HideWebView();
//		}
//
//		GUILayout.Box ("[NOTE] DidHideEvent is fired, when webview is removed from the screen.");
//
//		if (GUILayout.Button("Destroy"))
//		{		
//			DestroyWebView();
//		}
//
//		GUILayout.Box ("[NOTE] DidDestroyEvent is fired, when webview is destroyed.");
//	}
//
//	private void DrawControlTypes ()
//	{
//		GUILayout.Label("Control Types", kSubTitleStyle);
//
//		GUILayout.BeginHorizontal();
//		{
//			bool _usingNoControls	= m_webview.ControlType == eWebviewControlType.NO_CONTROLS;
//			bool _usingCloseButton	= m_webview.ControlType == eWebviewControlType.CLOSE_BUTTON;
//			bool _usingToolbar		= m_webview.ControlType == eWebviewControlType.TOOLBAR;
//
//			if (_usingNoControls != GUILayout.Toggle(_usingNoControls, "No Controls"))
//				m_webview.ControlType	= eWebviewControlType.NO_CONTROLS;
//
//			if (_usingCloseButton != GUILayout.Toggle(_usingCloseButton, "Close Button"))
//				m_webview.ControlType	= eWebviewControlType.CLOSE_BUTTON;
//
//			if (_usingToolbar != GUILayout.Toggle(_usingToolbar, "Tool Bar"))
//				m_webview.ControlType	= eWebviewControlType.TOOLBAR;
//		}
//		GUILayout.EndHorizontal();
//	}
//
//	private void DrawPropertiesAPI ()
//	{
//		GUILayout.Label("Properties", kSubTitleStyle);
//
//		GUILayout.BeginVertical(UISkin.scrollView);
//		GUILayout.BeginHorizontal();
//
//		bool _canHideNewValue				= GUILayout.Toggle(m_webview.CanHide, "CanHide");
//		bool _canBounceNewValue				= GUILayout.Toggle(m_webview.CanBounce, "CanBounce");
//		bool _showSpinnerOnLoadNewValue		= GUILayout.Toggle(m_webview.ShowSpinnerOnLoad, "ShowSpinnerOnLoad");
//
//		GUILayout.EndHorizontal();
//
//		GUILayout.BeginHorizontal();
//
//		bool _autoShowOnLoadFinishNewValue	= GUILayout.Toggle(m_webview.AutoShowOnLoadFinish, "AutoShowOnLoadFinish");
//		bool _scalesPageToFitNewValue		= GUILayout.Toggle(m_webview.ScalesPageToFit, "ScalesPageToFit");
//
//		GUILayout.EndHorizontal();
//		GUILayout.EndVertical();
//
//		// Update the value only on value change
//		if (_canHideNewValue != m_webview.CanHide)
//			m_webview.CanHide				= _canHideNewValue;
//
//		if (_canBounceNewValue != m_webview.CanBounce)
//			m_webview.CanBounce				= _canBounceNewValue;
//
//		if (_showSpinnerOnLoadNewValue != m_webview.ShowSpinnerOnLoad)
//			m_webview.ShowSpinnerOnLoad		= _showSpinnerOnLoadNewValue;
//
//		if (_autoShowOnLoadFinishNewValue != m_webview.AutoShowOnLoadFinish)
//			m_webview.AutoShowOnLoadFinish	= _autoShowOnLoadFinishNewValue;
//
//		if (_scalesPageToFitNewValue != m_webview.ScalesPageToFit)
//			m_webview.ScalesPageToFit		= _scalesPageToFitNewValue;
//
//		if (GUILayout.Button("Set Frame"))
//		{		
//			SetFrame();
//		}
//
//		if (GUILayout.Button("Set Full Screen Frame"))
//		{
//			SetFullScreenFrame();
//		}
//	}

	#endregion

	#region API Methods

	private void LoadRequest (string a_url)
	{
		m_webview.LoadRequest(a_url);
	}

	//public void LoadHTMLString ()
	//{
	//	m_webview.LoadHTMLString(m_HTMLString);
	//}

	//public void LoadHTMLStringWithJavaScript ()
	//{
	//	m_webview.LoadHTMLStringWithJavaScript(m_HTMLString, m_javaScript);						
	//}

	public void LoadFile ()
	{
//		m_webview.LoadFile(Demo.Utility.GetScreenshotPath(), "image/png", null, null);
	}

	//public void EvaluateJavaScriptFromString ()
	//{
	//	m_webview.EvaluateJavaScriptFromString(m_evalString);
	//}

	public void ShowWebView ()
	{
		m_webview.Show();
	}

	public void HideWebView ()
	{
		m_webview.Hide();
	}

	private void AddNewURLSchemeName ()
	{
		//		AddNewResult("Registered schemes for receiving web view messages.");

		for (int _iter = 0; _iter < m_schemeList.Length; _iter++)
			m_webview.AddNewURLSchemeName(m_schemeList[_iter]);
	}

    private void SetFrame()
    {
        if (Screen.width > 1080f && Screen.height > 1920)
        {
            navHeight = AppManager.Instnace.acpTrackingManager.navBarHeight + 25; //10
            footer = 90f + 80f;  //90 + 40f
        }
        else if(Screen.width == 1080f && Screen.height == 1920){

            navHeight = AppManager.Instnace.acpTrackingManager.navBarHeight;
            footer = 90f + 40f;  //90 + 40f
        }
        else
        {
            navHeight = AppManager.Instnace.acpTrackingManager.navBarHeight; //20
            footer = 90f + 30f;  //90 + 30f
        }
        float newHeight = Screen.height - navHeight;
        //m_webview.Frame = new Rect(0f, navHeight, Screen.width, newHeight - footer);
        m_webview.Frame = new Rect(0f, navHeight, Screen.width, newHeight - footer);

        //m_webview.Frame       = new Rect(0f, Screen.height * 0.75f, Screen.width, Screen.height * 0.2f);

        //      AddNewResult(string.Format("Setting new frame: {0} for web view.", m_webview.Frame));
    }

//	private void SetFrame ()
//	{
//		if (AppManager.Instnace.isIphoneX) {
//			navHeight = AppManager.Instnace.acpTrackingManager.navBarHeight + 50;  //30
//			footer = 90 + 50f;
//		} else {
//			navHeight = AppManager.Instnace.acpTrackingManager.navBarHeight + 16; //20
//			footer = 90;
//		}
//		float newHeight = Screen.height - navHeight;
//		m_webview.Frame		= new Rect(0f, navHeight, Screen.width, newHeight-footer);
//		//m_webview.Frame		= new Rect(0f, Screen.height * 0.75f, Screen.width, Screen.height * 0.2f);
//
//		//		AddNewResult(string.Format("Setting new frame: {0} for web view.", m_webview.Frame));
//	}
		
	private void ClearCache ()
	{
		m_webview.ClearCache ();

		//		AddNewResult("Cleared web view cache.");
	}
	#endregion

	#region Callback Methods

	private void DidShowEvent(WebView m_webview){
		Debug.Log ("DidShowEvent");
		acpTrackable = gameObject.GetComponent<ACPTrackableBehavior> ();
		AppManager.Instnace.messageBoxManager.HidePreloader ();
//		ACPUnityPlugin.Instnace.RemoveOldPreloader ();
		AppManager.Instnace.isDynamicDataLoaded = true;
		ACPUnityPlugin.Instnace.EnableButtons ();
		webViewIsLoaded = true;
	}

	private void DidHideEvent(WebView m_webview){
		Debug.Log ("DidHideEvent");
	}
	private void DidFailLoadWithErrorEvent(){
		Debug.Log ("DidFailLoadWithErrorEvent");
//		DestroyWebViewOnError ();
	}

	private void DidDestroyEvent (WebView _webview)
	{
		//		AddNewResult("Released web view instance.");
		webViewIsLoaded = false;
		Debug.Log("WebView - DidDestroyEvent");
	}

	private void DidStartLoadEvent (WebView _webview)
	{
		Debug.Log("WebView - DidStartLoadEvent");
//		AppManager.Instnace.messageBoxManager.ShowPreloaderDefaultWithBackGround ();
	}

	private void DidFinishLoadEvent (WebView _webview)
	{
		Debug.Log("WebView - DidFinishLoadEvent");
//		AppManager.Instnace.messageBoxManager.HidePreloader ();
	}

	private void DidFailLoadWithErrorEvent (WebView _webview, string _error)
	{
		Debug.Log("WebView - DidFailLoadWithErrorEvent");
//		DestroyWebViewOnError ();
	}

	private void DidFinishEvaluatingJavaScriptEvent (WebView _webview, string _result)
	{
		Debug.Log("WebView - DidFinishEvaluatingJavaScriptEvent");
	}

	private void DidReceiveMessageEvent (WebView _webview,  WebViewMessage _message)
	{
		Debug.Log("WebView - DidReceiveMessageEvent");
	}

	#endregion

	#region Viewa methods

	public void LoadWebUrl(
		ulong triggerId,
		int areaIndex,
		int frameIndex,
		int widgetIndex,
		string url, bool external, bool fullScreen, string shareTitle, string shareText, string shareUrl){

        webViewIsLoaded = true;

		Debug.Log ("WebViewPage:- LoadWebUrl - url :" + url);
        if (string.IsNullOrEmpty(url) || !url.Contains("://"))
        {
            AppManager.Instnace.messageBoxManager.ShowGenericPopup ("Alert", "Fail to load.", "Ok", DestroyWebView);
            return;
        }
        else
        {
            AppManager.Instnace.messageBoxManager.ShowPreloaderDefault();
            ACPUnityPlugin.Instnace.pauseVideo();
            CreateWebView(url);
            ACPUnityPlugin.Instnace.trackEvent("Web", "WebStart", "Start", 0);
            ACPUnityPlugin.Instnace.trackEvent("WebViewInternal", "Url:" + url, "Url", 0);
        }
	}

	public void DestroyWebView ()
	{
		webViewIsLoaded = false;
		AppManager.Instnace.messageBoxManager.HidePreloader ();
		ACPUnityPlugin.Instnace.RemoveOldPreloader ();
        ACPUnityPlugin.Instnace.trackEvent("Web", "WebEnd", "End", 0);
		Debug.Log("WebView - DestroyWebView");
        m_webview.Hide();
        //if (m_webview) {
        //	m_webview.Destroy ();
        //	m_webview = null;
        //          if (aWebView != null)
        //          {
        //              Destroy(aWebView);
        //          }
                  ClearCache();
        //}
	}

//	void DestroyWebViewOnError() {
//	
//		Debug.Log("WebView - DestroyWebViewOnError");
//		CallCloseWebView();
//	}

	//void CallCloseWebView(){
		
	//	AppManager.Instnace.acpTrackingManager.OnBackButtonTapped ();
	//}

    void CreateWebView(string url)
    {
        if (m_webview == null)
        {
            m_webview = m_webViewPrefab.GetComponent<WebView>();
            m_webview.AutoShowOnLoadFinish = false;
            m_webview.CanBounce = true;
            SetFrame();
        }

        SetFrame();
        m_webview.LoadRequest(url);
        m_webview.Show();
    }

    void LoadFileInWebView(string url, string a_filePath) {

        if (url.Contains(".pdf"))
        {
            Debug.Log("======================== @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@ >  Inside LoadFile " + a_filePath);
            string HtmlString = File.ReadAllText(a_filePath);
            m_webview.LoadHTMLString(HtmlString, null);
            //LoadFile(url);
        }
        else if (url.Contains(".jpg"))
        {
            m_webview.LoadFile(a_filePath, "image/png", null, null);
        }
    }
	#endregion

    public IEnumerator CacheFile(string a_url)
    {
        string Path = ReturnFilePath(a_url);
        File.Delete(Path);
        if (!File.Exists(Path))
        {
            WWW www = new WWW(a_url);
            yield return www;
            if (www.error == null)
            {
                Debug.Log("==========================> CacheFile =================>" + www.text);
                File.WriteAllText( Path, www.text);
                LoadFileInWebView(a_url, Path);
            }
        } else {

            LoadFileInWebView(a_url, Path);
        }
    }
    public string GetCachePath()
    {
        string path = Application.persistentDataPath + "/CacheFilePath/";

        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
        return path;
    }
    string ReturnFileName(string fileUrl)
    {
        string[] breakApart = fileUrl.Split('/');
        string str = breakApart[breakApart.Length - 1];
        string[] breakDot = str.Split('.');
        string fileName = breakDot[0];

        return fileName;
    }
    void LoadFile(string a_url) {
        StartCoroutine(CacheFile(a_url));
    } 
    string ReturnFilePath(string a_url) {

        string Path = GetCachePath() + ReturnFileName(a_url);
        return Path;
    }
}

