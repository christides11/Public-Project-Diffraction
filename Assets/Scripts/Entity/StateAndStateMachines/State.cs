namespace TightStuff
{
    [System.Serializable]
    public abstract class State
    {
        public readonly StateMachine sm;
    
        public State(StateMachine s)
        {
            sm = s;
        }
    
        public virtual void OnStateEnter() { }
        public virtual void OnStateUpdate(float timeScale) { }
        public virtual void OnStateExit() { }
    
        public void SetState<T>(T state) where T : State
        {
            sm.SetState(state);
        }
    }
}
