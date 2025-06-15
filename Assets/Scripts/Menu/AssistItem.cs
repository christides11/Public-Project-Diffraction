namespace TightStuff.Menu
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;
    using UnityEngine.Localization.Settings;
    
    public class AssistItem : MonoBehaviour
    {
        [SerializeField]
        public AssistProperties assist;
        [SerializeField]
        public ReadyFlash flash;
    
        [SerializeField]
        private Image _icon;
        [SerializeField]
        private Image _iconBG;
        [SerializeField]
        private Image _mascot;
        [SerializeField]
        private Image _hourGlass;
        [SerializeField]
        private Image _border;
        [SerializeField]
        private Text _cooldown;
        [SerializeField]
        private Text _name;
        [SerializeField]
        private Image _background;
    
        [SerializeField]
        private Color _highlightedColor;
        [SerializeField]
        private Color _dormantColor;
    
        // Start is called before the first frame update
        void Start()
        {
            ChangeUI();
            ChangeUIVisibility(0.5f);
            flash.SetAlpha(0.6f);
            UnhighlightItem();
        }
    
        public void ChangeUI()
        {
            if (assist == null)
            {
                _icon.sprite = null;
                _icon.color = new Color();
                _iconBG.color = new Color();
                _mascot.sprite = null;
                _mascot.color = new Color();
                _hourGlass.color = new Color();
                _border.color = new Color();
                _background.color = new Color();
                _cooldown.text = "";
                _name.text = "";
                return;
            }
            _hourGlass.color = Color.white;
            _border.color = Color.white;
            var a = Color.black;
            a.a = 0.23f;
            _background.color = a;
    
            if (assist.Icon != null)
            {
                _icon.sprite = assist.Icon;
                _icon.color = Color.white;
            }
            _iconBG.color = assist.IconColor;
            if (assist.Mascot != null)
            {
                _mascot.color = Color.white;
                _mascot.sprite = assist.Mascot;
            }
            _cooldown.text = "NONE ";
            if (assist.CoolDown != 0)
                _cooldown.text = assist.CoolDown.ToString();
            _name.text = "";
            if (assist.name != "None")
                _name.text = LocalizationSettings.StringDatabase.GetLocalizedString("AssistName", assist.name).ToUpper();
        }
    
        public void ChangeUIVisibility(float val)
        {
            if (assist == null)
            {
                _icon.sprite = null;
                _icon.color = new Color();
                _mascot.sprite = null;
                _mascot.color = new Color();
                _hourGlass.color = new Color();
                _border.color = new Color();
                _cooldown.text = "";
                _name.text = "";
                return;
            }
            var color = Color.white;
            color.a = val;
            _hourGlass.color = color;
            _border.color = color;
            _icon.color = color;
            if (_mascot.sprite != null)
                _mascot.color = color;
            _cooldown.color = color;
            _name.color = color;
    
            var bgcolor = assist.IconColor;
            bgcolor.a = val;
            _iconBG.color = bgcolor;
        }
    
        public void HighlightItem()
        {
            flash.SetBothFlashColors(_highlightedColor);
        }
    
        public void UnhighlightItem()
        {
            flash.SetBothFlashColors(_dormantColor);
        }
        public void SelectItem()
        {
            flash.SetBothFlashColors(_dormantColor);
            flash.SetAlphaInstantly(1);
            flash.SetAlpha(0.62f);
            flash.SetLerpSpeed(5);
        }
        public void DeselectItem()
        {
            flash.SetLerpSpeed(300);
        }

        public void FetchAssistProperty(int i)
        {
            assist = FighterAssistList.instance.AssistProperties[i];
        }
    }
}
