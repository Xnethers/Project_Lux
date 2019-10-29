﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankForceATK : Projectile {

	public override void OnTriggerEnter(Collider col)
	{
		if (col.GetComponent<DamageHandler>() == null)
        	return;
		SetRangeBuff(col);
		SetHeadBuff(col);
		ActorManager targetAm=col.GetComponentInParent<ActorManager>();
		if (targetAm != null && targetAm.photonView.IsMine)
			base.OnTriggerEnter(col);
	}
}