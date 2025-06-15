namespace TightStuff.UI
{
    using UnityEngine.Localization.Tables;
    using UnityEngine.Localization.Settings;
    using UnityEngine;
    using UnityEngine.UI;
    
    public class SpellcardFlash : UpdateAbstract
    {
        [SerializeField]
        private Text _shadowSubtitle;
        [SerializeField]
        private Text _actualSubtitle;
        [SerializeField]
        private RectTransform _targetSubtitle;
        private Vector2 _initPosSubtitle;
    
        [SerializeField]
        private Text _shadowTitle;
        [SerializeField]
        private Text _actualTitle;
        [SerializeField]
        private RectTransform _targetTitle;
        private Vector2 _initPosTitle;
    
        [SerializeField]
        private Image _border;
        [SerializeField]
        private RectTransform _targetBorder;
        private Vector2 _initPosBorder;
    
        [SerializeField]
        private Image _portrait;
        [SerializeField]
        private RectTransform _targetPortrait;
        private Vector2 _initPosPortrait;
    
        private bool _declaring;
        [SerializeField]
        private bool left;
    
        private float _delay;
        private TableReference _table;
    
        // Start is called before the first frame update
        void Start()
        {
            _initPosSubtitle = _shadowSubtitle.rectTransform.anchoredPosition;
            _initPosTitle = _shadowTitle.rectTransform.anchoredPosition;
            _initPosBorder = _border.rectTransform.anchoredPosition;
            _initPosPortrait = _portrait.rectTransform.anchoredPosition;
    
            _shadowSubtitle.rectTransform.anchoredPosition = _initPosSubtitle;
            _shadowTitle.rectTransform.anchoredPosition = _initPosTitle;
            _border.rectTransform.anchoredPosition = _initPosBorder;
            _portrait.rectTransform.anchoredPosition = _initPosPortrait;
    
    
            BaseFighterBehavior.SpellcardActivateEvent += OnSpellcardDeclare;
            BaseFighterBehavior.SpellcardActivateStopEvent += OnSpellcardDeclareStop;
        }
    
        private void OnDestroy()
        {
            BaseFighterBehavior.SpellcardActivateEvent -= OnSpellcardDeclare;
            BaseFighterBehavior.SpellcardActivateStopEvent -= OnSpellcardDeclareStop;
        }
    
        // Update is called once per frame
        public override void GUpdate()
        {
            if (_declaring)
            {
                _portrait.rectTransform.anchoredPosition = Vector2.Lerp(_portrait.rectTransform.anchoredPosition, _initPosPortrait, Time.fixedDeltaTime * 10f);
                _border.rectTransform.anchoredPosition = Vector2.Lerp(_border.rectTransform.anchoredPosition, _initPosBorder, Time.fixedDeltaTime * 10f);
                _shadowTitle.rectTransform.anchoredPosition = Vector2.Lerp(_shadowTitle.rectTransform.anchoredPosition, _initPosTitle, Time.fixedDeltaTime * 10f);
                _shadowSubtitle.rectTransform.anchoredPosition = Vector2.Lerp(_shadowSubtitle.rectTransform.anchoredPosition, _initPosSubtitle, Time.fixedDeltaTime * 10f);
            }
            else if (_delay <= 0)
            {
                _shadowTitle.color = Color.Lerp(_shadowTitle.color, new Color(0, 0, 0, 0), Time.fixedDeltaTime * 5f);
                _shadowSubtitle.color = Color.Lerp(_shadowSubtitle.color, new Color(0, 0, 0, 0), Time.fixedDeltaTime * 5f);
                _actualTitle.color = Color.Lerp(_actualTitle.color, new Color(1, 1, 1, 0), Time.fixedDeltaTime * 5f);
                _actualSubtitle.color = Color.Lerp(_actualTitle.color, new Color(1, 1, 1, 0), Time.fixedDeltaTime * 5f);
                _border.color = Color.Lerp(_border.color, new Color(0, 0, 0, 0), Time.fixedDeltaTime * 5f);
                _portrait.color = Color.Lerp(_portrait.color, new Color(1, 1, 1, 0), Time.fixedDeltaTime * 5f);
            }
            if (_delay > 0)
            {
                _delay -= Time.fixedDeltaTime;
            }
        }
    
        private void OnSpellcardDeclare(FighterState.AttackID id, FighterProperties ft, int fighterID)
        {
            if ((fighterID == 0 || fighterID == 2) ^ !left)
                return;
    
            _declaring = true;
            var secondColor = Color.green;
            _actualSubtitle.text = LocalizationSettings.StringDatabase.GetLocalizedString("SpellcardText", ft.name + "SCShortSide");
            _shadowSubtitle.text = LocalizationSettings.StringDatabase.GetLocalizedString("SpellcardText", ft.name + "SCShortSide");
            _actualTitle.text = LocalizationSettings.StringDatabase.GetLocalizedString("SpellcardText", ft.name + "SCLongSide");
            _shadowTitle.text = LocalizationSettings.StringDatabase.GetLocalizedString("SpellcardText", ft.name + "SCLongSide");
            if (id == FighterState.AttackID.Up)
            {
                secondColor = Color.red;
                _actualSubtitle.text = LocalizationSettings.StringDatabase.GetLocalizedString("SpellcardText", ft.name + "SCShortUp");
                _shadowSubtitle.text = LocalizationSettings.StringDatabase.GetLocalizedString("SpellcardText", ft.name + "SCShortUp");
                _actualTitle.text = LocalizationSettings.StringDatabase.GetLocalizedString("SpellcardText", ft.name + "SCLongUp");
                _shadowTitle.text = LocalizationSettings.StringDatabase.GetLocalizedString("SpellcardText", ft.name + "SCLongUp");
            }
            else if (id == FighterState.AttackID.Down)
            {
                secondColor = Color.blue;
                _actualSubtitle.text = LocalizationSettings.StringDatabase.GetLocalizedString("SpellcardText", ft.name + "SCShortDown");
                _shadowSubtitle.text = LocalizationSettings.StringDatabase.GetLocalizedString("SpellcardText", ft.name + "SCShortDown");
                _actualTitle.text = LocalizationSettings.StringDatabase.GetLocalizedString("SpellcardText", ft.name + "SCLongDown");
                _shadowTitle.text = LocalizationSettings.StringDatabase.GetLocalizedString("SpellcardText", ft.name + "SCLongDown");
            }
            _shadowTitle.color = new Color(0, 0, 0, 0.75f);
            _shadowSubtitle.color = new Color(0, 0, 0, 0.75f);
            _actualTitle.color = new Color(1, 1, 1, 1);
            _actualSubtitle.color = new Color(1, 1, 1, 1);
            _border.color = (secondColor + new Color(0, 0, 0, 0)) / 2;
            _border.rectTransform.anchoredPosition = _targetBorder.anchoredPosition;
            _shadowTitle.rectTransform.anchoredPosition = _targetTitle.anchoredPosition;
            _shadowSubtitle.rectTransform.anchoredPosition = _targetSubtitle.anchoredPosition;
            //_portrait.color = Color.white;
            _portrait.rectTransform.anchoredPosition = _targetPortrait.anchoredPosition;
        }
    
        private void OnSpellcardDeclareStop(FighterState.AttackID id, FighterProperties ft, int fighterID)
        {
            _declaring = false;
            _delay = 0.25f;
        }
    }
}
