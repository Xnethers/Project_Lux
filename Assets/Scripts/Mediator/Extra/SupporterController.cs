using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using EZCameraShake;
using UnityEngine.UI;
public class SupporterController : ICareerController {
	private KICareer ki;
	public ActorController AC{get{return ac;}}
	// private FieldOfViewHeight fovh;
	public CameraShake cameraShake;
	[Header("===== Audio Settings =====")]
	public AudioClip normalSlash;
	public AudioClip changeBuff;
	public AudioClip airGround;
	public AudioClip Qstart;
	public AudioClip Qend;
	public AudioClip Qlightning;
	public AudioClip forceSlash;
	public AudioClip storage;

	[Header("===== Others VFX Settings =====")]
	public GameObject VFX_Borislav_Qstart;
    public GameObject VFX_Borislav_Q;
	public GameObject shakeVFX;
	public GameObject changeBuffVFX;
	public GameObject[] buffsVFX;
	[Header("===== Buff Settings =====")]
	public GameObject[] buffObj;
	public float atkBuff = 1.5f;//Attack
    public float detBuff = 1.5f;//Defense
    public float hotBuff = 2.0f;//Heal Over Time
	public enum BuffType
	{
		ATKBuff, DEFBuff,HOTBuff 
	}
	public BuffType buffType;
	public Text buffText;
	[Space(10)]
    [Header("===== Shooter Settings =====")]
    [SerializeField] float ThrowerPower;
	public bool isForce;
	[Space(10)]
    [Header("===== Absorb Settings =====")]
	public float absorbDamage=0f;
	
	// Use this for initialization
	void Start () {
		muzzleR = transform.DeepFind("MuzzleR");
		ac = GetComponent<ActorController>();
		ki = GetComponent<KICareer>();
		// fovh = GetComponent<FieldOfViewHeight>();
		cameraShake = ac.camcon.GetComponentInParent<CameraShake>();
		foreach(GameObject bvfx in buffsVFX)
			bvfx.SetActive(false);
		buffsVFX[(int)buffType].SetActive(true);
	}
	
	// Update is called once per frame
	void Update () {
		if (ac.pi.isAI)
            return;
        if (!photonView.IsMine)
            return;
		skillMR.Tick();
		skillF.Tick();
        skillQ.Tick();
        skillAir.Tick();
        skillForce.Tick();
        forcingTimer.Tick();
		if(ac.am.sm.isDie){
			LockState();
            return;
		}
		if(ac.pi.isLatent){
			ac.SetBool("isArmour", false);
			return;
		}
		if(ac.CheckStateTag("armour") && CheckCD(skillQ)){//解除霸體
            // ac.SetBool("isArmour", false);
			ki.attackML=true;
        }
		if (ac.canAttack){
			if(ac.CheckStateTag("armour")){//霸體狀態
                if(ac.GetBool("isArmour")){//解除霸體
                    if (ki.attackML){//霸攻
                        UseSkill(3, careerValue.RushDamage+absorbDamage/4);
						ac.SetBool("isArmour", false);
						skillQ.atkTimer.state = MyTimer.STATE.FINISHED;
                    }
                }
            }
			else
			{
				//蓄力
				if(ki.forcingML && !isForce && ac.am.sm.isLocomotion){
					if(CheckCD(skillForce)){
						ac.SetBool("fullBody",true);
						UseSkill(5,careerValue.ForceMinDamage,"force");
						forcingTimer.Go(careerValue.ForcingCD);
						isForce=true;
						ac.am.sm.isForcingAim=true;
					}
				}
				if(!ac.am.sm.isForcingAim){
					if (ki.attackML){
						if (ac.height>3 && !ac.am.sm.isGround){ //空攻
							if (CheckCD(skillAir))
							{
								ac.gravity = ac.gravityConstant *2 ;
								ac._velocity.y = -ac.gravity;
								ac.SetBool("fullBody",true);
								UseSkill(4,careerValue.AirDamage);
								StartCD(skillAir, careerValue.AirCD);
							}
						}
						else{
							if(!isForce){//普攻
								ac.SetBool("fullBody",false);
								UseSkill(0,careerValue.NormalDamage,"attack",true);
							}
								
						}
					}
					if(ac.am.sm.isLocomotion){
						if (ki.auxiliaryMR){//群攻
							if(CheckCD(skillMR)){
								ac.SetBool("fullBody",false);
								UseSkill(1,careerValue.FirstDamage);
								StartCD(skillMR,careerValue.FirstCD);
							}
						}
						if(!ac.am.sm.isJump && !ac.am.sm.isFall){
							if (ki.attackF){
								if(CheckCD(skillF)){
									ac.SetBool("fullBody",true);
									UseSkill(2,careerValue.SecondDamage);
									StartCD(skillF,careerValue.SecondCD);
									photonView.RPC("RPC_Buff", RpcTarget.All,0);
								}  
							}
							if(ki.attackQ && ac.am.sm.RP>=100){//開啟霸體
								StartCD(skillQ,careerValue.RushingCD);
								// photonView.RPC("PS_creatQEffect", RpcTarget.All);
								ac.SetBool("isArmour", true);
								ac.am.sm.RP=0;
							}
						}
					}
				}
			}
		}
		//自動發射蓄力
        if(forcingTimer.state == MyTimer.STATE.FINISHED || !ki.forcingML){
            ki.forceReleaseML=true;
            forcingTimer.state = MyTimer.STATE.IDLE;
        }
        if(ki.forceReleaseML)
        {
            if(ac.CheckState("forcing")){
                UseSkill(5,ac.am.sm.ATK);
                StartCD(skillForce,careerValue.ForceCD);
                ac.am.sm.isForcingAim=false;
				SoundManager.Instance.StopEffectSound();
            }
        } 
		if(ki.attackML)
            isForce=false;
	}
	[PunRPC]
	public override void LockState(){
		base.LockState();
		ac.SetBool("isArmour", false);
		photonView.RPC("InitializeAbsorbDamage", RpcTarget.All);
		photonView.RPC("DisableAbsorbRange", RpcTarget.All);
		photonView.RPC("DisableBuffRange", RpcTarget.All);
	}
	public override void FirstAttack(){
		if (!photonView.IsMine)
            return;
		photonView.RPC("RPC_ChangeBuffType", RpcTarget.All);
		// buffText.text = buffType.ToString();
	}
    public void OnAttack1hAUpdate() {
        ac.thrustVec = ac.model.transform.forward * ac.anim.GetFloat("attack1hAVelocity");
        //anim.SetLayerWeight(anim.GetLayerIndex("attack"), Mathf.Lerp(anim.GetLayerWeight(anim.GetLayerIndex("attack")), lerpTarget, 0.4f));
    }
	public void OneAttack(){
		SoundManager.Instance.PlayEffectSound(normalSlash);
		Debug.Log("第一連擊");
		if (!photonView.IsMine)
            return;
		photonView.RPC("RPC_NearProjectile", RpcTarget.All,transform.position, 0);
	}
	public void TwoAttack(){
		SoundManager.Instance.PlayEffectSound(normalSlash);
		Debug.Log("第二連擊");
		if (!photonView.IsMine)
            return;
		photonView.RPC("RPC_NearProjectile", RpcTarget.All,transform.position, 1);
	}
	public void ThreeAttack(){
		SoundManager.Instance.PlayEffectSound(normalSlash);
		Debug.Log("第三連擊");
		if (!photonView.IsMine)
            return;
		photonView.RPC("RPC_NearProjectile", RpcTarget.All,transform.position, 2);
	}
	public void OnArmourEnter(){
		ki.inputEnabled = false;
		ac.canAttack=false;
	}
	public void OnArmourExit(){
		ki.inputEnabled = true;
		ac.canAttack=true;
		ac.anim.SetInteger("attackSkill", -1);
	}
	public void QstartVFX(){
		
	}
	[PunRPC]
	public void DisableBuffRange(){
		FieldOfViewBuff fovb=GetComponentInChildren<FieldOfViewBuff>();
		if(fovb!=null)
			fovb.Disable();
	}
	[PunRPC]
	public void DisableAbsorbRange(){
		FieldOfViewAbsorb fova=GetComponentInChildren<FieldOfViewAbsorb>();
		if(fova!=null)
			fova.Disable();
	}
	[PunRPC]
	public void InitializeAbsorbDamage(){
		absorbDamage=0;
	}
	public void RushArmourAttack(){
        SoundManager.Instance.PlayEffectSound(Qend);
        if (!photonView.IsMine)
            return;
		photonView.RPC("RPC_NearProjectile", RpcTarget.All,transform.position+transform.forward*2.12f, ac.anim.GetInteger("attackSkill"));//3
        
        //收刀動作
        // Invoke("ArmourIsPull",pullTime+.5f);
        // Invoke("ArmourNoPull",pullTime+1f);
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
		ac.lockPlanar=false;
		ki.InputInitialize();
		ki.inputEnabled = false;
		if (!photonView.IsMine)
            return;
		photonView.RPC("RPC_NearProjectile", RpcTarget.All,transform.position, ac.anim.GetInteger("attackSkill"));//4
		// photonView.RPC("RPC_ShakeAttack", RpcTarget.All);
		
	}
	public void AirAttackVFX(){
		SoundManager.Instance.PlayEffectSound(airGround);
		Instantiate(shakeVFX,transform.position,transform.rotation);
	}
	public void LockInput(){
		ac.pi.inputEnabled=false;
		ac.pi.inputMouseEnabled=false;
	}
	public void OnForcingEnter() {
        ki.inputEnabled = false;
		ac.camcon.isHorizontalView=true;
		SoundManager.Instance.PlayEffectSound(storage);
		float t=0.1f;
        for(int i =0;i<7;i++){
            Invoke("Invoke_AddATK",t);
            t+=0.1f;
        }
    }
	public void Invoke_AddATK()
    {
        if(ki.forcingML){
            ac.am.sm.ATK += careerValue.ForceDifferenceDamage;
        }
    }
	public override void ForceAttack()//蓄力
	{
		SoundManager.Instance.PlayEffectSound(forceSlash);
		if (!photonView.IsMine)
            return;
		photonView.RPC("RPC_Projectile", RpcTarget.All,muzzleR.position,new Vector3(RayAim().x,muzzleR.position.y,RayAim().z) ,ThrowerPower);//muzzleR.position, RayAim()
    }
	public void OnForceAttackExit(){
		ac.camcon.isHorizontalView=false;
		ac.canAttack=true;
	}
	
	public void OnRunUpdate() {
        ac.anim.SetLayerWeight(ac.anim.GetLayerIndex("run"), ac.anim.GetLayerWeight(ac.anim.GetLayerIndex("attack")));
    }
	public  void createQstartEffect(){
		GameObject vfx = Instantiate(VFX_Borislav_Qstart,transform.position,transform.rotation);
		vfx.transform.parent=transform;
	}
	public void QstartSound(){
		SoundManager.Instance.PlayEffectSound(Qstart);
	}
	[PunRPC]
	public void RPC_ChangeBuffType(){
		foreach(GameObject bvfx in buffsVFX)
			bvfx.SetActive(false);
		int type = (int)buffType;
		type++;
		if(type>System.Enum.GetNames (buffType.GetType ()).Length-1){
			type=0; 
		} 
		Debug.Log(type);
		buffType = (BuffType)type;
		GameObject vfx = Instantiate(changeBuffVFX,transform.position,transform.rotation);
		vfx.transform.parent=transform;
		SoundManager.Instance.PlayEffectSound(changeBuff);
		buffsVFX[type].SetActive(true);
	}
	[PunRPC]
	public void RPC_Buff(int fovSkill){
		GameObject buff = Instantiate(buffObj[fovSkill], transform.position, transform.rotation) as GameObject;
		buff.transform.parent=transform;
		FieldOfView fov = buff.GetComponent<FieldOfView>();
		fov.Initialize(ac.am);
	}
	[PunRPC]
	public void AddAbsorbDamage(float damage){
		if (!photonView.IsMine)
            return;
		absorbDamage+=damage;
	}
    void creatQingEffect()
    {
		SoundManager.Instance.PlayEffectSound(Qlightning);
        GameObject vfx = Instantiate(VFX_Borislav_Q,muzzleR);
		RPC_Buff(1);
    }
	void destoryQingEffect()
    {
		SoundManager.Instance.StopEffectSound();
		DestoryPS vfx = muzzleR.GetComponentInChildren<DestoryPS>();
		if(vfx!=null)
        	Destroy(vfx.gameObject);
    }
}
