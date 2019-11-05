using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldOfViewAbsorb : FieldOfView {
	public ActorManager supAm;
	private SupporterController supController;
	[SerializeField] protected float timeToLive = 6f;
	public override void Initialize(ActorManager am){
		supController=am.GetComponent<SupporterController>();
		tag=am.tag;
		supAm=am;
		StartFind(.1f);
		Invoke("Disable",timeToLive);
	}
	void Update()
	{
		// if(supController.KI.attackML)
		// 	Disable();
	}
	public override void FindUseTargets(){
		AbsorbInitialize();
		useTargets.Clear();
		for (int i = 0; i < visibleTargets.Count; i++){
			ActorManager tempAm=visibleTargets[i];
			if(tempAm.tag == tag){//tempAm != supAm
				tempAm.sm.sb.SetAbsorbed(supAm);
				useTargets.Add(tempAm);
			}
		}
	}
	public void AbsorbInitialize(){
		foreach(ActorManager tempAm in useTargets)
		{
			tempAm.sm.sb.SetAbsorbed(null);
		}
	}
	public void Disable(){
		AbsorbInitialize();
		Destroy(gameObject);
	}
}
