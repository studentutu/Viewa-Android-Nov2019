using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;

public class ImageCachedDownloader : MonoBehaviour {

	string imagePath;
	Image image;

	void Awake() {

		image = gameObject.GetComponent<Image> ();
	}


	public void StatDownloadingTexture(string aurl){
		StartCoroutine (CacheTexture (aurl));
	}

	public IEnumerator CacheTexture(string url) {
		if (File.Exists (Application.persistentDataPath + imagePath)) {
			print ("ImageCachedDownloader - Loading from the device");
			byte[] byteArray = File.ReadAllBytes (imagePath);
			Texture2D texture = new Texture2D (100, 100);
			texture.LoadImage (byteArray);
			image.sprite = Sprite.Create (texture, new Rect (0, 0, texture.width, texture.height), new Vector2 (0, 0));
		} else {
			WWW www = new WWW (url);
			yield return www;
			Texture2D texture = www.texture;
			image.sprite = Sprite.Create (www.texture, new Rect (0, 0, www.texture.width, www.texture.height), new Vector2 (0, 0));
			byte[] bytes = texture.EncodeToJPG ();
			imagePath = GetFileCachePath() + System.Guid.NewGuid();
			File.WriteAllBytes (imagePath, bytes);
		}
	}

	public string GetFileCachePath()
	{
		string path = Application.persistentDataPath + "/Cache/";

		if (!Directory.Exists(path))
		{
			Directory.CreateDirectory(path);
		}
		return path;
	}


}
