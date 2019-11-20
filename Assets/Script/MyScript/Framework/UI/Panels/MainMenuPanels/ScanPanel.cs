using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OTPL.UI;
using UnityEngine.UI;
using System.Text;
using UnityEngine.Networking;
using Vuforia;

public class ScanPanel : PanelBase {

	public UnityEngine.UI.Image progressImage;
	public float speed = 0.1f;
	public Text progressStatusText;

    //Button variables
    public Button ShareButton;
    public Button LovedButton;
	public Button backButton;
	public GameObject scanTextPanel;
	public Text history_Title;
	public UnityEngine.UI.Image history_Icon;
	public GameObject heartPrefab;
    [SerializeField] UnityEngine.UI.Image backgroundImage;
    [SerializeField] Sprite originalSprite;
    [SerializeField] Sprite newSprite;

	RectTransform m_scanTextRectTransform;
	
	List<CategoryDetail> categoryDetail = new List<CategoryDetail> ();
    Camera mainCamera;
    Transform heartDestination;
    BottomBarPanel bottomBarPanel;

	protected override void Awake ()
	{
		base.Awake ();
		backButton.gameObject.SetActive (false);
//		homeButton.gameObject.SetActive (false);
//		shareButton.gameObject.SetActive (false);
	
	}
	protected override void Start ()
	{
		base.Start ();


		if (AppManager.Instnace.isIphoneX) {
			m_scanTextRectTransform = scanTextPanel.GetComponent<RectTransform> ();
			m_scanTextRectTransform.localPosition = new Vector3 (0, m_scanTextRectTransform.localPosition.y + 30, 0);
		}

        Transform childTransform = LovedButton.transform.GetChild(0);
        childTransform.GetComponent<SVGImage>().sprite = originalSprite;
        Loader.Instnace.HideSocialLoader();
	}
    protected override void OnEnable()
    {
        base.OnEnable();

        backgroundImage.CrossFadeAlpha(0.0f, 1.0f, false);

        originalSprite = ACPUnityPlugin.Instnace.originalSprite;
        newSprite = ACPUnityPlugin.Instnace.newSprite;

        AppManager.Instnace.isLoggedInAndInside = true;
        //Jeetesh - flashing 28 May 2019
        SwitchOnVuforia();

        progressImage.fillAmount = 0;
        AppManager.Instnace.mDownloadProgress = 0;

        GameObject[] trackables = GameObject.FindGameObjectsWithTag("Trackable");
        if (trackables.Length == 0)
        {
            ACPUnityPlugin.Instnace.RemoveOldPreloader();
            //Jeetesh - flashing 28 May 2019
            AppManager.Instnace.acpTrackingManager.EnableScanning();
            LovedButton.gameObject.SetActive(false);
            backButton.gameObject.SetActive(false);
            history_Icon.gameObject.SetActive(false);
            categoryDetail.Clear();
            SetLovedButton(false);
        }
        mainCamera = Camera.main;

        if (mainCamera != null)
        {
            Camera.main.clearFlags = CameraClearFlags.SolidColor;
        }

        if (AppManager.Instnace.pushNotificationController.isPushNotificationShown)
        {

            AppManager.Instnace.acpTrackingManager.m_ScanLine.ShowScanLine(false);
            string combineStr = AppManager.Instnace.pushNotificationController.jsonCombine;
            AppManager.Instnace.acpTrackingManager.CreateACPTrackable(combineStr);
        }

        //tracking
        ACPUnityPlugin.Instnace.trackScreen("Scan");
        ACPUnityPlugin.Instnace.trackEvent("Scan", "ScanScreenStart", "", 1);
        bottomBarPanel = (BottomBarPanel)CanvasManager.Instnace.ReturnPanelManager(ePanelManager.BottomBarManager).ReturnPanel(ePanels.BottomBarPanel);
        heartDestination = bottomBarPanel.loveImage.transform;
        if (WebService.Instnace.appUser.Mobile.Length <= 0)
        {
            bottomBarPanel.mobileNotificationIcon.SetActive(true);
        }
        else
        {
            bottomBarPanel.mobileNotificationIcon.SetActive(false);
        }

        Loader.Instnace.HideSocialLoader();
    }

    protected override void OnDisable()
    {
        StopAllCoroutines();
        //if(AppManager.Instnace.acpTrackingManager != null){
        //    AppManager.Instnace.acpTrackingManager.DisableScanning();
        //}

        //AppManager.Instnace.acpTrackingManager.DisableVuforia();
        if(AppManager.Instnace.pushNotificationController != null){
            AppManager.Instnace.pushNotificationController.isPushNotificationShown = false;
        }

        progressImage.fillAmount = 0;
        if(mainCamera!= null){
            Camera.main.clearFlags = CameraClearFlags.Skybox;
        }

        AppManager.Instnace.isVuforiaOn = false;
        //      scanTextPanel.gameObject.SetActive (false);
        base.OnDisable();
        ACPUnityPlugin.Instnace.trackEvent("Scan", "ScanScreenEnd", "", 0);
    }
	void SwitchOnVuforia() {
		AppManager.Instnace.isVuforiaOn = true;
//		scanTextPanel.gameObject.SetActive (true);
		CanvasManager.Instnace.ShowPanelManager (ePanelManager.BottomBarManager);
	}


	protected override void OnUIButtonClicked (UnityEngine.UI.Button a_button)
	{
		
		switch(a_button.name){

		case "BackButton":
			Debug.Log ("Content Button -" + a_button.name);
			progressImage.fillAmount = 0;
            //if (AppManager.Instnace.webViewPage.webViewIsLoaded)
			    //AppManager.Instnace.webViewPage.DestroyWebView ();
			AppManager.Instnace.acpTrackingManager.OnBackButtonTapped ();
			break;
//		case "HomeButton":
//			Debug.Log ("Content Button -" + a_button.name);
//			OnHomeButtonClicked ();
//			break;
		case "ShareButton":
                //Jeetesh - Below line is to test the Android plugin test.
                CameraDevice.Instance.SetFlashTorchMode(true);
                
                //PluginTest.CallPluginMethod();
                //			Debug.Log ("Content Button -" + a_button.name);
                //			AppManager.Instnace.socialSharingScript.ShareDynamicContentOnSocialPlatform ();
                break;
		case "LovedButton":
			Debug.Log ("Button selected -" + a_button.name);
			AppManager.Instnace.PlayButtonSoundWithVibration ();

			Transform childTransform = a_button.transform.GetChild (0);
            if (childTransform.GetComponent<SVGImage>().sprite.name == newSprite.name) {
				childTransform.GetComponent<SVGImage> ().sprite = originalSprite;
				WebService.Instnace.catDetailParameter.IsLove = false;
			} else {
				StartCoroutine( ShowLovedAnimation (childTransform));
				childTransform.GetComponent<SVGImage> ().sprite = newSprite;
				WebService.Instnace.catDetailParameter.IsLove = true;
			}
			CallSingleLovedCapturedService ();
			break;

		}				
//		base.OnUIButtonClicked (a_button);
	}

	void CallSingleLovedCapturedService(){
//		AppManager.Instnace.messageBoxManager.ShowPreloaderDefault ();
		WebService.Instnace.catDetailParameter.CategoryID = "-1";
		categoryDetail.Add (WebService.Instnace.catDetailParameter);
		CategoryRoot catRoot = new CategoryRoot ();
		catRoot.LoveItemDetails = categoryDetail.ToArray ();
		string jsonString = catRoot.ReturnJsonString ();
		byte[] bodyRaw = Encoding.UTF8.GetBytes (jsonString);
		Debug.Log ("Json String Love: " + jsonString);
		WebService.Instnace.Post (AppManager.Instnace.baseURL + "/cloud/LoveResponseService.aspx", bodyRaw, "", LovedSendCallBack);
	}
	void LovedSendCallBack(UnityWebRequest response ){

//		AppManager.Instnace.messageBoxManager.HidePreloader ();
		Debug.Log ("LovedSendCallBack :" + response.downloadHandler.text);
	}
	public void SetLovedButton(bool isLoved){

		Transform childTransform = LovedButton.transform.GetChild (0);
		if (isLoved) {
            childTransform.GetComponent<SVGImage>().sprite = newSprite;
		} else {
            childTransform.GetComponent<SVGImage>().sprite = originalSprite;
		}
	}
	IEnumerator ShowLovedAnimation(Transform HeartTransform) {

		yield return new WaitForEndOfFrame ();

		GameObject heart = (GameObject) GameObject.Instantiate (heartPrefab, HeartTransform.position, HeartTransform.rotation);
		heart.transform.parent = heartDestination;
		heart.transform.position = HeartTransform.position;
        heart.transform.localScale = new Vector3(1f, 1f, 1);
		HeartPathFollower hpf = heart.GetComponent<HeartPathFollower> ();
		hpf.PathNode = new Transform[2];
		hpf.PathNode [0] = HeartTransform;
		hpf.PathNode [1] = heartDestination;
        //iTween.PunchScale(heart, new Vector3(8, 8, 0), 1.5f);  //iTween.PunchScale (heart, new Vector3 (4, 4, 0), 1.0f);
		hpf.StartHeartAnimation ();
	}
//	public void OnHomeButtonClicked(){
//		progressImage.fillAmount = 0;
//		Debug.Log ("OnBackButtonClicked");
//		AppManager.Instnace.acpTrackingManager.OnHomeButtonTapped ();
//	}

    //private void Update()
    //{
        //if (Application.platform == RuntimePlatform.Android)
        //{
        //    if (!ACPUnityPlugin.Instnace.IsBackButtonVisible)
        //    {
        //        if (Input.GetKey(KeyCode.Escape))
        //        {
        //            //Show a dialog box and exit the app here if user press device back button here.
        //            AppManager.Instnace.messageBoxManager.ShowMessageWithTwoButtons("Alert", "Are you sure you want to exit the app?", "OK", "Cancel", AppManager.Instnace.ExitApplication);
        //        }
        //    }
        //}
    //}
}
