namespace TightStuff.Menu
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.UI;
    
    public class PlayerSelectAestheticEvents : MonoBehaviour
    {
        public UnityEvent OnCharacterSelect;
        public UnityEvent OnFullReady;
        public UnityEvent OnUnready;
        public UnityEvent<bool> OnSelectAssist;
        public UnityEvent OnConfirmAssist;
    
        public void OnCharacterChangeSelection()
        {
            OnCharacterSelect?.Invoke();
        }
    
        public void OnReady(bool slider, int mode)
        {
            if (!slider && mode == 3)
                OnFullReady?.Invoke();
            else
                OnUnready?.Invoke();
        }
        public void SelectAssist(bool slider)
        {
            OnSelectAssist?.Invoke(slider);
        }
        public void ConfirmAssist()
        {
            OnConfirmAssist?.Invoke();
        }
    
        // Start is called before the first frame update
        void Start()
        {
            
        }
    
        // Update is called once per frame
        void Update()
        {
            
        }
    }
}
