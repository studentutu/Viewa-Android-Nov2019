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
using SQLite4Unity3d;

public class LovedPanel : PanelBase {

	public GameObject CategoryDetailPrefab;
	public GameObject parentObject;
	public Text navigationTitle;
	public GameObject heartPrefab;
    public Transform heartDestination;
	public Image backgroundImage;


	Button[] buttonArray;
	List<CategoryDetailWebService> catLovedDetailList = new List<CategoryDetailWebService>();
	List<CategoryDetailContainer> catDetailContainerList = new List<CategoryDetailContainer> ();
	List<CategoryDetail> categoryDetail = new List<CategoryDetail> ();
	Sprite originalSprite;
	Sprite newSprite;
	bool isErrorWhileLoadingTexture;
    BottomBarPanel bottomBarPanel;


	protected override void Awake ()
	{
		base.Awake ();

        originalSprite = ACPUnityPlugin.Instnace.originalSprite;
        newSprite = ACPUnityPlugin.Instnace.newSprite;
	}

	protected override void OnEnable ()
	{
		base.OnEnable ();

		//backgroundImage.CrossFadeAlpha (0.0f, 0.1f, false);

//		WebService.Instnace.InternetConnectivity ();
		categoryDetail.Clear ();
		catDetailContainerList.Clear ();
		buttonArray = null;
		//ACPUnityPlugin.Instnace.CreateOldPreloader ();
//		AppManager.Instnace.messageBoxManager.ShowPreloaderDefault ();
		CanvasManager.Instnace.ShowPanelManager (ePanelManager.BottomBarManager);
		navigationTitle.text = AppManager.Instnace.categoryName;
		string url = AppManager.Instnace.baseURL + "/cloud/LoveService.aspx?" + "emailId=" + AppManager.Instnace.userEmail + "&c=10&rid=" +AppManager.Instnace.regionId;
		WebService.Instnace.Post (url, null, "", LovedFetchCallBack);

        //tracking
        ACPUnityPlugin.Instnace.trackScreen("LovedPanel");
        bottomBarPanel = (BottomBarPanel)CanvasManager.Instnace.ReturnPanelManager(ePanelManager.BottomBarManager).ReturnPanel(ePanels.BottomBarPanel);
        heartDestination = bottomBarPanel.loveImage.transform;
	} 

	protected override void OnDisable() {

		StopAllCoroutines ();

		//backgroundImage.CrossFadeAlpha (0.0f, 0.1f, false);

		if (buttonArray != null && buttonArray.Length > 0) {
			foreach (Button button in buttonArray) {
				button.onClick.RemoveAllListeners ();
			}
		}
		if (catDetailContainerList.Count > 0) {
			foreach (CategoryDetailContainer catDetail in catDetailContainerList) {
				Destroy (catDetail.gameObject);
			}

			if (catDetailContainerList != null)
				catDetailContainerList.Clear ();
			buttonArray = null;
		}
		if (catLovedDetailList != null)
			catLovedDetailList.Clear ();

		StopAllCoroutines ();

		base.OnDisable ();
	}
	protected override void OnUIButtonClicked (UnityEngine.UI.Button a_button)
	{
//		switch (a_button.name) {
//		case "LeftButton":
//
//			for (int i = 0; i < catDetailContainerList.Count; i++) {
//
//				CategoryDetail catDetail = new CategoryDetail ();
//				catDetail.EmailId = AppManager.Instnace.userEmail;
//				catDetail.RegionId = AppManager.Instnace.regionId;
//				catDetail.CategoryID = AppManager.Instnace.categoryid;
//				catDetail.HubTrigger = catDetailContainerList [i].id;
//				catDetail.CloudTriggerId = catDetailContainerList [i].cloudId;
//				catDetail.IsLove = catDetailContainerList [i].isLoved;
//				catDetail.XmlLoveItemsDetails = "";
//				categoryDetail.Add (catDetail);
//			}
//			CategoryRoot catRoot = new CategoryRoot ();
//			catRoot.LoveItemDetails = categoryDetail.ToArray ();
//			string jsonString = catRoot.ReturnJsonString ();
//			byte[] bodyRaw = Encoding.UTF8.GetBytes (jsonString);
//			WebService.Instnace.Post (AppManager.Instnace.baseURL + "/cloud/LoveResponseService.aspx", bodyRaw, "", LovedSendCallBack);
//			break;
//		}
//
//		base.OnUIButtonClicked (a_button);
	}

	void OnCategoryDetailClicked(UnityEngine.UI.Button a_button) {


		switch (a_button.name) {

		case "ItemButton":
			Debug.Log ("Button selected -" + a_button.name);
			CategoryDetailContainer cat = a_button.transform.parent.GetComponent<CategoryDetailContainer> ();
			foreach (CategoryDetailWebService cws in catLovedDetailList) {

				if (cws.id == cat.id) {

					//*********************************************************************************
					// Adding the selected CategoryDetail object for Loved selecton in Dynamic content
					//*********************************************************************************
					WebService.Instnace.catDetailParameter.EmailId = AppManager.Instnace.userEmail;
					WebService.Instnace.catDetailParameter.RegionId = AppManager.Instnace.regionId;
					WebService.Instnace.catDetailParameter.CategoryID = cat.CategoryId;
					WebService.Instnace.catDetailParameter.CloudTriggerId = cat.cloudId;
					WebService.Instnace.catDetailParameter.IsLove = cat.isLoved;
					WebService.Instnace.catDetailParameter.XmlLoveItemsDetails = "";
					WebService.Instnace.catDetailParameter.HubTrigger = cat.id;

					string historyJsonString = "{\"history_title\":\"" + cws.history_title + "\", \"history_description\": \"" + cws.history_description + "\",\"history_image\":\"" + cws.history_image + "\",\"history_icon\":\"" + cws.history_icon + "\",\"keywords\": \"" + cws.keywords + "\"}";
					string combine = "" + cat.id + "|" + cat.cloudId + "|" + historyJsonString;

                        //AppManager.Instnace.isDynamicContentFromHub = true;

					if(AppManager.Instnace.acpTrackingManager != null)
						AppManager.Instnace.acpTrackingManager.CreateACPTrackable (combine);

					CanvasManager.Instnace.ReturnPanelManager (ePanelManager.MainMenuPanelManager).NavigateToPanel (ePanels.Blank_Panel);
				}
			}
			break;
		case "LovedButton":
			Debug.Log ("Button selected -" + a_button.name);
			AppManager.Instnace.PlayButtonSoundWithVibration ();

			CategoryDetailContainer catDC = catDetailContainerList [0];
			Transform childTransform = a_button.transform.GetChild (0);
			if (childTransform.GetComponent<Image> ().sprite == newSprite) {
				childTransform.GetComponent<Image> ().sprite = originalSprite;
				int val = int.Parse (a_button.transform.parent.parent.name);
				if (val < catDetailContainerList.Count) {
					catDetailContainerList [val].isLoved = false;
					catDC = catDetailContainerList [val];
				}
			} else {
				ShowLovedAnimation (childTransform);
				childTransform.GetComponent<Image> ().sprite = newSprite;
				int val = int.Parse (a_button.transform.parent.parent.name);
				if (val < catDetailContainerList.Count) {
					catDetailContainerList [val].isLoved = true;
					catDC = catDetailContainerList [val];
				}
			}
			CallSingleLovedCapturedService (catDC);
			break;
		}

		//		acpTrackingManager.CreateACPTrackable ();
		//		NSString *s = SF(@"{\"history_title\":\"%@\", \"history_description\": \"%@\",\"history_image\":\"%@\",\"history_icon\":\"%@\",
		//		\"keywords\": \"%@\"}", self.title,self.description,self.image_url,self.icon_url,self.keywords);
	}

	IEnumerator LoadTextureImage(int i) {
		WWW www = new WWW (catLovedDetailList [i].history_image);
		yield return www;
		if (string.IsNullOrEmpty (www.error)) {
			catDetailContainerList [i].history_image.sprite = Sprite.Create (www.texture, new Rect (0, 0, www.texture.width, www.texture.height), new Vector2 (0, 0));
		} else {
			isErrorWhileLoadingTexture = true;
		}
		if (catDetailContainerList.Count == i + 1) {
			ACPUnityPlugin.Instnace.RemoveOldPreloader ();
			//backgroundImage.CrossFadeAlpha (1.0f, 0.1f, false);
//			AppManager.Instnace.messageBoxManager.HidePreloader ();
			if (isErrorWhileLoadingTexture) {
				isErrorWhileLoadingTexture = false;
                AppManager.Instnace.messageBoxManager.ShowGenericPopup ("Alert", "Error downloading texture.", "OK");
			}
		}
	}

	IEnumerator LoadIconImage(int i) {

		WWW www_icon = new WWW (catLovedDetailList[i].history_icon);
		yield return www_icon;

		if (string.IsNullOrEmpty (www_icon.error)) {
			catDetailContainerList [i].history_icon.sprite = Sprite.Create (www_icon.texture, new Rect (0, 0, www_icon.texture.width, www_icon.texture.height), new Vector2 (0, 0));
		} else {
			isErrorWhileLoadingTexture = true;
		}
		if (catDetailContainerList.Count == i + 1) {
			
			if (isErrorWhileLoadingTexture) {
				isErrorWhileLoadingTexture = false;
                AppManager.Instnace.messageBoxManager.ShowGenericPopup ("Alert", "Error downloading Image.", "OK");
			}
		}
	}

	#region Love Capture Service and Function

	void SetLovedButton(bool isLoved, Button loveButton){

		Transform childTransform = loveButton.transform.GetChild (0);
		if (!isLoved) {
            childTransform.GetComponent<SVGImage> ().sprite = originalSprite;
		} else {
            childTransform.GetComponent<SVGImage> ().sprite = newSprite;
		}
	}


	void CallSingleLovedCapturedService(CategoryDetailContainer cdc){
//		AppManager.Instnace.messageBoxManager.ShowPreloaderDefault ();
		CategoryDetail catDetail = new CategoryDetail ();
		catDetail.EmailId = AppManager.Instnace.userEmail;
		catDetail.RegionId = AppManager.Instnace.regionId;
		catDetail.CategoryID = cdc.CategoryId;
		catDetail.HubTrigger = cdc.id;
		catDetail.CloudTriggerId = cdc.cloudId;
		catDetail.IsLove = cdc.isLoved;
		catDetail.XmlLoveItemsDetails = "";
		categoryDetail.Add (catDetail);
		CategoryRoot catRoot = new CategoryRoot ();
		catRoot.LoveItemDetails = categoryDetail.ToArray ();
		string jsonString = catRoot.ReturnJsonString ();
		byte[] bodyRaw = Encoding.UTF8.GetBytes (jsonString);
		WebService.Instnace.Post (AppManager.Instnace.baseURL + "/cloud/LoveResponseService.aspx", bodyRaw, "", LovedSendCallBack);
	}

	void CallLovedCapturedService(){

//		AppManager.Instnace.messageBoxManager.ShowPreloaderDefault ();

		for (int i = 0; i < catDetailContainerList.Count; i++) {

			CategoryDetail catDetail = new CategoryDetail ();
			catDetail.EmailId = AppManager.Instnace.userEmail;
			catDetail.RegionId = AppManager.Instnace.regionId;
			catDetail.CategoryID = catDetailContainerList[i].CategoryId;
			catDetail.HubTrigger = catDetailContainerList [i].id;
			catDetail.CloudTriggerId = catDetailContainerList [i].cloudId;
			catDetail.IsLove = catDetailContainerList [i].isLoved;
			catDetail.XmlLoveItemsDetails = "";
			categoryDetail.Add (catDetail);
		}
		CategoryRoot catRoot = new CategoryRoot ();
		catRoot.LoveItemDetails = categoryDetail.ToArray ();
		string jsonString = catRoot.ReturnJsonString ();
		byte[] bodyRaw = Encoding.UTF8.GetBytes (jsonString);
		WebService.Instnace.Post (AppManager.Instnace.baseURL + "/cloud/LoveResponseService.aspx", bodyRaw, "", LovedSendCallBack);
	}

	void LovedSendCallBack(UnityWebRequest response ){

//		AppManager.Instnace.messageBoxManager.HidePreloader ();
		CallLovedServiceAgain ();
		Debug.Log ("LovedSendResponse :" + response.downloadHandler.text);
	}

	void ReleaseObjects() {

		if (buttonArray != null && buttonArray.Length > 0) {
			foreach (Button button in buttonArray) {
				button.onClick.RemoveAllListeners ();
			}
		}
		if (catDetailContainerList.Count > 0) {
			foreach (CategoryDetailContainer catDetail in catDetailContainerList) {
				Destroy (catDetail.gameObject);
			}

			if (catDetailContainerList != null)
				catDetailContainerList.Clear ();
			buttonArray = null;
		}
		if (catLovedDetailList != null)
			catLovedDetailList.Clear ();
		AppManager.Instnace.messageBoxManager.HidePreloader ();
		base.OnDisable ();
	}
	#endregion

	#region Loved Fetch Callback

	void LovedFetchCallBack(UnityWebRequest response){

		Debug.Log ("LovedFetch :" +response.downloadHandler.text);
		if (this.gameObject.activeSelf) {
			var res = JSON.Parse (response.downloadHandler.text);

			if (res != null) {

				JSONNode test = JSONNode.Parse (response.downloadHandler.text);

				int count = test.Childs.Count ();

				if (catLovedDetailList != null)
					catLovedDetailList.Clear ();

				for (int i = 0; i < test.Childs.Count (); i++) {

					CategoryDetailWebService catDetail = new CategoryDetailWebService ();
					catDetail.id = test [i] ["id"] == null ? 0 : test [i] ["id"].AsInt;
					catDetail.cloud_id = test [i] ["cloud_id"] == null ? "" : test [i] ["cloud_id"].Value;
					catDetail.history_title = test [i] ["history_title"] == null ? "" : test [i] ["history_title"].Value;
					catDetail.history_description = test [i] ["history_description"] == null ? "" : test [i] ["history_description"].Value;
					catDetail.history_image = test [i] ["history_image"] == null ? "" : test [i] ["history_image"].Value;
					catDetail.history_icon = test [i] ["history_icon"] == null ? "" : test [i] ["history_icon"].Value;
					catDetail.keywords = "";//test [i] ["keywords"].Value;
					catDetail.channel_id = test [i] ["channel_id"] == null ? 0 : test [i] ["channel_id"].AsInt;
					catDetail.channel_name = test [i] ["channel_name"] == null ? "" : test [i] ["channel_name"].Value;
					catDetail.is_last = test [i] ["is_last"] == null ? false : test [i] ["is_last"].AsBool;
					catDetail.CategoryId = test [i] ["CategoryId"] == null ? "" : test [i] ["CategoryId"].Value;

					catLovedDetailList.Add (catDetail);
				}
				ACPUnityPlugin.Instnace.RemoveOldPreloader ();
//				AppManager.Instnace.messageBoxManager.HidePreloader ();
				//backgroundImage.CrossFadeAlpha (1.0f, 0.1f, false);
				LoadCategoriesDetailData ();
			} else {
				ACPUnityPlugin.Instnace.RemoveOldPreloader ();
//				AppManager.Instnace.messageBoxManager.HidePreloader ();
				//backgroundImage.CrossFadeAlpha (1.0f, 0.1f, false);
                AppManager.Instnace.messageBoxManager.ShowGenericPopup ("Alert", "No Loved Data Found.", "OK");
			}
		}
	}

	#endregion

	void LoadCategoriesDetailData() {
		if (catLovedDetailList.Count > 0) {
			catLovedDetailList = catLovedDetailList.Where (x => x.channel_name != null).OrderBy (x => x.channel_name).ToList ();
			if (catLovedDetailList.Count > 0) {
				int count = 0;
				foreach (CategoryDetailWebService cat in catLovedDetailList) {
					GameObject a_category = Instantiate (CategoryDetailPrefab) as GameObject;
					CategoryDetailContainer catContainer = a_category.GetComponent<CategoryDetailContainer> ();
					a_category.name = count.ToString ();
					a_category.transform.SetParent (parentObject.transform);
					a_category.transform.localPosition = new Vector3 (a_category.transform.position.x, a_category.transform.position.y, 0);
					a_category.transform.localScale = Vector3.one;

					catContainer.history_description.text = cat.history_description;
					catContainer.id = cat.id;
					catContainer.cloudId = cat.cloud_id;
					catContainer.isLoved = cat.is_last;
					catContainer.CategoryId = cat.CategoryId;
					SetLovedButton (catContainer.isLoved, catContainer.lovedButton);
					catDetailContainerList.Add (catContainer);
					count++;
				}
				if (catLovedDetailList.Count > 0) {
					for (int i = 0; i < catLovedDetailList.Count; i++) {
						if (catLovedDetailList [i].history_image != "" && catLovedDetailList [i].history_image != null) {
							StartCoroutine (LoadTextureImage (i));
						}
					}
					for (int j = 0; j < catLovedDetailList.Count; j++) {
						if (catLovedDetailList [j].history_icon != "" && catLovedDetailList [j].history_icon != null) {
							StartCoroutine (LoadIconImage (j));
						}
					}
				}
				buttonArray = parentObject.GetComponentsInChildren<Button> ();
				if (buttonArray.Length > 0) {
					foreach (Button button in buttonArray) {
						button.onClick.AddListener (() => OnCategoryDetailClicked (button));
					}
				}
			}
		} else {
            AppManager.Instnace.messageBoxManager.ShowGenericPopup ("Alert", "No Loved record found.","OK");
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

	void CallLovedServiceAgain() {

		if (buttonArray != null && buttonArray.Length > 0) {
			foreach (Button button in buttonArray) {
				button.onClick.RemoveAllListeners ();
			}
		}
		if (catDetailContainerList.Count > 0) {
			foreach (CategoryDetailContainer catDetail in catDetailContainerList) {
				Destroy (catDetail.gameObject);
			}

			if (catDetailContainerList != null)
				catDetailContainerList.Clear ();
			buttonArray = null;
		}
		if (catLovedDetailList != null)
			catLovedDetailList.Clear ();
		
		string url = AppManager.Instnace.baseURL + "/cloud/LoveService.aspx?" + "emailId=" + AppManager.Instnace.userEmail + "&c=10&rid=" +AppManager.Instnace.regionId;
		WebService.Instnace.Post (url, null, "", LovedFetchCallBack);
	}
}
