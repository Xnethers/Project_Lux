using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Realtime;
public class ResultListEntry : MonoBehaviour {
	public Player player;
	public ActorManager am;

	[Header("UI References")]
	public Text PlayerNameText;
	public Image PlayerCharacterImage;
	public Text PlayerAttackText;
	public Text PlayerHurtText;
	public Text PlayerKillText;
	public Text PlayerDeadText;
	public void Initialize(Player p)
	{
		// ownerId = p.ActorNumber;
		player = p;
		PlayerNameText.text = p.NickName;
		ActorManager[] PMs=FindObjectsOfType<ActorManager>();
        List<ActorManager> playerAm = new List<ActorManager>();
        foreach(ActorManager tempAm in PMs){
            if(!tempAm.ac.pi.isAI)
                playerAm.Add(tempAm);
        }
        foreach(ActorManager pm in playerAm){
			if(p.ActorNumber==pm.photonView.Owner.ActorNumber){
				am=pm;
			}
		}
	}
	public void Update(){
		if(am != null){
			PlayerAttackText.text = Mathf.Round(am.sm.AllAttack).ToString();
			PlayerHurtText.text = Mathf.Round(am.sm.AllHurt).ToString();
			PlayerKillText.text = am.sm.AllKill.ToString();
			PlayerDeadText.text = am.sm.AllDead.ToString();
		}
		else{
			if(Global.Level<=0){
				am = GameManager.Instance.GameMenu.PlayerAm;
			}
		}
		// else{
		// 	if(!GameManager.Instance.isResult)
		// 		Destroy(gameObject);
		// }
	}
}
