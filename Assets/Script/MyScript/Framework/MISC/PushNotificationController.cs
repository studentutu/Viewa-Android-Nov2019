using System.Collections;
using System.Collections.Generic;
using VoxelBusters.NativePlugins.Demo;
using VoxelBusters.NativePlugins;
using VoxelBusters.Utility;
using UnityEngine;
using OTPL.UI;


public class PushNotificationController : MonoBehaviour {

	#region Fields
	public string jsonCombine;
	public bool isPushNotificationShown;

	#pragma warning disable
	[SerializeField, EnumMaskField(typeof(NotificationType))]
	private NotificationType	m_notificationType;
	#pragma warning restore

	#endregion

	#region Unity Methods

	private void Start()
	{
		NPBinding.NotificationService.RegisterNotificationTypes(m_notificationType);
		NPBinding.NotificationService.RegisterForRemoteNotifications();
	}

	private void OnEnable ()
	{
		// Register RemoteNotificated related callbacks
		NotificationService.DidLaunchWithRemoteNotificationEvent		+= DidLaunchWithRemoteNotificationEvent;
		NotificationService.DidFinishRegisterForRemoteNotificationEvent	+= DidFinishRegisterForRemoteNotificationEvent;
		NotificationService.DidReceiveRemoteNotificationEvent			+= DidReceiveRemoteNotificationEvent;


	}

	private void OnDisable ()
	{
		// Un-Register from callbacks
		NotificationService.DidLaunchWithRemoteNotificationEvent		-= DidLaunchWithRemoteNotificationEvent;
		NotificationService.DidReceiveRemoteNotificationEvent			-= DidReceiveRemoteNotificationEvent;
		NotificationService.DidFinishRegisterForRemoteNotificationEvent	-= DidFinishRegisterForRemoteNotificationEvent;

	}

	#endregion


	#region API Callback Methods

	private void DidLaunchWithRemoteNotificationEvent (CrossPlatformNotification _notification)
	{
		Debug.Log("Application did launch with remote notification.");
		AppendNotificationResult(_notification);
	}

	private void DidReceiveRemoteNotificationEvent (CrossPlatformNotification _notification)
	{
		Debug.Log("Application received remote notification.");
		AppendNotificationResult(_notification);
	}

	private void DidFinishRegisterForRemoteNotificationEvent (string _deviceToken, string _error)
	{
		Debug.Log(string.Format("Request to register for remote notification finished. Error = {0}.", _error.GetPrintableString()));
		Debug.Log("DeviceToken = " + _deviceToken);
	}
	#endregion

	void AppendNotificationResult (CrossPlatformNotification _notification)
	{
		string _alert 					= _notification.AlertBody;

		#pragma warning disable
		// Exists only for local notifications which will be useful if we need to cancel a local notification
		string _notificationIdentifier 	= _notification.GetNotificationID();
		#pragma warning restore

		//Get UserInfo details
		IDictionary _userInfo 			= _notification.UserInfo;

		//Can get specific details of a notification based on platform
		/*
					//For Android
		_notification.AndroidProperties.ContentTitle
		_notification.AndroidProperties.TickerText

		//For iOS
		_notification.iOSProperties.AlertAction;
		_notification.iOSProperties.BadgeCount;
		*/

		// Append to result list
		Debug.Log("Alert = " + _alert);

		// Append user info
		string _userInfoDetails = null;

		if (_userInfo != null)
		{
			// Initialize and iterate through the list
			_userInfoDetails	= string.Empty;
			isPushNotificationShown = true;

			foreach (string _key in _userInfo.Keys)
			{
				_userInfoDetails	+= _key + " : " + _userInfo[_key] + "\n";

				if (_key == "jsonString") {

					jsonCombine = _userInfo [_key].ToString();

					if (AppManager.Instnace.isLoggedInAndInside) {

						if (AppManager.Instnace.isVuforiaOn) {
							AppManager.Instnace.messageBoxManager.ShowMessageWithTwoButtons ("Notification", "Would you like to see our exclusive product.", "Ok", "Cancel", ShowProduct);
						} else {
							AppManager.Instnace.messageBoxManager.ShowMessageWithTwoButtons ("Notification", "Would you like to see our exclusive product.", "Ok", "Cancel", ShowProduct);
						}
					}
				}
			}
		}
		else
		{
			_userInfoDetails	= "NULL";
		}

		Debug.Log("UserInfo = " + _userInfoDetails);	
//		AppManager.Instnace.acpTrackingManager.CreateACPTrackable (combine);
	}

	void ShowProduct() {

		AppManager.Instnace.acpTrackingManager.DestroyTrackableData ();
		CanvasManager.Instnace.ReturnPanelManager (ePanelManager.MainMenuPanelManager).NavigateToPanel (ePanels.Scan_Panel);
		AppManager.Instnace.acpTrackingManager.CreateACPTrackable (jsonCombine);
	}
}

