using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using frame8.Logic.Misc.Visual.UI.ScrollRectItemsAdapter;
using frame8.ScrollRectItemsAdapter.Util.Drawer;
using frame8.Logic.Misc.Visual.UI.MonoBehaviours;
using frame8.ScrollRectItemsAdapter.Util;
using frame8.Logic.Misc.Visual.UI;
using OTPL.UI;
using UnityEngine.Networking;
using System.Text;
using System.IO;
using SimpleJSON;
using System.Linq;
using UnityEngine.U2D;

namespace frame8.ScrollRectItemsAdapter.IncrementalItemFetchExample2
{
	/// <summary>
	/// This class demonstrates how items can be appended at the bottom as needed (i.e. when the user acually scrolls there), as opposed to directly downloading all of them.
	/// This is useful if the number of items can't be known beforehand (because of reasons). 
	/// Also, here it's demonstrated how items can be set custom sizes by overriding <see cref="CollectItemsSizes(ItemCountChangeMode, int, int, ItemsDescriptor)"/>
	/// Use this approach if it's impossible to know the total number of items in advance or there's simply too much overhead.
	///	If the number of items IS known, consider using placeholder prefabs with a "Loading..." text on them (which may also make for a nicer UX) while they're being downloaded/processed. 
	///	The placeholder approach is implemented in <see cref="GridExample.GridExample"/> - be sure to check it out  if interested.
	/// </summary>
    public class IncrementalItemFetch : SRIA<MyItemParams, MyItemScrollViewsHolder>, CategoryClickHandler.IOnClickCategory
	{
		// Instance of your ScrollRectItemsAdapter8 implementation
		public bool _Fetching;
		public Image backgroundImage;
		public Sprite originalSprite;
		public Sprite newSprite;
		//int itemIndexCount;
        MyScrollRect myScrollRect;
       
        //************************* Category Item *********************************
        public GameObject heartPrefab;
        public GameObject staticPanel;
        public int defaultItemCount;
        public bool isResetItem;
        public GameObject pullLoader;
        //public RectTransform categoryPanelTransform;


        MyItemScrollViewsHolder lastStoredViewHolder;
        ItemModel lastItemModel;
        Transform heartDestination;
        BottomBarPanel bottomBarPanel;
        bool isLoaded;
        //*************************************************************************

		#region SRIA implementation
		/// <inheritdoc/>

		protected override void Start()
		{
			base.Start();

            //Heart image
            originalSprite = ACPUnityPlugin.Instnace.originalSprite;
            newSprite = ACPUnityPlugin.Instnace.newSprite;
//			WebService.Instnace.InternetConnectivity ();
			ClearCachedRecyclableItems ();
			//backgroundImage.CrossFadeAlpha (0.0f, 0.1f, false);
            myScrollRect = gameObject.GetComponent<MyScrollRect>();
            //staticPanel.SetActive(false);
		}
		void OnEnable(){

            //Camera.main.pixelRect = new Rect(0, 90, Screen.width, Screen.height-90);
            GC.Collect();
            StopAllCoroutines();
            AppManager.Instnace.isIncrementalLoaded = true;
            bottomBarPanel = (BottomBarPanel)CanvasManager.Instnace.ReturnPanelManager(ePanelManager.BottomBarManager).ReturnPanel(ePanels.BottomBarPanel);
            heartDestination = bottomBarPanel.loveImage.transform;
            if (_Params.Data.Count > 0) {
                isLoaded = true;
                SetPullLoader(false);
            } else {
                isLoaded = false;
                SetPullLoader(false);
            }
		}
		void OnDisable(){

			StopAllCoroutines ();

            SetPullLoader(false);
            isLoaded = false;
            _Fetching = false;
			//if (backgroundImage != null) {
			//	backgroundImage.CrossFadeAlpha (1.0f, 0.1f, false);
			//}
		}
        public void ResetItemsInPanel() {

            SetPullLoader(false);
            isLoaded = false;
            isResetItem = true;
            _Fetching = false;
            staticPanel.SetActive(false);
            this.ResetItems(0);
            //itemIndexCount = 0;
            //_Params.Data.Clear();
            //_Params.historyIconDict.Clear ();
            //_Params.historyImageDict.Clear ();
            _Params.Data.Clear();
           
            ClearCachedRecyclableItems();
            ClearVisibleItems();
            //System.GC.Collect();
        }
    

        protected override void RebuildLayoutDueToScrollViewSizeChange()
		{
            base.RebuildLayoutDueToScrollViewSizeChange();
		}

		public override void ResetItems (int itemsCount, bool contentPanelEndEdgeStationary = false, bool keepVelocity = false)
		{
			base.ResetItems (itemsCount, contentPanelEndEdgeStationary, keepVelocity);
		}
		/// <inheritdoc/>
		protected override void Update()
		{
			
            base.Update();

            if (_Fetching || isResetItem)
            {
                return;
            }

            //if (Time.frameCount % 30 == 0)
            //{
            //    GC.Collect();
            //}


			int lastVisibleItemitemIndex = -1;
			if (_VisibleItemsCount > 0)
				lastVisibleItemitemIndex = _VisibleItems[_VisibleItemsCount - 1].ItemIndex;
			int numberOfItemsBelowLastVisible = _Params.Data.Count - (lastVisibleItemitemIndex + 1);

			// If the number of items available below the last visible (i.e. the bottom-most one, in our case) is less than <adapterParams.preFetchedItemsCount>, get more
			if (numberOfItemsBelowLastVisible <= 0) //(_Params.preFetchedItemsCount/3))
			{
//				int newPotentialNumberOfItems = _Params.Data.Count + _Params.preFetchedItemsCount;
//				if (_Params.totalCapacity > -1) // i.e. the capacity isn't unlimited
//					newPotentialNumberOfItems = Mathf.Min(newPotentialNumberOfItems, _Params.totalCapacity);
//
//				if (newPotentialNumberOfItems > _Params.Data.Count) // i.e. if we there's enough room for at least 1 more item
//					StartPreFetching(newPotentialNumberOfItems - _Params.Data.Count);
			
                if (_Params.Data.Count >= 0) { //_Params.Data.Count == 0

                    _Fetching = true;
					StartPreFetching(_Params.preFetchedItemsCount);
                }
			}

		}
			
		/// <inheritdoc/>
		protected override MyItemScrollViewsHolder CreateViewsHolder(int itemIndex)
		{
			Debug.Log ("CreateViewsHolder :" + itemIndex);
			var instance = new MyItemScrollViewsHolder();
			instance.Init(_Params.itemPrefab, itemIndex);
            instance.categoryClickHandler.iOnClickCategory = this;

			return instance;
		}

        #region CategoryItem 

        //********************************  Loved Button  *************************************************8

        public void OnHeartClickHandler(RectTransform withTrans) //LoveSelected()
        {
            MyItemScrollViewsHolder myItemScrollViewsHolder = GetItemViewsHolderIfVisible(withTrans);
            ItemModel myModel =_Params.Data[myItemScrollViewsHolder.ItemIndex];

            AppManager.Instnace.PlayButtonSoundWithVibration();
            Transform childTransform = myItemScrollViewsHolder.lovedButton.transform.GetChild(0);
            if (childTransform.GetComponent<SVGImage>().sprite == newSprite)
            {
                childTransform.GetComponent<SVGImage>().sprite = originalSprite;
                myModel.is_loved = false;
            }
            else
            {
                StartCoroutine(ShowLovedAnimation(childTransform));
                childTransform.GetComponent<SVGImage>().sprite = newSprite;
                myModel.is_loved = true;
            }
            CallSingleLovedCapturedService(myModel, myItemScrollViewsHolder);
        }

        void CallSingleLovedCapturedService(ItemModel a_model, MyItemScrollViewsHolder a_myItemScrollViewsHolder)
        {
            //AppManager.Instnace.messageBoxManager.ShowPreloaderDefault ();
            //List<CategoryDetail> categoryDetail = new List<CategoryDetail>();
            CategoryDetail catDetail = new CategoryDetail();
            catDetail.EmailId = AppManager.Instnace.userEmail;
            catDetail.RegionId = AppManager.Instnace.regionId;
            catDetail.CategoryID = a_model.CategoryId;
            catDetail.HubTrigger = a_model.id;
            catDetail.CloudTriggerId = a_model.cloud_id;
            catDetail.IsLove = a_model.is_loved;
            catDetail.XmlLoveItemsDetails = "";
            //categoryDetail.Add(catDetail);
            CategoryRoot catRoot = new CategoryRoot();
            //catRoot.LoveItemDetails = categoryDetail.ToArray();
            catRoot.LoveItemDetails = new CategoryDetail[1];
            catRoot.LoveItemDetails[0] = catDetail;
            string jsonString = catRoot.ReturnJsonString();
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonString);

            //LovedSendCallBack

            //if (AppManager.Instnace.triggerHistoryMode == eTriggerHistoryMode.TriggerHistoryModeLoved)
            //{
            //    AppManager.Instnace.messageBoxManager.ShowPreloaderDefault();
            //    IncrementalItemFetch incrementalItemFetch = myScrollRect.gameObject.GetComponent<IncrementalItemFetch>();
            //    //incrementalItemFetch.backgroundImage.CrossFadeAlpha(0.0f, 0.1f, false);
            //    incrementalItemFetch.ResetItemsInPanel();
            //}
             WebService.Instnace.Post(AppManager.Instnace.baseURL + "/cloud/LoveResponseService.aspx", bodyRaw, "", LovedSendCallBack); 

            if (AppManager.Instnace.triggerHistoryMode == eTriggerHistoryMode.TriggerHistoryModeLoved)
            {
                Debug.Log("inside TriggerHistoryModeLoved");
                //ChangeItemsCount(ItemCountChangeMode.REMOVE, 1, a_myItemScrollViewsHolder.ItemIndex);



                RemoveItems(a_myItemScrollViewsHolder.ItemIndex, 1);
                //ResetItemsInPanel();

                _Params.Data.Remove(a_model);
                //a_myItemScrollViewsHolder.root.gameObject.SetActive(false);
                //Destroy(a_myItemScrollViewsHolder.root.gameObject);

                ClearCachedRecyclableItems();
                Refresh();
                //RefreshSelectionStateForVisibleCells(a_myItemScrollViewsHolder);
            }

           
        }


        void LovedSendCallBack(UnityWebRequest response)
        {

            //AppManager.Instnace.messageBoxManager.HidePreloader ();
            //if (AppManager.Instnace.triggerHistoryMode == eTriggerHistoryMode.TriggerHistoryModeLoved)
            //{
            //    AppManager.Instnace.messageBoxManager.ShowPreloaderDefault();
            //    IncrementalItemFetch incrementalItemFetch = myScrollRect.gameObject.GetComponent<IncrementalItemFetch>();
            //    //incrementalItemFetch.backgroundImage.CrossFadeAlpha(0.0f, 0.1f, false);
            //    incrementalItemFetch.ResetItemsInPanel();
            //}
            Debug.Log("LovedSendResponse :" + response.downloadHandler.text);
        }

        IEnumerator ShowLovedAnimation(Transform HeartTransform)
        {
            yield return new WaitForEndOfFrame();

            GameObject heart = (GameObject)GameObject.Instantiate(heartPrefab, HeartTransform.position, HeartTransform.rotation);
            heart.transform.parent = heartDestination;
            heart.transform.position = HeartTransform.position;
            heart.transform.localScale = new Vector3(1f, 1f, 1);
            HeartPathFollower hpf = heart.GetComponent<HeartPathFollower>();
            hpf.PathNode = new Transform[2];
            hpf.PathNode[0] = HeartTransform;
            hpf.PathNode[1] = heartDestination;
            //iTween.PunchScale(heart, new Vector3(2, 2, 0), 0.5f);
            hpf.StartHeartAnimation();
        }
        //**************************  Category Button ****************************************

        public void OnCategoryClickHandler(RectTransform withTrans)  //CategoryDetailClicked()
        {
            //Debug.Log("myScrollRect.velocity Y:->>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>" + myScrollRect.velocity.y);

            //iTween.ShakeScale(withTrans.gameObject, new Vector3(2, 2, 0), 1.0f);
            //iTween.ScaleTo(withTrans.gameObject, new Vector3(2, 2, 0), 1.0f);

            MyItemScrollViewsHolder myItemScrollViewsHolder = GetItemViewsHolderIfVisible(withTrans);
            ItemModel myModel = _Params.Data[myItemScrollViewsHolder.ItemIndex];

            //Save the image to SocialSharingSheet
            AppManager.Instnace.socialSharingScript.m_ImageToShare = 
                myItemScrollViewsHolder.history_image.sprite.texture;


            if ((myScrollRect.velocity.y <= 1.0f && myScrollRect.velocity.y > -0.12f) && isLoaded)
            {
                
                //*********************************************************************************
                // Adding the selected CategoryDetail object for Loved selecton in Dynamic content
                //*********************************************************************************

                WebService.Instnace.catDetailParameter.EmailId = AppManager.Instnace.userEmail;
                WebService.Instnace.catDetailParameter.RegionId = AppManager.Instnace.regionId;
                WebService.Instnace.catDetailParameter.CategoryID = myModel.CategoryId;
                WebService.Instnace.catDetailParameter.CloudTriggerId = myModel.cloud_id;
                WebService.Instnace.catDetailParameter.IsLove = myModel.is_loved;
                WebService.Instnace.catDetailParameter.XmlLoveItemsDetails = "";
                WebService.Instnace.catDetailParameter.HubTrigger = myModel.id;

                WebService.Instnace.historyData.history_Title = myModel.history_title;
                WebService.Instnace.historyData.history_Description = myModel.history_description;
                WebService.Instnace.historyData.history_Image = myModel.history_image;
                WebService.Instnace.historyData.history_icon = myModel.history_icon;
                WebService.Instnace.historyData.history_keyword = myModel.keywords;

                //string historyJsonString = "{\"history_title\":\"" + myModel.history_title + "\", \"history_description\": \"" + myModel.history_description + "\",\"history_image\":\"" + myModel.history_image + "\",\"history_icon\":\"" + myModel.history_icon + "\",\"keywords\": \"" + myModel.keywords + "\"}";
                //string combine = "" + myModel.id + "|" + myModel.cloud_id + "|" + historyJsonString;

                //if (AppManager.Instnace.acpTrackingManager != null)
                    //AppManager.Instnace.acpTrackingManager.CreateACPTrackable(combine);

                AppManager.Instnace.isDynamicContentFromHub = true;
                lastStoredViewHolder = myItemScrollViewsHolder;
                lastItemModel = myModel;

                BlankPanel blankPanel = (BlankPanel)CanvasManager.Instnace.ReturnPanelManager(ePanelManager.MainMenuPanelManager).ReturnPanel(ePanels.Blank_Panel);
                blankPanel.loveCallBack = loveCallBackFunction;
                CanvasManager.Instnace.ReturnPanelManager(ePanelManager.MainMenuPanelManager).AddPanel(ePanels.Blank_Panel);
            }

        }

        public void loveCallBackFunction(bool value)
        {

            Transform childTransform = lastStoredViewHolder.lovedButton.transform.GetChild(0);
            if (value == false)
            {
                childTransform.GetComponent<SVGImage>().sprite = originalSprite;
                lastItemModel.is_loved = false;
            }
            else
            {
                childTransform.GetComponent<SVGImage>().sprite = newSprite;
                lastItemModel.is_loved = true;
            }
        }

        //************************************************************************************

        #endregion

        public MyItemScrollViewsHolder GetItemViewsHolderIfVisible(RectTransform withRoot)
        {
            MyItemScrollViewsHolder curItemViewsHolder;
            for (int i = 0; i < _VisibleItemsCount; ++i)
            {
                curItemViewsHolder = _VisibleItems[i];
                if (curItemViewsHolder.root == withRoot)
                    return curItemViewsHolder;
            }

            return null;
        }

        /// <inheritdoc/>
        protected override void UpdateViewsHolder(MyItemScrollViewsHolder newOrRecycled)
		{
			// Initialize the views from the associated model
			Debug.Log("newOrRecycled.ItemIndex :" + newOrRecycled.ItemIndex);
            if (newOrRecycled.ItemIndex >= 0 && newOrRecycled.ItemIndex < _Params.Data.Count) {
				
				ItemModel model = _Params.Data [newOrRecycled.ItemIndex];

				newOrRecycled.history_description.text = model.history_description;
                newOrRecycled.lovedButton.transform.GetChild (0).GetComponent<SVGImage> ().sprite = model.is_loved ? newSprite : originalSprite;
                newOrRecycled.history_image.preserveAspect = true;
                newOrRecycled.history_icon.preserveAspect = true;
                if (!isLoaded)
                    newOrRecycled.Sprite_Loader.SetActive(true);
                else
                    newOrRecycled.Sprite_Loader.SetActive(false);

                if (newOrRecycled.history_image.gameObject.activeSelf) {
					Debug.Log ("gameObject.activeSelf: " + gameObject.activeSelf);
//					System.GC.Collect ();
					LoadImageTexture (model.history_image, newOrRecycled);
					LoadIconTexture (model.history_icon, newOrRecycled);


                    /* Jeetesh - Uncomment from here */
//					if (_Params.historyImageDict.ContainsKey (newOrRecycled.ItemIndex)) {
//						Sprite sprite = _Params.historyImageDict [newOrRecycled.ItemIndex];
//						if (sprite != null)
//							newOrRecycled.history_image.sprite = sprite;
//					}
////					} else {
////						StartCoroutine (LoadTextureImage (model.history_image, newOrRecycled));
////						_Params.historyImageDict.Add (newOrRecycled.ItemIndex, null);
////					}
//					if (_Params.historyIconDict.ContainsKey (newOrRecycled.ItemIndex)) {
//						Sprite sprite = _Params.historyIconDict [newOrRecycled.ItemIndex];
//						if (sprite != null)
//							newOrRecycled.history_icon.sprite = sprite;
//					}
////					} else {
////						StartCoroutine (LoadTextureIcon (model.history_icon, newOrRecycled));
////						_Params.historyIconDict.Add (newOrRecycled.ItemIndex, null);
////					}
////					StartCoroutine (LoadTextureImage (model.history_image, newOrRecycled));
////					StartCoroutine (LoadTextureIcon (model.history_icon, newOrRecycled));
                    /* till here */
				} else {
					//StopAllCoroutines ();
				}
			}
		}
		#endregion

	
		// Setting _Fetching to true & starting to fetch 
		void StartPreFetching(int additionalItems)
		{
            if (_Params.Data.Count == 0)
            {
                AppManager.Instnace.messageBoxManager.ShowPreloaderDefault();
                //InsertItems(0, defaultItemCount, false, false /*keep the current velocity*/);
                //ACPUnityPlugin.Instnace.CreateOldPreloader();
            } else {
                SetPullLoader(true);
            }
//			StartCoroutine(FetchItemModelsFromServer(additionalItems, OnPreFetchingFinished));
			if (AppManager.Instnace.triggerHistoryMode == eTriggerHistoryMode.TriggerHistoryModeChannel) {
//				AppManager.Instnace.messageBoxManager.ShowPreloaderDefault ();
				//string stringUrl = AppManager.Instnace.baseURL + "/cloud/TriggersSnbnNew.aspx?rid=";
                string stringUrl = AppManager.Instnace.baseURL + "/cloud/Triggers.aspx?rid=";
				string url = LoadNextExploreItems (stringUrl, AppManager.Instnace.regionId, AppManager.Instnace.categoryid, additionalItems, _Params.Data.Count);
				WebService.Instnace.Post (url, null, "", WebCallback);

			}
			else if (AppManager.Instnace.triggerHistoryMode == eTriggerHistoryMode.TriggerHistoryModeRecent) {

//				AppManager.Instnace.messageBoxManager.ShowPreloaderDefault ();
                string stringUrl = AppManager.Instnace.baseURL + "/cloud/Recent.aspx?"; ///cloud/RecentSnbn.aspx?
				string url = ReturnRecentUrl (stringUrl, additionalItems, _Params.Data.Count);
				WebService.Instnace.Post (url, null, "", WebCallback);
			}
            else if (AppManager.Instnace.triggerHistoryMode == eTriggerHistoryMode.TriggerHistoryModeLoved)
            {

                //              AppManager.Instnace.messageBoxManager.ShowPreloaderDefault ();
                string stringUrl = AppManager.Instnace.baseURL + "/cloud/LoveService.aspx?";
                string url = ReturnRecentUrl(stringUrl, additionalItems, _Params.Data.Count);
                WebService.Instnace.Post(url, null, "", WebCallback);
            }
		}

		// Updating the models list and notify the adapter that it changed; 
		// it'll call GetItemHeight() for each item and UpdateViewsHolder for the visible ones.
		// Setting _Fetching to false
		void OnPreFetchingFinished(ItemModel[] models)
		{
			Debug.Log ("OnPreFetchingFinished");
            if (gameObject.activeInHierarchy)
            {
                StartCoroutine(Delay(models));
            }
		}
			
		IEnumerator Delay(ItemModel[] models){
            
			yield return new WaitForSeconds (2.0f);

            isLoaded = true;

            SetPullLoader(false);

            if (_Params.Data.Count == 0)
            {
                //AppManager.Instnace.messageBoxManager.HidePreloader();
                RemoveItems(0, defaultItemCount, false, false);
            }

            int elementsCount = _Params.Data.Count;

			_Params.Data.AddRange(models);

            InsertItems(elementsCount, models.Length, false, false /*keep the current velocity*/);

            _Fetching = false;
		}


		public string LoadNextExploreItems(string stringUrl, long regionId, long catId, int numberOfItemsToFetch, int exploreItemsCount) {

			string language = Application.systemLanguage.ToString();

			string url = stringUrl + AppManager.Instnace.regionId +"&catid=" + catId + "&s=" + exploreItemsCount.ToString() 
				+ "&c=" + numberOfItemsToFetch + "&language=" + language + "&emailId=" +AppManager.Instnace.userEmail;

			return url;
		}

		public string ReturnRecentUrl(string stringUrl, int numberOfItemsToFetch, int exploreItemsCount) {

			//get the user id here.
            string url = stringUrl + "emailId=" + AppManager.Instnace.userEmail + "&rid="+AppManager.Instnace.regionId+"&c="+ numberOfItemsToFetch +"&s=" + exploreItemsCount.ToString()+ "&devmode=1";
			return url;
		}

		void WebCallback(UnityWebRequest response ){

			Debug.Log ("Response Called :" +response.downloadHandler.text);

			if (this.gameObject.activeSelf) {
				var res = JSON.Parse (response.downloadHandler.text);
                if (response.downloadHandler.text != "" && response.downloadHandler.text != "[]") {
                    Debug.Log("res!=null");
                    if (_Params.Data.Count == 0)
                    {
                        InsertItems(0, defaultItemCount, false, false /*keep the current velocity*/);
                    }
                    AppManager.Instnace.messageBoxManager.HidePreloader();
					FetchedItemModelsFromServer(response, OnPreFetchingFinished);
				} else {
                    SetPullLoader(false);
					//ACPUnityPlugin.Instnace.RemoveOldPreloader ();
					//backgroundImage.CrossFadeAlpha (1.0f, 0.1f, false);
					//AppManager.Instnace.messageBoxManager.HidePreloader ();
                    Debug.Log("res==null No record found.");
                    if (_Params.Data.Count == 0)
                    {
                        if (AppManager.Instnace.triggerHistoryMode == eTriggerHistoryMode.TriggerHistoryModeLoved)
                        {
                            AppManager.Instnace.messageBoxManager.ShowGenericPopup("Alert", "No loved items.", "OK");
                            AppManager.Instnace.messageBoxManager.HidePreloader();
                            //RemoveItems(0, defaultItemCount, false, false);
                        }
                        else if (AppManager.Instnace.triggerHistoryMode == eTriggerHistoryMode.TriggerHistoryModeChannel){
                            
                            AppManager.Instnace.messageBoxManager.ShowGenericPopup("Alert", "No Feed items.", "OK");
                            AppManager.Instnace.messageBoxManager.HidePreloader();
                            //RemoveItems(0, defaultItemCount, false, false);
                        }
                        else
                        {
                            AppManager.Instnace.messageBoxManager.ShowGenericPopup("Alert", "No Recent items.", "OK");
                            AppManager.Instnace.messageBoxManager.HidePreloader();
                            //RemoveItems(0, defaultItemCount, false, false);
                        }
                    }
				}
			}
		}

		void FetchedItemModelsFromServer(UnityWebRequest response, Action<ItemModel[]> onDone)
		{
            if (!gameObject.activeSelf)
                return;

			Debug.Log ("inside FetchedItemModelsFromServer");
			JSONNode test = JSONNode.Parse (response.downloadHandler.text);
			Debug.Log ("FetchedItemModelsFromServer :" + response.downloadHandler.text);

			if (test.Childs.Count() > 0) {

				int count = test.Childs.Count ();

                var results = new ItemModel[count];

				for (int i = 0; i < test.Childs.Count (); i++) {
					
					results [i] = new ItemModel ();
					results [i].id = test [i] ["id"] == null ? 0 : test [i] ["id"].AsInt;
					results [i].cloud_id = test [i] ["cloud_id"] == null ? "" : test [i] ["cloud_id"].Value;
					results [i].history_title = test [i] ["history_title"] == null ? "" : test [i] ["history_title"].Value;
					results [i].history_description = test [i] ["history_description"] == null ? "" : test [i] ["history_description"].Value;
					results [i].history_image = test [i] ["history_image"] == null ? "" : test [i] ["history_image"].Value;
					results [i].history_icon = test [i] ["history_icon"] == null ? "" : test [i] ["history_icon"].Value;
					results [i].keywords = "";//test [i] ["keywords"].Value;
					results [i].channel_id = test [i] ["channel_id"] == null ? 0 : test [i] ["channel_id"].AsInt;
					results [i].channel_name = test [i] ["channel_name"] == null ? "" : test [i] ["channel_name"].Value;
					results [i].is_loved = test [i] ["is_love"] == null ? false : test [i] ["is_love"].AsBool;
					results [i].CategoryId = test [i] ["CategoryId"] == null ? "" : test [i] ["CategoryId"].Value;
					results [i].historyImageIndex = i;
					results [i].historyIconIndex = i;
				}
                //results = results.Where (x => x.channel_name != null).OrderBy (x => x.channel_name).c ();
                results = results.GroupBy(customer => customer.id).Select(group => group.First()).ToArray();

                results = results.Where(x => x.channel_name != null).OrderBy(x => x.is_loved == false).ToArray();


                //for (int l = 0; l < results.Count (); l++) {
                //	if (this.gameObject.activeInHierarchy) {
                //		StartCoroutine (LoadTextureIcon (results [l].history_icon, l + itemIndexCount));
                //	} else {
                //		StopCoroutine (LoadTextureIcon (results [l].history_icon, l + itemIndexCount));
                //		break;
                //	}
                //}
                //for (int k = 0; k < results.Count (); k++) {
                //	if (this.gameObject.activeInHierarchy) {
                //		StartCoroutine (LoadTextureImage (results [k].history_image, k + itemIndexCount, results, onDone));
                //	} else {
                //		StopCoroutine (LoadTextureImage (results [k].history_image, k + itemIndexCount, results, onDone));
                //	}
                //}

                for (int i = 0; i < results.Count(); i++)
                {
                    string finalstring = "";
                    if (results[i].history_icon != null || results[i].history_icon != "")
                    {
                        finalstring = results[i].history_icon.Replace("http://", "https://");
                    }

                    if(gameObject.activeInHierarchy) {
                        StartCoroutine(CacheIconTexture(finalstring)); //results[i].history_icon)
                    }
                    if(gameObject.activeInHierarchy){
                        StartCoroutine(CacheImageTexture(results[i].history_image, i, results, onDone));
                    }
                }


				//onDone (results);
			} 
		}

//		IEnumerator LoadTextureImage(string imageUrl, int index, ItemModel [] itemModal, Action<ItemModel[]> onDone) {
//			if (!gameObject.activeSelf) {
//				yield break;
//			}
//			if (imageUrl == "") {
////				_Params.historyImageDict.Add (index, null);
//				yield break;
//			}
//			WWW www = new WWW (imageUrl);
//			yield return www;
//			if (string.IsNullOrEmpty (www.error)) {
//				Sprite sprite = Sprite.Create (www.texture, new Rect (0, 0, www.texture.width, www.texture.height), new Vector2 (0, 0));
////				_Params.historyImageDict [index] = sprite;
//				if (!_Params.historyImageDict.ContainsKey(index)) {
//					_Params.historyImageDict.Add (index, sprite);
//				}

//				if (index == (itemIndexCount + itemModal.Count ()) - 1) {
//					onDone (itemModal);
//				}
////				vh.overlayImage.CrossFadeAlpha (0f, 0.2f, false);
//			} else {
//				_Params.historyImageDict.Add(index, null);
//			}
//		}
//		IEnumerator LoadTextureIcon(string imageUrl, int index ) {
//			if (!gameObject.activeSelf) {
//				yield break;
//			}
//			if (imageUrl == "") {
////				_Params.historyIconDict.Add (index, null);
//				yield break;
//			}
//			WWW www = new WWW (imageUrl);
//			yield return www;
//			if (string.IsNullOrEmpty (www.error)) {
//				Sprite sprite = Sprite.Create (www.texture, new Rect (0, 0, www.texture.width, www.texture.height), new Vector2 (0, 0));
////				_Params.historyIconDict [index] = sprite;
		//		_Params.historyIconDict.Add (index, sprite);
		//	}
		//}

        Sprite ReturnImageTextureIfExist(string imageName)
        {
            
            if (_Params.imageDictionaryFirst.ContainsKey(imageName))
            {
                return _Params.imageDictionaryFirst[imageName];
            }
            else if (_Params.imageDictionarySecond.ContainsKey(imageName))
            {
                return _Params.imageDictionarySecond[imageName];
            } 
            else 
            {
                return null;
            }
        }
        void AddImageTexture(string imageName, Sprite texture) {
            
            if (_Params.imageDictionaryFirst.Count <= 10)
            {
                if (!_Params.imageDictionaryFirst.ContainsKey(imageName))
                {
                    _Params.imageDictionaryFirst.Add(imageName, texture);
                }
            } 
            else if(_Params.imageDictionarySecond.Count <= 10) 
            {
                if (!_Params.imageDictionarySecond.ContainsKey(imageName))
                {
                    _Params.imageDictionarySecond.Add(imageName, texture);
                }
            } 
            else {

                if(_Params.imageDictionarySecond.Count == 10){
                    _Params.imageDictionaryFirst.Clear();
                    if (!_Params.imageDictionaryFirst.ContainsKey(imageName))
                    {
                        _Params.imageDictionaryFirst.Add(imageName, texture);
                    }

                } else if(_Params.imageDictionaryFirst.Count == 10){
                    _Params.imageDictionarySecond.Clear();
                    if (!_Params.imageDictionarySecond.ContainsKey(imageName))
                    {
                        _Params.imageDictionarySecond.Add(imageName, texture);
                    }
                }
            }
        }

        public void LoadImageTexture(string imageUrl, MyItemScrollViewsHolder vh) {
			if (imageUrl == "" || imageUrl == null) {
                vh.history_image.sprite = null;
                return;
			}
            string imageName = ReturnImageName(imageUrl);
            string imagePath = GetImageCachePath() + imageName;
            Debug.Log("ImageName :-------------------- " + imageName);
            Sprite tempTexture = ReturnImageTextureIfExist(imageName);

            if(tempTexture != null) {
                vh.history_image.sprite = tempTexture;
                    //Sprite.Create(tempTexture, new Rect(0, 0, tempTexture.width, tempTexture.height), new Vector2(0, 0));
            } else {

                if (File.Exists(imagePath))
                {
                    //print ("Loading Image from Cache: "+ imagePath);
                    byte[] byteArray = File.ReadAllBytes(imagePath);
                    Texture2D texture = new Texture2D (128, 128);
                    texture.LoadImage(byteArray);

                    if (texture != null)
                    {
                        vh.history_image.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0, 0));
                        AddImageTexture(imageName, vh.history_image.sprite);
                    }
                }
            }
		}

        Sprite ReturnIconTextureIfExist(string imageName)
        {

            if (_Params.iconDictionaryFirst.ContainsKey(imageName))
            {
                return _Params.iconDictionaryFirst[imageName];
            }
            else if (_Params.iconDictionarySecond.ContainsKey(imageName))
            {
                return _Params.iconDictionarySecond[imageName];
            }
            else
            {
                return null;
            }
        }
        void AddIconTexture(string imageName, Sprite texture)
        {

            if (_Params.iconDictionaryFirst.Count <= 10)
            {
                if (!_Params.iconDictionaryFirst.ContainsKey(imageName))
                {
                    _Params.iconDictionaryFirst.Add(imageName, texture);
                }
            }
            else if (_Params.iconDictionarySecond.Count <= 10)
            {
                if (!_Params.iconDictionarySecond.ContainsKey(imageName))
                {
                    _Params.iconDictionarySecond.Add(imageName, texture);
                }
            }
            else
            {

                if (_Params.iconDictionarySecond.Count == 10)
                {
                    _Params.iconDictionaryFirst.Clear();
                    if (!_Params.iconDictionaryFirst.ContainsKey(imageName))
                    {
                        _Params.iconDictionaryFirst.Add(imageName, texture);
                    }

                }
                else if (_Params.iconDictionaryFirst.Count == 10)
                {
                    _Params.iconDictionarySecond.Clear();
                    if (!_Params.iconDictionarySecond.ContainsKey(imageName))
                    {
                        _Params.iconDictionarySecond.Add(imageName, texture);
                    }
                }
            }
        }

        public void LoadIconTexture(string imageUrl, MyItemScrollViewsHolder vh) {
			if (imageUrl == "" || imageUrl == null) {
                vh.history_icon.sprite = null;
                return;
			}
            string imageName = ReturnImageName(imageUrl);
            string imagePath = GetIconCachePath() + imageName;

            Sprite tempTexture = ReturnIconTextureIfExist(imageName);
            if (tempTexture != null)
            {
                vh.history_icon.sprite = tempTexture;
                    //Sprite.Create(tempTexture, new Rect(0, 0, tempTexture.width, tempTexture.height), new Vector2(0, 0));
            } else {

                if (File.Exists(imagePath))
                {
                    //print ("Loading Icon from Cache: "+ imagePath);
                    Debug.Log("--------------------------------------> LoadIconTexture - :" + imagePath);
                    byte[] byteArray = File.ReadAllBytes(imagePath);
                    Texture2D texture = new Texture2D (100, 100);
                    texture.LoadImage(byteArray);
                    if (texture != null) //texture != null
                    {
                        vh.history_icon.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0, 0));
                        AddIconTexture(imageName, vh.history_icon.sprite);
                    }
                }

            }
           
		}
		public string GetImageCachePath()
		{
            string path = Application.persistentDataPath + "/CachedImage/";

			if (!Directory.Exists(path))
			{
				Directory.CreateDirectory(path);
			}
			return path;
		}
		public string GetIconCachePath()
		{
            string path = Application.persistentDataPath + "/CachedIcon/";

			if (!Directory.Exists(path))
			{
				Directory.CreateDirectory(path);
			}
			return path;
		}

        public IEnumerator CacheImageTexture(string imageUrl, int index, ItemModel[] itemModal, Action<ItemModel[]> onDone)
        {
            if (!gameObject.activeInHierarchy || imageUrl == "" || imageUrl == null)
            {
                StopCoroutine("CacheImageTexture");
                yield break;
            }
            string imagePath = GetImageCachePath() + ReturnImageName(imageUrl);

            if (!File.Exists(imagePath))
            {
                WWW www = new WWW(imageUrl);
                yield return www;
                if (www.error == null)
                {
                    Texture2D texture = www.texture;
                    byte[] bytes = texture.EncodeToJPG();
                    File.WriteAllBytes(imagePath, bytes);
                }
            }
            if (index == itemModal.Count() - 1)
            {
                onDone(itemModal);
            }
        }

        public IEnumerator CacheIconTexture(string imageUrl)
        {
            if (!gameObject.activeInHierarchy || imageUrl == "" || imageUrl == null)
            {
                StopCoroutine("CacheIconTexture");
                yield break;
            }
            string imagePath = GetIconCachePath() + ReturnImageName(imageUrl);
            if (!File.Exists(imagePath))
            {
                WWW www = new WWW(imageUrl);
                yield return www;
                if (www.error == null)
                {
                    Texture2D texture = www.texture;
                    byte[] bytes = texture.EncodeToJPG();
                    Debug.Log("--------------------------------------> CacheIconTexture - :" + imagePath);
                    Debug.Log("--------------------------------------> CacheIconTexture Url - :" + imageUrl);
                    File.WriteAllBytes(imagePath, bytes);
                }
            }
        }

        string ReturnImageName(string imageUrl){

            string[] breakApart = imageUrl.Split('/');
            string str = breakApart[breakApart.Length - 1];
            string[] breakDot = str.Split('.');
            string imageName = breakDot[0];

            return imageName;
        }

        void OnApplicationPause(bool paused) {
            if(paused) {
                ClearCachedRecyclableItems();
                GC.Collect();
            }
        }

        void SetPullLoader(bool value){
            if(value)
                transform.GetComponent<RectTransform>().offsetMin = new Vector2(0, 100);
            else
                transform.GetComponent<RectTransform>().offsetMin = new Vector2(0, 0);

            pullLoader.SetActive(value); 
        }

	}


	[Serializable]
	public class ItemModel { 
		
		public long id;
		public string CategoryId;
		public string history_image;
		public string history_icon;
		public string history_title; 
		public string history_description;
		public string cloud_id;
		public string keywords;
		public string channel_name;
		public long channel_id;
		public bool is_loved;
		public int historyImageIndex;
		public int historyIconIndex;
	}


	// This in almost all cases will contain the prefab and your list of models
	[Serializable] // serializable, so it can be shown in inspector
	public class MyItemParams : BaseParamsWithPrefabAndData<ItemModel>
	{
		public int preFetchedItemsCount;
		//public Dictionary<int, Sprite> historyImageDict = new Dictionary<int, Sprite> ();
		//public Dictionary<int, Sprite> historyIconDict = new Dictionary<int, Sprite> ();
		[Tooltip("Set to -1 if while fetching <preFetchedItemsCount> items, the adapter shouldn't check for a capacity limit")]
		public int totalCapacity;
        public Dictionary<string, Sprite> imageDictionaryFirst = new Dictionary<string, Sprite>();
        public Dictionary<string, Sprite> imageDictionarySecond = new Dictionary<string, Sprite>();
        public Dictionary<string, Sprite> iconDictionaryFirst = new Dictionary<string, Sprite>();
        public Dictionary<string, Sprite> iconDictionarySecond = new Dictionary<string, Sprite>();
	}


	public class MyItemScrollViewsHolder : BaseItemViewsHolder
	{
		public Image history_image;
		public Image history_icon;
		public Text history_description;
		public Button lovedButton;
		public Image overlayImage;
        public GameObject Sprite_Loader;
        public CategoryClickHandler categoryClickHandler;

		public override void CollectViews()
		{
			base.CollectViews();
			history_image = root.Find("Panel/HistoryImage").GetComponent<Image>();
            history_icon = root.Find ("Image/Channel/ChannelImage").GetComponent<Image> ();
			history_description = root.Find ("Image/Text").GetComponent<Text> ();
			lovedButton = root.Find ("Image/LovedButton").GetComponent<Button> ();
            categoryClickHandler = root.GetComponent<CategoryClickHandler>();
            Sprite_Loader = root.Find("Sprite_Loader").gameObject;
		}
	}
}
