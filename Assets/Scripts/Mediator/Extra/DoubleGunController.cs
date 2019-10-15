using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

//Adela
public class DoubleGunController : ICareerController
{
    public GameObject VFX_Adela_Q;
    public GameObject VFX_Adela_gunFire;
    private KIDoubleGun ki;

    new protected Transform[] muzzle = new Transform[3];
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
    public AudioClip gunFire;
    public AudioClip repelAttack;
    void Start()
    {
        muzzle[0] = transform.DeepFind("MuzzleL");
        muzzle[1] = transform.DeepFind("MuzzleR");
        muzzle[2] = transform.DeepFind("MuzzleF");
        ac = GetComponent<ActorController>();
        ki = GetComponent<KIDoubleGun>();
        foreach (var item in Beam)
        { item.SetActive(false); }
        //StartCoroutine("Timer_Forcing");

    }


    void FixedUpdate()
    {
        //target = FindTargetPoint().transform.position;
    }
    void Update()
    {

        if (!photonView.IsMine)
        { return; }

        skillF.Tick();
        skillQ.Tick();
        skillAir.Tick();
        skillForce.Tick();
        forcingTimer.Tick();
        if (ac.am.sm.isDie)
        {
            ac.camcon.DoUnAim();
            ac.anim.SetBool("aim", false);
            skillQ.atkTimer.state = MyTimer.STATE.IDLE;
            return;
        }
		if(ac.pi.isLatent)
			return;
        //Debug.Log(skillML.atkTimer.elapsedTime);
        if (ac.canAttack && !isFill)
        {
            //canAttack限制狀態機行為
            if (ki.attackML)
            {
                if (ac.anim.GetBool("isHighFall") && !ac.anim.GetBool("isGround"))
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
                        UseSkill(0, careerValue.NormalDamage);
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
                StartCD(skillQ, careerValue.RushingCD);
                ac.am.sm.RP = 0;
            }

            //蓄力
            if (ki.forcingML)
            {
                if (CheckCD(skillForce))
                {
                    UseSkill(5, careerValue.ForceMinDamage, "force");
                    forcingTimer.Go(careerValue.ForcingCD);
                    isForce = true;
                    ac.am.sm.isForcingAim = true;
                }
            }
        }


        if (ki.attackMR)
        {
            UseSkill(1, careerValue.FirstDamage);
        }


        if (forcingTimer.state == MyTimer.STATE.FINISHED)
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
            }
        }

        if (ki.attackML)
            isForce = false;

        if (ki.R && !isFill)
        { photonView.RPC("RPC_SetTrigger", RpcTarget.All, "fillBullet"); }

    }


    #region animator skill events
    public override void NormalAttack()
    {
        CreateGunFire();
        if (!photonView.IsMine)
            return;
        else
        {
            if (isleft)
            { photonView.RPC("RPC_Projectile", RpcTarget.All, muzzle[0].position, RayAim(), ThrowerPower); }
            else
            { photonView.RPC("RPC_Projectile", RpcTarget.All, muzzle[1].position, RayAim(), ThrowerPower); }
            magazine--;
            isleft = !isleft;
        }

    }

    public override void SecondAttack()//F
    {
        SoundManager.Instance.PlayEffectSound(repelAttack);
        if (!photonView.IsMine)
            return;
        photonView.RPC("RPC_Projectile", RpcTarget.All, muzzle[2].position, RayAim(), 0f);
    }

    public override void RushAttack()//Q
    {
        CreateGunFire();
        if (!photonView.IsMine)
        { return; }
        photonView.RPC("RPC_Projectile", RpcTarget.All, transform.position, RayAim(), 0f);
        magazine--;
    }

    public override void AirAttack()
    {
        CreateGunFire();
        if (!photonView.IsMine)
        { return; }
        else
        {
            photonView.RPC("RPC_Projectile", RpcTarget.All, muzzle[0].position, RayAim(), ThrowerPower);
            photonView.RPC("RPC_Projectile", RpcTarget.All, muzzle[1].position, RayAim(), ThrowerPower);
        }
    }

    public override void ForceAttack()//蓄力(0.7s)
    {
        foreach (var item in Beam)
        {
            item.SetActive(true);
            Projectile p = item.GetComponentInChildren<Projectile>();
            p.Initialize(ac.am, 0, RayAim());
        }
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
        magazine = 10;
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
    public void SniperIdle()
    {
        if (ac != null)
            ac.lerpTarget = 1.0f;
    }
    public void CreateGunFire()
    {
        if (VFX_Adela_gunFire != null)
        {
            GameObject vfx = Instantiate(VFX_Adela_gunFire, muzzle[0].transform.position, transform.rotation) as GameObject;
            vfx.transform.SetParent(muzzle[0].transform);
        }
        SoundManager.Instance.PlayEffectSound(gunFire);
    }
    #endregion



    #region RPC
    [PunRPC]
    public void RPC_Creatcube()
    {
        GameObject _cube = (GameObject)Instantiate(projectile[ac.anim.GetInteger("attackSkill")], transform.position - transform.up + transform.forward, Quaternion.identity);
    }

    [PunRPC]
    void PS_creatQEffect()
    {
        Instantiate(VFX_Adela_Q, transform);
    }

    #endregion

    Vector2 Concentric; // ScreenPoint Of Camera center


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