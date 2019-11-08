//----------------------------------------------
//       OTPL - Jeetesh
//       created: 5 Dec 2017
//       Copyright © 2017 OTPL
//       Description: This class will be responsible for overall app Management.
//----------------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OTPL.modal;
using System.Linq;
using SimpleJSON;
using OTPL.UI;
using System.IO;
using Vuforia;
using System.Text;
using UnityEngine.U2D;
using System;

public class AppManager : Singleton<AppManager>
{

    [HideInInspector]
    public bool isVuforiaOn;
    [HideInInspector]
    public string categoryName;
    [HideInInspector]
    public eTriggerHistoryMode triggerHistoryMode;
    [HideInInspector]
    public string accessToken;
    [HideInInspector]
    public string userId;
    [HideInInspector]
    public long regionId;  //default region id is Australia until the user does not select a different region.
    [HideInInspector]
    public string userEmail;
    [HideInInspector]
    public long categoryid;
    [HideInInspector]
    public bool isSocialSignInScreen;
    [HideInInspector]
    public string baseURL;
    [HideInInspector]
    public ACPTrackingManager acpTrackingManager;
    [HideInInspector]
    public bool isIphoneX;
    [HideInInspector]
    public bool isPopUpSurvay;
    [HideInInspector]
    public float mDownloadProgress;
    [HideInInspector]
    public eSocialSignUp eSocial;
    [HideInInspector]
    public List<CategoriesContainer> catContainerList = new List<CategoriesContainer>();
    [HideInInspector]
    public bool isRegionHasChanged;
    [HideInInspector]
    public GeoLocation geoLocation;
    [HideInInspector]
    public bool isDynamicDataLoaded;  //Important bool value
    [HideInInspector]
    public bool isLoggedInAndInside;  //It will be true once reached to Scan page.
    [HideInInspector]
    public string[] yearsArray;
    [HideInInspector]
    public bool isDynamicContentFromHub;
    [HideInInspector]
    public bool isShowPopUpMessage;
    [HideInInspector]
    public string popUpQuestionsResponse;
    [HideInInspector]
    public string trackingId;
    [HideInInspector]
    public string triggerId;
    [HideInInspector]
    public bool isIncrementalLoaded;
    [HideInInspector]
    public bool isGoingToGalleryFromScan;
    [HideInInspector]
    public string popup_keyword;
    [HideInInspector]
    public bool doesButtonPressed;
    [HideInInspector]
    public string baseLiveUrl = "https://cms.viewa.com"; //"http://13.236.75.11";
    [HideInInspector]
    public string baseStagingUrl = "http://cms.viewa.com:8080";
    [HideInInspector]
    public string AppVersionNumber;

    public bool isLive;


    public HubPanel hubPanel;
    public eScreenAnimationTransition ScreenAnimationTransition;
    public SocialSharingScript socialSharingScript;
    public MessageBoxManager messageBoxManager;
    public VideoPlayingPage videoPlayingPage;
    public WebViewPage webViewPage;
    public PushNotificationController pushNotificationController;
    //public AudioSource audioSourceClick;
    //public AudioSource audioSourceBack;
    public SpriteAtlas spriteAtlas;
    GoogleCloudLogin googleCloudLogin;

    string ProfilePicPath;
    bool isSwitchOffVuforia;


#region Initialization Methods

    private void Awake()
    {
        //Set the target framerate default to 60
        Application.targetFrameRate = 60;

        AppVersionNumber = Application.version;

        FacebookLogin.FacebookInit();

        if (acpTrackingManager == null)
        {
            if(Camera.main != null)
                acpTrackingManager = Camera.main.GetComponent<ACPTrackingManager>();
        }
         googleCloudLogin = gameObject.GetComponentInChildren<GoogleCloudLogin>(); 
        //baseURL = "https://cms.viewa.com";  //"https://cms.viewa.com";   //"http://13.236.75.11";
        if (isLive)
        {
            baseURL = baseLiveUrl;
        }
        else
        {
            baseURL = baseStagingUrl;
        }

        Caching.ClearCache();
        ScreenAnimationTransition = eScreenAnimationTransition.SelectScreenTransition;
        socialSharingScript = gameObject.GetComponent<SocialSharingScript>();
        ProfilePicPath = Application.persistentDataPath + "/profilePic.png";

        //Delete the user if exists from the database at the beginning of the App.
        //But this will not delete the user if the user has overwritten the application on an existing application.
        if (!PlayerPrefs.HasKey("flag"))
        {
            PlayerPrefs.DeleteAll();
            PlayerPrefs.SetInt("flag", 1);
            PlayerPrefs.Save();

            DeleteDatabase();
            UserDataService userDbService = new UserDataService();
            userDbService.DeleteAll();
        }

        if (Screen.width == 1125f && Screen.height == 2436)
        {

            isIphoneX = true;
        }

        Debug.Log("Screen Width: " + Screen.width + "Screen Height: " + Screen.height);

        //Hide these screens to show Loader initially
        Loader.Instnace.gameObject.SetActive(true);
        //Make the Default region as Australia.
        regionId = 53693;

        //If region is null call region service.
        RegionDataService regionDbService = new RegionDataService();
        int regionCount = regionDbService.GetRegionCount();

        if (regionCount <= 0)
        {
            WebService.Instnace.Get(baseURL + "/cloud/regions.aspx", WebCallRegion);
        }

        //Load the default localizedstring Data.
        LocalizeStringManager.LoadGameData();

        List<string> local_years = new List<string>();
        for (int i = 2018; i >= 1800; i--)
        {
            local_years.Add(i.ToString());
        }
        yearsArray = local_years.ToArray();
    }


    void DeleteDatabase()
    {
        if (File.Exists(Application.persistentDataPath + "/dbViewa.db"))
        {
            File.Delete(Application.persistentDataPath + "/dbViewa.db");
        }
    }

    void Start()
    {
        
        if (messageBoxManager == null)
            messageBoxManager = gameObject.GetComponent<MessageBoxManager>();

        ACPUnityPlugin.Instnace.trackEvent("ViewaApp", "AppStart", "", 1);
    }

    private void OnApplicationQuit()
    {
        ACPUnityPlugin.Instnace.trackEvent("ViewaApp", "AppEnd", "", 0);
    }

    void WebCallRegion(string response)
    {
        Debug.Log("Response :" + response);

        JSONNode test = JSONNode.Parse(response);

        if (test != null)
        {
            RegionDataService regionDbService = new RegionDataService();

            if (test.Childs.Count() > 0)
            {
                for (int i = 0; i < test.Childs.Count(); i++)
                {

                    Regions region = new Regions();
                    region.id = test[i]["id"].AsInt;
                    region.name = test[i]["name"].Value;
                    region.client_access_key = test[i]["client_access_key"].Value;
                    region.client_secret_key = test[i]["client_secret_key"].Value;
                    region.image = test[i]["image"];
                    region.autoDetect = test[i]["autoDetect"].AsInt;
                    region.countryCode = test[i]["countryCode"];
                    regionDbService.CreateOrReplaceRegion(region.id, region.name, region.client_access_key, region.client_secret_key, region.image, region.autoDetect, region.countryCode);
                }
            }
        }
    }

#endregion

#region ImageSaveLoad

    public void DeleteProfilePic()
    {
        if (File.Exists(ProfilePicPath))
        {
            File.Delete(ProfilePicPath);
        }
    }

    public string ReturnProfilePicPath(){
        return ProfilePicPath;
    }

    public IEnumerator SavePictureWithUrlAsPNG(string url)
    {
        WWW www = new WWW(url);
        yield return www;
        Texture2D texture = www.texture;
        byte[] bytes = texture.EncodeToPNG();
        File.WriteAllBytes(ProfilePicPath, bytes);
    }
    public IEnumerator SavePictureWithUrlAsJPG(string url)
    {
        WWW www = new WWW(url);
        yield return www;
        Texture2D texture = www.texture;
        byte[] bytes = texture.EncodeToJPG();
        File.WriteAllBytes(ProfilePicPath, bytes);
    }
    public void SavePicture(Texture2D texture, string imagePath)
    {
        if (texture != null)
        {
            byte[] bytes = texture.EncodeToPNG();
            File.WriteAllBytes(imagePath, bytes);
        }
        else
        {
            Debug.Log("AppManager.SavePicture - texture is null");
        }
    }
    public Texture2D LoadPicture(string a_imagePath)
    {
        if (File.Exists(a_imagePath))
        {
            byte[] byteArray = File.ReadAllBytes(a_imagePath);
            Texture2D texture = new Texture2D(200, 200);
            texture.LoadImage(byteArray);
            return texture;
        }
        else
        {
            Debug.Log("Texture does not exist");
            return null;
        }
    }

    public string GetManualProfilePicPath()
    {
        string path = Application.persistentDataPath + "/ProfilePicManual/";

        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
        return (path + WebService.Instnace.appUser.Email);
    }

#endregion

    public string GetUniqueIdentifier()
    {
        System.Guid uid = System.Guid.NewGuid();
        return uid.ToString();
    }

    public void GoToScanScreen()
    {
        if (acpTrackingManager != null)
        {
#if UNITY_ANDROID || UNITY_IOS
            acpTrackingManager.GetComponent<VuforiaBehaviour>().enabled = true;
#endif
            acpTrackingManager.GetComponent<DefaultInitializationErrorHandler>().enabled = true;
            CanvasManager.Instnace.ShowPanelManager(ePanelManager.BottomBarManager);
        }

        CanvasManager.Instnace.ReturnPanelManager(ePanelManager.MainMenuPanelManager).NavigateToPanel(ePanels.Scan_Panel);
    }

    public void PlayClickSound() {
        //audioSourceClick.volume = 0.2f;
        //audioSourceClick.pitch = 0.4f;
        //audioSourceClick.Play();
    }
    public void PlayBackSound() {
        //audioSourceBack.volume = 0.4f;
        //audioSourceBack.pitch = 0.5f;
        //audioSourceBack.Play();
    }

    public void PlayButtonSound()
    {
        //audioSource.volume = 0.4f;
        //audioSource.pitch = 0.8f;
        //audioSource.Play();
    }
    public void PlayButtonSoundWithVibration()
    {
        //audioSource.volume = 0.4f;
        //audioSource.pitch = 0.8f;
        //audioSource.Play();
        //Handheld.Vibrate();
    }

    public GoogleCloudLogin ReturnGoogleCloudLogin() {

        if (googleCloudLogin == null)         {             googleCloudLogin = gameObject.GetComponentInChildren<GoogleCloudLogin>();         }
        return googleCloudLogin;
    }


#region Popup survey functions

    public void ShowSurvayPopup()
    {
        acpTrackingManager.DisableScanning();
        isPopUpSurvay = true;
        CanvasManager.Instnace.ShowPanelManager(ePanelManager.PopupSurvayManager);
        CanvasManager.Instnace.ReturnPanelManager(ePanelManager.PopupSurvayManager).NavigateToPanel(ePanels.QAPopup_panel);
    }

    public bool CheckTrigger(string triggerId)
    {

        if (triggerId == "" || triggerId == null)
        {
            return true;
        }
        string triggerStr = PlayerPrefs.GetString(triggerId, "");

        if (triggerStr == triggerId)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void CacheTrigger(string triggerId)
    {
        PlayerPrefs.SetString(triggerId, triggerId);
    }

    //**************************************************************************
    //                      PopUp Survey Question Call
    //**************************************************************************

    public void GetQuestion()
    {

        string url = AppManager.Instnace.baseURL + "/cloud/PopUpSurveyServicesQA.aspx?Tid=" + WebService.Instnace.catDetailParameter.HubTrigger + "&EmailId=" + AppManager.Instnace.userEmail;
        ///cloud/PopUpSurveyServices.aspx?Tid=
        ///cloud/PopUpSurveyServicesQA.aspx?Tid=
        Debug.Log("GetQuestionResponse url:" + url);
        StartCoroutine(GetQuestionResponse(url));
    }

    IEnumerator GetQuestionResponse(string url)
    {

        WWW response = new WWW(url);

        yield return response;

        Debug.Log("GetQuestionResponse Response :" + response.text);
        popUpQuestionsResponse = response.text;
    }
	
	void OnApplicationPause(bool paused)
    {
        if (paused)
        {
            AppManager.Instnace.messageBoxManager.HidePreloader();
            GC.Collect();
        }
    }

    public void ExitApplication()
    {
        Application.Quit();
    }

#endregion

}




