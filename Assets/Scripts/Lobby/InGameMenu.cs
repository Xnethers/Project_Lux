using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameMenu : MonoBehaviour {
	public ActorManager PlayerAm;
	public GameObject MenuPanel;
	public bool isMenu;
	void Start(){
		MenuPanel.SetActive(false);
	}
	public virtual void Update()
	{
		if(Input.GetKeyDown(KeyCode.Escape)){//Input.GetKeyDown(KeyCode.Tab) || 
			isMenu = !isMenu;
		}	
		if(isMenu){
			Cursor.lockState = CursorLockMode.None;
            Cursor.visible=true;
			MenuPanel.SetActive(true);
			if(PlayerAm!=null){
				PlayerAm.ac.pi.enabled=false;
				PlayerAm.ac.pi.InputInitialize();
			}
			
		}
		else{
			Cursor.lockState = CursorLockMode.Locked;
			Cursor.visible=false;
			MenuPanel.SetActive(false);
			if(PlayerAm!=null){
				PlayerAm.ac.pi.enabled=true;
			}
		}
			
	}
}
