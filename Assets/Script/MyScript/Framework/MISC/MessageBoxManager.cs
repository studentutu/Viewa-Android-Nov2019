using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MessageBoxManager : MonoBehaviour {

	public string appleId = "itms-apps://itunes.apple.com/id375380948?mt=8";
	public string androidAppUrl = "market://details?id=com.google.earth";
    public GenericPopup genericPopup;

	public delegate void MsgCallBack();
	MsgCallBack msgCallBack;

	private void Start()
	{
        //Deactivate Generic popup.
        genericPopup.gameObject.SetActive(false);
	}

	/// <summary>
	/// Shows the message.
	/// </summary>
	/// <param name="heading">Heading.</param>
	/// <param name="desc">Desc.</param>
	/// <param name="buttontxt">Buttontxt.</param>
	public void ShowMessage(string heading, string desc, string buttontxt){

		//User already exists please select a different email id.
		MNPopup popup = new MNPopup (heading, desc);
		popup.AddAction (buttontxt, () => {
			Debug.Log ("Ok action callback");
		});
		popup.AddDismissListener (() => {
			Debug.Log ("dismiss listener");
		});
		popup.Show ();
	}

	public void ShowMessage(string heading, string desc, string buttontxt, MsgCallBack msgCallBack){

		//User already exists please select a different email id.
		MNPopup popup = new MNPopup (heading, desc);
		popup.AddAction (buttontxt, () => {
			Debug.Log ("Ok action callback");
			msgCallBack();
		});
		popup.AddDismissListener (() => {
			Debug.Log ("dismiss listener");
		});
		popup.Show ();
	}

	/// <summary>
	/// Shows the message with two buttons.
	/// </summary>
	/// <param name="heading">Heading.</param>
	/// <param name="desc">Desc.</param>
	/// <param name="button1">Button1.</param>
	/// <param name="button2">Button2.</param>
	public void ShowMessageWithTwoButtons(string heading, string desc, string button1, string button2){
	
		MNPopup popup = new MNPopup (heading, desc);
		popup.AddAction (button1, () => {
			Debug.Log("button 1 action callback");
		});
		popup.AddAction (button2, () => {
			Debug.Log("button 2 action callback");
		});
		popup.AddDismissListener (() => {Debug.Log("dismiss listener");});
		popup.Show ();
	}

	/// <summary>
	/// Shows the message with two buttons.
	/// </summary>
	/// <param name="heading">Heading.</param>
	/// <param name="desc">Desc.</param>
	/// <param name="button1">Button1.</param>
	/// <param name="button2">Button2.</param>
	public void ShowMessageWithTwoButtons(string heading, string desc, string button1, string button2, MsgCallBack msgCallOk){

		MNPopup popup = new MNPopup (heading, desc);
		popup.AddAction (button1, () => {
			Debug.Log("button 1 action callback");
			msgCallOk();
		});
		popup.AddAction (button2, () => {
			Debug.Log("button 2 action callback");
		});
		popup.AddDismissListener (() => {Debug.Log("dismiss listener");});
		popup.Show ();
	}

	/// <summary>
	/// Shows the rate my app.
	/// </summary>
	public void ShowRateMyApp(){
	
		MNRateUsPopup rateUs = new MNRateUsPopup ("Rate Us", "Rate Viewa on Play Store", "Rate Us", "No, Thanks", "Later");
		rateUs.SetAppleId (appleId);
		rateUs.SetAndroidAppUrl (androidAppUrl);
		rateUs.AddDeclineListener (() => { Debug.Log("rate us declined"); });
		rateUs.AddRemindListener (() => { Debug.Log("remind me later"); });
		rateUs.AddRateUsListener (() => { Debug.Log("rate us!!!"); });
		rateUs.AddDismissListener (() => { Debug.Log("rate us dialog dismissed :("); });
		rateUs.Show ();
	}

	public void ShowPreloaderDefaultWithBackGround(){
		#if UNITY_EDITOR || UNITY_IPHONE || UNITY_ANDROID
		if(Loader.Instnace != null)
			Loader.Instnace.ShowLoaderWithBackGround();
        
		//MNP.ShowPreloader("","");
		#endif
	}
	public void ShowPreloaderDefault(){
        #if UNITY_EDITOR || UNITY_IPHONE || UNITY_ANDROID
		if(Loader.Instnace != null)
			Loader.Instnace.ShowLoader();
		
		//MNP.ShowPreloader("","");
		#endif
	}
	public void ShowPreloaderDefault(float time){
        #if UNITY_EDITOR || UNITY_IPHONE || UNITY_ANDROID
		if(Loader.Instnace != null)
			Loader.Instnace.ShowLoader(time);
		
			//MNP.ShowPreloader("","");
			//Invoke("OnPreloaderTimeOut", time);
		#endif
	}
	public void ShowPreloader(string title, string msg){
        #if UNITY_EDITOR || UNITY_IPHONE || UNITY_ANDROID
		if(Loader.Instnace != null)
			Loader.Instnace.ShowLoader();
		
			//MNP.ShowPreloader(title, msg);
			//Invoke("OnPreloaderTimeOut", 3f);
		#endif
	}
	public void ShowPreloader(string title, string msg, float time){
        #if UNITY_EDITOR || UNITY_IPHONE || UNITY_ANDROID
		if(Loader.Instnace != null)
			Loader.Instnace.ShowLoader(time);
		
			//MNP.ShowPreloader(title, msg);
			//Invoke("OnPreloaderTimeOut", time);
		#endif
	}
	public void HidePreloader(){
        #if UNITY_EDITOR || UNITY_IPHONE || UNITY_ANDROID
		if(Loader.Instnace != null)
			Loader.Instnace.HideLoader();
		
			//MNP.HidePreloader();
		#endif
	}
	private void OnPreloaderTimeOut() {
        #if UNITY_EDITOR || UNITY_IPHONE || UNITY_ANDROID
		if(Loader.Instnace != null)
			Loader.Instnace.HideLoader();
		
			//MNP.HidePreloader();
		#endif
	}
	public void ShowNativeLoader(){
		MNP.ShowPreloader("","");
	} 
	public void HideNativeLoader(){
		MNP.HidePreloader();
	}

    //Show generic popup.
    public void ShowGenericPopup(string heading, string desc, string buttonText)
    {
        genericPopup.gameObject.SetActive(true);
        genericPopup.Show(heading, desc, buttonText);
    }

    public void ShowGenericPopup(string heading, string desc, string buttonText, GenericPopup.GenericCallBack gCallBack)
    {
        genericPopup.gameObject.SetActive(true);
        genericPopup.Show(heading, desc, buttonText, gCallBack);
    }

    public void ShowGenericPopupWithButtons(string heading, string desc, string buttonText, string buttonText2, GenericPopup.GenericCallBack gCallBack)
    {
        genericPopup.gameObject.SetActive(true);
        genericPopup.Show(heading, desc, buttonText, buttonText2, gCallBack);
    }
}
