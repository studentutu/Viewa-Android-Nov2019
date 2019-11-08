using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OTPL.UI;
using UnityEngine.UI; 
using System.Text;
using System.IO;

public class SideMenuPanel : PanelBase {

    [SerializeField] Text userName;
    [SerializeField] Text userEmail;
    public Image _myProfileImage;
    [SerializeField] Sprite _ProfileSprite;
    [SerializeField] GameObject popUpObject;

    //[SerializeField] Image profile_image;
    //[SerializeField] Image about_Image;
    //[SerializeField] Image region_Image;
    //[SerializeField] Image help_image;
    //[SerializeField] Image editProflePic;
	
    UserInfo _user;
    string mobileNumber;
    Texture2D profilePic;

	protected override void Awake ()
	{
		base.Awake ();

        //profile_image.sprite = AppManager.Instnace.spriteAtlas.GetSprite("Menu_Profile");
        //about_Image.sprite = AppManager.Instnace.spriteAtlas.GetSprite("Menu_AboutViewa");
        //region_Image.sprite = AppManager.Instnace.spriteAtlas.GetSprite("Menu_Region");
        //help_image.sprite = AppManager.Instnace.spriteAtlas.GetSprite("Menu_Help");
        //editProflePic.sprite = AppManager.Instnace.spriteAtlas.GetSprite("Menu_EditProfile");
	}

	protected override void OnEnable ()
	{
		base.OnEnable ();

        popUpObject.SetActive(false);

		StopAllCoroutines ();

        profilePic = AppManager.Instnace.LoadPicture(AppManager.Instnace.GetManualProfilePicPath());
        if (profilePic != null){
            profilePic = AppManager.Instnace.LoadPicture(AppManager.Instnace.GetManualProfilePicPath());
        }
        else {
            profilePic = AppManager.Instnace.LoadPicture(AppManager.Instnace.ReturnProfilePicPath());
        }
        if (profilePic != null){
            TextureScale.Bilinear(profilePic, 200, 200);
            _myProfileImage.sprite = Sprite.Create(profilePic, new Rect(0, 0, 200, 200), new Vector2());
        }
        else
            _myProfileImage.sprite = _ProfileSprite;

        UserDataService userDbService = new UserDataService();
        UserInfo user = userDbService.GetUser();


        //userName.text = WebService.Instnace.appUser.FirstName + " " + WebService.Instnace.appUser.LastName;
        //userEmail.text = WebService.Instnace.appUser.Email;
        //mobileNumber = WebService.Instnace.appUser.Mobile;    



        userName.text = user.FirstName + " " + user.LastName;
        userEmail.text = user.Email;
        mobileNumber = user.Mobile;    

        if (mobileNumber.Length <= 0){
            popUpObject.SetActive(true);
        }
        else{
            popUpObject.SetActive(false);
        }

        //UserDataService userDbService = new UserDataService();
        //_user = userDbService.GetUser();
        //if (_user != null)
        //{
        //    userName.text = _user.FirstName + " " + _user.LastName; 
        //    userEmail.text = _user.Email;
        //    mobileNumber = _user.Mobile;
        //    if(mobileNumber.Length <= 0){
        //        popUpObject.SetActive(true);
        //    } else {
        //        popUpObject.SetActive(false);
        //    }
        //}
 
        //tracking
        ACPUnityPlugin.Instnace.trackScreen("SideMenu");
	}

	protected override void OnDisable ()
	{
		base.OnDisable ();

		StopAllCoroutines ();
	}
	protected override void OnUIButtonClicked (Button a_button)
	{
		base.OnUIButtonClicked (a_button);

		switch (a_button.name)
		{
		case "HelpButton":
			Debug.Log ("Button selected -" + a_button.name);
                myManager.AddPanel (ePanels.Help_Panel);
			break;
		case "AboutButton":
			Debug.Log("Button selected -" + a_button.name);
                myManager.AddPanel(ePanels.About_Panel);
			break;
		case "ProfileButton":
			Debug.Log("Button selected -" + a_button.name);
                myManager.AddPanel(ePanels.Profile_Panel);
			break;
		case "RegionButton":
			Debug.Log("Button selected -" + a_button.name);
                myManager.AddPanel(ePanels.Region_Panel);
			break;
        case "Yes_Button":
            Debug.Log("Button selected -" + a_button.name);
                ProfilePanel profilePanel = (ProfilePanel)CanvasManager.Instnace.ReturnPanelManager(ePanelManager.MainMenuPanelManager).ReturnPanel(ePanels.Profile_Panel);
                profilePanel.isComingFromMobileNotification = true;
            myManager.AddPanel(ePanels.Profile_Panel);
            break;
        case "Cancel_Button":
            Debug.Log("Button selected -" + a_button.name);
                popUpObject.SetActive(false);
            break;
        case "EditProfilePicButton":
            Debug.Log("Button selected -" + a_button.name);
            NPBinding.UI.SetPopoverPointAtLastTouchPosition();
            AppManager.Instnace.socialSharingScript.PickImageFromAlbum();
            break;
		}
	}
}
