using System.Collections;
using System.Collections.Generic;
using System.IO;
using TightStuff.Menu;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace TightStuff
{
    public class MainProfile : MonoBehaviour
    {
        public static MainProfile Instance;

        public PlayerTag mainProfile;
        public LobbyManager.PlayerData playerData;

        public UnityEvent<PlayerTag> OnChangedMainProfile;

        public TextAsset defaultProfile;
        public PlayerInput input;

        // Start is called before the first frame update
        void Start()
        {
            if (Instance == null)
                Instance = this;
            else
            {
                Destroy(gameObject);
                return;
            }

            string loadedFileName = PlayerPrefs.GetString("MainProfile", "");
            if (string.IsNullOrEmpty(loadedFileName))
            {
                GetJsonFiles("PlayerProfiles");
            }
            else
            {
                mainProfile = PlayerTag.FromJson(defaultProfile.text);
                mainProfile.tag = "GUEST";
                mainProfile.fighterInUse = new PlayerTag.FavouriteFighter();
                mainProfile.fighterInUse.fighterID = 0;
                mainProfile.fighterInUse.favouriteCostumeID = 0;
                mainProfile.fighterInUse.tagColor = ColorAndPaletteAssigner.instance.colors[0].color;
                mainProfile.assistInUse = 1;
                Debug.Log("load new main");
            }

            if (mainProfile != null)
            {
                playerData = new LobbyManager.PlayerData(mainProfile);
                CheckAndSwitchTapToggles();
            }
            OnChangedMainProfile?.Invoke(mainProfile);

            //File.WriteAllText("PlayerProfiles/test.json", JsonUtility.ToJson(playerData));
        }

        public void CheckAndSwitchTapToggles()
        {
            if (mainProfile == null)
                return;
            if (mainProfile.tapOptions.Count <= 0) return;

            playerData.tapToggles = mainProfile.tapOptions[0];
            if (input.currentControlScheme == "Keyboard")
            {
                playerData.tapToggles = mainProfile.tapOptions[1];
            }
        }

        // Update is called once per frame
        void Update()
        {
        
        }

        public string[] GetJsonFiles(string folderPath)
        {
            if (Directory.Exists(folderPath))
            {
                string[] jsonFiles = Directory.GetFiles(folderPath, "*.json");


                foreach (string filePath in jsonFiles)
                {
                    string json = File.ReadAllText(filePath);
                    PlayerTag playerTag = PlayerTag.FromJson(json);

                    if (playerTag != null)
                    {
                        mainProfile = playerTag;
                        if (mainProfile != null)
                        {
                            mainProfile.fighterInUse = null;
                            //mainProfile.assistInUse = FighterAssistList.instance.AssistProperties.Count - 1;
                        }
                        return jsonFiles;
                    }
                    else
                    {
                        Debug.LogWarning("Failed to load PlayerTag from: " + filePath);
                    }
                }
                return jsonFiles;
            }
            else
            {
                Debug.LogWarning("Directory not found: " + folderPath);
                return new string[0];  // Return an empty array if the folder doesn't exist
            }
        }
    }
}
