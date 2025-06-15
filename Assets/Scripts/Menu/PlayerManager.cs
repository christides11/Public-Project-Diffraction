namespace TightStuff.Menu
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.InputSystem;
    using UnityEngine.InputSystem.UI;
    using UnityEngine.Events;
    
    public class PlayerManager : MonoBehaviour
    {
        public static PlayerManager instance;
        public PlayerInputManager inputManager;
        public StageBringer bringer;
    
        public List<PlayerController> controllerList = new List<PlayerController>();
        public List<PlayerSelectAestheticController> playerInfoList;
        public List<PlayerControlledCanvas> canvases;
        public int currentMode;
    
        public UnityEvent OnStartFight;
        public UnityEvent OnAllReady;
        public UnityEvent OnAllNotReady;
        public UnityEvent OnSelectStage;
        public UnityEvent OnSelectStageDelayed;
        public UnityEvent OnDeselectStage;
        public UnityEvent On3Players;

        public PlayerInput ignore;
    
        [System.Serializable]
        public class PlayerController
        {
            public PlayerInput input;
            public InputDevice device;
            public MultiplayerEventSystem multiplayerEvent;

            public int playerId;
        }
    
        // Start is called before the first frame update
        void OnEnable()
        {
            inputManager.joinBehavior = PlayerJoinBehavior.JoinPlayersWhenButtonIsPressed;
            controllerList = new List<PlayerController>();
            MatchManager.worldTime = 1;
            InputSystem.settings.updateMode = InputSettings.UpdateMode.ProcessEventsInDynamicUpdate;
            if (instance == null)
                instance = this;
            else
                Destroy(instance);
        }
    
        // Update is called once per frame
        void Update()
        {
            
        }
    
        public void OnJoin(PlayerInput input)
        {
            if (!inputManager.joiningEnabled)
                return;

            var cont = new PlayerController();
            cont.input = input;
            cont.device = input.devices[0];
            cont.multiplayerEvent = input.GetComponent<MultiplayerEventSystem>();
            controllerList.Add(cont);
            canvases[controllerList.Count - 1].player = cont;
            canvases[controllerList.Count - 1].Select(canvases[controllerList.Count - 1].firstSelected);
            canvases[controllerList.Count - 1].AddListener();
            //canvases[controllerList.Count - 1].UnderSlider = false;
            if (controllerList.Count == 3)
                On3Players?.Invoke();
        }
        public void OnLeave(PlayerInput input)
        {
            if (input == ignore)
                return;
            if (input.playerIndex >= controllerList.Count)
            controllerList.RemoveAt(input.playerIndex);
        }
    
        public void OnReady()
        {
            var ready = true;
            for (int i = 0; i < controllerList.Count; i++)
            {
                //Debug.Log((canvases[i].currentMode != 3).ToString() + " " + (canvases[i].UnderSlider).ToString());
                if (canvases[i].currentMode != 3 || canvases[i].UnderSlider)
                    ready = false;
            }
            if (ready)
            {
                Debug.Log("fuk");
                currentMode = 2;
                OnAllReady?.Invoke();
            }
            else if (currentMode == 2)
            {
                currentMode = 1;
                OnAllNotReady?.Invoke();
            }
        }
    
        public void OnSelectedStage()
        {
            OnSelectStage?.Invoke();
            currentMode = 1;
            Invoke("OnSelectedStageDelayed", 0.3f);
        }
        public void OnSelectedStageDelayed()
        {
            OnSelectStageDelayed?.Invoke();
        }
        public void OnDeelectedStage()
        {
            foreach (var can in canvases)
            {
                can.RemoveListener();
                can.player = null;
                can.UnderSlider = false;
                can.ResetCanvasState();
            }
            for (int i = controllerList.Count - 1; i >= 0; i--)
            {
                Destroy(controllerList[i].input.gameObject);
            }
    
            controllerList.Clear();
            OnDeselectStage?.Invoke();
            currentMode = 0;
        }
    
        public void DisableAllControls()
        {
            foreach (var control in controllerList)
            {
                control.multiplayerEvent.GetComponent<InputSystemUIInputModule>().enabled = false;
                control.multiplayerEvent.SetSelectedGameObject(null);
                control.multiplayerEvent.GetComponent<MenuControls>().enabled = false;
            }
            inputManager.DisableJoining();
        }
    
        public void OnStartGame()
        {
            var list = new List<PlayerController>();
            foreach (var controller in controllerList)
                list.Add(controller);

            foreach (var control in list)
                bringer.lobbyDevice.Add(control.device);

            Debug.Log("sorting player");
            OnStartFight?.Invoke();
            inputManager.joinBehavior = PlayerJoinBehavior.JoinPlayersManually;
            var music = bringer.stageInfo.music;
            bringer.stageInfo = new StageBuilder.StageInfo();
            bringer.stageInfo.timer = playerInfoList[0].sliderToTextChange[0].value * 60;
            bringer.stageInfo.stockCount = playerInfoList[0].sliderToTextChange[1].value;
            bringer.stageInfo.spellcardCount = playerInfoList[0].sliderToTextChange[2].value;
            bringer.stageInfo.player = new List<LobbyManager.PlayerData>();
            bringer.stageInfo.music = music;

            for (int i = 0; i < controllerList.Count; i++)
            {
                bringer.stageInfo.player.Add(new LobbyManager.PlayerData(playerInfoList[i].profile));

                bringer.stageInfo.player[i].rebindFileName = playerInfoList[i].profile.tag + playerInfoList[i].profile.id;

                if (bringer.stageInfo.player[i].fighterInUse == null)
                    bringer.stageInfo.player[i].fighterInUse = new PlayerTag.FavouriteFighter();

                bringer.stageInfo.player[i].fighterInUse.fighterID = playerInfoList[i].FighterPrefabId + 1;
                bringer.stageInfo.player[i].fighterInUse.favouriteCostumeID = playerInfoList[i].PlayerPalette;
                bringer.stageInfo.player[i].fighterInUse.tagColor = playerInfoList[i].PlayerColor;
                bringer.stageInfo.player[i].fighterInUse.colorID = ColorAndPaletteAssigner.instance.GetColorIdBasedOnColor(playerInfoList[i].PlayerColor);
                bringer.stageInfo.player[i].playerIndex = playerInfoList[i].PlayerSlot;
                bringer.stageInfo.player[i].assistInUse = playerInfoList[i].SelectedAssist;

                if (controllerList[i].input.currentControlScheme == "Keyboard")
                    bringer.stageInfo.player[i].tapToggles = playerInfoList[i].profile.tapOptions[1];
            }
        }

        public void DestroyObj()
        {
            Destroy(gameObject);
        }
    }
}
