namespace TightStuff
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using System;
    
    public class Grazer : Hitter
    {
        [SerializeField]
        private Hitter hitter;
    
        protected override void Start()
        {
            base.Start();
            if (hitter == null)
                hitter = GetComponent<Hitter>();
            order++;
            foreach (GrazeBox gz in hitboxes)
                OnHitRemove.AddListener(MatchGrazePointAndRemove);
        }
    
        public override void GUpdate()
        {
            foreach (var hit in hitboxes)
                CheckObjectHitAndRemoveGraze(hit as GrazeBox);
            base.GUpdate();
        }
    
        private void CheckObjectHitAndRemoveGraze(GrazeBox graze)
        {
            var tempGraze = graze.grazePoints;
            foreach (HitObject hitObject in hitter.hitObjects)
            {
                for (int i = 0; i < graze.grazePoints.Count; i++)
                {
                    GrazePoint grazePoint = graze.grazePoints[i];
                    if (grazePoint.fighter.entity == hitObject.box.owner && !(hitObject.box is HitBox))
                    {
                        grazePoint.DestroyGraze();
                        tempGraze.Remove(grazePoint);
                    }
                }
            }
            graze.grazePoints = tempGraze;
        }
    
        protected override void ActOnHit(HitObject obj)
        {
            if (obj.hitTimer == obj.hitbox.hitProperties.hitCoolDown)
            {
                obj.hitbox.OnHit.Invoke(obj);
            }
        }
    
        public void MatchGrazePointAndRemove(HitObject hit)
        {
            if (!(hit.hitbox is GrazeBox))
                return;
            var grazebox = hit.hitbox as GrazeBox;
            var tempGraze = grazebox.grazePoints;
            for (int i = 0; i < grazebox.grazePoints.Count; i++)
            {
                GrazePoint grazePoint = grazebox.grazePoints[i];
                if (grazePoint.fighter.entity == hit.box.owner)
                {
                    grazePoint.GiftGraze();
                    tempGraze.Remove(grazePoint);
                }
            }
            grazebox.grazePoints = tempGraze;
        }
    }
}
