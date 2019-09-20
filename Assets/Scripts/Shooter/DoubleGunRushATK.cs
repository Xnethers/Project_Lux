using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class DoubleGunRushATK : Projectile
{
    [Header("===== DoubleGunRushATK Settings =====")]

    public float range = 5;//範圍
    public float duration = 3;//持續時間
    public float Intervals = 0.1f;//間隔時間
    public float DelayedLaunch = 0.0f;
    public LayerMask mask;//目標對象layer
    Collider[] cols;//目標對象collider
    public GameObject RushZoneVFX;

    public override void Awake()
    {
        base.Awake();
        cols = Physics.OverlapSphere(transform.position, range, mask);
    }

    void Start()
    {
        Destroy(this, duration);
        InvokeRepeating("Invoke_DoMultiDamage", DelayedLaunch, Intervals);
    }

    void Update()
    {
    }
    public void Invoke_DoMultiDamage()
    {
        foreach (Collider col in cols)
        {
            if (col.tag == this.tag)
            {
                Debug.Log("col.tag == this.tag" + col.name);
                // isHit=true;
                return;
            }
            col.SendMessageUpwards("TryDoDamage", am.ac.careercon.careerValue.NormalDamage);
        }
    }

    public void find()
    { cols = Physics.OverlapSphere(transform.position, range, mask); }

}
