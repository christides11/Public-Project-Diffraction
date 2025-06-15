namespace TightStuff.Stage
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    
    public class Wind : MonoBehaviour
    {
        private CircleCollider2D cirCol;
    
        [SerializeField]
        private float _maxSize = 20f;
        [SerializeField]
        private float _initSize = 2f;
    
        [SerializeField]
        private float _decaySpd = 0.01f;
        [SerializeField]
        private float _inflateSpd = 0.5f;
        [SerializeField]
        private float _initStrength = 10f;
        [SerializeField]
        private float _initFrequency = 5f;
    
        private float _currentStrength;
        private float _currentFrequency;
    
        // Start is called before the first frame update
        void Awake()
        {
            cirCol = GetComponent<CircleCollider2D>();
        }
    
        // Update is called once per frame
        void FixedUpdate()
        {
            if (MatchManager.paused)
                return;
            transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, _currentStrength);
            transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, _currentFrequency);
    
            cirCol.radius += _inflateSpd * MatchManager.worldTime;
            _currentStrength -= _decaySpd * MatchManager.worldTime;
    
            if (cirCol.radius > _maxSize)
                gameObject.SetActive(false);
        }
    
        private void OnEnable()
        {
            _currentStrength = _initStrength;
            _currentFrequency = _initFrequency;
            cirCol.radius = _initSize;
        }
    }
}
