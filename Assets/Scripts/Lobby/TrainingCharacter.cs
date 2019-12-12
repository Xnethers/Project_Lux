using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class TrainingCharacter : InGameMenu {
	public GameObject TrainingPanel;
	public GameObject PlayerUI;	
	public override void Start(){
		base.Start();
		TrainingPanel.SetActive(false);
	}
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
		if(isMenu)
			return;
		if(PlayerUI == null)
			PlayerUI = PlayerAm.GetComponent<PlayerUI>().ScreenCanvas;
		if (!GameManager.Instance.isResult)
        {
            if (Input.GetKeyDown(KeyCode.Tab))
            {//Input.GetKeyDown(KeyCode.Tab) || 
                isTab = !isTab;
            }
			if (isTab)
			{
				PlayerDisable();
				PlayerUI.gameObject.SetActive(false);
				TrainingPanel.SetActive(true);
			}
			else{
				PlayerEnable();
				PlayerUI.gameObject.SetActive(true);
				TrainingPanel.SetActive(false);
			}
        }			
	}
	public void OnClickCharacter(int whichCharacter){
		PlayerInfo.PI.mySelectedCharacter = whichCharacter;
	}
	public void OnClickChoose(){
		GameManager.Instance.DestroyCharacter(PlayerAm.gameObject);
		GameManager.Instance.CreateCharacter();
		isTab = !isTab;
	}
}
