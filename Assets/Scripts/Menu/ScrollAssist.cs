namespace TightStuff.Menu
{
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;
    
    public class ScrollAssist : MonoBehaviour
    {
        [SerializeField]
        private RectTransform _rect;
    
        public int currentValue;
        [SerializeField]
        private List<AssistProperties> _assistProperties;
        [SerializeField]
        private GameObject _assistItemPrefab;
        private List<AssistItem> _assistItems;
    
        [SerializeField]
        private AssistItem _chosenAssistItems;
        public AssistItem ChosenAssist => _chosenAssistItems;
    
        [SerializeField]
        private LerpToColor _background;
    
        [SerializeField]
        private int _id;
    
        public int currentSelection;
        public int maxValue;
        public bool selected;
    
        // Start is called before the first frame update
        void Start()
        {
            _rect = GetComponent<RectTransform>();
            _assistItems = new List<AssistItem>();
            _assistProperties = FighterAssistList.instance.AssistProperties;
            SpawnAssistItems();
            SpawnAssistItems();
            SpawnAssistItems();
    
        }
    
        private void SpawnAssistItems()
        {
            for (int i = _assistProperties.Count - 1; i >= 0; i--)
            {
                AssistProperties assist = _assistProperties[i];
                var a = Instantiate(_assistItemPrefab).GetComponent<AssistItem>();
                a.assist = assist;
                a.transform.SetParent(_rect);
                a.GetComponent<RectTransform>().anchoredPosition = Vector3.zero;
                a.GetComponent<RectTransform>().localScale = Vector3.one;
                _assistItems.Add(a);
            }
        }
    
        // Update is called once per frame
        void FixedUpdate()
        {
            if (!selected)
            {
                _assistItems[(_assistProperties.Count - currentSelection + 1) + _assistProperties.Count - 1].ChangeUIVisibility(1);
                _assistItems[(_assistProperties.Count - currentSelection + 1) + _assistProperties.Count - 1].HighlightItem();
            }
            _rect.anchoredPosition = Vector3.Lerp(_rect.anchoredPosition, new Vector3(_rect.anchoredPosition.x, 13 + 37 * (currentSelection + _assistProperties.Count - 1), 0), 0.4f);
        }
    
        public void ChangeCurrentSelection(int val)
        {
            _assistItems[(_assistProperties.Count - currentSelection + 1) + _assistProperties.Count - 1].ChangeUIVisibility(0.5f);
            _assistItems[(_assistProperties.Count - currentSelection + 1) + _assistProperties.Count - 1].UnhighlightItem();
            currentSelection = val;
    
            if (val <= 0)
            {
                currentSelection = _assistProperties.Count;
                _rect.anchoredPosition = new Vector3(_rect.anchoredPosition.x, 13 + 37 * (_assistProperties.Count * 2));
            }
            if (val >= maxValue)
            {
                currentSelection = 1;
                _rect.anchoredPosition = new Vector3(_rect.anchoredPosition.x, 13 + 37 * (_assistProperties.Count - 1));
            }
        }
        public void SelectItem()
        {
            _chosenAssistItems.assist = _assistItems[(_assistProperties.Count - currentSelection + 1) + _assistProperties.Count - 1].assist;
            _chosenAssistItems.ChangeUI();
            var a = Color.black;
            a.a = 0.6f;
            _background.SetTargetColor(a);
            _background.SetCurrentColor(Color.white);
            _rect.anchoredPosition = new Vector3(_rect.anchoredPosition.x, 13 + 37 * (currentSelection + _assistProperties.Count - 1));
        }
        public void DeselectItem()
        {
            _chosenAssistItems.assist = null;
            _background.SetTargetColor(new Color());
            _chosenAssistItems.ChangeUI();
            _rect.anchoredPosition = new Vector3(_rect.anchoredPosition.x, 13 + 37 * (currentSelection + _assistProperties.Count - 1));
        }
    }
}
