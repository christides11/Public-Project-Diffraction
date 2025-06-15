namespace TightStuff
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using Cinemachine;
    
    public class CamShaker : MonoBehaviour
    {
        private CinemachineVirtualCamera cmVC;
        private float _shakeTimer;
        private float _shakeTimerTotal;
        private float _startingIntensity;
    
        // Start is called before the first frame update
        void Start()
        {
            cmVC = GetComponent<CinemachineVirtualCamera>();
        }
    
        // Update is called once per frame
        void FixedUpdate()
        {
            if (MatchManager.paused)
            {
                SetShakeAmplitude(0);
                return;
            }
            if (_shakeTimer > 0)
            {
                _shakeTimer -= Time.fixedDeltaTime;
                _startingIntensity = Mathf.Lerp(_startingIntensity, 0f, 1 - _shakeTimer / _shakeTimerTotal);
                SetShakeAmplitude(_startingIntensity);
            }
        }
    
        public void ShakeCamera(float intensity, float time)
        {
            if (intensity < _startingIntensity || time <= 0.05f)
                return;
            SetShakeAmplitude(intensity);
    
            _startingIntensity = intensity;
            _shakeTimer = time;
    
            _shakeTimerTotal = _shakeTimer;
        }
        public void ShakeCamera(int intensity, float time)
        {
            if (intensity < _startingIntensity || time <= 0.05f)
                return;
            SetShakeAmplitude(intensity);
    
            _startingIntensity = intensity;
            _shakeTimer = time;
    
            _shakeTimerTotal = _shakeTimer;
        }
    
        private void SetShakeAmplitude(float intensity)
        {
            CinemachineBasicMultiChannelPerlin cmBMCP = cmVC.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
            cmBMCP.m_AmplitudeGain = intensity;
        }
    }
}
