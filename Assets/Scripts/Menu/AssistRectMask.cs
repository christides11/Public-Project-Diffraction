namespace TightStuff.Menu
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;
    using ModularMotion;
    
    public class AssistRectMask : MonoBehaviour
    {
        [SerializeField]
        private RectMask2D _rectMask;
    
        [SerializeField]
        private Vector4 _unexpandedVec;
        [SerializeField]
        private Vector4 _expandedVec;
        [SerializeField]
        private Vector4 _expandedP3Vec;
    
        [SerializeField]
        public Vector4 targetVec;
        [SerializeField]
        public float targetSoftness;
    
        [SerializeField]
        private UIMotion _motion;
    
        private bool expanded;
        private bool _p3Existence;
    
        // Start is called before the first frame update
        void Start()
        {
            _rectMask = GetComponent<RectMask2D>();
            targetVec = _unexpandedVec;
        }
    
        // Update is called once per frame
        void FixedUpdate()
        {
            var rect = _rectMask.padding;
            _rectMask.padding = rect;
            _rectMask.padding = Vector4.Lerp(_rectMask.padding, targetVec, 0.4f * (_p3Existence ? 2 : 1));
            var soft = _rectMask.softness;
            soft.y = (int)Mathf.Lerp(soft.y, targetSoftness, 0.4f);
            _rectMask.softness = soft;
        }
    
        public void SetTargetSoftness(float val)
        {
            if (expanded)
                targetSoftness = val;
            else
            {
                _rectMask.padding = targetVec;
                var soft = _rectMask.softness;
                soft.y = 0;
                _rectMask.softness = soft;
                targetSoftness = 0;
            }
        }
        public void SetPostSelectionExpand(bool val)
        {
            expanded = val;
            if (val)
            {
                if (!_p3Existence)
                    targetVec = _expandedVec;
                else
                    targetVec = _expandedP3Vec;
            }
            else
                targetVec = new Vector4(0, 200, 0, 200);
        }
        public void SetTargetBottomPadding(bool val)
        {
            expanded = val;
            if (!expanded)
            {
                targetVec = _unexpandedVec;
            }
            else
            {
                if (!_p3Existence)
                    targetVec = _expandedVec;
                else
                    targetVec = _expandedP3Vec;
            }
        }
        public void InstantReachTarget()
        {
            _rectMask.padding = targetVec;
            var soft = _rectMask.softness;
            soft.y = (int)targetSoftness;
            _rectMask.softness = soft;
        }
        public void ExpandPostSelection()
        {
            expanded = !expanded;
            if (!expanded)
            {
                targetVec = _unexpandedVec + new Vector4(0, 20, 0, 20);
            }
            else
            {
                if (!_p3Existence)
                    targetVec = _expandedVec;
                else
                    targetVec = _expandedP3Vec;
            }
        }
    
        public void SetP3Existence(bool val)
        {
            _p3Existence = val;
            if (!expanded)
            {
                targetVec = _unexpandedVec;
            }
            else
            {
                if (!_p3Existence)
                    targetVec = _expandedVec;
                else
                    targetVec = _expandedP3Vec;
            }
        }
    }
}
