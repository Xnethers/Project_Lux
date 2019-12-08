using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangeTrigger : IFlowchart {
	public string rangeName;
	void OnTriggerEnter(Collider other)
	{
		flowchart.StopAllBlocks();
		if(rangeName == "Attack"){
			Fungus.Flowchart.BroadcastFungusMessage("Attack");
		}
		if(rangeName == "Latent"){
			Fungus.Flowchart.BroadcastFungusMessage("Latent");
		}
		this.gameObject.SetActive(false);
	}
}
