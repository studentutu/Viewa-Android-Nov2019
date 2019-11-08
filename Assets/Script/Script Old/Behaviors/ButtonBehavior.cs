using UnityEngine;
using System.Collections;
using ACP;

/// <summary>
/// Just a button..
/// </summary>
public class ButtonBehavior : WidgetBehavior
{

	public string imageUrl;
	public string imagePressedUrl;
	
	public Texture2D image;
	public Texture2D imagePressed;
	public Texture2D imageReflection;
	
	public GameObject plane;
	
	private bool isPressed = false;
	private bool hoverLastFrame = false;
	
	public bool Enabled = false;
	
	public ButtonData data;

   	public delegate void ButtonPressedEventHandler(ButtonBehavior sender);
    public event ButtonPressedEventHandler ButtonPressed;
	
	public override WidgetData Data { 
		get { return data; }
	}

	// Use this for initialization
	void Start ()
	{
		plane.gameObject.layer = 8;
		// Layer 8 is checked by the ACPTrackingManager script to look for tap events on shit.
		if (data.imageReflectionUrl != null) {
			plane.GetComponent<Renderer>().material = Camera.main.GetComponent<ACPTrackingManager> ().materialUIShiny;
		} else {
			plane.GetComponent<Renderer>().material = Camera.main.GetComponent<ACPTrackingManager> ().materialUITransparent;
		}
		//renderer.material.mainTextureScale = new Vector2(-1, -1);
		
		// TODO: I can't remember why this is here??? :)
		//		Color c = plane.renderer.material.color;
		//		c.a = 255;
		//		plane.renderer.material.color = c;
		//		
		// We should always have data here...
		if (data != null) {
		
			// Set up our properties from the JSON data...
			this.name = "Button " + data.action;
			
			plane.transform.localScale = new Vector3 (-data.size.x / 10.0f, 1, -data.size.z / 10.0f);
			transform.localPosition = data.position;
			transform.localRotation = Quaternion.Euler (data.rotation);
			imageUrl = data.imageUrl;
			imagePressedUrl = data.imagePressedUrl;
			
			image = GetTexture (imageUrl);
			if (image != null) {
				image.wrapMode = TextureWrapMode.Clamp;
				plane.GetComponent<Renderer>().material.mainTexture = image;
			} else {
				plane.SetActive(false);
			}
			
			imagePressed = GetTexture (imagePressedUrl);
			if (imagePressed != null)
				imagePressed.wrapMode = TextureWrapMode.Clamp;
			
			if (data.imageReflectionUrl != null) {
				imageReflection = GetTexture (data.imageReflectionUrl);
				plane.GetComponent<Renderer>().material.SetTexture ("_MaskTex", imageReflection);
				//StartCoroutine(tc.LoadTexture(data.imageReflectionUrl));
			}
			
			// TODO: Maybe only enable buttons once they have finished running their effects?? 			
			//float timeUntilEnabled = EffectData.FindLongestEffect(data.appearEffects);
			Enabled = true;
			foreach (EffectData effect in data.appearEffects) {
				effect.RunEffect (this.gameObject);
			}
		} else {
			Debug.LogError ("Danger Will Robinson! ButtonBehavior started with no data!");
		}
		
		if (data.autoPlay) {
			if((data.action != ButtonData.ButtonAction.Page) && (data.action != ButtonData.ButtonAction.Share)){
				ACPUnityPlugin.Instnace.buttonWasAutoPlayed(); //let the host app know that a button was autoPlayed
			}
			ButtonPressed (this);
		}
	}
	
	public void OnDestroy ()
	{
	}
	
	public void TextureLoaded(string url, Texture2D texture)
	{
		//Debug.Log("ButtonBehavior::TextureLoaded(" + url + "," + texture + ")");
		
		if (url == imageUrl)
		{
			image = texture;
			image.wrapMode = TextureWrapMode.Clamp;
			//image.Apply();
			
			if (!isPressed)  plane.GetComponent<Renderer>().material.mainTexture = image;

		}
		if (url == imagePressedUrl)
		{
			imagePressed = texture;
			imagePressed.wrapMode = TextureWrapMode.Clamp;
			//imagePressed.Apply();

			if (isPressed)  plane.GetComponent<Renderer>().material.mainTexture = imagePressed;
		}
		
		if (image != null && imagePressed != null)
		{
			//TextureCache.Instance.TextureLoaded -= TextureLoaded;
		}
	}
	
	// Update is called once per frame
	void Update ()
	{
		// Are we hovering? Or were pressed?
		if (hoverLastFrame)
		{
			hoverLastFrame = false;
		}
		else if (isPressed)
		{
			//Debug.Log("Button out");
			isPressed = false;
			plane.GetComponent<Renderer>().material.mainTexture = image;
			plane.GetComponent<Renderer>().material.color = Color.white;
		}
	}
	
	public void OnHover(bool released)
	{
		// Buttony type things ..
		
		if (!Enabled) return;
		
		hoverLastFrame = true;
		if (!isPressed)
		{
			isPressed = true;
			//Debug.Log("Button on");
			
			if (imagePressed != null)
			{
				plane.GetComponent<Renderer>().material.mainTexture = imagePressed;
				plane.GetComponent<Renderer>().material.color = Color.white;
			} else {
				Color pressedColor = Color.white;
				pressedColor.a = 0.8f;
				plane.GetComponent<Renderer>().material.color = pressedColor;
			}
		}
		
		if (isPressed && released)
		{
			Debug.Log("Button click:- " + this.name);
			isPressed = false;
			plane.GetComponent<Renderer>().material.mainTexture = image;
			plane.GetComponent<Renderer>().material.color = Color.white;

			if (ButtonPressed != null)
			{
                AppManager.Instnace.doesButtonPressed = true;
				ButtonPressed(this);
			}
		}
	}

	public override void Remove()
	{
		// On remove, run all of our effects then go away...
		Enabled = false;
		if (data.disappearEffects.Count > 0)
		{
			foreach (EffectData effect in data.disappearEffects)
			{
				effect.RunEffect(this.gameObject);
			}
			
			TimedObjectDestructor dest = gameObject.AddComponent<TimedObjectDestructor>();
			dest.timeOut = EffectData.FindLongestEffect(data.disappearEffects);
		}
		else
		{
			Destroy(gameObject);
		}
	}
	
}

