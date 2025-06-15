namespace TightStuff
{
    using UnityEngine;
    
    public abstract class ActionState : State
    {
        public readonly ActionSM fsm;
        public EntityProperties Et { get => fsm.entity.et; }
    
        public ActionState(ActionSM fsm) : base(fsm)
        {
            this.fsm = fsm;
        }
    
        protected Vector2 IndieSpd { get => fsm.entity.stateVars.indieSpd; set => fsm.entity.stateVars.indieSpd = value; }
        protected Vector2 SelfSpd { get => fsm.entity.stateVars.selfSpd; set => fsm.entity.stateVars.selfSpd = value; }
        protected Vector2 WindSpd { get => fsm.entity.stateVars.windSpd; set => fsm.entity.stateVars.windSpd = value; }
        protected Vector2 ExternalSpd { get => fsm.entity.stateVars.externalSpd; set => fsm.entity.stateVars.externalSpd = value; }
        protected bool Aerial { get => fsm.entity.stateVars.aerial; set => fsm.entity.stateVars.aerial = value; }
        protected bool Gravity { get => fsm.entity.stateVars.gravity; set => fsm.entity.stateVars.gravity = value; }
        protected bool PlatformCondition { get => fsm.entity.stateVars.platformCondition; set => fsm.entity.stateVars.platformCondition = value; }
        protected bool FlippedLeft { get => fsm.entity.stateVars.flippedLeft; set => fsm.entity.stateVars.flippedLeft = value; }
        protected bool EnteredCollision { get => fsm.entity.stateVars.enteredCollision; set => fsm.entity.stateVars.enteredCollision = value; }
        protected bool ContinueCollision { get => fsm.entity.stateVars.continueCollision; set => fsm.entity.stateVars.continueCollision = value; }
        protected bool ExitedCollision { get => fsm.entity.stateVars.exitedCollision; set => fsm.entity.stateVars.exitedCollision = value; }
        protected int FrameNum { get => fsm.entity.stateVars.frameNum; set => fsm.entity.stateVars.frameNum = value; }
        protected int AnimID { get => fsm.entity.stateVars.animID; set => fsm.entity.stateVars.animID = value; }
        protected float SelfTime { get => fsm.entity.stateVars.selfTime; set => fsm.entity.stateVars.selfTime = value; }
        protected float GivenTime { get => fsm.entity.stateVars.givenTime; set => fsm.entity.stateVars.givenTime = value; }
        protected float FreezeTime { get => fsm.entity.stateVars.freezeTime; set => fsm.entity.stateVars.freezeTime = value; }
        protected float GenericTimer { get => fsm.entity.stateVars.genericTimer; set => fsm.entity.stateVars.genericTimer = value; }
        protected float Percent { get => fsm.entity.stateVars.percent; set => fsm.entity.stateVars.percent = value; }
        protected bool Intangible { get => fsm.entity.stateVars.intangible; set => fsm.entity.stateVars.intangible = value; }
        protected bool Invulnerable { get => fsm.entity.stateVars.invulnerable; set => fsm.entity.stateVars.invulnerable = value; }
        protected State ActState { get => fsm.entity.stateVars.actState; set => fsm.entity.stateVars.actState = value; }
        protected State PhyState { get => fsm.entity.stateVars.phyState; set => fsm.entity.stateVars.phyState = value; }
    }
    
    public abstract class GroundedState : ActionState
    {
        public GroundedState(ActionSM fsm) : base(fsm) { }
        public override void OnStateUpdate(float time)
        {
            base.OnStateUpdate(time);
            TransitionAir();
        }
    
        protected virtual void TransitionAir()
        {
            if (fsm.entity.stateVars.aerial)
            {
                SetState(new AirIdle(fsm));
            }
        }
    }
    
    
    public abstract class AirborneState : ActionState
    {
        public AirborneState(ActionSM fsm) : base(fsm) { }
        public override void OnStateUpdate(float time)
        {
            base.OnStateUpdate(time);
            TransitionGround();
        }
    
        protected virtual void TransitionGround()
        {
            if (!Aerial)
            {
                SetState(new GroundIdle(fsm));
            }
        }
    }
    
    public class AirIdle : AirborneState
    {
        public AirIdle(ActionSM fsm) : base(fsm) { }
        public override void OnStateEnter()
        {
            base.OnStateEnter();
        }
    }
    
    public class GroundIdle : GroundedState
    {
        public GroundIdle(ActionSM fsm) : base(fsm) { }
        public override void OnStateEnter()
        {
            base.OnStateEnter();
        }
    }}
