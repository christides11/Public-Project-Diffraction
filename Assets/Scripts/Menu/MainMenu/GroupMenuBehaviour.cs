using Newtonsoft.Json.Bson;
using System;
using System.Collections;
using System.Collections.Generic;
using TightStuff.Menu;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace TightStuff.Menu
{
    public class GroupMenuBehaviour : MonoBehaviour
    {
        public enum State {Active, Disabled, Peek};

        public MenuStateChangeEvent OnChangedState;

        public UnityEvent OnActive;
        public UnityEvent OnDisabled;
        public UnityEvent OnPeek;

        public UnityEvent OnCancel;
        public UnityEvent OnCancelHoldComplete;
        public UnityEvent OnCancelRelease;
        public UnityEvent OnL;
        public UnityEvent OnR;
        public UnityEvent OnBigL;
        public UnityEvent OnBigR;
        public UnityEvent OnX;
        public UnityEvent OnY;

        public Button CurrentlySelected;
        public bool cancelHolding;
        public float timer;

        public List<Button> SubMenuButtons;

        [SerializeField]
        private State _currentState;
        public State CurrentState { get { return _currentState; } set { OnChangedState?.Invoke(value); _currentState = value; } }

        private MenuControls _menuControls;

        // Start is called before the first frame update
        void Awake()
        {
            SubMenuButtons = new List<Button>(GetComponentsInChildren<Button>());
            _menuControls = FindObjectOfType<MenuControls>();
            OnChangedState?.AddListener(StateChanged);
            if (CurrentState == State.Active) AddListeners();
        }

        private void AddListeners()
        {
            _menuControls.OnCancelButton.AddListener(OnCancelEvent);
            _menuControls.OnCancelButtonRelease.AddListener(OnCancelReleaseEvent);
            _menuControls.OnLButton.AddListener(OnLEvent);
            _menuControls.OnRButton.AddListener(OnREvent);
            _menuControls.OnXButton.AddListener(OnXEvent);
            _menuControls.OnYButton.AddListener(OnYEvent);
            _menuControls.OnBigLButton.AddListener(OnBigLEvent);
            _menuControls.OnBigRButton.AddListener(OnBigREvent);
        }
        private void RemoveListeners()
        {
            _menuControls.OnCancelButton.RemoveListener(OnCancelEvent);
            _menuControls.OnCancelButtonRelease.RemoveListener(OnCancelReleaseEvent);
            _menuControls.OnLButton.RemoveListener(OnLEvent);
            _menuControls.OnRButton.RemoveListener(OnREvent);
            _menuControls.OnXButton.RemoveListener(OnXEvent);
            _menuControls.OnYButton.RemoveListener(OnYEvent);
            _menuControls.OnBigLButton.RemoveListener(OnBigLEvent);
            _menuControls.OnBigRButton.RemoveListener(OnBigREvent);
        }

        private void OnDestroy()
        {
            OnChangedState?.RemoveAllListeners();
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            if (cancelHolding)
            {
                timer += Time.deltaTime;
                if (timer >= 1)
                {
                    OnCancelHoldComplete?.Invoke();
                    cancelHolding = false;
                    timer = 0;
                }
            }
        }

        public void StateChanged(State state)
        {
            switch (state)
            {
                case State.Active:
                    OnActive.Invoke(); 
                    AddListeners();
                    break;

                case State.Disabled:
                    OnDisabled.Invoke();
                    RemoveListeners();
                    break;

                default: 
                    OnPeek.Invoke();
                    RemoveListeners();
                    break;
            }
        }

        public void HideAllItems()
        {
            foreach (var button in SubMenuButtons)
            {
                if (button != CurrentlySelected)
                {
                    if (button.animator != null)
                        button.animator.SetTrigger("Hide");
                }
            }
        }
        public void UnhideAllItems()
        {
            foreach (var button in SubMenuButtons)
            {
                if (button.animator != null)
                    button.animator.SetTrigger("Normal");
            }
        }

        public void DisableState()
        {
            CurrentState = State.Disabled;
        }
        public void ActivateState()
        {
            CurrentState = State.Active;
        }
        public void PeekState()
        {
            CurrentState = State.Peek;
        }

        public void OnCancelEvent()
        {
            OnCancel?.Invoke();
            cancelHolding = true;
        }
        public void OnCancelReleaseEvent()
        {
            OnCancelRelease?.Invoke();
            cancelHolding = false;
            timer = 0;
        }
        public void OnLEvent()
        {
            OnL?.Invoke();
            Debug.Log("L Pressed");
        }
        public void OnREvent()
        {
            OnR?.Invoke();
        }
        public void OnBigLEvent()
        {
            OnBigL?.Invoke();
        }
        public void OnBigREvent()
        {
            OnBigR?.Invoke();
        }
        public void OnXEvent()
        {
            Debug.Log("X");
            OnX?.Invoke();
        }
        public void OnYEvent()
        {
            OnY?.Invoke();
        }

        public void SetCurrentlySelected(GameObject obj)
        {
            CurrentlySelected = obj.GetComponent<Button>();
        }

        public void SelectCurrentlySelected()
        {
            EventSystem.current.SetSelectedGameObject(CurrentlySelected.gameObject);
        }

        [System.Serializable]
        public class MenuStateChangeEvent : UnityEvent<State> { }
    }
}
