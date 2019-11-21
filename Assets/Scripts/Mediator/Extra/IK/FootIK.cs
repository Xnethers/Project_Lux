using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootIK : MonoBehaviour {
	public Animator anim; //Animator
	private Vector3 rightFootPosition, leftFootPosition, leftFootIkPosition, rightFootIkPosition;
    private Quaternion leftFootIkRotation, rightFootIkRotation;
    private float lastPelvisPositionY, lastRightFootPositionY, lastLeftFootPositionY;

    [Header("Feet Grounder")]
    public bool enableFeetIk = true;
    [Range(0, 2)] [SerializeField] private float heightFromGroundRaycast = 1.14f;
    [Range(0, 2)] [SerializeField] private float raycastDownDistance = 1.5f;
    [SerializeField] private LayerMask environmentLayer;
    [SerializeField] private float pelvisOffset = 0f;
    [Range(0, 1)] [SerializeField] private float pelvisUpAndDownSpeed = 0.28f;
    [Range(0, 1)] [SerializeField] private float feetToIkPositionSpeed = 0.5f;
    [Range(0, 1)] [SerializeField] private float weightIkPositionSpeed = 0.75f;

    public string leftFootAnimVariableName = "LeftFootCurve";
    public string rightFootAnimVariableName = "RightFootCurve";

    public bool useProIkFeature = false;
    public bool showSolverDebug = true;

    ActorController ac;
    [SerializeField] private float _RaycastOriginY = 0.5f;
    [SerializeField] private float _RaycastEndY = -0.1f;
    private float _weight;

	void Start()
    {
        anim = this.GetComponent<Animator>();
        ac = GetComponentInParent<ActorController>();
	}
	#region FeetGrounding

    /// <summary>
    /// We are updating the AdjustFeetTarget method and also find the position of each foot inside our Solver Position.
    /// </summary>
    private void FixedUpdate()
    {
        if(enableFeetIk == false) { return; }
        if(anim == null) { return; }

        AdjustFeetTarget(ref rightFootPosition, HumanBodyBones.RightFoot);
        AdjustFeetTarget(ref leftFootPosition, HumanBodyBones.LeftFoot);

        //find and raycast to the ground to find positions
        FeetPositionSolver(rightFootPosition, ref rightFootIkPosition, ref rightFootIkRotation); // handle the solver for right foot
        FeetPositionSolver(leftFootPosition, ref leftFootIkPosition, ref leftFootIkRotation); //handle the solver for the left foot

    }

    private void OnAnimatorIK(int layerIndex)
    {
        if(enableFeetIk == false) { return; }
        if(anim == null) { return; }
        /*if(ac._velocity.y<=0 && !ac.am.sm.isGround)
            _weight=0;
        else
        {
            if(ac.pi.Dmag!=0){// && ac.height>.1f //ac.pi.isMove //
                // UpdateFootIK(AvatarIKGoal.LeftFoot, anim.GetFloat(leftFootAnimVariableName), HumanBodyBones.LeftFoot, anim.leftFeetBottomHeight);
                // UpdateFootIK(AvatarIKGoal.RightFoot, anim.GetFloat(rightFootAnimVariableName), HumanBodyBones.RightFoot, anim.rightFeetBottomHeight);
                // return;
                _weight=.6f;
            }
            else
                _weight=1f;
        }*/
        if(ac.pi.isMove || ac.height>.1f || ac.am.sm.isDie)
            _weight=0f;
        else
            _weight=1f;
        
        MovePelvisHeight();

        //right foot ik position and rotation -- utilise the pro features in here
        anim.SetIKPositionWeight(AvatarIKGoal.RightFoot, Mathf.Lerp(anim.GetIKPositionWeight(AvatarIKGoal.RightFoot),_weight,weightIkPositionSpeed));

        if(useProIkFeature)
        {
            anim.SetIKRotationWeight(AvatarIKGoal.RightFoot, anim.GetFloat(rightFootAnimVariableName));
        }

        MoveFeetToIkPoint(AvatarIKGoal.RightFoot, rightFootIkPosition, rightFootIkRotation, ref lastRightFootPositionY);

        //left foot ik position and rotation -- utilise the pro features in here
        anim.SetIKPositionWeight(AvatarIKGoal.LeftFoot, Mathf.Lerp(anim.GetIKPositionWeight(AvatarIKGoal.LeftFoot),_weight,weightIkPositionSpeed));

        if (useProIkFeature)
        {
            anim.SetIKRotationWeight(AvatarIKGoal.LeftFoot, anim.GetFloat(leftFootAnimVariableName));
        }

        MoveFeetToIkPoint(AvatarIKGoal.LeftFoot, leftFootIkPosition, leftFootIkRotation, ref lastLeftFootPositionY);
    }

    #endregion



    #region FeetGroundingMethods

    /// <summary>
    /// Moves the feet to ik point.
    /// </summary>
    /// <param name="foot">Foot.</param>
    /// <param name="positionIkHolder">Position ik holder.</param>
    /// <param name="rotationIkHolder">Rotation ik holder.</param>
    /// <param name="lastFootPositionY">Last foot position y.</param>
    void MoveFeetToIkPoint (AvatarIKGoal foot, Vector3 positionIkHolder, Quaternion rotationIkHolder, ref float lastFootPositionY)
    {
        Vector3 targetIkPosition = anim.GetIKPosition(foot);

        if(positionIkHolder != Vector3.zero)
        {
            targetIkPosition = transform.InverseTransformPoint(targetIkPosition);
            positionIkHolder = transform.InverseTransformPoint(positionIkHolder);

            float yVariable = Mathf.Lerp(lastFootPositionY, positionIkHolder.y, feetToIkPositionSpeed);
            targetIkPosition.y += yVariable;

            lastFootPositionY = yVariable;

            targetIkPosition = transform.TransformPoint(targetIkPosition);

            anim.SetIKRotation(foot, rotationIkHolder);
        }

        anim.SetIKPosition(foot, targetIkPosition);
    }
    /// <summary>
    /// Moves the height of the pelvis.
    /// </summary>
    private void MovePelvisHeight()
    {

        if(rightFootIkPosition == Vector3.zero || leftFootIkPosition == Vector3.zero || lastPelvisPositionY == 0)
        {
            lastPelvisPositionY = anim.bodyPosition.y;
            // lastPelvisPositionY = transform.position.y;
            return;
        }
    
        float lOffsetPosition = leftFootIkPosition.y - transform.position.y;
        float rOffsetPosition = rightFootIkPosition.y - transform.position.y;

        float totalOffset = (lOffsetPosition < rOffsetPosition) ? lOffsetPosition : rOffsetPosition;

        Vector3 newPelvisPosition = anim.bodyPosition + Vector3.up * totalOffset * (1-ac.pi.Dmag);//

        newPelvisPosition.y = Mathf.Lerp(lastPelvisPositionY, newPelvisPosition.y, pelvisUpAndDownSpeed);

        anim.bodyPosition = newPelvisPosition;

        lastPelvisPositionY = anim.bodyPosition.y;
        // lastPelvisPositionY = transform.position.y;
    }

    /// <summary>
    /// We are locating the Feet position via a Raycast and then Solving
    /// </summary>
    /// <param name="fromSkyPosition">From sky position.</param>
    /// <param name="feetIkPositions">Feet ik positions.</param>
    /// <param name="feetIkRotations">Feet ik rotations.</param>
    private void FeetPositionSolver(Vector3 fromSkyPosition, ref Vector3 feetIkPositions, ref Quaternion feetIkRotations)
    {
        //raycast handling section 
        RaycastHit feetOutHit;

        if (showSolverDebug)
            Debug.DrawLine(fromSkyPosition, fromSkyPosition + Vector3.down * (raycastDownDistance + heightFromGroundRaycast), Color.yellow);

        if (Physics.Raycast(fromSkyPosition, Vector3.down, out feetOutHit, raycastDownDistance + heightFromGroundRaycast, environmentLayer))
        {
            //finding our feet ik positions from the sky position
            feetIkPositions = fromSkyPosition;
            feetIkPositions.y = feetOutHit.point.y + pelvisOffset;
            feetIkRotations = Quaternion.FromToRotation(Vector3.up, feetOutHit.normal) * transform.rotation;

            return;
        }

        feetIkPositions = Vector3.zero; //it didn't work :(

    }
    /// <summary>
    /// Adjusts the feet target.
    /// </summary>
    /// <param name="feetPositions">Feet positions.</param>
    /// <param name="foot">Foot.</param>
    private void AdjustFeetTarget (ref Vector3 feetPositions, HumanBodyBones foot)
    {
        feetPositions = anim.GetBoneTransform(foot).position;
        feetPositions.y = transform.position.y + heightFromGroundRaycast;

    }

    #endregion
    private void UpdateFootIK(AvatarIKGoal goal,float weight, HumanBodyBones foot , float footBottomHeight)//ExposedCurve curve
    {
        // var weight = curve.Evaluate(_Animancer);
        anim.SetIKPositionWeight(goal, weight);
        anim.SetIKRotationWeight(goal, weight);

        if (weight == 0)
            return;
        Transform footTransform;
        footTransform = anim.GetBoneTransform(foot);
        var position = footTransform.position;
        position.y = transform.position.y + _RaycastOriginY;

        var distance = _RaycastOriginY - _RaycastEndY;

        RaycastHit hit;
        if (Physics.Raycast(position, Vector3.down, out hit, distance))
        {
            // Use the hit point as the desired position.
            position = hit.point;
            position.y += footBottomHeight;
            anim.SetIKPosition(goal, position + new Vector3(0,pelvisOffset,0));

            // Use the hit normal to calculate the desired rotation.
            var rotation = anim.GetIKRotation(goal);
            var localUp = rotation * Vector3.up;

            var rotAxis = Vector3.Cross(localUp, hit.normal);
            var angle = Vector3.Angle(localUp, hit.normal);
            rotation = Quaternion.AngleAxis(angle, rotAxis) * rotation;

            anim.SetIKRotation(goal, rotation);
        }
        // Otherwise simply stretch the leg out to the end of the ray.
        else
        {
            position.y -= distance;
            position.y += footBottomHeight;
            anim.SetIKPosition(goal, position + new Vector3(0,pelvisOffset,0));
        }
    }
}
