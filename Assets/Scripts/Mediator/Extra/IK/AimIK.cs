using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AimIK : LookAtIK {
	// public Transform LeftHandTarget;
	public float aimDenominator = 1.5f;
	public Vector3 leftShoulderOffest;
	public Vector3 rightShoulderOffest;
	Transform leftShoulder;
	Transform rightShoulder;
	// public float leftOffest=5f;
	
	protected override void Start(){
		base.Start();
		leftShoulder =ac.anim.GetBoneTransform(HumanBodyBones.LeftShoulder);
		rightShoulder =ac.anim.GetBoneTransform(HumanBodyBones.RightShoulder);
		// LeftHandTarget = transform.DeepFind("LeftHandTarget");
	}
	void LateUpdate () {
		if(ac.am.sm.isDie)
			return;
		rightShoulderOffest.x= -ac.camcon.tempEulerX/aimDenominator;
		leftShoulderOffest.x= ac.camcon.tempEulerX/aimDenominator;
		// if(ac.camcon.doAim)
		// 	leftShoulderOffest.x= ac.camcon.tempEulerX/aimDenominator;
		// else
		// 	leftShoulderOffest.x= ac.camcon.tempEulerX/aimDenominator+leftOffest;
		leftShoulder.rotation=leftShoulder.rotation * Quaternion.Euler(leftShoulderOffest);
		rightShoulder.rotation=rightShoulder.rotation * Quaternion.Euler(rightShoulderOffest);
	}
	protected override void OnAnimatorIK(int layerIndex)
	{
		//頭
		base.OnAnimatorIK(layerIndex);
		if(ac.am.sm.isDie)
			return;
		
		if(ac.camcon.doAim){
			// ac.anim.SetIKPosition(AvatarIKGoal.LeftHand, LeftHandTarget.position);
			// ac.anim.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1f);

			// ac.anim.SetIKPosition(AvatarIKGoal.RightHand, HandTarget.position);
			// ac.anim.SetIKPositionWeight(AvatarIKGoal.RightHand, 1);
		}
	}
}
