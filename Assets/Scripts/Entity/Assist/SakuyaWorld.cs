namespace TightStuff
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Rendering;
    using UnityEngine.Rendering.Universal;
    using UnityEngine.UI;
    
    public class SakuyaWorld : UpdateAbstract
    {
        private Volume _volume;
        private BaseProjectileBehaviour _proj;
        private RawImage _playerOnlyCamera;
    
        private ColorAdjustments _bwColor;
    
        private void Start()
        {
            BaseFighterBehavior.SpellcardTimeStopEvent += CancelEffect;
            _proj = GetComponent<BaseProjectileBehaviour>();
            _playerOnlyCamera = GameObject.FindGameObjectWithTag("PlayerOnly").GetComponent<RawImage>();
            _volume = FindObjectOfType<Volume>();
            _volume.profile.TryGet(out _bwColor);
        }
    
        private void OnDestroy()
        {
            BaseFighterBehavior.SpellcardTimeStopEvent -= CancelEffect;
        }
    
        private void SetWorldTime(float time)
        {
            _bwColor.saturation.Override(-100);
    
            MatchManager.worldTime = time;
            _proj.entity.stateVars.givenTime = 1 - time;
            _proj.controlling.GetComponent<Entity>().stateVars.givenTime = 1 - time;
            if (time == 0)
            {
                _bwColor.saturation.Override(-100);
                _playerOnlyCamera.enabled = true;
            }
            else
            {
                _bwColor.saturation.Override(0);
                _playerOnlyCamera.enabled = false;
            }
        }
        private void CancelEffect()
        {
            if (!_proj.entity.stateVars.enabled)
                return;
            if (_proj.controlling == null)
                return;
    
            //_proj.controlling.GetComponent<Entity>().stateVars.givenTime = 0;
            _proj.entity.stateVars.givenTime = 0;
            _proj.entity.stateVars.frameNum = 0;
            _proj.entity.SetEntityActive(false);
            _bwColor.saturation.Override(0);
            _playerOnlyCamera.enabled = false;
        }
    }
}
