namespace TightStuff.Menu
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.EventSystems;
    using UnityEngine.UI;
    
    public class ReselectAfterInactiveNormal : MonoBehaviour
    {
        public EventSystem root;
        public Selectable button;
        public List<Selectable> backups = new List<Selectable>();

        public UnityEvent<BaseEventData> OnSelection;
    
        // Start is called before the first frame update
        void Start()
        {
            button = GetComponent<Selectable>();
            root = FindObjectOfType<EventSystem>();
        }
    
        // Update is called once per frame
        void OnDisable()
        {
            SelectOthersIfInactive();
        }

        public void SelectOthersIfInactive()
        {
            Debug.Log("oh");
            if (root == null)
                return;
            if (root.currentSelectedGameObject == null)
                return;
            if (!root.currentSelectedGameObject.Equals(gameObject))
                return;

            foreach (Selectable go in backups)
                if (go.interactable && go.isActiveAndEnabled)
                {
                    root.SetSelectedGameObject(go.gameObject);
                    return;
                }
        }
    }
}
