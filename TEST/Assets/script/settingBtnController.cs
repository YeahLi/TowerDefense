using UnityEngine;
using System.Collections;
using System.IO;
public class settingBtnController : MonoBehaviour {
	public enum BtnType{
		Enemy,
		Tower,
		Theme,
		Settings,
		Start,
		Quit,
		Confirm,
		Replay,
		Play,
		MainMenu,
		Interaction,
	}
	SpriteRenderer spriteRenderer;
	public BtnType type=BtnType.Enemy;
	public Sprite[] btns;
	public bool selected = false;
	public int ID=0;

	// Use this for initialization
	void Start () {
		spriteRenderer = gameObject.GetComponent<SpriteRenderer> ();

		if (type == BtnType.Interaction) {
			if(ID==Settings.Instance.interID){
				selected=true;
			}else{
				selected=false;
			}
		}

		if (selected) {
			spriteRenderer.sprite=btns[1];
		}else{
			spriteRenderer.sprite=btns[0];
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
					if(type!=BtnType.Settings){
						selected=true;
						spriteRenderer.sprite=btns[1];
					}
					switch(type){
					case BtnType.Enemy:
						Settings.Instance.enemyID=ID;
						//Debug.Log("enemyID:"+Settings.Instance.enemyID);
						break;
					case BtnType.Tower:
						Settings.Instance.towerID=ID;
						//Debug.Log("towerID:"+Settings.Instance.towerID);
						break;
					case BtnType.Theme:
						Settings.Instance.themeID=ID;
						//Debug.Log("themeID:"+Settings.Instance.themeID);
						break;
					case BtnType.Settings:
						Application.LoadLevel("SettingsMenu");
						break;
					case BtnType.Start:
//						Debug.Log("enemyID:"+Settings.Instance.enemyID+"\n"+
//						          "towerID:"+Settings.Instance.towerID+"\n"+
//						          "themeID:"+Settings.Instance.themeID);
						//Debug.Log(PlayerInfoController.Instance.playerName);
						Settings.Instance.playerName=PlayerInfoController.Instance.playerName;
						Settings.Instance.testTime=PlayerInfoController.Instance.testTime;

						Application.LoadLevel("ChooseLevel");
						break;
					case BtnType.Quit:
						PlayerPrefs.DeleteAll();
						Application.Quit();
						break;
					case BtnType.Confirm:
						ChangeResolution();

						PlayerPrefs.SetInt("SetInteraction",2222);
						PlayerPrefs.SetInt("InterID",Settings.Instance.interID);

						Application.LoadLevel("MainMenu");
						break;
					case BtnType.Replay:
						Application.LoadLevel("ReplayMenu");
						break;
					case BtnType.MainMenu:
						Application.LoadLevel("MainMenu");
						break;
					case BtnType.Play:
						LoadReplay();
						break;
					case BtnType.Interaction:
						Settings.Instance.interID=ID;
						break;
					}
				}else{
					if(hit.collider.tag==this.tag){
						selected=false;
						spriteRenderer.sprite=btns[0];
					}
				}
			}else{
				//Debug.LogError("Hit Nothing!");
			}
		}
		if (Input.GetMouseButtonUp (0)) {
			if(type==BtnType.Start||type==BtnType.Confirm || type==BtnType.Replay || type==BtnType.Play){
				selected=false;
				spriteRenderer.sprite=btns[0];
			}
		}
	}

	void ChangeResolution(){
		UpgradeLableController lable = GameObject.Find ("resolutionlable").GetComponent<UpgradeLableController> ();
		string[] strArray = lable.text.Split ('x');
		int width = int.Parse (strArray[0]);
		int height = int.Parse (strArray[1]);
		Settings.Instance.width = width;
		Settings.Instance.height = height;
		PlayerPrefs.SetInt ("setResolution",1111);
		PlayerPrefs.SetInt ("width",width);
		PlayerPrefs.SetInt ("height", height);
		//Debug.Log (width + "x" + height);
		Screen.SetResolution (width,height,false);
	}

	void LoadReplay(){
		//read the file
		UpgradeLableController ReplayNameLable=GameObject.Find("ReplayName").gameObject.GetComponent<UpgradeLableController>();
		string file = ReplayNameLable.text;
		string path = Path.Combine (Application.persistentDataPath,file);
		Settings.Instance.path = path;
		StreamReader sr =null;
		try{
			sr=File.OpenText(path);
		}catch(IOException e){
			Debug.Log(e);
			return;
		}
		string line;
		ArrayList lineList = new ArrayList ();
		while ((line=sr.ReadLine())!=null) {
			lineList.Add(line);
		}
		sr.Close ();
		//handle with the information
		string loadlevel =(string) lineList [1];
		string levelname = loadlevel.Split (':')[1];
		//Debug.Log (levelname);
		string settings = (string)lineList [3];
		string[] setAttrs =settings.Split(',');
		int width = int.Parse (setAttrs [0].Split (':') [1]);
		//Debug.Log (width);
		int height = int.Parse (setAttrs [1].Split (':') [1]);
		//Debug.Log (height);
		int EnemyID = int.Parse (setAttrs [2].Split (':') [1]);
		//Debug.Log (EnemyID); 
		int TowerID = int.Parse (setAttrs [3].Split (':') [1]);
		//Debug.Log (TowerID);
		int ThemeID = int.Parse (setAttrs [4].Split (':') [1]);
		//Debug.Log (ThemeID);
		bool music = bool.Parse (setAttrs [5].Split (':') [1]);
		//Debug.Log (music);
		bool sound = bool.Parse (setAttrs [6].Split (':') [1]);
		//Debug.Log (sound);
		PlayerPrefs.SetInt ("width", width);
		Settings.Instance.width = width;
		PlayerPrefs.SetInt ("height", height);
		Settings.Instance.height = height;
		Settings.Instance.enemyID = EnemyID;
		Settings.Instance.towerID = TowerID;
		Settings.Instance.themeID = ThemeID;
		if (music) {
			PlayerPrefs.SetInt("music",1);
		}else{
			PlayerPrefs.SetInt("music",0);
		}
		Settings.Instance.music = music;
		if (sound) {
			PlayerPrefs.SetInt("sound",1);
		}else{
			PlayerPrefs.SetInt("sound",0);
		}
		Settings.Instance.sound = sound;
		Settings.Instance.replay = true;
		Screen.SetResolution (width,height,false);
		Application.LoadLevel(levelname);
	}
}
