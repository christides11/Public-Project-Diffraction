using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;

namespace TightStuff
{
    public class SteamScript : MonoBehaviour
    {
        void Start()
        {
            if (SteamManager.Initialized)
            {
                string name = SteamFriends.GetPersonaName();
                Debug.Log(name);
            }
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }
}
