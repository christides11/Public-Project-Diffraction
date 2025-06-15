namespace TightStuff.Menu
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;
    using UnityEngine.InputSystem;
    
    public class SharedButtonList : MonoBehaviour
    {
        public List<SharedButtonList> sharedLists;
        public List<Selectable> buttons;
        public List<bool> buttonSelected;
    
        public ActivatedQuardrant quadrant;
    
        // Start is called before the first frame update
        void Start()
        {
            buttonSelected = new List<bool>();
            for (int i = 0; i < buttons.Count; i++)
                buttonSelected.Add(false);
        }
    
        // Update is called once per frame
        void Update()
        {
            for (int i = 0; i < buttonSelected.Count; i++)
            {
                int selectedCount = 0;
                for (int j = 0; j < sharedLists.Count; j++)
                {
                    var a = sharedLists[j].buttons[i].GetComponent<RectTransform>();
                    a.anchoredPosition = new Vector2(0, a.anchoredPosition.y);
                    if (sharedLists[j].buttonSelected[i])
                        selectedCount++;
                }
                if (selectedCount > 1)
                {
                    var right = selectedCount % 2 == 0;
                    selectedCount--;
                    for (int j = sharedLists.Count - 1; j >= 0; j--)
                    {
                        if (sharedLists[j].buttonSelected[i])
                        {
                            var a = sharedLists[j].buttons[i].GetComponent<RectTransform>();
                            a.anchoredPosition = Vector2.right * 15 * selectedCount + Vector2.up * a.anchoredPosition.y;
                            selectedCount -= 2;
                        }
                    }
                }
            }
        }
    
        public void DisableButton(int i)
        {
            foreach (var list in sharedLists)
                list.buttons[i].gameObject.SetActive(false);
        }
        public void EnableButton(int i)
        {
            foreach (var list in sharedLists)
                list.buttons[i].gameObject.SetActive(true);
        }
    
        public void SetEnableQuadrant(bool bol)
        {
            foreach (var list in sharedLists)
                list.buttons[quadrant.currentQuardrant].gameObject.SetActive(bol);
        }
    
        public void ActivateQuardrantParent(PlayerInput player)
        {
            foreach (var list in sharedLists)
                list.buttons[player.playerIndex].gameObject.SetActive(true);
        }
        public void SelectButton(int i)
        {
            buttonSelected[i] = true;
        }
        public void DeselectButton(int i)
        {
            buttonSelected[i] = false;
        }
    }
}
