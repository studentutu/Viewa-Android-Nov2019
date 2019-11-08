using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using VoxelBusters.NativePlugins;
using System.IO;
using OTPL.UI;
using VoxelBusters.Utility;

public class SocialSharingScript : MediaLibrary {  //Sharing

	public Button[] buttons;

	#region Properties

	private		float	scaleFactor;

	#endregion

	[SerializeField, Header("Message Sharing Properties")]
	protected 	string			m_smsBody			= "SMS body holds text message that needs to be sent to recipients";
	[SerializeField]
	protected 	string[] 		m_smsRecipients;

	[SerializeField, Header("Mail Sharing Properties")]
	protected 	string			m_mailSubject		= "Demo Mail";
	[SerializeField]
	protected 	string			m_plainMailBody		= "This is plain text mail.";
	[SerializeField]
	protected 	string			m_htmlMailBody		= "<html><body><h1>Hello</h1></body></html>";
	[SerializeField]
	protected 	string[] 		m_mailToRecipients;
	[SerializeField]
	protected 	string[] 		m_mailCCRecipients;
	[SerializeField]
	protected 	string[] 		m_mailBCCRecipients;

	[SerializeField, Header("Share Sheet Properties")]
	protected 	eShareOptions[]	m_excludedOptions	= new eShareOptions[0];

	[SerializeField, Header("Share Properties ")]
	protected 	string			m_shareMessage		= "share message";
	[SerializeField]
	protected 	string			m_shareURL			= "http://www.viewa.com";

	[SerializeField, Tooltip ("This demo consideres image relative to Application.persistentDataPath")]
	protected 	string 			m_shareImageRelativePath;

	public Texture2D m_ImageToShare;
    SideMenuPanel sideMenuPanel; //_profilePanel;

	void Awake()
	{
		buttons = gameObject.GetComponentsInChildren<Button>();
		SaveFileToPath ();
	}
		

	#region Message Sharing API Methods

	private bool IsMessagingServiceAvailable ()
	{
		return NPBinding.Sharing.IsMessagingServiceAvailable();
	}

	private void SendTextMessage ()
	{
		// Create composer
		MessageShareComposer	_composer	= new MessageShareComposer();
		_composer.Body						= m_smsBody;
		_composer.ToRecipients				= m_smsRecipients;

		// Show message composer
		NPBinding.Sharing.ShowView(_composer, FinishedSharing);
	}

	#endregion

	#region Mail Sharing API Methods

	private bool IsMailServiceAvailable ()
	{
		return NPBinding.Sharing.IsMailServiceAvailable();
	}

	private void SendPlainTextMail ()
	{
		// Create composer
		MailShareComposer	_composer	= new MailShareComposer();
		_composer.Subject				= m_mailSubject;
		_composer.Body					= m_plainMailBody;
		_composer.IsHTMLBody			= false;
		_composer.ToRecipients			= m_mailToRecipients;
		_composer.CCRecipients			= m_mailCCRecipients;
		_composer.BCCRecipients			= m_mailBCCRecipients;

		// Show share view
		NPBinding.Sharing.ShowView(_composer, FinishedSharing);
	}
    public void SendPlainTextMail (string subject, string mailBody, string [] toRecipients, string [] ccRecipients = null, string [] bccRecipients = null)
	{
		// Create composer
		MailShareComposer	_composer	= new MailShareComposer();
		_composer.Subject				= subject!=null?subject:m_mailSubject;
		_composer.Body					= mailBody!=null?mailBody:m_plainMailBody;
		_composer.IsHTMLBody			= false;
		_composer.ToRecipients			= toRecipients!= null?toRecipients:m_mailToRecipients;
		_composer.CCRecipients			= ccRecipients!= null?ccRecipients:m_mailCCRecipients;
		_composer.BCCRecipients			= bccRecipients!= null?bccRecipients:m_mailBCCRecipients;

		// Show share view
		NPBinding.Sharing.ShowView(_composer, FinishedSharing);
	}
	private void SendHTMLTextMail () 
	{
		// Create composer
		MailShareComposer	_composer	= new MailShareComposer();
		_composer.Subject				= m_mailSubject;
		_composer.Body					= m_htmlMailBody;
		_composer.IsHTMLBody			= true;
		_composer.ToRecipients			= m_mailToRecipients;
		_composer.CCRecipients			= m_mailCCRecipients;
		_composer.BCCRecipients			= m_mailBCCRecipients;

		// Show share view
		NPBinding.Sharing.ShowView(_composer, FinishedSharing);
	}

	public void SendHTMLTextMail (string subject, string mailBody, string [] toRecipients, string [] ccRecipients, string [] bccRecipients) 
	{
		// Create composer
		MailShareComposer	_composer	= new MailShareComposer();
		_composer.Subject				= subject!=null?subject:m_mailSubject;
		_composer.Body					= mailBody!=null?mailBody:m_htmlMailBody;
		_composer.IsHTMLBody			= true;
		_composer.ToRecipients			= toRecipients!= null?toRecipients:m_mailToRecipients;
		_composer.CCRecipients			= ccRecipients!= null?ccRecipients:m_mailCCRecipients;
		_composer.BCCRecipients			= bccRecipients!= null?bccRecipients:m_mailBCCRecipients;

		// Show share view
		NPBinding.Sharing.ShowView(_composer, FinishedSharing);
	}

	private void SendMailWithScreenshot ()
	{
		// Create composer
		MailShareComposer	_composer	= new MailShareComposer();
		_composer.Subject				= m_mailSubject;
		_composer.Body					= m_plainMailBody;
		_composer.IsHTMLBody			= false;
		_composer.ToRecipients			= m_mailToRecipients;
		_composer.CCRecipients			= m_mailCCRecipients;
		_composer.BCCRecipients			= m_mailBCCRecipients;
		_composer.AttachScreenShot();

		// Show share view
		NPBinding.Sharing.ShowView(_composer, FinishedSharing);
	}

	private void SendMailWithAttachment ()
	{
		// Create composer
		MailShareComposer	_composer	= new MailShareComposer();
		_composer.Subject				= m_mailSubject;
		_composer.Body					= m_plainMailBody;
		_composer.IsHTMLBody			= false;
		_composer.ToRecipients			= m_mailToRecipients;
		_composer.CCRecipients			= m_mailCCRecipients;
		_composer.BCCRecipients			= m_mailBCCRecipients;
		_composer.AddAttachmentAtPath(GetImageFullPath(), MIMEType.kPNG);

		// Show share view
		NPBinding.Sharing.ShowView(_composer, FinishedSharing);
	}

	#endregion

	#region FB Sharing API Methods

	private bool IsFBShareServiceAvailable ()
	{
		return NPBinding.Sharing.IsFBShareServiceAvailable();
	}

	private void ShareTextMessageOnFB ()
	{
		// Create composer
		FBShareComposer _composer	= new FBShareComposer();
		_composer.Text				= m_shareMessage;

		// Show share view
		NPBinding.Sharing.ShowView(_composer, FinishedSharing);			
	}

	private void ShareURLOnFB ()
	{
		// Create share sheet
		FBShareComposer _composer	= new FBShareComposer();
		_composer.Text				= m_shareMessage;
		_composer.URL				= m_shareURL;

		// Show composer
		NPBinding.Sharing.ShowView(_composer, FinishedSharing);
	}

	private void ShareScreenshotOnFB ()
	{
		// Create composer
		FBShareComposer _composer	= new FBShareComposer();
		_composer.Text				= m_shareMessage;
		_composer.AttachScreenShot();

		// Show share view
		NPBinding.Sharing.ShowView(_composer, FinishedSharing);
	}

	private void ShareImageOnFB ()
	{
		// Create composer
		FBShareComposer _composer	= new FBShareComposer();
		_composer.Text				= m_shareMessage;
		_composer.AttachImageAtPath(GetImageFullPath());

		// Show share view
		NPBinding.Sharing.ShowView(_composer, FinishedSharing);
	}

	#endregion

	#region Twitter Sharing API Methods

	private bool IsTwitterShareServiceAvailable ()
	{
		return NPBinding.Sharing.IsTwitterShareServiceAvailable();
	}

	private void ShareTextMessageOnTwitter ()
	{
		// Create composer
		TwitterShareComposer _composer	= new TwitterShareComposer();
		_composer.Text					= m_shareMessage;

		// Show share view
		NPBinding.Sharing.ShowView(_composer, FinishedSharing);			
	}

	private void ShareURLOnTwitter ()
	{
		// Create share sheet
		TwitterShareComposer _composer	= new TwitterShareComposer();
		_composer.Text					= m_shareMessage;
		_composer.URL					= m_shareURL;

		// Show composer
		NPBinding.Sharing.ShowView(_composer, FinishedSharing);			
	}

	private void ShareScreenshotOnTwitter ()
	{
		// Create composer
		TwitterShareComposer _composer	= new TwitterShareComposer();
		_composer.Text					= m_shareMessage;
		_composer.AttachScreenShot();

		// Show share view
		NPBinding.Sharing.ShowView(_composer, FinishedSharing);			
	}

	private void ShareImageOnTwitter ()
	{
		// Create composer
		TwitterShareComposer _composer	= new TwitterShareComposer();
		_composer.Text					= m_shareMessage;
		_composer.AttachImageAtPath(GetImageFullPath());

		// Show share view
		NPBinding.Sharing.ShowView(_composer, FinishedSharing);			
	}

	#endregion

	#region WhatsApp Sharing API Methods

	private bool IsWhatsAppServiceAvailable ()
	{
		return NPBinding.Sharing.IsWhatsAppServiceAvailable();
	}

	private void ShareTextMessageOnWhatsApp ()
	{
		// Create composer
		WhatsAppShareComposer _composer	= new WhatsAppShareComposer();
		_composer.Text					= m_shareMessage;

		// Show share view
		NPBinding.Sharing.ShowView(_composer, FinishedSharing);			
	}

	private void ShareScreenshotOnWhatsApp ()
	{
		// Create composer
		WhatsAppShareComposer _composer	= new WhatsAppShareComposer();
		_composer.AttachScreenShot();

		// Show share view
		NPBinding.Sharing.ShowView(_composer, FinishedSharing);	
	}

	private void ShareImageOnWhatsApp ()
	{
		// Create composer
		WhatsAppShareComposer _composer	= new WhatsAppShareComposer();
		_composer.AttachImageAtPath(GetImageFullPath());

		// Show share view
		NPBinding.Sharing.ShowView(_composer, FinishedSharing);	
	}

	#endregion

	#region Social Share Sheet API Methods

	private void ShareTextMessageOnSocialNetwork ()
	{
		// Create share sheet
		SocialShareSheet _shareSheet 	= new SocialShareSheet();	
		_shareSheet.Text				= m_shareMessage;

		// Show composer
		NPBinding.UI.SetPopoverPointAtLastTouchPosition();
		NPBinding.Sharing.ShowView(_shareSheet, FinishedSharing);
	}

	private void ShareURLOnSocialNetwork ()
	{
		// Create share sheet
		SocialShareSheet _shareSheet 	= new SocialShareSheet();	
		_shareSheet.Text				= m_shareMessage;
		_shareSheet.URL					= m_shareURL;

		// Show composer
		NPBinding.UI.SetPopoverPointAtLastTouchPosition();
		NPBinding.Sharing.ShowView(_shareSheet, FinishedSharing);
	}

	private void ShareScreenShotOnSocialNetwork ()
	{
		// Create share sheet
		SocialShareSheet _shareSheet 	= new SocialShareSheet();	
		_shareSheet.Text				= m_shareMessage;
		_shareSheet.AttachScreenShot();

		// Show composer
		NPBinding.UI.SetPopoverPointAtLastTouchPosition();
		NPBinding.Sharing.ShowView(_shareSheet, FinishedSharing);
	}

	private void ShareImageOnSocialNetwork ()
	{
        m_shareMessage = "Viewa is a mobile content publishing platform that connects physical objects, such as magazines, catalogues,\noutdoor and packaging to premium multimedia experiences via augmented reality technology.\nhttp://www.viewa.com/";
//		m_shareURL = "http://www.viewa.com/";
		// Create share sheet
		SocialShareSheet _shareSheet 	= new SocialShareSheet();	
		_shareSheet.Text				= m_shareMessage;
		_shareSheet.AttachImageAtPath(GetImageFullPath());

		// Show composer
		NPBinding.UI.SetPopoverPointAtLastTouchPosition();
		NPBinding.Sharing.ShowView(_shareSheet, FinishedSharing);
	}

	public void ShareContentOnSocialPlatform(){

        m_shareMessage = "Viewa is a mobile content publishing platform that connects physical objects, such as magazines, catalogues,\noutdoor and packaging to premium multimedia experiences via augmented reality technology.\nhttp://www.viewa.com"; 
        //if (Application.platform == RuntimePlatform.Android)
        //{
        //    m_shareURL = "https://play.google.com/store/apps/details?id=au.com.mocom.acp";
        //} 
        //else if(Application.platform == RuntimePlatform.IPhonePlayer) {

        //    m_shareURL = "https://itunes.apple.com/in/app/viewa/id526647985?mt=8";
        //}
        m_shareURL = "https://download.viewa.com/";
		//SocialShareSheet _shareSheet 	= new SocialShareSheet();
        ShareSheet _shareSheet = new ShareSheet();
		_shareSheet.Text				= m_shareMessage;
		_shareSheet.URL					= m_shareURL;
		//_shareSheet.AttachImageAtPath(GetImageFullPath());

		// Show composer
		NPBinding.UI.SetPopoverPointAtLastTouchPosition();
		NPBinding.Sharing.ShowView(_shareSheet, FinishedSharing);
	}

	public void ShareDynamicContentOnSocialPlatform(string shareMessage, string shareURL){

        m_shareMessage = "Viewa is a mobile content publishing platform that connects physical objects, such as magazines, catalogues,\noutdoor and packaging to premium multimedia experiences via augmented reality technology.\nhttp://www.viewa.com";
        //if (Application.platform == RuntimePlatform.Android)
        //{
        //    m_shareURL = "https://play.google.com/store/apps/details?id=au.com.mocom.acp";
        //}
        //else if (Application.platform == RuntimePlatform.IPhonePlayer)
        //{

        //    m_shareURL = "https://itunes.apple.com/in/app/viewa/id526647985?mt=8";
        //}
        if (shareURL == null || shareURL == "")
        {
            m_shareURL = "https://download.viewa.com/";
        }

        //SocialShareSheet _shareSheet 	= new SocialShareSheet();
        ShareSheet _shareSheet = new ShareSheet();

		if (shareMessage == "") {
            _shareSheet.Text				= WebService.Instnace.historyData.history_Description + "\n" + m_shareMessage;
		} else {
			_shareSheet.Text				= shareMessage;
		}
        _shareSheet.URL					= shareURL!= "" ? shareURL : m_shareURL;

		//_shareSheet.AttachImageAtPath(WebService.Instnace.historyData.history_Image);
        _shareSheet.AttachImage(m_ImageToShare);

        // Show composer
        NPBinding.UI.SetPopoverPointAtLastTouchPosition();
        NPBinding.Sharing.ShowView(_shareSheet, FinishedSharing);
	}

	#endregion

	#region Share Sheet API Methods

	private void ShareTextMessageUsingShareSheet ()
	{
		// Create share sheet
		ShareSheet _shareSheet 	= new ShareSheet();	
		_shareSheet.Text		= m_shareMessage;
		_shareSheet.ExcludedShareOptions	= m_excludedOptions;

		// Show composer
		NPBinding.UI.SetPopoverPointAtLastTouchPosition();
		NPBinding.Sharing.ShowView(_shareSheet, FinishedSharing);
	}

	private void ShareURLUsingShareSheet ()
	{
		// Create share sheet
		ShareSheet _shareSheet 	= new ShareSheet();	
		_shareSheet.Text		= m_shareMessage;
		_shareSheet.URL			= m_shareURL;
		_shareSheet.ExcludedShareOptions	= m_excludedOptions;

		// Show composer
		NPBinding.UI.SetPopoverPointAtLastTouchPosition();
		NPBinding.Sharing.ShowView(_shareSheet, FinishedSharing);
	}

	private void ShareScreenShotUsingShareSheet ()
	{
		// Create share sheet
		ShareSheet _shareSheet 	= new ShareSheet();	
		_shareSheet.Text		= m_shareMessage;
		_shareSheet.ExcludedShareOptions	= m_excludedOptions;
		_shareSheet.AttachScreenShot();

		// Show composer
		NPBinding.UI.SetPopoverPointAtLastTouchPosition();
		NPBinding.Sharing.ShowView(_shareSheet, FinishedSharing);
	}

	private void ShareImageAtPathUsingShareSheet ()
	{
		// Create share sheet
		ShareSheet _shareSheet 	= new ShareSheet();	
		_shareSheet.Text		= m_shareMessage;
		_shareSheet.ExcludedShareOptions	= m_excludedOptions;
		_shareSheet.AttachImageAtPath(GetImageFullPath());

		// Show composer
		NPBinding.UI.SetPopoverPointAtLastTouchPosition();
		NPBinding.Sharing.ShowView(_shareSheet, FinishedSharing);
	}

	#endregion

	#region API Callback Methods

	private void FinishedSharing (eShareResult _result)
	{
		
	}
	private void PickImageFinished (ePickImageFinishReason _reason, Texture2D _image)
	{
		if (_reason == ePickImageFinishReason.FAILED) {
			//AppManager.Instnace.messageBoxManager.ShowMessage ("Save", "Error uploading profile picture.", "Ok");
		}
		if (_reason == ePickImageFinishReason.SELECTED) {

            TextureScale.Bilinear (_image, 200, 200);
            sideMenuPanel = (SideMenuPanel)CanvasManager.Instnace.ReturnPanelManager(ePanelManager.MainMenuPanelManager).ReturnPanel(ePanels.SideMenuPanel);
            sideMenuPanel._myProfileImage.sprite = Sprite.Create (_image, new Rect (0, 0, 200, 200), new Vector2 ()); 
            AppManager.Instnace.SavePicture (_image, AppManager.Instnace.GetManualProfilePicPath());
            AppManager.Instnace.messageBoxManager.ShowGenericPopup ("Save", "Profile picture uploaded successfully.", "Ok");
		}

	}

	private void SaveImageToGalleryFinished (bool _saved)
	{
		
	}
	#endregion

	#region Misc. Methods

	public Texture2D GetImageTexture(){
		if (File.Exists (GetImageFullPath())) {
			byte[] byteArray = File.ReadAllBytes (GetImageFullPath());
			Texture2D texture = new Texture2D (128, 128);
			texture.LoadImage (byteArray);
			return texture;
		} else {
			Debug.Log ("Texture does not exist");
			return null;
		}
	}

	private string GetImageFullPath ()
	{
		return Application.persistentDataPath + "/" + m_shareImageRelativePath;
	}
	private void SaveFileToPath() {

		m_shareImageRelativePath = "shareImage.png";

		if (File.Exists (Application.persistentDataPath + "/" + m_shareImageRelativePath)) {
			Debug.Log ("Full Path: " + Application.persistentDataPath + "/" + m_shareImageRelativePath);
			return;
		} else {
			byte[] bytes = m_ImageToShare.EncodeToPNG();
			File.WriteAllBytes (Application.persistentDataPath + "/shareImage.png", bytes);
		}
	}
	#endregion

	/// <summary>
	/// Opens an user interface to pick an image from specified image source.
	/// </summary>
	/// <param name="_source">The source to use to pick an image.</param>
	/// <param name="_scaleFactor">The factor used to rescale selected image. Having value as 1.0f returns the image without any modification.</param>
	/// <param name="_onCompletion">Callback that will be called after operation is completed.</param>
	public void PickImageFromAlbum ()
	{
		// Pause unity player
		// Set popover to last touch position
		NPBinding.UI.SetPopoverPointAtLastTouchPosition();

		// Pick image
		NPBinding.MediaLibrary.PickImage(eImageSource.ALBUM, 1.0f, PickImageFinished);
	}

	public void PickImageFromCamera ()
	{
		// Set popover to last touch position
		NPBinding.UI.SetPopoverPointAtLastTouchPosition();

		// Pick image
		NPBinding.MediaLibrary.PickImage(eImageSource.CAMERA, 1.0f, PickImageFinished);
	}

	public void PickImageFromBoth ()
	{
		// Set popover to last touch position
		NPBinding.UI.SetPopoverPointAtLastTouchPosition();

		// Pick image
		NPBinding.MediaLibrary.PickImage(eImageSource.BOTH, 1.0f, PickImageFinished);
	}
}

