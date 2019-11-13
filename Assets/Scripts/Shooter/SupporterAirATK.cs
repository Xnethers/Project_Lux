﻿using System.Collections;
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
		if(!isHit && am.ac.height<1){
			AddBuffs(am.gameObject);
			if(fova.useTargets.Count!=0){
				Attack();
				isHit=true;
			}
		}
	}
	public override void OnTriggerEnter(Collider col){}
	public void Attack(){
		foreach(ActorManager targetAm in fova.useTargets){
			// Debug.Log(targetAm.gameObject.name);
			targetAm.SendMessage("TryDoDamage",GetATK());
			AddBuffs(targetAm.gameObject);
		}
	}
}