namespace TightStuff
{
    [System.Serializable]
    public abstract class StateMachine
    {
        protected State _state;
    
        public StateMachine() { }
    
        public virtual void SetState(State state)
        {
            if (_state != null) 
                if (_state != state)
                    _state.OnStateExit();
            _state = state;
            _state.OnStateEnter();
        }
    
        public virtual void StateUpdate(float time)
        {
            _state.OnStateUpdate(time);
        }
    
        public void StateRollback(State state)
        {
            _state = state;
        }
    
        public State GetState()
        {
            return _state;
        }
    }
}
