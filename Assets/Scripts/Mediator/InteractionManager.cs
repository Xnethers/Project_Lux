using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*偵測Trigger，有無物件(帶有EventCasterManager)*/
public class InteractionManager : IActorManagerInterface {

    private CapsuleCollider interCol;

    public List<EventCasterManager> overlapEcastms = new List<EventCasterManager>();

	//// Use this for initialization
	void Start () {
        interCol = GetComponent<CapsuleCollider>();
	}

    //// Update is called once per frame
    //void Update () {

    //}
    private void OnTriggerStay(Collider col)
    {
        //print(col.name);
        EventCasterManager[] ecastms = col.GetComponents<EventCasterManager>();
        foreach (var ecastm in ecastms)
        {
            //print(ecastm.eventName);
            //print(ecastm.active);
            if (!overlapEcastms.Contains(ecastm)) {
                overlapEcastms.Add(ecastm);
            }
        }
    }
    private void OnTriggerExit(Collider col) {
        EventCasterManager[] ecastms = col.GetComponents<EventCasterManager>();
        foreach (var ecastm in ecastms) {
            if (overlapEcastms.Contains(ecastm)) {
                overlapEcastms.Remove(ecastm);
            }
        }
    }
}
