using UnityEngine;
using System.Collections;

public class DefenderController : MonoBehaviour {
	public enum TowerType
	{
		LightTower,
		PoisonTower,
		HeavyTower,
		IceTower,
		ElecTower,
	}
	public TowerType m_towerType=TowerType.LightTower;
	public int spriteNum=0;
	public Sprite[] level1;
	public Sprite[] level2;
	public Sprite[] level3;
	public Sprite[] level4;
	[HideInInspector]
	public Sprite[] m_levelArray;

	public Sprite[] UpgradeButtons;

	Transform trans;
	SpriteRenderer spriteRenderer;
	int index=0;

	public GameObject projectile;
	//power
	[HideInInspector]
	public int m_power=5;
	//controll shoot animation
	bool shoot=false;
	float shootTimer=0.3f;

	//target enemy
	EnemyController m_targetEnemy;
	//attack area
	[HideInInspector]
	public float m_attackArea=4.0f;
	//attack time
	[HideInInspector]
	public float m_attackTime=1.0f;
	float m_timer=0.0f;
	//assigned target
	bool assignTarget=false;
	public GameObject icon;

	//whether the tower is selected
	bool selected = false;
	[HideInInspector]
	public int m_cost = 5;//need the button pass the value to it!!!!!!!!!!!
	int money=0;

	public enum TowerLevel{
		Low,
		Mid,
		High,
		Super
	}
	[HideInInspector]
	public TowerLevel m_level=TowerLevel.Low;

	public int powerLow=5;
	public int powerMid=7;
	public int powerHigh=9;
	public int powerSuper=15;

	public float attackAreaLow=2.0f;
	public float attackAreaMid=2.5f;
	public float attackAreaHigh=3.0f;
	public float attackAreaSuper=4.0f;

	public float attackTimeLow=1.0f;
	public float attackTimeMid=0.9f;
	public float attackTimeHigh=0.8f;
	public float attackTimeSuper=0.7f;

	public int costLow=5;
	public int costMid=10;
	public int costHigh=20;
	public int costSuper=45;

	Transform towerbutton;
	Transform upgrade;
	Transform attackArea;
	Transform lable;
	UpgradeLableController lableController;

	Animator anim;
	//Control the elec damage
	float electimer=0f;

	//sound
	public AudioClip upgradeSound;
	public AudioClip selling;
	public AudioClip attackSound;
	// Use this for initialization
	void Start () {
		anim = gameObject.GetComponent<Animator> ();

		trans=this.transform.FindChild("attack");
		spriteRenderer = this.gameObject.GetComponent<SpriteRenderer> ();
		towerbutton=this.transform.FindChild("towerbutton");
		upgrade=towerbutton.FindChild("upgrade");
		attackArea = this.transform.FindChild ("attackArea");
		//get the lable text
		lable=upgrade.FindChild("lable");
		lableController=lable.gameObject.GetComponent<UpgradeLableController>();
		//set the gridnode as CanNotStand
		float newX=(this.transform.position.x+(bgController.Instance.rate-1.0f)*6.66f) / bgController.Instance.rate;
		int inewX=(int)(newX*10.0f+0.5f)/10;
		GridMap.Instance.m_map [inewX, (int)this.transform.position.y].fieldtype = MapData.FieldTypeID.CanNotStand;

		//set init values
		m_levelArray = level1;
		m_level = TowerLevel.Low;
		m_power = powerLow;
		m_attackArea = attackAreaLow;
		attackArea.localScale = new Vector3 (m_attackArea,m_attackArea,0);
		m_attackTime = attackTimeLow;
		m_cost = costLow;

		//set the timer as 0
		m_timer = 0f;
	}
	// Update is called once per fixedDeltaTime
	void FixedUpdate () {
		if (m_timer > 0) {
			m_timer-=Time.fixedDeltaTime;
		}
		CheckAssignedTarget();
		FindEnemy();
		if (m_towerType != TowerType.ElecTower) {
			RotateTo ();
		} else {
			ElecTowerRotateTo();
			ElecTowerAttack();
		}
	}

	void Update(){
		if(!Settings.Instance.replay){
			AffordCheck ();
			ClickItem ();
		}
	}

	void RotateTo(){
		if (m_targetEnemy == null || m_targetEnemy.m_life<=0) {
			if (shoot) {
				shootTimer-=Time.fixedDeltaTime;
				if(shootTimer<0){
					shoot=false;
					shootTimer=0.3f;
					spriteRenderer.sprite = m_levelArray[index];
				}
			}
			return;
		}
		Vector3 current = trans.eulerAngles;

		Vector3 vec = m_targetEnemy.transform.position-trans.position;
		float angle = Mathf.Atan2(vec.y,vec.x) * Mathf.Rad2Deg;//vector.right as the direction
		trans.rotation = Quaternion.AngleAxis(angle, Vector3.forward);//along axis Z rotate angle degree
		Vector3 target = trans.eulerAngles;
		
		float next = Mathf.MoveTowardsAngle (current.z, target.z, 4000 * Time.fixedDeltaTime);//4000
		trans.eulerAngles = new Vector3 (0, 0, next);

		index = (int)((360 - trans.eulerAngles.z) / (360.0f/spriteNum));
		if (index == spriteNum) {
			index=0;
		}
		spriteRenderer.sprite = m_levelArray[index];

		if (trans.eulerAngles.z < (target.z + 3) && trans.eulerAngles.z > (target.z - 3) ) {//3
			Attack();
		}
	}

	void ElecTowerRotateTo(){
		if (m_targetEnemy == null || m_targetEnemy.m_life<=0) {
			return;
		}
		Vector3 vec = m_targetEnemy.transform.position-trans.position;
		float angle = Mathf.Atan2(vec.y,vec.x) * Mathf.Rad2Deg;//vector.right as the direction
		trans.rotation = Quaternion.AngleAxis(angle, Vector3.forward);//along axis Z rotate angle degree
	}

	void ElecTowerAttack(){
		//m_timer-=Time.fixedDeltaTime;
		electimer -= Time.fixedDeltaTime;
		if(m_targetEnemy==null){
			return;
		}
		if (m_targetEnemy.m_life <= 0) {
			return;
		}
		if(m_timer>0){
			return;
		}

		if (electimer < 0) {
			m_targetEnemy.SetDamage((int)(m_power*0.2f)+1);
			electimer=0.2f;
		}

		GameObject proj=(GameObject)Instantiate (projectile, trans.position, trans.rotation);
		proj.transform.parent = this.transform.parent;
		ProjectileController projController=proj.GetComponent<ProjectileController>();
		projController.damage = m_power;
		if(Settings.Instance.sound){
			AudioSource.PlayClipAtPoint(attackSound,transform.position,1.0f);
		}

		m_timer=m_attackTime;
	}

	void Shoot(){
		shoot = true;
		if (index == spriteNum) {
			index=0;
		}

		GameObject proj=(GameObject)Instantiate (projectile, trans.position, trans.rotation);
		if (m_levelArray.Length>spriteNum) {
			spriteRenderer.sprite = m_levelArray [index + spriteNum];
		}

		proj.transform.parent = this.transform.parent;
		ProjectileController projController=proj.GetComponent<ProjectileController>();
		projController.damage = m_power;
	}

	public void CheckAssignedTarget(){
		foreach(EnemyController enemy in GameManager.Instance.m_EnemyList){
			if(enemy.clicked){
				assignTarget=true;
				break;
			}else{
				assignTarget=false;
			}
		}
	}

	//Find target
	void FindEnemy(){
		//clean the current target
		m_targetEnemy=null;
		//use to compare the life of enemies
		int lastlife=0;
		//traversing the enemylist in GameManager
		foreach(EnemyController enemy in GameManager.Instance.m_EnemyList){
			//ignore the enemy whose life is under zero
			if(enemy.m_life==0){
				continue;
			}
			
			Vector3 pos1=this.transform.position;
			Vector3 pos2=enemy.transform.position;
			
			//calculate the distance
			float distance=Vector2.Distance(new Vector2(pos1.x,pos1.y),new Vector2(pos2.x,pos2.y));
			
			//ignore the enemy out of range
			if(distance>m_attackArea){
				continue;
			}
			
			//NEED TO ADD CHOOSE WHICH ONE TO KILL!!!!!!
			if(assignTarget){
				if(enemy.clicked && distance<=m_attackArea){
					m_targetEnemy=enemy;
					//create the target icon
					if(enemy.icon==false){
						Vector3 pos=new Vector3(enemy.transform.position.x,enemy.transform.position.y,enemy.transform.position.z-1);
						GameObject tarIcon=(GameObject)Instantiate(icon,pos,Quaternion.identity);
						tarIcon.transform.parent=enemy.transform;
						enemy.icon=true;
					}
					break;
				}else{
					//find the easiest to kill
					if(lastlife<=0 ||lastlife>enemy.m_life){
						m_targetEnemy=enemy;
						lastlife=enemy.m_life;
					}
				}
			}else{
				//find the easiest to kill
				if(lastlife<=0 ||lastlife>enemy.m_life){
					m_targetEnemy=enemy;
					lastlife=enemy.m_life;
				}
			}
		}
		
	}

	//Attack
	public void Attack(){
		///m_timer-=Time.fixedDeltaTime;//move to update()
		
		if(m_targetEnemy==null){
			return;
		}

		if (m_targetEnemy.m_life <= 0) {
			return;
		}
		
		if(m_timer>0){
			return;
		}
		//Fire
		Shoot ();
		if(Settings.Instance.sound){
			AudioSource.PlayClipAtPoint(attackSound,transform.position,1.0f);
		}
		
		m_timer=m_attackTime;
	}

	void ClickItem(){
		if (Input.GetMouseButtonDown (1)) {
			this.selected=false;
			towerbutton.gameObject.SetActive(false);
			attackArea.gameObject.SetActive(false);
		}else if(Input.GetMouseButtonDown(0)){
			Vector3 v3=Camera.main.ScreenToWorldPoint(Input.mousePosition); 
			Vector2 v2=new Vector2(v3.x,v3.y);
			RaycastHit2D hit=Physics2D.Raycast(v2,Vector2.zero);
			if(hit.collider!=null)
			{
				//Debug.Log(hit.collider.tag);
				if(hit.collider.gameObject==this.gameObject){
					if(GameManager.Instance.clickUpgradeButtons){
						GameManager.Instance.clickUpgradeButtons=false;
						return;
					}

					this.selected=true;
					//show the menu
					towerbutton.gameObject.SetActive(true);
					attackArea.gameObject.SetActive(true);

				}else{
					if(this.selected){
						if(hit.collider.tag.CompareTo("upgrade")==0){
							GameManager.Instance.clickUpgradeButtons=true;
							GameManager.Instance.ChangeClickUpgradeButton();
							if(m_level!=TowerLevel.Super){
								if(GameManager.Instance.m_point>=money){
									RecordController.Operation op=new RecordController.Operation();
									op.time=RecordController.Instance.timer;
									op.type=RecordController.OperationType.Upgrade;
									op.posX=this.transform.position.x;
									op.posY=this.transform.position.y;
									//Debug.Log ("tower up grade");
									UpdateTower();

									RecordController.Instance.ops.Add(op);
								}
							}
						}else if(hit.collider.tag.CompareTo("remove")==0){
							GameManager.Instance.clickUpgradeButtons=true;
							GameManager.Instance.ChangeClickUpgradeButton();
							RecordController.Operation op=new RecordController.Operation();
							op.time=RecordController.Instance.timer;
							op.type=RecordController.OperationType.Sell;
							op.posX=this.transform.position.x;
							op.posY=this.transform.position.y;
							//Debug.Log ("tower removed");
							RemoveTower();

							RecordController.Instance.ops.Add(op);
						}
					}
					this.selected=false;
					towerbutton.gameObject.SetActive(false);
					attackArea.gameObject.SetActive(false);
				}
			}else{
				Debug.Log("not hitted anything!");
			}	
		}
	}

	public void RemoveTower(){
		if(Settings.Instance.sound){
			AudioSource.PlayClipAtPoint(selling,Camera.main.transform.position,1.0f);
		}
		GameManager.Instance.SetPoint (m_cost/2);
		//set this GridNode as GuardPosition
		float newX = (this.transform.position.x + (bgController.Instance.rate - 1.0f) * 6.66f) / bgController.Instance.rate;
		int inewX=(int)(newX*10.0f+0.5f)/10;
		int iy=(int)this.transform.position.y;
		GridMap.Instance.m_map[inewX,iy].fieldtype=MapData.FieldTypeID.GuardPosition;
		Destroy (gameObject);
	}

	public void UpdateTower(){
		if(Settings.Instance.sound){
			AudioSource.PlayClipAtPoint(upgradeSound,Camera.main.transform.position,1.0f);
		}
		if(m_towerType==TowerType.ElecTower){
			UpdateElecTower();
		}else{
			switch (m_level) {
			case TowerLevel.Low:
				m_levelArray=level2;
				m_level=TowerLevel.Mid;
				m_power=powerMid;
				m_attackArea=attackAreaMid;
				m_attackTime=attackTimeMid;
				m_cost+=costMid;
				GameManager.Instance.SetPoint (-costMid);
				break;
			case TowerLevel.Mid:
				m_levelArray=level3;
				m_level=TowerLevel.High;
				m_power=powerHigh;
				m_attackArea=attackAreaHigh;
				m_attackTime=attackTimeHigh;
				m_cost+=costHigh;
				GameManager.Instance.SetPoint (-costHigh);
				break;
			case TowerLevel.High:
				m_levelArray=level4;
				m_level=TowerLevel.Super;
				m_power=powerSuper;
				m_attackArea=attackAreaSuper;
				m_attackTime=attackTimeSuper;
				m_cost+=costSuper;
				GameManager.Instance.SetPoint (-costSuper);
				break;
			case TowerLevel.Super:
				break;
			}

			if (index == spriteNum) {
				index=0;
			}
			//Debug.Log (index);

			spriteRenderer.sprite=m_levelArray[index];
			attackArea.localScale = new Vector3 (m_attackArea,m_attackArea,0);
		}
	}
	//Called in UpdateTower()
	void UpdateElecTower(){

		switch (m_level) {
		case TowerLevel.Low:
			m_level=TowerLevel.Mid;
			m_power=powerMid;
			m_attackArea=attackAreaMid;
			m_attackTime=attackTimeMid;
			m_cost+=costMid;
			GameManager.Instance.SetPoint (-costMid);
			break;
		case TowerLevel.Mid:
			m_level=TowerLevel.High;
			m_power=powerHigh;
			m_attackArea=attackAreaHigh;
			m_attackTime=attackTimeHigh;
			m_cost+=costMid;
			GameManager.Instance.SetPoint (-costHigh);
			break;
		case TowerLevel.High:
			m_level=TowerLevel.Super;
			m_power=powerSuper;
			m_attackArea=attackAreaSuper;
			m_attackTime=attackTimeSuper;
			m_cost+=costSuper;
			GameManager.Instance.SetPoint (-costSuper);
			break;
		case TowerLevel.Super:
			break;
		}
	
		anim.SetTrigger("upgrade");
		attackArea.localScale = new Vector3 (m_attackArea,m_attackArea,0);
	}

	void AffordCheck(){
		switch(m_level){
		case TowerLevel.Low:
			money=costMid;
			break;
		case TowerLevel.Mid:
			money=costHigh;
			break;
		case TowerLevel.High:
			money=costSuper;
			break;
		}
		//set the lable text
		lableController.text = "" + money;


		SpriteRenderer upgradeRenderer=upgrade.gameObject.GetComponent<SpriteRenderer>();
		//Can not afford mony to upgrade
		if(GameManager.Instance.m_point<money){
			//Debug.Log("trun grey");
			upgradeRenderer.sprite=UpgradeButtons[1];
		}else{
			if(m_level==TowerLevel.Super){
				upgradeRenderer.sprite=UpgradeButtons[2];
				lableController.text="";
			}else{
				upgradeRenderer.sprite=UpgradeButtons[0];
			}
		}
	}
}
