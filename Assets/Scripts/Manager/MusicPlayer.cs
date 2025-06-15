namespace TightStuff
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    
    public class MusicPlayer : MonoBehaviour
    {
        public static Action<MusicObject> MusicStartAction;
    
        public MusicObject currentMusic;
        public float currentVolume = 0.3f;
        public AudioSource musicSource;
    
        public float fadeSpd = 5;
    
        private float targetVol;

        public bool playOnStartUp;
    
        private void Start()
        {
            musicSource.ignoreListenerPause = true;
            if (playOnStartUp)
                StartMusic();
        }
    
        private void FixedUpdate()
        {
            musicSource.volume = Mathf.Lerp(musicSource.volume, targetVol, fadeSpd * 0.1f);
        }
    
        // Start is called before the first frame update
        public void StartMusic(MusicObject music)
        {
            currentMusic = music;

            musicSource.Stop();

            targetVol = music.defaultVolume * currentVolume;
            musicSource.volume = music.defaultVolume * currentVolume;
            musicSource.clip = currentMusic.musicLoop;
            musicSource.PlayOneShot(currentMusic.musicStart);
            musicSource.PlayScheduled(AudioSettings.dspTime + currentMusic.musicStart.length);
            if (MusicStartAction != null)
                MusicStartAction.Invoke(music);
        }
        public void StartMusic()
        {
            StartMusic(currentMusic);
        }
        public void SetTargetVolume(float val)
        {
            targetVol = currentMusic.defaultVolume * val;
        }
    }}
