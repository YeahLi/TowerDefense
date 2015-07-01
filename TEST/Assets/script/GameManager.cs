using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {
	public static GameManager Instance;

	//do not show wave in Inspector
	[HideInInspector]
	public int m_wave;
	[HideInInspector]
	public int m_life;
	[HideInInspector]
	public int m_point;
	[HideInInspector]
	public int WaveTotal=0;

	public int LifeIni=10;
	public int PointIni=10;

	//the performance of player which related to life lest and point, show in UI	
	[HideInInspector]
	public int m_result=0;
	public int LowMarks;
	public int mediumMarks;
	public int highMarks;
	//Use to BuildPath
	public bool m_debug=false;
	public ArrayList m_PathNodes;

	//Use to store enemy
	public ArrayList m_EnemyList=new ArrayList();

	//Use to Create Tower
	public GameObject createUI;
	GameObject m_ui;
	public GameObject forbidUI;
	GameObject m_forbid;
	int ix=-1;
	int iy=-1;

	//Create Tower
	GameObject m_tower;//This variable stands for the tower that instantiate in the game
	DefenderController m_towerController;//DefenderController pass this.m_cost to defender m_cost and costlow
	public AudioClip placing;
	[HideInInspector]
	public TowerButtonNew m_button;

	//UI
	UpgradeLableController moneyLable;
	UpgradeLableController waveLable;
	UpgradeLableController lifeLable;
	[HideInInspector]
	public UpgradeLableController resultLable;

	//GameOver
	public GameObject gameover;
	public AudioClip lose;
	bool over=false;
	void Awake(){
		Instance = this;
	}

	//Final Result
	[HideInInspector]
	public string final_result="";

	[HideInInspector]
	public bool clickUpgradeButtons=false; 

	// Use this for initialization
	void Start () {
		m_wave = 1;
		m_life = LifeIni;
		m_point = PointIni;
		m_result = 0;

		moneyLable = this.transform.FindChild ("money").FindChild ("lable").gameObject.GetComponent<UpgradeLableController> ();
		waveLable = this.transform.FindChild ("wave").FindChild ("lable").gameObject.GetComponent<UpgradeLableController> ();
		lifeLable = this.transform.FindChild ("life").FindChild ("lable").gameObject.GetComponent<UpgradeLableController> ();
		resultLable = this.transform.FindChild ("result").FindChild ("lable").gameObject.GetComponent<UpgradeLableController> ();

		moneyLable.text = "" + m_point;
		waveLable.text = "" + m_wave;
		lifeLable.text = "" + m_life;
		resultLable.text = "" + m_result;

		over = false;
		final_result = "";

		clickUpgradeButtons=false;

		m_button = null;
	}
	
	// Update is called once per frame
	void Update () {
		if (!Settings.Instance.replay) {
			detectCreateTower ();
		}
		if (m_life <= 0) {
			Time.timeScale=0;
			if (!over) {//only excute once
				over=true;
				final_result="Fail";
				m_result+=(m_wave-1)*10 + m_life*50;
				if(m_point-PointIni>0){
					m_result+= (m_point-PointIni)*2;
				}
				resultLable.text = "" + m_result;

				if(Settings.Instance.sound){
					AudioSource.PlayClipAtPoint(lose,Camera.main.transform.position,1.0f);
				}

				GameObject obj=(GameObject)Instantiate(gameover,new Vector3(this.transform.position.x,this.transform.position.y,-4),Quaternion.identity);
				obj.transform.parent=this.transform.parent;
			}
		}
	}

	public void SetWave(int wave){
		m_wave = wave;
		//UI
		waveLable.text = "" + m_wave;
	}

	public void SetDamage(int damage){
		m_life -= damage;
	
		//UI
		lifeLable.text = "" + m_life;
	}

	public void SetPoint(int point){
		m_point += point;
		//if the enemy is killed then add the result
		if(point>0){
			m_result = m_result + point;
		}
		//UI
		moneyLable.text = "" + m_point;
		resultLable.text = "" + m_result;
	}

	[ContextMenu("BuildPath")]
	void BuildPath(){
		m_PathNodes = new ArrayList ();
		GameObject[] objs=GameObject.FindGameObjectsWithTag("pathnode");

		for (int i=0; i<objs.Length; i++) {
			PathNode node=objs[i].GetComponent<PathNode>();
			m_PathNodes.Add(node);
		}
	}

	void OnDrawGizmos(){
		if (!m_debug || m_PathNodes == null) {
			return;
		}

		Gizmos.color = Color.blue;
		foreach (PathNode node in m_PathNodes) {
			if(node.m_next!=null){
				Gizmos.DrawLine(node.transform.position,node.m_next.transform.position);
			}
		}
	}

	void detectCreateTower(){
		//If game have already over
		if (m_life <= 0) {
			return;
		}
		if (Input.GetMouseButtonDown (1)) {//cancel m_ui
			if(m_ui){
				Destroy(m_ui);
			}
			if(m_button){
				if(m_button.m_state==TowerButtonNew.StateID.ACTIVE){
					m_button.m_state=TowerButtonNew.StateID.NORMAL;
					m_button.spriteRenderer.sprite=m_button.m_ButtonSkin[0];
				}
			}
		}
		else if (Input.GetMouseButtonDown (0)) {
			//get the mouse click position
			Vector3 v3 = Camera.main.ScreenToWorldPoint (Input.mousePosition); 
			Vector2 v2 = new Vector2 (v3.x, v3.y);

			RaycastHit2D hit = Physics2D.Raycast (v2, Vector2.zero);
			if (hit.collider != null) {

				//Debug.Log(hit.collider.tag);
				//check whether hit on the background
				if (hit.collider.tag.CompareTo("background")==0) {
					if(clickUpgradeButtons){
						clickUpgradeButtons=false;
						Debug.Log("click on upgrade buttons!");
						return;
					}

					if(m_ui){
						Destroy(m_ui);
					}
					float rate=bgController.Instance.rate;
					//convert click position to array index
					float newX=(v2.x+(rate-1.0f)*this.transform.position.x)/rate;
					int inewX=(int)(newX*10.0f+0.5f)/10;

					//if the point beyond the scene
					ix=inewX;	
					iy=(int)v2.y;
					if(ix>=GridMap.Instance.MapSizeX || iy>GridMap.Instance.MapSizeY || ix<0 || iy<0)
					{
						return;
					}

					//calculate the place position
					float posX=ix*rate-(rate-1.0f)*6.66f;
					Vector3 pos=new Vector3(posX+0.5f*rate,iy+0.5f,-2.0f);

					//check if this position can be put a defender
					if(GridMap.Instance.m_map[ix,iy].fieldtype==MapData.FieldTypeID.GuardPosition){
						if(Settings.Instance.interID==1){
							m_ui=(GameObject)Instantiate(createUI,pos,Quaternion.identity);
							m_ui.transform.parent=this.transform.parent;
							m_ui.transform.localScale=new Vector3(1.0f,1,1);
						}
						else if(Settings.Instance.interID==0){
							//Build a tower
							if(m_button){
								RecordController.Operation op=new RecordController.Operation();
								op.posX=pos.x;
								op.posY=pos.y;
								op.time=RecordController.Instance.timer;
								//create tower
								CreateTower(pos);
								if(Settings.Instance.sound){
									AudioSource.PlayClipAtPoint(placing,Camera.main.transform.position,1.0f);
								}
								
								switch(m_button.tower.GetComponent<DefenderController>().m_towerType){
								case DefenderController.TowerType.LightTower:
									op.type=RecordController.OperationType.LightTower;
									break;
								case DefenderController.TowerType.HeavyTower:
									op.type=RecordController.OperationType.HeavyTower;
									break;
								case DefenderController.TowerType.PoisonTower:
									op.type=RecordController.OperationType.PoisonTower;
									break;
								case DefenderController.TowerType.IceTower:
									op.type=RecordController.OperationType.IceTower;
									break;
								case DefenderController.TowerType.ElecTower:
									op.type=RecordController.OperationType.ElecTower;
									break;
								}
								RecordController.Instance.ops.Add(op);
								m_button = null;
							}
						}
					}else{
						m_button=null;
						//Debug.LogWarning("Cannot put here!!!");
						m_forbid=(GameObject)Instantiate(forbidUI,pos,Quaternion.identity);
						m_forbid.transform.parent=this.transform.parent;
						m_forbid.transform.localScale=new Vector3(1.0f,1,1);
						Destroy(m_forbid,0.3f);
					}
				}else{
					//Debug.Log("Do nothing");
				}
			}else{
				Debug.LogError("Click Nothing!");
			}
		}
	}

	void CreateTower(Vector3 createPos){
		//Debug.Log ("Create Tower");
		m_tower = (GameObject)Instantiate (m_button.tower, new Vector3(createPos.x,createPos.y,0f), Quaternion.identity);
		m_tower.transform.parent=GameObject.Find("Game").transform;
		m_tower.transform.localScale = new Vector3 (1, 1, 1);
		m_towerController = m_tower.GetComponent<DefenderController> ();
		m_towerController.m_cost = m_towerController.costLow;
		GameManager.Instance.SetPoint (-m_towerController.costLow);
	}

	public void ChangeClickUpgradeButton(){
		StartCoroutine(WaitAndChange(0.2f));
	}

	IEnumerator WaitAndChange(float waitTime) {
		//Debug.Log("run this function!");
		yield return new WaitForSeconds(waitTime);
		GameManager.Instance.clickUpgradeButtons=false;
		//Debug.Log ("now change to false");
	}
}
