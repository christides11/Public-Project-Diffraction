using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

namespace TightStuff
{
    public class MusicItemDisplay : MonoBehaviour
    {
        public Text musicName;
        public Text musicNameOG;

        public Text artistName;

        public MusicObject musicObject;
        public MusicPlayer player;

        public Transform musicCDAnchor;

        // Start is called before the first frame update
        void Start()
        {
            musicName.text = musicObject.musicName;
            musicNameOG.text = LocalizationSettings.StringDatabase.GetLocalizedString("OGMusicNames", musicObject.originalTrack);
            artistName.text = musicObject.composerName;
        }

        // Update is called once per frame
        void Update()
        {
        
        }

        public void PlayMusic()
        {
            player.StartMusic(musicObject);
        }
    }
}
