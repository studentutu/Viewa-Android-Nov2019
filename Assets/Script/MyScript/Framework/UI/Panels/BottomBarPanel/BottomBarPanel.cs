using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using OTPL.UI;
using ACP;
using UnityEngine.Networking;
using SimpleJSON;
using System.Linq;
using SQLite4Unity3d;
using OTPL.UI;
using OTPL.modal;
using frame8.ScrollRectItemsAdapter.IncrementalItemFetchExample2;
using UnityEngine.U2D;


public class BottomBarPanel : PanelBase {

	bool isToggle;
	Button prevButton;
	RectTransform rectTransform;
    public IncrementalItemFetch incrementalTableView;
   
    public SVGImage hubImage;
    public SVGImage loveImage;
    public SVGImage ScanImage;
    public SVGImage ShareImage;
    public SVGImage menuImage;

    public Button hubButton;
    public Button loveButton;
    public Button scanButton;
    public Button shareButton;
    public Button menuButton;

    public GameObject mobileNotificationIcon;

	protected override void Awake ()
	{
		base.Awake ();
	}


    protected override void Start() {

        base.Start();

		if (AppManager.Instnace.isIphoneX) {
			rectTransform = gameObject.GetComponent<RectTransform> ();
			rectTransform.sizeDelta = new Vector2 (rectTransform.sizeDelta.x, rectTransform.sizeDelta.y + 30f);
		}

        //hubImage.sprite = AppManager.Instnace.spriteAtlas.GetSprite("V_logo_white");
        //loveImage.sprite = AppManager.Instnace.spriteAtlas.GetSprite("BtmNav_Loved_White");
        //ScanImage.sprite = AppManager.Instnace.spriteAtlas.GetSprite("BtmNav_Scan_White");
        //ShareImage.sprite = AppManager.Instnace.spriteAtlas.GetSprite("BtmNav_Share_White");
        //menuImage.sprite = AppManager.Instnace.spriteAtlas.GetSprite("BtmNav_Menu_White");

        scanButton.transform.GetChild(0).GetComponent<SVGImage>().color = ACP.Utility.HexToColor("0093FF");
	}

	protected override void OnEnable ()
	{
		base.OnEnable ();
        mobileNotificationIcon.SetActive(false);
	}
	protected override void OnDisable ()
	{
		StopAllCoroutines ();
		base.OnDisable ();
	}
    #region Button Listners
    protected override void OnUIButtonClicked(UnityEngine.UI.Button a_button)
    {
		base.OnUIButtonClicked (a_button);


        //iTween.ScaleFrom(a_button.transform.GetChild(0).gameObject, new Vector3(0.15f, 0.15f, 0), 0.1f);
		//AppManager.Instnace.PlayButtonSoundWithVibration ();
        CanvasManager.Instnace.ReturnPanelManager(ePanelManager.MainMenuPanelManager).panelStack.Clear();

		//foreach (Button button in buttons) {
		//	button.transform.GetChild(0).GetComponent<SVGImage> ().color = Color.black;
		//}
			
		//if (CanvasManager.Instnace.ReturnPanelManager (ePanelManager.MainMenuPanelManager).ReturnPanelStatus (ePanels.Blank_Panel)
			//|| CanvasManager.Instnace.ReturnPanelManager (ePanelManager.MainMenuPanelManager).ReturnPanelStatus (ePanels.Scan_Panel)
			//|| CanvasManager.Instnace.ReturnPanelManager(ePanelManager.MainMenuPanelManager).ReturnPanelStatus(ePanels.ImageGallary_Panel)) {

            if (a_button.name != "ShareButton" && AppManager.Instnace.isDynamicDataLoaded) { //|| a_button.name != "SideMenuButton") {
				//call the OnCloseFromBottomBar
                if (AppManager.Instnace.webViewPage.webViewIsLoaded)
                    AppManager.Instnace.webViewPage.DestroyWebView();
                AppManager.Instnace.messageBoxManager.HidePreloader();
				AppManager.Instnace.acpTrackingManager.DestroyTrackableData ();
            }
		//}

		//On every button click make the buttons color black initially. 
		foreach (Button button in buttons) {
			
			button.transform.GetChild(0).GetComponent<SVGImage> ().color = Color.black;
		}

        if (AppManager.Instnace.isIncrementalLoaded && a_button.name != "ShareButton")
        {
            AppManager.Instnace.isIncrementalLoaded = false;
            incrementalTableView.ResetItemsInPanel();
        }

        switch (a_button.name)
        {
		case "HubButton":
			
                Debug.Log ("Button selected -" + a_button.name);
			//AppManager.Instnace.messageBoxManager.ShowPreloaderDefault ();
                hubButton.transform.GetChild(0).GetComponent<SVGImage>().color = ACP.Utility.HexToColor("0093FF");
			//a_button.transform.GetChild(0).GetComponent<SVGImage> ().color = ACP.Utility.HexToColor ("0093FF");
			    AppManager.Instnace.categoryName = "Hub";
                //incrementalTableView.backgroundImage.CrossFadeAlpha(0.0f, 0.1f, false);
                CanvasManager.Instnace.ReturnPanelManager (ePanelManager.MainMenuPanelManager).NavigateToPanel (ePanels.Hub_Panel); //HubDetail_Panel
			break;

		case "LovedButton":
                
			Debug.Log ("Button selected -" + a_button.name);
			//AppManager.Instnace.messageBoxManager.ShowPreloaderDefault ();
                loveButton.transform.GetChild(0).GetComponent<SVGImage>().color = ACP.Utility.HexToColor("0093FF");
			//a_button.transform.GetChild(0).GetComponent<SVGImage> ().color = ACP.Utility.HexToColor ("0093FF");
			AppManager.Instnace.triggerHistoryMode = eTriggerHistoryMode.TriggerHistoryModeLoved;
			AppManager.Instnace.categoryName = "Loved";
                HubDetailPanel ahubDetailPanel = (HubDetailPanel)CanvasManager.Instnace.ReturnPanelManager(ePanelManager.MainMenuPanelManager).ReturnPanel(ePanels.HubDetail_Panel);
                //ahubDetailPanel.LeftButton.SetActive(true);
                ahubDetailPanel.navigationTitle.text = AppManager.Instnace.categoryName;

                //incrementalTableView.backgroundImage.CrossFadeAlpha(0.0f, 0.1f, false);
                CanvasManager.Instnace.ReturnPanelManager (ePanelManager.MainMenuPanelManager).NavigateToPanel (ePanels.HubDetail_Panel);

			break;

		case "ScanButton":
                
			Debug.Log ("Button selected -" + a_button.name);
           
            
                scanButton.transform.GetChild(0).GetComponent<SVGImage>().color = ACP.Utility.HexToColor("0093FF");
			//a_button.transform.GetChild(0).GetComponent<SVGImage> ().color = ACP.Utility.HexToColor ("0093FF");
			CanvasManager.Instnace.ReturnPanelManager(ePanelManager.MainMenuPanelManager).NavigateToPanel(ePanels.Scan_Panel);
			break;

		case "ShareButton":
			Debug.Log ("Button selected -" + a_button.name);

			if (AppManager.Instnace.isDynamicDataLoaded) {
				AppManager.Instnace.socialSharingScript.ShareDynamicContentOnSocialPlatform (ACPUnityPlugin.Instnace._shareText, ACPUnityPlugin.Instnace._shareUrl);
			} else {
				AppManager.Instnace.socialSharingScript.ShareContentOnSocialPlatform ();
			}

            break;

		case "SideMenuButton":
                
			Debug.Log ("Button selected -" + a_button.name);
                menuButton.transform.GetChild(0).GetComponent<SVGImage>().color = ACP.Utility.HexToColor("0093FF");
                CanvasManager.Instnace.ReturnPanelManager(ePanelManager.MainMenuPanelManager).NavigateToPanel(ePanels.SideMenuPanel);
	
			//if (!CanvasManager.Instnace.ReturnPanelManager(ePanelManager.MainMenuPanelManager).ReturnPanelStatus(ePanels.SideMenuPanel)) {
   //                 menuButton.transform.GetChild(0).GetComponent<SVGImage>().color = ACP.Utility.HexToColor("0093FF");
   //                 //a_button.transform.GetChild(0).GetComponent<SVGImage> ().color = ACP.Utility.HexToColor("0093FF");
			//	CanvasManager.Instnace.ReturnPanelManager(ePanelManager.MainMenuPanelManager).NavigateToPanel(ePanels.SideMenuPanel);
			//} else {
			//	CanvasManager.Instnace.ReturnPanelManager (ePanelManager.MainMenuPanelManager).HidePanel (ePanels.SideMenuPanel);
			//}
            break;
        }
        if (WebService.Instnace.appUser.Mobile.Length <= 0){             mobileNotificationIcon.SetActive(true);         }         else{             mobileNotificationIcon.SetActive(false);         }
    }



    public void OnHubDetailLeftButtonClicked() {

        incrementalTableView.ResetItemsInPanel();
        loveButton.transform.GetChild(0).GetComponent<SVGImage>().color = Color.black;
        hubButton.transform.GetChild(0).GetComponent<SVGImage>().color = ACP.Utility.HexToColor("0093FF");
        CanvasManager.Instnace.ReturnPanelManager(ePanelManager.MainMenuPanelManager).BackToPanel(ePanels.Hub_Panel);

    //    HubDetailPanel ahubDetailPanel = (HubDetailPanel)CanvasManager.Instnace.ReturnPanelManager(ePanelManager.MainMenuPanelManager).ReturnPanel(ePanels.HubDetail_Panel);
    //    ahubDetailPanel.LeftButton.SetActive(false);
    //    ahubDetailPanel.navigationTitle.text = AppManager.Instnace.categoryName;
    //    //incrementalTableView.backgroundImage.CrossFadeAlpha(0.0f, 0.1f, false);
    //    incrementalTableView.backgroundImage.CrossFadeAlpha(0.0f, 0.1f, false);
    //    CanvasManager.Instnace.ReturnPanelManager(ePanelManager.MainMenuPanelManager).NavigateToPanel(ePanels.HubDetail_Panel);
    }

    #endregion //Button Listners
}


