using System;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using Photon.Pun.UtilityScripts;
using UnityEngine.UI;
namespace Photon.Pun.Demo.Asteroids{
	public class ChooseCharacter : MonoBehaviourPunCallbacks{
		public enum Character : byte { Osun =0, Enid, Adela, Shota, Megane, Otokonoko };
		public static Dictionary<Character, List<Player>> RedPlayersCharacter;
		public static Dictionary<Character, List<Player>> BluePlayersCharacter;
		public bool[] isRedChooseCharacter = new bool[6];
		public bool[] isBlueChooseCharacter= new bool[6];

		void Start()
		{
			RedPlayersCharacter = new Dictionary<Character, List<Player>> ();
			BluePlayersCharacter = new Dictionary<Character, List<Player>> ();
			Array enumVals = Enum.GetValues(typeof(Character));
			foreach (var enumVal in enumVals)
			{
				RedPlayersCharacter[(Character)enumVal] = new List<Player>();
				BluePlayersCharacter[(Character)enumVal] = new List<Player>();
			}
		}
		public override void OnDisable()
		{
			RedPlayersCharacter = new Dictionary<Character, List<Player>>();
			BluePlayersCharacter = new Dictionary<Character, List<Player>>();
		}
		void Update()
		{
			//MasterClient set roomCustomProperties, and the others get.
			Room room = PhotonNetwork.CurrentRoom;
			if (room == null) {
				return;
			}
			Hashtable cp = room.CustomProperties;
			if(PhotonNetwork.IsMasterClient){
				cp ["Blue"] = isBlueChooseCharacter;
				cp ["Red"] = isRedChooseCharacter;
				room.SetCustomProperties (cp);
				SetLockCharacter(RedPlayersCharacter,isRedChooseCharacter);
				SetLockCharacter(BluePlayersCharacter,isBlueChooseCharacter);
			}
			else{
				isBlueChooseCharacter = (bool[])cp ["Blue"] ;
				isRedChooseCharacter = (bool[])cp ["Red"];
			}
			
		}
		public void ResetLockCharacter(){
			for(int i=0;i<isRedChooseCharacter.Length;i++)
				isRedChooseCharacter[i] = false;
			for(int i=0;i<isBlueChooseCharacter.Length;i++)
				isBlueChooseCharacter[i] = false;
		}
		//Set isChooseCharacter based on player's character and ready.
		private void SetLockCharacter(Dictionary<Character, List<Player>> PlayersCharacter, bool[] isChooseCharacter){
			Dictionary<Character, List<Player>> tempPlayersCharacter = PlayersCharacter;
			foreach (KeyValuePair<Character, List<Player>> kvp in tempPlayersCharacter)
			{
				isChooseCharacter[(int)kvp.Key] = false;
				
				foreach (Player player in tempPlayersCharacter[kvp.Key]){
					//Debug.Log("player: "+ player.NickName+" character: "+kvp.Key);
					isChooseCharacter[player.GetCharacter()] = (bool)player.GetReady();
				}
			}
		}
		private void UpdateCharacter(){
			//Update dictionary : Red,BluePlayersCharacter.
			Array enumVals = Enum.GetValues(typeof(Character));
			foreach (var enumVal in enumVals)
			{
				RedPlayersCharacter[(Character)enumVal].Clear();
				BluePlayersCharacter[(Character)enumVal].Clear();
			}
			for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
			{
				Player player = PhotonNetwork.PlayerList[i];
				Character playerCharacter = (Character)player.GetCharacter();
				if(player.GetTeam() == PunTeams.Team.red){
					RedPlayersCharacter[playerCharacter].Add(player);
				}
				else if(player.GetTeam() == PunTeams.Team.blue){
					BluePlayersCharacter[playerCharacter].Add(player);
				}
			}
			
		}
		/*
		[PunRPC]
		public void add(Player player){
			Character playerCharacter = (Character)player.GetCharacter();
			if(player.GetTeam() == PunTeams.Team.red){
				RedPlayersCharacter[playerCharacter].Add(player);
			}
			else if(player.GetTeam() == PunTeams.Team.blue){
				BluePlayersCharacter[playerCharacter].Add(player);
			}
		}
		[PunRPC]
		public void remove(Player player){
			Character playerCharacter = (Character)player.GetCharacter();
			if(player.GetTeam() == PunTeams.Team.red){
				RedPlayersCharacter[playerCharacter].Remove(player);
			}
			else if(player.GetTeam() == PunTeams.Team.blue){
				BluePlayersCharacter[playerCharacter].Remove(player);
			}
		} */
		public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
        {
            this.UpdateCharacter();
        }
		//When player LeaveRoom, close ready.
		[PunRPC]
		public void RPC_ReadyFalse(Player player){
			player.SetReady(false);
			//remove(player);
		}
		//When player click readyButton.
		[PunRPC]
		public void CheckSameCharacter(Player onClickPlayer){
			Player randomPlayer=(Player)GetRandomPlayer(onClickPlayer);
			foreach (Player player in PhotonNetwork.PlayerList){
				if(player != onClickPlayer &&player.GetTeam() == onClickPlayer.GetTeam()){
					if(player.GetCharacter() == onClickPlayer.GetCharacter()){
						if(player==randomPlayer && player.GetReady())
						{}
						else
						{
							player.SetReady(false);
							//photonView.RPC("remove",RpcTarget.MasterClient,player);
							player.SetCharacter(RandomInt(onClickPlayer.GetCharacter()));
							Debug.Log("player: "+player.NickName + " character: "+player.GetCharacter());
							
						}
					}
				}
			}
		}
		private Player GetRandomPlayer(Player player){
			int character=player.GetCharacter();
			List<Player> value;
			Dictionary<Character, List<Player>> PlayersCharacter = new Dictionary<Character, List<Player>>();
			if(player.GetTeam()==PunTeams.Team.red)
				PlayersCharacter=RedPlayersCharacter;
			if(player.GetTeam()==PunTeams.Team.blue)
				PlayersCharacter=BluePlayersCharacter;
			if(PlayersCharacter.TryGetValue((Character)character,out value)){
				Player p = null;
				if(value.Count > 0)
					p = value[0];
				/* 
				if(value.Count == 0){}
				else if(value.Count==1)
					p = value[0];
				else
					p = value[UnityEngine.Random.Range(0,value.Count)];*/
				return p;
			}
			else
				return null;
		}
		
		private int RandomInt(int character){
			int tempInt = 0;
			if(PlayerInfo.PI.mySelectedCamp==0)
				tempInt = UnityEngine.Random.Range(0,3);
			else if(PlayerInfo.PI.mySelectedCamp==1)
				tempInt = UnityEngine.Random.Range(3,6);
			if(tempInt==character)
				return RandomInt(character);
			else
				return tempInt;
		}
	}
}