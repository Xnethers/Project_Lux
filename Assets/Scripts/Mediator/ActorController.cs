using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

/*animator的按鍵輸入和狀態輸出 */
public class ActorController : IActorManagerInterface {

    public GameObject model;
    public CameraController camcon;
    public IUserInput pi;
    public ICareerController careercon;
    public float walkSpeed = 2.4f;
    public float upSpeed = 1;
    public float runMultiplier = 2.0f;
    public float jumpVelocity = 5.5f;
    public float popUpVelocity = 1.5f;
    //public float repelVelocity = 10f;
    //public float repelDampVelocity = 1f;


    [Space(10)]
    [Header("===== Friction Settings =====")]
    public PhysicMaterial frictionOne;
    public PhysicMaterial frictionZero;

    public Animator anim;
    public bool canAttack;
    
    //private Rigidbody rigid;
    public CharacterController chacon;
    public float gravityConstant = 1.5f;
    public float gravity = 1f;
    public Vector3 _velocity;
    private Vector3 planarVec;
    public Vector3 thrustVec;
    public Vector3 attackerVec;
    
    public bool lockPlanar = false;
    private bool trackDirection = false;
    //private CapsuleCollider col;
    
    public float lerpTarget;
    private Vector3 deltaPos;
    //"===== Bounce Settings ====="彈跳點
    private float bonceVelocity;
    public bool isBounce;
    // Use this for initialization
    public void Awake() {
        IUserInput[] inputs = GetComponents<IUserInput>();
        foreach (var input in inputs)
        {
            if (input.enabled == true) {
                pi = input;
                break;
            }     
        }
        careercon = GetComponent<ICareerController>();
        anim = model.GetComponent<Animator>();
        //rigid = GetComponent<Rigidbody>();
        chacon = GetComponent<CharacterController>();
        //col = GetComponent<CapsuleCollider>();
        //if(!photonView.IsMine)
            //camcon.isAI=true;
        gravity = gravityConstant;
    }
    void Start()
    {
        am.bm.bcL.gameObject.SetActive(false);
    }
    // Update is called once per frame
    protected void Update() {
        if(pi.isAI)
            return;
        if(!photonView.IsMine)
            return;
            
        if(pi.esc)
            camcon.isCursorVisible = ! camcon.isCursorVisible;
        if(pi.latent && am.im.overlapEcastms.Count!=0){//按下潛光按鍵(暫定e鍵)
            if(am.im.overlapEcastms[0].tag == tag){
                
                photonView.RPC("RPC_SetLatent", RpcTarget.All);
                lockPlanar = false;
                camcon.tempEulerX= 0;//攝影機UpDown角度歸零
                pi.inputMouseEnabled = !pi.inputMouseEnabled;//鎖攝影機操作
                //人與潛光平行(轉角度)
                // Debug.Log(am.im.overlapEcastms[0].transform.eulerAngles.y);
                transform.eulerAngles = new Vector3(transform.eulerAngles.x,am.im.overlapEcastms[0].transform.eulerAngles.y-180f,transform.eulerAngles.z);
                //InteractionManager im為偵測有無物件(帶有EventCasterManager)
            }
        }
        
        if (trackDirection == false){
            model.transform.forward = transform.forward;
        }
        else {
            if(planarVec.magnitude < 0.1f ){
                model.transform.forward = transform.forward;
                print("forward");
            }
                
            else{
                model.transform.forward = planarVec.normalized; 
                print("planarVec.normalized");
            }
        }
        if (lockPlanar == false) {
            planarVec = pi.Dvec * walkSpeed * runMultiplier * upSpeed ;//((pi.run) ? runMultiplier : 1.0f
        }
        else{
            //planarVec = (pi.Dvec * walkSpeed * runMultiplier)/1.2f ;
        }
        Vector3 localDvec = transform.InverseTransformVector(pi.Dvec);
        anim.SetFloat("forward", localDvec.z * runMultiplier);
        anim.SetFloat("right", localDvec.x * runMultiplier);
        //anim.SetBool("defense", pi.defense);

        //
        //Input processing
        //
        

        if (pi.jump) {
            photonView.RPC("RPC_SetTrigger",RpcTarget.All,"jump");
        }
    }

    public void SetSpeedup(float upvalue)
    {
        upSpeed = upvalue;
    } 

    protected void FixedUpdate()
    {
        
        if(!photonView.IsMine)
            return;
        //cc.position += deltaPos;
        // //rigid.position += planarVec * Time.fixedDeltaTime;
        // rigid.velocity = new Vector3(planarVec.x, rigid.velocity.y, planarVec.z) + thrustVec;
        
        //取出動畫位移量
        chacon.Move(deltaPos);
        //重力
        // if (anim.GetBool("isGround") && _velocity.y < 0)
        // { _velocity.y = 0f; }
        if(!pi.isLatent){
            if(_velocity.y > -25f)
                _velocity.y += gravity * Physics.gravity.y * Time.fixedDeltaTime;
            if(am.sm.isDie && am.sm.isGround)
                _velocity.y=0;
            //移動
            if (lockPlanar == false && !isBounce)
                chacon.Move((new Vector3(planarVec.x, _velocity.y, planarVec.z) + thrustVec) * Time.fixedDeltaTime);
            else
                chacon.Move((new Vector3(planarVec.x/2f, _velocity.y, planarVec.z/2f) + thrustVec) * Time.fixedDeltaTime);
        }
        else
        {//潛光移動
            chacon.Move(planarVec * Time.fixedDeltaTime);
            // Debug.Log(planarVec);
        }
        
        thrustVec = Vector3.zero;
        deltaPos = Vector3.zero;
    }

    public bool CheckState(string stateName, string layerName ="Base Layer") {
        /*int layerIndex = anim.GetLayerIndex(layerName);
        bool result = anim.GetCurrentAnimatorStateInfo(layerIndex).IsName(stateName);
        return result;*/
        return anim.GetCurrentAnimatorStateInfo(anim.GetLayerIndex(layerName)).IsName(stateName);
    }
    public bool CheckStateTag(string tagName, string layerName = "Base Layer")
    {
        return anim.GetCurrentAnimatorStateInfo(anim.GetLayerIndex(layerName)).IsTag(tagName);
    }
    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer!=LayerMask.NameToLayer("Bounce") && other.gameObject.layer!=LayerMask.NameToLayer("Occupied")){
            _velocity.y = 0f;
            // Debug.Log("no bounce=fall:"+other.name);
        }
        //anim.SetBool("isHighFall", true);
        // Collider[] outputCols = Physics.OverlapBox(am.bm.transform.position , new Vector3(.5f,.2f,.5f),Quaternion.identity, LayerMask.GetMask("Bounce"));
        // if(!pi.jump && outputCols.Length==0){
            
        // }
            
            
    }
    void OnTriggerExit(Collider other)
    {
        if(other.gameObject.layer==LayerMask.NameToLayer("Ground") || other.gameObject.layer==LayerMask.NameToLayer("Wall")){
            if(_velocity.y<0){
                _velocity.y = 0f;
                Debug.Log("fall:"+other.name);
            }
                
        }
    }

    ///
    ///Message processing block 
    /// 
    public void OnLockEnter(){
        // pi.inputEnabled = false;
        // planarVec = Vector3.zero;//速度清0
    }
    public void OnBounceEnter(){
        pi.inputEnabled = true;
        pi.inputMouseEnabled = true;
        lockPlanar = false;
        _velocity=Vector3.zero;
        _velocity.y += Mathf.Sqrt(bonceVelocity * -0.5f * Physics.gravity.y);
        gravity = 1.3f;
        //gravity = 0.1f;
        isBounce=true;
    }
    public void OnJumpEnter() {
        //pi.inputEnabled = false;
        lockPlanar = true;
        //thrustVec = new Vector3(0, jumpVelocity, 0);
        _velocity.y = 1f;//0有飄浮感
        _velocity.y += Mathf.Sqrt(jumpVelocity * -0.5f * Physics.gravity.y);
        //trackDirection = true;
    }
    public void OnPopUpEnter(){
        pi.inputEnabled = false;
        pi.inputMouseEnabled = false;
        lockPlanar = true;
        // thrustVec = new Vector3(0, jumpVelocity, 0);
        _velocity.y = 1f;//0有飄浮感
        //_velocity.y += Mathf.Sqrt(-jumpVelocity * Physics.gravity.y);
        Debug.Log("OnPopUpEnter");
        lerpTarget = 0f;
    }
    public void OnPopUpUpdate(){
        thrustVec = transform.forward * anim.GetFloat("repelVelocity")+transform.up * popUpVelocity * anim.GetFloat("upVelocity");
        pi.inputEnabled = false;
        pi.inputMouseEnabled = false;
    }
    public void OnPopUpExit(){
        pi.inputEnabled = true;
        pi.inputMouseEnabled = true;
    }
    public void IsGround(){
        anim.SetBool("isGround", true);
        gravity = gravityConstant;
        isBounce=false;
    }
    public void IsNotGround()
    {
        anim.SetBool("isGround", false);
    }
    public void IsHighFall(){
        anim.SetBool("isHighFall", true);
    }
    public void IsNotHighFall(){
        anim.SetBool("isHighFall", false);
    }
    public void OnGroundEnter() {
        if(CheckState("attackIdle","attack")){//攻擊完才可操作
            pi.inputEnabled = true;
            pi.inputMouseEnabled = true;
        }
        
        lockPlanar = false;
        canAttack = true;
        am.bm.bcB.defCol.material = frictionOne;
        trackDirection = false;
    }
    public void OnGroundExit() {
        am.bm.bcB.defCol.material = frictionZero;
    }
    public void OnFallEnter(){
        pi.inputEnabled = false;
        lockPlanar = true;
        if(!isBounce)
            _velocity.y = 0f;
        //gravity = .8f;
    }
    /* public void OnJabEnter()
    {
        //pi.inputEnabled = false;
        lockPlanar = true;
        _velocity.y = 0f;
        //_velocity.y += Mathf.Sqrt(jabVelocity * -0.5f * Physics.gravity.y);
    }
    public void OnJabUpdate()
    {
        // if(anim.GetFloat("forward")<-0.1f){
        //     //cc.Move(model.transform.forward * anim.GetFloat("jabVelocity"));
        //     //thrustVec = model.transform.forward * anim.GetFloat("jabVelocity");
            
        // }
            
        // if(anim.GetFloat("right")<-0.1f)
        //     thrustVec = model.transform.right * anim.GetFloat("jabVelocity");
    }*/
    public void OnAttackIdleEnter() {
        lerpTarget = 0f;
        pi.inputEnabled = true;
        pi.inputMouseEnabled = true;
        canAttack=true;
    }
    public void OnAttackIdleUpdate() {
        anim.SetLayerWeight(anim.GetLayerIndex("attack"), Mathf.Lerp(anim.GetLayerWeight(anim.GetLayerIndex("attack")), lerpTarget, .1f));
    }
    public void OnAttackEnter() {
        //pi.inputEnabled = false;
        //lockPlanar = true;
        lerpTarget = 1.0f;
        
    }
    public void OnAttackUpdate() {
        //thrustVec = model.transform.forward * anim.GetFloat("attack1hAVelocity");
        anim.SetLayerWeight(anim.GetLayerIndex("attack"), Mathf.Lerp(anim.GetLayerWeight(anim.GetLayerIndex("attack")), lerpTarget, 0.1f));
    }

    public void OnAttackExit() {
        canAttack=true;
    }

    public void OnHitEnter(){
        pi.inputEnabled = false;
        planarVec = Vector3.zero;//速度清0
        // attackerVec = am.targetAm.transform.forward;
        
    }
    public void OnHitRepelUpdate(){
        //thrustVec = attackerVec * Mathf.Lerp(0,repelVelocity,repelDampVelocity);//transform.forward
        thrustVec = -attackerVec * anim.GetFloat("repelVelocity");
        //_velocity.y = Mathf.Sqrt(anim.GetFloat("repelVelocity") * 0.5f * Physics.gravity.y);
    }
    public void OnDieEnter() {
        pi.inputEnabled = false;
        pi.inputMouseEnabled = false;
        planarVec = Vector3.zero;
        canAttack =false;
        lerpTarget = 0f;
    }
    
    public void OnUpdateRM(object _deltaPos) {
        //print((Vector3)_deltaPos);
        if (CheckState("attack1hC"))
        {
            deltaPos += (0.8f * deltaPos + 0.2f * (Vector3)_deltaPos) / 1.0f;
        }
        
    }
    [PunRPC]
    public void SetBool(string boolName, bool value) {
        anim.SetBool(boolName, value);
    }
    [PunRPC]
    public void RPC_SetTrigger(string triggerName){
        anim.SetTrigger(triggerName);
    }
    [PunRPC]
    public void RPC_SetLatent(){
        pi.isLatent = ! pi.isLatent;//是否潛光中
        SetBool("lock",pi.isLatent);//鎖人物動作狀態
        am.bm.bcL.gameObject.SetActive(pi.isLatent);
        am.bm.SetChacontrollerSize(pi.isLatent);
        model.transform.GetChild(0).gameObject.SetActive(!pi.isLatent);
        // foreach(GameObject mesh in model.GetComponentsInChildren<GameObject>()){}
    }
    public void BounceTrigger(float bonceVelocity){
        this.bonceVelocity = bonceVelocity;
        isBounce=true;
        if(!photonView.IsMine)
            return;
        photonView.RPC("RPC_SetTrigger",RpcTarget.All,"bounce");
    }
}
