using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using OTPL.UI;
using System.Text;
using UnityEngine.Networking;
using Vuforia;
using frame8.ScrollRectItemsAdapter.IncrementalItemFetchExample2;
using OTPL.modal;

public class BlankPanel : PanelBase
{

    public UnityEngine.UI.Image progressImage;
    //Button variables
    public Button LovedButton;
    public Button backButton;
    public Text history_Title;
    public UnityEngine.UI.Image history_Icon;
    public GameObject heartPrefab;

    public delegate void LoveCallBack(bool val);
    public LoveCallBack loveCallBack;
    public IncrementalItemFetch incrementalTableView;
    public GameObject LeftButton;


    Sprite originalSprite;
    Sprite newSprite;
    List<CategoryDetail> categoryDetail = new List<CategoryDetail>();
    Transform heartDestination;
    BottomBarPanel bottomBarPanel;

	protected override void Awake ()
	{
		base.Awake ();
        originalSprite = ACPUnityPlugin.Instnace.originalSprite;
        newSprite = ACPUnityPlugin.Instnace.newSprite;
        SVGImage buttonImage = LeftButton.transform.GetChild(0).GetComponent<SVGImage>();
        buttonImage.sprite = AppManager.Instnace.spriteAtlas.GetSprite("TopNav_BackArrow_White"); 
	}

	protected override void OnEnable ()
	{
		base.OnEnable ();
		progressImage.fillAmount = 0;
		AppManager.Instnace.mDownloadProgress = 0;
		categoryDetail.Clear ();

        if(loveCallBack != null) {
            SetLovedButton(WebService.Instnace.catDetailParameter.IsLove, LovedButton);
        }
        bottomBarPanel = (BottomBarPanel)CanvasManager.Instnace.ReturnPanelManager(ePanelManager.BottomBarManager).ReturnPanel(ePanels.BottomBarPanel);
        heartDestination = bottomBarPanel.loveImage.transform;
        AppManager.Instnace.isDynamicContentFromHub = true;

        StartCoroutine(DelayFortime(1.0f));
	}

    IEnumerator DelayFortime(float sec) {
        
        yield return new WaitForSeconds(1.0f);

        HistoryData historyData = WebService.Instnace.historyData;
        string historyJsonString = "{\"history_title\":\"" + historyData.history_Title + "\", \"history_description\": \"" + historyData.history_Description + "\",\"history_image\":\"" + historyData.history_Image + "\",\"history_icon\":\"" + historyData.history_icon + "\",\"keywords\": \"" + historyData.history_keyword + "\"}";
        string combine = "" + WebService.Instnace.catDetailParameter.HubTrigger + "|" + WebService.Instnace.catDetailParameter.CloudTriggerId + "|" + historyJsonString;

        if (AppManager.Instnace.acpTrackingManager != null)
            AppManager.Instnace.acpTrackingManager.CreateACPTrackable(combine);
    }

	protected override void OnDisable ()
	{
		StopAllCoroutines ();
		progressImage.fillAmount = 0;
		//WebService.Instnace.historyData.history_icon = "";
		//LovedButton.gameObject.SetActive (false);
		AppManager.Instnace.isDynamicContentFromHub = false;
        //		CanvasManager.Instnace.ShowPanelManager (ePanelManager.BottomBarManager);
        System.GC.Collect();
		base.OnDisable ();
	}		

	protected override void OnUIButtonClicked (UnityEngine.UI.Button a_button)
	{

		switch(a_button.name){

		case "LeftButton":
			Debug.Log ("Content Button -" + a_button.name);
			AppManager.Instnace.PlayButtonSoundWithVibration ();
			progressImage.fillAmount = 0;
       //     if (AppManager.Instnace.webViewPage.webViewIsLoaded)
			    //AppManager.Instnace.webViewPage.DestroyWebView ();
			AppManager.Instnace.acpTrackingManager.OnBackButtonTapped ();
			break;
//		case "HomeButton":
//			Debug.Log ("Content Button -" + a_button.name);
//			progressImage.fillAmount = 0;
//			AppManager.Instnace.acpTrackingManager.OnHomeButtonTapped ();
//			break;
		case "ShareButton":
			Debug.Log ("Content Button -" + a_button.name);
//			AppManager.Instnace.socialSharingScript.ShareDynamicContentOnSocialPlatform ();
			break;
		case "LovedButton":
			Debug.Log ("Button selected -" + a_button.name);
			AppManager.Instnace.PlayButtonSoundWithVibration ();
			Transform childTransform = a_button.transform.GetChild (0);
                if (childTransform.GetComponent<SVGImage>().sprite == newSprite) {
                    childTransform.GetComponent<SVGImage> ().sprite = originalSprite;
				WebService.Instnace.catDetailParameter.IsLove = false;
                    if (loveCallBack != null)
                    {
                        loveCallBack(false);
                    }
			} else {
				StartCoroutine( ShowLovedAnimation (childTransform));
                    childTransform.GetComponent<SVGImage> ().sprite = newSprite;
				WebService.Instnace.catDetailParameter.IsLove = true;
                    if (loveCallBack != null)
                    {
                        loveCallBack(true);
                    }
			}
			CallSingleLovedCapturedService ();
			break;
		}
//		base.OnUIButtonClicked (a_button);
	}

	void CallSingleLovedCapturedService(){
		//AppManager.Instnace.messageBoxManager.ShowPreloaderDefault ();
		categoryDetail.Add (WebService.Instnace.catDetailParameter);
		CategoryRoot catRoot = new CategoryRoot ();
		catRoot.LoveItemDetails = categoryDetail.ToArray ();
		string jsonString = catRoot.ReturnJsonString ();
		byte[] bodyRaw = Encoding.UTF8.GetBytes (jsonString);
		WebService.Instnace.Post (AppManager.Instnace.baseURL + "/cloud/LoveResponseService.aspx", bodyRaw, "", LovedSendCallBack);
	}
	void LovedSendCallBack(UnityWebRequest response ){

		AppManager.Instnace.messageBoxManager.HidePreloader ();
		Debug.Log ("LovedSendCallBack :" + response.downloadHandler.text);
	}
	void SetLovedButton(bool isLoved, Button loveButton){

		Transform childTransform = loveButton.transform.GetChild (0);
		if (!isLoved) {
            childTransform.GetComponent<SVGImage> ().sprite = originalSprite;
            loveCallBack(false);
		} else {
            childTransform.GetComponent<SVGImage> ().sprite = newSprite;
            loveCallBack(true);
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
	
}
