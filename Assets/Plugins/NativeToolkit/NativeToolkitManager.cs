using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.SceneManagement;



public class NativeToolkitManager : MonoBehaviour
{
	// Delegates and events
	public delegate void ApplicationWillResignActiveHandler(); 
	
	// Fired whenever an interruption occurs (text message, push notification, app gets sent to background, etc)
	public static event ApplicationWillResignActiveHandler appWillResignActive;
	
	
    void Awake()
    {
		gameObject.name = this.GetType().ToString();
    }
    
    
    public void applicationWillResignActive( string empty )
    {
    	if( appWillResignActive != null )
    		appWillResignActive();
    }
    
    
	public void loadLevel( string scene )
	{
//		Application.LoadLevel( scene );
		SceneManager.LoadScene (scene);
	}
	
	
	public void loadLevelAsync( string scene )
	{
//		Application.LoadLevelAsync( scene );
		SceneManager.LoadSceneAsync (scene);
	}
	
	
	public void loadLevelAdditive( string scene )
	{
//		Application.LoadLevelAdditive( scene );
		SceneManager.LoadSceneAsync (scene);
	}
	
	
	public void loadLevelAdditiveAsync( string scene )
	{
//		Application.LoadLevelAdditiveAsync( scene );
		SceneManager.LoadSceneAsync (scene, LoadSceneMode.Additive);
	}

}