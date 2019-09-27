using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldOfViewHeight : FieldOfView {
	private CharacterController chacon;
	public List<ActorManager> sameHeightTargets = new List<ActorManager>();
	bool m_Started;
	// Use this for initialization
	void Start () {
		m_Started = true;
        chacon = GetComponent<CharacterController>();
		
	}
	
	public void StartFind(){
		StartCoroutine(FindTargetsWithDelay(.2f));
	}
	public void StopFind(){
		StopCoroutine(FindTargetsWithDelay(.2f));
	}
	public override IEnumerator FindTargetsWithDelay(float delay)
    {
        while (true)
        {
            yield return new WaitForSeconds(delay);
            FindVisibleTargets();
			FindSameHeightTargets();
        }
    }
	void FindSameHeightTargets(){
		sameHeightTargets.Clear();
		Collider[] sameHeightObjs = Physics.OverlapBox(transform.position,new Vector3(viewRadius*2,chacon.height*1.5f,viewRadius*2),Quaternion.identity,targetMask);
		for (int i = 0; i < visibleTargets.Count; i++){
			ActorManager tempAm=visibleTargets[i].GetComponent<ActorManager>();
			foreach(Collider sameHeight in sameHeightObjs){
				if(sameHeight.gameObject == tempAm.gameObject){
					sameHeightTargets.Add(tempAm);
				}
			}
		}
	}

	//Draw the Box Overlap as a gizmo to show where it currently is testing. Click the Gizmos button to see this
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        //Draw a cube where the OverlapBox is (positioned where your GameObject is as well as a size)
         //Check that it is being run in Play Mode, so it doesn't try to draw this in Editor mode
        if (m_Started)
            Gizmos.DrawWireCube(transform.position,new Vector3(viewRadius*2,chacon.height*1.5f,viewRadius*2));
    }
}
