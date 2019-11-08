//------------------------------------------------
// Description: Base class for all panels
//------------------------------------------------
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using OTPL.UI;

public class PanelBase : UIController
{
	public ePanels m_panelName;
	public PanelManager myManager;
	public Button[] buttons;
	[HideInInspector]
	public Transform statusBar;
	[HideInInspector]
	public Transform navBar;
    Animator m_animator;


    #region Panel functions
    /// <summary>
    /// Awake the Base Panel.
    /// </summary>
	protected virtual void Awake()
    {
        waiteTime = 0;
        //On Panel Active - Call this on Awake of each panel.
        Debug.Log("Base Panel Awake()");
        m_animator = gameObject.GetComponent<Animator>();
		myManager = gameObject.GetComponentInParent<PanelManager> ();
		buttons = gameObject.GetComponentsInChildren<Button>();
		statusBar = transform.Find ("StatusBar");
		navBar = transform.Find ("NavigationBarPanel");
    }

	protected virtual void Start()
	{
        waiteTime = 0;
		IphoneXFixFunction ();
	}
	/// <summary>
	/// Raises the enable event.
	/// </summary>
	protected virtual void OnEnable()
	{
        waiteTime = 0;
		Debug.Log("Base Panel OnEnable() - "+ gameObject.name);
		if (buttons.Length > 0) {
			foreach (Button button in buttons) {
				button.transform.SetAsLastSibling ();
				button.onClick.AddListener (() => OnUIButtonClicked (button));
			}
		}
		//isShow = true;
	}

	/// <summary>
	/// Raises the disable event.
	/// </summary>
	protected virtual void OnDisable()
	{
		Debug.Log("Base Panel OnDisable() - " + gameObject.name);
		if (buttons.Length > 0) {
			foreach (Button button in buttons) {
				button.onClick.RemoveAllListeners ();
			}
		}
	}

	/// <summary>
	/// Raises the user interface button clicked event.
	/// </summary>
	/// <param name="a_button">button Clicked on panel</param>
	protected virtual void OnUIButtonClicked(Button a_button){

		AppManager.Instnace.PlayButtonSoundWithVibration ();
		/* Can put the button sound here.
		 **/
		switch (a_button.name) {

		case "LeftButton":
			Debug.Log ("Button selected -" + a_button.name);
			AppManager.Instnace.PlayButtonSoundWithVibration ();
			if (myManager.panelStack.Count > 0) {
				myManager.BackToPanel (myManager.panelStack.Peek ());
			}
			break;
		}
		
	}

    /// <summary>
    /// Panel Open Animation - This is kept public so that the panels can play the animations even after they 
    /// are in open state.
    /// </summary>
    public virtual void OnPanelStartAnimate()
    {
        //Play open panel animation if associated
        if (m_animator != null)
        {
            //Play panel start animation here. 
        }
    }

	public virtual void OnPanelEndAnimate(){

		//Play open panel animation if associated
		if (m_animator != null)
		{
			//Play panel end animation here. 
		}
	
	}

	//TODO: Do we really require this? Can't we do open/close animation in OnPanelAnimate function with panel state.
	//		Animation can't play if you close the gameObject instantly.  Where to play closing Animation?

    /// <summary>
    /// Raises the panel close event.
    /// </summary>
    /// <param name="active">Want panel to be Active after closing<c>true</c> active.</param>
    public virtual void OnPanelClose(bool active)
    {

        //Play close panel animation if associated
        if (m_animator != null)
        {
            //Play panel close animation here.
            //isShow = false;
            gameObject.SetActive(false);
        } else {
            gameObject.SetActive(false);
        }
        //deactivate the panel after completing the animation.
//        gameObject.SetActive(active);
		
    }
    #endregion //Panel functions
	public void IphoneXFixFunction() {
		
		Debug.Log("Base Panel IphoneXFixFunction()");
		if (AppManager.Instnace.isIphoneX) {
			if (statusBar != null) {
				RectTransform rectTransform = statusBar.GetComponent<RectTransform> ();
				rectTransform.sizeDelta = new Vector2 (rectTransform.sizeDelta.x, rectTransform.sizeDelta.y + 40f);
			}
			if (navBar != null) {
				RectTransform rectTransform = navBar.GetComponent<RectTransform> ();
				rectTransform.localPosition = new Vector3 (0,rectTransform.localPosition.y-40, 0);
				//				rectTransform.localPosition = new Vector3 (0,rectTransform.localPosition.y - 30, 0);
				//				rectTransform.offsetMax = new Vector2 (rectTransform.offsetMax.x, (40f) * -1);	//Top
			}
		}
	}
   
    float waiteTime = 0;
    protected virtual void Update()
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            if (Input.GetKey(KeyCode.Escape))
            {
                //if (waiteTime > 0 && waiteTime < 1){
                //    return;
                //} else {
                //    waiteTime = 0;
                //}
                    
                
                    //waiteTime += Time.deltaTime;
                
                if (myManager.currentOpenPanel.m_panelName == ePanels.Hub_Panel
                    || myManager.currentOpenPanel.m_panelName == ePanels.SideMenuPanel
                    || myManager.currentOpenPanel.m_panelName == ePanels.Scan_Panel
                    || myManager.currentOpenPanel.m_panelName == ePanels.Loved_Panel
                    || myManager.currentOpenPanel.m_panelName == ePanels.GetStarted_Panel)
                {
                    //Show a dialog box and exit the app here if user press device back button here.
                    AppManager.Instnace.messageBoxManager.ShowGenericPopupWithButtons("Alert", "Are you sure you want to exit the app?", "Yes", "Cancel", AppManager.Instnace.ExitApplication);
                }
                else if(myManager.currentOpenPanel.m_panelName == ePanels.ImageGallary_Panel){
                    if (AppManager.Instnace.isGoingToGalleryFromScan)
                    {
                        AppManager.Instnace.isVuforiaOn = true;
                    }
                    else
                    {
                        AppManager.Instnace.isVuforiaOn = false;
                    }

                    AppManager.Instnace.acpTrackingManager.OnBackButtonTapped();
                }
                else {
                    
                    myManager.BackToPanel(myManager.panelStack.Peek());
                }
            }
        }
    }
}
