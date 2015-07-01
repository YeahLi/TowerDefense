using UnityEngine;
using System.Collections;

public class explosionController : MonoBehaviour {
	public enum ExplosionType
	{
		Arrow,
		Poison,
		Bomb,
		Ice,
		Elec,
	}
	public ExplosionType explosionType=ExplosionType.Arrow;
	Transform trans;

	//For Poison Tower
	public float lastTime=2.0f;
	float timer=1.0f;
	[HideInInspector]
	public EnemyController enemy;
	[HideInInspector]
	public	int damage=0;	
	[HideInInspector]
	public bool enemySetOK=false;

	bool revert=false;
	
	// Use this for initialization
	void Start () {
		trans = this.transform;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if (explosionType == ExplosionType.Poison && enemySetOK) {
			Damaging ();
		} else if (enemySetOK && explosionType == ExplosionType.Ice) {
			Slowing ();
		} 
	}
	void Damaging(){
		if(enemy.m_life<=0){
			//For skelet
			Destroy(gameObject);
		}
		lastTime-=Time.fixedDeltaTime;
		trans.position=enemy.transform.position;
		timer+=Time.fixedDeltaTime;
		if(timer>1){
			enemy.SetDamage(damage);
			timer=0;
		}
		if(lastTime<0){
			Destroy(gameObject);
		}
	}

	void Slowing(){
		if(enemy.m_life<=0){
			//For skelet
			Destroy(gameObject);
		}
		lastTime-=Time.fixedDeltaTime;
		trans.position=new Vector3(enemy.transform.position.x,
		                           enemy.transform.position.y-0.2f,
		                           enemy.transform.position.z);
		if(lastTime<0){
			if(!revert){
				revert=true;
				enemy.m_speed=enemy.m_speed/(1.0f- damage*0.04f);
			}
			Destroy(gameObject);
		}
	}

	public void destroyExplosion(){
		Destroy (gameObject);
	}
}
