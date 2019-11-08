using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;

/// <summary>
/// This was just an experiment ;)
/// </summary>

public class VideoPlayer1 : MonoBehaviour
{
	public int TextureWidth = 256;
	public int TextureHeight = 256;
	private Texture2D texture;
	
	public float lastUpdateTime = 0.0f;
	
	void Start() {
		texture = new Texture2D( TextureWidth, TextureHeight, TextureFormat.RGBA32, false);
		texture.Apply();
		
		this.gameObject.GetComponent<MeshRenderer>().material.mainTexture = texture;
		
//		ACPUnityPlugin.setupVideo("TEST:VIDEOURL", texture.GetNativeTexturePtr(), TextureWidth, TextureHeight);
//		ACPUnityPlugin.playVideo(texture.GetNativeTexturePtr(), Time.time);

		//AndroidJNIHelper
	}
	
	void Update() 
	{
		if (Input.touchCount >= 1 && Input.touches[0].phase == TouchPhase.Ended)
		{
			Vector2 touch = Input.touches[0].position;
			RaycastHit hit;
			if (Physics.Raycast(Camera.main.ScreenPointToRay(touch), out hit))
			{
				Debug.Log("Displaying view controller");
#if UNITY_IPHONE
//				jeetesh- Build
//				NativeToolkitBinding.activateUIWithController("TestViewController");
#endif
//#if UNITY_ANDROID
//				ACPUnityPlugin.startActivityWithName("TestActivity");
//#endif
			}
		}
		
		//updateTexture(texture.GetNativeTexturePtr(), TextureWidth, TextureHeight, Time.time);
		
#if UNITY_WEBPLAYER
#elif UNITY_IPHONE
		if (UnityEngine.iOS.Device.generation == UnityEngine.iOS.DeviceGeneration.iPhone3GS || UnityEngine.iOS.Device.generation == UnityEngine.iOS.DeviceGeneration.iPodTouch3Gen)
		{
			if ((Time.time - this.lastUpdateTime) > (0.25))
			{
//				ACPUnityPlugin.Instnace.updateVideo((int)texture.GetNativeTexturePtr(), Time.time);  //(int)texture.GetNativeTexturePtr()
				lastUpdateTime = Time.time;
			}
		}
		else
#endif
		{
//			ACPUnityPlugin.Instnace.updateVideo((int)texture.GetNativeTexturePtr(), Time.time); //(int)texture.GetNativeTexturePtr()
		}
	}	
	
    void OnApplicationPause(bool pause)
    {
		Debug.Log("VideoPlayer::OnApplicationPause");
	}	
	
}
