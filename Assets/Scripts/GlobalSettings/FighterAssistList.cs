using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace TightStuff
{
    public class FighterAssistList : MonoBehaviour
    {
        public static FighterAssistList instance;

        [SerializeField]
        private List<FighterProperties> _fighterProperties;
        [SerializeField]
        private List<AssistProperties> _assistProperties;

        public List<AssistProperties> AssistProperties => _assistProperties;
        public List<FighterProperties> FighterProperties => _fighterProperties;

        public AudioMixerGroup effectMixerGroup;

        // Start is called before the first frame update
        void Awake()
        {
            if (instance == null)
                instance = this;
            else
            {
                DestroyImmediate(gameObject);
                return;
            }
        }
        private void OnDestroy()
        {
            instance = null;
        }
    }
}
