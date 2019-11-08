using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ACP;

/// <summary>
/// This class is responsible for downloading textures from the internet, and saving them to disk locally so they can be re-accessed fast.
/// The general jist of it is you subscribe to the TextureLoaded and TextureFailed handlers, and then load textures via a StartCoroutine(LoadTexture(url))
/// </summary>
namespace ACP
{
	public class DownloadCache : MonoBehaviour
	{
		public enum DownloadType
		{
			TextureDownload,
			AudioClipDownload,
			FileDownload,
			AssetBundleDownload
		}
			
		// Delegates
	   	public delegate void DownloadCompleteFileEventHandler(string url, byte[] data);
	   	public delegate void DownloadCompleteTextureEventHandler(string url, Texture2D texture);
	   	public delegate void DownloadCompleteAudioClipEventHandler(string url, AudioClip audioClip);
		public delegate void DownloadCompleteAssetBundleEventHandler(string url, AssetBundle assetBundle);
	    public delegate void DownloadFailedFailedEventHandler(string url, DownloadType downloadType);
		
		// Events
		public event DownloadCompleteFileEventHandler DownloadCompleteFile;
		public event DownloadCompleteTextureEventHandler DownloadCompleteTexture;
		public event DownloadCompleteAudioClipEventHandler DownloadCompleteAudioClip;
		public event DownloadCompleteAssetBundleEventHandler DownloadCompleteAssetBundle;
		public event DownloadFailedFailedEventHandler DownloadFailed;
		
		// Keep track of active requests and the queue, etc...
		Dictionary<string, bool> ActiveRequests = new Dictionary<string, bool>();
		List<string> QueuedRequests = new List<string>(); // This is the list of url md5's that are in the queue waiting
		Dictionary<string, KeyValuePair<string, DownloadType>> QueuedRequestUrls = new Dictionary<string, KeyValuePair<string, DownloadType>>(); // Used to get the url for a md5

		//bed - 15th May 2014 - so originally this was set to 5 concurrent downloads - however - this made the new download progress bar not function very well from the user's perspective.
		// in addition I don't think UNITY inside iOS handled the 5 concurrent downloads very efficiently, but actually made things slower
		// so leaving these downloads as a serial queue makes the *perceived* progress faster and smoother 'more accurate' - and in practise the downloads seem faster too.
		public const int MaxConcurrentRequests = 1;  // How many requests can we run at the same time...

		private DateTime? ignoreCachedFilesEarlierThanDate = null;
		public DateTime? IgnoreCachedFilesEarlierThanDate
		{
			get {
				return ignoreCachedFilesEarlierThanDate;
			} set {
				ignoreCachedFilesEarlierThanDate = value;
				//Debug.Log ("Setting ignoreCachedFilesEarlierThanDate to "+ ignoreCachedFilesEarlierThanDate);
			}
		}

		public bool cacheEnabled = true;
		public bool CacheEnabled
		{
			get 
			{
				return cacheEnabled;
			} set {
				cacheEnabled = value;
				//Debug.Log ("Setting Cache Enabled to " + cacheEnabled);
			}
		}		
		
		private static DownloadCache instance;
		public static DownloadCache Instance
		{
			get
			{
				if (instance == null)
				{
					instance = DownloadCache.Create ();
				}
				return instance;
			}
		}
		
		public void Awake()
		{
//#if UNITY_EDITOR
//			CacheEnabled = false;
//#endif
		}
		
		public static DownloadCache Create()
		{
			GameObject gameObject = new GameObject();
			DownloadCache cache = gameObject.AddComponent<DownloadCache>();
			instance = cache;
			gameObject.name = "DownloadCache";
			//Debug.LogWarning ("Created New Download Cache!");
			return instance;
		}
		
		private DownloadCache ()
		{	
		}
		
		public string GetDownloadCachePath(DownloadType type)
		{
			switch (type)
			{
			case DownloadType.TextureDownload:
				return GetTextureCachePath();
			case DownloadType.AudioClipDownload:
				return GetAudioClipCachePath();
			case DownloadType.AssetBundleDownload:
				return GetFileCachePath();
			case DownloadType.FileDownload:
			default:
				return GetFileCachePath();
			}
		}
		
		public string GetFileCachePath()
		{
			string path = Utility.GetTempFilePath() + "/DownloadCache/";
			
			if (!Directory.Exists(path))
			{
				Directory.CreateDirectory(path);
			}
			return path;
		}
		
		public string GetTextureCachePath()
		{
			string path = Utility.GetTempFilePath() + "/Textures/";
			
			if (!Directory.Exists(path))
			{
				Directory.CreateDirectory(path);
			}
			return path;
		}
		
		public string GetAudioClipCachePath()
		{
			string path = Utility.GetTempFilePath() + "/Sounds/";
			
			if (!Directory.Exists(path))
			{
				Directory.CreateDirectory(path);
			}
			return path;
		}

		protected void SaveCacheFileIfRequired (WWW www, string path)
		{
#if UNITY_WEBPLAYER
#else
			// If the request was OK
			if (www.error == null)
			{
				// Try and save it to disk
				byte[] b = www.bytes;
				
				if (b != null)
				{
					File.WriteAllBytes(path, b);
					
					/*
					FileStream  fs = File.Create(path);
					fs.Write(b, 0, b.Length);
					fs.Close();
					*/
				}
			}
#endif
		}
		
		protected void DownloadComplete(string url, WWW www, DownloadType downloadType)
		{
			switch (downloadType)
			{
			case DownloadType.TextureDownload:
				if (DownloadCompleteTexture != null)
				{
					DownloadCompleteTexture(url, www.texture);
				}
				break;
			case DownloadType.AudioClipDownload:
				if (DownloadCompleteAudioClip != null)
				{
					DownloadCompleteAudioClip(url, www.GetAudioClip(false));
				}
				break;
			case DownloadType.AssetBundleDownload:
				if (DownloadCompleteAssetBundle != null)
				{
					DownloadCompleteAssetBundle(url, www.assetBundle);
				}
				break;
			case DownloadType.FileDownload:
			default:
				if (DownloadCompleteFile != null)
				{
				 	DownloadCompleteFile(url, www.bytes);
				}
				break;
			}
		}
		
		public void LoadFile(string url, DownloadType type)
		{
			StartCoroutine(LoadFileImpl(url, type));
		}
		
		private IEnumerator LoadFileImpl (string url, DownloadType type)
		{

			string md5 = Utility.Md5Sum (url);

			//we UNescape and then escape again to ensure we handle all cases - see https://visualjazz.jira.com/browse/VR-317
			string unescapedURL = Uri.UnescapeDataString(url);
			string escapedURL = Uri.EscapeUriString(unescapedURL);

			// Check we are not requesting the file currently
			if (!ActiveRequests.ContainsKey(md5) && !QueuedRequestUrls.ContainsKey(md5))
			{
				// If we are running too many requests, just add them to the queue
				if (ActiveRequests.Keys.Count >= MaxConcurrentRequests)
				{
					// If this url is not queued we add it to the queue
					if (!QueuedRequestUrls.ContainsKey(md5))
					{
						//ACPUnityPlugin.logNative(string.Format("Too many requests.. adding {0} to queue.", url));
						QueuedRequests.Insert(0, md5);
						QueuedRequestUrls[md5] = new KeyValuePair<string, DownloadType>(url, type);
					}
					else
					{
						// URL is already queued, move it to the front as it was requested again...
						//ACPUnityPlugin.logNative(string.Format("Too many requests.. moving {0} to front of queue", url));
						//Debug.Log ("1.... Moving to front");
						QueuedRequests.Remove(md5);
						QueuedRequests.Insert(0, md5);
					}
				}
				else
				{
					ActiveRequests[md5] = true;

					//Debug.Log ("Starting Download for "+url);
					WWW www= new WWW(escapedURL);
					bool fromFile = false;
					String cachePath = null;
					// Check for local file if we are caching
					if (this.CacheEnabled)
					{
						cachePath = GetDownloadCachePath(type) + md5;
						
						// HACK: Audio clip download check is a slight hack as cache doesn't seem to work correctly for those as unity struggles to figure out the file type...
						if (File.Exists(cachePath) && type != DownloadType.AudioClipDownload)
						{
							//Debug.Log ("Loading from disk cache for: " + url);
							//www.Dispose();
							bool useFileCache = true;
							//if we have a value for ignoreCachedFilesEarlierThanDate then check the local file created date
							if(ignoreCachedFilesEarlierThanDate.HasValue){
								DateTime resposnseModifiedDateTime = ignoreCachedFilesEarlierThanDate.Value;
								DateTime cachedFileDateTime = File.GetCreationTime(cachePath);
								//Debug.Log ("Cached File DateTime is " + cachedFileDateTime + " - response DateTime is " + resposnseModifiedDateTime);
								if(resposnseModifiedDateTime > cachedFileDateTime){
									useFileCache = false; // don't use the file cache
									//Debug.Log ("...ignoring cached file!");
								}
							}
							if(useFileCache){
								www = new WWW("file://" + cachePath);
								fromFile = true;
								//Debug.Log ("...returning file from file cache!");
							}
						}
					}

					yield return www;
					
					//Debug.Log ("Finished Download for "+url);
					// Save to cache if we need to
					if (this.CacheEnabled && !fromFile)
					{
						//Debug.Log("Saving file to cache: " + www.url);
						SaveCacheFileIfRequired (www, cachePath);
					}
					
					// Remove from active requests
					ActiveRequests.Remove(md5);
					
					// Check for waiting requests in the queue
					CheckRequestQueue();
					
					// If we error'd, let people know we failed
					if (www.error != null)
					{
						//Debug.LogError("Unable to load download data: " + www.error);
						if (DownloadFailed != null)
						{
							DownloadFailed(url, type);
						}
					}
					else
					{
						// Request completed!
						DownloadComplete(url, www, type);
					}

					www.Dispose();
				}
			}
			else
			{
				//Debug.Log("prioritising to the top: " + url);

				QueuedRequests.Remove(md5);
				QueuedRequests.Insert(0, md5);
				QueuedRequestUrls[md5] = new KeyValuePair<string, DownloadType>(url, type);
			}
		}
			
		
		public void CheckRequestQueue()
		{
			// Check to see if there are any downloads waiting
			if (QueuedRequests.Count > 0)
			{
				// There are! - get the md5 and url
				string queuedMd5 = QueuedRequests[0];
				KeyValuePair<string, DownloadType> queuedData = QueuedRequestUrls[queuedMd5];
				// Remove from the queue
				QueuedRequests.RemoveAt(0);
				QueuedRequestUrls.Remove(queuedMd5);
				
				// Start downloading them...
				StartCoroutine(LoadFileImpl(queuedData.Key, queuedData.Value));
			}
		}
		
	}
}

