namespace TightStuff
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    
    public class RespawnBehavior : Controllable
    {
        public BaseFighterBehavior owner;
        public Transform _respawnArea;
        public Transform desiredPos;
    
        [SerializeField]
        public float _moveSpd;
        [SerializeField]
        public float _interpSpd;
        [SerializeField]
        public float warningTime = 3;
        [SerializeField]
        public float standbyTime = 2;
    
        [SerializeField]
        public Flash flasher;
    
        // Start is called before the first frame update
        void Start()
        {
            entity = GetComponent<Entity>();
            flasher = GetComponent<Flash>();
            controlling = owner.controlling;
    
            order = 3000;
            lateOrder = 3000;
            entity.order += 2;
            _respawnArea = GameObject.FindGameObjectWithTag("Respawn").transform;
    
            transform.parent = null;
            transform.position = _respawnArea.position;
    
            desiredPos.parent = null;
            entity.actionState = new ControllableSM(this);
            entity.actionState.SetState(new InactiveState(entity.actionState as ControllableSM));
        }
    }
    
    public class ActiveState : ActionState
    {
        private RespawnBehavior Behavior { get => (fsm as ControllableSM).controlled as RespawnBehavior; }
        private Controller Control { get => (fsm as ControllableSM).controlled.controlling; }
        private float Timer { get => GenericTimer; }
    
        public ActiveState(ControllableSM fsm) : base(fsm) { }
    
        public override void OnStateEnter()
        {
            base.OnStateEnter();
            Behavior.owner.transform.position = Vector2.up * 2;
            Behavior.transform.position = Behavior._respawnArea.position;
            Behavior.desiredPos.position = Behavior._respawnArea.position;
            GenericTimer = 0;
            Behavior.flasher.enabled = true;
            fsm.entity.SetEntityActive(true);
        }
    
        public override void OnStateUpdate(float timeScale)
        {
            base.OnStateUpdate(timeScale);
    
            var maxX = Behavior._respawnArea.position.x + Behavior._respawnArea.localScale.x / 2;
            var minX = Behavior._respawnArea.position.x - Behavior._respawnArea.localScale.x / 2;
    
            var desiredX = Mathf.Clamp(Control.moveStick.raw.x * Behavior._moveSpd + Behavior.desiredPos.position.x, minX, maxX);
            Behavior.desiredPos.position = new Vector2(desiredX, Behavior._respawnArea.position.y);
    
            var lerpX = Mathf.Lerp(Behavior.transform.position.x, desiredX, Behavior._interpSpd);
            Behavior.transform.position = new Vector2(lerpX, Behavior._respawnArea.position.y);
    
            GenericTimer += Time.fixedDeltaTime * timeScale;
            if (Timer > Behavior.warningTime)
                SetState(new InactiveState(fsm as ControllableSM));
        }
    
        public override void OnStateExit()
        {
            base.OnStateExit();
            GenericTimer = 0;
    
            Behavior.owner.entity.SetEntityActive(true);
            Behavior.owner.transform.position = new Vector2(Behavior.transform.position.x, 25);
    
            Behavior.owner.entity.stateVars.flippedLeft = Behavior.transform.position.x > Behavior._respawnArea.position.x;
            Behavior.flasher.enabled = false;
    
            Behavior.owner.entity.actionState.SetState(new AirborneFighterRespawnState(Behavior.owner.entity.actionState as FighterSM));
        }
    }
    
    public class InactiveState : ActionState
    {
        private RespawnBehavior Behavior { get => (fsm as ControllableSM).controlled as RespawnBehavior; }
    
        public InactiveState(ControllableSM fsm) : base(fsm) { }
    
        public override void OnStateEnter()
        {
            base.OnStateEnter();
            fsm.entity.SetEntityActive(false);
        }
    }
    
    public class StandbyState : ActionState
    {
        private RespawnBehavior Behavior { get => (fsm as ControllableSM).controlled as RespawnBehavior; }
        private float Timer { get => GenericTimer; }
    
        public StandbyState(ControllableSM fsm) : base(fsm) { }
    
        public override void OnStateEnter()
        {
            base.OnStateEnter();
            GenericTimer = 0;
            Behavior.transform.position = new Vector2(100, 100);
            fsm.entity.SetEntityActive(true);
        }
        public override void OnStateUpdate(float timeScale)
        {
            base.OnStateUpdate(timeScale);
    
            GenericTimer += Time.fixedDeltaTime * timeScale;
            if (Timer > Behavior.standbyTime)
                SetState(new ActiveState(fsm as ControllableSM));
        }
    }}
