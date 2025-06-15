namespace TightStuff.Menu
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.EventSystems;
    using UnityEngine.Localization.Components;
    using UnityEngine.UI;
    
    public class SliderControl : MonoBehaviour, ISelectHandler, IDeselectHandler
    {
        public UnityEvent OnClickEvent;
        public UnityEvent<float> OnClickWithValueEvent;
        public UnityEvent OnCancelEvent;
        public UnityEvent OnSelectEvent;
        public UnityEvent OnDeselectEvent;

        public Text textField;
    
        [SerializeField]
        private Slider _slider;
        private float _ogVal;

        [SerializeField]
        private bool _ignoreOgVal;

        [SerializeField]
        private PlayerControlledCanvas _root;

        public string descriptionText { get; set; }
        private LocalizeStringEvent localization;

        // Start is called before the first frame update
        void OnEnable()
        {
            _slider = GetComponent<Slider>();
            _ogVal = _slider.value;
            localization = GetComponent<LocalizeStringEvent>();
            if (localization != null)
            {
                localization.OnUpdateString.AddListener((string value) => { descriptionText = value; });
                OnSelectEvent.AddListener(ChangeDescriptionText);
            }
        }
        public void ChangeDescriptionText()
        {
            Description.instance.descriptionText.text = descriptionText;
        }

        // Update is called once per frame
        void Update()
        {
    
        }
    
        public void OnSelect(BaseEventData eventData)
        {
            MenuControls menuControls = eventData.currentInputModule.GetComponent<MenuControls>();
            if (menuControls == null)
            {
                menuControls = FindObjectOfType<MenuControls>();
            }

            if (menuControls != null)
            {
                menuControls.OnConfirmButton.AddListener(OnClick);
                menuControls.OnCancelButton.AddListener(OnCancel);
            }
            OnSelectEvent?.Invoke();
        }
        public void OnDeselect(BaseEventData eventData)
        {
            MenuControls menuControls = eventData.currentInputModule.GetComponent<MenuControls>();
            if (menuControls == null)
            {
                menuControls = FindObjectOfType<MenuControls>();
            }
            menuControls.OnConfirmButton.RemoveListener(OnClick);
            menuControls.OnCancelButton.RemoveListener(OnCancel);
            OnDeselectEvent?.Invoke();
        }
        public void ResetOGVal()
        {
            if (_slider == null)
                _slider = GetComponent<Slider>();
            _ogVal = _slider.value;
        }
        public void OnClick()
        {
            OnClickWithValueEvent?.Invoke(_slider.value);
            _ogVal = _slider.value;
            Invoke("delayedClick", Time.fixedDeltaTime);
        }
        public void OnCancel()
        {
            if (!_ignoreOgVal)
                _slider.value = _ogVal;
            Invoke("delayedCancel", Time.fixedDeltaTime);
        }
        public void delayedClick()
        {
            OnClickEvent?.Invoke();
        }
        public void delayedCancel()
        {
            OnCancelEvent?.Invoke();
        }
        public void OnLoop(float val)
        {
            if (val <= 0)
                _slider.value = _slider.maxValue - 1;
            if (val >= _slider.maxValue)
                _slider.value = 1;
        }
        public void ForceRemoveListeners()
        {
            if (_root != null)
            {
                _root.player.multiplayerEvent.GetComponent<MenuControls>().OnConfirmButton?.RemoveListener(OnClick);
                _root.player.multiplayerEvent.GetComponent<MenuControls>().OnCancelButton?.RemoveListener(OnCancel);
            }
            else
            {
                var menuControls = FindObjectOfType<MenuControls>();
                menuControls.OnConfirmButton?.RemoveListener(OnClick);
                menuControls.OnCancelButton?.RemoveListener(OnCancel);
            }
        }
    
        public void SelectButton(GameObject obj)
        {
            _root.player.multiplayerEvent.SetSelectedGameObject(obj);
        }

        public void AddValue(int val)
        {
            _slider.value += val;
        }

        public void SetText()
        {
            if (textField != null)
            {
                textField.text = _slider.value.ToString();
            }
        }
    }
}
