using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class LookAtIK : MonoBehaviourPunCallbacks {
	protected Transform camAimTarget;
	public Transform Target;
	protected ActorController ac;
	// Use this for initialization
	protected virtual void Start () {
		ac = GetComponentInParent<ActorController>();
		if(photonView.IsMine)
            camAimTarget = Camera.main.transform.DeepFind("AimTarget");
	}
	protected virtual void OnAnimatorIK(int layerIndex)
	{
		if(ac.am.sm.isDie)
			return;
		//頭
		ac.anim.SetLookAtPosition(Target.position);
		ac.anim.SetLookAtWeight(1);
		if(photonView.IsMine){
			// camAimTarget.localPosition = new Vector3(ac.camcon.offsetXDistance - ac.camcon.transform.parent.localPosition.x,camAimTarget.localPosition.y,camAimTarget.localPosition.z);
			Target.position = camAimTarget.position;
		}
            
	}
}
