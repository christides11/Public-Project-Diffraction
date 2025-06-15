using Steamworks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TightStuff
{
    public class LobbyItem : MonoBehaviour
    {
        public CSteamID lobbyID;

        private void Start()
        {
        }

        public void JoinLobby()
        {
            SteamMatchmaking.JoinLobby(lobbyID);
        }
    }
}
