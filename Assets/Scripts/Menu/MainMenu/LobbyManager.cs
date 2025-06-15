using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace TightStuff.Menu
{
    public class LobbyManager : MonoBehaviour
    {
        public GameObject lobbyEntryPrefab;
        public Transform lobbyListParent;

        private Callback<LobbyMatchList_t> lobbyMatchListCallback;
        private Callback<LobbyCreated_t> lobbyCreatedCallback;
        private Callback<LobbyEnter_t> lobbyEnterCallback;

        public UnityEvent OnCreateLobby;
        public UnityEvent OnJoinLobby;

        public void Start()
        {
            lobbyMatchListCallback = Callback<LobbyMatchList_t>.Create(OnLobbyMatchList);
            // Register the lobby creation callback
            lobbyCreatedCallback = Callback<LobbyCreated_t>.Create(OnLobbyCreated);
            lobbyEnterCallback = Callback<LobbyEnter_t>.Create(OnLobbyEnter);
        }

        public void FetchLobbies()
        {
            ClearLobbyList();
            SteamMatchmaking.RequestLobbyList();
        }

        public void OnLobbyMatchList(LobbyMatchList_t callback)
        {
            Debug.Log($"Found {callback.m_nLobbiesMatching} lobbies.");
            for (int i = 0; i < callback.m_nLobbiesMatching; i++)
            {
                CSteamID lobbyID = SteamMatchmaking.GetLobbyByIndex(i);
                FetchLobbyDetails(lobbyID);
            }
        }

        private void FetchLobbyDetails(CSteamID lobbyID)
        {
            string lobbyName = SteamMatchmaking.GetLobbyData(lobbyID, "name");
            string currentPlayers = SteamMatchmaking.GetLobbyData(lobbyID, "playerCount");
            string maxPlayers = SteamMatchmaking.GetLobbyData(lobbyID, "maxPlayers");

            DisplayLobby(lobbyID, lobbyName, currentPlayers, maxPlayers);
        }
        public void ClearLobbyList()
        {
            foreach (Transform child in lobbyListParent)
            {
                Destroy(child.gameObject);
            }
        }

        private void DisplayLobby(CSteamID lobbyID, string lobbyName, string currentPlayers, string maxPlayers)
        {
            GameObject lobbyEntry = Instantiate(lobbyEntryPrefab, lobbyListParent);
            lobbyEntry.GetComponent<LobbyItem>().lobbyID = lobbyID;
            lobbyEntry.GetComponentInChildren<Text>().text = $"{lobbyName} ({currentPlayers}/{maxPlayers})";

            Button joinButton = lobbyEntry.GetComponentInChildren<Button>();
            joinButton.onClick.AddListener(() => JoinLobby(lobbyID));
        }

        private void JoinLobby(CSteamID lobbyID)
        {
            SteamMatchmaking.JoinLobby(lobbyID);
        }

        private void OnLobbyEnter(LobbyEnter_t callback)
        {
            Debug.Log("Joined lobby successfully!");
            CSteamID lobbyID = new CSteamID(callback.m_ulSteamIDLobby);

            SteamMatchmaking.SetLobbyMemberData(lobbyID, "playerData", JsonUtility.ToJson(MainProfile.Instance.playerData));
            OnJoinLobby?.Invoke();
        }

        // Called when the "Create Lobby" button is pressed
        public void CreateLobby()
        {
            SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypePublic, 4); // Public lobby for 4 players
            Debug.Log("Creating lobby...");
        }

        private void OnLobbyCreated(LobbyCreated_t callback)
        {
            if (callback.m_eResult == EResult.k_EResultOK)
            {
                Debug.Log("Lobby created successfully!");
                CSteamID lobbyID = new CSteamID(callback.m_ulSteamIDLobby);

                // Set initial lobby data
                SteamMatchmaking.SetLobbyData(lobbyID, "name", "My Lobby");
                SteamMatchmaking.SetLobbyData(lobbyID, "playerCount", "1");
                SteamMatchmaking.SetLobbyData(lobbyID, "maxPlayers", "4");

                SteamMatchmaking.SetLobbyMemberData(lobbyID, "playerData", JsonUtility.ToJson(MainProfile.Instance.playerData));

                OnCreateLobby?.Invoke();

                // Transition to the lobby screen or display success
                Debug.Log($"Lobby ID: {lobbyID}");
            }
            else
            {
                Debug.LogError("Failed to create lobby.");
            }
        }

        [System.Serializable]
        public class PlayerData
        {
            public string tag;
            public PlayerTag.FavouriteFighter fighterInUse;
            public int assistInUse;

            public bool autoLedgeGrab;
            public int bufferWindow;
            public int stickSensitivity;

            public int playerIndex;
            public ulong playerID;

            public PlayerTag.TapToggles tapToggles;

            public string rebindFileName;

            public void SetPlayerData(PlayerTag profile)
            {
                tag = profile.tag;
                if (profile.fighterInUse != null )
                {
                    fighterInUse = profile.fighterInUse;
                }
                else if (profile.favouriteFighters != null)
                {
                    if (profile.favouriteFighters.Count != 0)
                    {
                        fighterInUse = profile.favouriteFighters[0];
                    }
                }

                assistInUse = profile.assistInUse;

                bufferWindow = profile.bufferWindow;
                stickSensitivity = profile.stickSensitivity;

                tapToggles = profile.tapOptions[0];
                autoLedgeGrab = profile.autoLedgeGrab;
            }
            public PlayerData(PlayerTag profile)
            {
                tag = profile.tag;
                if (profile.fighterInUse != null)
                {
                    fighterInUse = profile.fighterInUse;
                }
                else if (profile.favouriteFighters != null)
                {
                    if (profile.favouriteFighters.Count != 0)
                    {
                        fighterInUse = profile.favouriteFighters[0];
                    }
                }
                assistInUse = profile.assistInUse;

                bufferWindow = profile.bufferWindow;
                stickSensitivity = profile.stickSensitivity;
                autoLedgeGrab = profile.autoLedgeGrab;
                
                tapToggles = profile.tapOptions[0];
            }
            public PlayerData() { }
            public PlayerTag ToProfile()
            {
                PlayerTag profile = new PlayerTag();
                profile.tag = tag;
                profile.fighterInUse = fighterInUse;

                profile.tapOptions = new List<PlayerTag.TapToggles> { tapToggles };

                profile.autoLedgeGrab = autoLedgeGrab;
                
                profile.bufferWindow = bufferWindow;
                profile.stickSensitivity = stickSensitivity;
                profile.assistInUse = assistInUse;

                return profile;
            }
        }
    }
}
