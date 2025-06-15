namespace TightStuff.Menu
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;
    using UnityEngine.InputSystem;
    using UnityEngine.InputSystem.UI;
    using UnityEngine.Events;
    using UnityEditor;
    
    public class CharacterSelectManager : MonoBehaviour
    {
        public static CharacterSelectManager instance;
    
        [SerializeField]
        public List<Player> player;
    
        [SerializeField]
        private List<GameObject> _playerSlotMenus;
    
        [SerializeField]
        private List<GameObject> _characterSelectMenus;
    
        [SerializeField]
        private List<PButton> _PButtons;
    
        [SerializeField]
        private List<RectTransform> _controllerReps;
    
        [SerializeField]
        private List<PlayerColor> _playerColors;
    
        [SerializeField]
        private List<DissolveToPlayerBackground> _bottomCovers;
    
        public UnityEvent OnSelectCharacter; 
        public UnityEvent OnSelectStage;
    
        public UnityEvent OnExceed2Players;
        public UnityEvent OnBelow2Players;
    
        public UnityEvent OnEveryoneReady;
        public UnityEvent OnEveryoneReadyCancel;
    
        public List<PlayerColor> PlayerColors => _playerColors;
        public List<PButton> PButtons => _PButtons;
    
        public ControllerInputs controller;
        public bool readyForBattle;
        public bool stageSelecting = true;
    
        [System.Serializable]
        public class PButton
        {
            public List<GameObject> button;
            public List<Player> selected = new List<Player>();
    
            public enum SelectionMode { NoSlot, CharacterSelecting, ChoosingAssist, SecondaryMenu, Ready }
            public SelectionMode currentMode;
            public SelectionMode previousMode;
    
            public UnityEvent OnConfirmCharacter;
            public UnityEvent OnConfirmPlayerSlot;
            public UnityEvent OnReady;
            public UnityEvent OnUnready;
            public UnityEvent OnCancelCharacter;
            public UnityEvent OnCancelPlayerSlot;
    
            public void OnCancel()
            {
                if (currentMode == SelectionMode.Ready)
                    OnCancelCharacter.Invoke();
                else if (currentMode == SelectionMode.CharacterSelecting)
                    OnCancelPlayerSlot.Invoke();
            }
        }
    
        [System.Serializable]
        public class PlayerColor
        {
            public Material gemColor;
            public bool occupied;
            public Color color;
    
            public void Occupy(bool val)
            {
                occupied = val;
            }
        }
    
        private int temp;
    
        [System.Serializable]
        public class Player
        {
            public PlayerTag tag;
            public PlayerInput input;
            public MultiplayerEventSystem multiplayerEvent;
    
            public FighterProperties currentFighter;
            public int id;
            public int currentPalette;
            public int currentColor;
    
            public bool fullReady;
    
            public Player(PlayerInput input)
            {
                this.input = input;
                input.gameObject.TryGetComponent(out multiplayerEvent);
                id = -1;
                currentColor = -1;
            }
            public int ScrollRightPalette()
            {
                currentPalette++;
                if (currentPalette > 7)
                    currentPalette = 0;
                return currentPalette;
            }
            public int ScrollLeftPalette()
            {
                currentPalette--;
                if (currentPalette < 0)
                    currentPalette = 7;
                return currentPalette;
            }
    
        }
    
        // Start is called before the first frame update
        void Start()
        {
            if (instance == null)
                instance = this;
            else
                Destroy(instance.gameObject);
            player = new List<Player>();
            foreach (PButton p in _PButtons)
                foreach (GameObject go in p.button)
                    go.SetActive(false);
        }
    
        // Update is called once per frame
        void FixedUpdate()
        {
            for (int i = 0; i < player.Count; i++)
            {
                for (int j = 0; j < _PButtons.Count; j++)
                {
                    GameObject button = _PButtons[j].button[i];
                    if (player[i].multiplayerEvent.currentSelectedGameObject == button)
                        _PButtons[j].selected.Add(player[i]);
                }
            }
            if (stageSelecting)
                return;
    
            for (int i = 0; i < player.Count; i++)
            {
                var selected = player[i].multiplayerEvent.currentSelectedGameObject.transform;
                _controllerReps[i].transform.SetParent(selected);
    
    
                var count = 0;
                var count2 = 0;
                for (int j = 0; j < _PButtons.Count; j++)
                {
                    for (int k = 0; k < _PButtons[j].selected.Count; k++)
                    {
                        Player player = _PButtons[j].selected[k];
                        if (player == this.player[i])
                        {
                            count2 = k;
                            count = _PButtons[j].selected.Count;
                        }
                    }
                }
    
                // Convert the angle from degrees to radians
                float angleInRadians = 0;
                if (count != 0)
                {
                    angleInRadians = -Mathf.Deg2Rad * ((360 / count) * count2 + 180 + (count == 3 ? 30 : 0));
                }
                float xComponent = Mathf.Cos(angleInRadians);
                float yComponent = Mathf.Sin(angleInRadians);
                Vector2 unitVector = new Vector2(xComponent, yComponent) * (count <= 1 ? 0 : 1);
    
                _controllerReps[i].anchoredPosition = Vector2.Lerp(_controllerReps[i].anchoredPosition, unitVector * 25, 0.5f);
                _controllerReps[i].localScale = Vector3.one;
            }
            foreach (var button in _PButtons)
                button.selected.Clear();
        }
    
        public void AddPlayer(PlayerInput input)
        {
            player.Add(new Player(input));
            player[player.Count - 1].multiplayerEvent.playerRoot = _playerSlotMenus[player.Count - 1];
            player[player.Count - 1].multiplayerEvent.firstSelectedGameObject = _playerSlotMenus[player.Count - 1].transform.Find("Unselected").gameObject;
            player[player.Count - 1].multiplayerEvent.SetSelectedGameObject(player[player.Count - 1].multiplayerEvent.firstSelectedGameObject);
            _playerSlotMenus[player.Count - 1].SetActive(true);
            foreach (GameObject p in _PButtons[player.Count - 1].button)
                p.SetActive(true);
            if (player.Count >= 3)
            {
                OnExceed2Players?.Invoke();
                foreach (var bottomCover in _bottomCovers)
                    bottomCover.SetDissolve(false);
            }
            player[player.Count - 1].multiplayerEvent.enabled = false;
        }
        public void RemovePlayer(PlayerInput input)
        {
            foreach(Player player in player)
                if(player.input.Equals(input))
                {
                    player.multiplayerEvent.playerRoot.SetActive(false);
                    this.player.Remove(player);
                }
            if (player.Count < 3)
            {
                OnBelow2Players?.Invoke();
                foreach (var bottomCover in _bottomCovers)
                    bottomCover.SetDissolve(true);
            }
            Destroy(input.gameObject);
        }
        public void PlayerSlotClick(int i)
        {
            foreach(GameObject p in _PButtons[i].button)
            {
                foreach (Player player in player)
                {
                    if (player.multiplayerEvent.currentSelectedGameObject == p)
                        player.multiplayerEvent.SetSelectedGameObject(player.multiplayerEvent.firstSelectedGameObject);
                }
                p.SetActive(false);
            }
            _PButtons[i].currentMode = PButton.SelectionMode.CharacterSelecting;
            _characterSelectMenus[i].SetActive(true);
            temp = i;
        }
        public void PlayerSlotCancel(int i)
        {
            foreach (GameObject p in _PButtons[i].button)
                p.SetActive(true);
            _PButtons[i].currentMode = PButton.SelectionMode.NoSlot;
            _characterSelectMenus[i].SetActive(false);
            for (int j = 0; j < player.Count; j++)
            {
                Player play = player[j];
                if (play.id == i)
                    DeassociateSlotWithID(j);
            }
            temp = i;
        }
        public void AssociateSlotWithID(int i)
        {
            _playerSlotMenus[i].SetActive(false);
            _controllerReps[i].GetComponent<Image>().enabled = false;
            _controllerReps[i].GetChild(0).GetComponent<RectTransform>().anchoredPosition = new Vector3(1, 0, 0);
            player[i].input.gameObject.GetComponent<MenuControls>().OnCancelButton.AddListener(_PButtons[temp].OnCancel);
            player[i].id = temp;
            player[i].multiplayerEvent.playerRoot = _characterSelectMenus[temp];
            player[i].multiplayerEvent.SetSelectedGameObject(_characterSelectMenus[temp].transform.Find("NameTag").gameObject);
        }
        public void DeassociateSlotWithID(int i)
        {
            _playerSlotMenus[i].SetActive(true);
            _controllerReps[i].GetComponent<Image>().enabled = true;
            _controllerReps[i].GetChild(0).GetComponent<RectTransform>().anchoredPosition = new Vector3(1, 22, 0);
            player[i].id = -1;
            player[i].multiplayerEvent.playerRoot = _playerSlotMenus[i];
            player[i].multiplayerEvent.SetSelectedGameObject(_playerSlotMenus[i].transform.Find("Unselected").gameObject);
            player[i].input.gameObject.GetComponent<MenuControls>().OnCancelButton.RemoveAllListeners();
            if (player[i].currentColor >= 0)
                _playerColors[player[i].currentColor].occupied = false;
            player[i].currentColor = -1;
            player[i].currentFighter = null;
        }
    
        public int[] AssignPaletteAndPlayerColor(FighterProperties fighter, int playerID)
        {
            if (player[playerID].currentColor >= 0)
                _playerColors[player[playerID].currentColor].occupied = false;
            player[playerID].currentFighter = fighter;
    
            var palette = 0;
            var color = 0;
            if (player[playerID].tag != null)
            {
                //implement tag favourites
            }
            var j = 0;
            for (int i = 0; i < player.Count; i++)
            {
                if (player[i].currentPalette == palette && player[i].currentFighter == fighter && i != playerID)
                {
                    palette = j;
                    j++;
                    i = -1;
                }
            }
            color = fighter.palette[palette].defaultPlayerColorID;
            if (_playerColors[color].occupied)
            {
                color = fighter.palette[palette].backupPlayerColorID;
                if (_playerColors[color].occupied)
                {
                    for (int i = 0; i < _playerColors.Count; i++)
                    {
                        PlayerColor col = _playerColors[i];
                        if (!col.occupied)
                        {
                            color = i;
                            break;
                        }
                    }
                }
            }
            player[playerID].currentPalette = palette;
            player[playerID].currentColor = color;
            _playerColors[color].occupied = true;
            int[] k = { palette, color };
            return k;
        }
    
        public void SelectedCharacter(int i)
        {
            _PButtons[i].previousMode = _PButtons[i].currentMode;
            _PButtons[i].currentMode = PButton.SelectionMode.ChoosingAssist;
            _PButtons[i].OnUnready?.Invoke();
            for (int j = 0; j < player.Count; j++)
            {
                if (player[j].id == i)
                {
                    player[j].fullReady = false;
                    CheckAllPlayersReady();
                    return;
                }
            }
        }
        public void SecondaryMenu(int i)
        {
            _PButtons[i].previousMode = _PButtons[i].currentMode;
            _PButtons[i].currentMode = PButton.SelectionMode.SecondaryMenu;
            _PButtons[i].OnUnready?.Invoke();
            for (int j = 0; j < player.Count; j++)
            {
                if (player[j].id == i)
                {
                    player[j].fullReady = false;
                    CheckAllPlayersReady();
                    return;
                }
            }
        }
        public void BackToPrevMode(int i)
        {
            if (_PButtons[i].previousMode == PButton.SelectionMode.Ready)
            {
                PlayerReady(i);
            }
            else if (_PButtons[i].previousMode == PButton.SelectionMode.CharacterSelecting)
            {
                if (_PButtons[i].currentMode == PButton.SelectionMode.ChoosingAssist)
                    _PButtons[i].OnCancelCharacter.Invoke();
                UnselectedCharacter(i);
            }
            else if (_PButtons[i].previousMode == PButton.SelectionMode.SecondaryMenu)
            {
                SecondaryMenu(i);
            }
        }
    
        public void UnselectedCharacter(int i)
        {
            _PButtons[i].previousMode = _PButtons[i].currentMode;
            _PButtons[i].currentMode = PButton.SelectionMode.CharacterSelecting;
            _PButtons[i].OnUnready?.Invoke();
            for (int j = 0; j < player.Count; j++)
            {
                if (player[j].id == i)
                {
                    player[j].fullReady = false;
                    CheckAllPlayersReady();
                    return;
                }
            }
        }
        public void PlayerReady(int i)
        {
            _PButtons[i].previousMode = _PButtons[i].currentMode;
            _PButtons[i].currentMode = PButton.SelectionMode.Ready;
            _PButtons[i].OnReady?.Invoke();
            for (int j = 0; j < player.Count; j++)
            {
                if (player[j].id == i)
                {
                    player[j].fullReady = true;
                    CheckAllPlayersReady();
                    return;
                }
            }
        }
    
        public void CheckAllPlayersReady()
        {
            for (int i = 0; i < player.Count; i++)
            {
                if (!player[i].fullReady)
                {
                    readyForBattle = false;
                    OnEveryoneReadyCancel?.Invoke();
                    return;
                }
            }
            OnEveryoneReady?.Invoke();
            readyForBattle = true;
        }
    
        public void OnSubmitPlayerSlot(int i)
        {
            _PButtons[i].OnConfirmPlayerSlot?.Invoke();
        }
        public void OnSubmitCharacter(int i)
        {
            _PButtons[i].OnConfirmCharacter?.Invoke();
        }
        public void OnConfirmStage()
        {
            OnSelectCharacter?.Invoke();
    
            foreach (Player player in player)
                player.multiplayerEvent.enabled = true;
            stageSelecting = true;
        }
        public void OnCancelStage()
        {
            OnSelectStage?.Invoke();
            foreach (Player player in player)
                player.multiplayerEvent.enabled = false;
            stageSelecting = false;
        }
    }
}
