using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class DoubleGunRushATK : Projectile
{
    [Header("===== DoubleGunRushATK Settings =====")]

    public float range = 5;//範圍
    public float duration = 3;//持續時間
    public int frequency = 30; //次數
    public float Intervals = 0.1f;//間隔時間
    public float DelayedLaunch = 0.0f;//延遲發動時間
    public LayerMask mask;//目標對象layer
    public Collider[] cols;//目標對象collider
    public GameObject RushZoneVFX;

    public override void Awake()
    {
        base.Awake();
        cols = Physics.OverlapSphere(transform.position, range, mask);
    }

    void Start()
    {
        Destroy(this, duration);
        cols = Physics.OverlapSphere(transform.position, range, mask);
        StartCoroutine("DoMultiDamag");
        for (int i = 0; i < frequency + 1; i++)
        {           
            //Invoke("Invoke_DoMultiDamage", Intervals * i);
        }
    }

    IEnumerator DoMultiDamag()
    {
        for (int i = 0; i < frequency + 1; i++)
        {
            foreach (Collider col in cols)
            {
                if (col.tag == this.tag)
                {
                    continue;
                }
                if (col.GetComponent<DamageHandler>() == null)
                {
                    continue;
                }
                SendTryDoDamage(col);
            }
            yield return new WaitForSeconds(Intervals);
        }
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
            if (col.GetComponent<DamageHandler>() == null)
            {
                return;
            }
            SendTryDoDamage(col);
        }
    }

    public void find()
    { cols = Physics.OverlapSphere(transform.position, range, mask); }

}
