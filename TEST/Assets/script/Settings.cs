using UnityEngine;
using System.Collections;
using System.IO;
public class Settings : MonoBehaviour {
	[System.Serializable]
	public class TowerInfo
	{
		public string type;
		public int[] power=new int[4];
		public float[] attackArea=new float[4];
		public float[] attackTime=new float[4];
		public int[] cost=new int[4];
	}
	public class EnemyInfo
	{
		public string name;
		public int life;
		public float speed;
		public int reward;
		public int harm;
	}

	public static Settings Instance;
	public int width=Screen.width;
	public int height=Screen.height;
	public int enemyID=0;
	public int towerID=0;
	public int themeID=0;
	public bool music=true;
	public bool sound=true;
	public bool replay=false;
	public int interID=0;
	//store the operation record path, assigned in the replay scene 
	public string path="";

	//the following variables store the info getting from Settings.txt
	public ArrayList towersInfo;
	public ArrayList enemiesInfo;
	private ArrayList lineList;

	public int initMoney=0;
	public int initLife=0;
	public int lowMarks=0;
	public int mediumMarks=0;
	public int highMarks=0;

	//Player Info
	public string playerName="";
	public string testTime="";

	void Awake(){
		Instance = this;
	}
	// Use this for initialization
	void Start () {
		//get the former game settings from PlayerPrefs
		int setResol = PlayerPrefs.GetInt ("setResolution");
		if (setResol == 1111) {
			width = PlayerPrefs.GetInt ("width");
			height = PlayerPrefs.GetInt ("height");
		} else {
			width = Screen.width;
			height = Screen.height;
		}
		//get music and sound from PlayerPrefs
		int pre_music = PlayerPrefs.GetInt ("music");
		int pre_sound=PlayerPrefs.GetInt("sound");
		if (pre_music==1) {
			music=true;
		}else{
			music=false;
		}
		if(pre_sound==1){
			sound=true;
		}else{
			sound=false;
		}
		//get interID from PlayerPrefs
		int setInter=PlayerPrefs.GetInt("SetInteraction");
		if(setInter==2222){
			interID=PlayerPrefs.GetInt("InterID");
		}else{
			interID=0;
		}
		//the replay flag
		replay=false;
		//Debug.Log ("Replay:" + replay);

		//init the settings variables
		lineList = new ArrayList ();
		enemiesInfo = new ArrayList ();
		towersInfo = new ArrayList ();
		//Load the parameters of tower and enemy into the Settings.cs
		ReadSettings ();
		GetInitManager ();
		GetEnemySetting();
		GetTowerSetting ();
	}
	//read the file and put every line into lineList
	void ReadSettings(){
		//get the information from Settings and put it into lineList
		TextAsset settingText = (TextAsset)Resources.Load ("Settings");
		StringReader reader = new StringReader (settingText.text);
		if ( reader == null )
		{
			Debug.Log("Settings.txt not found or not readable");
		}
		else
		{
			// Read each line from the file
			string line;
			lineList = new ArrayList ();
			while ((line=reader.ReadLine())!=null) {
				lineList.Add(line);
			}
		}
		reader.Close ();
	}
	//get GameManager parameters
	void GetInitManager(){
		string txt=(string)lineList[1];
		string[] info=txt.Split(',');
		initMoney=int.Parse(info[0].Split(':')[1]);
		initLife = int.Parse (info [1].Split (':') [1]);
		lowMarks=int.Parse (info [2].Split (':') [1]);
		mediumMarks=int.Parse (info [3].Split (':') [1]);
		highMarks=int.Parse (info [4].Split (':') [1]);
	}
	//get Enemy parameters
	void GetEnemySetting(){
		for (int i=3; i<14; i++) {
			string txt=(string)lineList[i];
			string[] info=txt.Split(',');
			EnemyInfo enemyInfo=new EnemyInfo();
			enemyInfo.name=info[0].Split(':')[1];
			enemyInfo.life=int.Parse (info [1].Split (':') [1]);
			enemyInfo.speed=float.Parse (info [2].Split (':') [1]);
			enemyInfo.reward=int.Parse (info [3].Split (':') [1]);
			enemyInfo.harm=int.Parse (info [4].Split (':') [1]);
			//add to enemiesInfo
			enemiesInfo.Add(enemyInfo);
		}
	}
	//get tower parameters
	void GetTowerSetting(){
		for (int i=15; i<20; i++) {
			string txt=(string)lineList[i];
			string[] info=txt.Split(',');
			TowerInfo towerInfo=new TowerInfo();
			towerInfo.type=info[0].Split(':')[1];
			string power=info[1].Split(':')[1];
			string area=info[2].Split(':')[1];
			string time=info[3].Split(':')[1];
			string cost=info[4].Split(':')[1];
			string[] powers=power.Split(' ');
			string[] areas=area.Split(' ');
			string[] times=time.Split(' ');
			string[] costs=cost.Split(' ');
			for(int j=0;j<4;j++){
				towerInfo.power[j]=int.Parse(powers[j]);
				towerInfo.attackArea[j]=float.Parse(areas[j]);
				towerInfo.attackTime[j]=float.Parse(times[j]);
				towerInfo.cost[j]=int.Parse(costs[j]);
			}
			towersInfo.Add(towerInfo);
		}
	}
}
