using UnityEngine;
using System.Collections;
using ACP;

/// <summary>
/// Displays an OBJ format model
/// </summary>
public class ModelObjBehavior : WidgetBehavior
{

	public Texture image;
	public Texture imageReflection;
	
	GameObject model;
	
	//bool hasSetBounds;
	
	public ModelObjData data;
	OBJ obj;
	
	public override WidgetData Data { 
		get { return data; }
	}

	
	// Use this for initialization
	void Start ()
	{
		/*
		renderer.material = Camera.main.GetComponent<ACPTrackingManager>().materialUITransparent;

		Color c = renderer.material.color;
		c.a = 255;
		renderer.material.color = c;
		*/
		
		if (data != null) {
			
			this.transform.localPosition = Vector3.zero;
			this.transform.localRotation = Quaternion.identity;
			this.transform.localScale = Vector3.one;
			
			model = GetModelObj(data);
			model.transform.parent = this.transform;
			StartCoroutine(OnObjFinishLoading());
			/*
			
			model = new GameObject ();
			model.transform.parent = this.transform;
			
			obj = model.AddComponent<OBJ> ();
//			obj.materialPath = data.materialUrl;
//			obj.texturePath = data.imageUrl;
//			obj.objPath = data.modelUrl;
			obj.materialString = System.Text.ASCIIEncoding.ASCII.GetString (GetFile (data.materialUrl));
			obj.objString = System.Text.ASCIIEncoding.ASCII.GetString (GetFile (data.modelUrl));
			obj.diffuseTexture = GetTexture (data.imageUrl);
			*/
			
			//hasSetBounds = false;
			
			transform.GetChild (0).localScale = Vector3.one;
			transform.GetChild (0).localPosition = Vector3.zero;
			transform.GetChild (0).localRotation = Quaternion.Euler (data.rotation);
			
//			TextureCache tc = TextureCache.Instance;
//			tc.TextureLoaded += TextureLoaded;

			//StartCoroutine(tc.LoadTexture(data.imageUrl));
			image = GetTexture (data.imageUrl);
			
			if (data.imageReflectionUrl != null && data.imageReflectionUrl.Length > 0) {
				imageReflection = GetTexture (data.imageReflectionUrl);
				//StartCoroutine(tc.LoadTexture(data.imageReflectionUrl));
			}

			this.name = "Model Obj";
			

			foreach (EffectData effect in data.appearEffects) {
				effect.RunEffect (this.transform.GetChild (0).gameObject);
			}
		}
		
	}
	
	public IEnumerator OnObjFinishLoading ()
	{
		yield return new WaitForSeconds(0.01f);
		
		model.gameObject.SetActive(true);

		var combinedBounds = new Bounds (transform.position, Vector3.zero);
		var renderers = transform.GetChild (0).GetComponentsInChildren<Renderer> ();
		foreach (var render in renderers) {
			if (render != GetComponent<Renderer>()) {
				combinedBounds.Encapsulate (render.bounds);
			}
		} 
			
		//Debug.Log ("Combined bounds: " + combinedBounds.ToString ());
		
		float widthRatio = this.data.size.x / combinedBounds.extents.x;
		widthRatio /= 2.0f;
		
		//Debug.Log (widthRatio);
		
		this.transform.GetChild (0).localScale = new Vector3 (widthRatio, widthRatio, widthRatio);
		this.transform.GetChild (0).localPosition = data.position;
	}
	
	public void OnDestroy ()
	{
		model.gameObject.SetActive(false);
		model.transform.parent = null;
	}

	
	public void TextureLoaded(string url, Texture texture)
	{
		//Debug.Log("ButtonBehavior::TextureLoaded(" + url + "," + texture + ")");
		
		if (url == data.imageUrl)
		{
			image = texture;
			//renderer.material.mainTexture = image;
			//TextureCache.Instance.TextureLoaded -= TextureLoaded;
		}
		
		if (url == data.imageReflectionUrl)
		{
			//Debug.Log("Loaded reflection:" + url);
			imageReflection = texture;
			//renderer.material = Camera.main.GetComponent<ACPTrackingManager>().materialUIShiny;
			//renderer.material.mainTexture = image;
			//renderer.material.SetTexture("_MaskTex", imageReflection);

		}
	}
	
	
	public void ObjLoadFinished (OBJ obj)
	{
		foreach (EffectData effect in data.appearEffects) {
			effect.RunEffect (this.transform.GetChild (0).gameObject);
		}
	}
	
	// Update is called once per frame
	public void Update ()
	{
//		if (!hasSetBounds)
//		{
//			Debug.Log("has not set bounds...");
//			if (model.renderer != null)
//			{
//				Debug.Log("Bounds: " + model.renderer.bounds);
//				Vector3 extents = model.renderer.bounds.extents;
//				
//				float scale = 1.0f;
//				if (extents.x > extents.y)
//				{
//					scale = 1.0f/extents.x;
//				}
//				else
//				{
//					scale = 1.0f/extents.y;
//				}
//				model.transform.localScale = new Vector3(scale, scale, scale);
//				
//				hasSetBounds = true;
//			}
//		}
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
				effect.RunEffect(this.transform.GetChild(0).gameObject);
			}
			
			TimedObjectDestructor dest = gameObject.AddComponent<TimedObjectDestructor>();
			dest.timeOut = EffectData.FindLongestEffect(data.disappearEffects);
			Debug.Log("Destroying in " + dest.timeOut);
		}
		else
		{
			Destroy(gameObject);
		}
	}
	
}

