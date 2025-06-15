namespace TightStuff.Menu
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    
    public class P3Existence : MonoBehaviour
    {
        [SerializeField]
        private RectTransform _rectTransform;
    
        [SerializeField]
        private Vector2 _p3existencePos;
        [SerializeField]
        private Vector2 _initPos;
        // Start is called before the first frame update
    
        public void P3Mode(bool val)
        {
            if (val)
                _rectTransform.anchoredPosition = _p3existencePos;
            else
                _rectTransform.anchoredPosition = _initPos;
        }
    }
}
