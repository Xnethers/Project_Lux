using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using EZCameraShake;

public class TankController : ICareerController {
	private KICareer ki;
	private FieldOfViewHeight fovh;
	public CameraShake cameraShake;
	[Header("===== Weapon Settings =====")]
    public Transform handBone;
	

	// Use this for initialization
	void Start () {
		muzzleR = transform.DeepFind("MuzzleR");
		ac = GetComponent<ActorController>();
		ki = GetComponent<KICareer>();
		fovh = GetComponent<FieldOfViewHeight>();
		cameraShake = ac.camcon.GetComponentInParent<CameraShake>();
		NeedleHand();
	}
	
	// Update is called once per frame
	void Update () {
		if (ac.pi.isAI)
            return;
        if (!photonView.IsMine)
            return;
		if(ac.am.sm.isDie){
			skillQ.atkTimer.state=MyTimer.STATE.IDLE;
            return;
		}
		if(ac.pi.isLatent)
			return;
		if (ac.canAttack){
			if (ki.attackML){
				if (ac.anim.GetBool("isHighFall") && !ac.anim.GetBool("isGround")){ //空攻
					ac.gravity = ac.gravityConstant *2 ;
					ac._velocity.y = -ac.gravity;
					
					UseSkill(4,careerValue.AirDamage);
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
		// ki.inputMouseEnabled = false;
        //lockPlanar = true;
        //lerpTarget = 1.0f;
        
    }
    public void OnAttack1hAUpdate() {
        ac.thrustVec = ac.model.transform.forward * ac.anim.GetFloat("attack1hAVelocity");
        //anim.SetLayerWeight(anim.GetLayerIndex("attack"), Mathf.Lerp(anim.GetLayerWeight(anim.GetLayerIndex("attack")), lerpTarget, 0.4f));
    }
	public void OneAttack(){
		Debug.Log("第一連擊");
		if (!photonView.IsMine)
            return;
		photonView.RPC("RPC_NearProjectile", RpcTarget.All,muzzleR.position, 0);
	}
	public void TwoAttack(){
		Debug.Log("第二連擊");
		if (!photonView.IsMine)
            return;
		photonView.RPC("RPC_NearProjectile", RpcTarget.All,muzzleR.position, 1);
	}
	public void ThreeAttack(){
		Debug.Log("第三連擊");
		if (!photonView.IsMine)
            return;
		photonView.RPC("RPC_NearProjectile", RpcTarget.All,muzzleR.position, 2);
	}
	//AirAtk
	public void OnAirAttackEnter(){
		fovh.TargetsListClear();
		fovh.StartFind();
		ki.inputEnabled = false;
		// ki.inputMouseEnabled = false;
	}
	public override void AirAttack()//空技
	{
		if (!photonView.IsMine)
            return;
		photonView.RPC("RPC_ShakeAttack", RpcTarget.All);
	}
	public void OnAirAttackExit(){
		fovh.StopFind();
	}
	[PunRPC]
	public void RPC_ShakeAttack(){
		// CameraShaker.Instance.ShakeOnce(4f,5f,.1f,1f);
		// StartCoroutine(cameraShake.Shake(cameraShake.duration,cameraShake.magnitude));//.15 .4
		foreach(ActorManager targetAm in fovh.sameHeightTargets){
			// Debug.Log(targetAm.gameObject.name);
			targetAm.TryDoDamage(ac.am.sm.GetATK(ac.am.sm.ATK));
			targetAm.SendMessage("SetAllDeBuff", new DamageBuff(false, false, false,true));
		}
	}
}
