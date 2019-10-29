﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

//Adela
public class PunchController : ICareerController
{
    public bool isForce;
    private KICareer ki;

    [Header("===== Punch Settings =====")]
    public GameObject[] Punches = new GameObject[2];
    private Rigidbody[] Punches_rb = new Rigidbody[2];
    public Transform[] CurvePoint = new Transform[2];
    public Transform[] origLocPos = new Transform[2];
    private bool isleft = true;
    private Vector3 targetPos;

    //------NomalAttack------
    public bool ispunching;
    private bool isreturn;
    public float speed = 1.5f;
    private float returnTime;

    //------FirstAttack------
    private bool isrotate = false;
    public float rotateSpeed = 10f;
    public float angled = 0; // set this to the maximum angle in degrees


    [Space(10)]
    [Header("===== Shooter Settings =====")]
    [SerializeField] float ThrowerPower;


    [Header("===== AudioClip Settings =====")]
    public AudioClip gunFire;
    public AudioClip repelAttack;

    [Header("===== VFX Settings =====")]
    public GameObject VFX_Adela_Q;
    public GameObject VFX_Adela_gunFire;

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

    #region Fixed Update
    void FixedUpdate()
    {
        if (isrotate)
        {
            Punches[0].GetComponent<Animator>().SetTrigger("rotate");
            Punches[1].GetComponent<Animator>().SetTrigger("rotate");
            isrotate = false;
            //rotate_around();
        }

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
    #endregion

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

        if (ki.R)
        { photonView.RPC("RPC_SetTrigger", RpcTarget.All, "fillBullet"); }

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
        if (!photonView.IsMine)
            return;
        // photonView.RPC("RPC_Projectile", RpcTarget.All, Punches_rb[0].position, Punches_rb[0].position, 0);
        // photonView.RPC("RPC_Projectile", RpcTarget.All, Punches_rb[1].position, Punches_rb[0].position, 0);
        GameObject[] bullet = new GameObject[2];
        bullet[0] = Instantiate(projectile[ac.anim.GetInteger("attackSkill")], Punches_rb[0].position, transform.rotation) as GameObject;
        bullet[0].transform.parent = Punches_rb[0].transform;
        bullet[1] = Instantiate(projectile[ac.anim.GetInteger("attackSkill")], Punches_rb[1].position, transform.rotation) as GameObject;
        bullet[1].transform.parent = Punches_rb[1].transform;
        foreach (var item in bullet)
        {
            foreach (Projectile projectile in item.GetComponentsInChildren<Projectile>())
            { projectile.Initialize(ac.am, 0, RayAim()); }
        }
        isrotate = true;
    }

    public override void SecondAttack()//F
    {
        SoundManager.Instance.PlayEffectSound(repelAttack);
        if (!photonView.IsMine)
            return;
        photonView.RPC("RPC_Projectile", RpcTarget.All, muzzle.position, RayAim(), 0f);
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
            photonView.RPC("RPC_Projectile", RpcTarget.All, muzzle.position, RayAim(), ThrowerPower);
            photonView.RPC("RPC_Projectile", RpcTarget.All, muzzle.position, RayAim(), ThrowerPower);
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
            stream.SendNext(ispunching);
            stream.SendNext(isreturn);
        }
        else
        {
            // Network player, receive data
            this.ispunching = (bool)stream.ReceiveNext();
            this.isreturn = (bool)stream.ReceiveNext();
        }
    }
    #endregion


    #region other method

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

    void rotate_around()
    {
        if (angled > 360)
        { isrotate = false; angled = 0; }
        else
        {
            Punches_rb[0].transform.RotateAround(transform.position, Vector3.up, rotateSpeed * Time.deltaTime);
            Punches_rb[1].transform.RotateAround(transform.position, Vector3.up, rotateSpeed * Time.deltaTime);
            angled += ((int)rotateSpeed * Time.deltaTime) % 360;///累加轉過的角度
            Debug.Log(angled);
        }
    }

    #endregion
}