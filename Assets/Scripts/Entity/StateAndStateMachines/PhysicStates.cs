namespace TightStuff
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    
    public abstract class PhysicState : State
    {
        public readonly PhysicSM psm;
        
        public EntityProperties Et { get => psm.entity.et; }
    
        public PhysicState(PhysicSM psm) : base(psm)
        {
            this.psm = psm;
        }
        protected Vector2 IndieSpd { get => psm.entity.stateVars.indieSpd; set => psm.entity.stateVars.indieSpd = value; }
        protected Vector2 SelfSpd { get => psm.entity.stateVars.selfSpd; set => psm.entity.stateVars.selfSpd = value; }
        protected Vector2 WindSpd { get => psm.entity.stateVars.windSpd; set => psm.entity.stateVars.windSpd = value; }
        protected Vector2 ExternalSpd { get => psm.entity.stateVars.externalSpd; set => psm.entity.stateVars.externalSpd = value; }
        protected bool Aerial { get => psm.entity.stateVars.aerial; set => psm.entity.stateVars.aerial = value; }
        protected bool Gravity { get => psm.entity.stateVars.gravity; set => psm.entity.stateVars.gravity = value; }
        protected bool FlippedLeft { get => psm.entity.stateVars.flippedLeft; set => psm.entity.stateVars.flippedLeft = value; }
        protected bool EnteredCollision { get => psm.entity.stateVars.enteredCollision; set => psm.entity.stateVars.enteredCollision = value; }
        protected bool ContinueCollision { get => psm.entity.stateVars.continueCollision; set => psm.entity.stateVars.continueCollision = value; }
        protected bool ExitedCollision { get => psm.entity.stateVars.exitedCollision; set => psm.entity.stateVars.exitedCollision = value; }
        protected int FrameNum { get => psm.entity.stateVars.frameNum; set => psm.entity.stateVars.frameNum = value; }
        protected int AnimID { get => psm.entity.stateVars.animID; set => psm.entity.stateVars.animID = value; }
        protected float SelfTime { get => psm.entity.stateVars.selfTime; set => psm.entity.stateVars.selfTime = value; }
        protected float GivenTime { get => psm.entity.stateVars.givenTime; set => psm.entity.stateVars.givenTime = value; }
        protected float FreezeTime { get => psm.entity.stateVars.freezeTime; set => psm.entity.stateVars.freezeTime = value; }
        protected float Percent { get => psm.entity.stateVars.percent; set => psm.entity.stateVars.percent = value; }
        protected State ActState { get => psm.entity.stateVars.actState; set => psm.entity.stateVars.actState = value; }
        protected State PhyState { get => psm.entity.stateVars.phyState; set => psm.entity.stateVars.phyState = value; }
    
        public virtual void CalculateSpd(float timeScale)
        {
            var rbSpd = IndieSpd + SelfSpd;
            if (psm.entity.rb != null)
                psm.entity.rb.velocity = rbSpd * timeScale * psm.entity.transform.lossyScale.y + ExternalSpd + WindSpd * timeScale;
        }
    }
    public class Freeze : PhysicState
    {
        public Freeze(PhysicSM psm) : base(psm) { }
    
        public override void OnStateUpdate(float timeScale)
        {
            base.OnStateUpdate(timeScale);
            CalculateSpd(timeScale);
        }
    }
    
    
    public class InAir : PhysicState
    {
        public InAir(PhysicSM psm) : base(psm) { }
        public override void OnStateUpdate(float timeScale)
        {
            base.OnStateUpdate(timeScale);
    
            if (!Aerial)
            {
                TransitionGround();
                CalculateSpd(timeScale);
                return;
            }
            if (Gravity)
                ApplyGravity(timeScale);
            ApplyAirResistance(timeScale);
            CalculateSpd(timeScale);
        }
    
        protected virtual void TransitionGround()
        {
            SetState(new OnGround(psm));
        }
    
        protected virtual void ApplyGravity(float timeScale)
        {
            if (IndieSpd.y > -Et.termVelo)
            {
                IndieSpd -= Vector2.up * Et.fallSpd * timeScale;
            }
        }
    
        protected virtual void ApplyAirResistance(float timeScale)
        {
            IndieSpd -= Vector2.right * IndieSpd.x * Et.airRes * timeScale;
            WindSpd -= WindSpd * psm.entity.et.launchRes * timeScale;
        }
    }
    public class InAirNoTransition : InAir
    {
        public InAirNoTransition(PhysicSM psm) : base(psm) { }
        protected override void TransitionGround() { }
        protected override void ApplyAirResistance(float timeScale)
        {
            IndieSpd -= IndieSpd * Et.airRes * timeScale;
            WindSpd -= WindSpd * psm.entity.et.launchRes * timeScale;
        }
    }
    
    public class OnGround : PhysicState
    {
        public OnGround(PhysicSM psm) : base(psm) { }
    
        public override void OnStateUpdate(float timeScale)
        {
            base.OnStateUpdate(timeScale);
            if (Aerial)
            {
                SetState(new InAir(psm));
                CalculateSpd(timeScale);
                return;
            }
            ApplyGroundFriction(timeScale);
            CalculateSpd(timeScale);
        }
    
        protected virtual void TransitionAir()
        {
            SetState(new InAir(psm));
        }
    
        public override void OnStateEnter()
        {
            base.OnStateEnter();
            IndieSpd *= Vector2.right;
            WindSpd *= Vector2.right;
        }
    
        protected void ApplyGroundFriction(float timeScale)
        {
            IndieSpd -= (Vector2.right * IndieSpd.x * Et.groundRes * 1.5f) * timeScale;
            WindSpd -= (Vector2.right * WindSpd.x * Et.groundRes * 1.5f) * timeScale;
        }
    }
    
    public class Launched : InAir
    {
        public Launched(PhysicSM psm) : base(psm) { }
    
        public override void OnStateUpdate(float timeScale)
        {
            base.OnStateUpdate(timeScale);
            Bounce();
        }
    
        protected override void TransitionGround()
        {
            if (Mathf.Abs(IndieSpd.y) < 4)
                base.TransitionGround();
        }
    
        private void Bounce()
        {
            if (psm.entity.stateVars.continueCollision)
            {
                var normals = psm.entity.GetTotalContactNormals();
                if (Vector2.Dot(normals.normalized, IndieSpd.normalized) < 0)
                    IndieSpd = Vector2.Reflect(IndieSpd, psm.entity.GetTotalContactNormals());
            }
        }
    
        protected override void ApplyAirResistance(float timeScale)
        {
            IndieSpd -= IndieSpd * psm.entity.et.launchRes * timeScale;
            WindSpd -= WindSpd * psm.entity.et.launchRes * timeScale;
        }
    }
    public class ProjectileLaunched : Launched
    {
        public ProjectileLaunched(PhysicSM psm) : base(psm) { }
    
        public override void OnStateUpdate(float timeScale)
        {
            base.OnStateUpdate(timeScale);
            if (IndieSpd.magnitude <= psm.entity.et.airSpd)
                SetState(new InAirNoTransition(psm));
        }
    
        public override void OnStateExit()
        {
            base.OnStateExit();
            psm.entity.stateVars.lastLaunchSpd = Vector2.zero;
        }
    
        protected override void TransitionGround()
        {
        }
        protected override void ApplyAirResistance(float timeScale)
        {
            IndieSpd -= IndieSpd * psm.entity.et.launchRes * timeScale;
        }
    }
    
    public class LaunchedMini : InAir
    {
        public LaunchedMini(PhysicSM psm) : base(psm) { }
    
        protected override void ApplyAirResistance(float timeScale)
        {
            IndieSpd -= IndieSpd * psm.entity.et.launchRes * timeScale;
            WindSpd -= WindSpd * psm.entity.et.launchRes * timeScale;
        }
    }
    
    public class Focus : InAir
    {
        public Focus(PhysicSM psm) : base(psm) { }
    
        protected override void ApplyAirResistance(float timeScale)
        {
            var Ft = Et as FighterProperties;
            IndieSpd -= IndieSpd * Ft.focusRes * timeScale;
            WindSpd -= WindSpd * psm.entity.et.launchRes * timeScale;
        }
        protected override void ApplyGravity(float timeScale) { }
    }
    public class MarisaAirRush : InAir
    {
        public MarisaAirRush(PhysicSM psm) : base(psm) { }
    
        protected override void ApplyAirResistance(float timeScale)
        {
            var Ft = Et as FighterProperties;
            IndieSpd -= IndieSpd * Ft.focusRes * timeScale;
            WindSpd -= WindSpd * psm.entity.et.launchRes * timeScale;
        }
        protected override void ApplyGravity(float timeScale) { }
        protected override void TransitionGround() { }
    }}
