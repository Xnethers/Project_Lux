using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankAirATK : Projectile {
	// private TankController tc;
	private FieldOfViewHeight fovh;
	public override void Initialize(ActorManager am,float speed,Vector3 targetPoint){
		base.Initialize(am,speed,targetPoint);
		// tc = am.GetComponent<TankController>();
		fovh = GetComponent<FieldOfViewHeight>();
	}
	void Update()
	{
		if(!isHit && am.ac.height<1){
			if(fovh.useTargets.Count!=0){
				ShakeAttack();
				isHit=true;
			}
		}
	}
	public override void OnTriggerEnter(Collider col){}
	public void ShakeAttack(){
		am.SendMessageUpwards("SetAllDeBuff", new DamageBuff(isBlind, isRepel, isMark,isShake));
		foreach(ActorManager targetAm in fovh.useTargets){
			// Debug.Log(targetAm.gameObject.name);
			targetAm.SendMessageUpwards("TryDoDamage",am.ac.careercon.careerValue.AirDamage*atkBuff);
			targetAm.SendMessageUpwards("SetAllDeBuff", new DamageBuff(isBlind, isRepel, isMark,isShake));
		}
	}
}
