using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterIK : LookAtIK {
	public CharacterInventory characterInventory;

	public Transform l_Hand;
	public Transform l_Hand_Target;
	public Transform r_Hand;

	public Quaternion lh_rot;

	public float rh_Weight;

	public Transform shoulder;
	public Transform aimPivot;
	Quaternion rotRight;
	Quaternion rotRightAim;
	// Use this for initialization
	protected override void Start(){
		base.Start();
		shoulder = ac.anim.GetBoneTransform(HumanBodyBones.RightShoulder).transform;

		aimPivot = new GameObject().transform;
		aimPivot.name = "aim pivot";
		aimPivot.transform.parent = transform;

		r_Hand = new GameObject().transform;
		r_Hand.name = "right hand";
		r_Hand.transform.parent = aimPivot;

		l_Hand = new GameObject().transform;
		l_Hand.name = "left hand";
		l_Hand.transform.parent = aimPivot;

		r_Hand.localPosition = characterInventory.firstWeapon.rHandPos;
		rotRight = Quaternion.Euler(characterInventory.firstWeapon.rHandRot.x,characterInventory.firstWeapon.rHandRot.y,characterInventory.firstWeapon.rHandRot.z);
		r_Hand.localRotation = rotRight;
		
		rotRightAim = Quaternion.Euler(characterInventory.secondWeapon.rHandRot.x,characterInventory.secondWeapon.rHandRot.y,characterInventory.secondWeapon.rHandRot.z);
	}
	
	// Update is called once per frame
	void Update () {
		// lh_rot = l_Hand.rotation;
		lh_rot = l_Hand_Target.rotation;
		l_Hand.position = l_Hand_Target.position;
		// l_Hand.rotation = lh_rot;

		if(ac.camcon.doAim){
			// rh_Weight +=Time.deltaTime *2;
			r_Hand.localRotation = rotRightAim;
			r_Hand.localPosition = characterInventory.secondWeapon.rHandPos;
		}	
		else{
			// rh_Weight -=Time.deltaTime *2;
			r_Hand.localRotation = rotRight;
			r_Hand.localPosition = characterInventory.firstWeapon.rHandPos;
		}	
		// rh_Weight = Mathf.Clamp(rh_Weight,0,1);
	}
	protected override void OnAnimatorIK(int layerIndex)
	{
		if(ac.am.sm.isDie)
			return;
		//頭
		base.OnAnimatorIK(layerIndex);

		aimPivot.position = shoulder.position;
		aimPivot.LookAt(Target);

		/*if(ac.camcon.doAim){
			aimPivot.LookAt(Target);
			ac.anim.SetLookAtWeight(1f,.4f,1f);
		}
		else
		{
			ac.anim.SetLookAtWeight(.3f,.3f,.3f);
		}
		ac.anim.SetLookAtPosition(Target.position);*/

		ac.anim.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1f);
		ac.anim.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1f);
		ac.anim.SetIKPosition(AvatarIKGoal.LeftHand, l_Hand.position);
		ac.anim.SetIKRotation(AvatarIKGoal.LeftHand, lh_rot);

		ac.anim.SetIKPositionWeight(AvatarIKGoal.RightHand, rh_Weight);
		ac.anim.SetIKRotationWeight(AvatarIKGoal.RightHand, rh_Weight);
		ac.anim.SetIKPosition(AvatarIKGoal.RightHand, r_Hand.position);
		ac.anim.SetIKRotation(AvatarIKGoal.RightHand, r_Hand.rotation);
	}
}
