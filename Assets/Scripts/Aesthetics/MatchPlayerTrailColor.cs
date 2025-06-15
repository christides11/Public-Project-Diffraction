namespace TightStuff.Aesthetics
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    
    public class MatchPlayerTrailColor : MonoBehaviour
    {
        [SerializeField]
        private float _alpha = 1;
    
        [SerializeField]
        private Controller _player;
    
        private void Start()
        {
            var a = GetComponent<TrailRenderer>();
            var color = _player.playerColor;
            color.a = _alpha;
            a.endColor = color;
        }
    }
}
