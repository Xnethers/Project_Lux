using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class NeedlerController : ICareerController
{
    private KICareer ki;

    [Space(10)]
    [Header("===== Shooter Settings =====")]
    [SerializeField] float ThrowerPower;
    [Space(10)]
    [Header("===== Throw Settings =====")]
    public Transform target;
    public Transform curve_point;
    public float pullTime = 1f;
    private float time=0.0f;
    [Space(10)]
    [Header("===== AirATK Settings =====")]
    private Transform airLine;
    [SerializeField] float BackMove = 1.5f;
    public bool isAirAttack;
    public Vector3 targetAir;
    private Vector3 airVelocity;
    public float airDamp = 0.2f;
    public float airLimitTime=0.5f;
    private float airTime=0f;
    private DrawLine drawLine ;
    public bool isForce;
    public GameObject obj ;
    [Header("===== Weapon Settings =====")]
    public Transform VFX_Lena_SwordTrail;
    public Transform waistBone;
    public Transform handBone;
    [Header("===== Others VFX Settings =====")]
    public GameObject AccumulateVFX;
    public GameObject VFX_Lena_Q;
    [Header("===== AudioClip Settings =====")]
    public AudioClip gunFire;
    public AudioClip repelAttack;
    void Start()
    {
        muzzle = transform.DeepFind("Muzzle");
        muzzleR = transform.DeepFind("MuzzleR");
        airLine = transform.DeepFind("AirLine");
        drawLine = airLine.GetComponent<DrawLine>();
        ac = GetComponent<ActorController>();
        ki = GetComponent<KICareer>();
        VFX_Lena_SwordTrail = transform.DeepFind("VFX_Lena_SwordTrail");
        NeedleWaist();
    }
    void Update()
    {
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
            ac.SetBool("isArmour", false);
            skillQ.atkTimer.state=MyTimer.STATE.IDLE;
            return;
        }
        
		if(ac.pi.isLatent)
			return;
        if(Input.GetKeyDown(KeyCode.Z)){
            RayAim();
            if(!rayhitAirWall){
                if(CheckCD(skillAir)){
                    UseSkill(4,careerValue.AirDamage) ;
                    StartCD(skillAir,careerValue.AirCD);
                }
            }
        }
        if(CheckCD(skillQ)){//解除霸體
            ac.SetBool("isArmour", false);
        }
        if (ac.canAttack){//canAttack限制狀態機行為
            if(ac.CheckState("needlerArmour","attack")){//霸體狀態
                if(!CheckCD(skillQ)){//解除霸體
                    if (ki.attackML&& !ac.anim.GetBool("isPull")){//霸攻
                        UseSkill(3, careerValue.RushDamage);
                    }
                }
            }
            else
            {//其他攻
                if (ki.attackML){
                    if (ac.height>3 && !ac.am.sm.isGround){ //空攻
                        RayAim();
                        if(!rayhitAirWall){
                            if(CheckCD(skillAir)){
                                UseSkill(4,careerValue.AirDamage) ;
                                StartCD(skillAir,careerValue.AirCD);
                            }
                        }
                    }
                    else{
                        //if(ac.CheckState("attackIdle","attack"))
                        if(!isForce)//普攻
                            UseSkill(0,careerValue.NormalDamage);
                    }
                }
                    
                if (ki.auxiliaryMR){//群攻
                    if(CheckCD(skillMR)){
                        UseSkill(1,careerValue.FirstDamage);
                        StartCD(skillMR,careerValue.FirstCD);
                    }
                }
                    
                if (ki.attackF){//擊退攻
                    if(CheckCD(skillF)){
                        UseSkill(2,careerValue.SecondDamage);
                        StartCD(skillF,careerValue.SecondCD);
                    }  
                }
                if(ki.attackQ && ac.am.sm.RP>=100){//開啟霸體
                    StartCD(skillQ,careerValue.RushingCD);
                    photonView.RPC("PS_creatQEffect", RpcTarget.All);
                    ac.anim.SetBool("isArmour", true);
                    ac.am.sm.RP=0;
                }
                //蓄力
                if(ki.forcingML && !isForce){
                    if(CheckCD(skillForce)){
                        UseSkill(5,careerValue.ForceMinDamage,"force");
                        forcingTimer.Go(careerValue.ForcingCD);
                        isForce=true;
                        ac.am.sm.isForcingAim=true;
                    }
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
            if(ac.CheckState("forcing","attack")){
                Debug.Log("發動蓄力攻擊!!!!!!!!!!!!!!");
                UseSkill(5,ac.am.sm.ATK);
                StartCD(skillForce,careerValue.ForceCD);
                ac.am.sm.isForcingAim=false;
            }
        } 
        if(ki.attackML)
            isForce=false;
        //空攻移動
        if(isAirAttack){
            // drawLine.SetLineEnabled(true);
            if(this.targetAir != transform.position){
                transform.position = Vector3.SmoothDamp(transform.position, targetAir, ref airVelocity, airDamp);
                ac._velocity.y = 0f;
                airTime+=Time.deltaTime;
            }
        }
            
        if(Vector3.Distance(transform.position,targetAir)<BackMove*2 || airTime>airLimitTime)
        {    
            photonView.RPC("RPC_SetTrigger",RpcTarget.All,"close");
            photonView.RPC("RPC_NeedleCloseAir",RpcTarget.All);
            // drawLine.SetLineEnabled(false);
            // OnBigNeedleEnter();
            // Destroy(obj.gameObject);
            // isAirAttack = false;
            targetAir = Vector3.zero;
            airTime=0f;
        }
        Debug.DrawRay(Camera.main.ScreenToWorldPoint(new Vector3(Screen.width / 2, Screen.height / 2, 3f)), Camera.main.transform.TransformDirection(Vector3.forward) * 100);
    }
    #region animator skill events
    public override void NormalAttack()//ForceAttack
    {
        SoundManager.Instance.PlayEffectSound(gunFire);
        NeedlesLDisable();
        if (!photonView.IsMine)
            return;
        float t=0;
        for(int i = 0;i<3;i++){
            Invoke("FireAttack", t);
            t+=0.1f;
        }
    }
    public void FireAttack()
    {
        photonView.RPC("RPC_Projectile", RpcTarget.All,muzzle.position, RayAim(),ThrowerPower);
    }
    public override void FirstAttack()
    {
        SoundManager.Instance.PlayEffectSound(gunFire);
        NeedlesLDisable();
        if (!photonView.IsMine)
            return;
        FireAttack();
    }
    public override void SecondAttack()//2技
	{
        SoundManager.Instance.PlayEffectSound(repelAttack);
        if (!photonView.IsMine)
            return;
        photonView.RPC("RPC_Projectile", RpcTarget.All,muzzleR.position,RayAim(),0f);
    }
    public void RushArmourAttack(){
        SoundManager.Instance.PlayEffectSound(gunFire);
        BigNeedleRDisable();
        ArmourIsPull();
        Invoke("ArmourNoPull",pullTime);
        if (!photonView.IsMine)
            return;
        photonView.RPC("RPC_Projectile", RpcTarget.All,muzzleR.position, RayAim(),ThrowerPower);
        
        //收刀動作
        // Invoke("ArmourIsPull",pullTime+.5f);
        // Invoke("ArmourNoPull",pullTime+1f);
    }
    public void ArmourIsPull(){
        ac.anim.SetBool("isPull",true);
    }
    public void ArmourNoPull(){
        ac.anim.SetBool("isPull",false);
        BigNeedleREnable();
    }
    public override void AirAttack()//空技
	{ 
        SoundManager.Instance.PlayEffectSound(gunFire);
        BigNeedleRDisable();
        if (!photonView.IsMine)
            return;
        photonView.RPC("RPC_SetTargetAir", RpcTarget.All,RayAim());
        isAirAttack=true;
        //this.targetAir = SetPos(RayAim());
    }
    public override void ForceAttack()//蓄力
	{
        Instantiate(AccumulateVFX,muzzle.position,muzzle.rotation);
        float t=0.1f;
        for(int i =0;i<7;i++){
            Invoke("Invoke_AddATK",t);
            t+=0.1f;
        }
        // isForced =true;
    }
    public void OnDrawEnter(){
        ac.canAttack=false;
    }
    public void OnDrawExit(){
        ac.canAttack=true;
    }
    public void OnAirDrawUpdate(){
        ac.gravity = 0.7f;
    }
    #endregion
    #region others skill funtion
    
    public void Invoke_AddATK()
    {
        if(ki.forcingML){
            ac.am.sm.ATK += careerValue.ForceDifferenceDamage;
        }
    }
    public Vector3 SetPos(Vector3 targetPoint){
        float tempVec3 = Vector3.Distance(transform.position,targetPoint);
        if(tempVec3 >5 && tempVec3<150)
            return targetPoint - transform.forward * BackMove;
        else
            return transform.position;
    }
    // public void OnNeedlesEnter(){
    //     if(ac!=null){
    //         ac.am.wm.wcR.wdata[0].gameObject.SetActive(true);
    //         ac.am.wm.wcR.wdata[1].gameObject.SetActive(false);
    //     }
        
    // }
    // public void OnBigNeedleEnter(){
    //     ac.am.wm.wcR.wdata[0].gameObject.SetActive(false);
    //     ac.am.wm.wcR.wdata[1].gameObject.SetActive(true);
    // }
    public void NeedlesLEnable(){
        ac.am.wm.wcL.wdata[0].gameObject.SetActive(true);
    }
    public void NeedlesLDisable(){
        ac.am.wm.wcL.wdata[0].gameObject.SetActive(false);
    }
    public void BigNeedleREnable(){
        ac.am.wm.wcR.wdata[0].gameObject.SetActive(true);
    }
    public void BigNeedleRDisable(){
        ac.am.wm.wcR.wdata[0].gameObject.SetActive(false);
    }
    public void BigNeedleSetParent(Transform targetPoint){
        ac.am.wm.wcR.wdata[0].transform.parent=targetPoint;
        ac.am.wm.wcR.wdata[0].transform.localPosition = Vector3.zero;
        ac.am.wm.wcR.wdata[0].transform.localRotation = Quaternion.identity;
    }
    public void NeedleHand() {
        BigNeedleSetParent(handBone);
        VFX_Lena_SwordTrail.gameObject.SetActive(true);
    }

    public void NeedleWaist()
    {
        BigNeedleSetParent(waistBone);
        VFX_Lena_SwordTrail.gameObject.SetActive(false);
    }
    #endregion


    #region RPC
    [PunRPC]
    public void RPC_SetTargetAir(Vector3 targetPoint){
        this.targetAir = SetPos(targetPoint);
        obj = Instantiate(projectile[ac.anim.GetInteger("attackSkill")], this.targetAir, transform.rotation) as GameObject;
        obj.transform.LookAt(targetAir);
        drawLine.destination = obj.transform;
        drawLine.SetLineEnabled(true);
    }
    [PunRPC]
     public void RPC_NeedleCloseAir(){
        drawLine.SetLineEnabled(false);
        BigNeedleREnable();
        Destroy(obj.gameObject);
        isAirAttack = false;
        ac.gravity = ac.gravityConstant;
    }
    [PunRPC]
    void PS_creatQEffect()
    {
        Instantiate(VFX_Lena_Q,ac.am.wm.wcR.wdata[0].transform);
    }
    #endregion
}
