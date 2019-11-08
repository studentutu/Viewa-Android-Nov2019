using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using OTPL.modal;
using UnityEngine.Networking;
using System.Text;
using SimpleJSON;
using System.IO;
using UnityEngine.UI;
using OTPL.UI;


public class QAPopupPanel : PanelBase {

    [SerializeField] 
	public GameObject mQAPopup;
	public GameObject mAskPopup;
    List<PopUpQuestions> PopupSurveyList = new List<PopUpQuestions>();
    List<PopUpItemDetail> popUpItemFullQuestionsDetailList = new List<PopUpItemDetail>();
	CanvasGroup canvasGroup;
    public Text askpopupText;

    int currentPopupCount;

	protected override void Awake ()
	{
		base.Awake ();
	}

	protected override void OnEnable ()
	{
		base.OnEnable ();
		
		if (AppManager.Instnace.isPopUpSurvay) {

            canvasGroup = gameObject.GetComponent<CanvasGroup>();
            canvasGroup.alpha = 1.0f;
            currentPopupCount = 0;

			AppManager.Instnace.isPopUpSurvay = false;

            if (AppManager.Instnace.popup_keyword == "" || AppManager.Instnace.popup_keyword == null || AppManager.Instnace.popup_keyword == "[]")
            {
                askpopupText.text = "Would you like to receive further information from this company?";
            } else {
                askpopupText.text = AppManager.Instnace.popup_keyword;
            }

			mAskPopup.SetActive (true);
			mQAPopup.SetActive (false);

			AppManager.Instnace.acpTrackingManager.m_ScanLine.ShowScanLine (false);
			AppManager.Instnace.acpTrackingManager.DisableScanning ();

            //AppManager.Instnace.popUpQuestionsResponse = "[{\"Id\":294343,\"AnswerId\":294343,\"MasterQuestionNodeText\":\"TestFirstSample\",\"QuestionText\":\"Did you read viewa Today?\",\"MasterAnswerNodeText\":[\"Option1\",\"Option2\",\"Option3\"],\"AnswerText\":[\"Yes\",\"No\",\"Other\"]},{\"Id\":294347,\"AnswerId\":294347,\"MasterQuestionNodeText\":\"FirstMultiple\",\"QuestionText\":\"Is Multiple Solutions?\",\"MasterAnswerNodeText\":[\"one\",\"Two\"],\"AnswerText\":[\"Yes\",\"MayBe\"]},{\"Id\":294350,\"AnswerId\":294350,\"MasterQuestionNodeText\":\"TestSecondSample\",\"QuestionText\":\"Did you sleep?\",\"MasterAnswerNodeText\":[],\"AnswerText\":[]}]";

            string response = AppManager.Instnace.popUpQuestionsResponse;

            if ( !string.IsNullOrEmpty(response))
            {
                Debug.Log("PopUpSurveyServices Response :" + response);
                canvasGroup.alpha = 1.0f;
                GetQuestionAnswerData(response);
                CallPopupSurvey(response);
            } 
		} else {
			return;
		}
	}

	protected override void OnDisable ()
	{
		base.OnDisable ();
        popUpItemFullQuestionsDetailList.Clear();
//		DeallocateObject ();
	}

	protected override void OnUIButtonClicked (Button a_button)
	{
		base.OnUIButtonClicked (a_button);

		switch (a_button.name) {
        case "Next_Button":
            if (currentPopupCount < PopupSurveyList.Count) {
                    currentPopupCount += 1;
                    SendPopUpDetailsToServer();
                } 
			break;
		case "Yes_Button":
			mQAPopup.SetActive (true);
			mAskPopup.SetActive (false);
			break;
		case "No_Button":
            DeallocateObject();
			AppManager.Instnace.acpTrackingManager.OnCloseAndNavigate ();
			CanvasManager.Instnace.HidePanelManager (ePanelManager.PopupSurvayManager);
			break;
        case "CrossButton":
            DeallocateObject();
            AppManager.Instnace.acpTrackingManager.OnCloseAndNavigate();
            CanvasManager.Instnace.HidePanelManager(ePanelManager.PopupSurvayManager);
            break;
		}
	}

	void CallPopupSurvey(string response) { //string url

		
        if(currentPopupCount < PopupSurveyList.Count()){
            
            QAObject qaObject = (QAObject)mQAPopup.GetComponent<QAObject>();
            qaObject.m_dropDownAnswer.ClearOptions();
            qaObject.FillData(PopupSurveyList[currentPopupCount]);
        }
	}

	void GetQuestionAnswerData(string response) {
		JSONNode jsonResponse = JSONNode.Parse (response);
		
		if (jsonResponse != null) {
            
			int count = jsonResponse.Childs.Count ();
			var res = JSON.Parse (response);
			//CategoriesDataService catDbService = new CategoriesDataService ();

			for (int i = 0; i < count; i++) {
                PopUpQuestions lstPopup = new PopUpQuestions ();
				lstPopup.Id = jsonResponse[i]["Id"].AsInt;
				lstPopup.AnswerId = jsonResponse [i] ["AnswerId"].AsInt;
				lstPopup.MasterQuestionNodeText = jsonResponse [i] ["MasterQuestionNodeText"].Value;
				lstPopup.QuestionText = jsonResponse [i] ["QuestionText"].Value;
                var AnswerNodeArray = jsonResponse[i]["MasterAnswerNodeText"];
                if(AnswerNodeArray != null) {
                    int AnswerNodeArrayCount = AnswerNodeArray.Childs.Count();
                    lstPopup.MasterAnswerNodeText = new string[AnswerNodeArrayCount];
                    for (int j = 0; j < AnswerNodeArray.Childs.Count(); j++)
                    {
                        lstPopup.MasterAnswerNodeText[j] = AnswerNodeArray[j].Value;
                    }
                }

                var AnswerTextArray = jsonResponse[i]["AnswerText"];
                if(AnswerTextArray != null){
                    int AnswerTextArrayCount = AnswerTextArray.Childs.Count();
                    lstPopup.AnswerText = new string[AnswerTextArrayCount];
                    for (int k = 0; k < AnswerNodeArray.Childs.Count(); k++)
                    {
                        lstPopup.AnswerText[k] = AnswerTextArray[k].Value;
                    }
                }
				PopupSurveyList.Add (lstPopup);
			}
		} 
	} 

	void SendPopUpDetailsToServer() {

          QAObject qaObject = (QAObject)mQAPopup.GetComponent<QAObject>();
          PopUpItemDetail popUpItemDetail = new PopUpItemDetail ();
          popUpItemDetail.EmailID = WebService.Instnace.appUser.Email;
          popUpItemDetail.RegionId = WebService.Instnace.appUser.RegionId;
          popUpItemDetail.Firstname = WebService.Instnace.appUser.FirstName;
          popUpItemDetail.Lastname = WebService.Instnace.appUser.LastName;
          popUpItemDetail.Gender = WebService.Instnace.appUser.Gender;
          popUpItemDetail.phone = WebService.Instnace.appUser.Mobile;


          if (WebService.Instnace.catDetailParameter.CategoryID == null || WebService.Instnace.catDetailParameter.CategoryID == "") {
              popUpItemDetail.CategoryID = -1;
          } else {
              popUpItemDetail.CategoryID = int.Parse (WebService.Instnace.catDetailParameter.CategoryID);
          }
          popUpItemDetail.TriggerId = WebService.Instnace.catDetailParameter.HubTrigger;
          popUpItemDetail.QuestionId = qaObject.questionAnswerObj.Id;
          popUpItemDetail.QuestionText = qaObject.questionAnswerObj.QuestionText;

        if(qaObject.userInputField.gameObject.activeSelf){
            if (AppManager.Instnace.isLive)
                popUpItemDetail.AnswerText = qaObject.userInputField.text;
            else
                popUpItemDetail.AnswerText = "Testing the response";
        } else {
            popUpItemDetail.AnswerText = qaObject.m_dropDownAnswer.options[qaObject.m_dropDownAnswer.value].text;
        }
          
          popUpItemDetail.trackingId = AppManager.Instnace.trackingId;
          popUpItemDetail.imageURL = WebService.Instnace.historyData.history_Image;

          
          popUpItemDetail.AnswerId = qaObject.questionAnswerObj.AnswerId;
          popUpItemDetail.IsViewed = true;
        if (currentPopupCount == PopupSurveyList.Count)
            popUpItemDetail.IsSubmit = true;
        else 
            popUpItemDetail.IsSubmit = false;
        
          Debug.Log ("popUpItemDetail.EmailID :" + popUpItemDetail.EmailID);
          Debug.Log ("popUpItemDetail.RegionId :" + popUpItemDetail.RegionId);
          Debug.Log ("popUpItemDetail.CategoryID :" + popUpItemDetail.CategoryID);
          Debug.Log ("popUpItemDetail.TriggerId :" + popUpItemDetail.TriggerId);
          Debug.Log ("popUpItemDetail.QuestionId :" + popUpItemDetail.QuestionId);
          Debug.Log("popUpItemDetail.QuestionText:" + popUpItemDetail.QuestionText);
          Debug.Log ("popUpItemDetail.AnswerText :" + popUpItemDetail.AnswerText);
          
          PopUpItemRoot popUpItemRoot = new PopUpItemRoot();

          List<PopUpItemDetail> popUpItemDetailList = new List<PopUpItemDetail>();
          
          popUpItemDetailList.Add(popUpItemDetail);
          popUpItemFullQuestionsDetailList.Add(popUpItemDetail);
         

          if (currentPopupCount == PopupSurveyList.Count) {
            popUpItemRoot.PopupItemDetails = popUpItemFullQuestionsDetailList.ToArray();
          }
          else {
            popUpItemRoot.PopupItemDetails = popUpItemDetailList.ToArray();
          }

          string jsonString = popUpItemRoot.ReturnJsonString ();
          Debug.Log ("QA jsonString :" + jsonString);
          byte[] bodyRaw = Encoding.UTF8.GetBytes (jsonString);
          WebService.Instnace.Post (AppManager.Instnace.baseURL + "/cloud/PopUpSurveyDataSavingServices.aspx", bodyRaw, "", null);
          PopUpSurveySent();
	}



    void PopUpSurveySent() {

        if (currentPopupCount == PopupSurveyList.Count)
        {
            AppManager.Instnace.CacheTrigger(WebService.Instnace.catDetailParameter.HubTrigger.ToString());

#if !UNITY_EDITOR
            AppManager.Instnace.messageBoxManager.ShowGenericPopup ("Feedback Sent", "Thank you for your feedback.\n\n\nThanks again.\nViewa Team", "OK", NavigatetoScreen);
#else
            NavigatetoScreen();
#endif
        } else {

            //We have to clear the drop down list here
            QAObject qaObject = (QAObject)mQAPopup.GetComponent<QAObject>();
            qaObject.m_questionText.text = "";
            qaObject.m_dropDownAnswer.ClearOptions();
            CallPopupSurvey(AppManager.Instnace.popUpQuestionsResponse);
            LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)mQAPopup.transform);
        }
    }

	void NavigatetoScreen() {
        mQAPopup.SetActive(true);
        CanvasManager.Instnace.HidePanelManager(ePanelManager.PopupSurvayManager);
		AppManager.Instnace.acpTrackingManager.OnCloseAndNavigate ();
		DeallocateObject ();
	}

	void DeallocateObject() {

        if (PopupSurveyList != null) {
            if (PopupSurveyList.Count > 0) {
                PopupSurveyList.Clear ();
          }
        }
		
		AppManager.Instnace.isPopUpSurvay = false;
	}

	void ErrorCall() {
		DeallocateObject ();
	}
}



public class PopUpQuestions
{
    public int Id { get; set; }
    public int AnswerId { get; set; }
    public string MasterQuestionNodeText { get; set; }
    public string QuestionText { get; set; }
    public string [] MasterAnswerNodeText { get; set; }
    public string [] AnswerText { get; set; }
}

/*Sending information to the server*/

[System.Serializable]
public class PopUpItemRoot
{
    public PopUpItemDetail[] PopupItemDetails;

    public string ReturnJsonString()
    {
        return JsonUtility.ToJson(this);
    }
}

[System.Serializable]
public class PopUpItemDetail
{
    //firstname, lastname, gender, phone
    public string Firstname;
    public string Lastname;
    public string Gender;
    public string phone;
    public string EmailID;
    public long RegionId;
    public long CategoryID;
    public long TriggerId;
    public long QuestionId;
    public string QuestionText;
    public long AnswerId;
    public string AnswerText;
    public bool IsViewed;
    public string trackingId;
    public string imageURL;
    public bool IsSubmit;
}


