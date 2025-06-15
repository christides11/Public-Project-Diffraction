namespace TightStuff.UI
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;
    
    public class MarisaPotionCount : MonoBehaviour
    {
        private FighterStatus _fighterUI;
        private RectTransform _rect;
        private Extension.MarisaFighterExtension _marisa;
    
        [SerializeField]
        private Image _guage;
        [SerializeField]
        private Image _guage2;
        [SerializeField]
        private Image _potion;
        [SerializeField]
        private Text _count;
        [SerializeField]
        private Text _countShadow;
    
        // Start is called before the first frame update
        void Start()
        {
            _rect = GetComponent<RectTransform>();
    
            _fighterUI = transform.parent.GetComponent<FighterStatus>();
            transform.localScale = Vector3.one;
            _guage.fillAmount = 0;
    
            _marisa = _fighterUI.FighterExtension as Extension.MarisaFighterExtension;
        }
    
        // Update is called once per frame
        void LateUpdate()
        {
            _guage2.fillAmount = 1 - (_marisa.CurrentPotionCD - _marisa.PotionCD) / _marisa.PotionCD;
            _guage.fillAmount = 1 - _marisa.CurrentPotionCD / _marisa.PotionCD;
            var count = 2 - Mathf.CeilToInt(_marisa.CurrentPotionCD / _marisa.PotionCD);
            _count.text = "x" + count.ToString();
            _countShadow.text = "x" + count.ToString();
            if (_marisa.CurrentPotionCD > _marisa.PotionCD)
                _potion.color = Color.Lerp(_potion.color, new Color(0.5f, 0.5f, 0.5f, 0.5f), Time.deltaTime * 10f);
            else
                _potion.color = Color.Lerp(_potion.color, Color.white, Time.deltaTime * 10f);
        }
    }
}
