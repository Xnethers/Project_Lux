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
        if (ActorManager.IsInLayerMask(col.gameObject.layer,layer))//col.gameObject.layer == layer
        {
            GameObject sp = Instantiate(SecondProjectile, transform.position, Quaternion.identity) as GameObject;
            sp.GetComponent<Projectile>().Initialize(am,0,targetPoint);
            Destroy(transform.root.gameObject);
        }
    }
    
    public override void AdditionalAttack(Collider col)
    {
        SendTryDoDamage(col);
    }

    public void Invoke_TryDoDamage()
    {
        SendTryDoDamage(col);
    }

}
