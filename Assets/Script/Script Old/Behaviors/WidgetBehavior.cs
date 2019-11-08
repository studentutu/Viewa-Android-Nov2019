using UnityEngine;
using System.Collections;
using ACP;

/// <summary>
/// Base class for a widget - basically just supplies the remove method :)
/// </summary>
public abstract class WidgetBehavior : MonoBehaviour
{
	public int index;

	// Use this for initialization
	void Start ()
	{
		
	}

	// Update is called once per frame
	void Update ()
	{
		
	}
	
	public abstract WidgetData Data { get; }
	
	public abstract void Remove();
	
	public GameObject GetModelObj(ModelObjData data) 
	{
		if (data== null) return null;
		
		AreaBehavior area = transform.parent.GetComponent<AreaBehavior> ();
		if (area != null) {
			//Debug.Log("GetTexture::Found the area!");
			if (area.Models.ContainsKey (data)) {
				//Debug.Log("Found the texture!");
				return area.Models [data];
			} else {
				//Debug.Log("GetTexture::Couldn't find the audio  clip :(");
			}
		} else {
			//Debug.Log("Couldn't find the area.... :(");
		}
		
		return null;
	}
	
	public Texture2D GetTexture (string url)
	{
		if (url == null)
			return null;
		
		AreaBehavior area = transform.parent.GetComponent<AreaBehavior> ();
		if (area != null) {
			//Debug.Log("GetTexture::Found the area!");
			if (area.Textures.ContainsKey (url)) {
				//Debug.Log("Found the texture!");
				return area.Textures [url];
			} else {
				Debug.LogError("GetTexture::Couldn't find the texture: " + url);
			}
		} else {
			//Debug.Log("Couldn't find the area.... :(");
		}
		
		return null;
	}
	
	public AudioClip GetAudioClip (string url)
	{
		if (url == null)
			return null;
		
		AreaBehavior area = transform.parent.GetComponent<AreaBehavior> ();
		if (area != null) {
			//Debug.Log("GetTexture::Found the area!");
			if (area.AudioClips.ContainsKey (url)) {
				//Debug.Log("Found the texture!");
				return area.AudioClips [url];
			} else {
				//Debug.Log("GetTexture::Couldn't find the audio  clip :(");
			}
		} else {
			//Debug.Log("Couldn't find the area.... :(");
		}
		
		return null;
	}
	
	public byte[] GetFile(string url)
	{
		if (url == null) return null;
	
			
		AreaBehavior area = transform.parent.GetComponent<AreaBehavior>();
		if (area != null)
		{
//			Debug.Log("GetFile::Found the area!");
			if (area.Files.ContainsKey(url))
			{
//				Debug.Log("Found the texture!");
				return area.Files[url];
			}
			else
			{
//				Debug.Log("GetTexture::Couldn't find the texture :(");
			}
		}
		else
		{
//			Debug.Log("Couldn't find the area.... :(");
		}
		
		return null;
	}

	public virtual void TrackingFound(bool sticky)
	{
	}

	public virtual void TrackingLost(bool sticky)
	{
	}
	
}

