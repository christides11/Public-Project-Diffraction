namespace TightStuff.Menu
{
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using UnityEngine;
    using UnityEngine.InputSystem;

    [System.Serializable]
    public class PlayerTag
    {
        public string tag;
        public List<FavouriteFighter> favouriteFighters;
        public FavouriteFighter fighterInUse;
        public InputActionAsset rebind;
        public int assistInUse;
        public int id;
        public string rebindOverrides;

        public int bufferWindow;
        public int stickSensitivity;

        public bool autoLedgeGrab;
        public List<TapToggles> tapOptions;

        [System.Serializable]
        public class TapToggles
        {
            public string inputDevice;

            public bool tapDash;
            public bool tapJump;
            public bool tapSmash;
            public bool tapAirdash;
            public bool tapDrop;

            public bool doubleTap;
        }

        [System.Serializable]
        public class FavouriteFighter
        {
            public int fighterID;
            public int favouriteCostumeID;
            public int colorID;
            public Color tagColor;

            public FavouriteFighter()
            {
                fighterID = 0;
                favouriteCostumeID = 0;
                tagColor = Color.white;
            }
        }
        // Save this PlayerTag object to a JSON file
        public void SaveToJson(string filePath, string folderPath)
        {
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }
            Wrapper wrapper = new Wrapper();
            wrapper.profile = this;
            wrapper.action = rebind?.ToJson() ?? "{}";

            string json = JsonUtility.ToJson(wrapper, true);
            File.WriteAllText(folderPath + "/" + filePath, json);
        }
        // Load a PlayerTag object from a JSON file
        public static PlayerTag LoadFromJson(string filePath)
        {

            if (File.Exists(filePath))
            {
                string json = File.ReadAllText(filePath);

                // Parse the main wrapper JSON
                var parsedJson = JsonUtility.FromJson<Wrapper>(json);

                PlayerTag prof = parsedJson.profile;

                // Deserialize ClassA
                prof.rebind = InputActionAsset.FromJson(parsedJson.action);

                return prof;
            }
            else
            {
                Debug.LogWarning("File not found: " + filePath);
                return null;
            }
        }

        // Load a PlayerTag object from a JSON file
        public static PlayerTag FromJson(string json)
        {
            // Parse the main wrapper JSON
            var parsedJson = JsonUtility.FromJson<Wrapper>(json);

            PlayerTag prof = parsedJson.profile;

            // Deserialize ClassA
            prof.rebind = InputActionAsset.FromJson(parsedJson.action);

            return prof;
        }

        public static void DeleteFile(string filePath)
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
                Debug.Log("Deleted file: " + filePath);
            }
            else
            {
                Debug.LogWarning("File not found: " + filePath);
            }
        }


        private class Wrapper
        {
            public PlayerTag profile;
            public string action;
        }
    }
}
