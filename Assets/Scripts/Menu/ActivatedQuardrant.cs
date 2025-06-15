namespace TightStuff.Menu
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Events;
    
    public class ActivatedQuardrant : MonoBehaviour
    {
        public PlayerControlledCanvas canvas;
        public RectTransform fighterSlider;
    
        public List<Quardrant> quardrants = new List<Quardrant>();
        public int currentQuardrant;
    
        private Vector3 initFighterPos;
    
        [System.Serializable]
        public class Quardrant
        {
            public string name;
            public GameObject parent;
            public GameObject assistSlider;
            public IntEvent OnSwitchMode;
            public UnityEvent<bool, int> OnUnderSliderMode;
            public List<GameObject> readyItems = new List<GameObject>();
            public List<GameObject> unreadyItems = new List<GameObject>();
    
            [System.Serializable]
            public class IntEvent : UnityEvent<int>
            {
            }
        }
    
        public void OnSwitchMode()
        {
            quardrants[currentQuardrant].OnSwitchMode?.Invoke(canvas.currentMode);
            fighterSlider.anchoredPosition = initFighterPos;
            if (currentQuardrant == 0 || currentQuardrant == 2)
                fighterSlider.anchoredPosition += Vector2.left * 15;
            else
                fighterSlider.anchoredPosition += Vector2.right * 15;
            if (currentQuardrant == 0 || currentQuardrant == 1)
                fighterSlider.anchoredPosition += Vector2.up * 12;
            else
                fighterSlider.anchoredPosition += Vector2.down * 24;
        }
        public void OnUnderSlider(bool val, int mode)
        {
            quardrants[currentQuardrant].OnUnderSliderMode?.Invoke(val, mode);
        }
    
        public void SetCurrentQuardrant(int i)
        {
            currentQuardrant = i;
        }
    
        public void EnableQuardant(bool bol)
        {
            quardrants[currentQuardrant].parent.SetActive(bol);
        }
        public void ReadyQuardant(bool bol)
        {
            foreach (var item in quardrants[currentQuardrant].readyItems)
                item.SetActive(bol);
            foreach (var item in quardrants[currentQuardrant].unreadyItems)
                item.SetActive(!bol);
        }
        public void SetAssistSlider()
        {
            canvas.Select(quardrants[currentQuardrant].assistSlider);
        }
        public void SelectQuardrantReadyItem(int i)
        {
            canvas.Select(quardrants[currentQuardrant].readyItems[i]);
        }
        public void SelectFirstItemQuardrant(int i)
        {
            if (canvas.currentMode == 3)
                canvas.Select(quardrants[currentQuardrant].readyItems[i]);
            else
                canvas.Select(canvas.canvasModes[canvas.currentMode].first);
        }
    
        private void Start()
        {
            canvas = GetComponent<PlayerControlledCanvas>();
            initFighterPos = fighterSlider.anchoredPosition;
        }
    }
}
