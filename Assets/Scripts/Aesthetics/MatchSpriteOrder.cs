namespace TightStuff.Aesthetics
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    
    public class MatchSpriteOrder : MonoBehaviour
    {
        [SerializeField]
        private SpriteRenderer _sprite;
    
        [SerializeField]
        private int _order;
    
        // Update is called once per frame
        void Update()
        {
            _sprite.sortingOrder = _order;
        }
    }
}
