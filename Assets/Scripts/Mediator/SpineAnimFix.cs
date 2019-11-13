using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpineAnimFix : MonoBehaviour {

    private Animator anim;
    private ActorController ac;
    public Vector3 a;
    public Vector3 b;
    public Vector3 c;
    private void Awake()
    {
        anim = GetComponent<Animator>();
        ac = GetComponentInParent<ActorController>();
    }
    private void OnAnimatorIK()
    {
        Transform spine = anim.GetBoneTransform(HumanBodyBones.Spine);
        // if(ac.CheckStateTag("attackAim","attack")){
        //     if(ac.pi.isMove){
        //         if(anim.GetFloat("right")<-0.1f && anim.GetFloat("forward")>0.1f)
        //             spine.localEulerAngles += 0.75f * b;
        //         if(anim.GetFloat("right")==0 && anim.GetFloat("forward")>0.1f)
        //             spine.localEulerAngles += 0.75f * c;
        //         if(anim.GetFloat("right")!=0)
        //             spine.localEulerAngles += 0.75f * a;
        //     }
        //     else{
        //         //spine.localEulerAngles = new Vector3(0,Mathf.Lerp(spine.localEulerAngles.y,0,0.1f),0);
        //     }
        // }
        anim.SetBoneLocalRotation(HumanBodyBones.Spine, Quaternion.Euler(spine.localEulerAngles));
        
    }
}
