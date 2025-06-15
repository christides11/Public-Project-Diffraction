namespace TightStuff.Menu
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;
    using UnityEngine.EventSystems;// Required when using Event data.
    using UnityEngine.Events;
    using UnityEngine.Localization.Settings;
    
    public class FighterSelectable : MonoBehaviour, ISelectHandler
    {
        [SerializeField]
        private List<SpriteRenderer> _fighterPortraits;
        [SerializeField]
        private List<Text> _fighterNames;
        [SerializeField]
        private ShadowColor _tagShadow;
    
        [SerializeField]
        private FighterProperties _fighterProperties;
        public UnityEvent<BaseEventData> OnSelection;
    
        [SerializeField]
        private ChangeCrystalMaterial _crystalMat;
        [SerializeField]
        private Material _ogMat;
    
        public int playerID;
    
        private Vector2 _ogPortraitPos;
    
        public void OnSelect(BaseEventData eventData)
        {
            var name = LocalizationSettings.StringDatabase.GetLocalizedString("Name", _fighterProperties.name + "Full").ToUpper();
            if (_fighterNames[0].text == name)
                return;
    
            CharacterSelectManager.instance.player[playerID].currentFighter = _fighterProperties;
            OnSelection?.Invoke(eventData);
    
            var colors = CharacterSelectManager.instance.AssignPaletteAndPlayerColor(_fighterProperties, playerID);
            _fighterPortraits[0].material = _fighterProperties.palette[colors[0]].colorMat;
            foreach (var fighterPortrait in _fighterPortraits)
                fighterPortrait.sprite = _fighterProperties.palette[colors[0]].portraitFull;
            foreach (var fighterName in _fighterNames)
                fighterName.text = name;
    
            var c = CharacterSelectManager.instance.PlayerColors[colors[1]].color;
            c.a = 0.5f;
            _fighterPortraits[0].transform.localPosition = _fighterProperties.CharSelectPortraitOffset + _ogPortraitPos;
            _crystalMat.ChangeCrystalMat(CharacterSelectManager.instance.PlayerColors[colors[1]].gemColor);
            _tagShadow.SetShadowColor(c);
    
            //throw new System.NotImplementedException();
        }
    
        public void ClearSelect()
        {
            var c = Color.black;
            c.a = 0.5f;
            _fighterPortraits[0].transform.localPosition *= 0;
            _crystalMat.ChangeCrystalMat(_ogMat);
            _tagShadow.SetShadowColor(c);
            foreach (var fighterPortrait in _fighterPortraits)
                fighterPortrait.sprite = null;
            foreach (var fighterName in _fighterNames)
                fighterName.text = "";
        }
    
        // Start is called before the first frame update
        void Start()
        {
            _ogPortraitPos = _fighterPortraits[0].transform.localPosition;
        }
    
        // Update is called once per frame
        void Update()
        {
            
        }
    }
}
