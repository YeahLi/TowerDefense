using UnityEngine;
using System.Collections;
using System.IO;
public class RecordController : MonoBehaviour {
	public enum OperationType{
		Upgrade,
		Sell,
		LightTower,
		HeavyTower,
		PoisonTower,
		IceTower,
		ElecTower,
		Enemy,
	}
	[System.Serializable]
	public class Operation
	{
		public OperationType type;
		public float posX;
		public float posY;
		public float time;
	}
	public static RecordController Instance;
	public EnemySpawner enemySpawner;
	public ArrayList ops;
	public float timer=0;
	// Use this for initialization
	void Awake () {
		Instance = this;
	}
	void Start(){
		ops = new ArrayList ();
		timer = 0f;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		timer += Time.fixedDeltaTime;
	}

	public void WriteData(){
		//string date = System.DateTime.Now.ToString ("yyyyMMddHHmmss");
		//Debug.Log (date);
		//string path1 = Path.Combine (Application.persistentDataPath, "Data");//We had better not use the custom folder!!!!!!
		string path = Path.Combine (Application.persistentDataPath, 
		                            Settings.Instance.playerName+Settings.Instance.testTime+".txt");//System.IO.Directory.GetCurrentDirectory ()
		Debug.Log ("Path:" + path);
		StreamWriter write;
		FileInfo t = new FileInfo (path);
		if (!t.Exists) {
			write=t.CreateText();
		}else{
			t.Delete();
			write=t.CreateText();
		}
		write.WriteLine ("LoadedLevel:" + Application.loadedLevelName);
		write.WriteLine ("Settings:");
		write.WriteLine ("width:"+Settings.Instance.width+",height:"+Settings.Instance.height
		                 +",EnemyID:"+Settings.Instance.enemyID+",TowerID:"+Settings.Instance.towerID
		                 +",ThemeID:"+Settings.Instance.themeID+",music:"+Settings.Instance.music
		                 +",sound:"+Settings.Instance.sound);
		write.WriteLine ("GameManager:");
		write.WriteLine ("Wave:"+GameManager.Instance.m_wave+"/"+GameManager.Instance.WaveTotal
		                 +",Life:"+GameManager.Instance.m_life+"/"+GameManager.Instance.LifeIni
		                 +",Money:"+GameManager.Instance.m_point+"/"+GameManager.Instance.PointIni
		                 +",Result:"+GameManager.Instance.m_result+","+GameManager.Instance.final_result);
		write.WriteLine ("Enemy Parameters:");
		foreach (EnemySpawner.EnemyTable item in enemySpawner.m_enemies) {
			EnemyController enemy=item.enemyPrefab.gameObject.GetComponent<EnemyController>();
			write.WriteLine("Name:"+item.enemyName+",Life:"+enemy.m_maxlife+",Speed:"+enemy.speedIni+",Reward:"+enemy.m_point
			                +",Harm:"+enemy.m_harm);
		}
		write.WriteLine ("Tower Parameters:");
		ArrayList towerlist = new ArrayList ();
		GameObject light = GameManager.Instance.createUI.transform.FindChild ("LightButton").GetComponent<TowerButton> ().tower;
		towerlist.Add (light);
		GameObject heavy = GameManager.Instance.createUI.transform.FindChild ("HeaveyButton").GetComponent<TowerButton> ().tower;
		towerlist.Add (heavy);
		GameObject poison = GameManager.Instance.createUI.transform.FindChild ("PoisonButton").GetComponent<TowerButton> ().tower;
		towerlist.Add (poison);
		GameObject ice = GameManager.Instance.createUI.transform.FindChild ("IceButton").GetComponent<TowerButton> ().tower;
		towerlist.Add (ice);
		GameObject elec = GameManager.Instance.createUI.transform.FindChild ("ElecButton").GetComponent<TowerButton> ().tower;
		towerlist.Add (elec);
		foreach(GameObject item in towerlist){
			DefenderController towerController=item.GetComponent<DefenderController>();
			write.WriteLine("Type:"+towerController.m_towerType+",Power:"+towerController.powerLow+" "+towerController.powerMid+" "
			                +towerController.powerHigh+" "+towerController.powerSuper+",AttackArea:"+ towerController.attackAreaLow+" "+
			                towerController.attackAreaMid+" "+towerController.attackAreaHigh+" "+towerController.attackAreaSuper+",AttackTime:"
			                +towerController.attackTimeLow+" "+towerController.attackTimeMid+" "+towerController.attackTimeHigh+" "+
			                towerController.attackTimeSuper+",Cost:"+towerController.costLow+" "+towerController.costMid+" "+towerController.costHigh+
			                " "+towerController.costSuper);
		}
		write.WriteLine ("Player:"+Settings.Instance.playerName+" "+"Test Time:"+Settings.Instance.testTime);
		write.Close ();
	}

	public void WriteOperations(){
		//string date = System.DateTime.Now.ToString ("yyyyMMddHHmmss");
		//Debug.Log (date);
		//string path1 = Path.Combine (Application.persistentDataPath, "Data");//We had better not use the custom folder!!!!!!
		string path = Path.Combine (Application.persistentDataPath,
		                            "Operation"+Settings.Instance.playerName+Settings.Instance.testTime+".txt");//System.IO.Directory.GetCurrentDirectory ()
		Debug.Log ("Path:" + path);
		StreamWriter write;
		FileInfo t = new FileInfo (path);
		if (!t.Exists) {
			write=t.CreateText();
		}else{
			t.Delete();
			write=t.CreateText();
		}
		write.WriteLine ("Player:"+Settings.Instance.playerName+","+"Test Time:"+Settings.Instance.testTime);
		write.WriteLine ("LoadedLevel:" + Application.loadedLevelName);
		write.WriteLine ("Settings:");
		write.WriteLine ("width:"+Settings.Instance.width+",height:"+Settings.Instance.height
		                 +",EnemyID:"+Settings.Instance.enemyID+",TowerID:"+Settings.Instance.towerID
		                 +",ThemeID:"+Settings.Instance.themeID+",music:"+Settings.Instance.music
		                 +",sound:"+Settings.Instance.sound);
		write.WriteLine ("Operations:");
		foreach (Operation item in ops) {
			write.WriteLine("Type:"+item.type+",PosX:"+item.posX+",PosY:"+item.posY+",Time:"+item.time);
		}
		write.Close ();
	}

}
