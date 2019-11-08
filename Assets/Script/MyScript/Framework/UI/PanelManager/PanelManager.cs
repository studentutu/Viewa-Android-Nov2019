//----------------------------------------------
//       OTPL - Jeetesh
//       created: 5 Dec 2017
//       Copyright © 2017 OTPL
//       Panel Manager is responsible for opening and closing the panels. 
//----------------------------------------------
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OTPL.UI;
using UnityEngine.UI;
using ACP;

public class PanelManager : MonoBehaviour {

    public ePanelManager m_name;
    public PanelBase initiallyOpenPanel;
    public List<PanelBase> panelList;
	public Stack<ePanels> panelStack = new Stack<ePanels>();
    public PanelBase _currentOpenPanel;


    public PanelBase currentOpenPanel {

        get {
            if(_currentOpenPanel != null){
                return _currentOpenPanel;
            } 
            return _currentOpenPanel;
        }
        set {
            _currentOpenPanel = value;
        }
    }

	#region Unity function
	/// <summary>
	/// Awake - Activates and shows the initiallyOpenPanel in the PanelManager and de-activate all other panels.
	/// </summary>
    public void Awake()
    {
        int childCount = transform.childCount;
        for (int i = 0; i < childCount; i++){
            panelList.Add(transform.GetChild(i).GetComponent<PanelBase>());
            panelList[i].gameObject.SetActive(false);
        }
    }

	public void OnEnable()
	{
        LoadInitiallyOpenPanel();
	}

    public void LoadInitiallyOpenPanel() {
       
        if (initiallyOpenPanel != null)
        {
            initiallyOpenPanel.gameObject.SetActive(true);
            initiallyOpenPanel.transform.SetAsLastSibling();
        }
    }
	#endregion

	#region Panel Navigation
	/// <summary>
	/// Navigates to panel - Here we are pushing the panel to stack.
	/// </summary>
	/// <param name="e_panelName">E panel name.</param>
	/// <param name="prevPanelState">set to <c>false/true</c> to make prev panel in-active/active.</param>
	public void NavigateToPanel(ePanels e_panelName)
	{
		AppManager.Instnace.messageBoxManager.HidePreloader();
      
        foreach(PanelBase panel in panelList){
            if (panel.m_panelName == e_panelName)
            {
                if (panel.m_panelName == ePanels.Scan_Panel)
                    panelStack.Clear();
                
                if (!panel.gameObject.activeSelf){
                    //panel.gameObject.SetActive(true);
                    if(currentOpenPanel != null){
                        currentOpenPanel.gameObject.SetActive(false);
                    }

                    panel.gameObject.SetActive(true);

                    panel.transform.SetAsLastSibling();
					currentOpenPanel = panel;
					break;
                } else {
                    if (currentOpenPanel != null){
                        currentOpenPanel.gameObject.SetActive(false);
                    }
                    panel.gameObject.SetActive(true);

                    panel.transform.SetAsLastSibling();
                    currentOpenPanel = panel;
                }
            }
        }
    }

    void loadPanelAsInitialPanel(ePanels mypanelName)
    {
        foreach (PanelBase panel in panelList)
        {
            if (panel.m_panelName == mypanelName)
            {
                if (!panel.gameObject.activeSelf)
                {
                    panel.gameObject.SetActive(true);
                    panel.transform.SetAsLastSibling();
                    currentOpenPanel = panel;
                    break;
                }
            }
        }
    }

	/// <summary>
	/// Backs to panel - Here we pop the panel from stack and de-activate the poped panel.
	/// </summary>
	/// <param name="e_panelName">E panel name.</param>
	public void BackToPanel(ePanels e_panelName)
	{
        //AppManager.Instnace.PlayBackSound();
		AppManager.Instnace.messageBoxManager.HidePreloader();

		if (currentOpenPanel.m_panelName == e_panelName) {
			return;
		}
        if (currentOpenPanel.m_panelName == ePanels.Scan_Panel)
        {
            panelStack.Clear();
        }
        if (panelStack.Count > 0)
        {
            panelStack.Pop();
        }
       
		foreach(PanelBase panel in panelList){
			if (panel.m_panelName == e_panelName)
			{
				if (!panel.gameObject.activeSelf){
                  
                    panel.gameObject.SetActive(true);

                    panel.SetShowScreenRight();
					panel.transform.SetAsLastSibling ();
                    //currentOpenPanel.SetHideScreenRight();
                    StartCoroutine(BackToPanelDelay(panel));

					break;
				}
			}
		}
	}

    IEnumerator BackToPanelDelay(PanelBase a_panel) {
        yield return new WaitForSeconds(0.5f);
        currentOpenPanel.OnPanelClose(false);
        currentOpenPanel = a_panel;
    }

    public void AddPanel(ePanels e_panelName) {
        
        if (currentOpenPanel == null)
        {
            loadPanelAsInitialPanel(e_panelName);
            return;
        }

        //Panels can stack up to 30 panels without going back.
        if (panelStack.Count <= 30)
        {
            panelStack.Push(currentOpenPanel.m_panelName);
        }

        foreach (PanelBase panel in panelList)
        {
            if (panel.m_panelName == e_panelName)
            {
                if (!panel.gameObject.activeSelf)
                {
                   
                    panel.gameObject.SetActive(true);

                    panel.SetShowScreenLeft();
                    panel.transform.SetAsLastSibling();
                    //currentOpenPanel.SetHideScreenLeft();
                    StartCoroutine(BackToPanelDelay(panel));

                    break;
                } 
            }
        }
    }
    IEnumerator AddPanelDelay(PanelBase a_panel)
    {
        yield return new WaitForSeconds(0.5f);
        currentOpenPanel.OnPanelClose(false);
        currentOpenPanel = a_panel;
    }

	public void ShowPanel(ePanels e_panelName){

		foreach (PanelBase panel in panelList) {
			if (panel.m_panelName == e_panelName) {
				if (!panel.gameObject.activeSelf) {
					panel.gameObject.SetActive (true); 
					panel.transform.SetAsLastSibling ();
				}
			}
		}
	}

	public void HidePanel(ePanels e_panelName){

		if (panelStack.Count > 0) {
			foreach (PanelBase panel in panelList) {
				if (panel.m_panelName == panelStack.Peek ()) {
					if (!panel.gameObject.activeSelf) {
						panel.gameObject.SetActive (true); 
						panel.transform.SetAsLastSibling ();
						currentOpenPanel.OnPanelClose (false);
						currentOpenPanel = panel;
					}
				}
			}
		}
	}

	public bool ReturnPanelStatus(ePanels e_panelName){

		foreach (PanelBase panel in panelList) {
			if (panel.m_panelName == e_panelName) {
				if (panel.gameObject.activeSelf) {
					return true; 
				} else {
					return false;
				}
			}
		}
		return false;
	}

	public PanelBase ReturnPanel(ePanels e_panelName){

		foreach (PanelBase panel in panelList) {
			if (panel.m_panelName == e_panelName) {
				return panel;  
			}
		}
		return null;
	}
	#endregion

}
