//----------------------------------------------
//       OTPL - Jeetesh
//       created: 5 Dec 2017
//       Copyright © 2017 OTPL
//       Canvas Manager manages all Panel Managers. 
//----------------------------------------------
using System.Collections;
using System.Collections.Generic;   
using UnityEngine;
using OTPL.UI;
using UnityEngine.UI;

public class CanvasManager: Singleton<CanvasManager> {

    public List<PanelManagerContainer> panelManagerList;


	/// <summary>
	/// Awake - Hide and show the Initial Screens - ie. PanelManagers at the start of the app.
	/// </summary>
    private void Awake() {
		
//		HidePanelManager (ePanelManager.BottomBarManager);
//		ShowPanelManager (ePanelManager.MainMenuPanelManager);
    }

	/// <summary>
	/// Returns the panel manager.
	/// </summary>
	/// <returns>The panel manager.</returns>
	/// <param name="panelManagerName">Return the Panel (Panel manager name).</param>
    public PanelManager ReturnPanelManager(ePanelManager panelManagerName){

		for (int i = 0; i < panelManagerList.Count; i++){

			if (panelManagerList [i].e_PanelManager == panelManagerName) {

//				if (panelManagerList [i].m_PanelManager.gameObject.activeSelf) {
					return panelManagerList [i].m_PanelManager;
//				}
			}
        }
        return null;
    }

	public bool ReturnStatus(ePanelManager panelManagerName){
		for (int i = 0; i < panelManagerList.Count; i++){

			if (panelManagerList [i].e_PanelManager == panelManagerName) {

				if (panelManagerList [i].m_PanelManager.gameObject.activeSelf) {
					return true;
				} else {
					return false;
				}
			}
		}
		return false;
	}

	/// <summary>
	/// Shows the panel manager.
	/// </summary>
	/// <param name="panelManagerName">Panel manager name.</param>
    public void ShowPanelManager(ePanelManager panelManagerName) {

		for (int i = 0; i < panelManagerList.Count; i++)
        {
			if (panelManagerList [i].e_PanelManager == panelManagerName) {

				if (panelManagerList [i].m_PanelManager != null) {
						panelManagerList [i].m_PanelManager.gameObject.SetActive (true);
                    panelManagerList[i].m_PanelManager.LoadInitiallyOpenPanel();
				}
			} 
        }
    }

    /// <summary>
    /// Hides the panel manager.
    /// </summary>
    /// <param name="panelManagerName">Panel manager name.</param>
    public void HidePanelManager(ePanelManager panelManagerName)
    {
        for (int i = 0; i < panelManagerList.Count; i++)
        {
            if (panelManagerList[i].e_PanelManager == panelManagerName)
            {
                panelManagerList[i].m_PanelManager.currentOpenPanel = null;
                for (int j = 0; j < panelManagerList[i].m_PanelManager.panelList.Count; j++)
                {
                    panelManagerList[i].m_PanelManager.panelList[j].gameObject.SetActive(false);
                }
            }
        }
    }

	/// <summary>
	/// Start Animate current open panel in manager.
	/// </summary>
	/// <param name="panelManagerName">Panel manager name.</param>
	public void StartAnimateCurrentPanelInPanelManager(ePanelManager panelManagerName) {

		for (int i = 0; i < panelManagerList.Count; i++)
		{
			if (panelManagerList[i].e_PanelManager == panelManagerName) {

				if (panelManagerList [i].m_PanelManager != null) {
					panelManagerList [i].m_PanelManager.gameObject.SetActive (true);
					panelManagerList [i].m_PanelManager.currentOpenPanel.OnPanelStartAnimate ();
				}
			}
		}
	}

	/// <summary>
	/// End Animate current open panels in panel manager.
	/// </summary>
	/// <param name="panelManagerName">Panel manager name.</param>
	public void EndAnimateCurrentPanelInPanelManager(ePanelManager panelManagerName) {

		for (int i = 0; i < panelManagerList.Count; i++)
		{
			if (panelManagerList[i].e_PanelManager == panelManagerName) {

				if (panelManagerList [i].m_PanelManager.gameObject.activeSelf) {
					panelManagerList [i].m_PanelManager.currentOpenPanel.OnPanelEndAnimate ();
				} else {
					panelManagerList [i].m_PanelManager.gameObject.SetActive (true);
					panelManagerList [i].m_PanelManager.currentOpenPanel.OnPanelEndAnimate ();
				}
			}
		}
	}
}

/*Contains all Panel manager in game [ Canvas in game ]
 * */
[System.Serializable]
public class PanelManagerContainer {
    
    public ePanelManager e_PanelManager;
    public PanelManager m_PanelManager;
}



