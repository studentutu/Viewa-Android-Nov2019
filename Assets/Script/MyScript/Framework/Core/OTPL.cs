//----------------------------------------------
//       OTPL - Jeetesh
//       created: 5 Dec 2017
//       Copyright © 2017 OTPL
//       Description: All config related code like enums and constants for the project.
//----------------------------------------------


using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace OTPL
{
    namespace INGAME {
       
        public enum VideoUpdateStatus
        {
            Finished,
            Loading,
            Playing
        }
    }
    namespace UI
    {
        
		public enum eScreenAnimationTransition {
			BackScreenTransition,
			SelectScreenTransition
		}

		public enum eTriggerHistoryMode {
			TriggerHistoryModeNone,
			TriggerHistoryModeLoved,  //Favourite
			TriggerHistoryModeCategory,  
			TriggerHistoryModeRecent,  //History
			TriggerHistoryModeChannel,
		}


        /// <summary>
        /// Panel Managers in app.
        /// </summary>

        public enum ePanelManager{
            MainMenuPanelManager,
            BottomBarManager,
            NavigationBarManager,
            PopupSurvayManager,
        }

        /// <summary>
        /// Panels in the app. 
        /// </summary>

        public enum ePanels
        {
            //MainMenuPanels
			Scan_Panel,
            Hub_Panel,
            Help_Panel,
            About_Panel,
            Troubleshooting_Panel,
            Profile_Panel,
            Region_Panel,
            HubDetail_Panel,
            Recent_Panel,
            Loved_Panel,
            StillDoesntWork_Panel,
            MusicResponse_Panel,
            Enquiries_Panel,
            Feedback_Panel,
            ReportAProblem_Panel,
            ForgetPassword_Panel,
            ImageGallary_Panel,
			SideMenuPanel,
			PrivacyPolicy_Panel,
			TermsOfUse_Panel,
			Blank_Panel,
			QAPopup_panel,
            GetStarted_Panel,
            SignUp_Panel,
            SignUpAccount,
            SignUpPassword,
            SignIn_panel,
            SignInPassword,
            PasswordPopup_panel,
            SignupPopupPanel,
            BottomBarPanel,
            ResetPasswordPanel,
            None_Panel
        }

    }

	namespace modal {

		//*******************************************
		//User 
		//*******************************************
		[System.Serializable]
		public enum eSocialSignUp
		{
			Social,
			Basic
		}

		[System.Serializable]
		public class D
		{
			public string __type;
			public string StatusMessage;
			public User User;
		}
		[System.Serializable]
		public class User 
		{
			public string Id;
			public string Address1;
			public string Address2;
			public string Country;
			public string DateOfBirth; 
			public string Email;
			public string FirstName;
			public string LastName;
			public string Gender;
			public string Mobile;
			public string PostCode;
			public string RelationshipStatus;
			public string State;
			public string Suburb;
			public string FacebookId;
			public string GoogleId;
			public string LinkedinId;
			public string Password;
			public string PasswordSalt;
			public long RegionId;
			public string DateCreated;
			public string interest;
		}
		[System.Serializable]
		public class RootObject
		{
			public D d;
		}

		//*******************************************
		//region 
		//*******************************************
		[System.Serializable]
		public class Regions {

			public long id;
			public string name;
			public string client_access_key;
			public string client_secret_key;
			public string image;
			public int autoDetect;
			public string countryCode;

			public string SaveToJsonString()
			{
				return JsonUtility.ToJson(this);
			}
		}

		[System.Serializable]
		public class GeoLocation {
			public string city;
			public string country;
			public string countryCode;
			public long lat;
			public long lon;
			public string regionName;
			public string timezone;
            public string zip;
            public string deviceId;
            public string deviceName;
		}

		//	[System.Serializable]
		//	public class RegionService {
		//
		//		public Regions [] RegionList;
		//	}

		//*******************************************
		//Categories
		//*******************************************
		[System.Serializable]
		public class Categories {

			public long id;
			public string name;
			public string image;

			public string SaveToJsonString()
			{
				return JsonUtility.ToJson(this);
			}
		}

		//*******************************************
		//HubDetail
		//*******************************************

		[System.Serializable]
		public class CategoryDetailWebService {

			public long id;
			public string cloud_id;
			public string history_title;
			public string history_description;
			public string history_image;
			public string history_icon;
			public string keywords;
			public string name;
			public string image;
			public long channel_id;
			public string channel_name;
			public bool is_last;
			public string CategoryId;

			public string SaveToJsonString()
			{
				return JsonUtility.ToJson(this);
			}
		}

		[System.Serializable]
		public class HistoryData {

			public string history_Title;
			public string history_Description;
			public string history_Image;
			public string history_icon;
            public string history_keyword;
		}
	}

	namespace Helper {

		/// <summary>
		/// Tests an E-Mail address.
		/// </summary>
		public static class TestEmail
		{
			/// <summary>
			/// Regular expression, which is used to validate an E-Mail address.
			/// </summary>
			public const string MatchEmailPattern =
				@"^(([\w-]+\.)+[\w-]+|([a-zA-Z]{1}|[\w-]{2,}))@"
            + @"((([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\."
              + @"([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])){1}|"
            + @"([a-zA-Z]+[\w-]+\.)+[a-zA-Z]{2,4})$";


			/// <summary>
			/// Checks whether the given Email-Parameter is a valid E-Mail address.
			/// </summary>
			/// <param name="email">Parameter-string that contains an E-Mail address.</param>
			/// <returns>True, wenn Parameter-string is not null and contains a valid E-Mail address;
			/// otherwise false.</returns>
			public static bool IsEmail(string email)
			{
				if (email != null) return Regex.IsMatch(email, MatchEmailPattern);
				else return false;
			}
		}

		public static class JsonHelper
		{
			public static T[] FromJson<T>(string json)
			{
				Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(json);
				return wrapper.Items;
			}

			public static string ToJson<T>(T[] array)
			{
				Wrapper<T> wrapper = new Wrapper<T>();
				wrapper.Items = array;
				return JsonUtility.ToJson(wrapper);
			}

			public static string ToJson<T>(T[] array, bool prettyPrint)
			{
				Wrapper<T> wrapper = new Wrapper<T>();
				wrapper.Items = array;
				return JsonUtility.ToJson(wrapper, prettyPrint);
			}

			[System.Serializable]
			private class Wrapper<T>
			{
				public T[] Items;
			}
		}

		public static class LocalizeStringKey {

			public const string AppStoreLink = "AppStoreLink";
			public const string GooglePlayLink = "GooglePlayLink";
			public const string OKButton = "OKButton";
			public const string CancelButton = "CancelButton";
			public const string ApplyButton = "ApplyButton";
			public const string RetryButton = "RetryButton";
			public const string DeleteButton = "DeleteButton";
			public const string LoadingStatus = "LoadingStatus";
			public const string GalleryLoadErrorTitle = "GalleryLoadErrorTitle";
			public const string GalleryLoadErrorMessage = "GalleryLoadErrorMessage";
			public const string MusicPlayerLoadErrorTitle = "MusicPlayerLoadErrorTitle";
			public const string MusicPlayerLoadErrorMessage = "MusicPlayerLoadErrorMessage";
			public const string AppVersionErrorTitle = "AppVersionErrorTitle";
			public const string AppVersionErrorMessage = "AppVersionErrorMessage";
			public const string AppVersionErrorCancelButton = "AppVersionErrorCancelButton";
			public const string AppVersionErrorAppStoreButton = "AppVersionErrorAppStoreButton";
			public const string AppVersionErrorGooglePlayButton = "AppVersionErrorGooglePlayButton";
			public const string TrackableDataDownloadErrorTitle = "TrackableDataDownloadErrorTitle";
			public const string TrackableDataDownloadErrorMessage = "TrackableDataDownloadErrorMessage";
			public const string ScanSuccessOverlayTitle = "ScanSuccessOverlayTitle";
			public const string Error = "Error";
			public const string Open = "Open";
			public const string RegionLoadErrorTitle = "RegionLoadErrorTitle";
			public const string RegionLoadErrorMessage = "RegionLoadErrorMessage";
			public const string SignUpValidationErrorTitle = "SignUpValidationErrorTitle";
			public const string SignUpValidationErrorEmail = "SignUpValidationErrorEmail";
			public const string SignUpValidationErrorPassword = "SignUpValidationErrorPassword";
			public const string IntroStartButton = "IntroStartButton";
			public const string IntroSkipButton = "IntroSkipButton";
			public const string LogoutButtonTitle = "LogoutButtonTitle";
			public const string ProfileEditButton = "ProfileEditButton";
			public const string LoginFormTitle = "LoginFormTitle";
			public const string EmailFieldLabel = "EmailFieldLabel";
			public const string PasswordFieldLabel = "PasswordFieldLabel";
			public const string PasswordFieldPlaceholder = "PasswordFieldPlaceholder";
			public const string LoginForgotTitle = "LoginForgotTitle";
			public const string LoginForgotButton = "LoginForgotButton";
			public const string ForgotSubmitTitle = "ForgotSubmitTitle";
			public const string ForgotSubmitText = "ForgotSubmitText";
			public const string LoginButton = "LoginButton";
			public const string ForgotFormTitle = "ForgotFormTitle";
			public const string ForgotButton = "ForgotButton";
			public const string SignupFormTitle = "SignupFormTitle";
			public const string SignupButton = "SignupButton";
			public const string FirstNameFieldLabel = "FirstNameFieldLabel";
			public const string LastNameFieldLabel = "LastNameFieldLabel";
			public const string DateOfBirthFieldLabel = "DateOfBirthFieldLabel";
			public const string GenderFieldLabel = "GenderFieldLabel";
			public const string PasswordConfirmFieldLabel = "PasswordConfirmFieldLabel";
			public const string PasswordConfirmFieldPlaceholder = "PasswordConfirmFieldPlaceholder";
			public const string GenderMale = "GenderMale";
			public const string GenderFemale = "GenderFemale";
			public const string GenderNotSpecified = "GenderNotSpecified";
			public const string FieldsRequiredTitle = "FieldsRequiredTitle";
			public const string FieldsOptionalTitle = "FieldsOptionalTitle";
			public const string FieldsPasswordTitle = "FieldsPasswordTitle";
			public const string SignInTitle = "SignInTitle";
			public const string PasswordResetErrorTitle = "PasswordResetErrorTitle";
			public const string PasswordResetUserNotFound = "PasswordResetUserNotFound";
			public const string LoginErrorTitle = "LoginErrorTitle";
			public const string LoginUserNotFound = "LoginUserNotFound";
			public const string LoginAuthenticationError = "LoginAuthenticationError";
			public const string SignUpErrorTitle = "SignUpErrorTitle";
			public const string SignUpAuthenticationError = "SignUpAuthenticationError";
			public const string UpdateUserErrorTitle = "UpdateUserErrorTitle";
			public const string UpdateUserAuthenticationError = "UpdateUserAuthenticationError";
			public const string GenericAPIRequestError = "GenericAPIRequestError";
			public const string VuforiaError1 = "VuforiaError1";
			public const string VuforiaError2 = "VuforiaError2";
			public const string VuforiaError3 = "VuforiaError3";
			public const string VuforiaError4 = "VuforiaError4";
			public const string VuforiaError5 = "VuforiaError5";
			public const string VuforiaError6 = "VuforiaError6";
			public const string VuforiaError7 = "VuforiaError7";
			public const string WebAutofillAlertNeverButton = "WebAutofillAlertNeverButton";
			public const string WebAutofillAlertLoginButton = "WebAutofillAlertLoginButton";
			public const string WebAutofillAlertNotNowButton = "WebAutofillAlertNotNowButton";
			public const string WebAutofillAlertTitle = "WebAutofillAlertTitle";
			public const string WebAutofillAlertMessage = "WebAutofillAlertMessage";
			public const string UnityLoadDataFailed = "UnityLoadDataFailed";
			public const string TwitterSuccess = "TwitterSuccess";
			public const string ApplicationVersion = "ApplicationVersion";
			public const string ShareText = "ShareText";
			public const string SignInWeiboButton = "SignInWeiboButton";
			public const string BackButton = "BackButton";
			public const string ScanHistory = "ScanHistory";
			public const string Done = "Done";
			public const string Day = "Day";
			public const string Edit = "Edit";
			public const string Favourites = "Favourites";
			public const string ScanHistoryEmptyMessage = "ScanHistoryEmptyMessage";
			public const string ScanHistoryNoResultsMessage = "ScanHistoryNoResultsMessage";
			public const string ScanHistoryNoFavourites = "ScanHistoryNoFavourites";
			public const string WeChat = "WeChat";
			public const string Chat = "Chat";
			public const string Moments = "Moments";
			public const string ImageCropSquare = "ImageCropSquare";
			public const string ImageCropPinch = "ImageCropPinch";
			public const string Line = "Line";
			public const string OpenIn = "OpenIn";
			public const string HowToUseTitle = "HowToUseTitle";
			public const string TermsOfUseTitle = "TermsOfUseTitle";
			public const string AboutViewTitle = "AboutViewTitle";
			public const string ContactUsTitle = "ContactUsTitle";
			public const string MainMenuRowProfile = "MainMenuRowProfile";
			public const string MainMenuRowHelp = "MainMenuRowHelp";
			public const string MainMenuRowExplore = "MainMenuRowExplore";
			public const string MainMenuRowAbout = "MainMenuRowAbout";
			public const string MainMenuRowRegion = "MainMenuRowRegion";
			public const string MainMenuRowScan = "MainMenuRowScan";
			public const string ScanOverlayTextDownloading = "ScanOverlayTextDownloading";
			public const string ScanOverlayTextHoldStill = "ScanOverlayTextHoldStill";
			public const string RegionAutodetectedLabel = "RegionAutodetectedLabel";
			public const string SendRetrievalEmailButton = "SendRetrievalEmailButton";
			public const string ForgotPasswordTitle = "ForgotPasswordTitle";
			public const string RegionSelectorTitle = "RegionSelectorTitle";
			public const string MobileFieldLabel = "MobileFieldLabel";
			public const string Address1FieldLabel = "Address1FieldLabel";
			public const string Address2FieldLabel = "Address2FieldLabel";
			public const string SuburbFieldLabel = "SuburbFieldLabel";
			public const string StateFieldLabel = "StateFieldLabel";
			public const string PostcodeFieldLabel = "PostcodeFieldLabel";
			public const string CountryFieldLabel = "CountryFieldLabel";
			public const string MyDetailsTitle = "MyDetailsTitle";
			public const string SignInFacebookButton = "SignInFacebookButton";
			public const string SignUpViewaButton = "SignUpViewaButton";
			public const string SignUpLoginLabel = "SignUpLoginLabel";
			public const string SignUpLoginButton = "SignUpLoginButton";
			public const string SignUpText = "SignUpText";
			public const string SelectRegion = "SelectRegion";
			public const string IntroStep1Text = "IntroStep1Text";
			public const string IntroStep2Text = "IntroStep2Text";
			public const string IntroStep3Text = "IntroStep3Text";
			public const string IntroStep4Text = "IntroStep4Text";
			public const string IntroStep5Text = "IntroStep5Text";
			public const string ABOUT = "ABOUT";
			public const string AboutViewaTitle = "AboutViewaTitle";
			public const string AboutViewaBlurb = "AboutViewaBlurb";
			public const string AboutEnquiresTitle = "AboutEnquiresTitle";
			public const string AboutTermsTitle = "AboutTermsTitle";
			public const string AboutTermsOfUseTitle = "AboutTermsOfUseTitle";
			public const string AboutPrivacyTitle = "AboutPrivacyTitle";
			public const string AboutUseViewaBrandTitle = "AboutUseViewaBrandTitle";
			public const string HELP = "HELP";
			public const string HelpHowToUseTitle = "HelpHowToUseTitle";
			public const string HelpTroubleShootingTitle = "HelpTroubleShootingTitle";
			public const string HelpWatchVideosTitle = "HelpWatchVideosTitle";
			public const string HelpStillNotWorkingTitle = "HelpStillNotWorkingTitle";
			public const string HelpMakeBetterTitle = "HelpMakeBetterTitle";
			public const string HelpRateTitle = "HelpRateTitle";
			public const string HelpFeedbackTitle = "HelpFeedbackTitle";
			public const string HelpReportProblemTitle = " HelpReportProblemTitle";
			public const string ReportProblemTitle = "ReportProblemTitle";
			public const string ReportProblemText = "ReportProblemText";
			public const string ReportProblemNameLabel = "ReportProblemNameLabel";
			public const string ReportProblemContactLabel = "ReportProblemContactLabel";
			public const string ReportProblemDescLabel = "ReportProblemDescLabel";
			public const string SubmitText = "SubmitText";
			public const string FeedbackTitle = "FeedbackTitle";
			public const string FeedbackText = "FeedbackText";
			public const string FeedbackNameLabel = "FeedbackNameLabel";
			public const string FeedbackContactLabel = "FeedbackContactLabel";
			public const string FeedbackCommentLabel = "FeedbackCommentLabel";
			public const string EnquireTitle = "EnquireTitle";
			public const string EnquireNameLabel = "EnquireNameLabel";
			public const string EnquireCompanyLabel = "EnquireCompanyLabel";
			public const string EnquireContactLabel = "EnquireContactLabel";
			public const string EnquireDescLabel = "EnquireDescLabel";
			public const string StillNotWorkingTitle = "StillNotWorkingTitle";
			public const string StillNotWorkingText = "StillNotWorkingText";
			public const string StillNotWorkingEmailUs = "StillNotWorkingEmailUs";
			public const string TroubleShootingSectionTitle = "TroubleShootingSectionTitle";
			public const string TroubleShooting1Title = "TroubleShooting1Title";
			public const string TroubleShooting1Text = "TroubleShooting1Text";
			public const string TroubleShooting2Title = "TroubleShooting2Title";
			public const string TroubleShooting2Text = "TroubleShooting2Text";
			public const string TroubleShooting3Title = "TroubleShooting3Title";
			public const string TroubleShooting3Text = "TroubleShooting3Text";
			public const string TroubleShooting4Title = "TroubleShooting4Title";
			public const string TroubleShooting4Text = "TroubleShooting4Text";
			public const string TroubleShooting5Title = "TroubleShooting5Title";
			public const string TroubleShooting5Text = "TroubleShooting5Text";
			public const string TroubleShooting6Title = "TroubleShooting6Title";
			public const string TroubleShooting6Text = "TroubleShooting6Text";
			public const string TroubleShooting7Title = "TroubleShooting7Title";
			public const string TroubleShooting7Text = "TroubleShooting7Text";
			public const string TroubleShooting8Title = "TroubleShooting8Title";
			public const string TroubleShooting8Text = "TroubleShooting8Text";
			public const string TroubleShooting9Title = "TroubleShooting9Title";
			public const string TroubleShooting9Text = "TroubleShooting9Text";
			public const string TroubleShooting10Title = "TroubleShooting10Title";
			public const string TroubleShooting10Text = "TroubleShooting10Text";
			public const string EXPLORE = "EXPLORE";
			public const string SCAN = "SCAN";
			public const string GalleryShareTitle = "GalleryShareTitle";
			public const string SearchHintText = "SearchHintText";
			public const string AddToFavourites = "AddToFavourites";
			public const string RemoveFromFavourites = "RemoveFromFavourites";
			public const string RegionDetectCorrectTitle = "RegionDetectCorrectTitle";
			public const string RegionDetectChangeTitle = "RegionDetectChangeTitle";
			public const string AndroidAppName = "AndroidAppName";
			public const string SavedToPhotoGalleryText = "SavedToPhotoGalleryText";
			public const string SavedToPhotoGalleryFailedText = "SavedToPhotoGalleryFailedText";
			public const string ShareIntentChooserTitle = "ShareIntentChooserTitle";
			public const string PhotoBoothStickerTitle = "PhotoBoothStickerTitle";
			public const string PhotoBoothStickerLoadErrorMessage = "PhotoBoothStickerLoadErrorMessage";
			public const string PhotoBoothStickerLoadErrorTitle = "PhotoBoothStickerLoadErrorTitle";
			public const string StatusNoCameraAccess = "StatusNoCameraAccess";
			public const string CameraPermissionTitle = "CameraPermissionTitle";
			public const string CameraPermissionMessage = "CameraPermissionMessage";
			public const string Settings = "Settings";
		}
	}
		
}



