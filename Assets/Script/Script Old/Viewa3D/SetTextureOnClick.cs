using UnityEngine;
using System.Collections;

namespace Viewa3D
{
	public class SetTextureOnClick : MonoBehaviour {

		public Renderer targetRenderer;
		public Texture texture;

		public void OnClick() {
			targetRenderer.material.mainTexture = texture;
		}
	}
}