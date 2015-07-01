using UnityEngine;
using System.Collections;

public class bgController : MonoBehaviour {
	public int width=4;
	public int height=3;
	[HideInInspector]
	public float rate;
	public static bgController Instance;

	// Use this for initialization
	void Awake () {
		//Debug.Log("Awake bgController");
		Instance = this;

		rate = (Screen.width*height*1.0f)/(Screen.height*width);
		//Debug.Log ("rate:"+rate);
		Vector3 scale = transform.localScale;
		this.transform.localScale = new Vector3 (scale.x * rate, scale.y, scale.z);
		//Camera.main.aspect=temp;
	}
}
