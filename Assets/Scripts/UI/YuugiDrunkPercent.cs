namespace TightStuff.UI
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;
    
    public class YuugiDrunkPercent : MonoBehaviour
    {
        private FighterStatus _fighterUI;
        private RectTransform _rect;
        private Extension.YuugiFighterExtension _yuugi;
    
        [SerializeField]
        private Text _count;
        [SerializeField]
        private Text _countShadow;
    
        [SerializeField]
        private Text _count2;
        [SerializeField]
        private Text _countShadow2;
    
        // Start is called before the first frame update
        void Start()
        {
            _rect = GetComponent<RectTransform>();
    
            _fighterUI = transform.parent.GetComponent<FighterStatus>();
            transform.localScale = Vector3.one;
    
            _yuugi = _fighterUI.FighterExtension as Extension.YuugiFighterExtension;
        }
    
        // Update is called once per frame
        void LateUpdate()
        {
            _count.text = ((int)(_yuugi.DrunkPercent % 10)).ToString();
            _countShadow.text = ((int)(_yuugi.DrunkPercent % 10)).ToString();
    
            int seconddigit = (int)(_yuugi.DrunkPercent / 10);
            _count2.text = seconddigit == 0 ? "" : seconddigit.ToString();
            _countShadow2.text = seconddigit == 0 ? "" : seconddigit.ToString();
        }
    }
}
