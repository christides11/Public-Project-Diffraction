namespace TightStuff.Aesthetics
{
    using UnityEngine;
    using UnityEngine.Rendering.Universal;
    
    public class MatchPlayerColorLight : MonoBehaviour
    {
        [SerializeField]
        private Controller _player;
    
        private void Start()
        {
            var a = GetComponent<Light2D>();
            var color = _player.playerColor;
            a.color = color;
        }
    }
}
