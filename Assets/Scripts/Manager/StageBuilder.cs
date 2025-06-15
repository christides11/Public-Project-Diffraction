namespace TightStuff
{
    using System.Collections;
    using System.Collections.Generic;
    using TightStuff.Menu;
    using UnityEngine;
    using UnityEngine.InputSystem;
    
    public class StageBuilder : MonoBehaviour
    {
        [SerializeField]
        private MatchManager _match;
        [SerializeField]
        private GameObject _HUDs;
        [SerializeField]
        private CamProperties _camera;
        [SerializeField]
        private GameObject _gamePanel; //TEMPORARY
    
        private PlayerInputManager _playerInputManager;
    
        public List<GameObject> assistList;
        public List<GameObject> fighterList;
    
        [SerializeField]
        private Transform _fighterContainer;
    
        [SerializeField]
        private List<Transform> _spawnPoints;
    
        public StageInfo stageInfo;
        public List<InputDevice> inputDevices;
    
        private void OnEnable()
        {
            //SetupFight();
            _playerInputManager = FindObjectOfType<PlayerInputManager>();
            _playerInputManager.joinBehavior = PlayerJoinBehavior.JoinPlayersManually;
        }
    
        [System.Serializable]
        public class StageInfo
        {
            public string stageName;
            public List<LobbyManager.PlayerData> player;
            public float timer;
            public int stockCount;
            public int spellcardCount;

            public bool teamBattle;

            public MusicObject music;
        }
    
    
        public void SetupFight(StageInfo info)
        {
            stageInfo = info;
    
            MatchManager.initTime = info.timer;
    
            for (int i = 0; i < info.player.Count; i++)
            {
                GameObject fighter = FighterAssistList.instance.FighterProperties[info.player[i].fighterInUse.fighterID].fighterPrefab;
                _playerInputManager.playerPrefab = fighter;
                GameObject fi;
                if (inputDevices != null && i < inputDevices.Count)
                {
                    fi = _playerInputManager.JoinPlayer(i, -1, null, inputDevices[i]).gameObject;
                    Debug.Log("shitfuck");
                }
                else
                    fi = Instantiate(fighter);
    
                fi.GetComponent<Pooler>().pools[0].prefab = FighterAssistList.instance.AssistProperties[info.player[i].assistInUse].AssistPrefab;
                fi.transform.SetParent(_fighterContainer);
                var fighterState = fi.GetComponent<BaseFighterBehavior>();
                fighterState.colorID = info.player[i].fighterInUse.favouriteCostumeID;
                fighterState.controlling.playerColor = info.player[i].fighterInUse.tagColor;
                fighterState.stateVarsF.stocks = info.stockCount;
                fighterState.stateVarsF.spellcardUpCount = info.spellcardCount;
                fighterState.stateVarsF.spellcardDownCount = info.spellcardCount;
                fighterState.stateVarsF.spellcardSideCount = info.spellcardCount;

                var controller = fi.GetComponent<Controller>();
                controller.playerTag = info.player[i].tag;
                controller.bufferWindow = info.player[i].bufferWindow;
                controller.stickSensitivity = info.player[i].stickSensitivity;
                controller.tapAirdash = info.player[i].tapToggles.tapAirdash;
                controller.tapDash = info.player[i].tapToggles.tapDash;
                controller.tapDrop = info.player[i].tapToggles.tapDrop;
                controller.tapJump = info.player[i].tapToggles.tapJump;
                controller.tapSmash = info.player[i].tapToggles.tapSmash;
                controller.doubleTap = info.player[i].tapToggles.doubleTap;
                controller.autoLedgeGrab = info.player[i].autoLedgeGrab;

                controller.rebindFileName = info.player[i].rebindFileName;

                fighterState.controlling.id = info.player[i].playerIndex;
                fi.transform.position = _spawnPoints[fighterState.controlling.id].position;
                if (fighterState.controlling.id % 2 != 0)
                    fighterState.entity.stateVars.flippedLeft = true;
                fi.SetActive(true);
            }
    
            _match.gameObject.SetActive(true);
            if (stageInfo.music != null)
                FindObjectOfType<MusicPlayer>().currentMusic = stageInfo.music;
            Invoke("DelayHUDs", 0.1f);
        }
        public void SetupFight()
        {
            SetupFight(stageInfo);
        }
    
        public void DelayHUDs()
        {
            _camera.enabled = true;
            _HUDs.SetActive(true);
            SharedGame.GameManager.Instance.StartLocalGame();
            //_match.replayer.LoadReplay();
    
    
            //UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(UnityEngine.EventSystems.EventSystem.current.firstSelectedGameObject);
            //_gamePanel.SetActive(true);
        }
    }
}
