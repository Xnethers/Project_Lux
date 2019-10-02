using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class RangeProjectile : Projectile
{
    [Header("===== Settings =====")]
    public float range = 5;
    public LayerMask mask;
    Collider[] cols;

    public override void Awake()
    {
        base.Awake();
        cols = Physics.OverlapSphere(transform.position, range, mask);
    }


    // Use this for initialization
    void Start()
    {
        Invoke("Invoke_RangeDamage", 0);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Invoke_RangeDamage()
    {
        foreach (Collider col in cols)
        {
            if (col.tag == this.tag)
            {
                Debug.Log("col.tag == this.tag" + col.name);
                // isHit=true;
                return;
            }
            SetRangeBuff(col);
            BattleController bc = col.GetComponent<BattleController>();
            if (bc != null)
            {
                // InstantiateVFX(col,new Vector3(col.transform.position.x,transform.position.y,col.transform.position.z));
                col.SendMessageUpwards("SetTargetAm", am);
                col.SendMessageUpwards("SetAllDeBuff", new DamageBuff(isBlind, isRepel, isMark,isShake));
            }
            col.SendMessageUpwards("TryDoDamage", GetATK());
        }
    }
}
