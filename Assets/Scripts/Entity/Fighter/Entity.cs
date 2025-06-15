namespace TightStuff
{
    using UnityEngine;
    using System.Collections.Generic;
    using UnityEngine.Events;
    
    public class Entity : UpdateAbstract
    {
        public Rigidbody2D rb;
        private Animator _anim;
        public EntityProperties et;
    
        public bool simulatePhysics = true;
        public bool setPlatLayers = false;
    
        public EntityState stateVars;
    
        public ActionSM actionState;
        public PhysicSM physicsState;
    
        [SerializeField]
        private string _actionStateName;
        [SerializeField]
        private string _physicsStateName;
    
        [SerializeField]
        private List<Behaviour> AssociatedComponents;
        [SerializeField]
        public List<Renderer> AssociatedRenderers;
        public List<Collider2D> AssociatedColliders;
    
        [SerializeField]
        public List<ParticleSystem> particles;
    
        public UnityEvent OnSpawn;
    
        [SerializeField]
        private bool _hasStates;
        [UnityEngine.Serialization.FormerlySerializedAs("_alwaysSaveState")]
        public bool alwaysSaveState;
        public bool followTarget;
        public int platformLayer;
        public int platformlessLayer;
    
        public float TrueTimeScale => stateVars.freezeTime > 0 ? 0 : Mathf.Clamp(MatchManager.worldTime * stateVars.selfTime, stateVars.givenTime, 4);
        public float NonFreezeTimeScale => Mathf.Clamp(MatchManager.worldTime * stateVars.selfTime, stateVars.givenTime, 4);
    
        private void Start()
        {
            order = 1000;
            lateOrder = 50000;
    
            rb = GetComponent<Rigidbody2D>();
            _anim = GetComponent<Animator>();
            if (_anim != null)
                _anim.enabled = false;
            if (_hasStates)
            {
                if (actionState == null)
                {
                    actionState = new ActionSM();
                    actionState.entity = this;
                    actionState.SetState(new AirIdle(actionState));
                }
                if (physicsState == null)
                {
                    physicsState = new PhysicSM();
                    physicsState.entity = this;
                    physicsState.SetState(new InAir(physicsState));
                }
            }
            SetEntityActive(stateVars.enabled);
            UpdateTransformState();
        }
    
        public override void GUpdate()
        {
            base.GUpdate();
            SetEntityActive(stateVars.enabled);
            if (!stateVars.enabled)
                return;
    
            CheckContacts();
    
            CheckCollisionEnter();
            CheckCollisionExit();
    
            IsGrounded();
    
            if (_hasStates)
                actionState.StateUpdate(TrueTimeScale);
    
            UpdateAnim(TrueTimeScale);
        }
    
        public override void LateGUpdate()
        {
            base.LateGUpdate();
            if (!stateVars.enabled)
            {
                UpdateTransformState();
                return;
            }
    
            if (stateVars.freezeTime > 0)
                stateVars.freezeTime -= NonFreezeTimeScale * Time.fixedDeltaTime;
    
            SetFlipped();
            SetPlatformLayer();
            if (_hasStates)
            {
                physicsState.StateUpdate(TrueTimeScale);
                SetStates();
                DisplayStateName();
            }
    
            ResetCollisionPerFrame();
            UpdateTransformState();
        }
        private void SetPlatformLayer()
        {
            if (rb == null || !setPlatLayers)
                return;
            if (AssociatedColliders.Count <= 0)
                return;
            foreach (Collider2D cols in AssociatedColliders)
                cols.gameObject.layer = stateVars.platformCondition ? platformLayer : platformlessLayer;
        }
    
        public void RollbackState()
        {
            SetEntityActive(stateVars.enabled);
    
            SetPlatformLayer();
            if (_hasStates)
            {
                if (stateVars.actState != null && stateVars.phyState != null)
                {
                    actionState.StateRollback(stateVars.actState);
                    physicsState.StateRollback(stateVars.phyState);
                }
            }
            transform.position = stateVars.position;
            transform.rotation = stateVars.rotation;
            transform.localScale = stateVars.scale;
    
            if (rb != null)
                rb.velocity = stateVars.trueRbSpd;
    
            if (_anim == null)
                return;
    
            PlayAnimAtCurrentFrameNumber();
        }
    
        public void UpdateTransformState()
        {
            stateVars.enabled = enabled;
    
            stateVars.position = transform.position;
            stateVars.rotation = transform.rotation;
            stateVars.scale = transform.localScale;
    
            if (rb != null)
                stateVars.trueRbSpd = rb.velocity;
        }
    
        private void DisplayStateName()
        {
            _actionStateName = stateVars.actState.GetType().Name;
            _physicsStateName = stateVars.phyState.GetType().Name;
        }
    
        private void SetStates()
        {
            stateVars.actState = actionState.GetState();
            stateVars.phyState = physicsState.GetState();
        }
    
        private void UpdateStates(float timeScale)
        {
            actionState.StateUpdate(timeScale);
            physicsState.StateUpdate(timeScale);
        }
    
        public void SetFlipped()
        {
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, 1);
    
            if (stateVars.flippedLeft)
                transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, 1);
        }
    
        public void SetEntityActive(int va)
        {
            var val = va == 1;
            SetEntityActive(val);
        }
    
        public void SetEntityActive(bool val)
        {
            stateVars.enabled = val;
            enabled = val;
    
            if (AssociatedComponents.Count > 0)
            {
                foreach (Behaviour c in AssociatedComponents)
                    c.enabled = val;
            }
            if (AssociatedRenderers.Count > 0)
            {
                foreach (Renderer r in AssociatedRenderers)
                    r.enabled = val;
            }
            if (AssociatedColliders.Count > 0)
            {
                foreach (Collider2D c in AssociatedColliders)
                    c.enabled = val;
            }
    
            if (rb == null)
                return;
            rb.isKinematic = !val || !simulatePhysics;
            rb.simulated = val && simulatePhysics;
        }
    
        //COLLISION FUNCTIONS============================================================================================================================================
        private void CheckContacts()
        {
            if (rb == null)
                return;
    
            stateVars.currentCollision = new ContactPoint2D[10];
            int i = rb.GetContacts(stateVars.currentCollision);
    
            if (i == 0)
            {
                stateVars.currentCollision = null;
                return;
            }
    
            stateVars.currentCollision = new ContactPoint2D[i];
            rb.GetContacts(stateVars.currentCollision);
        }
    
        private void ResetCollisionPerFrame()
        {
            stateVars.currentCollision = null;
            stateVars.exitedCollision = false;
            stateVars.enteredCollision = false;
        }
    
        private void CheckCollisionExit()
        {
            if (stateVars.currentCollision == null)
            {
                if (stateVars.continueCollision)
                    stateVars.exitedCollision = true;
                stateVars.continueCollision = false;
            }
        }
    
        private void CheckCollisionEnter()
        {
            if (stateVars.currentCollision != null)
            {
                if (!stateVars.continueCollision)
                    stateVars.enteredCollision = true;
                stateVars.continueCollision = true;
            }
        }
    
        public bool IsGrounded()
        {
            if (!stateVars.continueCollision)
            {
                stateVars.aerial = true;
                return false;
            }
    
            var land = false;
    
            foreach (ContactPoint2D contact in stateVars.currentCollision)
            {
                var layer = contact.collider.gameObject.layer;
                if (layer == 0 || layer == 10)
                {
                    if (contact.normal.y >= 0.9f)
                        land = true;
                }
            }
            stateVars.aerial = !land || (stateVars.indieSpd.y + stateVars.windSpd.y) > 0;
            return !stateVars.aerial;
        }
    
        public Vector2 GetTotalContactNormals()
        {
            var normal = Vector2.zero;
            var count = 0;
            if (stateVars.currentCollision == null)
                return Vector2.zero;
            foreach (ContactPoint2D contact in stateVars.currentCollision)
            {
                if (contact.collider.gameObject.layer == 10 && contact.normal.y < 0.9f)
                    continue;
                normal += contact.normal;
                count++;
            }
            if (count == 0)
                return Vector2.zero;
    
            return (normal / count).normalized;
        }
        public Vector2 GetAverageContactPoint()
        {
            var point = Vector2.zero;
            var count = 0;
            if (stateVars.currentCollision == null)
                return Vector2.zero;
            foreach (ContactPoint2D contact in stateVars.currentCollision)
            {
                if (contact.collider.gameObject.layer == 10 && contact.normal.y < 0.9f)
                    continue;
                point += contact.point;
                count++;
            }
            if (count == 0)
                return Vector2.zero;
    
            return point / count;
        }
    
    
        //ANIMATOR FUNCTIONS================================================================================================================================================
        public void UpdateAnim(float timeScale)
        {
            if (_anim == null)
                return;
    
            _anim.speed = Mathf.Clamp(timeScale, 1, 10);
    
            PlayAnimAtCurrentFrameNumber();
    
            if (stateVars.frameNum >= 0 && timeScale > 0)
                _anim.Update(Time.fixedDeltaTime);
    
            if (_anim.GetCurrentAnimatorStateInfo(0).fullPathHash != stateVars.animID || _anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1)
                stateVars.frameNum = 0;
            else
                stateVars.frameNum += Mathf.RoundToInt(10 * Mathf.Clamp(timeScale, 0, 1));
    
            stateVars.animID = _anim.GetCurrentAnimatorStateInfo(0).fullPathHash;
        }
    
        public void AnimEmptyUpdate()
        {
            _anim.fireEvents = false;
            _anim.Update(0);
            _anim.fireEvents = true;
        }
    
        public void PlayAnim(string name)
        {
            if (_anim == null)
                return;
    
            _anim.PlayInFixedTime(name, 0, 0);
            AnimEmptyUpdate();
            stateVars.animID = _anim.GetCurrentAnimatorStateInfo(0).fullPathHash;
            stateVars.frameNum = -10;
        }
    
        public bool CheckPrevAnimIsName(string name)
        {
            return _anim.GetCurrentAnimatorStateInfo(0).IsName(name);
        }
    
        public void SkipCurrentAnimToFrame(int frame)
        {
            stateVars.frameNum = frame;
            PlayAnimAtCurrentFrameNumber();
        }
    
        private void PlayAnimAtCurrentFrameNumber()
        {
            if (_anim == null)
                return;
            _anim.PlayInFixedTime(stateVars.animID, 0, Mathf.Floor(Mathf.Clamp(stateVars.frameNum / 10, 0, 1000000000000000)) * Time.fixedDeltaTime);
            AnimEmptyUpdate();
        }
        public void ResetSpeed()
        {
            stateVars.indieSpd *= 0;
        }
    
        public void PlayParticles()
        {
            foreach (var p in particles)
            {
                p.Play();
            }
        }
        public void StopParticles()
        {
            foreach (var p in particles)
            {
                p.Stop();
            }
        }
        public bool ParticlesIsPlaying()
        {
            if (particles == null)
                return false;
            if (particles.Count <= 0)
                return false;
            return particles[0].isEmitting;
        }
    }
    
    
    [System.Serializable]
    public struct EntityState
    {
        public bool enabled;
    
        public Vector3 position;
        public Quaternion rotation;
        public Vector3 scale;
    
        public Vector2 indieSpd;
        public Vector2 selfSpd;
        public Vector2 windSpd;
        public Vector2 externalSpd;
        public Vector2 trueRbSpd;
        public Vector2 lastLaunchSpd;
    
        public bool aerial;
        public bool flippedLeft;
        public bool gravity;
        public bool platformCondition;
    
        public bool enteredCollision;
        public bool continueCollision;
        public bool exitedCollision;
        public ContactPoint2D[] currentCollision;
    
        public int frameNum;
        public int animID;
    
        public float selfTime;
        public float givenTime;
        public float freezeTime;
    
        [Range(0f, 999.99f)]
        public float percent;
    
        public float damageArmor;
        public float knockbackArmor;
        public bool intangible;
        public bool invulnerable;
    
        public float genericTimer;
    
        public State actState;
        public State phyState;
    
        public int entityID;
    }
}
