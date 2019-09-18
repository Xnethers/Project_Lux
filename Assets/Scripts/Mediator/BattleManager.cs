using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/* 角色碰撞管理 */
[RequireComponent(typeof(CapsuleCollider))]
public class BattleManager : IActorManagerInterface {
	public BattleController bcH;
    public BattleController bcB;

	private void Awake(){
		GameObject hHandle,bHandle;
		hHandle = am.ac.model.transform.DeepFind("headHandle").gameObject;
        bHandle = am.ac.model.transform.DeepFind("bodyHandle").gameObject;
		bcH = BindBattleController(hHandle);
        bcB = BindBattleController(bHandle);
	}
	public BattleController BindBattleController(GameObject targetObj) {
        BattleController tempBc;
        tempBc = targetObj.GetComponent<BattleController>();
        if (tempBc == null) {
            tempBc = targetObj.AddComponent<BattleController>();
        }
        tempBc.bm = this;
        return tempBc;
    }
}
