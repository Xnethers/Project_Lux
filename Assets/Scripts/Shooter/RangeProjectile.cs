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
    public attackType attackType;
    public float Damage;

    // Use this for initialization
    public void Start()
    {
        cols = Physics.OverlapSphere(transform.position, range, LayerMask.NameToLayer("Body"));
        Damage = am.ac.careercon.careerValue.GetDamageByString(attackType);
        Invoke("Invoke_RangeDamage", 0);
    }

    public override void OnTriggerEnter(Collider col){}
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
            if (col.GetComponent<DamageHandler>() == null)
            {
                return;
            }
            SetRangeBuff(col);

            SendTryDoDamage(col);
        }
    }
}
