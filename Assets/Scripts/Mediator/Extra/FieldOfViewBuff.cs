using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldOfViewBuff : FieldOfView {
	public SupporterController supController;
	[SerializeField] protected float timeToLive = 10f;
	public override void Initialize(ActorManager am){
		tag=am.tag;
		supController = am.GetComponent<SupporterController>();
		StartFind(.2f);
		Invoke("Disable",timeToLive);
	}
	public override void FindUseTargets(){
		BuffInitialize();
		useTargets.Clear();
		for (int i = 0; i < visibleTargets.Count; i++){
			ActorManager tempAm=visibleTargets[i];
			if(tempAm.tag == tag){
				if(supController.buffType == SupporterController.BuffType.ATKBuff){
					tempAm.sm.sb.SetBuffValue(supController.atkBuff,1,1);
				}
				else if(supController.buffType == SupporterController.BuffType.DEFBuff){
					tempAm.sm.sb.SetBuffValue(1,supController.detBuff,1);
				}
				else if(supController.buffType == SupporterController.BuffType.HOTBuff){
					tempAm.sm.sb.SetBuffValue(1,1,supController.hotBuff);
				}
				useTargets.Add(tempAm);
			}
		}
	}
	public void BuffInitialize(){
		foreach(ActorManager tempAm in useTargets)
		{
			tempAm.sm.sb.SetBuffValue(1,1,1);
		}
	}
	public void Disable(){
		BuffInitialize();
		Destroy(gameObject);
	}
}
