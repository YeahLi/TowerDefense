using UnityEngine;
using System.Collections;

public class UpgradeLableController : MonoBehaviour {
	TextMesh textMesh;
	public string text="999";
	// Use this for initialization
	void Awake () {
		textMesh = this.gameObject.GetComponent<TextMesh> ();
		textMesh.text = text;
	}
	
	// Update is called once per frame
	void Update () {
		textMesh.text = text;
	}
}
