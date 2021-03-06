using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

//Adela
public class DoubleGunController : ICareerController
{
    public GameObject VFX_Adela_B1;
    public GameObject VFX_Adela_gunFire;
    private KICareer ki;

    protected Transform[] muzzles = new Transform[3];
    private bool isleft = true;

    [Space(10)]
    [Header("===== Shooter Settings =====")]
    [SerializeField] float ThrowerPower;

    [Header("===== Fill Settings =====")]
    /* 彈匣 */
    [SerializeField] GameObject Magazine;

    [SerializeField] GameObject Obj_magazine;
    public Transform[] magazinePos = new Transform[3];
    //[0]==左,[1]==右,[2]==F
    public bool isFill;
    public int magazine = 40;
    private int MaxMagazine;

    [Header("===== Force Settings =====")]
    public GameObject[] Beam = new GameObject[2];
    // public List<BodyCollider> APIV = new List<BodyCollider>(); //All Player In View 
    // public List<Collider> allbodycollider;
    // private BattleController[] allbc_obj;
    // private LayerMask mask;
    // public LayerMask ignoreMask;

    // [HideInInspector]
    // public Camera mainCamera;
    // private MyTimer.STATE isRunState;

    public bool isForce;
    [Header("===== AudioClip Settings =====")]
    public AudioClip Adela_Shot;
    public AudioClip Adela_Speedup;
    public AudioClip Adela_Q;
    public AudioClip Adela_Battery;
    public AudioClip repelAttack;
    public AudioClip storage;
    public AudioClip Adela_Leg;
    void Start()
    {
        muzzles[0] = transform.DeepFind("MuzzleL");
        muzzles[1] = transform.DeepFind("MuzzleR");
        muzzles[2] = transform.DeepFind("MuzzleF");
        ac = GetComponent<ActorController>();
        ki = GetComponent<KICareer>();
        foreach (var item in Beam)
        { item.SetActive(false); }
        //StartCoroutine("Timer_Forcing");
        MaxMagazine = magazine;
    }


    void FixedUpdate()
    {
        //target = FindTargetPoint().transform.position;
    }
    void Update()
    {
        if (ac.pi.isAI)
        { return; }
        if (!photonView.IsMine)
        { return; }
        skillMR.Tick();
        skillF.Tick();
        skillQ.Tick();
        skillAir.Tick();
        skillForce.Tick();
        forcingTimer.Tick();
        if (ac.am.sm.isDie)
        {
            LockState();
            return;
        }
        if (ac.pi.isLatent)
            return;
        //Debug.Log(skillML.atkTimer.elapsedTime);
        if (ac.canAttack && !isFill)
        {
            //canAttack限制狀態機行為
            if (ki.attackML)
            {
                if (ac.height > 3 && !ac.am.sm.isGround)
                {
                    if (CheckCD(skillAir))
                    {
                        UseSkill(4, careerValue.AirDamage);
                        StartCD(skillAir, careerValue.AirCD);
                    }
                }

                else
                {
                    if (!isForce)
                    {
                        UseSkill(0, careerValue.NormalDamage);
                        ac.anim.SetBool("isLeft", isleft);
                    }
                }
            }
            if (ki.attackF)
            {
                if (CheckCD(skillF))
                {
                    UseSkill(2, careerValue.SecondDamage);
                    StartCD(skillF, 5);
                }
            }

            if (ki.attackQ && ac.am.sm.RP >= 100)
            {
                UseSkill(3, careerValue.RushDamage);
                SoundManager.Instance.PlayEffectSound(Adela_Q);
                StartCD(skillQ, careerValue.RushingCD);
                ac.am.sm.RP = 0;
            }

            //蓄力
            if (ki.forcingML && !isForce)
            {
                if (CheckCD(skillForce))
                {
                    UseSkill(5, careerValue.ForceMinDamage, "force");
                    forcingTimer.Go(careerValue.ForcingCD);
                    isForce = true;
                    ac.am.sm.isForcingAim = true;
                }
            }
            if (ki.auxiliaryMR)
            {
                if(CheckCD(skillMR)){
                    UseSkill(1, careerValue.FirstDamage);
                    StartCD(skillMR,careerValue.FirstCD);
                }
            }
        }

        if (forcingTimer.state == MyTimer.STATE.FINISHED || !ki.forcingML)
        {
            ki.forceReleaseML = true;
            forcingTimer.state = MyTimer.STATE.IDLE;
        }
        if (ki.forceReleaseML)
        {
            if (ac.CheckState("forcing", "attack"))
            {
                UseSkill(5, ac.am.sm.ATK);
                StartCD(skillForce, careerValue.ForceCD);
                ac.am.sm.isForcingAim = false;
                SoundManager.Instance.StopEffectSound();
            }
        }

        if (ki.attackML)
            isForce = false;

        if (ki.R && !isFill)
        { photonView.RPC("RPC_SetTrigger", RpcTarget.All, "fillBullet"); }

    }
    [PunRPC]
    public override void LockState(){
        base.LockState();
		ac.camcon.DoUnAim();
        ac.anim.SetBool("aim", false);
	}

    #region animator skill events
    public override void NormalAttack()
    {
        
        if (!photonView.IsMine)
            return;
        else
        {
            if (isleft)
            { photonView.RPC("RPC_Projectile", RpcTarget.All, muzzles[0].position, RayAim(), ThrowerPower); CreateGunFire(muzzles[0]);}
            else
            { photonView.RPC("RPC_Projectile", RpcTarget.All, muzzles[1].position, RayAim(), ThrowerPower); CreateGunFire(muzzles[1]);}
            magazine--;
            isleft = !isleft;
        }
    }

    public override void FirstAttack()//
    {
        SoundManager.Instance.PlayEffectSound(Adela_Speedup);
        GameObject vfx = Instantiate(VFX_Adela_B1, transform.position,VFX_Adela_B1.transform.rotation) as GameObject;
        vfx.transform.SetParent(transform);
        SendMessage("AddBuff", "isSpeedup");
        //ac.anim.speed *= 2;

        if (!photonView.IsMine)
            return;
    }

    public override void SecondAttack()//F
    {
        SoundManager.Instance.PlayEffectSound(repelAttack);
        if (!photonView.IsMine)
            return;
        photonView.RPC("RPC_Projectile", RpcTarget.All, muzzles[2].position, RayAim(), 0f);
    }

    public override void RushAttack()//Q
    {
        CreateGunFire( muzzles[0]);
        CreateGunFire( muzzles[1]);
        if (!photonView.IsMine)
        { return; }
        photonView.RPC("RPC_Projectile", RpcTarget.All, transform.position, RayAim(), 0f);
        magazine--;
    }
    public void OnRushAttackEnter(){
        ac.pi.inputEnabled = false;
        ac.pi.inputMouseEnabled = false;
        ac.camcon.isHorizontalView = true;
    }
    public void OnRushAttackExit(){
        skillQ.atkTimer.state = MyTimer.STATE.FINISHED;
        ac.pi.inputEnabled = true;
        ac.pi.inputMouseEnabled = true;
        ac.camcon.isHorizontalView = false;
    }
    public override void AirAttack()
    {
        CreateGunFire( muzzles[0]);
        CreateGunFire( muzzles[1]);
        if (!photonView.IsMine)
        { return; }
        else
        {
            photonView.RPC("RPC_Projectile", RpcTarget.All, muzzles[0].position, RayAim() - transform.right, ThrowerPower);
            photonView.RPC("RPC_Projectile", RpcTarget.All, muzzles[1].position, RayAim() + transform.right, ThrowerPower);
        }
    }
    public void OnForcingEnter()
    {
        ki.inputEnabled = false;
        ac.camcon.isHorizontalView = true;
        SoundManager.Instance.PlayEffectSound(storage);
    }
    public override void ForceAttack()//蓄力(0.7s)
    {
        SoundManager.Instance.PlayEffectSound(Adela_Battery);
        foreach (var item in Beam)
        {
            item.SetActive(true);
            Projectile p = item.GetComponentInChildren<Projectile>();
            p.Initialize(ac.am, 0, RayAim());
        }
    }
    public void OnForceAttackExit()
    {
        ac.camcon.isHorizontalView = false;
        ki.inputEnabled = true;
        ac.canAttack=true;
    }
    public void magazineOperation() //確認是否填彈
    {
        if (magazine <= 0 && !isFill)
        {
            photonView.RPC("RPC_SetTrigger", RpcTarget.All, "fillBullet");
            isFill = true;
            ac.canAttack = false;
        }
        else
        {
            if (ac != null)
                ac.canAttack = true;
        }
    }

    public void BulletRecover() //填彈
    {
        magazine = MaxMagazine;
    }
    public void OnFillBulletExit() //填彈
    {
        isFill = false;
        ac.canAttack = true;
    }

    public void CreatFallMagazine()
    {
        GameObject _magazine = (GameObject)Instantiate(Magazine, magazinePos[0].position, magazinePos[0].rotation);
    }

    public void ChangeMagazinPos()
    {
        if (Obj_magazine.transform.parent == magazinePos[0])
        {
            Obj_magazine.SetActive(false);
            Obj_magazine.transform.SetParent(magazinePos[1]);
            Obj_magazine.transform.localPosition = Vector3.zero;
            Obj_magazine.transform.localRotation = Quaternion.identity;
        }
        else if (Obj_magazine.transform.parent == magazinePos[1])
        {
            Obj_magazine.transform.SetParent(magazinePos[0]);
            Obj_magazine.transform.localPosition = Vector3.zero;
            Obj_magazine.transform.localRotation = Quaternion.identity;
        }
    }

    public void MagazinAppear()
    {
        Obj_magazine.SetActive(true);
    }
    public void CreateGunFire0()
    {
        if (VFX_Adela_gunFire != null)
        {
            GameObject vfx = Instantiate(VFX_Adela_gunFire, muzzles[0].position, muzzles[0].rotation) as GameObject;
            vfx.transform.SetParent(muzzles[0]);
        }
        SoundManager.Instance.PlayEffectSound(Adela_Shot);
    }
    public void CreateGunFire1()
    {
        if (VFX_Adela_gunFire != null)
        {
            GameObject vfx = Instantiate(VFX_Adela_gunFire, muzzles[1].position, muzzles[1].rotation) as GameObject;
            vfx.transform.SetParent(muzzles[1]);
        }
        SoundManager.Instance.PlayEffectSound(Adela_Shot);
    }
    public void CreateGunFire(Transform trans)
    {
        if (VFX_Adela_gunFire != null)
        {
            GameObject vfx = Instantiate(VFX_Adela_gunFire, trans.position, trans.rotation) as GameObject;
            vfx.transform.SetParent(trans);
        }
        SoundManager.Instance.PlayEffectSound(Adela_Shot);
    }
    public void LegSound(){
        SoundManager.Instance.PlayEffectSound(Adela_Leg);
    }
    #endregion



    #region RPC
    [PunRPC]
    public void cube()
    {
        GameObject _cube = (GameObject)Instantiate(projectile[ac.anim.GetInteger("attackSkill")], transform.position - transform.up + transform.forward, Quaternion.identity);
    }

    [PunRPC]
    void PS_creatBuffEffect()
    {
        Instantiate(VFX_Adela_B1, transform);
    }

    #endregion

    #region PUN Callbacks
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // We own this player: send the others our data
            stream.SendNext(Beam[0].activeSelf);
            stream.SendNext(Beam[1].activeSelf);
        }
        else
        {
            // Network player, receive data
            this.Beam[0].SetActive((bool)stream.ReceiveNext());
            this.Beam[1].SetActive((bool)stream.ReceiveNext());
        }
    }
    #endregion
    public void butt(){
        ac.anim.SetLayerWeight(ac.anim.GetLayerIndex("butt"), Mathf.Lerp(ac.anim.GetLayerWeight(ac.anim.GetLayerIndex("butt")), 0.7f, 0.4f));
    }
    public void buttNone(){
        ac.anim.SetLayerWeight(ac.anim.GetLayerIndex("butt"), Mathf.Lerp(ac.anim.GetLayerWeight(ac.anim.GetLayerIndex("butt")), 0f, 0.4f));
    }

    #region other method

    // [PunRPC]
    // public void RPC_SetTargetLine(Vector3 targetPoint)
    // {
    //     GameObject drawLine = Instantiate(DrawLine, muzzle[0].transform.position, transform.rotation) as GameObject;
    //     drawLine.SetActive(true);
    //     drawLine.transform.LookAt(targetPoint);
    //     drawLine.GetComponent<_DrawLine>().DrawLine(targetPoint);
    // }


    #endregion
}