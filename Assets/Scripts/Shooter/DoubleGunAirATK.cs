using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoubleGunAirATK : Projectile {
	private FieldOfViewAttack fova;
	public AudioClip explosion;
	public override void Initialize(ActorManager am,float speed,Vector3 targetPoint){
		base.Initialize(am,speed,targetPoint);
		// fov = GetComponent<FieldOfView>();
		//Destroy(gameObject);
	}
	void Start()
	{
		SoundManager.Instance.PlayEffectSound(explosion);
		fova = GetComponent<FieldOfViewAttack>();
	}
	void Update()
	{
		if(!isHit){
			if(fova.useTargets.Count!=0){
				Attack();
				isHit=true;
			}
		}
	}
	public override void OnTriggerEnter(Collider col){}
	public void Attack(){
		foreach(ActorManager viewAm in fova.useTargets){
			Vector3 direction;
			float distance = Vector3.Distance(new Vector3(transform.position.x,viewAm.transform.position.y,transform.position.y),viewAm.transform.position);
			if(distance<0.5f){
				direction = randomDirection();
			}
			else
			{
				direction = Vector3.Normalize(viewAm.transform.position - transform.position);
				if(distance >=0.5f && distance<1f)
					direction *=1.5f;
				else
					direction *=0.8f;
			}
			viewAm.SendMessage("TryDoDamage",new DamageData(am, GetATK(),buffsName,direction*fova.viewRadius));
		}
	}
	public Vector3 randomDirection(){
		int random = (int)Mathf.Floor(Random.Range(0,8));
		Vector3 direction = Vector3.zero;
		if(random==0)
			direction = transform.forward;
		if(random==1)
			direction = -transform.forward;
		if(random==2)
			direction = transform.right;
		if(random==3)
			direction = -transform.right;
		if(random==4)
			direction = transform.forward*0.5f + transform.right*0.5f;
		if(random==5)
			direction = transform.forward*0.5f - transform.right*0.5f;
		if(random==6)
			direction = -transform.forward*0.5f + transform.right*0.5f;
		if(random==7)
			direction = -transform.forward*0.5f - transform.right*0.5f;
		return direction;
	}
}
