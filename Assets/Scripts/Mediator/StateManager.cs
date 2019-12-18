using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

/*根據ActorController輸出的資訊管理角色狀態 */
public class StateManager : IActorManagerInterface, IPunObservable
{

    //public ActorManager am;
    public StateBuff sb;
    public float HPMax = 100f;
    public float HP = 100f;
    public float RPMax = 100f;//RushPoint
    public float RP = 0f;
    public float ATK = 10.0f;//base
    public float HOT = 1.0f;
    public float ATKBuff = 1.0f;//Attack
    public float DEFBuff = 1.0f;//Defense
    public float HOTBuff = 1.0f;//Heal Over Time

    [Header("1st order state flags")]
    public bool isLocomotion;
    public bool isGround;
    public bool isJump;
    public bool isFall;
    public bool isAttack;
    public bool isRushAttack;
    public bool isForce;
    public bool isForcingAim;
    public bool isHit;
    public bool isRepelled;
    public bool isDie;
    

    //[Header("2nd order state flag")]
    protected Coroutine rpCoroutine;
    private bool isRPing;
    protected Coroutine hpCoroutine;
    public bool isHPing;
    public MyTimer dieTimer = new MyTimer();
    public bool die = false;
    public float deadTime = 5;

    ReliveZone mylivezone;
    [Header("Result Data")]
    public float AllAttack;
    public float AllHurt;
    public int AllKill;
    public int AllDead;
    private void Start()
    {
        sb = GetComponent<StateBuff>();
        //AddHP(0);
        HP = HPMax;
        //RP=0;

        if (am.ac.pi.isAI)
        {
            photonView.RPC("RPC_ReLive", RpcTarget.All, transform.position.x, transform.position.z);
        }
        else
        {
            if (gameObject.tag == "Red")
            {
                mylivezone = GameManager.Instance.RedRelivePoint;
            }
            if (gameObject.tag == "Blue")
            {
                mylivezone = GameManager.Instance.BlueRelivePoint;
            }
            if (photonView.IsMine)
                photonView.RPC("RPC_ReLive", RpcTarget.All, RandomPosition(mylivezone.MinX, mylivezone.MaxX), RandomPosition(mylivezone.MinZ, mylivezone.MaxZ));
        }
    }
    private void Update()
    {
        isLocomotion = am.ac.CheckStateTag("locomotion") && am.ac.CheckStateTag("locomotion","attack") && !isRushAttack;
        isGround = am.ac.CheckState("ground");
        isJump = am.ac.CheckState("jump");
        isFall = am.ac.CheckState("fall");
        //isAttack = am.ac.CheckStateTag("attackR") || am.ac.CheckStateTag("attackL");
        isRushAttack = !am.ac.careercon.CheckCD(am.ac.careercon.skillQ);//lena需收到才算大招結束
        isForce=am.ac.CheckStateTag("force") || am.ac.CheckStateTag("force","attack");
        // isHit = am.ac.CheckState("hit");
        //isDie = am.ac.CheckState("die");
        isRepelled=am.ac.CheckStateTag("repelled");
        if (HP <= 0 && !isDie)
        {
            // Debug.LogError("die");
            if(am.ac.pi.isLatent){
                photonView.RPC("RPC_SetLatent", RpcTarget.All,this.tag);
                if(photonView.IsMine)
                    am.ac.CloseLatentCol();
            }//退出潛光
            // Die();
            am.ac.OnDieEnter();//不能行動
            AllDead+=1;
            if (am.targetAm != null)
            { 
                am.targetAm.sm.AddRP(2); 
                am.targetAm.sm.AllKill+=1;
                // Debug.LogError("addRp+kill");
            }
            isDie = true;
        }
        if(isDie ){//在高處降落至地才死亡動畫
            am.ac.OnDieEnter();//不能行動
            if(!die &&(isGround || am.ac.height<1f)){//isGround
                Die();
                die = true;
            }
        }
        if (!photonView.IsMine)
            return;
        if(Input.GetKeyDown(KeyCode.K)){
            photonView.RPC("RPC_MaxRP", RpcTarget.All);
        }
        // if(!isHPing)
        //     StartAddHp();
        if (!isRPing)
            StartAddRp();
        if (isDie)
        {
            dieTimer.Tick();
            if (dieTimer.state == MyTimer.STATE.FINISHED)
            {
                if (am.ac.pi.isAI)
                {
                    photonView.RPC("RPC_ReLive", RpcTarget.All, transform.position.x, transform.position.z);
                }
                else
                {
                    photonView.RPC("RPC_ReLive", RpcTarget.All, RandomPosition(mylivezone.MinX, mylivezone.MaxX), RandomPosition(mylivezone.MinZ, mylivezone.MaxZ));
                    // if (gameObject.tag == "Red")
                    //     photonView.RPC("RPC_ReLive", RpcTarget.All, RandomPosition(42, 45), RandomPosition(-3, 2));
                    // if (gameObject.tag == "Blue")
                    //     photonView.RPC("RPC_ReLive", RpcTarget.All, RandomPosition(-47, -50), RandomPosition(-7, -2));
                }

                dieTimer.state = MyTimer.STATE.IDLE;
            }
        }
    }
    public float GetATK(float baseATK){
        return baseATK * ATKBuff;
    }
    public void Die()
    {
        am.ac.RPC_SetTrigger("die");
        // am.ac.pi.inputMouseEnabled = false;
        // am.ac.pi.inputEnabled = false;
        //am.ac.camcon.enabled = false;
        AddRP(3);
    }
    public void DeadTiming(){
        dieTimer.Go(deadTime);
    }

    public float RandomPosition(float min, float max)
    {
        float x = Random.Range(min, max);
        return x;
    }
    #region Points
    public void StartAddRp()
    {
        if (rpCoroutine != null)
        {
            StopCoroutine(rpCoroutine);
        }
        rpCoroutine = StartCoroutine(GroundRp(1f));
    }
    IEnumerator GroundRp(float rpValue)
    {
        isRPing = true;
        yield return new WaitForSeconds(1f);
        AddRP(rpValue);
        isRPing = false;
    }
    public void AddRP(float value)
    {
        if (!isDie && !isRushAttack)
        {
            RP += value;
            RP = Mathf.Clamp(RP, 0, RPMax);
        }
    }
    public void StartAddHp(float hpValue,float waitTime = 1f)
    {
        if (!photonView.IsMine)
            return;
        if (hpCoroutine != null)
        {
            StopCoroutine(hpCoroutine);
        }
        hpCoroutine = StartCoroutine(GroundHp(hpValue,waitTime));
    }
    IEnumerator GroundHp(float hpValue,float waitTime)
    {
        isHPing = true;
        yield return new WaitForSeconds(waitTime);
        AddHP(hpValue);
        isHPing = false;
    }
    
    public void AddHP(float value)
    {
        if(!isDie){
            HP += value;
            HP = Mathf.Clamp(HP, 0, HPMax);
        }
        
    }
    #endregion
    #region PUN Callbacks
    [PunRPC]
    public void RPC_ReLive(float x, float z)
    {
        photonView.RPC("RPC_SetTrigger", RpcTarget.All, "reLife");
        HP = HPMax;
        isDie = false;
        die = false;
        if (am.ac.pi.isAI)
            return;
        transform.localPosition = new Vector3(x, mylivezone.Y, z);
        // Debug.LogError(mylivezone);
        // if (am.ac.pi.isAI)
        //     return;
        // am.ac.camcon.enabled = true;
        // if (photonView.IsMine)
        //     Camera.main.transform.position = am.ac.camcon.transform.position;
    }
    // public void RPC_Lock(){
    //     if(am.ac.pi.isLatent)
    //         return;
    //     photonView.RPC("RPC_SetTrigger", RpcTarget.All, "reLife");
    //     photonView.RPC("LockState", RpcTarget.All);
    // }
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // We own this player: send the others our data
            stream.SendNext(HP);
            stream.SendNext(ATK);
            stream.SendNext(RP);
            stream.SendNext(AllAttack);
            stream.SendNext(AllHurt);
            stream.SendNext(AllKill);
            stream.SendNext(AllDead);
        }
        else
        {
            // Network player, receive data
            this.HP = (float)stream.ReceiveNext();
            this.ATK = (float)stream.ReceiveNext();
            this.RP = (float)stream.ReceiveNext();
            this.AllAttack = (float)stream.ReceiveNext();
            this.AllHurt = (float)stream.ReceiveNext();
            this.AllKill = (int)stream.ReceiveNext();
            this.AllDead = (int)stream.ReceiveNext();
        }
    }
    [PunRPC]
    public void RPC_MaxRP(){
        RP=RPMax;
    }
    [PunRPC]
    public void RPC_AddAllAttack(float tempAttack){
        AllAttack+=tempAttack;
    }
    #endregion
}
