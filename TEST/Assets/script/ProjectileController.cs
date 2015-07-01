using UnityEngine;
using System.Collections;

public class ProjectileController : MonoBehaviour {
	public enum ProjType
	{
		Arrow,
		Poison,
		Bomb,
		Ice,
		Elec,
	}
	public ProjType projectileType=ProjType.Arrow;
	public float m_speed=20.0f;
	public int damage=5;
	public GameObject explosion;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void FixedUpdate () {
		this.transform.Translate (m_speed*Time.fixedDeltaTime,0,0);
		if (transform.position.x > 14 || transform.position.x < 0 ||
			transform.position.y > 10 || transform.position.y < 0)
		{
			Destroy(gameObject);
		}
	}

	void OnTriggerEnter2D(Collider2D other) {
		if (other.tag.CompareTo ("enemy") == 0) {
			Destroy (gameObject);
			GameObject exp=(GameObject)Instantiate(explosion,other.transform.position,Quaternion.identity);
			//add to enemy
			exp.transform.parent=other.transform;
			if(projectileType==ProjType.Ice){
				exp.transform.position=new Vector3(other.transform.position.x,
				                                   other.transform.position.y-0.2f,
				                                   other.transform.position.z);
			}
			EnemyController enemy=other.GetComponent<EnemyController>();
			if(projectileType==ProjType.Poison||
			   projectileType==ProjType.Ice)
			{
				explosionController expController=exp.GetComponent<explosionController>();
				expController.enemy=enemy;
				expController.enemySetOK=true;
				expController.damage=damage;
				if(projectileType==ProjType.Ice){
					enemy.SlowDown(damage);
					enemy.SetDamage(damage);
				}
			}else{
				if(projectileType==ProjType.Elec){
					return;
				}else{
					enemy.SetDamage(damage);
				}
			}
		}
	}
	
}
