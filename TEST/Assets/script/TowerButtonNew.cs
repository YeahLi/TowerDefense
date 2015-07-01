using UnityEngine;
using System.Collections;

public class TowerButtonNew : MonoBehaviour {
	public enum StateID
	{
		NORMAL=0,
		ACTIVE,   //Seclected
		UNAVAILABLE //grey
	}
	//Button Skin Library
	public Sprite[] m_ButtonSkin;
	//Button ID
	public int m_ID=0;
	//Button cost
	[HideInInspector]
	public int m_cost=5;
	//Button state
	[HideInInspector]
	public StateID m_state=StateID.NORMAL;
	//Button image and lable
	[HideInInspector]
	public SpriteRenderer spriteRenderer;
	UpgradeLableController lableController;

	//Tower Object
	public GameObject tower;//This variable stands for the prefab in TowerButton.cs
	DefenderController towerController;

	// Use this for initialization
	void Start () {
		spriteRenderer = this.gameObject.GetComponent<SpriteRenderer> ();

		towerController = tower.GetComponent<DefenderController> ();
		//Set the tower prefab values from Settings.txt
		SetTowerValue ();

		m_cost = towerController.costLow;
		lableController = this.transform.FindChild ("lable").GetComponent<UpgradeLableController> ();
		lableController.text = "" + m_cost;

		m_state = StateID.NORMAL;
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

	//Check is affordable
	void AffordCheck(){
		if (GameManager.Instance.m_point < m_cost) {
			spriteRenderer.sprite = m_ButtonSkin [1];
			m_state=StateID.UNAVAILABLE;
		} else {
			if(m_state==StateID.UNAVAILABLE){
				spriteRenderer.sprite = m_ButtonSkin [0];
				m_state=StateID.NORMAL;
			}
		}
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
					if(m_state!=StateID.UNAVAILABLE){
						if(m_state==StateID.NORMAL){
							m_state=StateID.ACTIVE;
							spriteRenderer.sprite=m_ButtonSkin[2];
							GameManager.Instance.m_button=this.gameObject.GetComponent<TowerButtonNew>();
						}
						else if(m_state==StateID.ACTIVE){
							m_state=StateID.NORMAL;
							spriteRenderer.sprite=m_ButtonSkin[0];
							GameManager.Instance.m_button=null;
						}
					}
				}else{
					if(m_state==StateID.ACTIVE){
						m_state=StateID.NORMAL;
						spriteRenderer.sprite=m_ButtonSkin[0];
					}
					if(hit.collider.tag.CompareTo("background")!=0
					   && hit.collider.tag.CompareTo("twButton")!=0){
						GameManager.Instance.m_button=null;
					}
				}
			}
		}
	}
}
