using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class TowerGuard : MonoBehaviourPunCallbacks
{//, IPunObservable
    public GameObject projectile;
    public float radius = 5;
    public float aim_time = 3;
    public float rotateAngle = 10;

    [Header("Projectile Setting")]
    public float projectileSpeed = 20;
    public float projectileAtk = 10;
    public LayerMask layer;
    public LayerMask ignorePlayer;


    [Header("Material Setting")]
    public Material usual;
    public Material aim;
    private SkinnedMeshRenderer meshRenderer;

    enum status
    { aim, shoot, search, finished}
    [SerializeField]
    private status Status;
    private BattleManager bm;
    [SerializeField] private Collider[] Playerlist;
    private DrawLine dl;
    private PhotonView pv;
    public MyTimer Timer = new MyTimer();
    private Animator animator;
    [Header("===== Audio Settings =====")]
    public bool isWatch;
    public AudioClip Watching;
	public AudioClip Fire;
    public Rigidbody rb;

    [Header("===== VFX Settings =====")]
    public GameObject VFX_Watch;
    public GameObject VFX_WatchFire;
    private Transform muzzle;

    // Use this for initialization
    void Start()
    {
        layer = 1 << LayerMask.NameToLayer("Body");
        dl = GetComponent<DrawLine>();
        pv = GetComponent<PhotonView>();
        animator = GetComponent<Animator>();
        dl.SetLineEnabled(true);
        dl.destination = this.transform;
        dl.Disappear();
        meshRenderer = GetComponent<SkinnedMeshRenderer>();
        muzzle = transform.GetChild(0);
        this.tag = transform.parent.tag;
        rb=GetComponent<Rigidbody>();
        rb.useGravity = false;
    }

    // Update is called once per frame
    void Update()
    {
        Playerlist = Physics.OverlapSphere(transform.position, radius, layer);
        Timer.Tick();
        if (GameManager.Instance.isResult)
        { 
            meshRenderer.sharedMaterial = usual;
            rb.useGravity = true;
            animator.enabled=false;
            return; 
        }
        
        switch (Status)
        {
            case status.aim:
                {
                    
                    if (CheckPlayerExist())
                    {
                        if(!isWatch){
                            SoundManager.Instance.PlayEffectSound(Watching);
                            isWatch=true;
                        }

                        meshRenderer.sharedMaterial = aim;
                        transform.LookAt(dl.destination);
                        dl.Appear(0.02f);
                                               
                        if (muzzle.childCount == 0)
                        {
                            GameObject vfx = GameObject.Instantiate(VFX_Watch, muzzle.position, muzzle.rotation);
                            vfx.transform.SetParent(muzzle);
                        }

                        if (Timer.state == MyTimer.STATE.RUN)
                        { }
                        else if (Timer.state == MyTimer.STATE.IDLE)
                        { Timer.Go(aim_time); }
                        else if (Timer.state == MyTimer.STATE.FINISHED)
                        {
                            Timer.state = MyTimer.STATE.IDLE;
                            Status = status.shoot;
                        }
                    }
                    else
                    {
                        Status = status.search;
                        // Destroy();
                        animator.SetBool("Aim", false);
                        Timer.Reset();
                    }
                    break;
                }
            case status.shoot:
                {
                    if (GameManager.Instance.isResult)
                    { return; }
                    else
                    {

                        GameObject vfx = GameObject.Instantiate(VFX_WatchFire, muzzle.position, transform.rotation);
                        vfx.transform.SetParent(muzzle);

                        transform.LookAt(dl.destination);
                        dl.Disappear();
                        // dl.SetLineEnabled(false);
                        if (PhotonNetwork.IsConnected && PhotonNetwork.IsMasterClient)
                        {
                            pv.RPC("ShootProjectile", RpcTarget.All);
                        }

                        Timer.Reset();
                        Status = status.finished;
                    }
                    break;
                }
            case status.search:
                {
                    Vector3 relativePos = new Vector3( transform.parent.position.x ,transform.position.y,transform.parent.position.z) - transform.position;
                    transform.rotation =Quaternion.LookRotation(relativePos)*Quaternion.Euler(0,90*-rotateAngle,0);
                    dl.destination = null;
                    dl.Disappear();
                    transform.RotateAround(transform.parent.position, Vector3.up, rotateAngle);
                    //dl.SetLineEnabled(false);
                    if (CheckPlayerExist())
                    { animator.SetBool("Aim", true); Status = status.aim; break; }
                    meshRenderer.sharedMaterial = usual;
                    animator.SetBool("Aim", false);
                    break;
                }
            case status.finished:
                {
                    isWatch=false;
                    if (Timer.state == MyTimer.STATE.RUN)
                    { }
                    else if (Timer.state == MyTimer.STATE.IDLE)
                    { Timer.Go(1F); }
                    else if (Timer.state == MyTimer.STATE.FINISHED)
                    {
                        Timer.state = MyTimer.STATE.IDLE;
                        Timer.Reset();
                        Status = status.search;
                    }
                    break;
                }
        }
    }
    bool CheckPlayerExist()
    {
        if (Physics.CheckSphere(transform.position, radius, layer))
        {
            if (dl.destination == null)
            {
                for (int i = 0; i < Playerlist.Length; i++)
                {
                    if (Playerlist[i].tag != this.tag)
                    {
                        if (!Physics.Linecast(transform.position, Playerlist[i].transform.position, ignorePlayer))
                        {
                            dl.destination = Playerlist[i].transform;
                            return true;
                        }
                        else
                        { continue; }
                    }
                    else
                    { continue; }
                }
                return false;
            }
            else
            {
                if (!Physics.Linecast(transform.position, dl.destination.transform.position, ignorePlayer))
                { return true; }
                else
                {
                    dl.destination = null;
                    return false;
                }
            }
        }
        else
        { return false; }
    }

    [PunRPC]
    void ShootProjectile()
    {
        GameObject bullet = Instantiate(projectile, transform.position, transform.rotation) as GameObject;
        bullet.GetComponentInChildren<TrackingProjectile>().setTrack(dl.destination, projectileSpeed);
        bullet.transform.LookAt(dl.destination);
        bullet.GetComponentInChildren<Projectile>().Initialize(this.tag, projectileAtk, projectileSpeed, dl.destination.position);
        SoundManager.Instance.PlayEffectSound(Fire);
    }

    // public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    // {
    //     throw new System.NotImplementedException();
    // }
    //Physics.OverlapSphere()

}
