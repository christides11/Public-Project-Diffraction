namespace TightStuff.Aesthetics
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    
    public class MasterSparkGetSFX : MonoBehaviour
    {
        private BaseFighterBehavior _fighter;
    
        [SerializeField]
        private HitBox _hitbox;
    
        // Start is called before the first frame update
        void Start()
        {
            GetComponent<BaseProjectileBehaviour>().ShootEvent.AddListener(AddSound);
        }
    
        private void AddSound()
        {
            _fighter = _hitbox.owner.gameObject.GetComponent<BaseFighterBehavior>();
            _hitbox.OnHit.AddListener(_fighter.extension.RandomRangeSound);
        }
    }
}
