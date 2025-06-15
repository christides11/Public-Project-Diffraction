namespace TightStuff.Menu
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.EventSystems;
    using UnityEngine.InputSystem;
    using UnityEngine.InputSystem.UI;
    using UnityEngine.UI;
    
    public class SharedButton : MonoBehaviour, ISelectHandler, IDeselectHandler
    {
        private MultiplayerEventSystem _selectedBy;
        [SerializeField]
        private List<Selectable> _backupButtons;
    
        // Start is called before the first frame update
        void Start()
        {
            
        }
    
        // Update is called once per frame
        void Update()
        {
    
        }
    
        private void OnDisable()
        {
            if (_selectedBy != null)
                foreach (Selectable p in _backupButtons)
                    if (p.gameObject.activeInHierarchy)
                    {
                        _selectedBy.SetSelectedGameObject(p.gameObject);
                        _selectedBy = null;
                        return;
                    }
        }
    
        public void OnSelect(BaseEventData eventData)
        {
            _selectedBy = eventData.currentInputModule.GetComponent<MultiplayerEventSystem>();
        }
        public void OnDeselect(BaseEventData eventData)
        {
            _selectedBy = null;
        }
    }
}
