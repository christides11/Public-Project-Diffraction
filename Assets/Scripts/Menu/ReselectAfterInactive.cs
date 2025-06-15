namespace TightStuff.Menu
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.EventSystems;
    using UnityEngine.UI;
    
    public class ReselectAfterInactive : MonoBehaviour, ISelectHandler
    {
        public PlayerControlledCanvas root;
        public Selectable button;
        public List<Selectable> backups = new List<Selectable>();

        public UnityEvent<BaseEventData> OnSelection;
    
        // Start is called before the first frame update
        void Start()
        {
            button = GetComponent<Selectable>();
        }
    
        // Update is called once per frame
        void OnDisable()
        {
            SelectOthersIfInactive();
        }

        public void SelectOthersIfInactive()
        {
            Debug.Log("oh");
            if (root.player == null || root.player.multiplayerEvent == null)
                return;
            if (root.player.multiplayerEvent.currentSelectedGameObject == null)
                return;
            if (!root.player.multiplayerEvent.currentSelectedGameObject.Equals(gameObject))
                return;
            if (backups == null || backups.Count == 0)
                return;
            if (root == null)
                return;
            foreach (Selectable go in backups)
                if (go.interactable && go.isActiveAndEnabled && go.gameObject.activeInHierarchy)
                {
                    root.Select(go.gameObject);
                    return;
                }
        }

        public void OnSelect(BaseEventData eventData)
        {
            if (!gameObject.activeInHierarchy)
                OnSelection?.Invoke(eventData);
        }
    }
}
