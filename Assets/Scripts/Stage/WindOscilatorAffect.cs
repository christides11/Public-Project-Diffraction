namespace TightStuff.Stage
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    
    public class WindOscilatorAffect : UpdateAbstract
    {
        public IBlowable blow;
    
        [SerializeField]
        private float _blownTime = 120;
        [SerializeField]
        private float _windScale = 1;
    
        private float _recentWindAmplitudeScale;
        private float _recentWindFrequencyScale;
    
        private float _addedAmplitude;
        private float _addedRate;
        private float _timer;
    
        private void OnTriggerEnter2D(Collider2D collision)
        {
            var windStrength = Mathf.Clamp(Mathf.Round(collision.transform.localPosition.z * _windScale), 0, Mathf.Infinity);
            var windFrequency = Mathf.Clamp(Mathf.Round(collision.transform.localScale.z * _windScale), 0, Mathf.Infinity);
    
            if (collision.gameObject.layer == 20 && _timer <= 00)
            {
                _recentWindAmplitudeScale = windStrength;
                _recentWindFrequencyScale = windFrequency;
    
                windStrength *= Time.fixedDeltaTime * _blownTime;
                windFrequency *= Time.fixedDeltaTime * _blownTime;
    
                _addedAmplitude = windStrength;
                _addedRate = windFrequency;
                _timer = _blownTime;
            }
        }
    
        // Start is called before the first frame update
        void Start()
        {
            blow = GetComponent<IBlowable>();
        }
    
        // Update is called once per frame
        public override void GUpdate()
        {
            if (MatchManager.paused)
                return;
            if (_timer <= 0)
            {
                _addedAmplitude = -100;
                _addedRate = -100;
                blow.Blown(_addedAmplitude, _addedRate);
                return;
            }
    
            blow.Blown(_addedAmplitude, _addedRate);
    
            _addedAmplitude = -Time.fixedDeltaTime * _recentWindAmplitudeScale * MatchManager.worldTime;
            _addedRate = -Time.fixedDeltaTime * _recentWindFrequencyScale * MatchManager.worldTime;
            _timer--;
        }
    }
}
