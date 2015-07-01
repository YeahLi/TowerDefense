using UnityEngine;
using System.Collections;

public class TowerButton : MonoBehaviour {
	//Button Skins
	public Sprite[] m_ButtonSkin;
	//Button ID
	public int m_ID=0;
	//Button cost
	[HideInInspector]
	public int m_cost=5;
	//afford
	bool afford=true;


	//Create Tower
	public GameObject tower;//This variable stands for the prefab in TowerButton.cs
		DefenderController towerController;
	GameObject m_tower;//This variable stands for the tower that instantiate in the game
		DefenderController m_towerController;//DefenderController pass this.m_cost to defender m_cost and costlow
	//The position to create the tower
	[HideInInspector]
	public Vector3 createPos;//let its father's pos

	public AudioClip placing;

	SpriteRenderer spriteRenderer;
	UpgradeLableController lableController;

	// Use this for initialization
	void Start () {
		spriteRenderer = this.gameObject.GetComponent<SpriteRenderer> ();
		towerController = tower.GetComponent<DefenderController> ();

		//Set the tower prefab values from Settings.txt
		SetTowerValue ();

		m_cost = towerController.costLow;
		lableController = this.transform.FindChild ("lable").GetComponent<UpgradeLableController> ();
		lableController.text = "" + m_cost;

		createPos = this.transform.parent.position;
	}

	//Set the tower prefab values from Settings.txt
	void SetTowerValue(){
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

	// Update is called once per frame
	void Update () {
		AffordCheck ();
		ClickItem ();
	}

	//button is clicked
	void ClickItem(){
		if (Input.GetMouseButtonDown (0)) {
			Vector3 v3 = Camera.main.ScreenToWorldPoint (Input.mousePosition); 
			Vector2 v2 = new Vector2 (v3.x, v3.y);
			RaycastHit2D hit = Physics2D.Raycast (v2, Vector2.zero);
			if (hit.collider != null) {
				//Debug.Log(hit.collider.tag);
				if (hit.collider.gameObject == this.gameObject) {
					//Debug.Log(m_ID);
					if(afford){
						RecordController.Operation op=new RecordController.Operation();
						op.posX=createPos.x;
						op.posY=createPos.y;
						op.time=RecordController.Instance.timer;
						//create tower
						CreateTower();
						if(Settings.Instance.sound){
							AudioSource.PlayClipAtPoint(placing,Camera.main.transform.position,1.0f);
						}

						switch(tower.GetComponent<DefenderController>().m_towerType){
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
					}
					Destroy(this.transform.parent.gameObject);
				}else{
					if(hit.collider.tag.CompareTo("twButton")==0){
						//do nothing
						//Debug.Log("Do nothing");
					}else{
						Destroy(this.transform.parent.gameObject);
					}
				}

			} else {
					Debug.LogError ("Click nothing");
			}
		}
	}
	
	//Check is affordable
	void AffordCheck(){
		if (GameManager.Instance.m_point < m_cost) {
			spriteRenderer.sprite = m_ButtonSkin [1];
			afford=false;
		} else {
			spriteRenderer.sprite = m_ButtonSkin [0];
			afford=true;
		}
	}

	void CreateTower(){
		//Debug.Log ("Create Tower");
		m_tower = (GameObject)Instantiate (tower, new Vector3(createPos.x,createPos.y,0f), Quaternion.identity);
		m_tower.transform.parent=GameObject.Find("Game").transform;
		m_tower.transform.localScale = new Vector3 (1, 1, 1);
		m_towerController = m_tower.GetComponent<DefenderController> ();
		m_towerController.m_cost = m_towerController.costLow;
		GameManager.Instance.SetPoint (-m_towerController.costLow);
	}
}
