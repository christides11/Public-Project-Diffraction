using System;
using System.Collections;
using System.Collections.Generic;
using TightStuff.Menu;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace TightStuff
{
    public class MusicItemList : MonoBehaviour
    {
        public UnityEvent OnButtonSelect;
        public UnityEvent OnButtonClick;

        public MusicPlayer player;

        public List<MusicObject> musicObjectList;
        public GameObject musicItemPrefab;

        private float _moveAmount;
        private Vector2 _idealPos;

        public float moveSpd = 0.25f;

        private RectTransform _rectTransform;
        private int _childCount;

        public RectTransform playingCD;

        public int currentSelectedChild;
        public int itemCountUntilScroll = 3;

        // Start is called before the first frame update
        void Start()
        {
            _rectTransform = GetComponent<RectTransform>();
            foreach (MusicObject obj in musicObjectList)
            {
                var item = Instantiate(musicItemPrefab).GetComponent<MusicItemDisplay>();
                item.musicObject = obj;
                item.player = player;
                item.transform.SetParent(transform, false);
            }

            foreach(Transform child in transform)
            {
                child.GetComponent<ButtonExtension>().OnSelectEvent.AddListener(OnButtonSelect.Invoke);
                child.GetComponent<ButtonExtension>().OnSelectWithObjectEvent.AddListener(SetIdealPos);
                child.GetComponent<Button>().onClick.AddListener(child.GetComponent<MusicItemDisplay>().PlayMusic);
                child.GetComponent<Button>().onClick.AddListener(SetCDParent);
                child.GetComponent<Button>().onClick.AddListener(OnButtonClick.Invoke);
                _moveAmount = child.GetComponent<RectTransform>().sizeDelta.y + GetComponent<VerticalLayoutGroup>().spacing;
                _childCount++;
            }
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            _rectTransform.anchoredPosition = Vector2.Lerp(_rectTransform.anchoredPosition, _idealPos, moveSpd);
        }

        private void SetIdealPos(BaseEventData obj)
        {
            int index = obj.selectedObject.transform.GetSiblingIndex();
            index = Mathf.Clamp(index - itemCountUntilScroll, 0, _childCount - itemCountUntilScroll * 2);
            _idealPos = new Vector2(0, index * _moveAmount);
            currentSelectedChild = obj.selectedObject.transform.GetSiblingIndex();
        }
        private void SetCDParent()
        {
            playingCD.SetParent(GetComponentsInChildren<MusicItemDisplay>()[currentSelectedChild].musicCDAnchor);
            playingCD.anchoredPosition = Vector2.zero;
        }
    }
}
