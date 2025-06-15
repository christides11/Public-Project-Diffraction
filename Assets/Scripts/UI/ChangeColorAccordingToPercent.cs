namespace TightStuff.UI
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;
    
    public class ChangeColorAccordingToPercent : MonoBehaviour
    {
        [SerializeField]
        private List<Text> _percents;
    
        [SerializeField]
        private RectTransform _rect;
    
        private Vector2 _initPos;
    
        [SerializeField]
        private Color _mediumColor;
        [SerializeField]
        private Color _highColor;
    
        [SerializeField]
        private float _mediumPercent = 65;
        [SerializeField]
        private float _highPercent = 130;
    
        private void Start()
        {
            _rect = GetComponent<RectTransform>();
            _initPos = _rect.anchoredPosition;
        }
    
        public void ChangePercentColor(float fighterPercent, bool alive)
        {
            foreach (var percent in _percents)
            {
                if (!alive)
                    _rect.anchoredPosition = new Vector2(_rect.anchoredPosition.x, -50);
                else
                    _rect.anchoredPosition = Vector2.Lerp(_rect.anchoredPosition, _initPos, Time.deltaTime * 5f);
    
                if (fighterPercent < _mediumPercent)
                    percent.color = _mediumColor * (1 - (_mediumPercent - fighterPercent) / _mediumPercent) + Color.white * ((_mediumPercent - fighterPercent) / _mediumPercent);
                else if (fighterPercent < _highPercent)
                    percent.color = _highColor * (1 - (_highPercent - fighterPercent) / (_highPercent - _mediumPercent)) + _mediumColor * ((_highPercent - fighterPercent) / (_highPercent - _mediumPercent));
                else
                    percent.color = _highColor;
            }
        }
    }
}
