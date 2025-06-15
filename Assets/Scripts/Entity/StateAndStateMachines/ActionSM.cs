namespace TightStuff
{
    using System;
    
    public class ActionSM : StateMachine
    {
        public Entity entity;
        public static event Action<State> OnStateEnter;
        public static event Action<State> OnStateExit;
        public static event Action<State> OnStateUpdate;
    
        public override void SetState(State state)
        {
            if (_state != null && OnStateExit != null)
                OnStateExit(_state);
            base.SetState(state);
            if (_state != null && OnStateEnter != null)
                OnStateEnter(_state);
        }
    
        public override void StateUpdate(float time)
        {
            if (_state != null && OnStateUpdate != null)
                OnStateUpdate(_state);
            base.StateUpdate(time);
        }
    }
    
    public class ControllableSM : ActionSM
    {
        public readonly Controllable controlled;
    
        public ControllableSM(Controllable f)
        {
            controlled = f;
            entity = f.entity;
        }
    }
    public class ProjectileSM : ControllableSM
    {
        public readonly BaseProjectileBehaviour projectile;
    
        public ProjectileSM(Controllable f) : base(f)
        {
            projectile = f.GetComponent<BaseProjectileBehaviour>();
            entity = f.entity;
        }
    }
    
    public class FighterSM : ControllableSM
    {
        public BaseFighterBehavior fighter;
    
        public FighterSM(BaseFighterBehavior f) : base(f)
        {
            fighter = f;
            entity = f.entity;
        }
    }
}
