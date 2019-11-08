using UnityEngine;
using System.Collections;
using ACP;

/// <summary>
/// Just an image...
/// </summary>
public class ImageBehavior : WidgetBehavior
{

	public Texture image;
	public Texture imageReflection;
	public GameObject plane;

	
	public override WidgetData Data { 
		get { return data; }
	}

	public ImageData data;

	// Use this for initialization
	void Start ()
	{
		if (data.imageReflectionUrl != null)
		{
			plane.GetComponent<Renderer>().material = Camera.main.GetComponent<ACPTrackingManager>().materialUIShiny;
		}
		else
		{
			plane.GetComponent<Renderer>().material = Camera.main.GetComponent<ACPTrackingManager>().materialUITransparent;
		}
		//plane.renderer.material.mainTextureScale = new Vector2(-1, -1);

//		Color c = plane.renderer.material.color;
//		c.a = 255;
//		plane.renderer.material.color = c;
		
		if (data != null)
		{
			plane.transform.localScale = new Vector3(-data.size.x / 10.0f, 1, -data.size.z / 10.0f);

			transform.localPosition = data.position;
			transform.localRotation = Quaternion.Euler(data.rotation);
			
			image = GetTexture(data.imageUrl);
			
			if (image != null)
			{
			image.wrapMode = TextureWrapMode.Clamp;
			plane.GetComponent<Renderer>().material.mainTexture = image;
			}
			//TextureCache.Instance.TextureLoaded -= TextureLoaded;
			
			//StartCoroutine(tc.LoadTexture(data.imageUrl));
			if (data.imageReflectionUrl != null)
			{
				imageReflection = GetTexture(data.imageReflectionUrl);
				if (imageReflection != null)
				{
					plane.GetComponent<Renderer>().material.SetTexture("_MaskTex", imageReflection);
				}
				//StartCoroutine(tc.LoadTexture(data.imageReflectionUrl));
			}

			this.name = "Image";
			
			foreach (EffectData effect in data.appearEffects)
			{
				effect.RunEffect(this.gameObject);
			}
		}
	}
	
	public void OnDestroy ()
	{
	}

	
	public void TextureLoaded(string url, Texture texture)
	{
		//Debug.Log("ButtonBehavior::TextureLoaded(" + url + "," + texture + ")");
		
		if (url == data.imageUrl)
		{
			image = texture;
			image.wrapMode = TextureWrapMode.Clamp;
			plane.GetComponent<Renderer>().material.mainTexture = image;
			//TextureCache.Instance.TextureLoaded -= TextureLoaded;
		}
		
		if (url == data.imageReflectionUrl)
		{
			//Debug.Log("Loaded reflection:" + url);
			imageReflection = texture;

			plane.GetComponent<Renderer>().material.SetTexture("_MaskTex", imageReflection);
			//plane.renderer.material.mainTextureScale = new Vector2(-1, -1);
		}
	}
	
	
	// Update is called once per frame
	void Update ()
	{
	}
	
	void FixedUpdate()
	{
	}
	

	public override void Remove()
	{
		if (data.disappearEffects.Count > 0)
		{
			foreach (EffectData effect in data.disappearEffects)
			{
				effect.RunEffect(this.gameObject);
			}
			
			TimedObjectDestructor dest = gameObject.AddComponent<TimedObjectDestructor>();
			dest.timeOut = EffectData.FindLongestEffect(data.disappearEffects);
			//Debug.Log("Destroying in " + dest.timeOut);
		}
		else
		{
			Destroy(gameObject);
		}
	}
	
}

