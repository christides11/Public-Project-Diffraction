namespace TightStuff.UI
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;
    
    public class CharacterStaminaDisplay : UpdateAbstract
    {
        [SerializeField]
        private BaseFighterBehavior _fighter;
        private Entity _timer;
    
        private float _prevStam;
    
        [SerializeField]
        private float _timeAlive;
        [SerializeField]
        private float _lerpSpd;
        [SerializeField]
        private float _height;
    
        [SerializeField]
        private Color _lowStamina;
        [SerializeField]
        private Color _mediumStamina;
    
        private float _visibleStamina;
    
        [Header("Stamina Guage")]
        [SerializeField]
        private Image _staminaGuage;
        [SerializeField]
        private List<Image> _spareStamina;
    
        // Start is called before the first frame update
        void Start()
        {
            _timer = GetComponent<Entity>();
            _timer.stateVars.genericTimer = 5;
            transform.SetParent(null);
            _prevStam = _fighter.stateVarsF.stamina;
    
            foreach (var spare in _spareStamina)
            {
                var b = spare.color;
                b.a = 0;
                spare.color = b;
            }
            var a = _staminaGuage.color;
            a.a = 0;
            _staminaGuage.color = a;
        }
    
        // Update is called once per frame
        public override void GUpdate()
        {
    
        }
        // Update is called once per frame
        public override void LateGUpdate()
        {
            var stamChanged = _prevStam != _fighter.stateVarsF.stamina;
            _prevStam = _fighter.stateVarsF.stamina;
    
            if (stamChanged)
                _timer.stateVars.genericTimer = 0;
    
            transform.position = _fighter.transform.position + Vector3.up * _height;
            if (_timer.stateVars.genericTimer >= _timeAlive && _fighter.stateVarsF.stamina > 2)
            {
                foreach (var spare in _spareStamina)
                {
                    var ass = spare.color;
                    ass.a = 0;
                    spare.color = Color.Lerp(spare.color, ass, 7 * Time.fixedDeltaTime);
                }
                var c = _staminaGuage.color;
                c.a = 0;
                _staminaGuage.color = Color.Lerp(_staminaGuage.color, c, 7 * Time.fixedDeltaTime);
                return;
            }
    
    
            foreach (var spare in _spareStamina)
            {
                var b = spare.color;
                if (_fighter.ActionState.GetState() is GroundedFighterDeadState)
                    b.a = 0;
                else
                    b.a = 1;
                spare.color = b;
            }
            var a = _staminaGuage.color;
    
            if (_fighter.ActionState.GetState() is GroundedFighterDeadState)
                a.a = 0;
            else
                a.a = 1;
            _staminaGuage.color = a;
    
            var lerpspd = _lerpSpd * Time.fixedDeltaTime;
    
            _visibleStamina += (_visibleStamina > _fighter.stateVarsF.stamina) ? -lerpspd : lerpspd;
            var snapCondition = _visibleStamina < _fighter.stateVarsF.stamina + 0.1f && _fighter.stateVarsF.stamina > _fighter.stateVarsF.stamina - 0.1f;
            _visibleStamina = snapCondition ? _fighter.stateVarsF.stamina : _visibleStamina;
            _staminaGuage.fillAmount = Mathf.Clamp(_visibleStamina - Mathf.Floor(Mathf.Clamp(_visibleStamina - 0.01f, 0, _fighter.stateVarsF.maxStamina - 1)), 0, 1);
    
            _timer.stateVars.genericTimer += Time.fixedDeltaTime;
    
            for (int i = 0; i < 4 - 1; i++)
                _spareStamina[i].gameObject.SetActive(_visibleStamina - 1 > i);
    
            if (_visibleStamina <= 1.1f)
            {
                for (int i = 3; i < 6; i++)
                    _spareStamina[i].color = Color.Lerp(_spareStamina[i].color, _lowStamina, Time.fixedDeltaTime * 8);
                return;
            }
            else if (_visibleStamina <= 2.1f)
            {
                for (int i = 3; i < 6; i++)
                    _spareStamina[i].color = Color.Lerp(_spareStamina[i].color, _mediumStamina, Time.fixedDeltaTime * 8);
    
                if (_staminaGuage.fillAmount >= 0.33f && _staminaGuage.fillAmount < 0.67f)
                    _staminaGuage.color = _mediumStamina * (1 - (0.67f - _staminaGuage.fillAmount) / 0.33f) + _lowStamina * ((0.67f - _staminaGuage.fillAmount) / 0.33f);
                else if (_staminaGuage.fillAmount < 0.33f)
                    _staminaGuage.color = _lowStamina;
    
                return;
            }
            for (int i = 3; i < 6; i++)
                _spareStamina[i].color = Color.Lerp(_spareStamina[i].color, Color.white, Time.fixedDeltaTime * 8);
    
            if (_staminaGuage.fillAmount >= 0.67f)
                _staminaGuage.color = Color.white * (1 - (1 - _staminaGuage.fillAmount) / 0.33f) + _mediumStamina * ((1 - _staminaGuage.fillAmount) / 0.33f);
            else if (_staminaGuage.fillAmount >= 0.33f)
                _staminaGuage.color = _mediumStamina * (1 - (0.67f - _staminaGuage.fillAmount) / 0.33f) + _lowStamina * ((0.67f - _staminaGuage.fillAmount) / 0.33f);
            else
                _staminaGuage.color = _lowStamina;
        }
    }
}
