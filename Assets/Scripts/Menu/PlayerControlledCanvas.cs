namespace TightStuff.Menu
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.EventSystems;
    using UnityEngine.InputSystem.UI;
    using UnityEngine.UI;
    
    public class PlayerControlledCanvas : MonoBehaviour
    {
        public PlayerManager.PlayerController player;
        public GameObject firstSelected;
        public RectTransform playerRep;
        public RectTransform initRepParent;
        public Image repImage;
        public Vector3 target;
        public GameObject CurrentlySelected;
        public UnityEvent<GameObject> OnSelected;
    
        public int currentMode;
        private bool _underslider;
        public bool UnderSlider { get => _underslider; set { _underslider = value; canvasModes[currentMode].OnUnderSliderChanged?.Invoke(value, currentMode); } }
    
    
        public List<CanvasMode> canvasModes;
    
        [System.Serializable]
        public class CanvasMode
        {
            public string name;
            public bool longPressCancel;
            public bool holding;
            public float cancelTimer;
            public float maxTimer;

            public GameObject first;
            public List<GameObject> GameObjectsToEnable;
            public List<GameObject> GameObjectsToDisable;
    
            public UnityEvent OnEnterMode;
            public UnityEvent OnExitMode;
            public UnityEvent OnConfirm;
            public UnityEvent OnCancel;
            public UnityEvent OnCancelHoldStart;
            public UnityEvent OnCancelHoldCut;
            public UnityEvent OnCancelHoldFinished;
            public UnityEvent OnLPress;
            public UnityEvent OnRPress;
            public UnityEvent OnBigLPress;
            public UnityEvent OnBigRPress;
            public UnityEvent OnStartPress;
            public UnityEvent OnXPress;
            public UnityEvent OnYPress;
            public UnityEvent<bool, int> OnUnderSliderChanged;
        }
    
    
        public void AssignPlayer(MultiplayerEventSystem multi)
        {
            multi.playerRoot = gameObject;
            multi.firstSelectedGameObject = firstSelected;
        }
    
        public void SelectFirst()
        {
            Select(firstSelected);
        }
    
        public void Select(GameObject obj)
        {
            if (obj == null)
                return;
            if (player == null || !obj.activeInHierarchy)
                return;
            if (player.multiplayerEvent == null)
                return;
            player.multiplayerEvent.SetSelectedGameObject(obj);
        }

        public void SelectIncludeInactive(GameObject obj)
        {
            if (obj == null)
                return;
            if (player == null)
                return;
            player.multiplayerEvent.SetSelectedGameObject(obj);
            if (!obj.activeInHierarchy)
                if (obj.TryGetComponent<ReselectAfterInactive>(out var select))
                {
                    select.OnSelect(new BaseEventData(player.multiplayerEvent));
                }
        }

        public void SwitchMode(int i)
        {
            if (i == currentMode)
                return;
            canvasModes[currentMode].OnExitMode?.Invoke();
    
            currentMode = i;
    
            foreach (var obj in canvasModes[i].GameObjectsToEnable)
                obj.SetActive(true);
            foreach (var obj in canvasModes[i].GameObjectsToDisable)
                obj.SetActive(false);
            canvasModes[i].OnEnterMode?.Invoke();
        }
    
        public void ResetCanvasState()
        {
            for (int i = currentMode; i >= 0; i--)
                SwitchMode(i);
        }
    
        public void AddListener()
        {
            if (player == null)
                return;
            if (player.multiplayerEvent == null)
                return;
            if (!player.multiplayerEvent.TryGetComponent<MenuControls>(out var controls))
                return;
            controls.OnConfirmButton?.AddListener(OnClick);
            controls.OnCancelButton?.AddListener(OnCancel);
            controls.OnCancelButtonRelease?.AddListener(OnCancelRelease);

            controls.OnStartButton?.AddListener(OnStart);

            controls.OnLButton?.AddListener(OnL);
            controls.OnRButton?.AddListener(OnR);
            controls.OnBigLButton?.AddListener(OnBigL);
            controls.OnBigRButton?.AddListener(OnBigR);

            controls.OnXButton?.AddListener(OnX);
            controls.OnYButton?.AddListener(OnY);
        }
    
        public void RemoveListener()
        {
            if (player == null)
                return;
            if (player.multiplayerEvent == null)
                return;
            if (!player.multiplayerEvent.TryGetComponent<MenuControls>(out var controls))
                return;
            controls.OnConfirmButton?.RemoveListener(OnClick);
            controls.OnCancelButton?.RemoveListener(OnCancel);
            controls.OnCancelButtonRelease?.RemoveListener(OnCancelRelease);

            controls.OnStartButton?.RemoveListener(OnStart);

            controls.OnLButton?.RemoveListener(OnL);
            controls.OnRButton?.RemoveListener(OnR);
            controls.OnBigLButton?.RemoveListener(OnBigL);
            controls.OnBigRButton?.RemoveListener(OnBigR);

            controls.OnXButton?.RemoveListener(OnX);
            controls.OnYButton?.RemoveListener(OnY);
        }
    
        public void OnClick()
        {
            Invoke("OnDelayedClick", Time.fixedDeltaTime);
        }
        public void OnCancel()
        {
            if (canvasModes[currentMode].longPressCancel)
            {
                canvasModes[currentMode].holding = true;
                Invoke("OnDelayedCancelHoldStart", Time.fixedDeltaTime);
            }
            else
                Invoke("OnDelayedCancel", Time.fixedDeltaTime);
        }
        public void OnCancelRelease()
        {
            if (!canvasModes[currentMode].longPressCancel)
                return;
            Invoke("OnDelayedCancelHoldCut", Time.fixedDeltaTime);
            canvasModes[currentMode].holding = false;
            canvasModes[currentMode].cancelTimer = 0;
        }
        public void OnDelayedClick()
        {
            canvasModes[currentMode].OnConfirm?.Invoke();
        }
        public void OnDelayedCancel()
        {
            if (!UnderSlider)
                canvasModes[currentMode].OnCancel?.Invoke();
        }
        public void OnDelayedCancelHoldStart()
        {
            canvasModes[currentMode].OnCancelHoldStart?.Invoke();
        }
        public void OnDelayedCancelHoldCut()
        {
            canvasModes[currentMode].OnCancelHoldCut?.Invoke();
        }
        public void OnDelayedCancelHoldFinish()
        {
            canvasModes[currentMode].OnCancelHoldFinished?.Invoke();
        }

        public void OnStart()
        {
            Invoke("OnDelayedStart", Time.fixedDeltaTime);
        }
        public void OnDelayedStart()
        {
            canvasModes[currentMode].OnStartPress?.Invoke();
        }

        public void OnL()
        {
            Invoke("OnDelayedL", Time.fixedDeltaTime);
        }
        public void OnR()
        {
            Invoke("OnDelayedR", Time.fixedDeltaTime);
        }
        public void OnDelayedL()
        {
            canvasModes[currentMode].OnLPress?.Invoke();
        }
        public void OnDelayedR()
        {
            canvasModes[currentMode].OnRPress?.Invoke();
        }

        public void OnBigL()
        {
            Invoke("OnDelayedBigL", Time.fixedDeltaTime);
        }
        public void OnBigR()
        {
            Invoke("OnDelayedBigR", Time.fixedDeltaTime);
        }
        public void OnDelayedBigL()
        {
            canvasModes[currentMode].OnBigLPress?.Invoke();
        }
        public void OnDelayedBigR()
        {
            canvasModes[currentMode].OnBigRPress?.Invoke();
        }


        public void OnX()
        {
            Invoke("OnDelayedX", Time.fixedDeltaTime);
        }
        public void OnY()
        {
            Invoke("OnDelayedY", Time.fixedDeltaTime);
        }
        public void OnDelayedX()
        {
            canvasModes[currentMode].OnXPress?.Invoke();
        }
        public void OnDelayedY()
        {
            canvasModes[currentMode].OnYPress?.Invoke();
        }

        public void SetPlayerID(int i)
        {
            player.playerId = i;
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            if (playerRep == null || player == null)
                return;
            if (player.multiplayerEvent == null)
                return;

            foreach (var mode in canvasModes)
            {
                if (mode.holding)
                {
                    mode.cancelTimer += Time.fixedDeltaTime;
                    if (mode.cancelTimer > mode.maxTimer)
                    {
                        Invoke("OnDelayedCancel", Time.fixedDeltaTime);
                        Invoke("OnDelayedCancelHoldFinish", Time.fixedDeltaTime);
                        mode.holding = false;
                        mode.cancelTimer = 0;
                    }
                }
            }

            SetCursorParent();
            playerRep.anchoredPosition = Vector2.Lerp(playerRep.anchoredPosition, target, 0.5f);
            playerRep.localScale = Vector3.one;
            SetCursorImageAndSize();
        }
    
        private void SetCursorParent()
        {
            if (player.multiplayerEvent.currentSelectedGameObject == null)
                return;
            if (player.multiplayerEvent.currentSelectedGameObject != CurrentlySelected)
            {
                if (CurrentlySelected == null)
                    CurrentlySelected = player.multiplayerEvent.currentSelectedGameObject;
    
                else if (CurrentlySelected.TryGetComponent(out CursorTarget cursorInteractBefore))
                    cursorInteractBefore.OnDeselectEvent?.Invoke(target);
    
                playerRep.SetParent(player.multiplayerEvent.currentSelectedGameObject.transform);
                if (player.multiplayerEvent.currentSelectedGameObject.TryGetComponent(out Slider slid))
                    if (slid.transition == Selectable.Transition.None)
                        playerRep.SetParent(slid.handleRect);
                CurrentlySelected = player.multiplayerEvent.currentSelectedGameObject;
                SetCursorImageAndSize();
    
                if (player.multiplayerEvent.currentSelectedGameObject.TryGetComponent(out CursorTarget cursorInteractAfter))
                    cursorInteractAfter.OnSelectEvent?.Invoke(target);
            }
        }
        public void SetCursorParentToInit()
        {
            playerRep.SetParent(initRepParent);
            playerRep.anchoredPosition *= 0;
        }

        public void SetCursorVisible(bool val)
        {
            playerRep.gameObject.SetActive(val);
        }
    
        private void SetCursorImageAndSize()
        {
            if (player.multiplayerEvent.currentSelectedGameObject == null)
                return;
    
            if (player.multiplayerEvent.currentSelectedGameObject.TryGetComponent(out Image img))
            {
                if (img.sprite != null)
                {
                    repImage.sprite = img.sprite;
                    repImage.rectTransform.sizeDelta = img.rectTransform.sizeDelta;
                }
                else
                    repImage.rectTransform.sizeDelta *= 0;
            }
            else
                repImage.rectTransform.sizeDelta *= 0;
        }
    }
}
