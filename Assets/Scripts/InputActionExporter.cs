using UnityEngine;
using UnityEngine.InputSystem;
using System.IO;

namespace TightStuff
{
    public class InputActionExporter : MonoBehaviour
    {
        public string nameTag = "Default";

        public InputActionAsset inputActions;
        public string outputPath = "ActionMaps/Default.json";

        void Start()
        {
            outputPath = "ActionMaps/" + nameTag + ".json";
            ExportToJson();
        }

        public void ExportToJson()
        {
            if (inputActions == null)
            {
                Debug.LogError("No InputActionAsset assigned!");
                return;
            }

            // Convert the InputActionAsset to JSON
            string jsonContent = inputActions.ToJson();

            // Write the JSON to a file
            File.WriteAllText(outputPath, jsonContent);

            Debug.Log($"Input actions exported to JSON at: {outputPath}");
        }
    }
}