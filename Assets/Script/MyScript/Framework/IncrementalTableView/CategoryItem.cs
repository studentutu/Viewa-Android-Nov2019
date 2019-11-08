using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using frame8.ScrollRectItemsAdapter.IncrementalItemFetchExample2;
using OTPL.UI;
using UnityEngine.Networking;
using System.Text;
using SimpleJSON;
using System.IO;
using UnityEngine.EventSystems;

public class CategoryItem: MonoBehaviour {

	//public long id;
	//public string cloudId;
	//public bool isLoved;
	//public string CategoryId;
	//public Image history_image;
	//public Image history_icon;
	//public Text history_description;
	////public Button lovedButton;
 //   public Image lovedButtonImage;
	//public GameObject heartPrefab;
	
	//public MyScrollRect myScrollRect;

	//[HideInInspector]
	//public ItemModel myModel;
	//[HideInInspector]
	//public Sprite originalSprite;
	//[HideInInspector]
	//public Sprite newSprite;

	//List<CategoryDetail> categoryDetail = new List<CategoryDetail> ();

 //   bool mouseOver = false;
 //   bool mouseReleased = false;
 //   int m_itemIndex;

 //   Texture2D texture2D;
 //   Texture2D icontexture2D;
 //   Transform heartDestination;
 //   BottomBarPanel bottomBarPanel;

	//private void Start()
	//{
 //       texture2D = new Texture2D(128,128);
 //       icontexture2D = new Texture2D(128,128);
	//}

	//void OnEnable() {

	//	history_image.preserveAspect = true;
	//	history_icon.preserveAspect = true;
 //       bottomBarPanel = (BottomBarPanel)CanvasManager.Instnace.ReturnPanelManager(ePanelManager.BottomBarManager).ReturnPanel(ePanels.BottomBarPanel);
 //       heartDestination = bottomBarPanel.loveImage.transform;
	//}

	//void OnDisable() {
		
	//	//history_image.sprite = null;
	//	//history_icon.sprite = null;
	//	//history_description.text = "";
	//}



	//void OnDestroy() {
	//	categoryDetail.Clear ();
	//	categoryDetail = null;
	//	myModel = null;
	//	originalSprite = null;
	//	newSprite = null;
	//	myScrollRect = null;
	//	history_image = null;
	//	history_icon = null;
	//	history_description = null;
 //       lovedButtonImage = null;
	//	heartPrefab = null;
	//	heartDestination = null;
	//}

	//public void AddCategoryData(ItemModel itemModel, int itemIndex) {

	//	myModel = itemModel;
 //       m_itemIndex = itemIndex;
	//	history_description.text = itemModel.history_description;
	//	//lovedButton.transform.GetChild (0).GetComponent<Image> ().sprite = itemModel.is_loved ? newSprite : originalSprite;
 //       lovedButtonImage.sprite = itemModel.is_loved ? newSprite : originalSprite;
	//}

	//#region Loved Button

	//public void LoveSelected(){
		
	//	AppManager.Instnace.PlayButtonSoundWithVibration ();
	//	//Transform childTransform = lovedButton.transform.GetChild (0);
 //       if (lovedButtonImage.sprite == newSprite) {
 //           lovedButtonImage.sprite = originalSprite;
	//		myModel.is_loved = false;
	//	} else {
 //           StartCoroutine( ShowLovedAnimation (lovedButtonImage.transform));
 //           lovedButtonImage.sprite = newSprite;
	//		myModel.is_loved = true;
	//	}
	//	CallSingleLovedCapturedService ();
	//}

	//void CallSingleLovedCapturedService(){
	//	//AppManager.Instnace.messageBoxManager.ShowPreloaderDefault ();
	//	CategoryDetail catDetail = new CategoryDetail ();
	//	catDetail.EmailId = AppManager.Instnace.userEmail;
	//	catDetail.RegionId = AppManager.Instnace.regionId;
	//	catDetail.CategoryID = myModel.CategoryId;
	//	catDetail.HubTrigger = myModel.id;
	//	catDetail.CloudTriggerId = myModel.cloud_id;
	//	catDetail.IsLove = myModel.is_loved;
	//	catDetail.XmlLoveItemsDetails = "";
	//	categoryDetail.Add (catDetail);
	//	CategoryRoot catRoot = new CategoryRoot ();
	//	catRoot.LoveItemDetails = categoryDetail.ToArray ();
	//	string jsonString = catRoot.ReturnJsonString ();
	//	byte[] bodyRaw = Encoding.UTF8.GetBytes (jsonString);
 //       categoryDetail.Clear();

	//	WebService.Instnace.Post (AppManager.Instnace.baseURL + "/cloud/LoveResponseService.aspx", bodyRaw, "", LovedSendCallBack);
       
	//}

	//void LovedSendCallBack(UnityWebRequest response ){

	//	AppManager.Instnace.messageBoxManager.HidePreloader ();
 //       if (AppManager.Instnace.triggerHistoryMode == eTriggerHistoryMode.TriggerHistoryModeLoved)
 //       {
 //           //AppManager.Instnace.messageBoxManager.ShowPreloaderDefault();
 //           IncrementalItemFetch incrementalItemFetch = myScrollRect.gameObject.GetComponent<IncrementalItemFetch>();
 //           //incrementalItemFetch.backgroundImage.CrossFadeAlpha(0.0f, 0.1f, false);
 //           //incrementalItemFetch.ResetItemsInPanel();
 //           //incrementalItemFetch.RemoveItems(m_itemIndex, 1);
 //           //incrementalItemFetch.ChangeItemsCount(frame8.Logic.Misc.Visual.UI.ScrollRectItemsAdapter.ItemCountChangeMode.REMOVE, 1, m_itemIndex);
 //           incrementalItemFetch.ResetItemsInPanel();
 //       }
	//	Debug.Log ("LovedSendResponse :" + response.downloadHandler.text);
	//}
	//IEnumerator ShowLovedAnimation(Transform HeartTransform) {

	//	yield return new WaitForEndOfFrame ();

	//	GameObject heart = (GameObject) GameObject.Instantiate (heartPrefab, HeartTransform.position, HeartTransform.rotation);
	//	heart.transform.parent = heartDestination;
	//	heart.transform.position = HeartTransform.position;
 //       heart.transform.localScale = new Vector3(1f, 1f, 1);
	//	HeartPathFollower hpf = heart.GetComponent<HeartPathFollower> ();
	//	hpf.PathNode = new Transform[2];
	//	hpf.PathNode [0] = HeartTransform;
	//	hpf.PathNode [1] = heartDestination;
 //       //iTween.PunchScale (heart, new Vector3 (8, 8, 0), 2f);  //iTween.PunchScale (heart, new Vector3 (4, 4, 0), 1.0f);
	//	hpf.StartHeartAnimation ();
	//}
 //   #endregion


 //   //private void Update()
 //   //{
       
 //       //mouseOver = false;
       
 //       //Vector2 screenPoint = Input.mousePosition;

 //       //if (Input.touches.Length > 0)
 //       //{
 //       //    mouseOver = true;

 //       //    screenPoint = Input.touches[0].position;

 //       //    if (Input.touches[0].phase == TouchPhase.Ended)
 //       //    {
 //       //        mouseReleased = true;
 //       //    }
 //       //}

 //       //if (Input.GetMouseButton(0))
 //       //{
 //       //    mouseOver = true;
 //       //    mouseReleased = false;
 //       //}

 //       //if (Input.GetMouseButtonUp(0))
 //       //{
 //       //    mouseReleased = true;
 //       //}
 //   //}



 //   #region Category Button
 //   public void CategoryDetailClicked() {

 //       Debug.Log("myScrollRect.velocity Y:->>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>" + myScrollRect.velocity.y);

 //       if (myScrollRect.velocity.y <= 0.0f && myScrollRect.velocity.y > -0.18f)
 //       {


 //           //*********************************************************************************
 //           // Adding the selected CategoryDetail object for Loved selecton in Dynamic content
 //           //*********************************************************************************
 //           //AppManager.Instnace.PlayButtonSound();

 //           WebService.Instnace.catDetailParameter.EmailId = AppManager.Instnace.userEmail;
 //           WebService.Instnace.catDetailParameter.RegionId = AppManager.Instnace.regionId;
 //           WebService.Instnace.catDetailParameter.CategoryID = myModel.CategoryId;
 //           WebService.Instnace.catDetailParameter.CloudTriggerId = myModel.cloud_id;
 //           WebService.Instnace.catDetailParameter.IsLove = myModel.is_loved;
 //           WebService.Instnace.catDetailParameter.XmlLoveItemsDetails = "";
 //           WebService.Instnace.catDetailParameter.HubTrigger = myModel.id;


 //           string historyJsonString = "{\"history_title\":\"" + myModel.history_title + "\", \"history_description\": \"" + myModel.history_description + "\",\"history_image\":\"" + myModel.history_image + "\",\"history_icon\":\"" + myModel.history_icon + "\",\"keywords\": \"" + myModel.keywords + "\"}";
 //           string combine = "" + myModel.id + "|" + myModel.cloud_id + "|" + historyJsonString;

 //           if (AppManager.Instnace.acpTrackingManager != null)
 //               AppManager.Instnace.acpTrackingManager.CreateACPTrackable(combine);

 //           //if (!AppManager.Instnace.CheckTrigger(WebService.Instnace.catDetailParameter.HubTrigger.ToString()))
 //           //{
 //           //    AppManager.Instnace.GetQuestion();
 //           //}
 //           //AppManager.Instnace.isDynamicContentFromHub = true;

 //           BlankPanel blankPanel = (BlankPanel)CanvasManager.Instnace.ReturnPanelManager(ePanelManager.MainMenuPanelManager).ReturnPanel(ePanels.Blank_Panel);
 //           blankPanel.loveCallBack = loveCallBackFunction;
 //           CanvasManager.Instnace.ReturnPanelManager(ePanelManager.MainMenuPanelManager).NavigateToPanel(ePanels.Blank_Panel);
 //       }

	//}

 //   public void loveCallBackFunction(bool value) {

 //        //Transform childTransform = lovedButton.transform.GetChild(0);
 //       if (value == false)
 //       {
 //           lovedButtonImage.sprite = originalSprite;
 //           myModel.is_loved = false;
 //       }
 //       else
 //       {
 //           lovedButtonImage.sprite = newSprite;
 //           myModel.is_loved = true;
 //       }
 //   }
	//#endregion

}
	
