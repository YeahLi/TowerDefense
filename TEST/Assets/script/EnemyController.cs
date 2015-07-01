using UnityEngine;
using System.Collections;

public class EnemyController : MonoBehaviour {
	[HideInInspector]//Do not show in Inspector
	public bool die=false;
	//For skelet
	public bool is_Revive=false;
	//Animation
	[HideInInspector]
	public Animator anim;
	//PathNode
	public PathNode m_currentNode;
	//Life
	[HideInInspector]
	public int m_life;
	public int m_maxlife=15;
	//speed
	public float speedIni=2.0f;
	[HideInInspector]
	public float m_speed=2.0f;
	//point
	public int m_point=5;
	//harm
	public int m_harm=1;
	//Enemy Type
	public enum TYPE_ID{
		Enemy,
		Friend,
	}
	public TYPE_ID m_type=TYPE_ID.Enemy;

	//Enemy AnimState State
	public enum AnimState{
		Up,
		Down,
		Left,
		Right,
		Die
	}
	[HideInInspector]
	public AnimState m_state=AnimState.Right;

	//Connect to EnemySpawner
	[HideInInspector]
	public EnemySpawner m_spawn;

	//Manual as a target used in defender
	[HideInInspector]
	public bool clicked=false;
	[HideInInspector]
	public bool icon=false;

	//Health Bar
	public GameObject healthbar;
	GameObject m_bar;
	healthBarController m_barController;

	//sound
	public AudioClip runSound;
	public AudioClip dieSound;
	public float yelltime=3.0f;
	float yelltimer;
	public AudioClip castlehit;

	//use this flag to make that the death only excute once
	bool is_dead=false;

	void Awake(){
		anim = GetComponent<Animator> ();
	}
	// Use this for initialization
	void Start () {
		m_life = m_maxlife;
		m_state=AnimState.Right;
		m_speed = speedIni;

		m_spawn.m_liveEnemy++;
		GameManager.Instance.m_EnemyList.Add (this);

		//create a health bar
		m_bar = (GameObject) Instantiate (healthbar,
		                     new Vector3 (this.transform.position.x - 0.5f, this.transform.position.y + 0.6f, 0),
		                     Quaternion.identity);
		m_bar.transform.parent = this.transform;
		m_barController = m_bar.transform.FindChild("above").GetComponent<healthBarController> ();
		m_barController.UpdateHealthBar (m_life * 1.0f / m_maxlife);

		yelltimer = yelltime;

		is_dead = false;
	}
	//called before Destroy()
	void OnDisable(){
		if (m_spawn) {
			m_spawn.m_liveEnemy--;
		}
		if (GameManager.Instance) {
			GameManager.Instance.m_EnemyList.Remove(this);
		}
	}

	public void SetDamage(int damage){
		m_life -= damage;
		m_barController.UpdateHealthBar (m_life * 1.0f / m_maxlife);
		if (m_life <= 0) {
			if(!is_dead){
				is_dead=true;//only excute once!!!
				//GameManager.Instance.m_EnemyList.Remove(this);//SHOULD BE DELETE
				if (m_bar) {
					Destroy(m_bar.gameObject);
				}
				anim.SetTrigger("die");
				m_state=AnimState.Die;

				if(Settings.Instance.sound){
					AudioSource.PlayClipAtPoint(dieSound,transform.position,1.0f);
				}

				GameManager.Instance.SetPoint(m_point);
			}
		}
	}

	public void SlowDown(int damage){
		m_speed =m_speed*(1.0f- damage*0.04f);
		//Debug.Log (m_speed);
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if (this.clicked==false) {//if the enemy is not clicked then destroy the target icon
			if(icon==true){
				Destroy(this.transform.FindChild("tarIcon(Clone)").gameObject);
				icon=false;
			}
		}

		if (m_life > 0) {
			RotateTo ();
			MoveTo ();
		}
		//play sound
		yelltimer -= Time.fixedDeltaTime;
		if(yelltimer<0){
			if(Settings.Instance.sound){
				AudioSource.PlayClipAtPoint(runSound,transform.position,1.0f);
			}
			yelltimer=yelltime;
		}
	}

	void Update(){
		ClickItem();
	}

	//turn to m_currentNode
	public void RotateTo(){
		Vector3 current = this.transform.eulerAngles;
		//Look at
		Vector3 vec = m_currentNode.transform.position - this.transform.position;
		float angle = Mathf.Atan2(vec.y,vec.x) * Mathf.Rad2Deg;//vector.right as the direction
			//Debug.Log (angle);
		if (angle >= -180 && angle < -135) {
			anim.SetTrigger("left");
			m_state=AnimState.Left;
			angle+=180;
		}
		else if (angle >= -135 && angle < -45) {
			anim.SetTrigger("front");
			m_state=AnimState.Down;
			angle+=90;
		}
		else if (angle >= -45 && angle < 45) {
			anim.SetTrigger("right");
			m_state=AnimState.Right;
		}
		else if (angle >= 45 && angle < 135) {
			anim.SetTrigger("back");
			m_state=AnimState.Up;
			angle-=90;
		}else if(angle>=135 && angle<180){
			anim.SetTrigger("left");
			m_state=AnimState.Left;
			angle-=180;
		}
		transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);//along axis Z rotate angle degree

		Vector3 target = this.transform.eulerAngles;

		float next = Mathf.MoveTowardsAngle (current.z, target.z, 120 * Time.fixedDeltaTime);
		this.transform.eulerAngles = new Vector3 (0, 0, next);
	}

	//move to m_currentNode
	public void MoveTo(){
		Vector3 pos1 = this.transform.position;
		Vector3 pos2 = m_currentNode.transform.position;

		float dist = Vector2.Distance (new Vector2 (pos1.x, pos1.y), new Vector2 (pos2.x, pos2.y));
		if (dist < 0.25f) {
			if(m_currentNode.m_next==null){
				GameManager.Instance.SetDamage(m_harm);
				if(Settings.Instance.sound){
					AudioSource.PlayClipAtPoint(castlehit,Camera.main.transform.position,1.0f);
				}
				Destroy(this.gameObject);
			}else{
				m_currentNode=m_currentNode.m_next;
			}
		}
		switch (m_state) {
		case AnimState.Right:
			this.transform.Translate(new Vector3(m_speed*Time.fixedDeltaTime,0,0));
			break;
		case AnimState.Up:
			this.transform.Translate(new Vector3(0,m_speed*Time.fixedDeltaTime,0));
			break;
		case AnimState.Left:
			this.transform.Translate(new Vector3(-m_speed*Time.fixedDeltaTime,0,0));
			break;
		case AnimState.Down:
			this.transform.Translate(new Vector3(0,-m_speed*Time.fixedDeltaTime,0));
			break;
		default:
			break;
		}

	}

	//DestroyGameObject is called in die animation
	void DestroyGameObject(){
		Destroy (gameObject);
		die = true;
	}

	//For the skelet that can revive
	void skeletDestroy(){

		if (die == false) {
			die=true;
			anim.SetTrigger ("revive");
		} else {
			Destroy(gameObject);
		}
	}

	void skeletRevive(){
		//Debug.Log("revive");
		//judge the rotation then set the trigger
		Vector3 vec = m_currentNode.transform.position - this.transform.position;
		float angle = Mathf.Atan2(vec.y,vec.x) * Mathf.Rad2Deg;//vector.right as the direction
		//Debug.Log (angle);
		if (angle >= -180 && angle < -135) {
			anim.SetTrigger("reviveleft");
			m_state=AnimState.Left;
			angle+=180;
		}
		else if (angle >= -135 && angle < -45) {
			anim.SetTrigger("revivefront");
			m_state=AnimState.Down;
			angle+=90;
		}
		else if (angle >= -45 && angle < 45) {
			anim.SetTrigger("reviveright");
			m_state=AnimState.Right;
		}
		else if (angle >= 45 && angle < 135) {
			anim.SetTrigger("reviveback");
			m_state=AnimState.Up;
			angle-=90;
		}else if(angle>=135 && angle<180){
			anim.SetTrigger("reviveleft");
			m_state=AnimState.Left;
			angle-=180;
		}
		transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);//along axis Z rotate angle degree
		//health return to initial status
		m_life = m_maxlife;
		m_speed = speedIni;
		//create a health bar
		m_bar = (GameObject) Instantiate (healthbar,
		                                  new Vector3 (this.transform.position.x - 0.5f, this.transform.position.y + 0.6f, 0),
		                                  Quaternion.identity);
		m_bar.transform.parent = this.transform;
		m_barController = m_bar.transform.FindChild("above").GetComponent<healthBarController> ();
		m_barController.UpdateHealthBar (m_life * 1.0f / m_maxlife);
	}

	void ClickItem(){
		if(Input.GetMouseButtonDown(0)){
			Vector3 v3=Camera.main.ScreenToWorldPoint(Input.mousePosition); 
			Vector2 v2=new Vector2(v3.x,v3.y);
			RaycastHit2D hit=Physics2D.Raycast(v2,Vector2.zero);
			if(hit.collider!=null)
			{
				if(hit.collider.gameObject==this.gameObject){
					RecordController.Operation op=new RecordController.Operation();
					op.time=RecordController.Instance.timer;
					op.type=RecordController.OperationType.Enemy;
					op.posX=v2.x;
					op.posY=v2.y;

					if(!Settings.Instance.replay){
						EnemyOnClick();
					}

					RecordController.Instance.ops.Add(op);
				}
			}else{
				Debug.Log("not hitted");
			}	
		}
	}
	public void EnemyOnClick(){
		foreach(EnemyController enemy in GameManager.Instance.m_EnemyList){
			enemy.clicked=false;
		}
		this.clicked=true;
	}
}
