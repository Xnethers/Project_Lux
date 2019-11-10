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
        if(photonView.IsMine){
            SoundManager.Instance.EffectsSource = GetComponent<AudioSource>();
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
    [PunRPC]
    public void TryDoDamage(float damage) {
        //sm.HP -= 5;
        /*if (sm.HP > 0) {
            sm.AddHP(-5);
        }*/
        Debug.Log("TryDoDamageTryDoDamageTryDoDamage");
        //this.targetAm =targetAm;
        //ac.attackerVec = targetAm.transform.forward;
        if(!photonView.IsMine)//人物父層才有photonView
            return;
        if(sm.isDie)
            return;
        float currentDamage=(damage*(2-sm.DEFBuff));
        if(sm.sb.AbsorbAm != null){
            if(sm.sb.AbsorbAm == this){
                sm.sb.AbsorbAm.photonView.RPC("AddAbsorbDamage", RpcTarget.All,currentDamage);
            }
            else{
                // sm.sb.AbsorbAm.photonView.RPC("AddAbsorbDamage", RpcTarget.All,currentDamage*1.5f);
                sm.sb.AbsorbAm.photonView.RPC("TryDoDamage", RpcTarget.All,currentDamage*1.5f);
                return;
            }
            
        }
        //可以加targetAm狀態來判定要不要扣血
        sm.AddHP(-currentDamage);
        sm.AddRP(0.3f);
    }
    private void SetTag(Player p,ActorManager am){
        if(p.GetTeam() == PunTeams.Team.red){
            am.transform.tag = "Red";
            am.bm.bcH.tag = "Red";
            am.bm.bcB.tag = "Red";
            am.bm.bcL.tag = "Red";
            // am.wm.wcL.transform.GetChild(0).tag = "Red";
            // am.wm.wcR.transform.GetChild(0).tag = "Red";
        }
            
        else if(p.GetTeam() == PunTeams.Team.blue){
            am.transform.tag = "Blue";
            am.bm.bcH.tag = "Blue";
            am.bm.bcB.tag = "Blue";
            am.bm.bcL.tag = "Blue";
            // am.wm.wcL.transform.GetChild(0).tag = "Blue";
            // am.wm.wcR.transform.GetChild(0).tag = "Blue";
        }
    }
    public void SetTargetAm(ActorManager targetAm){
        this.targetAm = targetAm;
        ac.attackerVec = targetAm.transform.forward;
    }
}
