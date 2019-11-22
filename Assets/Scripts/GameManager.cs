using System;
using System.Collections;


using UnityEngine;
using UnityEngine.SceneManagement;


using Photon.Pun;
using Photon.Realtime;



    public class GameManager : MonoBehaviourPunCallbacks
    {
        #region Public Fields
        public static GameManager Instance;
        [Tooltip("The prefab to use for representing the player")]
        public GameObject[] playerPrefab;

        [Header("Two Side Relive Point")]
        public ReliveZone RedRelivePoint;
        public ReliveZone BlueRelivePoint;
        [Header("Menu")]
        public InGameMenu GameMenu;

        #endregion

        #region Photon Callbacks
        /// <summary>
        /// Called when the local player left the room. We need to load the launcher scene.
        /// </summary>
        public override void OnLeftRoom()
        {
            SceneManager.LoadScene(0);
        }

        public override void OnPlayerEnteredRoom(Player other)
        {
            Debug.LogFormat("OnPlayerEnteredRoom() {0}", other.NickName); // not seen if you're the player connecting


            if (PhotonNetwork.IsMasterClient)
            {
                Debug.LogFormat("OnPlayerEnteredRoom IsMasterClient {0}", PhotonNetwork.IsMasterClient); // called before OnPlayerLeftRoom
                //LoadArena();
            }
        }


        public override void OnPlayerLeftRoom(Player other)
        {
            Debug.LogFormat("OnPlayerLeftRoom() {0}", other.NickName); // seen when other disconnects


            if (PhotonNetwork.IsMasterClient)
            {
                Debug.LogFormat("OnPlayerLeftRoom IsMasterClient {0}", PhotonNetwork.IsMasterClient); // called before OnPlayerLeftRoom
                //LoadArena();
            }
        }

        #endregion

        #region MonoBehaviour CallBacks
        void Start()
        {
            Instance = this;
            GameMenu = GetComponent<InGameMenu>();
            if (playerPrefab == null)
            {
                Debug.LogError("<Color=Red><a>Missing</a></Color> playerPrefab Reference. Please set it up in GameObject 'Game Manager'", this);
            }
            else
            {
                if (PlayerInfo.PI != null)
                {
                    Debug.LogFormat("We are Instantiating LocalPlayer from {0}", SceneManagerHelper.ActiveSceneName);
                    // we're in a room. spawn a character for the local player. it gets synced by using PhotonNetwork.Instantiate
                    //if(PlayerInfo.PI.mySelectedCharacter!=-1)
                    SoundManager.Instance.FadeOutBGM();
                    CreateCharacter();
                }
                // else
                // {
                //     Debug.LogFormat("Ignoring scene load for {0}", SceneManagerHelper.ActiveSceneName);
                // }
            }
        }
        #endregion



        #region Public Methods

        public void LeaveRoom()
        {

            PhotonNetwork.LeaveRoom();
        }


        #endregion

        #region Private Methods

        void LoadArena()
        {
            if (!PhotonNetwork.IsMasterClient)
            {
                Debug.LogError("PhotonNetwork : Trying to Load a level but we are not the master Client");
            }
            Debug.LogFormat("PhotonNetwork : Loading Level : {0}", PhotonNetwork.CurrentRoom.PlayerCount);
            //按人數加載場景(須改)
            PhotonNetwork.LoadLevel(Global.Level);// + PhotonNetwork.CurrentRoom.PlayerCount);
        }
        public void CreateCharacter(){
            GameObject player = PhotonNetwork.Instantiate("Character/" + this.playerPrefab[PlayerInfo.PI.mySelectedCharacter].name, new Vector3(0f, 5f, 0f), Quaternion.identity, 0);
            GameMenu.PlayerAm = player.GetComponent<ActorManager>();
        }
        public void DestroyCharacter(GameObject obj){
            PhotonNetwork.Destroy(obj);
        }

        #endregion
    }
