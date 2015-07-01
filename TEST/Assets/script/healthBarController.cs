using UnityEngine;
using System.Collections;

public class healthBarController : MonoBehaviour {
	float initScaleX;
	bool init;
	//public float rate=1.0f;//rate= m_life/maxLife
	// Use this for initialization
	void Start () {
		init = false;
	}

	public void UpdateHealthBar (float r) {
		if (!init) {
			initScaleX=this.transform.localScale.x;
			init=true;
		}
		if (r > 0) {
			this.transform.localScale = new Vector3 (initScaleX * r, 1, 1);
		}
	}
}
