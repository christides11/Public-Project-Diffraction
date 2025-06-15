namespace TightStuff
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    
    public class ProjectileAirborneState : AirborneState
    {
        public readonly new ProjectileSM fsm;
        public ProjectileAirborneState(ProjectileSM fsm) : base(fsm)
        {
            this.fsm = fsm;
        }
    
        public override void OnStateUpdate(float time)
        {
            base.OnStateUpdate(time);
            CheckCastCollision();
            CheckPhysicalCollision();
            CheckPlatformCondition();
        }
    
        private void CheckPhysicalCollision()
        {
            if (ContinueCollision)
            {
                if (Vector2.Dot(fsm.entity.GetTotalContactNormals().normalized, IndieSpd.normalized) < 0)
                    fsm.projectile.CollidePhysicalEvent.Invoke(fsm.entity.stateVars.currentCollision);
            }
        }
    
        protected virtual void CheckPlatformCondition()
        {
            if (IndieSpd.y > 0)
                PlatformCondition = false;
            else
                PlatformCondition = true;
        }
    
        private void CheckCastCollision()
        {
            foreach (Collider2D col in fsm.entity.AssociatedColliders)
            {
                if (!col.enabled)
                    continue;
    
                Vector3 origin = fsm.entity.transform.position + fsm.entity.transform.rotation * Vector3.Scale((Vector3)col.offset, fsm.entity.transform.lossyScale);
    
                if (col is CircleCollider2D)
                {
                    var circle = col as CircleCollider2D;
                    var size = circle.radius * fsm.entity.transform.lossyScale.y;
                    Collider2D[] hits = Physics2D.OverlapCircleAll(origin, size, fsm.projectile.collisionLayers);
    
                    RegisterHit(hits);
                }
                else if (col is BoxCollider2D)
                {
                    var box = col as BoxCollider2D;
                    var size = box.size * fsm.entity.transform.lossyScale.y * Vector2.one;
                    Collider2D[] hits = Physics2D.OverlapBoxAll(origin, size, fsm.entity.transform.eulerAngles.z, fsm.projectile.collisionLayers);
    
                    RegisterHit(hits);
    
                }
            }
        }
    
        protected virtual void RegisterHit(Collider2D[] hits)
        {
            foreach (Collider2D hit in hits)
            {
                if (hit.gameObject.layer == 10 && fsm.entity.stateVars.indieSpd.y >= 0)
                    return;
                if (hit.isTrigger)
                    return;
    
                fsm.projectile.CollideCastEvent.Invoke(hit);
            }
        }
    
        protected override void TransitionGround()
        {
            //base.TransitionGround();
        }
    }
    
    public class ProjectileAirborneCollideCastIgnoresPlatformState : ProjectileAirborneState
    {
        public readonly new ProjectileSM fsm;
        public ProjectileAirborneCollideCastIgnoresPlatformState(ProjectileSM fsm) : base(fsm) { }
    
    
        protected override void RegisterHit(Collider2D[] hits)
        {
            foreach (Collider2D hit in hits)
            {
                if (hit.gameObject.layer == 10)
                    return;
                if (hit.isTrigger)
                    return;
    
                fsm.projectile.CollideCastEvent.Invoke(hit);
            }
        }
    }
    
    public class ProjectileAirborneNoPlatformOverrideState : ProjectileAirborneState
    {
        public ProjectileAirborneNoPlatformOverrideState(ProjectileSM fsm) : base(fsm) { }
        protected override void CheckPlatformCondition()
        {
        }
    }
    
    public class ProjectileAirborneControllableState : ProjectileAirborneState
    {
        public ProjectileAirborneControllableState(ProjectileSM fsm) : base(fsm) { }
    
        public override void OnStateUpdate(float time)
        {
            base.OnStateUpdate(time);
            if (fsm.projectile.controlling.specialStick.raw.magnitude > 0.3f)
            {
                if (IndieSpd.magnitude <= fsm.entity.et.airSpd)
                    IndieSpd += Mathf.Pow(fsm.entity.et.airAcl, 2) * 0.1f * fsm.projectile.controlling.cStick.raw;
                else
                    IndieSpd = fsm.entity.et.airSpd * (fsm.projectile.controlling.specialStick.raw * 0.25f + IndieSpd.normalized).normalized;
            }
            else if (fsm.projectile.controlling.moveStick.raw.magnitude > 0.3)
            {
                if (IndieSpd.magnitude <= fsm.entity.et.airSpd)
                    IndieSpd += Mathf.Pow(fsm.entity.et.airAcl, 2) * 0.1f * fsm.projectile.controlling.moveStick.raw;
                else
                    IndieSpd = fsm.entity.et.airSpd * (fsm.projectile.controlling.moveStick.raw * 0.25f + IndieSpd.normalized).normalized;
            }
            else
            {
                IndieSpd *= 0.75f;
            }
        }
    }
    public class ProjectileAirborneControllableHorizontalState : ProjectileAirborneState
    {
        public ProjectileAirborneControllableHorizontalState(ProjectileSM fsm) : base(fsm) { }
    
        public override void OnStateUpdate(float time)
        {
            base.OnStateUpdate(time);
            if (fsm.projectile.controlling.specialStick.raw.magnitude > 0.3f)
            {
                if (IndieSpd.magnitude <= fsm.entity.et.airSpd)
                    IndieSpd += Mathf.Pow(fsm.entity.et.airAcl, 2) * 0.1f * fsm.projectile.controlling.specialStick.raw * Vector2.right;
                else
                    IndieSpd = fsm.entity.et.airSpd * (fsm.projectile.controlling.cStick.raw * Vector2.right * 0.25f + IndieSpd.normalized).normalized;
            }
            else if (fsm.projectile.controlling.moveStick.raw.magnitude > 0.3)
            {
                if (IndieSpd.magnitude <= fsm.entity.et.airSpd)
                    IndieSpd += Mathf.Pow(fsm.entity.et.airAcl, 2) * 0.1f * fsm.projectile.controlling.moveStick.raw * Vector2.right;
                else
                    IndieSpd = fsm.entity.et.airSpd * (fsm.projectile.controlling.moveStick.raw * Vector2.right * 0.25f + IndieSpd.normalized).normalized;
            }
        }
    }
    public class ProjectileAirborneRemoteAimState : ProjectileAirborneState
    {
        public ProjectileAirborneRemoteAimState(ProjectileSM fsm) : base(fsm) { }
    
        public override void OnStateUpdate(float time)
        {
            base.OnStateUpdate(time);
            var SpecialAim = (Vector2)fsm.entity.transform.up;
            if (fsm.projectile.controlling.cStick.raw.magnitude > 0.8f)
            {
                SpecialAim = fsm.projectile.controlling.cStick.raw;
            }
            else if (fsm.projectile.controlling.moveStick.raw.magnitude > 0.8f)
            {
                SpecialAim = fsm.projectile.controlling.moveStick.raw;
            }
    
            fsm.entity.transform.rotation = Quaternion.Lerp(fsm.entity.transform.rotation, Quaternion.Euler(0, 0, Mathf.Atan2(SpecialAim.y, SpecialAim.x) * Mathf.Rad2Deg - 90), time * Et.runAcl * Time.fixedDeltaTime);
    
    
            if ((!fsm.projectile.controlling.specialButton.raw && fsm.projectile.controlling.cStick.raw.magnitude <= 0.8f) && fsm.entity.stateVars.frameNum > 100 && fsm.entity.stateVars.frameNum < 600)
                fsm.entity.SkipCurrentAnimToFrame(610);
        }
        protected override void RegisterHit(Collider2D[] hits)
        {
            foreach (Collider2D hit in hits)
            {
                if (hit.gameObject.layer == 10)
                    return;
                if (hit.isTrigger)
                    return;
    
                fsm.projectile.CollideCastEvent.Invoke(hit);
            }
        }
    }
    public class ProjectileAirbornePropelForwardState : ProjectileAirborneState
    {
        public ProjectileAirbornePropelForwardState(ProjectileSM fsm) : base(fsm) { }
    
        public override void OnStateUpdate(float time)
        {
            base.OnStateUpdate(time);
            IndieSpd += Et.airAcl * (Vector2)fsm.entity.transform.up * time * Time.fixedDeltaTime;
        }
    }
    
    
    public class ProjectileGroundedState : GroundedState
    {
        public ProjectileGroundedState(ActionSM fsm) : base(fsm)
        {
        }
    }
    
    public class ProjectileAirborneMoveStraightState : ProjectileAirborneState
    {
        public ProjectileAirborneMoveStraightState(ProjectileSM fsm) : base(fsm) { }
    
        public override void OnStateUpdate(float time)
        {
            base.OnStateUpdate(time);
            WindSpd = Et.airSpd * fsm.entity.transform.up;
        }
    }}
