using UnityEngine;
using System.Collections;

public class RecordButtonController : MonoBehaviour {
	SpriteRenderer spriteRenderer;
	public Sprite[] btns;
	bool clicked=false;
	// Use this for initialization
	void Start () {
		spriteRenderer = gameObject.GetComponent<SpriteRenderer> ();
		spriteRenderer.sprite=btns[0];
		clicked = false;

		if (Settings.Instance.replay) {
			this.gameObject.SetActive(false);
		}
	}
	
	// Update is called once per frame
	void Update () {
		ClickItem ();
	}

	void ClickItem(){
		if (Input.GetMouseButtonDown (0)) {
			Vector3 v3 = Camera.main.ScreenToWorldPoint (Input.mousePosition); 
			Vector2 v2 = new Vector2 (v3.x, v3.y);
			RaycastHit2D hit = Physics2D.Raycast (v2, Vector2.zero);
			if (hit.collider != null) {
				//Debug.Log(hit.collider.tag);
				//create and write the data file
				if (hit.collider.gameObject == this.gameObject) {
					if(!clicked){
						RecordController.Instance.WriteData();
						RecordController.Instance.WriteOperations();
						spriteRenderer.sprite=btns[1];
						clicked=true;
						this.gameObject.SetActive(false);
					}
				}
			}else{
				//Debug.LogError("Hit Nothing!");
			}
		}
	}
}
