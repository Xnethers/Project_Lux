using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SupporterForceATK : Projectile {

	public override void OnTriggerEnter(Collider col)
	{
		if (col.GetComponent<DamageHandler>() == null)
        	return;
		SetRangeBuff(col);
		SetHeadBuff(col);
		ActorManager targetAm=col.GetComponentInParent<ActorManager>();
		if(targetAm.ac.pi.isAI){
			base.OnTriggerEnter(col);
		}
		else{
			if (targetAm != null && targetAm.photonView.IsMine)
				base.OnTriggerEnter(col);
		}
	}
}
