using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VoxelBusters.Utility;
//using VoxelBusters.Utility.UnityGUI.MENU;
using VoxelBusters.NativePlugins;
using OTPL.UI;


public class VideoPlayingPage : MediaLibrary {


	private 	string		m_videoURL;
	[SerializeField]
	private 	string		m_youtubeVideoID;
	[SerializeField]
	private		TextAsset	m_vimeoPlayerHTML;
	[SerializeField]
	private 	string		m_vimeoVideoID;

	#region Video API Methods
	public void LoadFullscreenVideo(
		ulong triggerId,
		int areaIndex,
		int frameIndex,
		int widgetIndex,
		string url, float time, string endImage, string endUrl, string shareTitle, string shareText, string shareUrl) {
	
		Debug.Log ("VideoPlayingPage - LoadFullscreenVideo");

		PlayVideoFromURL (url);
	}

	private void PlayYoutubeVideo ()
	{
		NPBinding.MediaLibrary.PlayYoutubeVideo(m_youtubeVideoID, PlayVideoFinished);
	}

	private void PlayVideoFromURL (string _url)
	{
		Debug.Log ("PlayVideoFromURL: "+ _url);
		AppManager.Instnace.isDynamicDataLoaded = true;
		ACPUnityPlugin.Instnace.EnableButtons ();
        ACPUnityPlugin.Instnace.trackEvent("FullScreenVideo", "FullScreenVideoStart", "Start", 0);
        ACPUnityPlugin.Instnace.trackEvent("FullScreenVideo", "PlayFullScreenVideo", "Play-" + _url, 0);

		NPBinding.MediaLibrary.PlayVideoFromURL(URL.URLWithString(_url), PlayVideoFinished);
	}

	private void PlayVideoFromGallery ()
	{
		// Set popover to last touch position
		NPBinding.UI.SetPopoverPointAtLastTouchPosition();

		// Play video from gallery
		NPBinding.MediaLibrary.PlayVideoFromGallery(PickVideoFinished, PlayVideoFinished);
	}

	private void PlayEmbeddedVideo ()
	{
		string _embeddedVideoHTML	= m_vimeoPlayerHTML.text.Replace("$video_id", m_vimeoVideoID);
		NPBinding.MediaLibrary.PlayEmbeddedVideo(_embeddedVideoHTML, PlayVideoFinished);
	}

	#endregion

	#region API Callback Methods

	private void PickVideoFinished (ePickVideoFinishReason _reason)
	{
		Debug.Log("Request to pick video from gallery finished. Reason for finish is " + _reason + ".");

//		Screen.orientation = ScreenOrientation.Portrait;
	}
		
	public void PlayVideoFinished (ePlayVideoFinishReason _reason)
	{
        ACPUnityPlugin.Instnace.trackEvent("FullScreenVideo", "FullScreenVideoEnd", "End", 0);
		//Here we assume that the fullscreenvideo is played when areaFrameHistory.count == 1 i.e frameIndex == 0)
		//So after closing the video it should navigate to the prev screen or remain on that screen (scanScreen).
		Debug.Log("Request to play video finished. Reason for finish is " + _reason + ".");
        AppManager.Instnace.acpTrackingManager.OnBackButtonTapped();
	}

	#endregion
}
