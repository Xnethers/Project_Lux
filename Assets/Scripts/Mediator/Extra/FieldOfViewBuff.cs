using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldOfViewBuff : FieldOfView {
	public TankController tankController;
	[SerializeField] protected float timeToLive = 10f;
	public void Initialize(TankController tank){
		
		tankController=tank;
		tag=tankController.tag;
		StartFind();
		Invoke("Disable",timeToLive);
	}
	public override void FindUseTargets(){
		useTargets.Clear();
		for (int i = 0; i < visibleTargets.Count; i++){
			ActorManager tempAm=visibleTargets[i];
			if(tempAm.tag == tag){
				if(tankController.buffType == TankController.BuffType.ATKBuff){
					tempAm.sm.SetBuff(tankController.atkBuff,1,1);
				}
				else if(tankController.buffType == TankController.BuffType.DEFBuff){
					tempAm.sm.SetBuff(1,tankController.detBuff,1);
				}
				else if(tankController.buffType == TankController.BuffType.HOTBuff){
					tempAm.sm.SetBuff(1,1,tankController.hotBuff);
				}
				useTargets.Add(tempAm);
			}
		}
	}
	public void Disable(){
		foreach(ActorManager tempAm in useTargets)
		{
			tempAm.sm.SetBuff(1,1,1);
		}
		Destroy(gameObject);
	}
}
