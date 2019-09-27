﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;


public class TwoStageProjectile : Projectile
{
    [Header("===== Second Stage Projectile  Settings =====")]
    public GameObject SecondProjectile;
    public attackType attackType;
    public float Damage;

    public void Start()
    {
        Damage = am.ac.careercon.careerValue.GetDamageByString(attackType);
    }


    void Update()
    {

    }

    public override void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.layer == LayerMask.NameToLayer("Wall"))
        {
            GameObject sp = Instantiate(SecondProjectile, transform.position, Quaternion.identity) as GameObject;
            Destroy(transform.root.gameObject);
        }
    }

    public override void AdditionalAttack(Collider col)
    {
        col.SendMessageUpwards("TryDoDamage", Damage);
    }

    public void Invoke_TryDoDamage()
    {
        col.SendMessageUpwards("TryDoDamage", Damage);
    }

}