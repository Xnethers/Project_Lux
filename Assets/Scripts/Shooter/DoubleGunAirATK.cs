using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoubleGunAirATK : Projectile {
	private FieldOfView fov;
	public override void Initialize(ActorManager am,float speed,Vector3 targetPoint){
		base.Initialize(am,speed,targetPoint);
		fov = GetComponent<FieldOfView>();
		//Destroy(gameObject);
	}
	void Update()
	{
		if(!isHit){
			if(fov.visibleTargets.Count!=0){
				Attack();
				isHit=true;
			}
		}
	}
	public override void OnTriggerEnter(Collider col){}
	public void Attack(){
		foreach(ActorManager viewAm in fov.visibleTargets){
			if(viewAm.tag !=this.tag)
				SendTryDoDamage(viewAm.bm.bcB.defCol);
			else{
				viewAm.sm.sb.AddBuffsByStrings(buffsName);
				if(viewAm == am)
					viewAm.ac.attackerVec = -am.transform.forward;
				else
					viewAm.ac.attackerVec = am.transform.forward;
			}
		}
	}
}
