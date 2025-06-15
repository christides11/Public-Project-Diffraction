using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TightStuff.Menu;
using UnityEngine;
using UnityEngine.UI;

namespace TightStuff.Menu
{
    public class ProfileDisplay : MonoBehaviour, IGridViewable
    {
        [SerializeField]
        public PlayerTag profile;

        [SerializeField]
        private Image _fighterPortrait;
        [SerializeField]
        private Text _profileName;
        [SerializeField]
        private Image _figherName;
        [SerializeField]
        private Image _figherNameBG;
        [SerializeField]
        private Image _profileBG;
        [SerializeField]
        private Image _profileHighlightBG;

        [SerializeField]
        public AssistItem assistItem;

        [SerializeField]
        private RectTransform _stockList;

        [SerializeField]
        private Sprite _noSprite;

        private Color _OGColor;

        public ProfileManager profileManager;
        public GroupMenuBehaviour subMenu;
        public Button editButton;
        public Button deleteButton;

        public bool concise;

        public int profileOrder;

        // Start is called before the first frame update
        void Start()
        {
            UpdateView();
        }

        public void SetProfile(PlayerTag profile)
        {
            this.profile = profile;
            UpdateView();
        }

        private void UpdateView()
        {
            _profileName.text = profile.tag;
            _OGColor = _figherNameBG.color;

            if (profile.fighterInUse == null)
            {
                if (profile.favouriteFighters == null || profile.favouriteFighters.Count <= 0)
                {
                    var a = Color.white;
                    a.a = 0;
                    _fighterPortrait.color = a;
                    _figherNameBG.color = a;
                    _figherName.color = a;
                    return;
                }
            }

            _figherNameBG.color = _OGColor;
            PlayerTag.FavouriteFighter fighterToDisplay;
            if (profile.fighterInUse != null)
                fighterToDisplay = profile.fighterInUse;
            else
                fighterToDisplay = profile.favouriteFighters[0];

            _figherName.color = (fighterToDisplay.tagColor + Color.white) / 2;

            var bgColor = fighterToDisplay.tagColor / 2;
            bgColor.a = _profileBG.color.a;
            _profileBG.color = bgColor;
            bgColor = (fighterToDisplay.tagColor * 3 + Color.white) / 4;
            //bgColor.a = _profileHighlightBG.color.a;
            _profileHighlightBG.color = bgColor;

            if (assistItem != null && profile != null)
            {
                assistItem.FetchAssistProperty(profile.assistInUse);
                assistItem.ChangeUI();
            }

            _fighterPortrait.color = Color.white;
            _fighterPortrait.sprite = FighterAssistList.instance.FighterProperties[fighterToDisplay.fighterID].palette[fighterToDisplay.favouriteCostumeID].portrait;
            _figherName.sprite = FighterAssistList.instance.FighterProperties[fighterToDisplay.fighterID].nameJap;
            _figherNameBG.sprite = FighterAssistList.instance.FighterProperties[fighterToDisplay.fighterID].nameJapBG;

            if (concise && profile.favouriteFighters != null && profile.favouriteFighters.Count != 0)
            {
                var stockIcon = Instantiate(FighterAssistList.instance.FighterProperties[fighterToDisplay.fighterID].palette[fighterToDisplay.favouriteCostumeID].stockIcon);
                stockIcon.transform.SetParent(_stockList);
                if (stockIcon.TryGetComponent(out RectTransform rectTransform))
                    rectTransform.localScale = Vector3.one;
            }
            if (profile.favouriteFighters == null)
                return;

            if (profile.favouriteFighters.Count <= 1)
                return;
            for (int i = 1; i < profile.favouriteFighters.Count; i++)
            {
                var stockIcon = Instantiate(FighterAssistList.instance.FighterProperties[profile.favouriteFighters[i].fighterID].palette[profile.favouriteFighters[i].favouriteCostumeID].stockIcon);
                stockIcon.transform.SetParent(_stockList);
                if (stockIcon.TryGetComponent(out RectTransform rectTransform))
                    rectTransform.localScale = Vector3.one;
            }
        }

        public void SetProfile()
        {
            profileManager.SetProfile(profile);
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void DeleteFile()
        {
            Regex rgx = new Regex("[^a-zA-Z0-9]");

            var fileName = profile.id.ToString();
            if (!string.IsNullOrEmpty(profile.tag))
                fileName = rgx.Replace(profile.tag, "_") + profile.id.ToString();
            PlayerTag.DeleteFile("PlayerProfiles/" + fileName + ".json");
            Destroy(gameObject);
        }

        public int ViewID()
        {
            return profileOrder;
        }
    }
}
