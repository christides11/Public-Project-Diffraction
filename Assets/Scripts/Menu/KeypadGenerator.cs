using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

namespace TightStuff
{
    public class KeypadGenerator /* : EditorWindow */
    {
        /*
        [SerializeField] private GameObject buttonPrefab;
        [SerializeField] private Transform keypadContainer;
        private string allowedCharacters = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz .,!?'";

        [MenuItem("Tools/Generate Keypad")]
        public static void ShowWindow()
        {
            GetWindow<KeypadGenerator>("Keypad Generator");
        }

        private void OnGUI()
        {
            GUILayout.Label("Keypad Settings", EditorStyles.boldLabel);

            buttonPrefab = (GameObject)EditorGUILayout.ObjectField("Button Prefab", buttonPrefab, typeof(GameObject), false);
            keypadContainer = (Transform)EditorGUILayout.ObjectField("Keypad Container", keypadContainer, typeof(Transform), true);

            if (GUILayout.Button("Generate Keypad"))
            {
                GenerateKeypad();
            }
        }

        private void GenerateKeypad()
        {
            if (buttonPrefab == null || keypadContainer == null)
            {
                Debug.LogError("Button prefab or keypad container not set!");
                return;
            }

            // Clear existing buttons
            foreach (Transform child in keypadContainer)
            {
                DestroyImmediate(child.gameObject);
            }

            // Instantiate buttons for each allowed character
            foreach (char character in allowedCharacters)
            {
                GameObject button = PrefabUtility.InstantiatePrefab(buttonPrefab, keypadContainer) as GameObject;
                button.GetComponentInChildren<Text>().text = character.ToString();
                button.name = $"Button_{character}";

                // Optional: Attach a listener to the button if you want it ready at runtime
                button.GetComponent<Button>().onClick.AddListener(() => Debug.Log($"Character selected: {character}"));
            }

            Debug.Log("Keypad generated successfully!");
        }*/
    }
}
