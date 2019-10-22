using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
public class SniperController : ICareerController
{
    public GameObject VFX_Enid_Q;
    public GameObject VFX_Enid_gunFire;
    public GameObject DrawLine;
    private KICareer ki;

    [Space(10)]
    [Header("===== Shooter Settings =====")]
    [SerializeField] float ThrowerPower;

    [Header("===== Fill Settings =====")]
    /* 彈匣 */
    [SerializeField] GameObject Magazine;

    [SerializeField] GameObject Obj_magazine;
    [SerializeField] Transform[] magazinePos;//[0]==槍,[1]==手
    public bool isFill;
    public int magazine = 10;

    [Header("===== Rush Settings =====")]
    public Collider target;
    public List<BodyCollider> APIV = new List<BodyCollider>(); //All Player In View 
    public List<Collider> allbodycollider;
    private BattleController[] allbc_obj;
    private LayerMask mask;
    public LayerMask ignoreMask;

    [HideInInspector]
    public Camera mainCamera;
    private MyTimer.STATE isRunState;

    public bool isForce;
    [Header("===== AudioClip Settings =====")]
    public AudioClip gunFire;
    public AudioClip repelAttack;
    void Start()
    {
        muzzle = transform.DeepFind("Muzzle");
        ac = GetComponent<ActorController>();
        ki = GetComponent<KICareer>();
        mask = LayerMask.NameToLayer("Body");
        mainCamera = Camera.main;
        Concentric = mainCamera.rect.center;
        StartCoroutine(RushFindTarget());
        //StartCoroutine("Timer_Forcing");
        isRunState = MyTimer.STATE.RUN;
        Updatebc();
    }

    void Updatebc()
    {
        allbc_obj = GameObject.FindObjectsOfType<BattleController>();
        for (int i = 0; i < allbc_obj.Length; i++)
        {
            Collider c = allbc_obj[i].GetComponent<Collider>();
            if (!allbodycollider.Contains(c))
            {
                if (allbc_obj[i].gameObject.layer == mask && allbc_obj[i].transform.root != this.transform.root)
                { allbodycollider.Add(c); }
            }
        }
    }
    void FixedUpdate()
    {
        FindTargetPoint();
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
            if (!ki.aimingMR)
            {
                if (ki.attackML)
                {
                    if (ac.height>3 && !ac.am.sm.isGround)
                    {
                        if (CheckCD(skillAir))
                        {
                            UseSkill(4, careerValue.AirDamage);
                            StartCD(skillAir, careerValue.AirCD);
                        }
                    }
                    else if (skillQ.atkTimer.state == isRunState)
                    {
                        UseSkill(3, careerValue.RushDamage);
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
                    Updatebc();
                    StartCD(skillQ, careerValue.RushingCD);
                    photonView.RPC("PS_creatQEffect", RpcTarget.All);
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

            else
            {
                if (ki.attackML)
                {
                    UseSkill(1, careerValue.FirstDamage);
                }
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
                Debug.Log("發動蓄力攻擊!!!!!!!!!!!!!!");
                UseSkill(5, ac.am.sm.ATK);
                StartCD(skillForce, careerValue.ForceCD);
                ac.am.sm.isForcingAim = false;
            }
        }
        if (ki.attackML)
            isForce = false;
        if (ki.auxiliaryMR)
        {
            ac.camcon.DoAim();
            ac.anim.SetBool("aim", true);
        }
        if (ki.unAimMR)
        {
            ac.camcon.DoUnAim();
            ac.anim.SetBool("aim", false);
        }
        if (ki.R && !isFill)
        { photonView.RPC("RPC_SetTrigger", RpcTarget.All, "fillBullet"); }

        if (Input.GetKeyDown(KeyCode.P))
        {
            RushFindTarget();
            for (int i = 0; i < APIV.Count; i++)
            {
                Debug.Log("NO." + i + ":" + APIV[i].collider.transform.root.name);
            }
        }

    }

    // void OnDrawGizmos()
    // {

    //     Matrix4x4 rotationMatrix = Matrix4x4.TRS(transform.position, Camera.main.transform.rotation, transform.lossyScale);
    //     Gizmos.matrix = rotationMatrix;
    //     Gizmos.color = Color.green;
    //     Gizmos.DrawWireCube(new Vector3(0, 0, Maxdistance / 2), new Vector3(6.6f, 3.7f, Maxdistance));

    // }



    #region animator skill events
    public override void NormalAttack()
    {
        CreateGunFire();
        if (!photonView.IsMine)
            return;
        photonView.RPC("RPC_Projectile", RpcTarget.All, muzzle.position, RayAim(), ThrowerPower);
        magazine--;
    }
    public void AimNormalAttack()
    {
        CreateGunFire();
        // DrawLine.DrawLine(RayAim());
        if (!photonView.IsMine)
            return;
        photonView.RPC("RPC_SetTargetLine", RpcTarget.All, RayAim());
        photonView.RPC("RPC_Projectile", RpcTarget.All, RayAim(), RayAim(), 0f);
        magazine--;
    }

    public override void SecondAttack()//2技
    {
        SoundManager.Instance.PlayEffectSound(repelAttack);
        if (!photonView.IsMine)
        { return; }
        photonView.RPC("RPC_Projectile", RpcTarget.All, muzzle.position, RayAim(), 0f);
    }

    public override void RushAttack()
    {
        CreateGunFire();
        if (!photonView.IsMine)
        { return; }
        photonView.RPC("RPC_Projectile", RpcTarget.All, muzzle.position, target.transform.position, ThrowerPower);
        magazine--;
    }

    public override void AirAttack()
    {
        CreateGunFire();
        if (!photonView.IsMine)
        { return; }
        else
        { photonView.RPC("RPC_Creatcube", RpcTarget.All); }
    }

    public override void ForceAttack()//蓄力(0.7s)
    {
        CreateGunFire();
        if (!photonView.IsMine)
        { return; }
        photonView.RPC("RPC_Projectile", RpcTarget.All, muzzle.position, RayAim(), ThrowerPower);
        magazine--;
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
        if (VFX_Enid_gunFire != null)
        {
            GameObject vfx = Instantiate(VFX_Enid_gunFire, muzzle.transform.position, transform.rotation) as GameObject;
            vfx.transform.SetParent(muzzle.transform);
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
        Instantiate(VFX_Enid_Q, transform);
    }

    #endregion

    Vector2 Concentric; // ScreenPoint Of Camera center


    #region other method
    public void FindTargetPoint() //找到離準心最近的body collider
    {
        APIV.Clear();
        foreach (var item in allbodycollider)
        {
            Vector2 viewPos = mainCamera.WorldToViewportPoint(item.transform.position);
            Vector3 dir = (item.transform.position - mainCamera.transform.position).normalized;
            float dot = Vector3.Dot(mainCamera.transform.forward, dir); //判断物体是否在相机前面
            if (Physics.Linecast(transform.position, item.transform.position, ~ignoreMask))//如果射線碰撞到物體
            { }
            else
            {
                if (item.transform.root.GetComponent<ActorManager>() != null)
                {
                    if (!item.transform.root.GetComponent<ActorManager>().sm.isDie &&
                    dot > 0 && viewPos.x >= 0 && viewPos.x <= 1 && viewPos.y >= 0 && viewPos.y <= 1)
                    {
                        BodyCollider i = new BodyCollider(item, Vector2.Distance(viewPos, Concentric));
                        APIV.Add(i);
                    }
                }
            }
        }
        if (APIV != null)
        {
            //按距離由小到大排列
            IEnumerable<BodyCollider> SBD = APIV.OrderBy(BodyCollider => BodyCollider.distance).ToArray(); // SBD =  Sort By Distance
            APIV = SBD.ToList();
            foreach (var item in APIV)
            {
                if (!item.collider.transform.root.GetComponent<ActorManager>().sm.isDie)
                {
                    target = item.collider;
                    break;
                }
            }
        }
        else
        {
            APIV.Clear();
        }

    }

    IEnumerator RushFindTarget()
    {
        while (true)
        {
            if (skillQ.atkTimer.state == isRunState)
            {
                FindTargetPoint();
                yield return new WaitForSeconds(0.2f);
            }
            else
            {
                target = null;
                yield return null;
            }
        }
    }
    [PunRPC]
    public void RPC_SetTargetLine(Vector3 targetPoint)
    {
        GameObject drawLine = Instantiate(DrawLine, muzzle.transform.position, transform.rotation) as GameObject;
        drawLine.SetActive(true);
        drawLine.transform.LookAt(targetPoint);
        drawLine.GetComponent<_DrawLine>().DrawLine(targetPoint);

    }


    #endregion
}

public class BodyCollider
{
    public Collider collider;
    public float distance;

    public BodyCollider(Collider collider, float distance)
    {
        this.collider = collider;
        this.distance = distance;
    }
}
