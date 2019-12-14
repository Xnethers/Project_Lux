using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum LatentType
{
	Vertical,
	Horizontal
}
public class EventCasterManager : IActorManagerInterface {
	public LatentType latentType;
	public GameObject ColliderObj;
    // public string eventName;
    // public bool active;
	// public Vector3 offset = new Vector3(0,0,0.5f);
	public float offset=0.2f;
	//// Use this for initialization
	void Start () {
        if (am == null) {
            am = GetComponentInParent<ActorManager>();
        }
		if(ColliderObj!=null){
			ColliderObj.SetActive(false);
		}
	}
	
	//// Update is called once per frame
	//void Update () {
		
	//}
	/// <summary>
	/// OnTriggerStay is called once per frame for every Collider other
	/// that is touching the trigger.
	/// </summary>
	/// <param name="other">The other Collider involved in this collision.</param>
	void OnTriggerStay(Collider other)
	{
		ActorManager am = other.GetComponent<ActorManager>();
		if(am != null && am.ac.pi.isLatent)
		{
			if (!am.sm.isHPing)
				am.sm.StartAddHp(1,1/am.sm.HOT*am.sm.HOTBuff);//加1滴為單位
		}
	}
}
