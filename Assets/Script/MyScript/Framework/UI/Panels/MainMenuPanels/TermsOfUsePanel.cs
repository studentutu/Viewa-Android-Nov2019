using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;

public class TermsOfUsePanel : PanelBase
{

    public Text[] textField = new Text[4];
    string[] filePathArray = new string[4];
    string path;

    protected override void Awake()
    {
        base.Awake();

        for (int i = 0; i < filePathArray.Length; i++)
        {
            string filePath = Path.Combine(Application.streamingAssetsPath, "TermsOfUse" + i + ".txt");
            StartCoroutine(ReadFile(filePath, textField[i]));
        }
	}

    IEnumerator ReadFile(string a_filePath, Text a_textField)
    {

        // Read the json from the file into a string
        WWW wwwfile = new WWW(a_filePath);

        yield return wwwfile;

        string content = wwwfile.text; //File.ReadAllText(a_filePath);
        a_textField.text = content;
        Debug.Log("TermsOfService file loaded Successfully");

    }
		
	protected override void OnEnable ()
	{
		base.OnEnable ();

        //tracking
        ACPUnityPlugin.Instnace.trackScreen("TermsOfUse");
	}

	protected override void OnDisable ()
	{
		base.OnDisable ();

	}

	protected override void OnUIButtonClicked (UnityEngine.UI.Button a_button)
	{
		base.OnUIButtonClicked (a_button);
	}

	//private void Update()
	//{
 //       //Jeetesh - functionality to go back from Device back button.
 //       if (Application.platform == RuntimePlatform.Android)
 //       {
 //           if (Input.GetKey(KeyCode.Escape))
 //           {
 //               myManager.BackToPanel(myManager.panelStack.Peek());
 //           }
 //       }
	//}
}
