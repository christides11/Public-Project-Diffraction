namespace TightStuff
{
    using System.Collections;
    using System.IO;
    using System.Runtime.Serialization.Formatters.Binary;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.SceneManagement;
    using UnityEngine.InputSystem;
    using System;
    using TightStuff.Menu;
    using UnityEngine.EventSystems;
    using UnityEngine.InputSystem.UI;

    public class StageBringer : MonoBehaviour
    {
        public string StageToLoad { get; set; }
    
        public List<GameObject> fighters;
        public StageBuilder.StageInfo stageInfo;
        public MusicPlayer musicPlayer;
        public MusicObject stageSelectTheme;

        public List<InputDevice> lobbyDevice;
    
        private static bool created = false;
        // Start is called before the first frame update
        void Start()
        {
            if (!created)
            {
                DontDestroyOnLoad(gameObject);
                created = true;
            }
            lobbyDevice = new List<InputDevice>();
        }
        void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        private void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    
        void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
    
            // If scene is your game scene, change panel
            if (SceneManager.GetActiveScene().name == "PreBattle" || SceneManager.GetActiveScene().name == "MainMenu" || SceneManager.GetActiveScene().name == "TitleScreen")
                return;
            
            var builder = FindObjectOfType<StageBuilder>();
    
            var devices = new List<InputDevice>();
            
            devices = lobbyDevice;

            builder.inputDevices = devices;
    
            builder.SetupFight(stageInfo);
            Invoke("DestroyObj", 5);
        }
    
        public void LoadScene()
        {
            if (SceneManager.GetActiveScene().name == "PreBattle" || SceneManager.GetActiveScene().name == "MainMenu")
            {
                stageInfo.stageName = StageToLoad;
                SceneManager.LoadScene(StageToLoad, LoadSceneMode.Single);
            }
        }
        public void LoadSceneSimple()
        {
            SceneManager.LoadScene(stageInfo.stageName, LoadSceneMode.Single);
        }

        public void DestroyObj()
        {
            created = false;
            Destroy(gameObject);
        }
    
        public void LoadReplay()
        {
            string[] filePaths = Directory.GetFiles(Directory.GetCurrentDirectory(), "*.scenario");
            if (filePaths.Length <= 0)
                return;
            try
            {
                using (Stream stream = File.Open(filePaths[0], FileMode.Open))
                {
                    var bformatter = new BinaryFormatter();
    
                    stageInfo = (StageBuilder.StageInfo)bformatter.Deserialize(stream);
                    StageToLoad = stageInfo.stageName;
                    Replayer.replaying = true;
                }
            }
            catch (Exception e)
            {
                Debug.Log("Ah");
            }
        }

        public void ExitApp()
        {
            Application.Quit();
        }
        public void SetStageMusic(MusicObject music)
        {
            stageInfo.music = music;
        }
        public void SetPlayingMusicAsStageMusic()
        {
            if (musicPlayer.currentMusic != stageSelectTheme)
            {
                Debug.Log(musicPlayer.currentMusic);
                var stage = stageInfo;
                stage.music = musicPlayer.currentMusic;
                stageInfo = stage;
            }
        }
    }
}
