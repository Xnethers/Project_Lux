﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldOfViewTower : FieldOfView {
	// Use this for initialization
	void Start () {
		StartFind(.2f);
	}
	public override void FindUseTargets(){
		useTargets.Clear();
		
		for (int i = 0; i < visibleTargets.Count; i++){
			ActorManager tempAm=visibleTargets[i];
			if(tempAm.tag !=tag){
				useTargets.Add(tempAm);
			}
		}
	}
}
