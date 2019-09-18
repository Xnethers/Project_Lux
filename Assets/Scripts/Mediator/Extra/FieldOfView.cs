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
    void Start()
    {
        StartCoroutine(FindTargetsWithDelay(.2f));
    }

    IEnumerator FindTargetsWithDelay(float delay)
    {
        while (true)
        {
            yield return new WaitForSeconds(delay);
            FindVisibleTargets();
        }
    }

    void FindVisibleTargets()
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
                    if(target.tag != this.tag){
                        ActorManager tempAm=target.GetComponent<ActorManager>();
                        visibleTargets.Add(tempAm);
                    }
                        
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
}
