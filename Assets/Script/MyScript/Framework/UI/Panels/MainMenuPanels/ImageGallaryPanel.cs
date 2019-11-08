using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using OTPL.UI;
using SimpleJSON;
using System.Text;
using UnityEngine.Networking;

//using Unicache;
//using Unicache.Plugin;
//using UniRx;

public class ImageGallaryPanel : PanelBase {

	public GameObject GalleryImagePrefab;
	public GameObject GalleryThumbImagePrefab;
	public Transform GalleryImageParentObject;
	public Transform GalleryThumbImageParentObject;
	List<UnityEngine.UI.Image> GalleryImageList = new List<UnityEngine.UI.Image> ();
	List<UnityEngine.UI.Image> GalleryThumbList = new List<UnityEngine.UI.Image> ();
	public UnityEngine.UI.Text navigationTitle;
	RootObject galleryRoot;
	Button[] buttonArray;
	Button[] imageThumbButtons;
	public ImageGalleryController imageGalleryController;

	public Button LovedButton;
	Sprite originalSprite;
	Sprite newSprite;
	List<CategoryDetail> categoryDetail = new List<CategoryDetail> ();
	public GameObject heartPrefab;
	public GameObject imageGalleryObj;
	public Image backgroundImage;

	//private IUnicache cache;
	private int textureNum;
    RectTransform rT;
    BottomBarPanel bottomBarPanel;
    Transform heartDestination;

    //-----------------------
    public ulong mTriggerId;
    public int mAreaIndex;
    public int mFrameIndex;
    public int mWidgetIndex;
    public string mUrl;

	[System.Serializable]
	public class GImages
	{
		public string image_url;
		public string title;
		public string button_text;
		public string button_link;
		public string button_link_ios;
		public string button_link_android;
		public string form_type;
		public string tracking_id;
		public bool button_link_external;
		public string share_title;
		public string share_text;
		public string share_link;
		public bool button_link_full_screen;
	}
	[System.Serializable]
	public class RootObject
	{
		public List<GImages> images;
		public string tracking_id;
	}

	public void ClearCache()
	{
		//this.cache.Clear();
	}

	protected override void Awake ()
	{
//		imageGalleryController.gameObject.SetActive (false);
		base.Awake ();
		//this.cache = new FileCache(this.gameObject);
		//this.cache.Handler = new SimpleDownloadHandler();
		//this.cache.UrlLocator = new SimpleUrlLocator();
		//this.cache.CacheLocator = new SimpleCacheLocator();
//		backgroundImage.CrossFadeAlpha (0.0f, 0.1f, false);

        if (AppManager.Instnace.isIphoneX)
        {
            rT = transform.GetChild(0).GetComponent<RectTransform>();
            rT.sizeDelta = new Vector2(rT.sizeDelta.x, rT.sizeDelta.y + 100f);

            RectTransform rectTransform = transform.GetChild(1).GetComponent<RectTransform>();
            rectTransform.localPosition = new Vector3(rectTransform.localPosition.x, rectTransform.localPosition.y + 40, 0);
        }
	}
	protected override void OnEnable ()
	{
		base.OnEnable ();
		textureNum = 0;
		CanvasManager.Instnace.ShowPanelManager (ePanelManager.BottomBarManager);
        originalSprite = ACPUnityPlugin.Instnace.originalSprite;
        newSprite = ACPUnityPlugin.Instnace.newSprite;
		SetLovedButton (WebService.Instnace.catDetailParameter.IsLove, LovedButton);
		imageGalleryController.enabled = false;
        ACPUnityPlugin.Instnace.pauseVideo ();
//		if (backgroundImage != null) {
//			backgroundImage.CrossFadeAlpha (0.0f, 0.1f, false);
//		}
//		ACPUnityPlugin.Instnace.CreateOldPreloader ();

        //tracking
        ACPUnityPlugin.Instnace.trackScreen("ImageGallery");
        ACPUnityPlugin.Instnace.trackEvent("ImageGallery", "ImageGalleryStart", "", 0);
        bottomBarPanel = (BottomBarPanel)CanvasManager.Instnace.ReturnPanelManager(ePanelManager.BottomBarManager).ReturnPanel(ePanels.BottomBarPanel);
        heartDestination = bottomBarPanel.loveImage.transform;
        Debug.Log("ImageGalleryPanel - OnEnable");

        _imageGalleryStart(mTriggerId, mAreaIndex, mFrameIndex, mWidgetIndex, mUrl);

	}

	protected override void OnDisable() {
        
		if (imageThumbButtons != null && imageThumbButtons.Length > 0) {
			foreach (Button button in imageThumbButtons) {
				button.onClick.RemoveAllListeners ();
			}
			imageThumbButtons = null;
		}
		if (galleryRoot != null) {
			if (galleryRoot.images != null)
				galleryRoot.images.Clear ();
			galleryRoot = null;
		}
		if (GalleryImageList != null) {
			foreach (Image img in GalleryImageList) {
				Destroy (img.gameObject);
			}
			GalleryImageList.Clear ();
		}
		if (GalleryThumbList != null) {
			foreach (Image img in GalleryThumbList) {
				Destroy (img.transform.parent.gameObject);
			}
			GalleryThumbList.Clear ();
		}
//		if (backgroundImage != null) {
//			backgroundImage.CrossFadeAlpha (0.0f, 0.1f, false);
//		}

		imageGalleryController.enabled = false;

		System.GC.Collect ();

		base.OnDisable ();

        ACPUnityPlugin.Instnace.trackEvent("ImageGallery", "ImageGalleryEnd", "", 0);
	}
	protected override void OnUIButtonClicked (UnityEngine.UI.Button a_button)
	{
		switch (a_button.name) {
		case "LeftButton":
                //			if (AppManager.Instnace.isVuforiaOn) {
                //				CanvasManager.Instnace.ReturnPanelManager (ePanelManager.MainMenuPanelManager).BackToPanel (ePanels.Blank_Panel);
                //			} else {
                //				CanvasManager.Instnace.ReturnPanelManager (ePanelManager.MainMenuPanelManager).BackToPanel (ePanels.Scan_Panel);
                //			}
                //jeetesh - This is required only if we are loading gallery directly from Hub.
                Debug.Log("ON_back_button_tap");
                //GameObject[] trackables = GameObject.FindGameObjectsWithTag("Trackable");

                if (AppManager.Instnace.isGoingToGalleryFromScan){
                    AppManager.Instnace.isVuforiaOn = true;
                }else{
                    AppManager.Instnace.isVuforiaOn = false;
                }

                AppManager.Instnace.acpTrackingManager.OnBackButtonTapped();

                //if (trackables.Length <= 0)
                //{
                //    AppManager.Instnace.acpTrackingManager.OnBackButtonTapped();
                //} else {
                //    base.OnUIButtonClicked(a_button);
                //}
			break;
		case "LovedButton":
			Debug.Log ("Button selected -" + a_button.name);
			AppManager.Instnace.PlayButtonSoundWithVibration ();
			Transform childTransform = a_button.transform.GetChild (0);
			if (childTransform.GetComponent<SVGImage>().sprite == newSprite) {
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
		//base.OnUIButtonClicked (a_button);
	}

	public void _imageGalleryStart(ulong triggerId,
		int areaIndex,
		int frameIndex,
		int widgetIndex,
		string url) {
		Debug.Log ("Inside _imageGalleryStart");
		categoryDetail.Clear ();
        //		ACPUnityPlugin.Instnace.CreateOldPreloader ();
        //if (url != null || url != "")
        //{
        //    url = url.Replace("http://", "https://");
        //}
        string a_url = AppManager.Instnace.baseURL + "/cloud/gallery.aspx?gid=" + ReturnImageName(url);

        AppManager.Instnace.messageBoxManager.ShowPreloaderDefault();
        WebService.Instnace.Get(a_url, WebCallBack);
	}

    string ReturnImageName(string imageUrl)
    {

        string[] breakApart = imageUrl.Split('/');
        string str = breakApart[breakApart.Length - 1];
        string[] breakDot = str.Split('.');
        string imageName = breakDot[0];

        return imageName;
    }

    string finalUrl;
	void WebCallBack(string response){

//		Debug.Log ("Response :" + response);
////		CanvasManager.Instnace.ShowPanelManager (ePanelManager.BottomBarManager);
////		navigationTitle.text = AppManager.Instnace.categoryName;

		//var res = JSON.Parse(response);

		//if (res != null) {
			//galleryRoot = JsonUtility.FromJson<RootObject> (response);

			//for (int i = 0; i < galleryRoot.images.Count; i++) {

     //           finalUrl = galleryRoot.images[i].image_url;
     //           //if (galleryRoot.images[i].image_url != null || galleryRoot.images[i].image_url != "")
     //           //{
     //           //    finalUrl = galleryRoot.images[i].image_url.Replace("http://assets.viewa.com/", "https://s3-ap-southeast-2.amazonaws.com/assets.viewa/");
     //           //}
     //           this.cache.Fetch(finalUrl)
     //               .ByteToTexture2D(name: finalUrl)
					//.Subscribe(texture =>
						//{
							//GameObject gallery_image = Instantiate(GalleryImagePrefab, new Vector3(0,0,0), Quaternion.identity)as GameObject;
							//UnityEngine.UI.Image gImage = gallery_image.GetComponentInChildren<UnityEngine.UI.Image> ();
							//gallery_image.transform.SetParent(GalleryImageParentObject);
							//gallery_image.transform.localPosition = new Vector3 (gallery_image.transform.position.x, gallery_image.transform.position.y, 0);
							//gallery_image.transform.localScale = Vector3.one;
							//GalleryImageList.Add (gImage);
//							gImage.sprite = Sprite.Create (texture, new Rect (0f, 0f,texture.width, texture.height), new Vector2 (0, 0));
//							gImage.preserveAspect = true;

//							GameObject galleryThumb_image = Instantiate(GalleryThumbImagePrefab, new Vector3(0,0,0), Quaternion.identity) as GameObject;
//							UnityEngine.UI.Image gTImage = galleryThumb_image.transform.GetChild(0).GetComponent<UnityEngine.UI.Image> ();
//							galleryThumb_image.transform.SetParent(GalleryThumbImageParentObject);
//							galleryThumb_image.transform.localPosition = new Vector3 (galleryThumb_image.transform.position.x, galleryThumb_image.transform.position.y, 0);
//							galleryThumb_image.transform.localScale = Vector3.one;
//							GalleryThumbList.Add (gTImage);
//							Color32[] pix = texture.GetPixels32();
//							Texture2D textureNew = new Texture2D(texture.width,texture.height); 
//							textureNew.SetPixels32(pix);
//							textureNew.Apply();
////							TextureScale.Bilinear (textureNew, 107, 107);
//							gTImage.sprite = Sprite.Create (textureNew, new Rect (0f, 0f, textureNew.width, textureNew.height), new Vector2 (0, 0));
//							gTImage.preserveAspect = true;

//							if (galleryRoot.images.Count == textureNum + 1) {

//								imageGalleryController.enabled = true;
//								imageGalleryController.Start ();
//								AppManager.Instnace.isDynamicDataLoaded = true;
//								AppManager.Instnace.messageBoxManager.HidePreloader ();
//								imageGalleryController.InitialiseChildObjectsFromScene();
////								ACPUnityPlugin.Instnace.RemoveOldPreloader();
////								ACPUnityPlugin.Instnace.RemoveOldPreloader ();
////								backgroundImage.CrossFadeAlpha (1.0f, 0.1f, false);


//								//Add listener to imageThumbButtons
//								imageThumbButtons = GalleryThumbImageParentObject.GetComponentsInChildren<UnityEngine.UI.Button> ();
//								if (imageThumbButtons.Length > 0) {
//									foreach (UnityEngine.UI.Button button in imageThumbButtons) {
//										button.onClick.AddListener (() => OnThumbClicked (button));
//									}
//								}
//							}

//							textureNum ++;
//						});
////				StartCoroutine (LoadImage (i));
		//	}
		//}	
	}

	void OnThumbClicked(UnityEngine.UI.Button a_button) {

		for(int i = 0; i< imageThumbButtons.Length; i++) {
//			if (a_button.gameObject.name == i.ToString()) {
//				//highlight the image and show the same image in Gallery image.
//				imageGalleryController.GoToScreen(i);
//			} 
			if (a_button == imageThumbButtons[i]) {
				//highlight the image and show the same image in Gallery image.
				imageGalleryController.GoToScreen(i);
			} 
		}
	}

	IEnumerator LoadImage(int i) {
		WWW www = new WWW (galleryRoot.images[i].image_url);
		yield return www;

		GalleryImageList[i].sprite = Sprite.Create (www.texture, new Rect (0f, 0f, www.texture.width, www.texture.height), new Vector2 (0, 0));
		GalleryThumbList[i].sprite = Sprite.Create (www.texture, new Rect (0f, 0f, www.texture.width, www.texture.height), new Vector2 (0f, 0f));
		GalleryThumbList [i].gameObject.name = i.ToString();
		if (galleryRoot.images.Count == i + 1) {
			AppManager.Instnace.isDynamicDataLoaded = true;
			AppManager.Instnace.messageBoxManager.HidePreloader ();
			imageGalleryController.InitialiseChildObjectsFromScene ();
		}
	}

	#region Loved functions

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
		} else {
			childTransform.GetComponent<SVGImage> ().sprite = newSprite;
		}
	}
	#endregion

	IEnumerator ShowLovedAnimation(Transform HeartTransform) {

		yield return new WaitForEndOfFrame ();

		GameObject heart = (GameObject) GameObject.Instantiate (heartPrefab, HeartTransform.position, HeartTransform.rotation);
		heart.transform.parent = heartDestination;
		heart.transform.position = HeartTransform.position;
        heart.transform.localScale = new Vector3(1f, 1f, 1f);
		HeartPathFollower hpf = heart.GetComponent<HeartPathFollower> ();
		hpf.PathNode = new Transform[2];
		hpf.PathNode [0] = HeartTransform;
		hpf.PathNode [1] = heartDestination;
		//iTween.PunchScale (heart, new Vector3 (8, 8, 0), 2.0f);
		hpf.StartHeartAnimation ();
	}

	//private void Update()
	//{
 //       if (Application.platform == RuntimePlatform.Android)
 //       {
 //           if (Input.GetKey(KeyCode.Escape))
 //           {
 //               if (AppManager.Instnace.isGoingToGalleryFromScan){
 //                   AppManager.Instnace.isVuforiaOn = true;
 //               }
 //               else{
 //                   AppManager.Instnace.isVuforiaOn = false;
 //               }

 //               AppManager.Instnace.acpTrackingManager.OnBackButtonTapped();

 //           }
 //       }
	//}

}
