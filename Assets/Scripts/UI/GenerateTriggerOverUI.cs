namespace TightStuff.UI
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    
    public class GenerateTriggerOverUI : MonoBehaviour
    {
        private RectTransform _rect;
        private Transform _trigger;
        private Camera cam;
    
        [SerializeField]
        public GameObject _triggerObj;
    
        [SerializeField]
        private Vector2 _colSize;
    
        private void Start()
        {
            cam = Camera.main;
            _rect = GetComponent<RectTransform>();
            if (_triggerObj == null)
                return;
            _trigger = _triggerObj.transform;
            _trigger.position = cam.ScreenToWorldPoint(_rect.transform.position);
            _trigger.position = new Vector3(_trigger.position.x, _trigger.position.y, 0);
            var col = _trigger.gameObject.AddComponent<BoxCollider2D>();
    
            col.isTrigger = true;
            col.size = _colSize;
            col.transform.localScale = new Vector3(col.transform.localScale.x / col.transform.lossyScale.x, col.transform.localScale.y / col.transform.lossyScale.y, 1);
        }
    
        private void FixedUpdate()
        {
            if (_trigger == null)
                return;
            _trigger.position = cam.ScreenToWorldPoint(_rect.transform.position);
            _trigger.position = new Vector3(_trigger.position.x, _trigger.position.y, 0);
        }
    }
}
