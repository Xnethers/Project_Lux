using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[RequireComponent(typeof(CapsuleCollider))]
public class BattleController : DamageHandler {

    //public ActorManager am;

    // private CapsuleCollider defCol;
    // private BoxCollider headCol;
    // private CapsuleCollider bodyCol;
    public BattleManager bm;
    public Collider defCol;

    //// Use this for initialization
    void Start () {
        defCol = GetComponent<Collider>();
        // defCol = GetComponent<CapsuleCollider>();
        // defCol.center = Vector3.up * 0.8f;
        // defCol.height = 1.5f;
        // defCol.radius = 0.4f;
        // defCol.isTrigger = true;
    }

    private void OnTriggerEnter(Collider col)
    {
        // if(!bm.am.photonView.IsMine)//人物父層才有photonView
        //     return;
        // Projectile projectile = null;
        // if(col.gameObject.layer == LayerMask.NameToLayer("Projectile"))
        //     projectile = col.GetComponent<Projectile>();
        // if (projectile == null) {
        //     return;
        // }
        // if(col.tag != bm.am.tag && projectile.GetComponent<Collider>().enabled){
        //     projectile.OnBulletTriggerEnter(defCol);//改變射程Buff
        //     bm.am.TryDoDamage(projectile.GetAm(),projectile.GetATK());//本玩家受傷害
        //     projectile.AdditionalAttack(bm.am);//本玩家受子彈附加攻擊/Buff，隱藏/刪除物件
        //     projectile.GetComponent<Collider>().enabled=false;
        // }
    }
}
