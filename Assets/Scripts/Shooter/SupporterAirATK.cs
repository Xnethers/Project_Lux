using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SupporterAirATK : Projectile {
	private FieldOfViewAttack fova;
	public bool ShakeCam;
	public override void Initialize(ActorManager am,float speed,Vector3 targetPoint){
		base.Initialize(am,speed,targetPoint);
		fova = GetComponent<FieldOfViewAttack>();
	}
	void Update()
	{
		
		if(am.ac.height<1f || am.ac.GetBool("isGround") ){//
			if(!ShakeCam && am.photonView.IsMine){
				Debug.Log("Shake");
				am.sm.sb.AddBuffsByStrings(buffsName);
				ShakeCam = true;
			}
			if(!isHit){
				if(fova.useTargets.Count!=0){
					Attack();
					isHit=true;
				}
			}
		}
		
		// if(!isVFX){//am.ac.height<0.15
		// 	if(am.ac.height<0.1f || am.ac.GetBool("isGround") ){//
		// 		GameObject vfx = Instantiate(normalhitVFX,transform.position,am.transform.rotation);
		// 		// vfx.transform.parent = am.transform;
		// 		isVFX=true;
		// 	}
		// }
		// else{
		// 	if (transform.parent != null)
		// 		Destroy(this.transform.parent.gameObject, 0.5f);
		// }
	}
	public override void OnTriggerEnter(Collider col){}
	public void Attack(){
		foreach(ActorManager viewAm in fova.useTargets){
			// Debug.Log(targetAm.gameObject.name);
			// SendTryDoDamage(viewAm.bm.bcB.defCol);
			viewAm.SendMessage("TryDoDamage",new DamageData(am, GetATK(),buffsName,am.transform.forward));
		}
	}
}
