//----------------------------------------------
//       OTPL - Jeetesh
//       created: 5 Dec 2017
//       Copyright © 2017 OTPL
//       Description: Generic singelton, all singeltons in the game are derived from this class.
//----------------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T>: MonoBehaviour where T: MonoBehaviour
{
    private static T instance;

    public static T Instnace {

        get {
            if (instance == null)
            {
                instance = (T)FindObjectOfType(typeof(T));

                if (instance == null)
                {
                    GameObject container = new GameObject(typeof(T) + "-Singleton");
                    instance = (T)container.AddComponent(typeof(T));    
                }
            }
            return instance;
        }
    }
}
