using UnityEngine;
using System.Collections;

public class EnemySpawner : MonoBehaviour {

	//define the enemy mark
	[System.Serializable]
	public class EnemyTable
	{
		public string enemyName = "";
		public Transform enemyPrefab;
	}
	//XML data
	public class SpawnData{
		public int wave=1;
		public string enemyname = "";
		public int level=1;
		public float wait=1.0f;
	}
	//Enemy array
	public EnemyTable[] m_enemies;
	//Start Point
	public PathNode m_startNode;
	//xml
	public TextAsset xmldata;
	//save all data
	ArrayList m_enemyList;

	float m_timer=0;
	int m_index=0;

	//only all the enemies in one wave are destroyed the next would come
	[HideInInspector]
	public int m_liveEnemy=0;

	public GameObject gamewin;
	bool win=false;

	public AudioClip begin;
	bool playbegin=true;
	public GameObject Go;
	public AudioClip vectroy;

	// Use this for initialization
	void Start(){
		//Set the values for enemies from Settings.txt
		SetEnemiesValue ();
		//Read the xml file
		ReadXML ();
		//Get the next enemy's waiting time
		SpawnData data = (SpawnData)m_enemyList [m_index];
		m_timer = data.wait;
		//connect with RecordController
		RecordController.Instance.enemySpawner = this;

		win = false;
		playbegin = true;
	}

	void SetEnemiesValue (){
		foreach (EnemyTable item in m_enemies) {
			EnemyController enemyCon=item.enemyPrefab.gameObject.GetComponent<EnemyController>();
			foreach(Settings.EnemyInfo info in Settings.Instance.enemiesInfo){
				if(info.name==item.enemyName){
					enemyCon.m_maxlife=info.life;
					enemyCon.speedIni=info.speed;
					enemyCon.m_point=info.reward;
					enemyCon.m_harm=info.harm;
					break;
				}
			}
		}
	}

	void ReadXML(){
		m_enemyList=new ArrayList();

		XMLParser xmlparse = new XMLParser ();
		XMLNode node = xmlparse.Parse (xmldata.text);

		XMLNodeList list=node.GetNodeList("ROOT>0>table");
		for (int i=0; i<list.Count; i++) {
			string wave=node.GetValue("ROOT>0>table>"+i+">@wave");
			string enemyname=node.GetValue("ROOT>0>table>"+i+">@enemyname");
			string level=node.GetValue("ROOT>0>table>"+i+">@level");
			string wait=node.GetValue("ROOT>0>table>"+i+">@wait");

			SpawnData data=new SpawnData();
			data.wave=int.Parse(wave);
			data.enemyname=enemyname;
			data.level=int.Parse(level);
			data.wait=float.Parse(wait);

			m_enemyList.Add(data);
		}

	}

	// Update is called once per frame
	void FixedUpdate () {
		SpawnEnemy ();
	}

	void SpawnEnemy(){
		//if have already spawned all the enemy
		if (m_index >= m_enemyList.Count) {
			if(m_liveEnemy==0){
				if(GameManager.Instance.m_life>0){
					Time.timeScale=0;
					if(!win){//only excute once!
						//Debug.Log("YOU WIN");
						GameManager.Instance.m_result+=(GameManager.Instance.m_wave-1)*10 + GameManager.Instance.m_life*50;
						if(GameManager.Instance.m_point-GameManager.Instance.PointIni>0){
							GameManager.Instance.m_result+= (GameManager.Instance.m_point-GameManager.Instance.PointIni)*2;
						}
						GameManager.Instance.resultLable.text = "" + GameManager.Instance.m_result;

						GameObject obj=(GameObject)Instantiate(gamewin,new Vector3(this.transform.parent.position.x,this.transform.parent.position.y,-4),Quaternion.identity);
						obj.transform.parent=this.transform.parent;
						if(Settings.Instance.sound){
							AudioSource.PlayClipAtPoint(vectroy,Camera.main.transform.position,1.0f);
						}
					}
					win=true;
					GameManager.Instance.final_result="Win";
				}
			}
			return;
		}

		//get the next enemy
		SpawnData data = (SpawnData)m_enemyList [m_index];
		//if the enemy is from the next wave then wait until the current wave's enemies all be destroyed
		if (GameManager.Instance.m_wave < data.wave && m_liveEnemy > 0) {
			return;
		}

		//wait
		m_timer -= Time.fixedDeltaTime;
		if (m_timer > 0) {
			return;
		}
		//Go to next wave
		if (GameManager.Instance.m_wave < data.wave) {
			GameManager.Instance.SetWave(data.wave);
		}
		//play sound
		if (playbegin) {
			SpawnData lastData = (SpawnData)m_enemyList [m_enemyList.Count-1];
			GameManager.Instance.WaveTotal = lastData.wave;
			if(Settings.Instance.sound){

				AudioSource.PlayClipAtPoint(begin,Camera.main.transform.position,1.0f);
			}
			GameObject item = (GameObject)Instantiate(Go);
			playbegin=false;
			Destroy(item,1.0f);
		}
		//Find enemy
		Transform enemyPrefab = FindEnemy (data.enemyname);
		//create enemy
		if (enemyPrefab != null) {
			Transform trans=(Transform)Instantiate(enemyPrefab,this.transform.position,Quaternion.identity);
			trans.parent=this.transform.parent;
			//trans.localScale=new Vector3(1,1,1);
			EnemyController enemy=trans.GetComponent<EnemyController>();
			enemy.m_currentNode=m_startNode;
			//Connect to the enemy spawn
			enemy.m_spawn=this;

			//set the enemy initial rotation
				//Look at
				Vector3 vec = enemy.m_currentNode.transform.position - this.transform.position;
				float angle = Mathf.Atan2(vec.y,vec.x) * Mathf.Rad2Deg;//vector.right as the direction
				//Debug.Log (angle);
				if (angle >= -180 && angle < -135) {
					enemy.anim.SetTrigger("left");
					enemy.m_state=EnemyController.AnimState.Left;
					angle+=180;
				}
				else if (angle >= -135 && angle < -45) {
					enemy.anim.SetTrigger("front");
					enemy.m_state=EnemyController.AnimState.Down;
					angle+=90;
				}
				else if (angle >= -45 && angle < 45) {
					enemy.anim.SetTrigger("right");
					enemy.m_state=EnemyController.AnimState.Right;
				}
				else if (angle >= 45 && angle < 135) {
					enemy.anim.SetTrigger("back");
					enemy.m_state=EnemyController.AnimState.Up;
					angle-=90;
				}else if(angle>=135 && angle<180){
					enemy.anim.SetTrigger("left");
					enemy.m_state=EnemyController.AnimState.Left;
					angle-=180;
				}
				transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);//along axis Z rotate angle degree
			//set Enemy Level.........................................

		}

		m_index++;
		if (m_index >= m_enemyList.Count) {
			return;
		}
		//get the next enemy waiting time
		SpawnData nextdata = (SpawnData)m_enemyList [m_index];
		m_timer = nextdata.wait;
	}
	//Find the Enemy in EnemyTable[]
	Transform FindEnemy(string enemyname){
		foreach( EnemyTable enemy in m_enemies) {
			if(enemy.enemyName.CompareTo(enemyname)==0){
				return enemy.enemyPrefab;
			}
		}
		return null;
	}

	void OnDrawGizmos(){
		Gizmos.DrawIcon(transform.position,"spawner.tif");
	}
}
