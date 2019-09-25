using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class TankController : ICareerController {
	private KICareer ki;
	[Header("===== Weapon Settings =====")]
    public Transform handBone;
	

	// Use this for initialization
	void Start () {
		muzzleR = transform.DeepFind("MuzzleR");
		ac = GetComponent<ActorController>();
		ki = GetComponent<KICareer>();
		NeedleHand();
	}
	
	// Update is called once per frame
	void Update () {
		if (ac.pi.isAI)
            return;
        if (!photonView.IsMine)
            return;
		if (ac.canAttack){
			if (ki.attackML){
				if (ac.anim.GetBool("isHighFall") && !ac.anim.GetBool("isGround")){ //空攻
					// RayAim();
					// if(!rayhitAirWall){
					// 	if(CheckCD(skillAir)){
					// 		UseSkill(4,careerValue.AirDamage) ;
					// 		StartCD(skillAir,careerValue.AirCD);
					// 	}
					// }
				}
				else{
					// if(!isForce)//普攻
						UseSkill(0,careerValue.NormalDamage,"attack",true);
				}
			}
		}
	}
	public void NeedleHand() {
        BigNeedleSetParent(handBone);
    }
	public void BigNeedleSetParent(Transform targetPoint){
        ac.am.wm.wcR.wdata[0].transform.parent=targetPoint;
        ac.am.wm.wcR.wdata[0].transform.localPosition = Vector3.zero;
        ac.am.wm.wcR.wdata[0].transform.localRotation = Quaternion.identity;
    }
	public void OnAttack1hAEnter() {
        ki.inputEnabled = false;
        //lockPlanar = true;
        //lerpTarget = 1.0f;
        
    }
    public void OnAttack1hAUpdate() {
        ac.thrustVec = ac.model.transform.forward * ac.anim.GetFloat("attack1hAVelocity");
        //anim.SetLayerWeight(anim.GetLayerIndex("attack"), Mathf.Lerp(anim.GetLayerWeight(anim.GetLayerIndex("attack")), lerpTarget, 0.4f));
    }
	public void One(){
		Debug.Log("第一連擊");
		photonView.RPC("RPC_NearProjectile", RpcTarget.All,muzzleR.position,0);
	}
	public void Two(){
		Debug.Log("第二連擊");
		photonView.RPC("RPC_NearProjectile", RpcTarget.All,muzzleR.position,1);
	}
	public void Three(){
		Debug.Log("第三連擊");
		photonView.RPC("RPC_NearProjectile", RpcTarget.All,muzzleR.position,2);
	}
}
