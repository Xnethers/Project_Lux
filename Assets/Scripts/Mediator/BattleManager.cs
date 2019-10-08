using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/* 角色碰撞管理 */
[RequireComponent(typeof(CapsuleCollider))]
public class BattleManager : IActorManagerInterface {
    
	public BattleController bcH;
    public BattleController bcB;
    public BattleController bcL;
    public float[] OriginColValue;
    public CapsuleCollider latentCol;
    
	private void Awake(){
		GameObject hHandle,bHandle,lHandle;
		hHandle = am.ac.model.transform.DeepFind("headHandle").gameObject;
        bHandle = am.ac.model.transform.DeepFind("bodyHandle").gameObject;
        lHandle = am.transform.DeepFind("bodyLatent").gameObject;
		bcH = BindBattleController(hHandle);
        bcB = BindBattleController(bHandle);
        bcL = BindBattleController(lHandle);
        latentCol = bcL.GetComponent<CapsuleCollider>();
        OriginColValue = new float[3];
        OriginColValue[0] = am.ac.chacon.center.y;
        OriginColValue[1] = am.ac.chacon.radius;
        OriginColValue[2] = am.ac.chacon.height;
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
    public void SetChacontrollerSize(bool isLatent){
        if(isLatent){
            am.ac.chacon.center = latentCol.center;
            am.ac.chacon.radius = latentCol.radius;
            am.ac.chacon.height = latentCol.height;
        }
        else
        {
            am.ac.chacon.center = new Vector3(0,OriginColValue[0],0);
            am.ac.chacon.radius = OriginColValue[1];
            am.ac.chacon.height = OriginColValue[2];
        }
        
    }
}
