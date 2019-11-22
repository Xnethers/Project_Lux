using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class TrainingCharacter : InGameMenu {
	public GameObject PlayerUI;
	public override void Update()
	{
		base.Update();
		/*if(PlayerAm == null){
			ActorManager[] Ams = FindObjectsOfType<ActorManager>();
			foreach(ActorManager tempAm in Ams){
				if(!tempAm.ac.pi.isAI){
					PlayerAm = tempAm;
					PlayerUI = PlayerAm.GetComponent<PlayerUI>().ScreenCanvas;
				}
			}
		}*/
		if(PlayerUI == null)
			PlayerUI = PlayerAm.GetComponent<PlayerUI>().ScreenCanvas;
		if(isMenu){
			PlayerUI.gameObject.SetActive(false);
		}
		else{
			PlayerUI.gameObject.SetActive(true);
		}
			
	}
	public void OnClickCharacter(int whichCharacter){
		PlayerInfo.PI.mySelectedCharacter = whichCharacter;
	}
	public void OnClickChoose(){
		GameManager.Instance.DestroyCharacter(PlayerAm.gameObject);
		GameManager.Instance.CreateCharacter();
		isMenu = !isMenu;
	}
}
