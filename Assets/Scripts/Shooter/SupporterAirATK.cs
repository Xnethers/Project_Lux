using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SupporterAirATK : Projectile {
	private FieldOfViewHeight fovh;
	public override void Initialize(ActorManager am,float speed,Vector3 targetPoint){
		base.Initialize(am,speed,targetPoint);
		fovh = GetComponent<FieldOfViewHeight>();
	}
	void Update()
	{
		if(!isHit && am.ac.height<1){
			if(fovh.useTargets.Count!=0){
				Attack();
				isHit=true;
			}
		}
	}
	public override void OnTriggerEnter(Collider col){}
	public void Attack(){
		AddBuffs(am.gameObject);
		foreach(ActorManager targetAm in fovh.useTargets){
			// Debug.Log(targetAm.gameObject.name);
			targetAm.SendMessage("TryDoDamage",GetATK());
			AddBuffs(targetAm.gameObject);
		}
	}
}
