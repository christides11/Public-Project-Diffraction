using Steamworks;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace TightStuff.Menu
{
    public class Lobby : MonoBehaviour
    {
        public CSteamID lobbyID;

        public Transform playerQueue;
        public Transform playersToMatch;

        public List<LobbyManager.PlayerData> profiles;
        public List<ProfileDisplay> displays;

        private Callback<LobbyDataUpdate_t> lobbyDataUpdateCallback;
        public UnityEvent OnJoinLobby;

        public StageBringer stageBringer;

        // Start is called before the first frame update
        void Start()
        {
            lobbyDataUpdateCallback = Callback<LobbyDataUpdate_t>.Create(OnLobbyDataUpdate);
        }

        // Update is called once per frame
        void Update()
        {
        
        }

        public void OnLobbyDataUpdate(LobbyDataUpdate_t callback)
        {
            lobbyID = new CSteamID(callback.m_ulSteamIDLobby);
            CSteamID memberID = new CSteamID(callback.m_ulSteamIDMember);

            UpdateProfileList(lobbyID);
            UpdateProfileView();
            StartGame();
        }
        public static LobbyManager.PlayerData GetLobbyMemberData(CSteamID lobbyID, CSteamID playerID)
        {
            string json = SteamMatchmaking.GetLobbyMemberData(lobbyID, playerID, "playerData");
            var player = JsonUtility.FromJson<LobbyManager.PlayerData>(json);
            player.playerID = playerID.m_SteamID;
            return player;
        }

        public void UpdateProfileList(CSteamID lobbyID)
        {
            profiles = new List<LobbyManager.PlayerData>();
            for (int i = 0; i < SteamMatchmaking.GetNumLobbyMembers(lobbyID); i++)
            {
                CSteamID memberID = SteamMatchmaking.GetLobbyMemberByIndex(lobbyID, i);
                profiles.Add(GetLobbyMemberData(lobbyID, memberID));
                profiles[i].playerIndex = i;
            }
        }

        public void UpdateProfileView()
        {
            var i = 0;
            foreach (var item in profiles)
            {
                displays[i].SetProfile(item.ToProfile());
                displays[i].assistItem.ChangeUIVisibility(1);
                displays[i].gameObject.SetActive(true);
                i++;
            }
            for (int j; i < displays.Count; i++)
            {
                displays[i].profile = null;
                displays[i].gameObject.SetActive(false);
            }
        }

        public void TriggerStart()
        {
            // Ensure only the host can start the game
            if (!SteamMatchmaking.GetLobbyOwner(lobbyID).Equals(SteamUser.GetSteamID()))
            {
                Debug.LogWarning("Only the host can start the game!");
                return;
            }

            // Broadcast a "game start" message
            SteamMatchmaking.SetLobbyData(lobbyID, "gameState", "starting");
            Debug.Log("Starting the game...");
        }

        private void StartGame()
        {
            if (SteamMatchmaking.GetLobbyData(lobbyID, "gameState") != "starting") return;
            Debug.Log("GameTransition!");
            // Load the game scene
            UpdateProfileList(lobbyID);

            stageBringer.lobbyDevice = new List<UnityEngine.InputSystem.InputDevice>
            {
                FindObjectOfType<PlayerInput>().devices[0]
            };

            stageBringer.StageToLoad = "HakureiShrine";

            stageBringer.stageInfo.timer = 60 * 7; //DUMMY
            stageBringer.stageInfo.stockCount = 4; //DUMMY
            stageBringer.stageInfo.spellcardCount = 2; //DUMMY

            foreach (var item in profiles)
            {
                stageBringer.stageInfo.player.Add(item);
            }

            stageBringer.LoadScene();
        }
    }
}
