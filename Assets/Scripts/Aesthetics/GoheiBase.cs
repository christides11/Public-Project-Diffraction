namespace TightStuff.Aesthetics
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    
    public class GoheiBase : MonoBehaviour
    {
        [SerializeField]
        private Entity _owner;
        [SerializeField]
        private List<Transform> _hook;
        [SerializeField]
        private List<SpriteRenderer> _sprite;
    
        [SerializeField]
        private List<Gohei> _parts;
    
        [SerializeField]
        private int _spriteOrder;
        [SerializeField]
        private Vector2 _windDirection = Vector2.up + Vector2.left * 0.3f;
        [SerializeField]
        private float _oscilationRate = 6;
        [SerializeField]
        private bool _visible = true;
    
        // Start is called before the first frame update
        void Start()
        {
            foreach (var hook in _hook)
            {
                if (hook.parent.parent != null)
                    hook.parent.parent = null;
            }
        }
    
        // Update is called once per frame
        void Update()
        {
            if (MatchManager.paused)
                return;
            _windDirection = new Vector2(transform.lossyScale.x < 0? Mathf.Abs(_windDirection.x) : -Mathf.Abs(_windDirection.x), _windDirection.y);
    
            foreach (var part in _parts)
            {
                part.gravityDirection = _windDirection.normalized;
                part.oscilateRate = _oscilationRate;
            }
    
            foreach(var hook in _hook)
                hook.position = transform.position;
    
            if (_sprite[1].sortingOrder < 100)
                _sprite[1].color = _sprite[0].color;
            else
                _sprite[1].color = Color.white;
    
            for (var i = 0; i < _sprite.Count; i++)
            {
                _sprite[i].sortingOrder = _spriteOrder + i;
                _sprite[i].enabled = _visible;
                var alpha = _sprite[i].color;
                alpha.a = (_owner.AssociatedRenderers[0] as SpriteRenderer).color.a;
                _sprite[i].color = alpha;
            }
        }
    
        public void FlickGohei(float x, float y)
        {
            var xFlip = transform.lossyScale.x < 0? -x : x;
            foreach (var part in _parts)
            {
                part.Flick(new Vector2(xFlip, y));
                part.transform.position = transform.position;
            }
        }
    }
}
