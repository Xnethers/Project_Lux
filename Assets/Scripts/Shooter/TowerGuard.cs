using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class TowerGuard : MonoBehaviour, IPunObservable
{
    public GameObject projectile;
    public float radius = 5;
    public float aim_time = 3;
    public float rotateAngle = 10;

    [Header("Projectile Setting")]
    public float projectileSpeed = 20;
    public float projectileAtk = 10;
    public LayerMask layer;

    enum status
    { aim, shoot, search, finished }
    [SerializeField]
    private status Status;
    private BattleManager bm;
    [SerializeField] private Collider[] Playerlist;
    private DrawLine dl;
    private PhotonView pv;
    public MyTimer Timer = new MyTimer();
    private Transform target;


    // Use this for initialization
    void Start()
    {
        layer = 1 << LayerMask.NameToLayer("Body");
        dl = GetComponent<DrawLine>();
        pv = GetComponent<PhotonView>();
        dl.SetLineEnabled(true);
        dl.destination = this.transform;
        dl.Disappear();
        this.tag = transform.parent.tag;
    }

    // Update is called once per frame
    void Update()
    {
        Playerlist = Physics.OverlapSphere(transform.position, radius, layer);
        Timer.Tick();
        switch (Status)
        {
            case status.aim:
                {
                    if (CheckPlayerExist())
                    {
                        transform.LookAt(dl.destination);
                        dl.Appear(0.02f);

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
                        Timer.Reset();
                    }
                    break;
                }
            case status.shoot:
                {
                    dl.Disappear();
                    // dl.SetLineEnabled(false);
                    if (PhotonNetwork.IsConnected)
                    { pv.RPC("ShootProjectile", RpcTarget.All); }
                    else
                    { ShootProjectile(); }
                    Timer.Reset();
                    Status = status.finished;
                    break;
                }
            case status.search:
                {
                    dl.destination = null;
                    dl.Disappear();
                    transform.RotateAround(transform.parent.position, Vector3.up, rotateAngle);
                    //dl.SetLineEnabled(false);
                    if (CheckPlayerExist())
                    { Status = status.aim; }
                    break;
                }
            case status.finished:
                {
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
                        dl.destination = Playerlist[i].transform;
                        return true;
                    }
                    else
                    { continue; }
                }
                return false;
            }
            else
            { return true; }
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
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        throw new System.NotImplementedException();
    }
    //Physics.OverlapSphere()

}
