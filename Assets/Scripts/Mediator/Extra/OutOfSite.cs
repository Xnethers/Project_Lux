using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutOfSite : MonoBehaviour {

	void OnTriggerEnter(Collider other)
	{
		ActorManager am = other.GetComponent<ActorManager>();
		if(am != null)
		{
			am.sm.HP=0;
			am.targetAm=null;
		}
	}
}
