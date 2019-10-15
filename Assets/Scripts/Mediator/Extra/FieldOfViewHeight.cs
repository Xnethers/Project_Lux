using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldOfViewHeight : FieldOfView {
	// private CharacterController chacon;
	// bool m_Started;
	// Use this for initialization
	void Start () {
		// m_Started = true;
        // chacon = GetComponent<CharacterController>();
		// StartFind();
	}
	public override void FindUseTargets(){
		useTargets.Clear();
		// Collider[] sameHeightObjs = Physics.OverlapBox(transform.position,new Vector3(viewRadius*2,chacon.height*1.5f,viewRadius*2),Quaternion.identity,targetMask);
		for (int i = 0; i < visibleTargets.Count; i++){
			ActorManager tempAm=visibleTargets[i];
			if(tempAm.tag !=tag){
				if(Mathf.Abs(transform.position.y-visibleTargets[i].transform.position.y)<viewRadius/3){
					useTargets.Add(tempAm);
				}
			}
			// foreach(Collider sameHeight in sameHeightObjs){
			// 	if(sameHeight.gameObject == tempAm.gameObject){
			// 		sameHeightTargets.Add(tempAm);
			// 	}
			// }
		}
	}

	//Draw the Box Overlap as a gizmo to show where it currently is testing. Click the Gizmos button to see this
    // void OnDrawGizmos()
    // {
    //     Gizmos.color = Color.red;
    //     //Draw a cube where the OverlapBox is (positioned where your GameObject is as well as a size)
    //      //Check that it is being run in Play Mode, so it doesn't try to draw this in Editor mode
    //     if (m_Started)
    //         Gizmos.DrawWireCube(transform.position,new Vector3(viewRadius*2,chacon.height*1.5f,viewRadius*2));
    // }
}
