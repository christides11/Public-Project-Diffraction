namespace TightStuff.UI
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;
    
    public class ReimuFloatStaminaCooldown : MonoBehaviour
    {
        private FighterStatus _fighterUI;
        private RectTransform _rect;
    
        private Image _guage;
    
        // Start is called before the first frame update
        void Start()
        {
            _rect = GetComponent<RectTransform>();
            _guage = GetComponent<Image>();
    
            _fighterUI = transform.parent.GetComponent<FighterStatus>();
            transform.SetParent(_fighterUI.StaminaGuage4.transform);
            _rect.anchoredPosition *= 0;
            transform.localScale = Vector3.one;
            _guage.fillAmount = 0;
        }
    
        // Update is called once per frame
        void LateUpdate()
        {
            var maxStamina = _fighterUI.Fighter.maxStamina;
            if (maxStamina >= 3)
                transform.SetParent(_fighterUI.StaminaGuage4.transform);
            else if (maxStamina >= 2)
                transform.SetParent(_fighterUI.StaminaGuage3.transform);
            else if (maxStamina >= 1)
                transform.SetParent(_fighterUI.StaminaGuage2.transform);
            else
                transform.SetParent(_fighterUI.StaminaGuage1.transform);
            _rect.anchoredPosition *= 0;
            if (_fighterUI.Fighter.specialCooldown > 0)
                _guage.fillAmount = (1.5f - _fighterUI.Fighter.specialCooldown) / 1.5f;
            else
                _guage.fillAmount = 0;
        }
    }
}
