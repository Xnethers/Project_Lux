using UnityEngine;
using System;
using System.Collections;

[RequireComponent(typeof(Animator))]

public class SniperIKCtrl : MonoBehaviour
{

    //动画控制
    protected Animator animator;
    //是否开始IK动画
    public bool ikActive = false;
    //右手子节点参考的目标
    public Transform rightHandObj = null;
    public Transform leftHandObj = null;
    public Transform Gun = null;

    public SniperController sc;
    public Transform Dir;
    GameObject Dir_object;


    void Start()
    {
        //得到动画控制对象
        animator = GetComponent<Animator>();

        Dir_object = new GameObject();
        Dir_object.transform.rotation = rightHandObj.localRotation;
        Dir_object.transform.parent = rightHandObj;
        Dir_object.transform.position = rightHandObj.root.position;

        // Dir_object.transform.rotation = Quaternion.Inverse(rightHandObj.rotation);
        Dir_object.name = "Direction";
        Dir = Dir_object.transform;
        Debug.Log(rightHandObj.eulerAngles);

        //原本的eulerAngles + camera.eulerAngles.y;

    }

    //a callback for calculating IK
    //它是回调访法。
    //前提是在Unity导航菜单栏中打开Window->Animator打开动画控制器窗口，在这里必须勾选IK Pass！！！
    /// <summary>
    /// Callback for setting up animation IK (inverse kinematics).
    /// </summary>
    /// <param name="layerIndex">Index of the layer on which the IK solver is called.</param>
    void OnAnimatorIK(int layerIndex)
    {
        if (animator)
        {

            //if the IK is active, set the position and rotation directly to the goal. 
            //即或IK动画后开始让右手节点寻找参考目标。 
            if (ikActive)
            {

                //weight = 1.0 for the right hand means position and rotation will be at the IK goal (the place the character wants to grab)
                //设置骨骼的权重，1表示完整的骨骼，如果是0.5哪么骨骼权重就是一半呢，可移动或旋转的就是一半，大家可以修改一下参数试试。
                animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 0.5f);
                animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 0.5f);
                animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 1f);
                animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1f);

                //set the position and the rotation of the right hand where the external object is
                if (rightHandObj != null)
                {
                    Vector3 Targetpos = sc.RayAim();
                    Dir.LookAt(Targetpos, -rightHandObj.transform.up);


                    //设置右手根据目标点而旋转移动父骨骼节点
                    animator.SetIKPosition(AvatarIKGoal.RightHand, Targetpos);
                    animator.SetIKPosition(AvatarIKGoal.LeftHand, Targetpos);

                    animator.SetIKRotation(AvatarIKGoal.RightHand, Dir.rotation);
                    // Gun.rotation = Quaternion.LookRotation(Targetpos - Gun.position);
                    //animator.SetIKRotation(AvatarIKGoal.LeftHand, Dir.rotation);
                }



            }

            //if the IK is not active, set the position and rotation of the hand back to the original position
            //如果取消IK动画，哪么重置骨骼的坐标。
            else
            {
                animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 0);
                animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 0);
                animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 0);
                animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 0);
            }
        }
    }
}