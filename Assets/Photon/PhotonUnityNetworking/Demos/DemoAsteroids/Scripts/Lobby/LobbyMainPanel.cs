using ExitGames.Client.Photon;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun.UtilityScripts;

namespace Photon.Pun.Demo.Asteroids
{
    public class LobbyMainPanel : MonoBehaviourPunCallbacks
    {

        [Header("Login Panel")]
        public GameObject LoginPanel;

        public InputField PlayerNameInput;

        [Header("Selection Panel")]
        public GameObject SelectionPanel;

        [Header("Create Room Panel")]
        public GameObject CreateRoomPanel;

        public InputField RoomNameInputField;
        public InputField MaxPlayersInputField;

        [Header("Join Random Room Panel")]
        public GameObject JoinRandomRoomPanel;

        [Header("Room List Panel")]
        public GameObject RoomListPanel;

        public GameObject RoomListContent;
        public GameObject RoomListEntryPrefab;

        [Header("Inside Room Panel")]
        public GameObject InsideRoomPanel;

        public Button StartGameButton;
        public GameObject myAllCharacters;
        public int charactersCount;
        public GameObject[] myAllCamps;

        [Header("NoviceTeaching Panel")]
        public GameObject NoviceTeachingPanel;
        public Button TeachButton;
        public Button TrainingButton;

        [Header("Players List Panel ")]
        public GameObject PlayersListPanel;
        public GameObject PlayerListEntryPrefab;

        [Header("Teams Panel ")]

        public GameObject MyTeamPanel;
        public GameObject OtherTeamPanel;

        private Dictionary<string, RoomInfo> cachedRoomList;
        private Dictionary<string, GameObject> roomListEntries;
        private Dictionary<int, GameObject> playerListEntries;

        private GameObject[] myCharacter;
        private GameObject[] myCharacterUI;
        private GameObject[] lockCharacterUI;



        #region UNITY

        public void Awake()
        {
            PhotonNetwork.AutomaticallySyncScene = true;

            cachedRoomList = new Dictionary<string, RoomInfo>();
            roomListEntries = new Dictionary<string, GameObject>();

            if (PlayerPrefs.HasKey("PlayerName"))
            { PlayerNameInput.text = PlayerPrefs.GetString("PlayerName"); }
            else
            { PlayerNameInput.text = "Player " + Random.Range(1000, 10000); }

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        public void Start()
        {
            //角色陣列物件
            myCharacter = new GameObject[charactersCount];
            myCharacterUI = new GameObject[charactersCount];
            lockCharacterUI = new GameObject[charactersCount];
            for (int i = 0; i < charactersCount; i++)
            {
                myCharacter[i] = myAllCharacters.transform.GetChild(i).gameObject;
                if (i < 3)
                    myCharacterUI[i] = myAllCamps[0].transform.GetChild(i).gameObject;
                else
                    myCharacterUI[i] = myAllCamps[1].transform.GetChild(i - 3).gameObject;
                lockCharacterUI[i] = myCharacterUI[i].transform.GetChild(0).gameObject;
            }
            //myCharacter = SetRoomObj(myAllCharacters);
            SoundManager.Instance.PlaySceneEffect(SoundManager.Instance.BGM);
        }
        private GameObject[] SetRoomObj(GameObject tempDadObj)
        {
            GameObject[] go = new GameObject[6];
            for (int i = 0; i < go.Length; i++)
                go[i] = tempDadObj.transform.GetChild(i).gameObject;
            return go;
        }

        #endregion

        #region PUN CALLBACKS

        public override void OnConnectedToMaster()
        {
            this.SetActivePanel(SelectionPanel.name);
        }

        public override void OnRoomListUpdate(List<RoomInfo> roomList)
        {
            ClearRoomListView();

            UpdateCachedRoomList(roomList);
            UpdateRoomListView();
        }

        public override void OnLeftLobby()
        {
            cachedRoomList.Clear();
            ClearRoomListView();
        }

        public override void OnCreateRoomFailed(short returnCode, string message)
        {
            SetActivePanel(SelectionPanel.name);
        }

        public override void OnJoinRoomFailed(short returnCode, string message)
        {
            SetActivePanel(SelectionPanel.name);
        }

        public override void OnJoinRandomFailed(short returnCode, string message)
        {
            string roomName = "Room " + Random.Range(1000, 10000);

            RoomOptions options = new RoomOptions { MaxPlayers = 8 };
            PhotonNetwork.CreateRoom(roomName, options, null);
        }

        public override void OnJoinedRoom()
        {
            if (Global.Level != 0)
            { SetActivePanel(InsideRoomPanel.name); }
            else
            { SetActivePanel(NoviceTeachingPanel.name); }

            if (playerListEntries == null)
            {
                playerListEntries = new Dictionary<int, GameObject>();
            }

            CalculateTeamToStartOn();

            foreach (Player p in PhotonNetwork.PlayerList)
            {

                GameObject entry = Instantiate(PlayerListEntryPrefab);

                if (p.GetTeam() == PhotonNetwork.LocalPlayer.GetTeam())
                { entry.transform.SetParent(MyTeamPanel.transform); }
                else if (p.GetTeam() != PhotonNetwork.LocalPlayer.GetTeam())
                { entry.transform.SetParent(OtherTeamPanel.transform); }

                entry.transform.localScale = Vector3.one;
                entry.GetComponent<RectTransform>().localPosition = Vector3.zero;
                entry.GetComponent<PlayerListEntry>().Initialize(p.ActorNumber, p.NickName);

                entry.GetComponent<PlayerListEntry>().SetPlayerReady(p.GetReady());

                playerListEntries.Add(p.ActorNumber, entry);
            }
            StartGameButton.gameObject.SetActive(CheckPlayersReady());

            Hashtable props = new Hashtable
            {
                {AsteroidsGame.PLAYER_LOADED_LEVEL, false}
            };
            PhotonNetwork.LocalPlayer.SetCustomProperties(props);

        }

        public override void OnLeftRoom()
        {
            SetActivePanel(SelectionPanel.name);

            foreach (GameObject entry in playerListEntries.Values)
            {
                Destroy(entry.gameObject);
            }

            PhotonNetwork.LocalPlayer.SetTeam(PunTeams.Team.none);

            playerListEntries.Clear();
            playerListEntries = null;
        }

        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            GameObject entry = Instantiate(PlayerListEntryPrefab);

            if (newPlayer.GetTeam() == PhotonNetwork.LocalPlayer.GetTeam())
            { entry.transform.SetParent(MyTeamPanel.transform); }
            else if (newPlayer.GetTeam() != PhotonNetwork.LocalPlayer.GetTeam())
            { entry.transform.SetParent(OtherTeamPanel.transform); }

            entry.transform.localScale = Vector3.one;
            entry.GetComponent<RectTransform>().localPosition = Vector3.zero;
            entry.GetComponent<PlayerListEntry>().Initialize(newPlayer.ActorNumber, newPlayer.NickName);

            playerListEntries.Add(newPlayer.ActorNumber, entry);

            StartGameButton.gameObject.SetActive(CheckPlayersReady());
        }

        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            Destroy(playerListEntries[otherPlayer.ActorNumber].gameObject);
            playerListEntries.Remove(otherPlayer.ActorNumber);

            StartGameButton.gameObject.SetActive(CheckPlayersReady());
        }

        public override void OnMasterClientSwitched(Player newMasterClient)
        {
            if (PhotonNetwork.LocalPlayer.ActorNumber == newMasterClient.ActorNumber)
            {
                StartGameButton.gameObject.SetActive(CheckPlayersReady());
            }
        }

        public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
        {
            if (playerListEntries == null)
            {
                playerListEntries = new Dictionary<int, GameObject>();
            }

            GameObject entry;

            if (playerListEntries.TryGetValue(targetPlayer.ActorNumber, out entry))
            {
                object isPlayerReady;
                if (changedProps.TryGetValue(PlayerInfo.PLAYER_READY, out isPlayerReady))
                {
                    entry.GetComponent<PlayerListEntry>().SetPlayerReady((bool)isPlayerReady);
                }

                if (targetPlayer.GetTeam() == PhotonNetwork.LocalPlayer.GetTeam())
                { entry.transform.SetParent(MyTeamPanel.transform); }
                else if (targetPlayer.GetTeam() != PhotonNetwork.LocalPlayer.GetTeam())
                { entry.transform.SetParent(OtherTeamPanel.transform); }

                entry.transform.localScale = Vector3.one;
            }
            UpdateCharacterObj();
            StartGameButton.gameObject.SetActive(CheckPlayersReady());
        }
        public override void OnRoomPropertiesUpdate(Hashtable changedProps)
        {
            if (PhotonNetwork.LocalPlayer.GetTeam() == PunTeams.Team.red)
                ObjVisiable((bool[])changedProps["Red"]);//顯示紅隊角色選擇資訊
            if (PhotonNetwork.LocalPlayer.GetTeam() == PunTeams.Team.blue)
                ObjVisiable((bool[])changedProps["Blue"]);//顯示藍隊角色選擇資訊
        }

        #endregion

        #region UI CALLBACKS

        public void OnBackButtonClicked()
        {
            SoundManager.Instance.PlaySceneEffect(SoundManager.Instance.ClikUI);
            if (PhotonNetwork.InLobby)
            {
                PhotonNetwork.LeaveLobby();
            }

            SetActivePanel(SelectionPanel.name);
        }
        public void OnCreateRoomPanelButtonClicked()
        {
            SoundManager.Instance.PlaySceneEffect(SoundManager.Instance.ClikUI);
            SetActivePanel(CreateRoomPanel.name);
        }
        public void OnCreateRoomButtonClicked()
        {
            SoundManager.Instance.PlaySceneEffect(SoundManager.Instance.ClikUI);
            string roomName = RoomNameInputField.text;
            roomName = (roomName.Equals(string.Empty)) ? "Room " + Random.Range(1000, 10000) : roomName;

            byte maxPlayers;
            byte.TryParse(MaxPlayersInputField.text, out maxPlayers);
            maxPlayers = (byte)Mathf.Clamp(maxPlayers, 2, 8);

            RoomOptions options = new RoomOptions { MaxPlayers = maxPlayers };

            PhotonNetwork.CreateRoom(roomName, options, null);
        }

        public void OnJoinRandomRoomButtonClicked()
        {
            SoundManager.Instance.PlaySceneEffect(SoundManager.Instance.ClikUI);
            SetActivePanel(JoinRandomRoomPanel.name);
            Global.Level = Random.Range(1, 3);
            PhotonNetwork.JoinRandomRoom();
        }

        public void OnLeaveGameButtonClicked()
        {
            SoundManager.Instance.PlaySceneEffect(SoundManager.Instance.ClikUI);
            FindObjectOfType<ChooseCharacter>().photonView.RPC("RPC_ReadyFalse", RpcTarget.MasterClient, PhotonNetwork.LocalPlayer);
            FindObjectOfType<ChooseCharacter>().ResetLockCharacter();
            PhotonNetwork.LeaveRoom();
        }

        public void OnLoginButtonClicked()
        {
            SoundManager.Instance.PlaySceneEffect(SoundManager.Instance.ClikUI);
            string playerName = PlayerNameInput.text;

            if (!playerName.Equals(""))
            {
                PhotonNetwork.LocalPlayer.NickName = playerName;
                PhotonNetwork.ConnectUsingSettings();
            }
            else
            {
                Debug.LogError("Player Name is invalid.");
            }
        }

        public void OnRoomListButtonClicked()
        {
            SoundManager.Instance.PlaySceneEffect(SoundManager.Instance.ClikUI);
            if (!PhotonNetwork.InLobby)
            {
                PhotonNetwork.JoinLobby();
            }

            SetActivePanel(RoomListPanel.name);
        }

        public void OnStartGameButtonClicked()
        {
            SoundManager.Instance.PlaySceneEffect(SoundManager.Instance.ClikUI);
            PhotonNetwork.CurrentRoom.IsOpen = false;
            PhotonNetwork.CurrentRoom.IsVisible = false;

            PhotonNetwork.LoadLevel("Room for " + Global.Level);
        }

        public void OnTeachButtonClicked()//新手教學
        {
            if (PhotonNetwork.IsMasterClient)
            { PhotonNetwork.LoadLevel("NoviceTeaching"); }
        }
        public void OnTrainingButtonClicked()//訓練模式
        {
            if (PhotonNetwork.IsMasterClient)
            { PhotonNetwork.LoadLevel("Training"); }
        }

        public void OnClickCamp(int whichCamp)
        {//選擇隊伍
            SoundManager.Instance.PlaySceneEffect(SoundManager.Instance.ClikUI);
            //顯示所選隊伍(角色)
            ObjVisiable(myAllCamps, whichCamp);
            PlayerInfo.PI.mySelectedCamp = whichCamp;
        }
        public void OnClickCharacter(int whichCharacter)
        {
            SoundManager.Instance.PlaySceneEffect(SoundManager.Instance.ClikUI);
            if ((bool)PhotonNetwork.LocalPlayer.CustomProperties[PlayerInfo.PLAYER_READY])
                return;
            if (PhotonNetwork.LocalPlayer.GetTeam() == PunTeams.Team.red)
            {
                if (FindObjectOfType<ChooseCharacter>().isRedChooseCharacter[whichCharacter])
                    return;
            }
            if (PhotonNetwork.LocalPlayer.GetTeam() == PunTeams.Team.blue)
            {
                if (FindObjectOfType<ChooseCharacter>().isBlueChooseCharacter[whichCharacter])
                    return;
            }
            PhotonNetwork.LocalPlayer.SetCharacter(whichCharacter);
        }

        public void OnClickRoom(int i)
        {
            Global.Level = i;
        }

        public void OnNoviceTeachingClicked()
        {
            //SoundManager.Instance.PlaySceneEffect(SoundManager.Instance.ClikUI);
            if (playerListEntries == null)
            {
                playerListEntries = new Dictionary<int, GameObject>();
            }
            RoomOptions options = new RoomOptions { MaxPlayers = 1 };
            PhotonNetwork.CreateRoom(null, options, null);

            PlayerInfo.PI.mySelectedCharacter = 1;

            PhotonNetwork.LocalPlayer.SetCharacter(0);
            PhotonNetwork.LocalPlayer.SetReady(true);
            Hashtable props = new Hashtable
            {
                {AsteroidsGame.PLAYER_LOADED_LEVEL, false}
            };
            PhotonNetwork.LocalPlayer.SetCustomProperties(props);
            PhotonNetwork.LocalPlayer.SetTeam(PunTeams.Team.red);
            Global.Level = 0;
        }

        #endregion
        #region CHOOSE CHARACTERS
        private void UpdateCharacterObj()
        {
            int tempVisible = PhotonNetwork.LocalPlayer.GetCharacter();//玩家所選角色
            ObjSclae(myCharacterUI, tempVisible);//放大所選角色
            ObjVisiable(myCharacter, tempVisible);//顯示3D角色模型
            PlayerInfo.PI.mySelectedCharacter = tempVisible;
        }

        private void ObjSclae(GameObject[] tempObjs, int visible)
        {
            foreach (GameObject obj in tempObjs)
            {
                obj.GetComponent<RectTransform>().localScale = new Vector3(0.5f, 0.5f, 0.5f);
            }
            tempObjs[visible].GetComponent<RectTransform>().localScale = new Vector3(0.6f, 0.6f, 0.6f);
        }
        private void ObjVisiable(GameObject[] tempObjs, int visible)
        {
            foreach (GameObject obj in tempObjs)
            {
                obj.SetActive(false);
            }
            tempObjs[visible].SetActive(true);
        }
        private void ObjVisiable(bool[] tempBools)
        {
            if (tempBools == null)
                return;
            for (int i = 0; i < lockCharacterUI.Length; i++)
            {
                if (tempBools[i])
                    lockCharacterUI[i].SetActive(true);
                else if (!tempBools[i])
                    lockCharacterUI[i].SetActive(false);
            }
        }
        #endregion
        private bool CheckPlayersReady()
        {
            if (!PhotonNetwork.IsMasterClient)
            {
                return false;
            }

            foreach (Player p in PhotonNetwork.PlayerList)
            {
                object isPlayerReady;
                if (p.CustomProperties.TryGetValue(PlayerInfo.PLAYER_READY, out isPlayerReady))
                {
                    if (!(bool)isPlayerReady)
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }

            return true;
        }

        private void ClearRoomListView()
        {
            foreach (GameObject entry in roomListEntries.Values)
            {
                Destroy(entry.gameObject);
            }

            roomListEntries.Clear();
        }

        public void LocalPlayerPropertiesUpdated()
        {
            StartGameButton.gameObject.SetActive(CheckPlayersReady());
        }

        private void SetActivePanel(string activePanel)
        {
            LoginPanel.SetActive(activePanel.Equals(LoginPanel.name));
            SelectionPanel.SetActive(activePanel.Equals(SelectionPanel.name));
            CreateRoomPanel.SetActive(activePanel.Equals(CreateRoomPanel.name));
            JoinRandomRoomPanel.SetActive(activePanel.Equals(JoinRandomRoomPanel.name));
            RoomListPanel.SetActive(activePanel.Equals(RoomListPanel.name));    // UI should call OnRoomListButtonClicked() to activate this
            InsideRoomPanel.SetActive(activePanel.Equals(InsideRoomPanel.name));
            NoviceTeachingPanel.SetActive(activePanel.Equals(NoviceTeachingPanel.name));
        }

        private void UpdateCachedRoomList(List<RoomInfo> roomList)
        {
            foreach (RoomInfo info in roomList)
            {
                // Remove room from cached room list if it got closed, became invisible or was marked as removed
                if (!info.IsOpen || !info.IsVisible || info.RemovedFromList)
                {
                    if (cachedRoomList.ContainsKey(info.Name))
                    {
                        cachedRoomList.Remove(info.Name);
                    }

                    continue;
                }

                // Update cached room info
                if (cachedRoomList.ContainsKey(info.Name))
                {
                    cachedRoomList[info.Name] = info;
                }
                // Add new room info to cache
                else
                {
                    cachedRoomList.Add(info.Name, info);
                }
            }
        }

        private void UpdateRoomListView()
        {
            foreach (RoomInfo info in cachedRoomList.Values)
            {
                GameObject entry = Instantiate(RoomListEntryPrefab);
                entry.transform.SetParent(RoomListContent.transform);
                entry.transform.localScale = Vector3.one;
                entry.GetComponent<RectTransform>().localPosition = Vector3.zero;
                entry.GetComponent<RoomListEntry>().Initialize(info.Name, (byte)info.PlayerCount, info.MaxPlayers);

                roomListEntries.Add(info.Name, entry);
            }
        }

        public void CalculateTeamToStartOn()
        {
            if (PhotonNetwork.IsConnectedAndReady)
            {
                int redAmount = 0;
                int blueAmount = 0;

                foreach (Player player in PunTeams.PlayersPerTeam[PunTeams.Team.red])
                { redAmount++; }

                foreach (Player player in PunTeams.PlayersPerTeam[PunTeams.Team.blue])
                { blueAmount++; }

                if (redAmount == 0 && blueAmount == 0)
                { PhotonNetwork.LocalPlayer.SetTeam(PunTeams.Team.red); }

                if (redAmount > blueAmount)
                { PhotonNetwork.LocalPlayer.SetTeam(PunTeams.Team.blue); }
                else
                { PhotonNetwork.LocalPlayer.SetTeam(PunTeams.Team.red); }
            }
        }
    }
}

public class Global
{
    public static int Level;
}