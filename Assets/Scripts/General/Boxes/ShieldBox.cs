namespace TightStuff
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    
    public class ShieldBox : InteractBox
    {
        protected override void Start()
        {
            base.Start();
            lateOrder += 1;
            OnHitted.AddListener(MatchFreeze);
            OnHitted.AddListener(MatchLaunch);
        }
    
        private void MatchFreeze(HitObject hit)
        {
            owner.stateVars.freezeTime = entity.stateVars.freezeTime;
            entity.stateVars.freezeTime = 0;
        }
        private void MatchLaunch(HitObject hit)
        {
            owner.stateVars.indieSpd = entity.stateVars.indieSpd;
            entity.stateVars.indieSpd *= 0;
        }
    }
}
