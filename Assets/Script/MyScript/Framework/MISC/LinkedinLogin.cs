using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using OTPL.UI;
using OTPL.modal;
using SimpleJSON;
using UnityEngine.Networking;

//https://api.linkedin.com/v1/people-search:(people:((id,title,summary,start-date,end-date,is-current,company:(id,name,type,size,industry,ticker)),educations:(id,school-name,field-of-study,start-date,end-date,degree,activities,notes)),num-results)?first-name=parameter&last-name=parameter
//https://api.linkedin.com/v1/people/~:(id,first-name,last-name,headline,picture-url,industry,summary,specialties,positions:(id,title,summary,start-date,end-date,is-current,company:(id,name,type,size,industry,ticker)),educations:(id,school-name,field-of-study,start-date,end-date,degree,activities,notes),associations,interests,num-recommenders,date-of-birth,publications:(id,title,publisher:(name),authors:(id,name),date,url,summary),patents:(id,title,summary,number,status:(id,name),office:(name),inventors:(id,name),date,url),languages:(id,language:(name),proficiency:(level,name)),skills:(id,skill:(name)),certifications:(id,name,authority:(name),number,start-date,end-date),courses:(id,name,number),recommendations-received:(id,recommendation-type,recommendation-text,recommender),honors-awards,three-current-positions,three-past-positions,volunteer)?oauth2_access_token=PUT_YOUR_TOKEN_HERE

public class LinkedinLogin : MonoBehaviour
{
	
	public class UserDataObject
	{
		public string emailAddress;
		public string firstName;
		public string id;
		public string lastName;
		public int numConnections;
		public string pictureUrl;
	}

	public void Login(){

		Debug.Log("Linkedin Login()");
		string uniqueStr = AppManager.Instnace.GetUniqueIdentifier ();
		//string url = "https://www.linkedin.com/oauth/v2/authorization?response_type=code&client_id=81ncvifa9o7was&redirect_uri=http://54.79.87.133/Authenticated.html&state="+uniqueStr+"&scope=r_basicprofile";
		//Application.OpenURL (url);
//		WebService.Instnace.Get (url, Authentication);
	}

	void Authentication(string response){

		Debug.Log ("User Response: "+ response);
		//LoginProcess ();
	}

	void LoginProcess()
	{
		Debug.Log("aLinkedin - Operation Response");
		string url = "https://api.linkedin.com/v1/people/~:(id,email-address,first-name,last-name,num-connections,picture-url)?oauth2_access_token=" +AppManager.Instnace.accessToken+"&format=json";
		WebService.Instnace.Get (url, LIUserInfoCallback);
	}


	void LIUserInfoCallback(string response){

		Debug.Log ("Linked User Response: "+ response); 
		var res = JSON.Parse(response);
		if (res != null) {

			UserDataObject userLinkedinData = JsonUtility.FromJson<UserDataObject> (response); 
			//***********************************************************************************************
			// Sending the User parameter to Server before Register to check if user exist
			//***********************************************************************************************
			IsUserAlreadyRegistered(userLinkedinData);
		}
	}

	void IsUserAlreadyRegistered(UserDataObject userLiData) {

		WebService.Instnace.appUser.Address1 = "";
		WebService.Instnace.appUser.Address2 = "";
		WebService.Instnace.appUser.Country = "";
		WebService.Instnace.appUser.DateOfBirth = "";
		WebService.Instnace.appUser.Email = userLiData.emailAddress;
		WebService.Instnace.appUser.Gender = "";
		WebService.Instnace.appUser.FirstName = userLiData.firstName;
		WebService.Instnace.appUser.LastName = userLiData.lastName;
		WebService.Instnace.appUser.LinkedInId = userLiData.id;
		WebService.Instnace.appUser.Mobile = "";
		WebService.Instnace.appUser.Password = "";
		WebService.Instnace.appUser.PostCode = "";
		WebService.Instnace.appUser.RegionId = AppManager.Instnace.regionId;
		WebService.Instnace.appUser.RelationshipStatus = "";
		WebService.Instnace.appUser.State = "";
		WebService.Instnace.appUser.Suburb = "";
		WebService.Instnace.appUser.LoginInfo = "Linkedin";
		WebService.Instnace.appUser.isLoggedIn = 1;
		AppManager.Instnace.isSocialSignInScreen = true;


		string jsonHeader = "Linkedin " + Convert.ToBase64String (Encoding.UTF8.GetBytes (userLiData.emailAddress));
		WebService.Instnace.headerString = jsonHeader;
		AppManager.Instnace.messageBoxManager.ShowPreloaderDefault ();
		string jsonParameter = JsonUtility.ToJson (WebService.Instnace.appUser);
		WebService.Instnace.headerString = jsonHeader;
		byte[] bodyRaw = Encoding.UTF8.GetBytes (jsonParameter);
		//****************************************************************************
		//						VerifyUsers
		//****************************************************************************
		WebService.Instnace.Post (AppManager.Instnace.baseURL + "/cloud/ViewaUserActions.aspx/VerifyUsers", bodyRaw, jsonHeader, UserCheckCallback);
	}

	void UserCheckCallback(UnityWebRequest response ){
		
		var res = JSON.Parse (response.downloadHandler.text);
		//user already exist code = 500
		if (response.responseCode == 200) {
			
			if (res != null) {
				RootObject responseModel = JsonUtility.FromJson<RootObject> (response.downloadHandler.text); 
				SaveUser (responseModel);
			} else {
				//Jeetesh - Hack - User already exist but we didn't get user data from server.
				Debug.Log("User present but details didn't come from server");
				AppManager.Instnace.messageBoxManager.HidePreloader ();
			}
			//User dosen't exist
		} else if(response.responseCode == 402) {
			
			UserNotRegistered ();
		}
	}
	void SaveUser(RootObject responseModel) {
		
			D _d = responseModel.d;
			OTPL.modal.User _user = _d.User;
	
			AppManager.Instnace.userId = _user.Id;
			AppManager.Instnace.regionId = _user.RegionId;
			AppManager.Instnace.userEmail = _user.Email;

			UserDataService userDbService = new UserDataService ();

//				AppManager.Instnace.SavePicture (null);

			//if the user id exist then update user otherwise 
			if (userDbService.UserExist (_user.Email) > 0) {
				userDbService.CreateOrReplaceUser (_user.Id, _user.Address1, _user.Address2, _user.Country, _user.DateOfBirth, _user.Email, _user.FirstName, _user.LastName, _user.Gender, _user.Mobile, _user.PostCode, 
				_user.RelationshipStatus, _user.State, _user.Suburb, _user.FacebookId, _user.GoogleId, _user.LinkedinId, _user.Password, _user.PasswordSalt, _user.RegionId, WebService.Instnace.appUser.LoginInfo, WebService.Instnace.appUser.isLoggedIn, _user.DateCreated, _user.interest);
			} else {
				userDbService.CreateOrReplaceUser (_user.Id, _user.Address1, _user.Address2, _user.Country, _user.DateOfBirth, _user.Email, _user.FirstName, _user.LastName, _user.Gender, _user.Mobile, _user.PostCode, 
				_user.RelationshipStatus, _user.State, _user.Suburb, _user.FacebookId, _user.GoogleId, _user.LinkedinId, _user.Password, _user.PasswordSalt, _user.RegionId, WebService.Instnace.appUser.LoginInfo, WebService.Instnace.appUser.isLoggedIn, _user.DateCreated, _user.interest);
			}

		AppManager.Instnace.GoToScanScreen ();
	}
	void UserNotRegistered() {
		CanvasManager.Instnace.ReturnPanelManager(ePanelManager.MainMenuPanelManager).NavigateToPanel(ePanels.SignUp_Panel);
	}
		
}


