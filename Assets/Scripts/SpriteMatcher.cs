namespace TightStuff
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    
    public class SpriteMatcher : UpdateAbstract
    {
        [SerializeField]
        private SpriteRenderer _refSprite;
        private SpriteRenderer _sprite;
    
        [SerializeField]
        private int _orderOffset = -1;
    
        void Start()
        {
            lateOrder = 10000000;
            _sprite = GetComponent<SpriteRenderer>();
        }
    
        // Update is called once per frame
        public override void LateGUpdate()
        {
            _sprite.sprite = _refSprite.sprite;
            _sprite.renderingLayerMask = _refSprite.renderingLayerMask;
            _sprite.sortingOrder = _refSprite.sortingOrder + _orderOffset;
        }
    }
}
