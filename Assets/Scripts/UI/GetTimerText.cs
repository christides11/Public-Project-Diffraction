namespace TightStuff.UI
{
    using UnityEngine.UI;
    using UnityEngine;
    using System;
    using TMPro;
    
    public class GetTimerText : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI _selfText;
    
        private void Start()
        {
            _selfText = GetComponent<TextMeshProUGUI>();
        }
    
        // Update is called once per frame
        void Update()
        {
            _selfText.text = "<mspace=mspace=18>" + (TimeSpan.FromSeconds(MatchManager.currentTime).ToString("mm'</mspace><mspace=mspace=9>:</mspace><mspace=mspace=18>'ss'</mspace><mspace=mspace=9>:</mspace><mspace=mspace=18>'ff")) + "</mspace>";
        }
    }
}
