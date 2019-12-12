using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Realtime;
public class ResultListEntry : MonoBehaviour {
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
			PlayerAttackText.text = am.sm.AllAttack.ToString();
			PlayerHurtText.text = am.sm.AllHurt.ToString();
			PlayerKillText.text = am.sm.AllKill.ToString();
			PlayerDeadText.text = am.sm.AllDead.ToString();
		}
		else{
			if(!GameManager.Instance.isResult)
				Destroy(gameObject);
		}
	}
}
