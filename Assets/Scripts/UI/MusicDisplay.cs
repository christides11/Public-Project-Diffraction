namespace TightStuff.UI
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;
    
    public class MusicDisplay : MonoBehaviour
    {
        private Animator _anim;
    
        [SerializeField]
        private List<Text> _musicNames;
        [SerializeField]
        private List<Text> _musicianNames;
    
        // Start is called before the first frame update
        void Awake()
        {
            _anim = GetComponent<Animator>();
    
            MusicPlayer.MusicStartAction += OnMusicStart;
        }
    
        private void OnDestroy()
        {
            MusicPlayer.MusicStartAction -= OnMusicStart;
        }
    
        private void OnMusicStart(MusicObject music)
        {
            _anim.enabled = true;
            foreach (var text in _musicNames)
                text.text = music.musicName;
            foreach (var text in _musicianNames)
                text.text = music.composerName + " ";
            _anim.Play("MusicSlideIn");
        }
    }
}
