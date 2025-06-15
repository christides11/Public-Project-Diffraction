namespace TightStuff.Menu
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.EventSystems;
    using UnityEngine.Localization;
    using UnityEngine.Localization.Components;
    using UnityEngine.UI;
    
    public class ButtonExtension : MonoBehaviour, ISelectHandler, IDeselectHandler
    {
        public UnityEvent OnCancelEvent;
        public UnityEvent OnDeselectEvent;
        public UnityEvent OnSelectEvent;

        public UnityEvent<BaseEventData> OnSelectWithObjectEvent;
        public string descriptionText {  get; set; }
        private LocalizeStringEvent localization;

        // Start is called before the first frame update
        void OnEnable()
        {
            localization = GetComponent<LocalizeStringEvent>();
            if (localization != null )
            {
                localization.OnUpdateString.AddListener( (string value) =>{ descriptionText = value; });

                OnSelectEvent.AddListener(ChangeDescriptionText);
            }
        }
    
        // Update is called once per frame
        void Update()
        {
    
        }
        public void ChangeDescriptionText()
        {
            Description.instance.descriptionText.text = descriptionText;
        }
        public void OnSelect(BaseEventData eventData)
        {
            eventData.currentInputModule?.GetComponent<MenuControls>()?.OnCancelButton.AddListener(OnCancel);
            OnSelectEvent?.Invoke();
            OnSelectWithObjectEvent?.Invoke(eventData);
        }
        public void OnDeselect(BaseEventData eventData)
        {
            eventData.currentInputModule?.GetComponent<MenuControls>()?.OnCancelButton.RemoveListener(OnCancel);
            OnDeselectEvent?.Invoke();
        }
        public void OnCancel()
        {
            Invoke("delayedCancel", Time.fixedDeltaTime);
        }
        public void delayedCancel()
        {
            OnCancelEvent?.Invoke();
        }
    }
}
