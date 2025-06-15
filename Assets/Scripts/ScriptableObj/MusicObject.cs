namespace TightStuff
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    [CreateAssetMenu(fileName = "New Music", menuName = "Scriptable Object/Music")]
    public class MusicObject : ScriptableObject
    {
        [TextArea]
        public string musicName;
        public string originalTrack;
        public string affliateName;
        public Sprite affliateIcon;
        public string composerName;
        public AudioClip musicStart;
        public AudioClip musicLoop;
    
        public float defaultVolume = 1;
    }
}
