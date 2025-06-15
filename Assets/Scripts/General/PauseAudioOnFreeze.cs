namespace TightStuff
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    
    public class PauseAudioOnFreeze : UpdateAbstract
    {
        private AudioSource _audioSource;
        [SerializeField]
        public Entity entity;
    
        // Start is called before the first frame update
        void Start()
        {
            _audioSource = GetComponent<AudioSource>();
            if (entity == null)
                entity = GetComponent<Entity>();
            if (entity == null)
                entity = GetComponentInParent<Entity>();
            if (entity == null)
                entity = GetComponentInChildren<Entity>();
        }
    
        // Update is called once per frame
        public override void GUpdate()
        {
            if (_audioSource == null)
                return;
            if (MatchManager.worldTime <= 0)
                _audioSource.Pause();
            else
                _audioSource.UnPause();
    
            if (entity == null)
                return;
            if (entity.NonFreezeTimeScale <= 0)
                _audioSource.Pause();
            else
                _audioSource.UnPause();
        }
    }
}
