using UnityEngine;
using System.Collections;

public class playButtonController : MonoBehaviour {
	public enum ButtonType
	{
		Play,
		Replay,
		Settings,
		Back,
		Sound,
		Music,
		Confirm,
		Quit,
		NextLevel,
	}
	public ButtonType type;
	public Sprite[] buttons;
	SpriteRenderer spriteRenderer;
	bool play=true;

	public enum BoardType{
		Game,
		Over,
	}
	public BoardType occation=BoardType.Game;
	// Use this for initialization
	void Start () {
		spriteRenderer = gameObject.GetComponent<SpriteRenderer> ();
		play = true;
		Time.timeScale = 1;
		if (type == ButtonType.Play) {
			PlaySprite();
		}else if(type==ButtonType.Settings){
			spriteRenderer.sprite=buttons[0];
		}else if(type==ButtonType.Sound){
			SoundSprite();
		}else if(type==ButtonType.Music){
			MusicSprite();
		}
		//hide NextLevel Button when replay
		if (Settings.Instance.replay) {
			if(type==ButtonType.NextLevel){
				this.gameObject.SetActive(false);
			}
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
				if (hit.collider.gameObject == this.gameObject) {
					if(type==ButtonType.Play){
						StartorPause();
					}else if(type==ButtonType.Replay){
						Replay();
					}else if(type==ButtonType.Settings){
						ShowBoard();
					}else if(type==ButtonType.Back){
						BackToGame();
					}else if(type==ButtonType.Sound){
						SoundChange();
					}else if(type==ButtonType.Music){
						MusicChange();
					}
					else if(type==ButtonType.Confirm){
						Confirm();
					}else if(type==ButtonType.Quit){
						Quit();
					}else if(type==ButtonType.NextLevel){
						GoToNext();
					}

				}
			}else{
				//Debug.LogError("Click nothing");
			}
		}
	}

	void StartorPause(){
		if(play){
			play = false;
			spriteRenderer.sprite=buttons[1];
			Time.timeScale = 0;
			//Debug.Log("Pause Game");
		}else{
			play=true;
			spriteRenderer.sprite=buttons[0];
			Time.timeScale=1;
		}
	}

	void PlaySprite(){
		if (play) {
			spriteRenderer.sprite = buttons [0];
		} else {
			spriteRenderer.sprite = buttons [1];
		}
	}

	void Replay(){
		Time.timeScale=1;
		Application.LoadLevel (Application.loadedLevel);
		if (occation == BoardType.Over) {
			Destroy(this.transform.parent.gameObject);
		}
	}

	void ShowBoard(){
		spriteRenderer.sprite = buttons [1];
		//suspend the game
		Time.timeScale = 0;
		transform.FindChild ("setBoard").gameObject.SetActive(true);
	}

	void BackToGame(){
		playButtonController settings = transform.parent.parent.gameObject.GetComponent<playButtonController>();
		settings.spriteRenderer.sprite = settings.buttons [0];
		//play the game
		Time.timeScale = 1;
		transform.parent.gameObject.SetActive (false);
	}

	void SoundSprite(){
		if (Settings.Instance.sound) {
			spriteRenderer.sprite=buttons[0];
		}else{
			spriteRenderer.sprite=buttons[1];
		}
	}
	void SoundChange(){
		if (Settings.Instance.sound) {
			spriteRenderer.sprite=buttons[1];
			Settings.Instance.sound=false;
			PlayerPrefs.SetInt("sound",0);
		}else{
			spriteRenderer.sprite=buttons[0];
			Settings.Instance.sound=true;
			PlayerPrefs.SetInt("sound",1);
		}
	}

	void MusicSprite(){
		if (Settings.Instance.music) {
			spriteRenderer.sprite=buttons[0];
		}else{
			spriteRenderer.sprite=buttons[1];
		}
	}
	void MusicChange(){
		if (Settings.Instance.music) {
			spriteRenderer.sprite=buttons[1];
			Settings.Instance.music=false;
			PlayerPrefs.SetInt("music",0);
		}else{
			spriteRenderer.sprite=buttons[0];
			Settings.Instance.music=true;
			PlayerPrefs.SetInt("music",1);
		}
	}

	void Confirm(){
		BackToGame ();
	}

	void Quit(){
		//Go to the start scene
		Application.LoadLevel (0);
	}

	void GoToNext(){
		int loadlevel = Application.loadedLevel + 1;
		Debug.Log (loadlevel);
		Application.LoadLevel (loadlevel);
	}
}
