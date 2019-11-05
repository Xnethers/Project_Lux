using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldOfView : MonoBehaviour {

    public float viewRadius;
    [Range(0,360)]
    public float viewAngle;

    public LayerMask targetMask;
    public LayerMask obstacleMask;

    //[HideInInspector]
    public List<ActorManager> visibleTargets = new List<ActorManager>();
    public List<ActorManager> useTargets = new List<ActorManager>();
    protected virtual void Start()
    {
        StartFind(.2f);
        // StartCoroutine(FindTargetsWithDelay(.2f));
        // Debug.Log("?????????????????????????????????????????????");
    }
    public virtual void Initialize(ActorManager am){

    }
    public virtual IEnumerator FindTargetsWithDelay(float delay)
    {
        while (true)
        {
            yield return new WaitForSeconds(delay);
            FindVisibleTargets();
            FindUseTargets();
        }
    }

    public void FindVisibleTargets()
    {
        visibleTargets.Clear();
        Transform target=null;
        Collider[] targetsInViewRadius = Physics.OverlapSphere(transform.position, viewRadius ,targetMask);
        for (int i = 0; i < targetsInViewRadius.Length; i++)
        {
            target = targetsInViewRadius[i].transform;
            Vector3 dirToTarget = (target.position - transform.position).normalized;
            if (Vector3.Angle(transform.forward, dirToTarget) < viewAngle / 2)
            {
                float dsToTarget = Vector3.Distance(transform.position, target.position);
                if (!Physics.Raycast((transform.position), dirToTarget, dsToTarget, obstacleMask))
                {
                    ActorManager tempAm=target.GetComponent<ActorManager>();
                    visibleTargets.Add(tempAm);
                }
            }
        }
    }
    public Vector3 DirFromAngle(float angleInDegrees,bool angleIsGlobal)
    {
        if (!angleIsGlobal)
        {
            angleInDegrees+= transform.eulerAngles.y;
        }
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }
    public virtual void FindUseTargets(){
        
    }
    public virtual void StartFind(float delay){
		StartCoroutine(FindTargetsWithDelay(delay));
	}
	public virtual void StopFind(float delay){
		StopCoroutine(FindTargetsWithDelay(delay));
	}
	public virtual void TargetsListClear(){
		visibleTargets.Clear();
        useTargets.Clear();
	}

}
