namespace TightStuff.Aesthetics
{
    using UnityEngine;
    
    public class MatchPlayerColorSprite : MonoBehaviour
    {
        [SerializeField]
        private float _alpha = 1;
    
        [SerializeField]
        private Controller _player;
    
        private void Start()
        {
            var a = GetComponent<SpriteRenderer>();
            var color = _player.playerColor;
            color.a = _alpha;
            a.color = color;
        }
    }
}
