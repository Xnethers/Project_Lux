// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PlayerListEntry.cs" company="Exit Games GmbH">
//   Part of: Asteroid Demo,
// </copyright>
// <summary>
//  Player List Entry
// </summary>
// <author>developer@exitgames.com</author>
// --------------------------------------------------------------------------------------------------------------------

using UnityEngine;
using UnityEngine.UI;

using ExitGames.Client.Photon;
using Photon.Realtime;
using Photon.Pun.UtilityScripts;
using Photon.Pun.Demo.Asteroids;
namespace Photon.Pun.Demo.Asteroids
{
    public class PlayerListEntry : MonoBehaviourPunCallbacks
    {
        [Header("UI References")]
        public Text PlayerNameText;

        public Image PlayerTeamImage;
        public Button PlayerReadyButton;
        public Image PlayerReadyImage;

        private int ownerId;
        //private bool isPlayerReady;
        private int startCharacter;
        
        #region UNITY

        public void OnEnable()
        {
            PlayerNumbering.OnPlayerNumberingChanged += OnPlayerNumberingChanged;
        }

       


        public void Start()
        {
            if (PhotonNetwork.LocalPlayer.ActorNumber != ownerId)
            {
                PlayerReadyButton.gameObject.SetActive(false);
            }
            else
            {
                if(PlayerInfo.PI.mySelectedCamp==0)
                    startCharacter=1;//0
                else
                    startCharacter=4;//3
                PhotonNetwork.LocalPlayer.SetCharacter(startCharacter);
                PhotonNetwork.LocalPlayer.SetReady(false);
                
                foreach (Player player in PhotonNetwork.PlayerList)
                    FindObjectOfType<ChooseCharacter>().CheckSameCharacter(player);
                
                PhotonNetwork.LocalPlayer.SetScore(0);
                PlayerReadyButton.onClick.AddListener(() =>
                {
                    SoundManager.Instance.PlaySceneEffect(SoundManager.Instance.ClikUI);
                    if(!PhotonNetwork.LocalPlayer.GetReady()){
                        //FindObjectOfType<ChooseCharacter>().photonView.RPC("add",RpcTarget.MasterClient,PhotonNetwork.LocalPlayer);
                        PhotonNetwork.LocalPlayer.SetReady(true);
                        //Call funtion at same time.
                        Invoke("Invoke_CheckSameCharacter",Random.Range(0.2f,0.4f));
                    }
                    else{
                        //FindObjectOfType<ChooseCharacter>().photonView.RPC("remove",RpcTarget.MasterClient,PhotonNetwork.LocalPlayer);
                        PhotonNetwork.LocalPlayer.SetReady(false);
                    }
                        
                    if (PhotonNetwork.IsMasterClient)
                    {
                        FindObjectOfType<LobbyMainPanel>().LocalPlayerPropertiesUpdated();
                    }
                });
            }
        }
        public void Invoke_CheckSameCharacter(){
            FindObjectOfType<ChooseCharacter>().photonView.RPC("CheckSameCharacter",RpcTarget.MasterClient,PhotonNetwork.LocalPlayer);
        }
        public void OnDisable()
        {
            PlayerNumbering.OnPlayerNumberingChanged -= OnPlayerNumberingChanged;
        }

        #endregion

        public void Initialize(int playerId, string playerName)
        {
            ownerId = playerId;
            PlayerNameText.text = playerName;
        }


        private void OnPlayerNumberingChanged()
        {
            foreach (Player p in PhotonNetwork.PlayerList)
            {
                if (p.ActorNumber == ownerId)
                {
                    //PlayerColorImage.color = AsteroidsGame.GetColor(p.GetPlayerNumber());
                }
            }
        }


        public void SetPlayerReady(bool playerReady)
        {
            PlayerReadyButton.GetComponentInChildren<Text>().text = playerReady ? "Ready!" : "Ready?";
            
            PlayerReadyImage.enabled = playerReady;
            PlayerTeamImage.enabled = playerReady;
        }
        public static void OnClickReady(){

        }
    }
    
}
namespace Photon.Pun.UtilityScripts{
    public static class ChooseCharacterExtensions{
        public static bool GetReady(this Player player)
        {
            object ready;
            if (player.CustomProperties.TryGetValue(PlayerInfo.PLAYER_READY, out ready))
            {
                return (bool)ready;
            }

            return false;
        }
        public static void SetReady(this Player player,bool isready)
        {
            Hashtable ready = new Hashtable();  // using PUN's implementation of Hashtable
            ready[PlayerInfo.PLAYER_READY] = isready;

            player.SetCustomProperties(ready);
        }
        public static int GetCharacter(this Player player)
        {
            object character;
            if (player.CustomProperties.TryGetValue(PlayerInfo.PLAYER_Character, out character))
            {
                return (int)character;
            }

            return 0;
        }
        public static void SetCharacter(this Player player,int character)
        {
            Hashtable chooseCharacter = new Hashtable();  // using PUN's implementation of Hashtable
            chooseCharacter[PlayerInfo.PLAYER_Character] = character;

            player.SetCustomProperties(chooseCharacter);
        }
    }
}