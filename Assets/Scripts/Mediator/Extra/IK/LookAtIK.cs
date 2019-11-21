using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class LookAtIK : MonoBehaviourPunCallbacks {
	private Transform camAimTarget;
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
		if(photonView.IsMine)
            Target.position = camAimTarget.position;
	}
}
