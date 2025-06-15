namespace TightStuff.Menu
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.Localization.Settings;
    using UnityEngine.UI;
    
    public class PlayerSelectAestheticController : MonoBehaviour
    {
        public ActivatedQuardrant quadrant;
        public Outline playerOutlineColor;
        public Text playerRepText;
    
        public List<SliderIntToTextChange> sliderToTextChange;
        public SliderIntToSpriteChange sliderToImageChange;
        public List<SliderIntToAssistChange> assistTarget;
        public List<ProfileManager> profiles;
        public UnityEvent OnP3Exist;
        public UnityEvent OnP3Gone;

        public UnityEvent<int> OnPaletteChange;
        public UnityEvent<int, Color> OnColorQuadrantChange;
        public UnityEvent<Color> OnColorChange;

        public PlayerTag profile;
        public TextAsset defaultProfile;

        public int PlayerSlot => quadrant.currentQuardrant;
        public int FighterPrefabId => (int)sliderToImageChange.control.value - 1;
        public int PlayerPalette => sliderToImageChange.currentPalette;
        public Color PlayerColor => playerOutlineColor.effectColor;
        public int SelectedAssist => assistTarget[quadrant.currentQuardrant].assistTarget.currentSelection - 1;
    
        [System.Serializable]
        public class SliderIntToTextChange
        {
            public Slider control;
            public Slider targetSlider;
            public Text target;
            public bool hasZero;
    
            public int value;
    
            public UnityEvent OnValueChanged;
        }
    
        [System.Serializable]
        public class SliderIntToSpriteChange
        {
            public Slider control;
            public List<Slider> paletteSlider;
            public List<QuadrantSprites> targets;
            public List<FighterProperties> fp;
            public int currentPalette;
            public int currentColor;
    
            [System.Serializable]
            public class QuadrantSprites
            {
                public List<SpriteRenderer> imageTargets;
                public List<Image> stockTargets;
                public List<Text> textTargets;
                public ShadowColor shadowColor;
                public List<MeshRenderer> CrystalMaterials;
                public UnityEvent OnValueChanged;
            }
        }
        [System.Serializable]
        public class SliderIntToAssistChange
        {
            public Slider control;
            public ScrollAssist assistTarget;
            public UnityEvent OnValueChanged;
            public UnityEvent<bool> OnSelectingAssist;
            public UnityEvent OnConfirmAssist;
        }
    
        public void ReflectCurrentTargetValues()
        {
            foreach (var slider in sliderToTextChange)
            {
                slider.control.value = slider.targetSlider.value;
                slider.value = (int)slider.targetSlider.value;
                slider.control.GetComponent<SliderControl>().ResetOGVal();
            }
        }
        public void ReflectAllOtherProfiles(PlayerTag profile)
        {
            this.profile = profile;
            foreach (var prof in profiles)
                prof.currentProfile = profile;
            profiles[quadrant.currentQuardrant].profileNameNormalText.text = profile.tag;
            playerRepText.text = profile.tag;
            if (profile.tag == "")
                profiles[quadrant.currentQuardrant].profileNameNormalText.text = "P" + (quadrant.currentQuardrant + 1);
        }
        public void UpdateProfileNameView()
        {
            profiles[quadrant.currentQuardrant].profileNameNormalText.text = profile.tag;
            if (profile.tag == "")
                profiles[quadrant.currentQuardrant].profileNameNormalText.text = "P" + (quadrant.currentQuardrant + 1);
        }
    
        public void ConvertSliderValueToText(int i)
        {
            sliderToTextChange[i].targetSlider.value = sliderToTextChange[i].control.value;
            sliderToTextChange[i].value = (int)sliderToTextChange[i].control.value;
            sliderToTextChange[i].target.text = sliderToTextChange[i].control.value.ToString();
            if (sliderToTextChange[i].hasZero)
            {
                if (sliderToTextChange[i].control.value == sliderToTextChange[i].control.maxValue - 1)
                {
                    sliderToTextChange[i].value = 0;
                    sliderToTextChange[i].target.text = "0";
                }
                if (sliderToTextChange[i].control.value == sliderToTextChange[i].control.maxValue - 2)
                {
                    sliderToTextChange[i].value = -1;
                    sliderToTextChange[i].target.text = "\u221E";
                }
            }
            else
            {
                if (sliderToTextChange[i].control.value == sliderToTextChange[i].control.maxValue - 1)
                {
                    sliderToTextChange[i].value = -1;
                    sliderToTextChange[i].target.text = "\u221E";
                }
            }
            sliderToTextChange[i].OnValueChanged?.Invoke();
        }
        public void ConvertSliderValueToImage()
        {
            foreach (var target in sliderToImageChange.targets[quadrant.currentQuardrant].imageTargets)
                target.sprite = sliderToImageChange.fp[(int)sliderToImageChange.control.value].palette[0].portraitFull;
            sliderToImageChange.targets[quadrant.currentQuardrant].imageTargets[0].transform.localPosition = sliderToImageChange.fp[(int)sliderToImageChange.control.value].CharSelectPortraitOffset;
    
            var paletteID = ColorAndPaletteAssigner.instance.GetFighterPaletteID(quadrant.currentQuardrant, (int)sliderToImageChange.control.value);
            FighterProperties.ColorPalettes palette = SetPalette(paletteID);
    
            sliderToImageChange.currentColor = ColorAndPaletteAssigner.instance.GetPlayerColor(quadrant.currentQuardrant, palette.defaultPlayerColorID, palette.backupPlayerColorID);
            SetPlayerColor(ColorAndPaletteAssigner.instance.colors[sliderToImageChange.currentColor]);
    
            foreach (var target in sliderToImageChange.targets[quadrant.currentQuardrant].textTargets)
                target.text = LocalizationSettings.StringDatabase.GetLocalizedString("Name", sliderToImageChange.fp[(int)sliderToImageChange.control.value].name + "Full").ToUpper();
    
            for (int i = 0; i < sliderToImageChange.targets[quadrant.currentQuardrant].stockTargets.Count; i++)
            {
                sliderToImageChange.targets[quadrant.currentQuardrant].stockTargets[i].sprite = sliderToImageChange.fp[(int)sliderToImageChange.control.value].palette[i].stockIcon.GetComponent<Image>().sprite;
            }
    
            sliderToImageChange.targets[quadrant.currentQuardrant].OnValueChanged?.Invoke();
        }
        public void ConvertSliderValueToImage(PlayerTag profile)
        {

            if (profile.favouriteFighters == null || profile.favouriteFighters.Count == 0)
            {
                sliderToImageChange.targets[quadrant.currentQuardrant].imageTargets[0].transform.localPosition = new Vector2(1000, 1000);
                sliderToImageChange.currentColor = 9;
                FighterProperties.ColorPalettes palettee = SetPalette(0);
                SetPlayerColor(ColorAndPaletteAssigner.instance.colors[9]);
                foreach (var target in sliderToImageChange.targets[quadrant.currentQuardrant].textTargets)
                    target.text = "";
                return;
            }
            sliderToImageChange.control.value = profile.favouriteFighters[0].fighterID;

            foreach (var target in sliderToImageChange.targets[quadrant.currentQuardrant].imageTargets)
                target.sprite = sliderToImageChange.fp[profile.favouriteFighters[0].fighterID].palette[profile.favouriteFighters[0].favouriteCostumeID].portraitFull;
            sliderToImageChange.targets[quadrant.currentQuardrant].imageTargets[0].transform.localPosition = sliderToImageChange.fp[profile.favouriteFighters[0].fighterID].CharSelectPortraitOffset;

            var paletteID = profile.favouriteFighters[0].favouriteCostumeID;
            FighterProperties.ColorPalettes palette = SetPalette(paletteID);

            sliderToImageChange.currentColor = profile.favouriteFighters[0].colorID;
            SetPlayerColor(ColorAndPaletteAssigner.instance.colors[sliderToImageChange.currentColor]);

            foreach (var target in sliderToImageChange.targets[quadrant.currentQuardrant].textTargets)
                target.text = LocalizationSettings.StringDatabase.GetLocalizedString("Name", sliderToImageChange.fp[profile.favouriteFighters[0].fighterID].name + "Full").ToUpper();

            for (int i = 0; i < sliderToImageChange.targets[quadrant.currentQuardrant].stockTargets.Count; i++)
            {
                sliderToImageChange.targets[quadrant.currentQuardrant].stockTargets[i].sprite = sliderToImageChange.fp[profile.favouriteFighters[0].fighterID].palette[i].stockIcon.GetComponent<Image>().sprite;
            }

            sliderToImageChange.targets[quadrant.currentQuardrant].OnValueChanged?.Invoke();
        }

        private void SetPlayerColor(ColorAndPaletteAssigner.PlayerColor mat)
        {
            foreach (var crystal in sliderToImageChange.targets[quadrant.currentQuardrant].CrystalMaterials)
                crystal.material = mat.gemColor;
            var col = mat.color;
            col.a = 0.5f;
            sliderToImageChange.targets[quadrant.currentQuardrant].shadowColor.SetShadowColor(col);
            if (playerOutlineColor != null)
                playerOutlineColor.effectColor = mat.color;
            OnColorQuadrantChange.Invoke(quadrant.currentQuardrant, mat.color);
            OnColorChange.Invoke(mat.color);
        }
    
        private FighterProperties.ColorPalettes SetPalette(int paletteID)
        {
            sliderToImageChange.currentPalette = paletteID;
            var palette = sliderToImageChange.fp[(int)sliderToImageChange.control.value].palette[paletteID];
            sliderToImageChange.targets[quadrant.currentQuardrant].imageTargets[0].material = palette.colorMat;
            foreach (var slider in sliderToImageChange.paletteSlider)
                slider.value = paletteID + 1;
            return palette;
        }
    
        public void ChangePalette(float val)
        {
            var difference = (int)val - 1 - sliderToImageChange.currentPalette;
            if (difference == 0)
                return;
    
            if (Mathf.Abs(difference) >= 7)
                difference = 1 * -(int)Mathf.Sign(difference);
            //Debug.Log(difference);
            sliderToImageChange.currentPalette = ColorAndPaletteAssigner.instance.GetNextFighterPaletteID(quadrant.currentQuardrant, (int)sliderToImageChange.control.value, sliderToImageChange.currentPalette, difference);
            FighterProperties.ColorPalettes palette = SetPalette(sliderToImageChange.currentPalette);
    
            sliderToImageChange.currentColor = ColorAndPaletteAssigner.instance.GetPlayerColor(quadrant.currentQuardrant, palette.defaultPlayerColorID, palette.backupPlayerColorID);
            SetPlayerColor(ColorAndPaletteAssigner.instance.colors[sliderToImageChange.currentColor]);
        }
    
        public void ResetSliderValueToImage()
        {
            var mat = ColorAndPaletteAssigner.instance.colors[9];
            foreach (var crystal in sliderToImageChange.targets[quadrant.currentQuardrant].CrystalMaterials)
                crystal.material = mat.gemColor;
            var col = mat.color;
            col.a = 0.5f;
            sliderToImageChange.targets[quadrant.currentQuardrant].shadowColor.SetShadowColor(col);
            playerOutlineColor.effectColor = mat.color;
        }
        public void ResetTagColor()
        {
            playerOutlineColor.effectColor = Color.white;
        }
        public void ResetPaletteColor()
        {
            ColorAndPaletteAssigner.instance.UnoccupyPlayer(quadrant.currentQuardrant);
        }
        public void ConvertSliderValueToAssistSelection(float val)
        {
            assistTarget[quadrant.currentQuardrant].assistTarget.ChangeCurrentSelection((int)val);
            assistTarget[quadrant.currentQuardrant].OnValueChanged?.Invoke();
        }
        public void SelectingAssist(bool val)
        {
            assistTarget[quadrant.currentQuardrant].OnSelectingAssist?.Invoke(val);
        }
        public void ConfirmAssist()
        {
            assistTarget[quadrant.currentQuardrant].OnConfirmAssist?.Invoke();
        }
    
        public void ClearName()
        {
            foreach (var target in sliderToImageChange.targets[quadrant.currentQuardrant].textTargets)
                target.text = "";
        }
    
        public void P3Exists()
        {
            OnP3Exist?.Invoke();
        }
        public void P3Gone()
        {
            OnP3Gone?.Invoke();
        }
    
        // Start is called before the first frame update
        void Start()
        {
            profile = PlayerTag.FromJson(defaultProfile.text);
            profile.fighterInUse.fighterID = quadrant.currentQuardrant + 1;
            foreach (var prof in profiles)
            {
                prof.currentProfile = profile;
            }
            ReflectCurrentTargetValues();
        }
    
        // Update is called once per frame
        void Update()
        {
            
        }
    }
}
