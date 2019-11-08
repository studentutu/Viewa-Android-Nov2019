using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OTPL.UI;
using UnityEngine.UI;
using System.Linq;
using OTPL.modal;
using UnityEngine.Networking;
using System.Text;
using SimpleJSON;
using System.IO;
using frame8.ScrollRectItemsAdapter.IncrementalItemFetchExample2;
using ACP;
using Vuforia;


public class HubDetailPanel : PanelBase {

//	public GameObject CategoryDetailPrefab;
//	public GameObject parentObject;
//	public Text navigationTitle;
//	public GameObject heartPrefab;
//	public Transform heartDestination;
    public IncrementalItemFetch incrementalTableView;
	//public RectTransform categoryDetailPrefab;
	public RectTransform viewPort;
	public Text navigationTitle;
    public GameObject LeftButton;
    public string desc;
    Button[] buttonArray;


	protected override void Awake ()
	{
		base.Awake ();	
        SVGImage buttonImage = LeftButton.transform.GetChild(0).GetComponent<SVGImage>();
        buttonImage.sprite = AppManager.Instnace.spriteAtlas.GetSprite("TopNav_BackArrow_White"); 
	}

	protected override void OnEnable ()
	{
		base.OnEnable ();

        incrementalTableView._Fetching = false;

		navigationTitle.text = AppManager.Instnace.categoryName;
        //AppManager.Instnace.acpTrackingManager.mFocusMode = CameraDevice.FocusMode.FOCUS_MODE_NORMAL;
//		WebService.Instnace.InternetConnectivity ();
		buttonArray = null;

		CanvasManager.Instnace.ShowPanelManager (ePanelManager.BottomBarManager);

        incrementalTableView.isResetItem = false;
        //Tracking
        if (AppManager.Instnace.triggerHistoryMode == eTriggerHistoryMode.TriggerHistoryModeChannel)
        {
            LeftButton.SetActive(true);
            incrementalTableView.staticPanel.SetActive(true);
            incrementalTableView.staticPanel.transform.Find("parent/headText").GetComponent<Text>().text = navigationTitle.text;
            incrementalTableView.staticPanel.transform.Find("parent/descText").GetComponent<Text>().text = desc;                    
            incrementalTableView.transform.GetComponent<RectTransform>().offsetMax = new Vector2(0, -120); //-128
            ACPUnityPlugin.Instnace.trackScreen("HubScreen");
            ACPUnityPlugin.Instnace.trackEvent("Hub", "HubScreenStart", "", 1);
        } 
        else if(AppManager.Instnace.triggerHistoryMode == eTriggerHistoryMode.TriggerHistoryModeLoved)
        {
            LeftButton.SetActive(false);
            incrementalTableView.staticPanel.SetActive(false);
            transform.GetComponent<RectTransform>().offsetMax = new Vector2(0, 0);
            ACPUnityPlugin.Instnace.trackScreen("LovedScreen");
            ACPUnityPlugin.Instnace.trackEvent("Loved", "LovedScreenStart", "", 1);
        }
        else if(AppManager.Instnace.triggerHistoryMode == eTriggerHistoryMode.TriggerHistoryModeRecent)
        {
            LeftButton.SetActive(true);
            incrementalTableView.staticPanel.SetActive(false);
            transform.GetComponent<RectTransform>().offsetMax = new Vector2(0, 0);
            ACPUnityPlugin.Instnace.trackScreen("RecentScreen");
            ACPUnityPlugin.Instnace.trackEvent("Recent", "RecentScreenStart", "", 1);
        }
	}

	protected override void OnDisable() {

//		transform.GetChild (1).gameObject.SetActive (false);
		StopAllCoroutines ();

		if (buttonArray != null && buttonArray.Length > 0) {
			foreach (Button button in buttonArray) {
				button.onClick.RemoveAllListeners ();
			}
		}
        incrementalTableView.transform.GetComponent<RectTransform>().offsetMax = new Vector2(0, 0);

//		if (Directory.Exists(incrementalTableView.GetImageCachePath ())) {
//			for(int i = 0; i< 1000; i++) {
//				if (File.Exists (incrementalTableView.GetImageCachePath () + i)) {
//					Debug.Log ("Image i :"+ i);
//					File.Delete (incrementalTableView.GetImageCachePath () + i);
//				}
//			}
//			Directory.Delete (incrementalTableView.GetImageCachePath ());
//		}
//		if (Directory.Exists (incrementalTableView.GetIconCachePath ())) {
//
//			for(int i = 0; i< 1000; i++) {
//				if (File.Exists (incrementalTableView.GetIconCachePath () + i)) {
//					Debug.Log ("Icon i :"+ i);
//					File.Delete (incrementalTableView.GetIconCachePath () + i);
//				}
//			}
//			Directory.Delete (incrementalTableView.GetIconCachePath ());
//		}
		Resources.UnloadUnusedAssets ();
		base.OnDisable ();
        if (AppManager.Instnace.triggerHistoryMode == eTriggerHistoryMode.TriggerHistoryModeChannel)
        {
            ACPUnityPlugin.Instnace.trackEvent("Hub", "HubScreenEnd", "", 0);
        } 
        else if (AppManager.Instnace.triggerHistoryMode == eTriggerHistoryMode.TriggerHistoryModeLoved) 
        {
            ACPUnityPlugin.Instnace.trackEvent("Loved", "LovedScreenEnd", "", 0);
        }
        else if(AppManager.Instnace.triggerHistoryMode == eTriggerHistoryMode.TriggerHistoryModeRecent)
        {
            ACPUnityPlugin.Instnace.trackEvent("Recent", "RecentScreenEnd", "", 0);
        }
	}

	protected override void OnUIButtonClicked (UnityEngine.UI.Button a_button)
	{
        //base.OnUIButtonClicked(a_button);

		switch (a_button.name) {
		case "LeftButton":
                //return;
                OnLeftButtonClicked();
			break;
		}
		
	}

    public void OnLeftButtonClicked() {
        
        //incrementalTableView.backgroundImage.CrossFadeAlpha(0.0f, 0.1f, false);
        incrementalTableView.staticPanel.SetActive(false);
        BottomBarPanel bottomBarPanel = (BottomBarPanel)CanvasManager.Instnace.ReturnPanelManager(ePanelManager.BottomBarManager).ReturnPanel(ePanels.BottomBarPanel);
        bottomBarPanel.OnHubDetailLeftButtonClicked();
    }

	//private void Update()
	//{
 //       if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.OSXEditor)
 //       {
 //           if (Input.GetKey(KeyCode.Escape)|| Input.GetKey(KeyCode.Y))
 //           {
 //               OnLeftButtonClicked();
 //           }
 //       }
	//}
}
//********************************************************************
//For Loved Send Service 
//********************************************************************

[System.Serializable]
public class CategoryRoot {

	public  CategoryDetail [] LoveItemDetails;

	public string ReturnJsonString()
	{
		return JsonUtility.ToJson(this);
	}
}

[System.Serializable]
public class CategoryDetail {

	public string EmailId;
	public long RegionId;
	public string CategoryID;
	public long HubTrigger;  //id
	public string CloudTriggerId; //cloud_id
	public bool IsLove;
	public string XmlLoveItemsDetails;
	//	public string cloud_id;
	//	public string history_title;
	//	public string keywords;
	//	public string name;
	//	public string image;
	//	public long channel_id;
	//	public string channel_name;
	//	public bool is_last;
}