namespace TightStuff.Menu
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Events;
    
    public class KeyRebindingEvents : MonoBehaviour
    {
        public UnityEvent OnCancelEvent;
        public UnityEvent OnSelectEvent;
        public UnityEvent OnSelectActionEvent;
        public UnityEvent OnDeselectActionEvent;
        public UnityEvent OnRebindStartEvent;
        public UnityEvent OnRebindStopEvent;
    
        public UnityEvent<GameObject> SelectEvent;
    
        public void OnCancel()
        {
            OnCancelEvent?.Invoke();
        }
        public void OnSelect()
        {
            OnSelectEvent?.Invoke();
        }
        public void OnSelectAction()
        {
            OnSelectActionEvent?.Invoke();
        }
        public void OnDeselectAction()
        {
            OnDeselectActionEvent?.Invoke();
        }
        public void OnRebindStart()
        {
            OnRebindStartEvent?.Invoke();
        }
        public void OnRebindStop()
        {
            OnRebindStopEvent?.Invoke();
        }
    
        public void Select(GameObject obj)
        {
            SelectEvent?.Invoke(obj);
        }
    }
}
