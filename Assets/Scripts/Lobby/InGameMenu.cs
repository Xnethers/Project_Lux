using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameMenu : MonoBehaviour {
	public ActorManager PlayerAm;
	public GameObject MenuPanel;
	public GameObject ResultPanel;
	public bool isMenu;
	void Start(){
		MenuPanel.SetActive(false);
		ResultPanel.SetActive(false);
	}
	public virtual void Update()
	{
		if(!GameManager.Instance.isResult){
			if(Input.GetKeyDown(KeyCode.Escape)){//Input.GetKeyDown(KeyCode.Tab) || 
				isMenu = !isMenu;
			}	
		}
		if(isMenu){
			Cursor.lockState = CursorLockMode.None;
            Cursor.visible=true;
			if(!GameManager.Instance.isResult)
				MenuPanel.SetActive(true);
			else
				ResultPanel.SetActive(true);
			if(PlayerAm!=null){
				// PlayerAm.ac.pi.enabled=false;
				// PlayerAm.ac.camcon.enabled = false;
				PlayerAm.ac.pi.inputActive = false;
				PlayerAm.ac.pi.InputInitialize();
			}
			
		}
		else{
			Cursor.lockState = CursorLockMode.Locked;
			Cursor.visible=false;
			MenuPanel.SetActive(false);
			ResultPanel.SetActive(false);
			if(PlayerAm!=null){
				PlayerAm.ac.pi.inputActive = true;
				// PlayerAm.ac.pi.enabled=true;
				// PlayerAm.ac.camcon.enabled = true;
			}
		}
			
	}
}
