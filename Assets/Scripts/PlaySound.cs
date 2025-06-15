namespace TightStuff
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    
    public class PlaySound : MonoBehaviour
    {
        private AudioSource _audio;
        void Start()
        {
            _audio = GetComponent<AudioSource>();
        }
    
        private void Play()
        {
            _audio.Play();
        }
        private void Deactivate()
        {
            gameObject.SetActive(false);
        }
    }
}
