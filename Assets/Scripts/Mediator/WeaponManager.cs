using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

/*管理武器 */
public class WeaponManager : IActorManagerInterface {

    //public ActorManager am;
    // private Collider weaponColL;
    // private Collider weaponColR;

    public GameObject whL;
    public GameObject whR;

    public WeaponController wcL;
    public WeaponController wcR;
    private void Awake()
    {
        whL = transform.DeepFind("weaponHandleL").gameObject;
        whR = transform.DeepFind("weaponHandleR").gameObject;

        wcL = BindWeaponController(whL);
        wcR = BindWeaponController(whR);

        // weaponColL = whL.GetComponentInChildren<Collider>();
        // weaponColR = whR.GetComponentInChildren<Collider>();

        // weaponColL.enabled = false;
        // weaponColR.enabled = false;
    }

    public WeaponController BindWeaponController(GameObject targetObj) {
        WeaponController tempWc;
        tempWc = targetObj.GetComponent<WeaponController>();
        if (tempWc == null) {
            tempWc = targetObj.AddComponent<WeaponController>();
        }
        tempWc.wm = this;
        return tempWc;
    }
    // public void WeaponEnable() {
    //     if (am.ac.CheckStateTag("attackL")) {
    //         weaponColL.enabled = true;
    //         //print("WeaponEnableL");
    //     }
    //     else {
    //         weaponColR.enabled = true;
    //     }
    // }

    // public void WeaponDisable()
    // {
    //     weaponColL.enabled = false;
    //     weaponColR.enabled = false;
    //     //print("WeaponDisable");
    // }
}
