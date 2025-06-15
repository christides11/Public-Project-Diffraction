namespace TightStuff.UI
{
    using UnityEngine.UI;
    using UnityEngine;
    using System.Collections.Generic;
    using TMPro;
    using UnityEngine.Events;
    using UnityEngine.Localization.Tables;
    using UnityEngine.Localization.Settings;
    
    public class FighterStatus : MonoBehaviour
    {
        private MatchManager _matchManager;
    
        [Header("Fighter Portrait & Name")]
        [SerializeField]
        private Image _name;
        [SerializeField]
        private Image _nameBG;
        [SerializeField]
        private Image _portrait;
        [SerializeField]
        private TranslucentOnTriggerEnter _portraitTranslucency;
        [SerializeField]
        private TranslucentOnTriggerEnter _nameTranslucency;
        [SerializeField]
        private Color _borderColor;
    
        [Header("Damage Percentage")]
        [SerializeField]
        private ChangeColorAccordingToPercent _percentColor;
        [SerializeField]
        private Text _percentDigit1;
        [SerializeField]
        private Text _percentDigit2;
        [SerializeField]
        private Text _percentDigit3;
        [SerializeField]
        private Text _percentDecimal;
        [SerializeField]
        private ParticleSystem _percentBreak;
        [SerializeField]
        private ParticleSystem _staminaBreak;
    
        [SerializeField]
        private FloatEvent OnDamage;
    
        [Header("Stamina Border")]
        [SerializeField]
        private Image _staminaBorder1;
        [SerializeField]
        private Image _staminaBorder2;
        [SerializeField]
        private Image _staminaBorder3;
        [SerializeField]
        private Image _staminaBorder4;
    
        [Header("Stamina Guage")]
        [SerializeField]
        private Image _staminaGuage1;
        [SerializeField]
        private Image _staminaGuage2;
        [SerializeField]
        private Image _staminaGuage3;
        [SerializeField]
        private Image _staminaGuage4;
    
        [SerializeField]
        private Color _mediumStamina;
        [SerializeField]
        private Color _lowStamina;
    
        [Header("Graze Meter")]
        [SerializeField]
        private Slider _grazeMeter;
        [SerializeField]
        private Image _grazeColor;
        [SerializeField]
        private Color _scColorPositive;
        [SerializeField]
        private Color _scColorNegative;
    
        [Header("Stocks")]
        [SerializeField]
        private GameObject _stockArea;
        private List<RectTransform> _stockIcons;
    
        [Header("Spellcard Icons")]
        [SerializeField]
        private GameObject _scUpPrefab;
        [SerializeField]
        private GameObject _scUpArea;
        private List<Image> _scUpIcons;
    
        [SerializeField]
        private GameObject _scSidePrefab;
        [SerializeField]
        private GameObject _scSideArea;
        private List<Image> _scSideIcons;
    
        [SerializeField]
        private GameObject _scDownPrefab;
        [SerializeField]
        private GameObject _scDownArea;
        private List<Image> _scDownIcons;
    
        [Header("Spellcard Selection")]
        private Image _selectedCard;
        [SerializeField]
        private RectTransform _selectedCardPos;
        [SerializeField]
        private Material _cardFlash;
    
        [SerializeField]
        private Image _cardNameSplash;
        [SerializeField]
        private Text _cardName;
    
        [Header("Assists")]
        [SerializeField]
        private Image _assistCDImage;
        [SerializeField]
        private Image _assistCDIcon;
        [SerializeField]
        private Image _assistCDBG;
    
        [Header("Player Tag")]
        [SerializeField]
        private List<TextMeshProUGUI> _tags;
    
        [SerializeField]
        private int _id;
        public FighterStateVars Fighter => _matchManager.fighters[_id].stateVarsF;
        public EntityState FighterEntity => _matchManager.fighters[_id].entity.stateVars;
        public Extension.BaseFighterExtensions FighterExtension => _matchManager.fighters[_id].extension;
    
        private Color _initGrazeColor;
        private Camera cam;
    
        #region Encapsulation
        public Image Name { get => _name; }
        public Image Portrait { get => _portrait; }
        public TranslucentOnTriggerEnter PortraitTranslucency { get => _portraitTranslucency; }
        public TranslucentOnTriggerEnter NameTranslucency { get => _nameTranslucency; }
        public ChangeColorAccordingToPercent PercentColor { get => _percentColor; }
        public Text PercentDigit1 { get => _percentDigit1; }
        public Text PercentDigit2 { get => _percentDigit2; }
        public Text PercentDigit3 { get => _percentDigit3; }
        public Text PercentDecimal { get => _percentDecimal; }
        public ParticleSystem PercentBreak { get => _percentBreak; }
        public ParticleSystem StaminaBreak { get => _staminaBreak; }
        public FloatEvent OnDamage1 { get => OnDamage; }
        public Image StaminaBorder1 { get => _staminaBorder1; }
        public Image StaminaBorder2 { get => _staminaBorder2; }
        public Image StaminaBorder3 { get => _staminaBorder3; }
        public Image StaminaBorder4 { get => _staminaBorder4; }
        public Image StaminaGuage1 { get => _staminaGuage1; }
        public Image StaminaGuage2 { get => _staminaGuage2; }
        public Image StaminaGuage3 { get => _staminaGuage3; }
        public Image StaminaGuage4 { get => _staminaGuage4; }
        public Color MediumStamina { get => _mediumStamina; }
        public Color LowStamina { get => _lowStamina; }
        public Slider GrazeMeter { get => _grazeMeter; }
        public Image GrazeColor { get => _grazeColor; }
        public Color ScColorPositive { get => _scColorPositive; }
        public Color ScColorNegative { get => _scColorNegative; }
        public GameObject StockArea { get => _stockArea; }
        public List<RectTransform> StockIcons { get => _stockIcons; }
        public GameObject ScUpPrefab { get => _scUpPrefab; }
        public GameObject ScUpArea { get => _scUpArea; }
        public List<Image> ScUpIcons { get => _scUpIcons; }
        public GameObject ScSidePrefab { get => _scSidePrefab; }
        public GameObject ScSideArea { get => _scSideArea; }
        public List<Image> ScSideIcons { get => _scSideIcons; }
        public GameObject ScDownPrefab { get => _scDownPrefab; }
        public GameObject ScDownArea { get => _scDownArea; }
        public List<Image> ScDownIcons { get => _scDownIcons; }
        public Image SelectedCard { get => _selectedCard; }
        public RectTransform SelectedCardPos { get => _selectedCardPos; }
        public Material CardFlash { get => _cardFlash; }
        public Image CardNameSplash { get => _cardNameSplash; }
        public Text CardName { get => _cardName; }
        public List<TextMeshProUGUI> Tags { get => _tags; }
        public int Id { get => _id; }
        #endregion
    
        void Start()
        {
            _matchManager = FindObjectOfType<MatchManager>();
            if (_id > _matchManager.fighters.Count - 1)
            {
                gameObject.SetActive(false);
                return;
            }
    
            _stockIcons = new List<RectTransform>();
    
            _scUpIcons = new List<Image>();
            _scSideIcons = new List<Image>();
            _scDownIcons = new List<Image>();
    
            _initGrazeColor = _grazeColor.color;
            SetPlayerTag();
            InitializePortraindAndName();
            InitializeStocks();
            InitializeSpellcards();
            ActionSM.OnStateEnter += OnStateEnter;
            cam = Camera.main;
            _percentBreak.transform.SetParent(null);
            _staminaBreak.transform.SetParent(null);
    
            _percentBreak.transform.localScale = new Vector3(_percentBreak.transform.localScale.x / _percentBreak.transform.lossyScale.x, _percentBreak.transform.localScale.y / _percentBreak.transform.lossyScale.y, 1);
            _staminaBreak.transform.localScale = new Vector3(_staminaBreak.transform.localScale.x / _staminaBreak.transform.lossyScale.x, _staminaBreak.transform.localScale.y / _staminaBreak.transform.lossyScale.y, 1);
    
            if (_matchManager.fighters[_id].AssistObj != null)
            {
                Debug.Log(_matchManager.fighters[_id].AssistObj.entity);
                _assistCDIcon.sprite = (_matchManager.fighters[_id].AssistObj.entity.AssociatedRenderers[0] as SpriteRenderer).sprite;
                var temp = (_matchManager.fighters[_id].AssistObj.entity.AssociatedRenderers[0] as SpriteRenderer).color;
                _assistCDImage.color = temp;
                temp.a = 0.25f;
                _assistCDBG.color = temp;
    
            }
            else
                _assistCDImage.transform.parent.gameObject.SetActive(false);
    
            if (_matchManager.fighters[_id].Ft.fighterExtraUI != null)
            {
                var extra = Instantiate(_matchManager.fighters[_id].Ft.fighterExtraUI);
                var rect = extra.GetComponent<RectTransform>();
                var initPos = rect.anchoredPosition;
                extra.transform.SetParent(transform);
                rect.anchoredPosition = initPos;
                extra.SetActive(true);
            }
        }
        private void OnDestroy()
        {
            ActionSM.OnStateEnter -= OnStateEnter;
        }
    
        private void FixedUpdate()
        {
            SetPercentBreakPos();
            SetStaminaBreakPos(Fighter);
            SetStamina(Fighter);
        }
    
        void LateUpdate()
        {
            if (_id > _matchManager.fighters.Count - 1)
            {
                gameObject.SetActive(false);
                return;
            }
    
            SetPercent(FighterEntity);
            SetStaminaBorder(Fighter);
            SetGrazeMeter(Fighter);
            SetStocks(Fighter);
            SetSpellcardDeck(Fighter);
            SetSpellcard(Fighter);
            SetDeadPortrait(FighterEntity);
            SetCardName(Fighter);
            SetAssistCDFill();
        }
    
        private void SetAssistCDFill()
        {
            _assistCDImage.fillAmount = 1 - Fighter.assistCD / _matchManager.fighters[_id].AssistObj.MaxLifeTime;
            if (_assistCDImage.fillAmount < 1f)
                _assistCDIcon.color = Color.Lerp(_assistCDIcon.color, new Color(0.5f, 0.5f, 0.5f, 0.5f), Time.deltaTime * 10f);
            else
                _assistCDIcon.color = Color.Lerp(_assistCDIcon.color, Color.white, Time.deltaTime * 10f);
        }
    
        private void SetCardName(FighterStateVars fighter)
        {
            if (_selectedCard != null)
            {
                var secondColor = Color.green;
                var isEn = LocalizationSettings.SelectedLocale.Identifier.Code == "en";
                var shortName = LocalizationSettings.StringDatabase.GetLocalizedString("SpellcardText", _matchManager.fighters[_id].Ft.name + "SCShortSide");
                if (isEn)
                    shortName = "";
                var longName = LocalizationSettings.StringDatabase.GetLocalizedString("SpellcardText", _matchManager.fighters[_id].Ft.name + "SCLongSide");
                if (fighter.currentSpellcardID == FighterState.AttackID.Up)
                {
                    secondColor = Color.red;
                    if (!isEn)
                        shortName = LocalizationSettings.StringDatabase.GetLocalizedString("SpellcardText", _matchManager.fighters[_id].Ft.name + "SCShortUp");
                    longName = LocalizationSettings.StringDatabase.GetLocalizedString("SpellcardText", _matchManager.fighters[_id].Ft.name + "SCLongUp");
                }
                else if (fighter.currentSpellcardID == FighterState.AttackID.Down)
                {
                    secondColor = Color.blue;
                    if (!isEn)
                        shortName = LocalizationSettings.StringDatabase.GetLocalizedString("SpellcardText", _matchManager.fighters[_id].Ft.name + "SCShortDown");
                    longName = LocalizationSettings.StringDatabase.GetLocalizedString("SpellcardText", _matchManager.fighters[_id].Ft.name + "SCLongDown");
                }
                if (isEn)
                    _cardName.text = longName;
                else
                {
                    if (longName.Length <= 15)
                        _cardName.text = shortName + "\n  " + longName;
                    else
                        _cardName.text = shortName + "\n" + longName;
                }
                _cardName.color = Color.white;
                _cardNameSplash.color = (secondColor + new Color(0, 0, 0, 0)) / 2;
                _cardNameSplash.rectTransform.anchoredPosition = Vector2.Lerp(_cardNameSplash.rectTransform.anchoredPosition, new Vector2(_cardNameSplash.rectTransform.anchoredPosition.x, 0), Time.deltaTime * 5f);
            }
            else
            {
                _cardNameSplash.color = Color.Lerp(_cardNameSplash.color, new Color(0, 0, 0, 0), Time.deltaTime * 4f);
                _cardName.color = Color.Lerp(_cardName.color, new Color(1, 1, 1, 0), Time.deltaTime * 10f);
                if (CardNameSplash.color.a < 0.1f)
                    _cardNameSplash.rectTransform.anchoredPosition = new Vector2(_cardNameSplash.rectTransform.anchoredPosition.x, -40);
            }
        }
    
        private void SetPercentBreakPos()
        {
            _percentBreak.transform.position = cam.ScreenToWorldPoint(_grazeMeter.transform.position);
            _percentBreak.transform.position = new Vector3(_percentBreak.transform.position.x + 0.5f, _percentBreak.transform.position.y + 0.5f, 0);
        }
        private void SetStaminaBreakPos(FighterStateVars fighter)
        {
            var maxStamina = fighter.maxStamina;
            var pos = Vector2.zero;
            if (maxStamina == 4)
                pos = _staminaBorder4.transform.position;
            else if (maxStamina == 3)
                pos = _staminaBorder3.transform.position;
            else if (maxStamina == 2)
                pos = _staminaBorder2.transform.position;
            else 
                pos = _staminaBorder1.transform.position;
    
            _staminaBreak.transform.position = cam.ScreenToWorldPoint(pos);
            _staminaBreak.transform.position = new Vector3(_percentBreak.transform.position.x - 1, _percentBreak.transform.position.y - 1, 0);
        }
    
        private void SetDeadPortrait(EntityState fighterEntity)
        {
            if (!fighterEntity.enabled)
            {
                var colorDead = Color.gray;
                colorDead.a = 0.5f;
                _portraitTranslucency.initAlpha = 0.5f;
                _nameTranslucency.initAlpha = 0.5f;
                _portrait.color = colorDead;
                _name.color = colorDead;
            }
            else
            {
                _portraitTranslucency.initAlpha = 1;
                _nameTranslucency.initAlpha = 1;
                _portrait.color = Color.Lerp(_portrait.color, Color.white, Time.deltaTime * 5);
                if (_matchManager.fighters[_id].controlling.playerColor.r < 0.5 && _matchManager.fighters[_id].controlling.playerColor.g < 0.5 && _matchManager.fighters[_id].controlling.playerColor.b < 0.5)
                    _name.color = Color.Lerp(_name.color, Color.white, Time.deltaTime * 5);
                else if (_matchManager.fighters[_id].controlling.playerColor == Color.white)
                    _name.color = Color.Lerp(_name.color, _borderColor, Time.deltaTime * 5);
                else
                    _name.color = Color.Lerp(_name.color, (Color.white + _matchManager.controllers[_id].playerColor) / 2, Time.deltaTime * 5);
            }
        }
    
        private void SetSpellcard(FighterStateVars fighter)
        {
            if (_selectedCard != null)
            {
                _selectedCard.rectTransform.anchoredPosition = Vector2.Lerp(_selectedCard.rectTransform.anchoredPosition, _selectedCardPos.anchoredPosition, Time.deltaTime * 10f);
                if (Vector2.Distance(_selectedCard.rectTransform.anchoredPosition, _selectedCardPos.anchoredPosition) < 0.2f)
                    _selectedCard.rectTransform.anchoredPosition = _selectedCardPos.anchoredPosition;
                if (fighter.grazeMeter <= 0)
                {
                    _selectedCard.material = _cardFlash;
                    _selectedCard.color = Color.Lerp(_selectedCard.color, new Color(1, 1, 1, 0), Time.deltaTime * 10f);
                    if (_selectedCard.color.a <= 0.1f)
                        Destroy(_selectedCard);
                }
                else
                    _selectedCard.color = Color.white;
            }
        }
    
        private void SetStocks(FighterStateVars fighter)
        {
            while (_stockIcons.Count < fighter.stocks && _stockIcons.Count > 0)
                AddStock(_stockIcons.Count);
            while (_stockIcons.Count > fighter.stocks && _stockIcons.Count > 0)
                RemoveStockFromLast();
        }
        private void SetSpellcardDeck(FighterStateVars fighter)
        {
            GameObject cardSelect = null;
    
            if (_selectedCard != null)
            {
                foreach (var icon in _scUpIcons)
                    icon.color = Color.Lerp(icon.color, Color.gray, Time.deltaTime * 10f);
                foreach (var icon in _scSideIcons)
                    icon.color = Color.Lerp(icon.color, Color.gray, Time.deltaTime * 10f);
                foreach (var icon in _scDownIcons)
                    icon.color = Color.Lerp(icon.color, Color.gray, Time.deltaTime * 10f);
            }
            else
            {
                foreach (var icon in _scUpIcons)
                    icon.color = Color.Lerp(icon.color, new Color(1, 1, 1, icon.color.a), Time.deltaTime * 10f);
                foreach (var icon in _scSideIcons)
                    icon.color = Color.Lerp(icon.color, new Color(1, 1, 1, icon.color.a), Time.deltaTime * 10f);
                foreach (var icon in _scDownIcons)
                    icon.color = Color.Lerp(icon.color, new Color(1, 1, 1, icon.color.a), Time.deltaTime * 10f);
            }
    
            while (_scUpIcons.Count < fighter.spellcardUpCount && _scUpIcons.Count > 0)
                AddCard(_scUpArea, _scUpPrefab, ref _scUpIcons);
            while (_scUpIcons.Count > fighter.spellcardUpCount && _scUpIcons.Count > 0)
                cardSelect = RemoveCard(ref _scUpIcons, fighter);
    
            while (_scSideIcons.Count < fighter.spellcardSideCount && _scSideIcons.Count > 0)
                AddCard(_scSideArea, _scSidePrefab, ref _scSideIcons);
            while (_scSideIcons.Count > fighter.spellcardSideCount && _scSideIcons.Count > 0)
                cardSelect = RemoveCard(ref _scSideIcons, fighter);
    
            while (_scDownIcons.Count < fighter.spellcardDownCount && _scDownIcons.Count > 0)
                AddCard(_scDownArea, _scDownPrefab, ref _scDownIcons);
            while (_scDownIcons.Count > fighter.spellcardDownCount && _scDownIcons.Count > 0)
                cardSelect = RemoveCard(ref _scDownIcons, fighter);
    
            if (cardSelect == null)
                return;
    
            if (cardSelect.TryGetComponent(out Image rect))
                _selectedCard = rect;
        }
    
        private void SetGrazeMeter(FighterStateVars fighter)
        {
            var desiredColor = _initGrazeColor;
            _grazeMeter.value = Mathf.Lerp(_grazeMeter.value, fighter.grazeMeter / 30, Time.deltaTime * 15);
            if (fighter.grazeMeter >= 30)
            {
                if (_grazeMeter.value < 1)
                    _grazeColor.color = Color.white;
                _grazeMeter.value = 1;
            }
            var timeSined = Mathf.Sin(Time.time * 5);
            var timeSinedAbs = Mathf.Abs(timeSined);
    
            if (fighter.spellcardState || fighter.grazeMeter >= 30)
            {
                if (timeSined > 0)
                    desiredColor = timeSinedAbs * _scColorPositive + (1 - timeSinedAbs) * _initGrazeColor;
                else
                    desiredColor = timeSinedAbs * _scColorNegative + (1 - timeSinedAbs) * _initGrazeColor;
            }
    
            _grazeColor.color = Color.Lerp(_grazeColor.color, desiredColor, Time.deltaTime * 5);
        }
        private void SetPlayerTag()
        {
    
            foreach (var tag in _tags)
            {
                var text = _matchManager.controllers[_id].playerTag;
                if (text != "")
                    tag.text = text;
                else
                    tag.text = "P" + (_id + 1);
    
                if (_matchManager.fighters[_id].controlling.playerColor == Color.white)
                    tag.color = Color.white;
            }
            if (_matchManager.fighters[_id].controlling.playerColor.r < 0.5 && _matchManager.fighters[_id].controlling.playerColor.g < 0.5 && _matchManager.fighters[_id].controlling.playerColor.b < 0.5)
                _tags[_tags.Count - 1].color = Color.white;
            else if (_matchManager.fighters[_id].controlling.playerColor == Color.white)
                _tags[_tags.Count - 1].color = _borderColor;
            else
                _tags[_tags.Count - 1].color = (_matchManager.controllers[_id].playerColor + Color.white) / 2;
        }
    
        private void SetStamina(FighterStateVars fighter)
        {
            var interpSpd = Time.deltaTime * 4;
    
            SetSingleWheelDisplay(ref _staminaGuage1, fighter, interpSpd, 0);
            SetSingleWheelDisplay(ref _staminaGuage2, fighter, interpSpd, 1);
            SetSingleWheelDisplay(ref _staminaGuage3, fighter, interpSpd, 2);
            SetSingleWheelDisplay(ref _staminaGuage4, fighter, interpSpd, 3);
    
            void SetSingleWheelDisplay(ref Image guage, FighterStateVars fighter, float interpSpd, int wheelNum)
            {
                var stamina = fighter.stamina;
                if (MatchManager.rollingBack <= 0)
                {
                    guage.fillAmount += (guage.fillAmount > Mathf.Clamp(stamina - wheelNum, 0, 1)) ? -interpSpd : interpSpd;
                    var snapCondition = guage.fillAmount < Mathf.Clamp(stamina - wheelNum, 0, 1) + 0.1f && guage.fillAmount > Mathf.Clamp(stamina - wheelNum, 0, 1) - 0.1f;
                    guage.fillAmount = snapCondition ? Mathf.Clamp(stamina - wheelNum, 0, 1) : guage.fillAmount;
                }
                else
                {
                    guage.fillAmount = Mathf.Clamp(stamina - wheelNum, 0, 1);
                }
    
                if (fighter.stamina <= 1.1f)
                {
                    guage.color = Color.Lerp(guage.color, _lowStamina, Time.fixedDeltaTime * 8);
                    return;
                }
                else if (fighter.stamina <= 2.1f)
                {
                    guage.color = Color.Lerp(guage.color, _mediumStamina, Time.fixedDeltaTime * 8);
    
                    if (guage.fillAmount >= 0.33f && guage.fillAmount < 0.67f)
                        guage.color = _mediumStamina * (1 - (0.67f - guage.fillAmount) / 0.33f) + _lowStamina * ((0.67f - guage.fillAmount) / 0.33f);
                    else if (guage.fillAmount < 0.33f)
                        guage.color = _lowStamina;
    
                    return;
                }
    
                if (guage.fillAmount >= 0.67f)
                    guage.color = Color.white * (1 - (1 - guage.fillAmount) / 0.33f) + _mediumStamina * ((1 - guage.fillAmount) / 0.33f);
                else if (guage.fillAmount >= 0.33f)
                    guage.color = _mediumStamina * (1 - (0.67f - guage.fillAmount) / 0.33f) + _lowStamina * ((0.67f - guage.fillAmount) / 0.33f);
                else
                    guage.color = _lowStamina;
            }
        }
    
        private void SetStaminaBorder(FighterStateVars fighter)
        {
            var interpSpd = Time.deltaTime * 4;
    
            SetSingleWheelDisplay(ref _staminaBorder1, fighter, 0);
            SetSingleWheelDisplay(ref _staminaBorder2, fighter, 1);
            SetSingleWheelDisplay(ref _staminaBorder3, fighter, 2);
            SetSingleWheelDisplay(ref _staminaBorder4, fighter, 3);
    
            void SetSingleWheelDisplay(ref Image guage, FighterStateVars fighter, int wheelNum)
            {
                var maxStamina = fighter.maxStamina;
    
                guage.color = Color.Lerp(guage.color, Color.white, Time.fixedDeltaTime * 8);
                if (wheelNum > maxStamina - 1)
                    guage.color = new Color(1, 1, 1, 0);
            }
        }
    
        private void SetPercent(EntityState fighterEntity)
        {
            int decimalNum = (int)(fighterEntity.percent * 10) % 10;
            int digit1 = (int)fighterEntity.percent % 10;
            int digit2 = (int)(fighterEntity.percent / 10) % 10;
            int digit3 = (int)(fighterEntity.percent / 100) % 10;
    
            int displayDecimal = int.Parse(_percentDecimal.text);
            int displayDigit1 = int.Parse(_percentDigit1.text);
            int displayDigit2 = _percentDigit2.text == "" ? 0 : int.Parse(_percentDigit2.text);
            int displayDigit3 = _percentDigit3.text == "" ? 0 : int.Parse(_percentDigit3.text);
    
            int actualTotal = decimalNum + digit1 * 10 + digit2 * 100 + digit3 * 1000;
            int displayTotal = displayDecimal + displayDigit1 * 10 + displayDigit2 * 100 + displayDigit3 * 1000;
    
            var difference = actualTotal - displayTotal;
    
            if (difference > 0)
                OnDamage.Invoke(difference / 10f);
    
            _percentDecimal.text = decimalNum.ToString();
            _percentDigit1.text = digit1.ToString();
            _percentDigit2.text = digit2.ToString();
            _percentDigit3.text = digit3.ToString();
            _percentColor.ChangePercentColor(fighterEntity.percent, fighterEntity.enabled);
            if ((int)(fighterEntity.percent / 10) <= 0)
                _percentDigit2.text = "";
            if ((int)(fighterEntity.percent / 100) <= 0)
                _percentDigit3.text = "";
        }
    
        private void InitializePortraindAndName()
        {
            _portrait.sprite = _matchManager.fighters[_id].Ft.palette[_matchManager.fighters[_id].colorID].portrait;
            _portrait.color = Color.white;
            _name.sprite = _matchManager.fighters[_id].Ft.nameJap;
            _name.color = Color.white;
            _nameBG.sprite = _matchManager.fighters[_id].Ft.nameJapBG;
            _nameBG.color = _borderColor;
            if (_matchManager.fighters[_id].controlling.playerColor == Color.white)
                _nameBG.color = Color.white;
        }
    
        private void InitializeStocks()
        {
            for (int i = 0; i < _matchManager.fighters[_id].stateVarsF.stocks; i++)
            {
                AddStock(i);
            }
        }
        private void InitializeSpellcards()
        {
            for (int i = 0; i < _matchManager.fighters[_id].stateVarsF.spellcardUpCount; i++)
            {
                AddCard(_scUpArea, _scUpPrefab, ref _scUpIcons);
            }
            for (int i = 0; i < _matchManager.fighters[_id].stateVarsF.spellcardSideCount; i++)
            {
                AddCard(_scSideArea, _scSidePrefab, ref _scSideIcons);
            }
            for (int i = 0; i < _matchManager.fighters[_id].stateVarsF.spellcardDownCount; i++)
            {
                AddCard(_scDownArea, _scDownPrefab, ref _scDownIcons);
            }
        }
    
        private void AddStock(int stockNum)
        {
            var icon = Instantiate(_matchManager.fighters[_id].Ft.palette[_matchManager.fighters[_id].colorID].stockIcon).GetComponent<RectTransform>();
            icon.SetParent(_stockArea.transform);
            icon.anchoredPosition = 17 * stockNum * Vector2.right;
            icon.localScale = Vector3.one;
            _stockIcons.Add(icon);
        }
        private void RemoveStockFromLast()
        {
            var stock = _stockIcons[_stockIcons.Count - 1];
            _stockIcons.RemoveAt(_stockIcons.Count - 1);
            Destroy(stock.gameObject);
        }
    
        private void AddCard(GameObject area, GameObject prefab, ref List<Image> currentIcons)
        {
            var icon = Instantiate(prefab).GetComponent<Image>();
            icon.rectTransform.SetParent(area.transform);
            icon.rectTransform.anchoredPosition *= 0;
            icon.rectTransform.localScale = Vector3.one;
            foreach (var item in currentIcons)
                item.rectTransform.anchoredPosition += 3 * Vector2.right;
            currentIcons.Add(icon);
        }
        private GameObject RemoveCard(ref List<Image> currentIcons, FighterStateVars fighter)
        {
            var stock = currentIcons[currentIcons.Count - 1];
            currentIcons.RemoveAt(currentIcons.Count - 1);
            foreach (var item in currentIcons)
                item.rectTransform.anchoredPosition -= 3 * Vector2.right;
            stock.rectTransform.anchorMin = Vector2.one * 0.5f;
            stock.rectTransform.anchorMax = Vector2.one * 0.5f;
            stock.rectTransform.anchoredPosition3D -= Vector3.forward;
            stock.rectTransform.pivot = Vector2.one * 0.5f;
            stock.rectTransform.SetParent(_selectedCardPos.parent);
            if (stock.TryGetComponent(out GenerateTriggerOverUI trigger))
                Destroy(trigger._triggerObj.gameObject);
            return stock.gameObject;
        }
    
        private void OnStateEnter(State s)
        {
            if (s is GroundedFighterDeadState)
            {
                var state = s as FighterState;
                if (state.fsm.fighter == _matchManager.fighters[_id])
                    _percentBreak.Play();
            }
            if (s is GroundedFighterShieldBreakState)
            {
                var state = s as FighterState;
                if (state.fsm.fighter == _matchManager.fighters[_id])
                    _staminaBreak.Play();
            }
        }
    }
    
    [System.Serializable]
    public class FloatEvent : UnityEvent<float>
    {
    }}
