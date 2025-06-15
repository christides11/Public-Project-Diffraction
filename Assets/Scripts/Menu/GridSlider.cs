using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace TightStuff.Menu
{
    public class GridSlider : MonoBehaviour
    {
        public Slider slider;

        public float desiredSliderValue;

        public float slideSpd = 0.2f;

        public int row;
        public int col;

        public RectTransform content;
        public int leftExceedRequirements;
        public int rightExceedRequirements;

        private Vector2 _startPoint;
        private Vector2 _startEdge;
        private Vector2 _endEdge;
        private Vector2 _step;

        // Start is called before the first frame update
        void Start()
        {
            _startPoint = content.anchoredPosition;
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            slider.value = Mathf.Lerp(slider.value, desiredSliderValue, slideSpd);
            var progress = (slider.value - slider.minValue) / (slider.maxValue - slider.minValue);

            var offset = _startEdge - _startPoint;
            var endPoint = _endEdge + offset;
            //content.anchoredPosition = _startPoint - endPoint * progress;
            content.anchoredPosition = _startPoint - _step * slider.value * col;
            content.anchoredPosition = new Vector2(content.anchoredPosition.x, _startPoint.y);
        }

        public void UpdateSliderMax()
        {
            slider.maxValue = Mathf.FloorToInt((float)Mathf.CeilToInt((float)content.childCount / row) / col) - 1;
            StartCoroutine(SetEndEdges());
        }

        private IEnumerator SetEndEdges()
        {
            yield return new WaitForEndOfFrame();
            if (content.childCount != 0 && content.childCount > row)
            {
                _startEdge = content.GetChild(0).GetComponent<RectTransform>().localPosition;
                _endEdge = content.GetChild(content.childCount - 1).GetComponent<RectTransform>().localPosition;
                _step = content.GetChild(row).GetComponent<RectTransform>().localPosition - new Vector3(_startEdge.x, _startEdge.y);
            }
            yield return null;
        }

        public void OnOutsideView(BaseEventData eventData)
        {
            var currentlySelectedColumn = eventData.selectedObject.GetComponent<IGridViewable>().ViewID();
            currentlySelectedColumn = Mathf.FloorToInt((float)currentlySelectedColumn / row);
            Debug.Log(currentlySelectedColumn);

            int val = Mathf.FloorToInt(((float)currentlySelectedColumn) / col);
            if (currentlySelectedColumn - desiredSliderValue * col < leftExceedRequirements || currentlySelectedColumn - desiredSliderValue * col > rightExceedRequirements)
                desiredSliderValue = val;
        }
    }
}
