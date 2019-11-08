using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using System.IO;
using System.Linq;
using SimpleJSON;

public class LocalizeStringManager {

	static JSONNode localizeStringNode;

    public static IEnumerator LoadGameData()
	{
		// Path.Combine combines strings into a file path
		// Application.StreamingAssets points to Assets/StreamingAssets in the Editor, and the StreamingAssets folder in a build
        string filePath = Path.Combine(Application.streamingAssetsPath, "Localizable_English.txt");

        WWW wwwfile = new WWW(filePath);

        yield return wwwfile;

        if(wwwfile.text != null && wwwfile.text != "") { 
			// Read the json from the file into a string
            string dataAsJson = wwwfile.text;   //File.ReadAllText(filePath); 
			// Pass the json to JsonUtility, and tell it to create a GameData object from it
			localizeStringNode = JSONNode.Parse (dataAsJson);
			Debug.Log("Game data loaded Successfully");
		}
		else
		{
			Debug.LogError("Cannot load game data!");
		}
	}

	public static string ReturnLocalizeString(string localizeString){

		string returnStringValue = localizeStringNode [localizeString].Value;
		if(returnStringValue != null && returnStringValue != ""){
			return returnStringValue;
		} else {
			Debug.Log ("No localized data for" + localizeString + " available.");
			return "No Data";
		}
	}
		
}

