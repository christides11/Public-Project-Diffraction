namespace TightStuff
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    
    public class CPUControl : MonoBehaviour
    {
        [SerializeField]
        private MatchManager _match;
    
        [SerializeField]
        private int _controlID;
    
        [SerializeField]
        private bool _DILeft;
        [SerializeField]
        private bool _noDI;
        [SerializeField]
        private Transform _opponent;
        private float timer;
    
        private BaseFighterBehavior _fighter;
    
        // Start is called before the first frame update
        void Start()
        {
            _fighter = _match.fighters[_controlID];
        }
    
        // Update is called once per frame
        void Update()
        {
            timer -= Time.deltaTime;
            _fighter.controlling.moveIntXRaw = (int)(_match.fighters[0].controlling.moveStick.raw.y * 1000);
            _fighter.controlling.moveIntYRaw = (int)(_match.fighters[0].controlling.moveStick.raw.x * 1000);
            _fighter.controlling.jumpButtonRaw = _match.fighters[0].controlling.jumpButton.raw;
            _fighter.controlling.shieldButtonRaw = _match.fighters[0].controlling.shieldButton.raw;
            _fighter.controlling.moveStickRaw = _match.fighters[0].controlling.moveStick.raw;
    
            return;
            if ((_fighter.ActionState.GetState() is AirborneFighterHurtHeavyState || _fighter.ActionState.GetState() is AirborneFighterHurtNormalState) || _fighter.ActionState.GetState() is AirborneFighterHurtFreezeState || _fighter.ActionState.GetState() is GroundedFighterHurtFreezeState || _fighter.ActionState.GetState() is GroundedFighterHurtState || _fighter.ActionState.GetState() is GroundedFighterHurtMiniState)
            {
                if (!_noDI)
                    _fighter.controlling.moveIntXRaw = _DILeft? -1000 : 1000;
                _fighter.controlling.moveStickRaw = new Vector2((float)_fighter.controlling.moveIntX / 1000f, (float)_fighter.controlling.moveIntY / 1000f);
            }
            else
            {
                _fighter.controlling.moveIntXRaw = 0;
                _fighter.controlling.moveStickRaw = new Vector2((float)_fighter.controlling.moveIntX / 1000f, (float)_fighter.controlling.moveIntY / 1000f);
            }
            if ((_fighter.ActionState.GetState() is AirborneFighterHurtHeavyState || _fighter.ActionState.GetState() is AirborneFighterHurtNormalState || _fighter.ActionState.GetState() is AirborneFighterHurtFreezeState || _fighter.ActionState.GetState() is GroundedFighterHurtFreezeState || _fighter.ActionState.GetState() is GroundedFighterHurtState || _fighter.ActionState.GetState() is GroundedFighterHurtMiniState) && _fighter.stateVarsF.stunStopTime < 0.1f)
            {
                if (_fighter.entity.stateVars.aerial)
                {
                    _fighter.controlling.shieldButtonRaw = false;
                    _fighter.controlling.jumpButtonRaw = true;
                }
                else
                {
                    _fighter.controlling.jumpButtonRaw = false;
                    _fighter.controlling.shieldButtonRaw = true;
                }
                timer = 1f;
            }
            else
            {
                timer -= Time.deltaTime;
                if (timer <= 0)
                {
                    _fighter.controlling.shieldButtonRaw = false;
                    _fighter.controlling.jumpButtonRaw = false;
                }
            }
        }
    }
}
