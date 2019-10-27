using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using EZCameraShake;
using UnityEngine.UI;
public class TankController : ICareerController {
	private KICareer ki;
	private FieldOfViewHeight fovh;
	public CameraShake cameraShake;
	[Header("===== Weapon Settings =====")]
    public Transform handBone;
	[Header("===== Buff Settings =====")]
	public GameObject buffObj;
	public float atkBuff = 1.5f;//Attack
    public float detBuff = 1.5f;//Defense
    public float hotBuff = 2.0f;//Heal Over Time
	public enum BuffType
	{
		ATKBuff, DEFBuff,HOTBuff 
	}
	public BuffType buffType;
	private TankController tankController;
	public Text buffText;
	[Space(10)]
    [Header("===== Shooter Settings =====")]
    [SerializeField] float ThrowerPower;
	public bool isForce;

	// Use this for initialization
	void Start () {
		muzzleR = transform.DeepFind("MuzzleR");
		ac = GetComponent<ActorController>();
		ki = GetComponent<KICareer>();
		tankController = GetComponent<TankController>();
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
		skillF.Tick();
        skillQ.Tick();
        skillAir.Tick();
        skillForce.Tick();
        forcingTimer.Tick();
		if(ac.am.sm.isDie){
			skillQ.atkTimer.state=MyTimer.STATE.IDLE;
            return;
		}
		if(ac.pi.isLatent)
			return;
		if (ac.canAttack){
			if (ki.attackML){
				if (ac.height>3 && !ac.am.sm.isGround){ //空攻
					ac.gravity = ac.gravityConstant *2 ;
					ac._velocity.y = -ac.gravity;
					ac.SetBool("fullBody",true);
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
					if(!isForce){//普攻
						ac.SetBool("fullBody",false);
						UseSkill(0,careerValue.NormalDamage,"attack",true);
					}
						
				}
			}
			if (ki.auxiliaryMR){//群攻
				// UseSkill(1,careerValue.FirstDamage);
				photonView.RPC("RPC_ChangeBuffType", RpcTarget.All);
				buffText.text = buffType.ToString();
			}
			if (ki.attackF){//擊退攻
				if(CheckCD(skillF)){
					ac.SetBool("fullBody",true);
					UseSkill(2,careerValue.SecondDamage);
					photonView.RPC("RPC_Buff", RpcTarget.All);
					StartCD(skillF,careerValue.SecondCD);
				}  
			}
			//蓄力
			if(ki.forcingML){
				if(CheckCD(skillForce)){
					ac.SetBool("fullBody",true);
					UseSkill(5,careerValue.ForceMinDamage,"force");
					forcingTimer.Go(careerValue.ForcingCD);
					isForce=true;
					ac.am.sm.isForcingAim=true;
				}
			}
		}
		//自動發射蓄力
        if(forcingTimer.state == MyTimer.STATE.FINISHED){
            ki.forceReleaseML=true;
            forcingTimer.state = MyTimer.STATE.IDLE;
        }
        if(ki.forceReleaseML)
        {
            if(ac.CheckState("forcing")){
                UseSkill(5,ac.am.sm.ATK);
                StartCD(skillForce,careerValue.ForceCD);
                ac.am.sm.isForcingAim=false;
            }
        } 
		if(ki.attackML)
            isForce=false;
	}
	public void NeedleHand() {
        BigNeedleSetParent(handBone);
    }
	public void BigNeedleSetParent(Transform targetPoint){
        ac.am.wm.wcR.wdata[0].transform.parent=targetPoint;
        ac.am.wm.wcR.wdata[0].transform.localPosition = Vector3.zero;
        ac.am.wm.wcR.wdata[0].transform.localRotation = Quaternion.identity;
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
	/*public void OnAirJumpEnter(){
		// fovh.TargetsListClear();
		// fovh.StartFind();
		ki.inputEnabled = false;
		// ki.inputMouseEnabled = false;
	}
	public void OnAirAttackExit(){
		fovh.StopFind();
	}
	[PunRPC]
	public void RPC_ShakeAttack(){
		// CameraShaker.Instance.ShakeOnce(4f,5f,.1f,1f);
		// StartCoroutine(cameraShake.Shake(cameraShake.duration,cameraShake.magnitude));//.15 .4
		foreach(ActorManager targetAm in fovh.useTargets){
			// Debug.Log(targetAm.gameObject.name);
			targetAm.TryDoDamage(ac.am.sm.GetATK(ac.am.sm.ATK));
			targetAm.SendMessage("SetAllDeBuff", new DamageBuff(false, false, false,true));
		}
	}*/
	public override void AirAttack()//空技
	{
		ki.inputEnabled = false;
		if (!photonView.IsMine)
            return;
		photonView.RPC("RPC_NearProjectile", RpcTarget.All,transform.position, ac.anim.GetInteger("attackSkill"));//4
		// photonView.RPC("RPC_ShakeAttack", RpcTarget.All);
		
	}
	public void OnForcingEnter() {
        ki.inputEnabled = false;
		ac.camcon.isHorizontalView=true;
		// ki.inputMouseEnabled = false;
        //lockPlanar = true;
        //lerpTarget = 1.0f;
        
    }
	public void LockInput(){
		ac.pi.inputEnabled=false;
		ac.pi.inputMouseEnabled=false;
	}
	public override void ForceAttack()//蓄力
	{
		if (!photonView.IsMine)
            return;
		photonView.RPC("RPC_Projectile", RpcTarget.All,muzzleR.position,new Vector3(RayAim().x,muzzleR.position.y,RayAim().z) ,ThrowerPower);//muzzleR.position, RayAim()
    }
	public void OnForceAttackExit(){
		ac.camcon.isHorizontalView=false;
	}
	
	public void OnRunUpdate() {
        ac.anim.SetLayerWeight(ac.anim.GetLayerIndex("run"), ac.anim.GetLayerWeight(ac.anim.GetLayerIndex("attack")));
    }
	
	[PunRPC]
	public void RPC_ChangeBuffType(){
		int type = (int)buffType;
		type++;
		if(type>System.Enum.GetNames (buffType.GetType ()).Length-1){
			type=0; 
		} 
		Debug.Log(type);
		buffType = (BuffType)type;
	}
	[PunRPC]
	public void RPC_Buff(){
		GameObject buff = Instantiate(buffObj, transform.position, transform.rotation) as GameObject;
		buff.transform.parent=transform;
		FieldOfViewBuff fovb = buff.GetComponent<FieldOfViewBuff>();
		fovb.Initialize(tankController);
	}
}
