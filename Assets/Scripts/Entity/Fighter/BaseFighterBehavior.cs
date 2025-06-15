namespace TightStuff
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;
    using UnityEngine.Events;
    
    public class BaseFighterBehavior : Controllable
    {
        public FighterStateVars stateVarsF;
    
        public Extension.BaseFighterExtensions extension;
    
        public Transform groundCheckPoint;
        public Transform groundCheckPoint2;
    
        public List<Transform> hurtbox;
        public List<Transform> hitboxParents;
    
        public Collider2D boxcastCol;
        public CircleCollider2D ledgecastCol;
    
        public LayerMask groundLayers;
        public LayerMask groundplatLayers;
        public LayerMask platLayers;
        public LayerMask playerLayers;
        public LayerMask ledgeLayer;
    
        [SerializeField]
        public RespawnBehavior _respawnWarning;
    
        public UnityEvent ClearAttacks;
    
        public Entity shield;
    
        public Pooler shooter;
        public Pooler aesthetics;
        public Pooler assist;
        [SerializeField]
        private Animator _grazeOutlineAnimator;
    
        private BaseProjectileBehaviour _assistObj;
    
        [SerializeField]
        private ParticleSystem _petals;
        public int colorID;
    
        public bool BounceCondition => Mathf.Abs(entity.stateVars.indieSpd.y) >= 4;
    
        public static Action SpellcardTimeStopEvent;
        public static Action<FighterState.AttackID, FighterProperties, int> SpellcardActivateEvent;
        public static Action<FighterState.AttackID, FighterProperties, int> SpellcardActivateStopEvent;
    
        public ActionSM ActionState { get => entity.actionState; set => entity.actionState = value; }
        public FighterProperties Ft { get => entity.et as FighterProperties; }
        public BaseProjectileBehaviour AssistObj => _assistObj;
    
        // Start is called before the first frame update
        void Start()
        {
            order = 2000;
            lateOrder = 2000;
    
            controlling = GetComponent<Controller>();
            assist = GetComponent<Pooler>();
    
            entity = GetComponent<Entity>();
            var sm = new FighterSM(this);
            ActionState = sm;
            ActionState.entity = entity;
            ActionState.SetState(new GroundedFighterIntroState(sm));
    
            var hitboxes = new List<Hitter>();
            foreach(Transform hitboxparent in hitboxParents)
                hitboxes.AddRange(hitboxparent.GetComponentsInChildren<Hitter>());
            foreach (var hitbox in hitboxes)
                ClearAttacks.AddListener(hitbox.ClearHits);
    
            _assistObj = assist.poolDict["Assist"][0].GetComponent<BaseProjectileBehaviour>();
    
            _respawnWarning.owner = this;
            stateVarsF.hurtboxRotation = Vector2.up;
        }
    
        public override void GUpdate()
        {
            base.GUpdate();
    
            if (stateVarsF.grazeMeter >= 30 || stateVarsF.spellcardState)
                _petals.Play();
            else
                _petals.Stop();
    
            if (!entity.stateVars.aerial)
                stateVarsF.usedAirSpecial = false;
            stateVarsF.assistCD -= Time.fixedDeltaTime * entity.TrueTimeScale;
            if (stateVarsF.assistCD < 0)
                stateVarsF.assistCD = 0;
            stateVarsF.spawnInvulnTimer = Mathf.Clamp(stateVarsF.spawnInvulnTimer - Time.fixedDeltaTime * entity.TrueTimeScale, 0, Mathf.Infinity);
            entity.stateVars.invulnerable = stateVarsF.spawnInvulnTimer > 0;
            if (entity.stateVars.intangible)
                stateVarsF.playerCollision = false;
    
            shield.stateVars.percent -= Time.fixedDeltaTime * entity.NonFreezeTimeScale * 5f;
            shield.stateVars.percent = Mathf.Clamp(shield.stateVars.percent, 0, shield.et.health);
    
            stateVarsF.grazeMeter = Mathf.Clamp(stateVarsF.grazeMeter, 0, 33);
    
            controlling.timeScale = entity.TrueTimeScale;
    
            transform.position += (Vector3)stateVarsF.displace; 
            stateVarsF.displace *= 0;
    
            if (stateVarsF.spellcardState)
                stateVarsF.grazeMeter -= 2.5f * entity.NonFreezeTimeScale * Time.fixedDeltaTime;
            if (stateVarsF.grazeMeter <= 0)
                stateVarsF.spellcardState = false;
    
            if (entity.stateVars.indieSpd.magnitude > 5 && entity.physicsState.GetState() is Launched)
            {
                if (!entity.ParticlesIsPlaying())
                    entity.PlayParticles();
            }
            else
                entity.StopParticles();
        }
        public override void LateGUpdate()
        {
            base.LateGUpdate();
            if (entity.actionState.GetState() is AirborneFighterHurtFreezeState || entity.actionState.GetState() is GroundedFighterHurtFreezeState)
            {
                hurtbox[1].transform.localPosition = Mathf.Sin(entity.stateVars.freezeTime * 200) * Mathf.Clamp(entity.stateVars.freezeTime * entity.stateVars.lastLaunchSpd.magnitude * 0.05f, 0, 0.25f) * Vector2.right;
            }
        }
        public void ExpendStamina(float amount)
        {
            stateVarsF.stamina = Mathf.Clamp(stateVarsF.stamina - amount, 0, stateVarsF.maxStamina);
        }
    
        public void TransitionIdle()
        {
            if (!entity.stateVars.aerial)
                ActionState.SetState(new GroundedFighterIdleState(ActionState as FighterSM));
            else
                ActionState.SetState(new AirborneFighterIdleState(ActionState as FighterSM));
        }
        public void TransitionCrouch()
        {
            ActionState.SetState(new GroundedFighterCrouchState(ActionState as FighterSM));
        }
    
        public void CrouchLagCancel()
        {
            stateVarsF.crouchLag = false;
        }
    
        public void SetTurning(int turn)
        {
            stateVarsF.turning = turn == 1;
        }
    
        public void Jump()
        {
            var currentSpd = entity.stateVars.indieSpd + entity.stateVars.selfSpd;
            var stickInfluence = controlling.moveStick.raw.x * Mathf.Clamp(entity.et.runSpd - Mathf.Abs(currentSpd.x), 0, 10000000);
            var dashBoost = stateVarsF.dashJump ? (Ft.dashSpd * Mathf.Sign(currentSpd.x)) / 4 : 0;
            
            entity.stateVars.indieSpd = new Vector2(Mathf.Clamp(currentSpd.x + stickInfluence + dashBoost, -Ft.dashSpd, Ft.dashSpd) , Ft.jumpHeight) + entity.stateVars.externalSpd;
            entity.stateVars.externalSpd *= 0;
        }
    
        public void DashBoostStop()
        {
            ActionState.SetState(new GroundedFighterDashIdleState(ActionState as FighterSM));
            if (stateVarsF.stopDash)
                ActionState.SetState(new GroundedFighterDashStopState(ActionState as FighterSM));
        }
        public void DashEnd()
        {
            ActionState.SetState(new GroundedFighterIdleState(ActionState as FighterSM));
        }
    
        public void AirdashCancelable()
        {
            stateVarsF.airdashCancelable = true;
        }
        public void AirdashLagEnd()
        {
            ActionState.SetState(new AirborneFighterIdleState(ActionState as FighterSM));
        }
    
        public void Focus()
        {
            if (controlling.shieldButton.raw)
            {
                ActionState.SetState(new AirborneFighterFocusState(ActionState as FighterSM));
                return;
            }
            ActionState.SetState(new AirborneFighterFocusSpinEndState(ActionState as FighterSM));
        }
        public void TransitionAirIdle()
        {
            ActionState.SetState(new AirborneFighterIdleState(ActionState as FighterSM));
        }
        public void TransitionRollLag()
        {
            ActionState.SetState(new GroundedFighterRollLagState(ActionState as FighterSM));
        }
        public void TransitionRollMove()
        {
            ActionState.SetState(new GroundedFighterRollMoveState(ActionState as FighterSM));
        }
        public void TransitionNormalShield()
        {
            ActionState.SetState(new GroundedFighterShieldState(ActionState as FighterSM));
        }
        public void Turn()
        {
            entity.stateVars.flippedLeft = !entity.stateVars.flippedLeft;
        }
        public void TransitionLedgeIdle()
        {
            ActionState.SetState(new AirborneFighterLedgeIdleState(ActionState as FighterSM));
        }
        public void TransitionAirLedgeEnd()
        {
            ActionState.SetState(new AirborneFighterLedgeEndState(ActionState as FighterSM));
        }
        public void TransitionSmashCharge()
        {
            ActionState.SetState(new GroundedFighterAttackSmashChargeState(ActionState as FighterSM));
        }
        public void TransitionSmashAttack()
        {
            ActionState.SetState(new GroundedFighterAttackSmashState(ActionState as FighterSM));
        }
        public void TransitionThrow(HitObject hit)
        {
            if (hit.box.entity.stateVars.invulnerable)
                return;
            if (ActionState.GetState() is GroundedFighterAttackThrowState || ActionState.GetState() is GroundedFighterThrowTechState)
                return;
    
            if (!hit.box.entity.TryGetComponent(out BaseFighterBehavior fighter))
                return;
            if ((fighter.ActionState.GetState() is GroundedFighterGrabState || fighter.ActionState.GetState() is GroundedFighterThrowTechState) && fighter.entity.stateVars.flippedLeft != hit.hitbox.entity.stateVars.flippedLeft)
            {
                ActionState.SetState(new GroundedFighterThrowTechState(ActionState as FighterSM));
                return;
            }
            ActionState.SetState(new GroundedFighterAttackThrowState(ActionState as FighterSM));
        }
        public void TransitionCombo(string attack)
        {
            ActionState.SetState(new GroundedFighterAttackComboState(ActionState as FighterSM, attack));
        }
        public void TransitionGroundLag()
        {
            ActionState.SetState(new GroundedFighterAttackEndState(ActionState as FighterSM));
        }
        public void SetCrouchIdleAnimation()
        {
            entity.PlayAnim("Ground_Crouch_Idle");
        }
        public void SetAirIdleAnimation()
        {
            entity.PlayAnim("Air_Idle");
        }
        public void SetIdleAnimation()
        {
            if (!entity.stateVars.aerial)
                entity.PlayAnim("Idle");
            else
                entity.PlayAnim("Air_Idle");
        }
    
        public void MatchTransformWithHurtbox()
        {
            transform.position = hurtbox[0].transform.position;
        }
        public void ClearAttackHits()
        {
            ClearAttacks.Invoke();
        }
        public void MovePlayer(string input)
        {
            if (input != null)
            {
                var vals = input.Split(',').Select(s => s.Trim()).ToArray();
                if (vals.Length == 2)
                {
                    float v1 = 0;
                    float v2 = 0;
    
                    if (float.TryParse(vals[0], out v1) && float.TryParse(vals[1], out v2))
                    { 
                        entity.stateVars.indieSpd = new Vector2(entity.stateVars.flippedLeft ? -v1 : v1, v2);
                    }
                }
            }
        }
        public void MovePlayer(Vector2 input)
        {
            entity.stateVars.indieSpd = new Vector2(entity.stateVars.flippedLeft ? -input.x : input.x, input.y);
        }
    
        public void SetWorldTime(float time)
        {
            MatchManager.worldTime = time;
            SpellcardTimeStopEvent?.Invoke();
        }
        public void SetGivenTime(float time)
        {
            entity.stateVars.givenTime = time;
        }
        public void SetSpellcadState()
        {
            stateVarsF.spellcardState = true;
        }
    
        public void ReduceSpellcardCount()
        {
            if (MatchManager.training)
                return;
            if (stateVarsF.currentSpellcardID == FighterState.AttackID.Up)
                stateVarsF.spellcardUpCount--;
            if (stateVarsF.currentSpellcardID == FighterState.AttackID.Down)
                stateVarsF.spellcardDownCount--;
            if (stateVarsF.currentSpellcardID == FighterState.AttackID.Forward || stateVarsF.currentSpellcardID == FighterState.AttackID.Back || stateVarsF.currentSpellcardID == FighterState.AttackID.Neutral)
                stateVarsF.spellcardSideCount--;
        }
    
        public void DeductGraze(HitObject hit)
        {
            if (stateVarsF.grazeMeter > 30)
                stateVarsF.grazeMeter = 30;
            if (stateVarsF.spellcardState)
                stateVarsF.grazeMeter -= hit.hitbox.hitProperties.damage * 0.25f;
        }
        public void ReduceGraze(int amount)
        {
            if (stateVarsF.grazeMeter > 30)
                stateVarsF.grazeMeter = 30;
            
            stateVarsF.grazeMeter -= amount;
        }
    
        public void PlayGrazeOutlineAnimation()
        {
            _grazeOutlineAnimator.enabled = true;
            _grazeOutlineAnimator.PlayInFixedTime(_grazeOutlineAnimator.GetCurrentAnimatorStateInfo(0).fullPathHash, 0, 0);
        }
    
        public void SpellcardActivate()
        {
            SpellcardActivateEvent.Invoke(stateVarsF.currentSpellcardID, Ft, controlling.id);
        }
        public void SpellcardActivateStop()
        {
            SpellcardActivateStopEvent.Invoke(stateVarsF.currentSpellcardID, Ft, controlling.id);
        }
        private bool CheckLedge()
        {
            var midHit = Physics2D.Raycast(transform.position, Vector2.down, (boxcastCol as BoxCollider2D).size.y, groundplatLayers);
            var leftHit = Physics2D.Raycast(transform.position + (boxcastCol as BoxCollider2D).size.x * 0.5f * Vector3.left * transform.lossyScale.y, Vector2.down, (boxcastCol as BoxCollider2D).size.y * transform.lossyScale.y, groundplatLayers);
            var rightHit = Physics2D.Raycast(transform.position + (boxcastCol as BoxCollider2D).size.x * 0.5f * Vector3.right * transform.lossyScale.y, Vector2.down, (boxcastCol as BoxCollider2D).size.y * transform.lossyScale.y, groundplatLayers);
            if ((!rightHit && (entity.stateVars.selfSpd + entity.stateVars.indieSpd).x > 0) || (!leftHit && (entity.stateVars.selfSpd + entity.stateVars.indieSpd).x < 0))
            {
                entity.stateVars.selfSpd *= 0;
                entity.stateVars.indieSpd *= 0;
                if (midHit)
                    return false;
                if (!rightHit)
                    transform.position -= 0.1f * Vector3.right;
                else if (!leftHit)
                    transform.position += 0.1f * Vector3.right;
                return false;
            }
            return true;
        }
    }
    
    [System.Serializable]
    public struct FighterStateVars
    {
        public Vector2 hurtboxRotation;
        public float spawnInvulnTimer;
    
        public bool playerCollision;
    
        public float walktimePassed;
        public bool turning;
        public bool lastLeftDir;
    
        public bool crouchLag;
    
        public bool stopDash;
        public bool dashJump;
        public float jumptimePassed;
    
        public bool jumping;
        public bool jumpingTap;
        public bool jumpingToggle;
    
        public Vector2 airdashDir;
        public bool airdashCancelable;
        public float airdashTimePassed;
        public float airdashDownSpeed;
    
        public int rollDir;
        public float focusTimePassed;
    
        public float platformInvuln;
        public float platformDropTimer;
    
        public FighterState.AttackID currentAttackID;
        public bool autoCancel;
        public bool smashAutoCancel;
        public float attackCharge;
    
        public FighterState.AttackID currentSpecialID;
        public Vector2 specialAim;
        public float specialCharge;
        public float specialTimer;
        public float specialCooldown;
        public int specialCount;
        public bool usedAirSpecial;
    
        public FighterState.AttackID currentSpellcardID;
        public bool spellcardState;
        public int spellcardUpCount;
        public int spellcardSideCount;
        public int spellcardDownCount;
    
        public float stunStopTime;
        public float techCooldown;
    
        public Vector2 displace;
    
        public float assistCD;
    
        public int stocks;
        public float stamina;
        public float maxStamina;
        [Range(0f, 30f)]
        public float grazeMeter;
        public float shieldHealth;
        public float shieldMaxHealth;
    }}
