using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

//Adela
public class PunchController : ICareerController
{
    public GameObject[] Punches = new GameObject[2];
    private Rigidbody[] Punches_rb = new Rigidbody[2];
    public Transform[] CurvePoint = new Transform[2];
    public Transform[] origLocPos = new Transform[2];
    public GameObject VFX_Adela_Q;
    public GameObject VFX_Adela_gunFire;
    private KICareer ki;

    new protected Transform[] muzzle = new Transform[3];
    private bool isleft = true;

    [Header("===== Punch Settings =====")]
    public bool ispunching;
    public bool isreturn;
    public float speed = 1.5f;
    [SerializeField] private float returnTime;
    private Vector3 targetPos;

    [Space(10)]
    [Header("===== Shooter Settings =====")]
    [SerializeField] float ThrowerPower;

    public bool isForce;
    [Header("===== AudioClip Settings =====")]
    public AudioClip gunFire;
    public AudioClip repelAttack;

    void Start()
    {
        for (int i = 0; i < Punches.Length; i++)
        {
            Punches_rb[i] = Punches[i].GetComponent<Rigidbody>();
            Punches[i].transform.position = origLocPos[i].position;
            CurvePoint[i] = transform.DeepFind("curvepoint (" + i + ")");
        }

        ac = GetComponent<ActorController>();
        ki = GetComponent<KICareer>();
        //StartCoroutine("Timer_Forcing");

    }

    void FixedUpdate()
    {
        if (ispunching)
        {
            if (isreturn && IfReurn())
            {
                isleft = !isleft;
                isreturn = ispunching = false;
                returnTime = 0;
                Punches_rb[0].transform.parent = transform;
                Punches_rb[1].transform.parent = transform;
            }
            else if (!isreturn && returnTime < 1)
            {
                if (!isleft)
                {
                    Punches_rb[0].transform.parent = null;
                    Punches_rb[0].position = Bezier.GetPoint(origLocPos[0].position, CurvePoint[0].position, targetPos, returnTime);
                    returnTime += Time.deltaTime * speed;
                }
                else
                {
                    Punches_rb[1].transform.parent = null;
                    Punches_rb[1].position = Bezier.GetPoint(origLocPos[1].position, CurvePoint[1].position, targetPos, returnTime);
                    returnTime += Time.deltaTime * speed;
                }
            }
            else if (isreturn && !IfReurn())
            {
                if (!isleft)
                {
                    Punches_rb[0].position = Bezier.GetPoint(targetPos, CurvePoint[0].position, origLocPos[0].position, returnTime);
                    returnTime += Time.deltaTime * speed;
                }
                else
                {
                    Punches_rb[1].position = Bezier.GetPoint(targetPos, CurvePoint[1].position, origLocPos[1].position, returnTime);
                    returnTime += Time.deltaTime * speed;
                }
            }
            else if (!isreturn && returnTime >= 1)
            {
                if (!isleft)
                {
                    //targetPos = Punches_rb[0].position;
                    photonView.RPC("RPC_Projectile", RpcTarget.All, Punches_rb[0].transform.position, RayAim(), 0f);
                }
                else
                {
                    //targetPos = Punches_rb[1].position;
                    photonView.RPC("RPC_Projectile", RpcTarget.All, Punches_rb[1].transform.position, RayAim(), 0f);
                }
                PunchStartReturn();
            }

        }
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
        if (ac.pi.isLatent)
            return;
        //Debug.Log(skillML.atkTimer.elapsedTime);
        if (ac.canAttack)
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


        if (ki.auxiliaryMR)
        {
            if (ac.CheckState("forcing", "attack"))
            {
                UseSkill(1, careerValue.FirstDamage);
            }
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

        if (ki.R)
        { photonView.RPC("RPC_SetTrigger", RpcTarget.All, "fillBullet"); }

    }

    bool IfReurn()
    {
        float d;
        if (!isleft)
        { d = Vector3.Distance(Punches_rb[0].position, origLocPos[0].position); }
        else
        { d = Vector3.Distance(Punches_rb[1].position, origLocPos[1].position); }
        if (d > 0)
        { return false; }
        else
        { return true; }
    }

    public void PunchStartReturn()
    {
        // pullPosition = weapon.position;
        // weaponRb.Sleep();
        // weaponRb.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
        // weaponRb.isKinematic = true;
        isreturn = true;
        returnTime = 0;
    }


    #region animator skill events
    public override void NormalAttack()
    {
        CreatePunchFire();
        if (!photonView.IsMine)
            return;
        else
        {
            ispunching = true;
            targetPos = RayAim();
        }
    }

    public override void FirstAttack()//
    {
        //SoundManager.Instance.PlayEffectSound(gunFire);
        ac.am.sm.sb.isSpeedup = true;
        ac.anim.speed *= 2;

        if (!photonView.IsMine)
            return;
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
        if (!photonView.IsMine)
        { return; }
        photonView.RPC("RPC_Projectile", RpcTarget.All, transform.position, RayAim(), 0f);
    }

    public override void AirAttack()
    {
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

    }

    public void magazineOperation() //確認是否填彈
    {
    }

    public void BulletRecover() //填彈
    {
    }
    public void OnFillBulletExit() //填彈
    {
    }

    // public void CreatFallMagazine()
    // {
    //     GameObject _magazine = (GameObject)Instantiate(Magazine, magazinePos[0].position, magazinePos[0].rotation);
    // }

    // public void ChangeMagazinPos()
    // {
    //     if (Obj_magazine.transform.parent == magazinePos[0])
    //     {
    //         Obj_magazine.SetActive(false);
    //         Obj_magazine.transform.SetParent(magazinePos[1]);
    //         Obj_magazine.transform.localPosition = Vector3.zero;
    //         Obj_magazine.transform.localRotation = Quaternion.identity;
    //     }
    //     else if (Obj_magazine.transform.parent == magazinePos[1])
    //     {
    //         Obj_magazine.transform.SetParent(magazinePos[0]);
    //         Obj_magazine.transform.localPosition = Vector3.zero;
    //         Obj_magazine.transform.localRotation = Quaternion.identity;
    //     }
    // }

    // public void MagazinAppear()
    // {
    //     Obj_magazine.SetActive(true);
    // }
    public void SniperIdle()
    {
        if (ac != null)
            ac.lerpTarget = 1.0f;
    }
    public void CreatePunchFire()
    {
        if (VFX_Adela_gunFire != null)
        {
            GameObject vfx = Instantiate(VFX_Adela_gunFire, Punches[0].transform.position, transform.rotation) as GameObject;
            vfx.transform.SetParent(Punches[0].transform);
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

    #region PUN Callbacks
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // We own this player: send the others our data
            stream.SendNext(Punches[0].transform.position);
            stream.SendNext(Punches[1].transform.position);
        }
        else
        {
            // Network player, receive data
            this.Punches[0].transform.position = (Vector3)stream.ReceiveNext();
            this.Punches[1].transform.position = (Vector3)stream.ReceiveNext();
        }
    }
    #endregion


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