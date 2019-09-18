using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour {

    public WeaponManager wm;
	public WeaponData[] wdata;

	//// Use this for initialization
	void Awake () {
		wdata=GetComponentsInChildren<WeaponData>();
		// if(wdata.Length>1)
		// 	wdata[1].gameObject.SetActive(false);
	}
	// public float GetATK(){
	// 	Debug.Log(wdata.ATK + wm.am.sm.ATK);
	// 	return wdata.ATK + wm.am.sm.ATK;
	// }
	
	//// Update is called once per frame
	//void Update () {
		
	//}
}
