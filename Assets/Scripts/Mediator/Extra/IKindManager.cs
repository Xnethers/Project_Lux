using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class IKindManager : MonoBehaviourPunCallbacks {
	public IKindManager targetAm;
	public ActorController ac;

    [Header("=== Auto Generate if Null ===")]
    public BattleManager bm;
    public WeaponManager wm;
    public StateManager sm;
	protected T Bind<T>(GameObject go) where T : IActorManagerInterface {
        T tempInstance;
        tempInstance = go.GetComponent<T>();
        if (tempInstance == null) {
            tempInstance = go.AddComponent<T>();
        }
        //tempInstance.am = this;
        return tempInstance;
    }
	public virtual void TryDoDamage(IKindManager targetAm,float damage){}
}
