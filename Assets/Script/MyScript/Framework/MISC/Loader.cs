using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OTPL.UI;
using UnityEngine.UI;
using Vuforia;
using System;
using System.IO;

public class Loader : Singleton<Loader>
{

    //Jeetesh - Splash panel is no longer there. 
    public GameObject splashPanel;
    public GameObject loadingPanel;
    public GameObject socialLoadingPanel;
    //public float speed = 0.2f;
    //public LoadingEffect loadingEffect;
    float progressCount = 0;
    bool isShowingSplash;
    RectTransform rectTransform;

    void Start()
    {
        //set the loading panel inactive and splashPanel for Video playing active by default.
        loadingPanel.SetActive(false);
        socialLoadingPanel.SetActive(false);
        splashPanel.SetActive(true);

        CanvasManager.Instnace.HidePanelManager(ePanelManager.MainMenuPanelManager);
        CanvasManager.Instnace.HidePanelManager(ePanelManager.BottomBarManager);
        CanvasManager.Instnace.HidePanelManager(ePanelManager.PopupSurvayManager);

#if UNITY_EDITOR
        DeactivateSplash();
#else
        isShowingSplash = true;
        StartCoroutine(WaitForTime(5f));
#endif


    }

    public void ShowLoader(float t)
    {
        loadingPanel.SetActive(true);
        //loadingEffect.loading = true;
        StartCoroutine(WaitForTime(t));
    }

    IEnumerator WaitForTime(float t)
    {
        yield return new WaitForSeconds(t);
        if (isShowingSplash)
        {
            isShowingSplash = false;
            DeactivateSplash();
        }
        else
        {
            HideLoader();
        }
    }
    void DeactivateSplash()
    {
        splashPanel.gameObject.SetActive(false);
        //Neet to update the user info in AppManager as well.

        UserDataService userDbService = new UserDataService();
        UserInfo _user = userDbService.GetUser();

#if UNITY_EDITOR
        //userDbService.DeleteAll();
#else
        if (!PlayerPrefs.HasKey("flag"))
        {
            userDbService.DeleteAll();
        }
#endif


        if (_user != null && PlayerPrefs.HasKey("flag")){
            /*Jeetesh -  Old already existing user.
             * User will also come here when he overwrites the application.*/
            //AppManager.Instnace.messageBoxManager.ShowMessage("Loader.cs", "_user != null PlayerPrefs.HasKey:" + PlayerPrefs.HasKey("flag"),"OK");

			Debug.Log ("User Exist with Email" + _user.Email);
			AppManager.Instnace.userId = _user.ViewaUser_id;
			AppManager.Instnace.regionId = _user.RegionId;
			AppManager.Instnace.userEmail = _user.Email;

            WebService.Instnace.appUser.Email = _user.Email;
            WebService.Instnace.appUser.RegionId = _user.RegionId;
            WebService.Instnace.appUser.FirstName = _user.FirstName;
            WebService.Instnace.appUser.LastName = _user.LastName;
            WebService.Instnace.appUser.Mobile = _user.Mobile;
            WebService.Instnace.appUser.Country = _user.Country;
            WebService.Instnace.appUser.Gender = _user.Gender;
            WebService.Instnace.appUser.interest = _user.interest;

			CanvasManager.Instnace.ShowPanelManager(ePanelManager.MainMenuPanelManager);
            AppManager.Instnace.GoToScanScreen ();
			return;
		} else {
            /*Jeetesh -  If the user is a completely new user and not re-downloaded one.*/
			Debug.Log ("User Does not exist");
			//CanvasManager.instance.ReturnPanelManager (ePanelManager.MainMenuPanelManager).showInitiallyOpenPanel = true;
			CanvasManager.Instnace.ShowPanelManager(ePanelManager.MainMenuPanelManager);
            CanvasManager.Instnace.ReturnPanelManager(ePanelManager.MainMenuPanelManager).NavigateToPanel(ePanels.GetStarted_Panel);
		}
	}

	public void ShowLoaderWithBackGround(){
		ResizeLoaderScreen ();
		//loadingPanel.GetComponent<UnityEngine.UI.Image> ().enabled = true;
		loadingPanel.SetActive (true);
		//loadingEffect.loading = true;
	}
    public void ShowSocialLoader()
    {
        socialLoadingPanel.SetActive(true);
    }
    public void HideSocialLoader(){

        socialLoadingPanel.SetActive(false);
    }
	public void ShowLoader(){
		//loadingPanel.GetComponent<UnityEngine.UI.Image> ().enabled = false;
		loadingPanel.SetActive (true);
		//loadingEffect.loading = true;
	}
	public void HideLoader(){
		loadingPanel.SetActive (false);
		//loadingEffect.loading = false;
	}

	//TODO: jeetesh - Verify if this is correct.
	public void ResizeLoaderScreen(){

		rectTransform = loadingPanel.GetComponent<RectTransform> ();

		if (!AppManager.Instnace.isLoggedInAndInside) {
			rectTransform.offsetMax = new Vector2 (rectTransform.offsetMax.x, 0f);	//Top
			rectTransform.offsetMin = new Vector2 (rectTransform.offsetMin.x, 0f); 	//Bottom
		} else {
            if (AppManager.Instnace.isIphoneX) {
				rectTransform.offsetMax = new Vector2 (rectTransform.offsetMax.x, (110 + 40) * -1);	//Top  //80
				rectTransform.offsetMin = new Vector2 (rectTransform.offsetMin.x, 90f + 40);	//Bottom   //80
			} else {
				rectTransform.offsetMax = new Vector2 (rectTransform.offsetMax.x, 107f * -1);//Top
				rectTransform.offsetMin = new Vector2 (rectTransform.offsetMin.x, 90f);	//Bottom
			}
		}
	}
}
