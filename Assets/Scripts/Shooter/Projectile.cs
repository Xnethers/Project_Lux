using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Projectile : MonoBehaviourPunCallbacks
{

    public GameObject normalhitVFX;
    public GameObject specialhitVFX;
    [SerializeField] protected bool isHit;
    public bool isVFX = false;

    [Header("===== Kind Settings =====")]
    [SerializeField] bool isBullet = false;
    [SerializeField] bool isKinematic = true;

    [Header("===== DeBuff Settings =====")]

    public string[] buffsName;

    [Header("===== Damage Settings =====")]
    [SerializeField] float baseATK = 1f;
    public float atkBuff = 1f;
    [SerializeField] float rangeBuff = 1f;
    [SerializeField] float headBuff = 1f;
    [SerializeField] float comboBuff = 1f;
    [SerializeField] protected float timeToLive = 3f;
    protected ActorManager am;//子彈發射者
    protected Collider col;//攻擊對象
    protected TowerHealth towerHealth;
    protected Vector3 originVec3;
    protected Vector3 targetPoint;
    protected Rigidbody rb;
    private float speed;
    // public LayerMask layerMask;
    // public float distance = 2f;

    public virtual void Initialize(ActorManager am, float speed, Vector3 targetPoint)
    {
        this.am = am;
        this.tag = am.tag;

        this.baseATK = am.sm.ATK;
        this.atkBuff = am.sm.ATKBuff;
        this.originVec3 = transform.position;
        this.targetPoint = targetPoint;
        rb = GetComponent<Rigidbody>();
        rb.velocity = rb.transform.forward * speed;
        //this.speed=speed;
    }
    public virtual void Initialize(string tag, float atk, float speed, Vector3 targetPoint)
    {
        this.tag = tag;
        this.baseATK = atk;
        this.originVec3 = transform.position;
        this.targetPoint = targetPoint;
        rb = GetComponent<Rigidbody>();
        rb.velocity = rb.transform.forward * speed;
    }
    public virtual void Awake()
    {
        Destroy(transform.root.gameObject, timeToLive);
    }

    void Update()
    {
        //transform.Translate(Vector3.forward * speed * Time.deltaTime);

        //Debug.Log(headBuff);
        // RaycastHit hitInfo;

        // if (Physics.Linecast(originVec3, targetPoint,out hitInfo,layerMask)){
        // 	ProjectileHit(hitInfo.collider);
        // 	targetPoint =Vector3.zero;
        // 	rb.isKinematic=true;
        // }	
        // if(Vector3.Distance(transform.position,targetPoint)< distance && !rb.isKinematic && !isHit){

        // }
        // Debug.DrawRay(originVec3, hitInfo.point,Color.blue);		
    }

    public virtual void OnTriggerEnter(Collider col)
    {
        if (isHit)
            return;
        if (isKinematic)
            rb.isKinematic = true;
        if (isBullet)
        {
            transform.GetChild(0).gameObject.SetActive(false);
            if (normalhitVFX != null && !isVFX)
            {//子彈火花
                GameObject vfx = Instantiate(normalhitVFX, targetPoint - transform.forward * 0.5f, transform.rotation) as GameObject;
                isVFX = true;
            }
        }

        if (col.tag == this.tag)
        {
            // Debug.Log("col.tag == this.tag"+col.name);
            // isHit=true;
            return;
        }

        if (col.GetComponent<DamageHandler>() == null)
        {
            // Debug.Log("col.GetComponent<DamageHandler>() == null"+col.name);
            // isHit=true;
            // Destroy(transform.gameObject);
            return;
        }

        if (isBullet)
        {
            SetRangeBuff(col);
            SetHeadBuff(col);
            if (specialhitVFX != null && !isVFX)
            {//蜂窩火花
                GameObject vfx = Instantiate(specialhitVFX, targetPoint, transform.rotation) as GameObject;
                isVFX = true;
            }
        }
        else
        {
            if (normalhitVFX != null && !isVFX)
            {//被刀擊
                GameObject vfx = Instantiate(normalhitVFX, transform.position, transform.rotation) as GameObject;
                isVFX = true;
            }
        }
        AdditionalAttack(col);
        SendTryDoDamage(col);
        isHit = true;
    }
    protected void SendTryDoDamage(Collider col){
        col.SendMessageUpwards("TryDoDamage",new DamageData(am, GetATK(),buffsName));
    }
    protected void SetRangeBuff(Collider col)
    {
        float rangeDistance = Vector3.Distance(originVec3, col.transform.position);
        if (rangeDistance <= 10)
            rangeBuff = 1f;
        else if (10 < rangeDistance && rangeDistance <= 20)
            rangeBuff = 0.9f;
        else if (20 < rangeDistance && rangeDistance <= 30)
            rangeBuff = 0.7f;
        else if (rangeDistance > 30)
            rangeBuff = 0.5f;

    }
    protected void SetHeadBuff(Collider col)
    {
        if (col.gameObject.layer == LayerMask.NameToLayer("Head"))
        {
            headBuff = 1.7f;
            // Debug.Log("Head");
            //return;
        }

        if (col.gameObject.layer == LayerMask.NameToLayer("Body"))
        {
            headBuff = 1f;
            // Debug.Log("Body");
            //return;
        }
    }
    public float GetATK()
    {
        Debug.Log("ATK:" + baseATK * rangeBuff * comboBuff * headBuff * atkBuff);
        return baseATK * rangeBuff * headBuff * comboBuff * atkBuff;
    }
    public virtual void AdditionalAttack(Collider col)
    {

        // if(isBullet)
        // 	transform.GetChild(0).gameObject.SetActive(false);
    }
}