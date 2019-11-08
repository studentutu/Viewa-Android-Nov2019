
using UnityEngine;

[AddComponentMenu("NGUI/Interaction/Center Scroll View on Click")]
public class ARGalleryThumbnailItemClicker : MonoBehaviour
{
	public int index;
	public delegate void ARGalleryThumbnailItemEventHandler(ARGalleryThumbnailItemClicker sender);
	public event ARGalleryThumbnailItemEventHandler ItemPressed;

	void OnClick ()
	{
		//Debug.Log ("ARGallery Pressed");
		ItemPressed (this);
	}
}
