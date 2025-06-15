namespace TightStuff.Menu
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Localization.Settings;
    using UnityEngine.UI;
    
    public class FighterInfo : MonoBehaviour
    {
        [SerializeField]
        public FighterProperties _ft;
    
        public List<Text> fighterShortName;
        public List<Image> fighterFacePortrait;
    
        // Start is called before the first frame update
        void Start()
        {
            foreach (var fightername in fighterShortName)
                fightername.text = LocalizationSettings.StringDatabase.GetLocalizedString("Name", _ft.name + "Given").ToUpper();
            foreach (var facePortrait in fighterFacePortrait)
                facePortrait.sprite = _ft.palette[0].portrait;
        }
    
        // Update is called once per frame
        void Update()
        {
            foreach (var fightername in fighterShortName)
                fightername.text = LocalizationSettings.StringDatabase.GetLocalizedString("Name", _ft.name + "Given").ToUpper();
            foreach (var facePortrait in fighterFacePortrait)
                facePortrait.sprite = _ft.palette[0].portrait;
        }
    }
}
