using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using Photon.Pun.UtilityScripts;

/*Manager總管理處 */
public class ActorManager : MonoBehaviourPunCallbacks {
    public ActorManager targetAm;
    public ActorController ac;

    [Header("=== Auto Generate if Null ===")]
    public BattleManager bm;
    public WeaponManager wm;
    public StateManager sm;
    public InteractionManager im;


	// Use this for initialization
	void Awake () {
        ac = Bind<ActorController>(gameObject);
        GameObject sensor = transform.Find("sensor").gameObject;
        GameObject model = ac.model;
        
        bm = Bind<BattleManager>(sensor);
        wm = Bind<WeaponManager>(model);
        sm = Bind<StateManager>(gameObject);
        im = Bind<InteractionManager>(sensor);
        
	}
    void Start()
    {
        if(ac.pi.isAI)
            return;
        DontDestroyOnLoad(this.gameObject);
        AmSetTag();
        if(photonView.IsMine){
            SoundManager.Instance.EffectsSource = GetComponent<AudioSource>();
        }
    }
    public void AmSetTag(){
        if(ac.pi.isAI)
            return;
        ActorManager[] PMs=FindObjectsOfType<ActorManager>();
        List<ActorManager> playerAm = new List<ActorManager>();
        foreach(ActorManager tempAm in PMs){
            if(!tempAm.ac.pi.isAI)
                playerAm.Add(tempAm);
        }
        foreach(Player p in PhotonNetwork.PlayerList){
            Debug.Log("playrID"+p.ActorNumber);
            Debug.Log("photonViewid"+photonView.Owner.ActorNumber);

            foreach(ActorManager pm in playerAm){
                if(p.ActorNumber==pm.photonView.Owner.ActorNumber){
                    SetTag(p,pm);
                }
            }
        }
    }
    private T Bind<T>(GameObject go) where T : IActorManagerInterface {
        T tempInstance;
        tempInstance = go.GetComponent<T>();
        if (tempInstance == null) {
            tempInstance = go.AddComponent<T>();
        }
        tempInstance.am = this;
        return tempInstance;
    }
	
	// Update is called once per frame
	void Update () {
		// if(Input.GetKeyDown(KeyCode.K)){
        //     Die();
        // } 
        
	}
    public static bool IsInLayerMask(int layer, LayerMask layermask)
    {
        return layermask == (layermask | (1 << layer));
    }
    [PunRPC]
    public void TryDoDamage(DamageData damageData) {
        //sm.HP -= 5;
        /*if (sm.HP > 0) {
            sm.AddHP(-5);
        }*/
        Debug.Log("TryDoDamageTryDoDamageTryDoDamage");
        //this.targetAm =targetAm;
        //ac.attackerVec = targetAm.transform.forward;
        ac.attackerVec = damageData.DamageVec;
        if(damageData.AttackerAm != null)
            this.targetAm = damageData.AttackerAm;
        if(!photonView.IsMine)//人物父層才有photonView
            return;
        if(sm.isDie)
            return;
        float currentDamage= 0;
        if(!damageData.AbsorbOthers){
            currentDamage=(damageData.Damage*(2-sm.DEFBuff));
            // Debug.Log("自己受傷");
        }
        else{
            currentDamage=damageData.Damage;
            // Debug.Log("幫別人吸收");
        }
            
        if(sm.sb.AbsorbAm != null){
            if(sm.sb.AbsorbAm == this){
                sm.sb.AbsorbAm.photonView.RPC("AddAbsorbDamage", RpcTarget.All,currentDamage);
            }
            else{
                // sm.sb.AbsorbAm.photonView.RPC("AddAbsorbDamage", RpcTarget.All,currentDamage*1.5f);
                foreach(ResultListEntry result in GameManager.Instance.playersResult){
                    if(damageData.AttackerAm == result.am){
                        sm.sb.AbsorbAm.photonView.RPC("AbsorbTryDoDamage", RpcTarget.All,result.player,currentDamage*1.5f,damageData.BuffsName,damageData.DamageVec,true);
                        return;
                    }
                }
            }
        }
        //可以加targetAm狀態來判定要不要扣血
        if (sm.HP - currentDamage > 0)
        {
            sm.AllHurt += currentDamage;
            if (damageData.AttackerAm != null)
                damageData.AttackerAm.photonView.RPC("RPC_AddAllAttack", RpcTarget.All, currentDamage);
        }
        else
        {
            sm.AllHurt += sm.HP;
            if (damageData.AttackerAm != null)
                damageData.AttackerAm.photonView.RPC("RPC_AddAllAttack", RpcTarget.All, sm.HP);
        }
        
        sm.AddHP(-currentDamage);
        sm.AddRP(0.3f);
        HitOrDie(damageData);
    }
    [PunRPC]
    public void AbsorbTryDoDamage(Player player,float damage,string[] BuffsName,Vector3 damageVec,bool absorbOthers){
        foreach(ResultListEntry result in GameManager.Instance.playersResult){
            if(player == result.player){
                TryDoDamage(new DamageData(result.am,damage,BuffsName,damageVec,absorbOthers));
                return;
            }
        }
    }
    public void HitOrDie(DamageData damageData) {
        if (sm.HP < 0) {
            //Already dead
        }
        else {
            if (sm.HP > 0) {
                // Hit();
                //do some VFX, liked splatter blood...
                sm.sb.AddBuffsByStrings(damageData.BuffsName);
                // ac.attackerVec = targetAm.transform.forward;
            }
            else {
                // Die();
            }
        }
    }
    private void SetTag(Player p,ActorManager am){
        if(p.GetTeam() == PunTeams.Team.red){
            am.transform.tag = "Red";
            am.bm.bcH.tag = "Red";
            am.bm.bcB.tag = "Red";
            am.bm.bcL.tag = "Red";
            ac.VFX_Latenting_Y.SetActive(true);
            ac.VFX_Latenting_P.SetActive(false);
            // am.wm.wcL.transform.GetChild(0).tag = "Red";
            // am.wm.wcR.transform.GetChild(0).tag = "Red";
        }
            
        else if(p.GetTeam() == PunTeams.Team.blue){
            am.transform.tag = "Blue";
            am.bm.bcH.tag = "Blue";
            am.bm.bcB.tag = "Blue";
            am.bm.bcL.tag = "Blue";
            ac.VFX_Latenting_P.SetActive(true);
            ac.VFX_Latenting_Y.SetActive(false);
            // am.wm.wcL.transform.GetChild(0).tag = "Blue";
            // am.wm.wcR.transform.GetChild(0).tag = "Blue";
        }
    }
}
public class DamageData{
    public ActorManager AttackerAm;
    public float Damage;
    public string[] BuffsName;
    public Vector3 DamageVec;//方向
    public bool AbsorbOthers = false;
    public DamageData(ActorManager attackerAm,float damage,string[] BuffsName,Vector3 damageVec){
        this.AttackerAm = attackerAm;
        this.Damage = damage;
        this.BuffsName = BuffsName;
        this.DamageVec = damageVec;
    }
    public DamageData(ActorManager attackerAm,float damage,string[] BuffsName,Vector3 damageVec,bool absorbOthers){
        this.AttackerAm = attackerAm;
        this.Damage = damage;
        this.BuffsName = BuffsName;
        this.DamageVec = damageVec;
        this.AbsorbOthers = absorbOthers;
    }
}