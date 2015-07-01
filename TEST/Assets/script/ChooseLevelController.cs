using UnityEngine;
using System.Collections;

public class ChooseLevelController : MonoBehaviour {
	public int ID=0;
	SpriteRenderer spriteRenderer;
	public Sprite[] buttons;
	// Use this for initialization
	void Start () {
		spriteRenderer = gameObject.GetComponent<SpriteRenderer> ();
		spriteRenderer.sprite = buttons [0];
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetMouseButtonDown (0)) {
			Vector3 v3 = Camera.main.ScreenToWorldPoint (Input.mousePosition); 
			Vector2 v2 = new Vector2 (v3.x, v3.y);
			RaycastHit2D hit = Physics2D.Raycast (v2, Vector2.zero);
			if (hit.collider != null) {
				//Debug.Log(hit.collider.tag);
				if (hit.collider.gameObject == this.gameObject) {
					spriteRenderer.sprite = buttons [1];
					string levelName="level"+ID;
					Application.LoadLevel(levelName);
				}
			}
		}
	}
}
