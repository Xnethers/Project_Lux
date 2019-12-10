using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoviceTeachManager : IFlowchart {
	public GameObject rangeAttack;
	public GameObject AI;
	public GameObject rangeLatent;
	public GameObject Latent;
	public OccupiedTest occupied;
	public TowerHealth tower;
	public bool isLatentFinish;
	public bool isOccupiedFinish;
	public bool isTowerFinish;
	private PlayerUI playerUI;
	public override void Start(){
		base.Start();
		rangeAttack.SetActive(false);
		AI.SetActive(false);
		rangeLatent.SetActive(false);
		Latent.SetActive(false);
		occupied.redValue = 85;
		occupied.gameObject.SetActive(false);
		tower.gameObject.SetActive(false);
	}
	void Update(){
		if(!isLatentFinish && GameManager.Instance.GameMenu.PlayerAm.ac.latentCount>=3){
			flowchart.StopAllBlocks();
			Fungus.Flowchart.BroadcastFungusMessage("OccupiedTower");
			isLatentFinish = true;
			if(playerUI == null){
				playerUI = GameManager.Instance.GameMenu.PlayerAm.GetComponent<PlayerUI>();
				playerUI.occupied = occupied;
			}
		}
		if(!isOccupiedFinish && occupied.occupiedState == OccupiedTest.OccupiedState.Red){
			flowchart.StopAllBlocks();
			Fungus.Flowchart.BroadcastFungusMessage("OccupiedFinish");
			isOccupiedFinish = true;
		}
		if(!isTowerFinish && GameManager.Instance.isResult){
			flowchart.StopAllBlocks();
			Fungus.Flowchart.BroadcastFungusMessage("TowerFinish");
			isTowerFinish = true;
		}
	}
	public void rangeAttackActive(){
		rangeAttack.SetActive(true);
	}
	public void aitActive(){
		AI.SetActive(true);
	}
	public void rangeLatentActive(){
		rangeLatent.SetActive(true);
	}
	public void LatentActive(){
		Latent.SetActive(true);
	}
	public void OccupiedTowerActive(){
		occupied.gameObject.SetActive(true);
		tower.gameObject.SetActive(true);
	}
}
