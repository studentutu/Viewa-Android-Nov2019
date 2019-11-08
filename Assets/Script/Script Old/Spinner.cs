using UnityEngine;
using System.Collections;

public class Spinner : MonoBehaviour {

	public Color[] colors;
	public bool finishedAppearing = false;
	
	public GameObject blockRoot;
	public GameObject viewaLogo;
	
	public bool isRemoving = false;
	
	public bool useColors = false;
	public float progress = 0;
	
	public float Progress 
	{
		get { return progress; }
		set {
			progress = Mathf.Clamp (value, 0.0f, 1.0f);
//			Debug.Log ("Value :"+ value);
//			Debug.Log ("Progress :"+ progress);
			int max = (int)Mathf.Round (progress * (float)blockRoot.transform.childCount);
			Debug.Log ("Max :"+ max);
			for (int i = 0; i < max; i++) {
				Transform t = blockRoot.transform.GetChild (i);
				if (useColors) {
					t.gameObject.GetComponent<Renderer>().material.color = colors[i];
				} else {
					t.gameObject.GetComponent<Renderer>().material.color = Color.white;
				}
			}
		}
	}


	// Use this for initializatio
	IEnumerator Start ()
	{
		
		iTween.RotateBy (blockRoot, iTween.Hash ("y", 1, "time", 5f, "loopType", "loop", "easeType", "linear"));
		
		//MeshRenderer[] blocks = GetComponentsInChildren<MeshRenderer>();
		for (int i = 0; i < blockRoot.transform.childCount; i++)
		{
			Transform t = blockRoot.transform.GetChild (i);
			//t.gameObject.renderer.material.color = colors[i];
			//t.gameObject.renderer.material.SetColor("_Color", colors[i]);
						/*
			Color[] ca = t.gameObject.GetComponent<ParticleAnimator>().colorAnimation;
			for (int c = 0; c < ca.Length; c++)
			{
				float alpha = ca[c].a;
				ca[c] = colors[i];
				ca[c].a = alpha;
			}
			t.gameObject.GetComponent<ParticleAnimator>().colorAnimation = ca;
			*/
			
			Vector3 pos = t.localPosition;
			pos.y += (float)i * 0.005f;
			t.localPosition = pos;

			
			iTween.ScaleFrom(t.gameObject,iTween.Hash("scale",new Vector3(0,0,0),"time",.5f,"delay",0.1*(i+1),"isLocal",true));
		}
		
		viewaLogo.GetComponent<Renderer>().material.color = new Color32(255, 255, 255, 0);
		iTween.ColorTo(viewaLogo, iTween.Hash("color", new Color(1.0f, 1.0f, 1.0f, 1.0f), "time", 0.5f));
		iTween.PunchScale(viewaLogo, iTween.Hash("amount", new Vector3(2.5f, 2.5f, 2.5f), "time", 1.5f));
		
		yield return new WaitForSeconds(0.5f + (transform.childCount * 0.1f));
		finishedAppearing = true;
		
		int index = 7;
		while(true){
			
			Transform t = blockRoot.transform.GetChild(index);
			
			iTween.PunchRotation(t.gameObject,iTween.Hash("y", 20, "time", 1.24, "isLocal", true));
			yield return new WaitForSeconds(0.2f);
			
			index--;
			if(index<0)index=7;
		}
		
	}
	
	public float Remove () 
	{
		isRemoving = true;
		for (int i = 0; i < blockRoot.transform.childCount; i++)
		{
			Transform t = blockRoot.transform.GetChild(i);
			iTween.ScaleTo(t.gameObject,iTween.Hash("scale",new Vector3(0,0,0),"time",.5f,"delay",0.1*(i+1),"isLocal",true));
		}
		iTween.ColorTo(viewaLogo, iTween.Hash("color", new Color(1.0f, 1.0f, 1.0f, 0.0f), "time", 0.5f));

		TimedObjectDestructor dest = this.gameObject.AddComponent<TimedObjectDestructor>();
		dest.timeOut = 0.0f + (0.1f*transform.childCount);
		return dest.timeOut / 1.0f;
	}
}
