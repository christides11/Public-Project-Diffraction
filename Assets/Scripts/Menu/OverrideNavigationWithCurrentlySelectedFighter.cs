namespace TightStuff.Menu
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;
    using UnityEngine.EventSystems;
    
    public class OverrideNavigationWithCurrentlySelectedFighter : MonoBehaviour
    {
        [SerializeField]
        private bool[] _navButtons;
    
        public Selectable buttonToOverride;
        private Selectable _buttonToBeOverriden;
        private ButtonNavigationExplicitBackup _buttonNavigationExplicitBackup;
    
        public bool[] NavButtons { get { return _navButtons; } set { _navButtons = value; } }
    
        // Start is called before the first frame update
        void Start()
        {
            _buttonToBeOverriden = GetComponent<Selectable>();
            _buttonNavigationExplicitBackup = GetComponent<ButtonNavigationExplicitBackup>();
        }
    
        // Update is called once per frame
        void Update()
        {
            
        }
    
        public void OverrideNavigation(BaseEventData eventData)
        {
            buttonToOverride = eventData.selectedObject.GetComponent<Selectable>();
            var temp = _buttonToBeOverriden.navigation;
            if (_navButtons[0])
            {
                temp.selectOnUp = buttonToOverride;
                if (_buttonNavigationExplicitBackup != null)
                    _buttonNavigationExplicitBackup.ogUp = buttonToOverride;
            }
            if (_navButtons[1])
            {
                temp.selectOnDown = buttonToOverride;
                if (_buttonNavigationExplicitBackup != null)
                    _buttonNavigationExplicitBackup.ogDown = buttonToOverride;
    
            }
            if (_navButtons[2])
            {
                temp.selectOnLeft = buttonToOverride;
                if (_buttonNavigationExplicitBackup != null)
                    _buttonNavigationExplicitBackup.ogLeft = buttonToOverride;
    
            }
            if (_navButtons[3])
            {
                temp.selectOnRight = buttonToOverride;
                if (_buttonNavigationExplicitBackup != null)
                    _buttonNavigationExplicitBackup.ogRight = buttonToOverride;
    
            }
            _buttonToBeOverriden.navigation = temp;
        }
    
        public void SelectCurrentFighter()
        {
            FighterSelectable charselect = buttonToOverride.GetComponent<FighterSelectable>();
            CharacterSelectManager.instance.player[charselect.playerID].multiplayerEvent.SetSelectedGameObject(buttonToOverride.gameObject);
        }
    }
}
