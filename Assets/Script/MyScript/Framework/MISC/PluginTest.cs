using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PluginTest
{
    //const string pluginName = "au.com.mocom.unity.MyPlugin";
    const string pluginName = "au.com.mocom.unity.FlashLight";

    static AndroidJavaClass _pluginClass;
    static AndroidJavaObject _pluginInstance;

    public static AndroidJavaClass PluginClass
    {
        get {

            if(_pluginClass == null){
                _pluginClass = new AndroidJavaClass(pluginName);
            }
            return _pluginClass;
        }
    }

    public static AndroidJavaObject PluginInstance 
    {
        get {
            if(_pluginInstance == null) {
                _pluginInstance = PluginClass.CallStatic<AndroidJavaObject>("getInstance");
            }
            return _pluginInstance;
        }    
    }

    // Start is called before the first frame update
    public static void CallPluginMethod()
    {
        //Debug.Log("PluginText:----->" + PluginClass.CallStatic<string>("GetTextFromPlugin", 100000000));
        Debug.Log("CallPluginMethod - enableFlash Called");
        AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
        AndroidJavaObject context = activity.Call<AndroidJavaObject>("getApplicationContext");
        PluginClass.CallStatic<bool>("enableFlash", context, Vuforia.CameraDevice.Instance);
        
    }
}
