using UnityEngine;
using System.Collections;
using ACP;

/// <summary>
/// ARGallery is an image with a thumbnail scrubber on the bottom
/// </summary>
public class ARGalleryBehavior : WidgetBehavior
{

	public ARGalleryData data;
	//private GameObject plane; //plane for just the image

	public GameObject imagePlane; //hidden plane for just the image
	public GameObject thumbBGPlane; //hidden plane for the thumbnail background
	public GameObject scrollView; //scrollview for thumbnails
	public GameObject grid; //grid under the scrollview

	private Texture2D currentImage;
	private string currentImageUrl;
	private JSONObject imagesJson; //array of the images - http://001.viewa.mocom.uat.visualjazz.com.au/54137
	private ArrayList thumbnailItems;
	float thumbWidth = 60;
	float thumbHeight = 34;
	Vector3 fullLocalAreaScale;
	private bool haveSetScrollToStart = false;
	private bool firstThumbLoaded = false;

	public override WidgetData Data { 
		get { return data; }
	}

	// Use this for initialization
	void Start ()
	{
		Debug.Log ("1. Start ARGalleryBehavoir! ");

		Color brightWhite = Color.white;
		brightWhite.a = 1f;
		brightWhite.r = 1f;
		brightWhite.g = 1f;
		brightWhite.b = 1f;
		Color brightRed = Color.red;
		brightRed.a = 0.5f;
		brightRed.r = 1f;


		// We should always have data here...
		if (data != null) {
		
			// Set up our properties from the JSON data...
			this.name = "ARGalleryBehavoir";

			//thumbWidth = data.size.x / 4f; //fit four thumbs
			thumbHeight = (data.size.z / 4f); //thumb height is 1/4 of the height of the main image
			thumbWidth = thumbHeight; //square thumbs to support all orientations

			fullLocalAreaScale = new Vector3 (-data.size.x / 10.0f, 1, -data.size.z / 10.0f);
			//plane.transform.localScale = new Vector3 (-data.size.x / 10.0f, 1, -data.size.z / 10.0f);
			transform.localPosition = data.position;
			transform.localRotation = Quaternion.Euler (data.rotation);

			foreach (EffectData effect in data.appearEffects) {
				effect.RunEffect (this.gameObject);
			}

			//create our sub planes - for the image itself and the thumbnail strip
			imagePlane.gameObject.layer = 8;
			imagePlane.GetComponent<Renderer>().material = Camera.main.GetComponent<ACPTrackingManager> ().materialUITransparent;
			imagePlane.GetComponent<Renderer>().material.color = brightWhite;
			imagePlane.transform.localScale = new Vector3(fullLocalAreaScale.x,fullLocalAreaScale.y, fullLocalAreaScale.z * 0.75f);
			imagePlane.transform.localPosition = new Vector3(0,0,0 - fullLocalAreaScale.z * 1.25f);
			imagePlane.SetActive(true); //hidden until the picture is loaded
			Texture2D image = Resources.Load("semitransparent", typeof(Texture2D)) as Texture2D;
			imagePlane.GetComponent<Renderer>().material.mainTexture = image;

			thumbBGPlane.gameObject.layer = 8;
			thumbBGPlane.GetComponent<Renderer>().material = Camera.main.GetComponent<ACPTrackingManager> ().materialUITransparent;
			thumbBGPlane.GetComponent<Renderer>().material.color = brightRed;
			thumbBGPlane.transform.localScale = new Vector3(fullLocalAreaScale.x,fullLocalAreaScale.y, fullLocalAreaScale.z * 0.25f);
			thumbBGPlane.transform.localPosition = new Vector3(0,0,imagePlane.transform.localPosition.z - data.size.z/2f);
			thumbBGPlane.transform.localRotation = transform.localRotation;
			thumbBGPlane.SetActive(false); //never show this, scrollview takes it absolute position

			//construct NGUI scroll grid view monstrosity based on the example in "Example 7 - ScrollView (Panel)" scene from NGUI
			scrollView.layer = 8;
			UIPanel panel = scrollView.AddComponent<UIPanel>() as UIPanel; //ngui script
			panel.clipping = UIDrawCall.Clipping.SoftClip;
			panel.clipSoftness = new Vector2(0,0);
			panel.baseClipRegion = new Vector4(0,0,data.size.x,thumbHeight);
			scrollView.AddComponent<UIScrollView>(); //ngui script

			grid = new GameObject("ARGallery Grid");
			grid.layer = 8;
			grid.transform.parent = scrollView.transform;
			//grid.AddComponent("UICenterOnChild"); //ngui script
			UIGrid uigrid = grid.AddComponent<UIGrid>() as UIGrid;//ngui script
			uigrid.cellWidth = thumbWidth;
			uigrid.cellHeight = thumbHeight;

			scrollView.transform.position = thumbBGPlane.transform.position;
			scrollView.transform.localRotation = Quaternion.identity;
			scrollView.transform.Rotate(90,0,0);

			StartCoroutine(loadGalleryJson());

		} else {
			Debug.LogError ("Danger Will Robinson! ARGalleryBehavoir started with no data!");
		}
		
		if (data.autoPlay) {
			//TODO auto play for AR Gallery could mean slide show?
		}
		Debug.Log ("2. Start ARGalleryBehavoir! ");

	}

	private void createThumbnailItem ()
	{
		Debug.Log ("Created Thumbnail Item");

		int i = thumbnailItems.Count;
		GameObject item = new GameObject("Item "+i);
		item.layer = 8;
		item.transform.parent = grid.transform;
		BoxCollider boxCollider = item.AddComponent<BoxCollider>() as BoxCollider;
		boxCollider.isTrigger = true;
		boxCollider.size = new Vector3(thumbWidth,thumbHeight,1);
		item.AddComponent<UIDragScrollView>();
		//item.AddComponent("UICenterOnClick");
		ARGalleryThumbnailItemClicker clicker = item.AddComponent<ARGalleryThumbnailItemClicker>() as ARGalleryThumbnailItemClicker;
		clicker.ItemPressed += ThumbnailClicked;
		clicker.index = i;
		clicker.name = "clicker";

		//Color brightRed = Color.red;
		//brightRed.a = 0.5f;
		//brightRed.r = 1f;
		UIPanel panel = item.AddComponent<UIPanel>() as UIPanel; //ngui script
		panel.clipping = UIDrawCall.Clipping.SoftClip;
		panel.baseClipRegion = new Vector4(0,0,thumbWidth-1,thumbHeight-1);
		panel.clipSoftness = new Vector2(0,0);
		panel.widgetsAreStatic = true;
		GameObject pic = new GameObject("UITexture");
		pic.layer = 8;
		pic.name = "pic";
		pic.transform.parent = item.transform;
		UITexture texture = pic.AddComponent<UITexture>() as UITexture;
		texture.color = Color.clear;
		texture.enabled = false;
		UIStretch stretch = pic.AddComponent<UIStretch>() as UIStretch;
		stretch.container = item;
		stretch.style = UIStretch.Style.FillKeepingRatio;
		stretch.runOnlyOnce = false;
		//stretch.borderPadding = new Vector2(1,1);

		item.transform.localPosition = new Vector3(i*(thumbWidth),0 ,0);
		item.transform.localRotation = Quaternion.identity;

		if(thumbnailItems == null){
			thumbnailItems = new ArrayList();
		}
		thumbnailItems.Add(item);
	}

	private IEnumerator loadGalleryJson ()
	{
		Debug.Log ("Loading Gallery Json");
		//download the images json
		WWW www = new WWW(data.galleryUrl);
		yield return www;
		if (www.error != null) {
			Debug.LogWarning ("ERROR Downloading AR Gallery gallery: "+data.galleryUrl);
		} else  {
			Debug.Log ("Received Gallery Json!");
			string jsonString = www.text;
			Debug.Log (jsonString);
			
			JSONObject jsonObject = new JSONObject (jsonString);
			if(jsonObject["images"]){
				imagesJson = jsonObject["images"];
				thumbnailItems = new ArrayList();

				DownloadCache.Instance.DownloadCompleteTexture += TextureLoaded; //assign the callback event handler
				Debug.Log ("About to create thumbnails");

				//create thumbnails
				for (int imageIndex = 0; imageIndex < imagesJson.Count; imageIndex++) {
					createThumbnailItem();
				}
				//start image fetching (queue these in reverse)
				for (int imageIndex = imagesJson.Count-1; imageIndex >= 0; imageIndex--) {
					string url = imagesJson[imageIndex]["image_url"].str;
					Debug.LogWarning ("IMAGE URL TO DOWNLOAD " + url);
					Texture2D image = GetTexture(url);
					if(image){
						Debug.Log ("Thumbnail for item "+ imageIndex+ " already cached ");
					} else {
						Debug.Log ("Thumbnail for item "+ imageIndex+ " queued for downloading ");
						DownloadCache.Instance.LoadFile(url, DownloadCache.DownloadType.TextureDownload);
					}
					updateThumbnailImage(imageIndex, image);
				}
				UIScrollView sv = scrollView.GetComponent<UIScrollView>();
				sv.scrollWheelFactor = 0;
				sv.restrictWithinPanel = true;
				sv.contentPivot = UIWidget.Pivot.Left;
				currentImageUrl = imagesJson[0]["image_url"].str;
				currentImage = GetTexture (currentImageUrl);
				updateCurrentImage();

			} else {
				Debug.Log ("invalid json - invalid images json received?");
			}
		}
	}

	private void setScrollToStart () {
		float offset = 0; 
		GameObject firstItem = thumbnailItems[0] as GameObject;
		Transform firstPicTransform = firstItem.transform.Find("pic") as Transform;
		UITexture firstTexture = firstPicTransform.GetComponent<UITexture>();
		if(firstTexture.width > firstTexture.height){
			//adjust for the weird mapping UITexture does that causes the scrollview origin to be off visually.
			offset = (firstTexture.width - firstTexture.height)/2; 
		}

		UIScrollView sv = scrollView.GetComponent<UIScrollView>();
		sv.ResetPosition();
		sv.UpdatePosition();
		sv.MoveAbsolute(new Vector3(0 - offset,0,0));
		Debug.Log("setting scroll to start");
		haveSetScrollToStart = true;

		foreach (GameObject item in thumbnailItems){
			Transform picTransform = item.transform.Find("pic") as Transform;
			UITexture texture = picTransform.GetComponent<UITexture>();
			texture.color = Color.white;
			texture.enabled = true;
		}
		//only enable scrolling if the thumbs wont fit
		float totalThumbWidth = thumbnailItems.Count*(thumbWidth);
		float scrollContentWidth = data.size.x;
		if(totalThumbWidth <= scrollContentWidth){
			UIScrollView scrollview = scrollView.GetComponent<UIScrollView>();
			scrollview.enabled = false;
		}
	}

	public void updateThumbnailImage(int imageIndex, Texture2D image)
	{
		if(image == null){			
			//set semi transparent placeholder
			image = Resources.Load("semitransparent", typeof(Texture2D)) as Texture2D;
		} else if(image && (imageIndex == 0)){
			firstThumbLoaded = true;
		}

		if(image!= null){
			image.wrapMode = TextureWrapMode.Clamp;
			GameObject item = thumbnailItems[imageIndex] as GameObject;			
			Transform picTransform = item.transform.Find("pic") as Transform;
			if(picTransform){
				UITexture texture = picTransform.GetComponent<UITexture>();
				texture.mainTexture = image;
				texture.enabled = true;
				UIStretch stretch = picTransform.GetComponent<UIStretch>();
				stretch.initialSize = new Vector2(image.width,image.height);
				stretch.borderPadding = new Vector2(0,0);
			}
		}
	}
	
	public void updateCurrentImage()
	{
		if (currentImage != null) {
			currentImage.wrapMode = TextureWrapMode.Clamp;
			imagePlane.GetComponent<Renderer>().material.mainTexture = currentImage;

			//our image should always be displayed in 16:9 - so if its NOT this ratio already we need to adjust the scale below accordingly
			// we may need to crop if necessary - depending if our frame area is landscape or portrait
			//Debug.Log ("Texture Size is "+currentImage.texelSize.x+"x"+currentImage.texelSize.y);
			float textureAspect = currentImage.texelSize.y / currentImage.texelSize.x;
			float areaWidth = data.size.x;
			float areaHeight = data.size.z * 0.75f;
			//Debug.Log ("AreaWidth "+areaWidth + " AreaHeight "+areaHeight);
			if(areaWidth > areaHeight){
				float areaAspect = areaWidth / areaHeight;
				float yScale = textureAspect / areaAspect;
				if(yScale > 1.0f){
					//shrink the plane down to match
					imagePlane.transform.localScale = new Vector3(imagePlane.transform.localScale.x,imagePlane.transform.localScale.y, fullLocalAreaScale.z*0.75f / yScale);
					yScale = 1.0f;
				}
				//float yOffset = yScale * currentImage.texelSize.y;
				//Debug.Log ("Texture y Scale is "+yScale + " y offset is "+yOffset);
				imagePlane.GetComponent<Renderer>().material.mainTextureScale = new Vector2 (1f, yScale);
				imagePlane.GetComponent<Renderer>().material.mainTextureOffset = new Vector2 (0f, (1.0f-(yScale))/2f);
			} else {
				//TODO portrait pictures
			}
			imagePlane.SetActive(true);
		} else {
			DownloadCache.Instance.LoadFile(currentImageUrl, DownloadCache.DownloadType.TextureDownload);
		}
	}

	public void ThumbnailClicked(ARGalleryThumbnailItemClicker sender)
	{
		//thumbnail was clicked - this will auto center but we need to change
		currentImageUrl = imagesJson[sender.index]["image_url"].str;
		currentImage = GetTexture (currentImageUrl);
		updateCurrentImage();
	}

	public void TextureLoaded(string url, Texture2D texture)
	{
		//Debug.Log("ARGalleryBehavior::TextureLoaded(" + url + "," + texture + ")");
		if (url == currentImageUrl)
		{
			currentImage = texture;
			updateCurrentImage();
		}
		for (int imageIndex = 0; imageIndex < imagesJson.Count; imageIndex++) {
			string thumbUrl = imagesJson[imageIndex]["image_url"].str;
			if(url == thumbUrl){
				updateThumbnailImage(imageIndex, texture);
			}
		}
	}

	public void Update ()
	{
		//hack fix to prevent scrollview vertical movement (which can occur despite the appropriate UIScrollView property being set
		scrollView.transform.localPosition = new Vector3(scrollView.transform.localPosition.x,0,scrollView.transform.localPosition.z);
		UIPanel panel = scrollView.GetComponent<UIPanel>();
		panel.clipOffset = new Vector2(panel.clipOffset.x,0);

		if(!haveSetScrollToStart && firstThumbLoaded){
			setScrollToStart();
		}

		if(Input.GetKeyDown("c")){
			setScrollToStart();
		}
	}

	public void OnDestroy ()
	{
		DownloadCache.Instance.DownloadCompleteTexture -= TextureLoaded;
		Destroy(scrollView);
	}
	
	public void OnHover(bool released)
	{
		// Buttony type things ..		
	}
	
	public override void Remove()
	{
		// On remove, run all of our effects then go away...
		if (data.disappearEffects.Count > 0)
		{
			foreach (EffectData effect in data.disappearEffects) {
				effect.RunEffect(this.gameObject);
			}
			
			TimedObjectDestructor dest = gameObject.AddComponent<TimedObjectDestructor>();
			dest.timeOut = EffectData.FindLongestEffect(data.disappearEffects);
		} else {
			Destroy(gameObject);
		}
	}
	
}

