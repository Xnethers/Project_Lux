using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SupporterAirATK : Projectile {
	private FieldOfViewAttack fova;
	public override void Initialize(ActorManager am,float speed,Vector3 targetPoint){
		base.Initialize(am,speed,targetPoint);
		fova = GetComponent<FieldOfViewAttack>();
	}
	void Update()
	{
		if(am.ac.height<1f || am.ac.anim.GetBool("isGround")){
			if(!isHit){
				am.sm.sb.AddBuffsByStrings(buffsName);
				if(fova.useTargets.Count!=0){
					Attack();
					isHit=true;
				}
			}
			if(!isVFX){//am.ac.height<0.15
				GameObject vfx;
				if(am.ac.height<1f)
					vfx = Instantiate(normalhitVFX,transform.GetChild(0).position,am.transform.rotation);//am.GetComponent<BossAIController>().targetAir
				if(am.ac.anim.GetBool("isGround"))
					vfx = Instantiate(normalhitVFX,transform.position,am.transform.rotation);
				// vfx.transform.parent = am.transform;
				isVFX=true;
			}
			else{
				if (transform.parent != null)
					Destroy(this.transform.parent.gameObject, 0.5f);
			}
		}
	}
	public override void OnTriggerEnter(Collider col){}
	public void Attack(){
		foreach(ActorManager viewAm in fova.useTargets){
			// Debug.Log(targetAm.gameObject.name);
			SendTryDoDamage(viewAm.bm.bcB.defCol);
		}
	}
}
