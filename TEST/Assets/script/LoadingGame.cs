using UnityEngine;
using System.Collections;
using System.IO;
public class LoadingGame : MonoBehaviour {
	public GameObject[] maps;
	public GameObject[] entrances;
	public GameObject[] exits;
	public GameObject[] GameManagers;
	public GameObject[] EnemySpawners;
	public GameObject[] NewTowerButtons;

	GameObject map;
	GameObject entrance;
	GameObject exit;
	GameObject gameManager;
	GameObject enemySpawner;
	GameObject towerButtons;
	//Start Point
	PathNode startNode;

	//All for Replay
	bool replay=false;
	ArrayList lineList;
	int index=5;

	string line="";
	string type="";
	float PosX=0.0f;
	float PosY=0.0f;
	float time=0.0f;
	GameObject[] towers;
	public GameObject[] towers1;
	public GameObject[] towers2;
	public GameObject[] towers3;
	public AudioClip placing;

	// Use this for initialization
	void Awake () {
		//Debug.Log("Awake LoadingGame");
		//map
		map = (GameObject)Instantiate (maps [Settings.Instance.themeID]);
		map.transform.parent = this.transform;

		//entrance
		entrance = (GameObject)Instantiate (entrances [Settings.Instance.themeID]);
		entrance.transform.parent = this.transform;

		//exit
		exit = (GameObject)Instantiate (exits [Settings.Instance.themeID]);
		exit.transform.parent = this.transform;

		//gamemanager
		gameManager = (GameObject)Instantiate (GameManagers [Settings.Instance.towerID]);
		gameManager.transform.parent = this.transform;
		GameManager gmController = gameManager.GetComponent<GameManager> ();
		//give parameters to GameManager
		gmController.PointIni = Settings.Instance.initMoney;
		gmController.LifeIni = Settings.Instance.initLife;
		gmController.LowMarks = Settings.Instance.lowMarks;
		gmController.mediumMarks = Settings.Instance.mediumMarks;
		gmController.highMarks = Settings.Instance.highMarks;

		//enemySpawner
		enemySpawner = (GameObject)Instantiate (EnemySpawners [Settings.Instance.enemyID]);
		enemySpawner.transform.parent = this.transform;
		startNode = this.transform.FindChild("Path").FindChild("start").gameObject.GetComponent<PathNode> ();
		enemySpawner.transform.position = startNode.transform.position;
		EnemySpawner spawnerController=enemySpawner.GetComponent<EnemySpawner> ();
		//Assign different parameters of enemySpawner
		spawnerController.m_startNode = startNode;
		spawnerController.xmldata = (TextAsset)Resources.Load ("XML/"+Application.loadedLevelName);
		if (spawnerController.xmldata == null) {
			Debug.LogError("Load XML fail!");
		}

		//NewTowerButtons
		if(!Settings.Instance.replay){
			if(Settings.Instance.interID==0){
				towerButtons = (GameObject)Instantiate (NewTowerButtons [Settings.Instance.towerID]);
				towerButtons.transform.parent=this.transform;
				if(Application.loadedLevelName.CompareTo("level2")==0){
					towerButtons.transform.position=new Vector3(1.81f,5f,0f);
				}
			}
		}
	}

	void Start(){
		replay = Settings.Instance.replay;
		index = 5;
		if (replay) {
			if(Settings.Instance.towerID==0){
				towers=towers1;
			}else if(Settings.Instance.towerID==1){
				towers=towers2;
			}else if(Settings.Instance.towerID==2){
				towers=towers3;
			}
			SetTowersValue();
			ReadFile(Settings.Instance.path);
			GetInfo();
		}

	}

	void FixedUpdate(){
		if (replay) {
			if(RecordController.Instance.timer > time-Time.fixedDeltaTime/2){
				//Do the operations
				Operation();
				//Debug.Log(RecordController.Instance.timer);
				index++;
				if(index >= lineList.Count){
					replay=false;
				}else{
					GetInfo();
				}
			}
		}
	}
	//Read the record file
	void ReadFile(string path){
		StreamReader sr =null;
		try{
			sr=File.OpenText(path);
		}catch(IOException e){
			Debug.Log(e);
			return;
		}
		string line;
		lineList = new ArrayList ();
		while ((line=sr.ReadLine())!=null) {
			lineList.Add(line);
		}
		sr.Close ();
	}
	//Get the info at number index line of the operation file
	void GetInfo(){
		line=(string)lineList[index];
		string[] info=line.Split(',');
		type = info [0].Split (':') [1];
		PosX = float.Parse (info [1].Split (':') [1]);
		PosY = float.Parse (info [2].Split (':') [1]);
		time = float.Parse (info [3].Split (':') [1]);
	}
	//Set the parameters of towers from Setttings.txt for the replay function
	void SetTowersValue(){
		foreach (GameObject item in towers) {
			DefenderController towerController=item.GetComponent<DefenderController>();
			int index = 0;
			switch (towerController.m_towerType) {
			case DefenderController.TowerType.LightTower:
				index=0;
				break;
			case DefenderController.TowerType.HeavyTower:
				index=1;
				break;
			case DefenderController.TowerType.PoisonTower:
				index=2;
				break;
			case DefenderController.TowerType.IceTower:
				index=3;
				break;
			case DefenderController.TowerType.ElecTower:
				index=4;
				break;
			}
			
			//set the numbers of tower
			towerController.powerLow=((Settings.TowerInfo)Settings.Instance.towersInfo[index]).power[0];
			towerController.powerMid=((Settings.TowerInfo)Settings.Instance.towersInfo[index]).power[1];
			towerController.powerHigh=((Settings.TowerInfo)Settings.Instance.towersInfo[index]).power[2];
			towerController.powerSuper=((Settings.TowerInfo)Settings.Instance.towersInfo[index]).power[3];
			towerController.attackAreaLow=((Settings.TowerInfo)Settings.Instance.towersInfo[index]).attackArea[0];
			towerController.attackAreaMid=((Settings.TowerInfo)Settings.Instance.towersInfo[index]).attackArea[1];
			towerController.attackAreaHigh=((Settings.TowerInfo)Settings.Instance.towersInfo[index]).attackArea[2];
			towerController.attackAreaSuper=((Settings.TowerInfo)Settings.Instance.towersInfo[index]).attackArea[3];
			towerController.attackTimeLow=((Settings.TowerInfo)Settings.Instance.towersInfo[index]).attackTime[0];
			towerController.attackTimeMid=((Settings.TowerInfo)Settings.Instance.towersInfo[index]).attackTime[1];
			towerController.attackTimeHigh=((Settings.TowerInfo)Settings.Instance.towersInfo[index]).attackTime[2];
			towerController.attackTimeSuper=((Settings.TowerInfo)Settings.Instance.towersInfo[index]).attackTime[3];
			towerController.costLow=((Settings.TowerInfo)Settings.Instance.towersInfo[index]).cost[0];
			towerController.costMid=((Settings.TowerInfo)Settings.Instance.towersInfo[index]).cost[1];
			towerController.costHigh=((Settings.TowerInfo)Settings.Instance.towersInfo[index]).cost[2];
			towerController.costSuper=((Settings.TowerInfo)Settings.Instance.towersInfo[index]).cost[3];
		}
	}
	//Do the operation
	void Operation(){
		//Debug.Log(line);
		if (type == "LightTower") {
			GameObject m_tower = (GameObject)Instantiate (towers[0], new Vector3(PosX,PosY,0f), Quaternion.identity);
			m_tower.transform.parent=GameObject.Find("Game").transform;
			m_tower.transform.localScale = new Vector3 (1, 1, 1);
			DefenderController towerController = m_tower.GetComponent<DefenderController> ();
			towerController.m_cost = towerController.costLow;
			GameManager.Instance.SetPoint (-towerController.costLow);
			if(Settings.Instance.sound){
				AudioSource.PlayClipAtPoint(placing,Camera.main.transform.position,1.0f);
			}
		}else if(type == "HeavyTower"){
			GameObject m_tower = (GameObject)Instantiate (towers[1], new Vector3(PosX,PosY,0f), Quaternion.identity);
			m_tower.transform.parent=GameObject.Find("Game").transform;
			m_tower.transform.localScale = new Vector3 (1, 1, 1);
			DefenderController towerController = m_tower.GetComponent<DefenderController> ();
			towerController.m_cost = towerController.costLow;
			GameManager.Instance.SetPoint (-towerController.costLow);
			if(Settings.Instance.sound){
				AudioSource.PlayClipAtPoint(placing,Camera.main.transform.position,1.0f);
			}
		}else if(type == "PoisonTower"){
			GameObject m_tower = (GameObject)Instantiate (towers[2], new Vector3(PosX,PosY,0f), Quaternion.identity);
			m_tower.transform.parent=GameObject.Find("Game").transform;
			m_tower.transform.localScale = new Vector3 (1, 1, 1);
			DefenderController towerController = m_tower.GetComponent<DefenderController> ();
			towerController.m_cost = towerController.costLow;
			GameManager.Instance.SetPoint (-towerController.costLow);
			if(Settings.Instance.sound){
				AudioSource.PlayClipAtPoint(placing,Camera.main.transform.position,1.0f);
			}
		}else if(type == "IceTower"){
			GameObject m_tower = (GameObject)Instantiate (towers[3], new Vector3(PosX,PosY,0f), Quaternion.identity);
			m_tower.transform.parent=GameObject.Find("Game").transform;
			m_tower.transform.localScale = new Vector3 (1, 1, 1);
			DefenderController towerController = m_tower.GetComponent<DefenderController> ();
			towerController.m_cost = towerController.costLow;
			GameManager.Instance.SetPoint (-towerController.costLow);
			if(Settings.Instance.sound){
				AudioSource.PlayClipAtPoint(placing,Camera.main.transform.position,1.0f);
			}
		}else if(type == "ElecTower"){
			GameObject m_tower = (GameObject)Instantiate (towers[4], new Vector3(PosX,PosY,0f), Quaternion.identity);
			m_tower.transform.parent=GameObject.Find("Game").transform;
			m_tower.transform.localScale = new Vector3 (1, 1, 1);
			DefenderController towerController = m_tower.GetComponent<DefenderController> ();
			towerController.m_cost = towerController.costLow;
			GameManager.Instance.SetPoint (-towerController.costLow);
			if(Settings.Instance.sound){
				AudioSource.PlayClipAtPoint(placing,Camera.main.transform.position,1.0f);
			}
		}else if(type=="Upgrade"){
			Vector2 v2=new Vector2(PosX,PosY);
			RaycastHit2D hit=Physics2D.Raycast(v2,Vector2.zero);
			if(hit.collider!=null)
			{
				//Debug.Log(hit.collider.tag);
				if(hit.collider.tag.CompareTo("tower")==0){
					GameObject tower=hit.collider.gameObject;
					DefenderController towerController=tower.GetComponent<DefenderController>();
					towerController.UpdateTower();
				}
			}
		}else if(type=="Sell"){
			Vector2 v2=new Vector2(PosX,PosY);
			RaycastHit2D hit=Physics2D.Raycast(v2,Vector2.zero);
			if(hit.collider!=null)
			{
				//Debug.Log(hit.collider.tag);
				if(hit.collider.tag.CompareTo("tower")==0){
					GameObject tower=hit.collider.gameObject;
					DefenderController towerController=tower.GetComponent<DefenderController>();
					towerController.RemoveTower();
				}
			}
		}else if(type=="Enemy"){
			Vector2 v2=new Vector2(PosX,PosY);
			RaycastHit2D hit=Physics2D.Raycast(v2,Vector2.zero);
			if(hit.collider!=null)
			{
				//Debug.Log(hit.collider.tag);
				if(hit.collider.tag.CompareTo("enemy")==0){
					//selected enemy
					GameObject enemy=hit.collider.gameObject;
					EnemyController enemyController=enemy.GetComponent<EnemyController>();
					enemyController.EnemyOnClick();
				}
			}
		}
	}
}
