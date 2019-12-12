﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
public class FightSettlement : InGameMenu {
	private CanvasGroup PlayersListCanvasGroup;
	public override void Start(){
		base.Start();
		PlayersListCanvasGroup = PlayersListPanel.GetComponent<CanvasGroup>();
	}
	public override void Update()
	{
		base.Update();
		if(isMenu){
			// if (!GameManager.Instance.isResult)
			// 	SettlementDisable();
			// else
			// 	PlayersListCanvasGroup.DOFade(1,duration/2);
			return;
		}
			
		if (!GameManager.Instance.isResult)
        {
			if(Input.GetKeyDown(KeyCode.Tab)){
				GameManager.Instance.Settlement();
			}
            if (Input.GetKey(KeyCode.Tab))
            {
                isTab = true;	
            }
			else{
				isTab = false;
			}
			if (isTab)
			{
				SettlementEnable();
			}
			else{
				SettlementDisable();
			}
        }
	}
	public void SettlementEnable(){
		// PlayersListPanel.gameObject.SetActive(true);
		PlayersListCanvasGroup.DOFade(1,duration/2);
		SettlementPanelEnable();
	}
	public void SettlementDisable(){
		PlayersListCanvasGroup.DOFade(0,duration/2);
		if(PlayersListCanvasGroup.alpha < 0.1f){
			// PlayersListPanel.gameObject.SetActive(false);
			SettlementPanelDisable();
			GameManager.Instance.DestroySettlement();
		}
	}
}
