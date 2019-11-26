using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;


public class TwoStageProjectile : Projectile
{
    [Header("===== Second Stage Projectile  Settings =====")]
    public GameObject SecondProjectile;
    public attackType attackType;
    public float Damage;

    public LayerMask layer;

    public void Start()
    {
        Damage = am.ac.careercon.careerValue.GetDamageByString(attackType);
    }


    void Update()
    {

    }

    public override void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.layer == layer)
        {
            GameObject sp = Instantiate(SecondProjectile, transform.position, Quaternion.identity) as GameObject;
            sp.GetComponent<Projectile>().Initialize(am,0,targetPoint);
            Destroy(transform.root.gameObject);
        }
    }

    public override void AdditionalAttack(Collider col)
    {
        col.SendMessageUpwards("TryDoDamage", Damage * atkBuff);
    }

    public void Invoke_TryDoDamage()
    {
        col.SendMessageUpwards("TryDoDamage", Damage * atkBuff);
    }

}
