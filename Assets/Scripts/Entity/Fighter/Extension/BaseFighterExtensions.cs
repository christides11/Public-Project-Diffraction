namespace TightStuff.Extension
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    
    public abstract class BaseFighterExtensions : UpdateAbstract
    {
        protected CamShaker _camShake;
        protected BaseFighterBehavior _fighter;
        protected Hitter[] _hitters;
        [SerializeField]
        protected List<HitBox> _smashBoxes;
    
        [SerializeField]
        protected ParticleSystem _spawnInvuln;
    
        protected virtual void Start()
        {
            _camShake = FindObjectOfType<CamShaker>();
            _fighter = GetComponent<BaseFighterBehavior>();
            order = 3000;
    
            _hitters = GetComponentsInChildren<Hitter>();
            ActionSM.OnStateEnter += AirdashParticle;
        }
    
        protected virtual void OnDestroy()
        {
            ActionSM.OnStateEnter -= AirdashParticle;
        }
    
        public override void GUpdate()
        {
            base.GUpdate();
            ChargeSmashAttacks();
            if (_fighter.stateVarsF.spawnInvulnTimer > 0 && _fighter.ActionState.GetState() is not GroundedFighterDeadState)
                _spawnInvuln.Play();
            else
                _spawnInvuln.Stop();
        }
    
        protected virtual void ChargeSmashAttacks()
        {
            foreach (var smashbox in _smashBoxes)
            {
                smashbox.hitProperties.damage *= 1 + _fighter.stateVarsF.attackCharge * 0.5f;
                smashbox.hitProperties.knockback *= 1 + _fighter.stateVarsF.attackCharge * 0.5f;
            }
        }
    
        public void SpecialTrigger(Vector2 specialDir, FighterState.AttackID specialID)
        {
            _fighter.stateVarsF.currentSpecialID = specialID;
            _fighter.stateVarsF.specialAim = Vector2.zero;
            if (specialID == FighterState.AttackID.Up)
            {
                SpecialUp(specialDir);
            }
            if (specialID == FighterState.AttackID.Down)
            {
                SpecialDown(specialDir);
            }
            if (specialID == FighterState.AttackID.Forward || specialID == FighterState.AttackID.Back)
            {
                SpecialHorizontal(specialID);
            }
            if (specialID == FighterState.AttackID.Neutral)
            {
                SpecialNeutral(specialDir);
            }
        }
    
        protected virtual void SpecialUp(Vector2 specialDir)
        {
            if (Mathf.Abs(specialDir.x) > 0.175f)
                _fighter.entity.stateVars.flippedLeft = specialDir.x < 0;
            _fighter.stateVarsF.specialCharge = 0;
            WaveBounce();
        }
        protected virtual void SpecialDown(Vector2 specialDir)
        {
            if (Mathf.Abs(specialDir.x) > 0.175f)
                _fighter.entity.stateVars.flippedLeft = specialDir.x < 0;
            _fighter.stateVarsF.specialCharge = 0;
            WaveBounce();
        }
        protected virtual void SpecialHorizontal(FighterState.AttackID specialID)
        {
            if (specialID == FighterState.AttackID.Back)
                _fighter.entity.stateVars.flippedLeft = !_fighter.entity.stateVars.flippedLeft;
            _fighter.stateVarsF.specialCharge = 0;
            WaveBounce();
        }
        protected virtual void SpecialNeutral(Vector2 specialDir)
        {
            _fighter.stateVarsF.specialCharge = 0;
            WaveBounce();
        }
    
    
        public void SpellcardTrigger(Vector2 specialDir, FighterState.AttackID specialID)
        {
            if (specialID == FighterState.AttackID.Up)
            {
                SpellcardUp(specialDir);
            }
            if (specialID == FighterState.AttackID.Down)
            {
                SpellcardDown(specialDir);
            }
            if (specialID == FighterState.AttackID.Forward || specialID == FighterState.AttackID.Back || specialID == FighterState.AttackID.Neutral)
            {
                SpellcardHorizontal(specialID);
            }
        }
    
        protected virtual void SpellcardUp(Vector2 specialDir)
        {
            if (Mathf.Abs(specialDir.x) > 0.175f)
                _fighter.entity.stateVars.flippedLeft = specialDir.x < 0;
            _fighter.stateVarsF.grazeMeter = 0;
        }
        protected virtual void SpellcardDown(Vector2 specialDir)
        {
            if (Mathf.Abs(specialDir.x) > 0.175f)
                _fighter.entity.stateVars.flippedLeft = specialDir.x < 0;
            _fighter.stateVarsF.grazeMeter = 0;
        }
        protected virtual void SpellcardHorizontal(FighterState.AttackID specialID)
        {
            if (specialID == FighterState.AttackID.Back)
                _fighter.entity.stateVars.flippedLeft = !_fighter.entity.stateVars.flippedLeft;
            _fighter.stateVarsF.grazeMeter = 0;
        }
    
        private void WaveBounce()
        {
            if (_fighter.entity.stateVars.flippedLeft != _fighter.controlling.moveStick.raw.x < 0 && Mathf.Abs(_fighter.controlling.moveStick.raw.x) > 0.1)
            {
                _fighter.entity.stateVars.indieSpd *= new Vector2(-0.8f, 1);
                _fighter.entity.stateVars.selfSpd *= new Vector2(-0.8f, 1);
                _fighter.entity.stateVars.flippedLeft = _fighter.controlling.moveStick.raw.x < 0;
            }
        }
        public void TransitionCharge8DirReady()
        {
            if (!_fighter.entity.stateVars.aerial)
                _fighter.entity.actionState.SetState(new GroundedFighterSpecial8DirectionalChargeReadyState(_fighter.entity.actionState as FighterSM));
            else
                _fighter.entity.actionState.SetState(new AirborneFighterSpecial8DirectionalChargeReadyState(_fighter.entity.actionState as FighterSM));
        }
        public void TransitionCharge8DirReady2()
        {
            if (!_fighter.entity.stateVars.aerial)
                _fighter.entity.actionState.SetState(new GroundedFighterSpecial8DirectionalChargeReady2State(_fighter.entity.actionState as FighterSM));
            else
                _fighter.entity.actionState.SetState(new AirborneFighterSpecial8DirectionalChargeReady2State(_fighter.entity.actionState as FighterSM));
        }
        public void TransitionChargeReady()
        {
            if (!_fighter.entity.stateVars.aerial)
                _fighter.entity.actionState.SetState(new GroundedFighterSpecialChargeReadyState(_fighter.entity.actionState as FighterSM));
            else
                _fighter.entity.actionState.SetState(new AirborneFighterSpecialChargeReadyState(_fighter.entity.actionState as FighterSM));
        }
        public void TransitionSpeciaShoot()
        {
            if (!_fighter.entity.stateVars.aerial)
            {
                _fighter.entity.PlayAnim("Ground_Special_Shoot");
                _fighter.entity.actionState.SetState(new GroundedFighterSpecialShootState(_fighter.entity.actionState as FighterSM));
            }
            else
            {
                _fighter.entity.PlayAnim("Air_Special_Shoot");
                _fighter.entity.actionState.SetState(new AirborneFighterSpecialShootState(_fighter.entity.actionState as FighterSM));
            }
        }
        public void TransitionSpeciaNoMoveShoot()
        {
            if (!_fighter.entity.stateVars.aerial)
            {
                _fighter.entity.PlayAnim("Ground_Special_NoMove_Shoot");
                _fighter.entity.actionState.SetState(new GroundedFighterSpecialNoMoveShootState(_fighter.entity.actionState as FighterSM));
            }
            else
            {
                _fighter.entity.PlayAnim("Air_Special_NoMove_Shoot");
                _fighter.entity.actionState.SetState(new AirborneFighterSpecialNoMoveShootState(_fighter.entity.actionState as FighterSM));
            }
        }
    
        public void TransitionSpecia8DirShoot()
        {
            if (Mathf.Abs(_fighter.stateVarsF.specialAim.x) > 0.35f)
                _fighter.entity.stateVars.flippedLeft = _fighter.stateVarsF.specialAim.x < 0;
    
            if (!_fighter.entity.stateVars.aerial)
            {
                _fighter.entity.PlayAnim("Ground_Special_8Directional_Shoot_Right");
                _fighter.entity.actionState.SetState(new GroundedFighterSpecial8DirectionalShootRightState(_fighter.entity.actionState as FighterSM));
    
                if (_fighter.stateVarsF.specialAim.y > 0.9f)
                {
                    _fighter.entity.PlayAnim("Ground_Special_8Directional_Shoot_Up");
                    _fighter.entity.actionState.SetState(new GroundedFighterSpecial8DirectionalShootUpState(_fighter.entity.actionState as FighterSM));
                    return;
                }
                if (_fighter.stateVarsF.specialAim.y < -0.9f)
                {
                    _fighter.entity.PlayAnim("Ground_Special_8Directional_Shoot_Down");
                    _fighter.entity.actionState.SetState(new GroundedFighterSpecial8DirectionalShootDownState(_fighter.entity.actionState as FighterSM));
                    return;
                }
                if (_fighter.stateVarsF.specialAim.y > 0.35f && Mathf.Abs(_fighter.stateVarsF.specialAim.x) > 0.35f)
                {
                    _fighter.entity.PlayAnim("Ground_Special_8Directional_Shoot_UpRight");
                    _fighter.entity.actionState.SetState(new GroundedFighterSpecial8DirectionalShootUpRightState(_fighter.entity.actionState as FighterSM));
                    return;
                }
                if (_fighter.stateVarsF.specialAim.y < -0.35f && Mathf.Abs(_fighter.stateVarsF.specialAim.x) > 0.35f)
                {
                    _fighter.entity.PlayAnim("Ground_Special_8Directional_Shoot_DownRight");
                    _fighter.entity.actionState.SetState(new GroundedFighterSpecial8DirectionalShootDownRightState(_fighter.entity.actionState as FighterSM));
                    return;
                }
            }
            else
            {
                _fighter.entity.PlayAnim("Air_Special_8Directional_Shoot_Right");
                _fighter.entity.actionState.SetState(new AirborneFighterSpecial8DirectionalShootRightState(_fighter.entity.actionState as FighterSM));
    
                if (_fighter.stateVarsF.specialAim.y > 0.9f)
                {
                    _fighter.entity.PlayAnim("Air_Special_8Directional_Shoot_Up");
                    _fighter.entity.actionState.SetState(new AirborneFighterSpecial8DirectionalShootUpState(_fighter.entity.actionState as FighterSM));
                    return;
                }
                if (_fighter.stateVarsF.specialAim.y < -0.9f)
                {
                    _fighter.entity.PlayAnim("Air_Special_8Directional_Shoot_Down");
                    _fighter.entity.actionState.SetState(new AirborneFighterSpecial8DirectionalShootDownState(_fighter.entity.actionState as FighterSM));
                    return;
                }
                if (_fighter.stateVarsF.specialAim.y > 0.35f && Mathf.Abs(_fighter.stateVarsF.specialAim.x) > 0.35f)
                {
                    _fighter.entity.PlayAnim("Air_Special_8Directional_Shoot_UpRight");
                    _fighter.entity.actionState.SetState(new AirborneFighterSpecial8DirectionalShootUpRightState(_fighter.entity.actionState as FighterSM));
                    return;
                }
                if (_fighter.stateVarsF.specialAim.y < -0.35f && Mathf.Abs(_fighter.stateVarsF.specialAim.x) > 0.35f)
                {
                    _fighter.entity.PlayAnim("Air_Special_8Directional_Shoot_DownRight");
                    _fighter.entity.actionState.SetState(new AirborneFighterSpecial8DirectionalShootDownRightState(_fighter.entity.actionState as FighterSM));
                    return;
                }
            }
        }
        public void TransitionSpecia8DirShoot2()
        {
            if (Mathf.Abs(_fighter.stateVarsF.specialAim.x) > 0.35f)
                _fighter.entity.stateVars.flippedLeft = _fighter.stateVarsF.specialAim.x < 0;
    
            if (!_fighter.entity.stateVars.aerial)
            {
                _fighter.entity.PlayAnim("Ground_Special_8Directional_Shoot_Right2");
                _fighter.entity.actionState.SetState(new GroundedFighterSpecial8DirectionalShootRight2State(_fighter.entity.actionState as FighterSM));
    
                if (_fighter.stateVarsF.specialAim.y > 0.9f)
                {
                    _fighter.entity.PlayAnim("Ground_Special_8Directional_Shoot_Up2");
                    _fighter.entity.actionState.SetState(new GroundedFighterSpecial8DirectionalShootUp2State(_fighter.entity.actionState as FighterSM));
                    return;
                }
                if (_fighter.stateVarsF.specialAim.y < -0.9f)
                {
                    _fighter.entity.PlayAnim("Ground_Special_8Directional_Shoot_Down2");
                    _fighter.entity.actionState.SetState(new GroundedFighterSpecial8DirectionalShootDown2State(_fighter.entity.actionState as FighterSM));
                    return;
                }
                if (_fighter.stateVarsF.specialAim.y > 0.35f && Mathf.Abs(_fighter.stateVarsF.specialAim.x) > 0.35f)
                {
                    _fighter.entity.PlayAnim("Ground_Special_8Directional_Shoot_UpRight2");
                    _fighter.entity.actionState.SetState(new GroundedFighterSpecial8DirectionalShootUpRight2State(_fighter.entity.actionState as FighterSM));
                    return;
                }
                if (_fighter.stateVarsF.specialAim.y < -0.35f && Mathf.Abs(_fighter.stateVarsF.specialAim.x) > 0.35f)
                {
                    _fighter.entity.PlayAnim("Ground_Special_8Directional_Shoot_DownRight2");
                    _fighter.entity.actionState.SetState(new GroundedFighterSpecial8DirectionalShootDownRight2State(_fighter.entity.actionState as FighterSM));
                    return;
                }
            }
            else
            {
                _fighter.entity.PlayAnim("Air_Special_8Directional_Shoot_Right2");
                _fighter.entity.actionState.SetState(new AirborneFighterSpecial8DirectionalShootRight2State(_fighter.entity.actionState as FighterSM));
    
                if (_fighter.stateVarsF.specialAim.y > 0.9f)
                {
                    _fighter.entity.PlayAnim("Air_Special_8Directional_Shoot_Up2");
                    _fighter.entity.actionState.SetState(new AirborneFighterSpecial8DirectionalShootUp2State(_fighter.entity.actionState as FighterSM));
                    return;
                }
                if (_fighter.stateVarsF.specialAim.y < -0.9f)
                {
                    _fighter.entity.PlayAnim("Air_Special_8Directional_Shoot_Down2");
                    _fighter.entity.actionState.SetState(new AirborneFighterSpecial8DirectionalShootDown2State(_fighter.entity.actionState as FighterSM));
                    return;
                }
                if (_fighter.stateVarsF.specialAim.y > 0.35f && Mathf.Abs(_fighter.stateVarsF.specialAim.x) > 0.35f)
                {
                    _fighter.entity.PlayAnim("Air_Special_8Directional_Shoot_UpRight2");
                    _fighter.entity.actionState.SetState(new AirborneFighterSpecial8DirectionalShootUpRight2State(_fighter.entity.actionState as FighterSM));
                    return;
                }
                if (_fighter.stateVarsF.specialAim.y < -0.35f && Mathf.Abs(_fighter.stateVarsF.specialAim.x) > 0.35f)
                {
                    _fighter.entity.PlayAnim("Air_Special_8Directional_Shoot_DownRight2");
                    _fighter.entity.actionState.SetState(new AirborneFighterSpecial8DirectionalShootDownRight2State(_fighter.entity.actionState as FighterSM));
                    return;
                }
            }
        }
        public void TransitionSpellcardChargeReady2()
        {
            _fighter.entity.actionState.SetState(new GroundedFighterSpellcardCharge2ReadyState(_fighter.entity.actionState as FighterSM));
        }
        public void TransitionSpellcardShoot2()
        {
            _fighter.entity.actionState.SetState(new GroundedFighterSpellcardShoot2State(_fighter.entity.actionState as FighterSM));
        }
    
        public void SpawnDashingParticle()
        {
            if (!_fighter.aesthetics.enabled)
                return;
            var particle = _fighter.aesthetics.SpawnEntityFromPool("Dashing", _fighter.shooter.transform.position, Quaternion.identity, Vector3.one);
            particle.stateVars.flippedLeft = _fighter.transform.lossyScale.x < 0;
            particle.transform.localScale = new Vector3(_fighter.transform.lossyScale.x, _fighter.transform.lossyScale.y, 1);
            particle.stateVars.selfTime = 1f;
            particle.PlayParticles();
        }
        public void SpawnSound(string sound)
        {
            if (!_fighter.aesthetics.enabled)
                return;
            var soundFX = _fighter.aesthetics.SpawnFromPool(sound, _fighter.shooter.transform.position, Quaternion.identity, Vector3.one);
        }
        public void SpurtSpellcardParticle()
        {
            if (!_fighter.aesthetics.enabled)
                return;
            var point = transform.position - 0.2f * Vector3.up;
            var particle = _fighter.aesthetics.SpawnEntityFromPool("SpellcardPetals", point, Quaternion.identity, Vector3.one);
    
            if (particle.TryGetComponent(out SimulationSpeedMatcher sim))
            {
                sim.SetEntity(_fighter.entity);
            }
            particle.PlayParticles();
        }
    
        public void AirdashParticle(State state)
        {
            if (!_fighter.aesthetics.enabled)
                return;
    
            if (state is not AirborneFighterAirdashState)
                return;
    
            if ((state as AirborneFighterAirdashState).fsm.entity != _fighter.entity)
                return;
    
            var obj = _fighter.aesthetics.GetOGPrefabInfo("Airdash");
            var ad = _fighter.aesthetics.SpawnEntityFromPool("Airdash", transform.position + Quaternion.AngleAxis(transform.eulerAngles.z, Vector3.forward) * obj.transform.position, transform.rotation * obj.transform.rotation, obj.transform.localScale);
        }
        public void RandomRangeSound(HitObject hit)
        {
            if (hit.box.entity.stateVars.invulnerable || hit.box is ShieldBox)
                return;
            if (hit.box.entity.gameObject.CompareTag("Floating"))
                return;
            var hitHitbox = 1f;
            if (hit.box is HitBox)
                hitHitbox = 0.25f;
            var point = hit.hitCol.ClosestPoint((Vector2)hit.hitBy.transform.position + hit.hitBy.offset);
            var sound = _fighter.aesthetics.SpawnFromPool("SoundRandom", point, Quaternion.identity, Vector3.one);
            if (sound.TryGetComponent(out AudioSource audio))
            {
                audio.pitch = (1.25f - Random.value * 0.5f) * hitHitbox;
            }
        }
    }
    
    
    public class GroundedFighterSpecialState : GroundedFighterState
    {
        public GroundedFighterSpecialState(FighterSM fsm) : base(fsm)
        {
            lag = true;
        }
    
        public override void OnStateUpdate(float timeScale)
        {
            base.OnStateUpdate(timeScale);
            if (FrameNum < 30)
                WaveBounce();
        }
    
        protected virtual void WaveBounce()
        {
            if (FlippedLeft != Controlling.moveStick.raw.x < 0 && Mathf.Abs(Controlling.moveStick.raw.x) > 0.1)
            {
                IndieSpd *= new Vector2(-0.8f, 1);
                SelfSpd *= new Vector2(-0.8f, 1);
                FlippedLeft = Controlling.moveStick.raw.x < 0;
            }
        }
    }
    public class AirborneFighterSpecialState : AirborneFighterState
    {
        public AirborneFighterSpecialState(FighterSM fsm) : base(fsm)
        {
            lag = true;
        }
    
        public override void OnStateUpdate(float timeScale)
        {
            base.OnStateUpdate(timeScale);
            if (FrameNum < 30)
                WaveBounce();
        }
    
        protected virtual void WaveBounce()
        {
            if (FlippedLeft != Controlling.moveStick.raw.x < 0 && Mathf.Abs(Controlling.moveStick.raw.x) > 0.1)
            {
                IndieSpd *= new Vector2(-0.8f, 1);
                SelfSpd *= new Vector2(-0.8f, 1);
                FlippedLeft = Controlling.moveStick.raw.x < 0;
            }
        }
    }
    
    public class GroundedFighterSpecial8DirectionalChargeStartState : GroundedFighterSpecialState
    {
        public GroundedFighterSpecial8DirectionalChargeStartState(FighterSM fsm) : base(fsm)
        {
            lag = true;
        }
    
        public override void OnStateEnter()
        {
            base.OnStateEnter();
            var currentFrame = FrameNum;
            fsm.fighter.entity.PlayAnim("Ground_Special_8Directional_Charge");
            fsm.fighter.entity.SkipCurrentAnimToFrame(currentFrame);
        }
        public override void OnStateUpdate(float timeScale)
        {
            base.OnStateUpdate(timeScale);
            RecordSpecialAim();
        }
    
        protected override void TransitionAir()
        {
            IndieSpd += ExternalSpd;
            ExternalSpd = Vector2.zero;
            PlatformInvuln = 0.2f;
            SetState(new AirborneFighterSpecial8DirectionalChargeStartState(fsm));
        }
    }
    public class AirborneFighterSpecial8DirectionalChargeStartState : AirborneFighterSpecialState
    {
        public AirborneFighterSpecial8DirectionalChargeStartState(FighterSM fsm) : base(fsm)
        {
            lag = true;
        }
        public override void OnStateEnter()
        {
            base.OnStateEnter();
            var currentFrame = FrameNum;
            fsm.fighter.entity.PlayAnim("Air_Special_8Directional_Charge");
            fsm.fighter.entity.SkipCurrentAnimToFrame(currentFrame);
        }
        public override void OnStateUpdate(float timeScale)
        {
            base.OnStateUpdate(timeScale);
            RecordSpecialAim();
        }
    
        public override void TransitionGround()
        {
            PlatformInvuln = 0;
            SetState(new GroundedFighterSpecial8DirectionalChargeStartState(fsm));
        }
    }
    
    public class GroundedFighterSpecial8DirectionalChargeReadyState : GroundedFighterSpecialState
    {
        public GroundedFighterSpecial8DirectionalChargeReadyState(FighterSM fsm) : base(fsm)
        {
            lag = true;
        }
    
        public override void OnStateEnter()
        {
            base.OnStateEnter();
            var currentFrame = FrameNum;
            fsm.fighter.entity.PlayAnim("Ground_Special_8Directional_Charge");
            fsm.fighter.entity.SkipCurrentAnimToFrame(currentFrame);
        }
    
        public override void OnStateUpdate(float timeScale)
        {
            base.OnStateUpdate(timeScale);
            RecordSpecialAim();
            SpecialCharge += Time.fixedDeltaTime * timeScale;
            if (SpecialCancelCondition)
                fsm.fighter.extension.TransitionSpecia8DirShoot();
        }
    
        protected override void TransitionAir()
        {
            IndieSpd += ExternalSpd;
            ExternalSpd = Vector2.zero;
            PlatformInvuln = 0.2f;
            SetState(new AirborneFighterSpecial8DirectionalChargeReadyState(fsm));
        }
        protected override void WaveBounce()
        {
            //base.WaveBounce();
        }
    }
    public class AirborneFighterSpecial8DirectionalChargeReadyState : AirborneFighterSpecialState
    {
        public AirborneFighterSpecial8DirectionalChargeReadyState(FighterSM fsm) : base(fsm)
        {
            lag = true;
        }
        public override void OnStateEnter()
        {
            base.OnStateEnter();
            var currentFrame = FrameNum;
            fsm.fighter.entity.PlayAnim("Air_Special_8Directional_Charge");
            fsm.fighter.entity.SkipCurrentAnimToFrame(currentFrame);
        }
    
        public override void OnStateUpdate(float timeScale)
        {
            base.OnStateUpdate(timeScale);
            RecordSpecialAim();
            SpecialCharge += Time.fixedDeltaTime * timeScale;
            if (SpecialCancelCondition)
                fsm.fighter.extension.TransitionSpecia8DirShoot();
        }
    
        public override void TransitionGround()
        {
            PlatformInvuln = 0;
            SetState(new GroundedFighterSpecial8DirectionalChargeReadyState(fsm));
        }
        protected override void WaveBounce()
        {
            //base.WaveBounce();
        }
    }
    
    public class GroundedFighterSpecial8DirectionalShootState : GroundedFighterSpecialState
    {
        public GroundedFighterSpecial8DirectionalShootState(FighterSM fsm) : base(fsm)
        {
            lag = true;
        }
    
        public override void OnStateEnter()
        {
            base.OnStateEnter();
            var currentFrame = FrameNum;
            fsm.fighter.entity.PlayAnim("Ground_Special_8Directional_Shoot_Right");
    
            fsm.fighter.entity.SkipCurrentAnimToFrame(currentFrame);
        }
    
        protected override void TransitionAir()
        {
            IndieSpd += ExternalSpd;
            ExternalSpd = Vector2.zero;
            PlatformInvuln = 0.2f;
            SetState(new AirborneFighterSpecial8DirectionalShootState(fsm));
        }
    
        protected override void WaveBounce()
        {
            //base.WaveBounce();
        }
    }
    public class AirborneFighterSpecial8DirectionalShootState : AirborneFighterSpecialState
    {
        public AirborneFighterSpecial8DirectionalShootState(FighterSM fsm) : base(fsm)
        {
            lag = true;
        }
        public override void OnStateEnter()
        {
            base.OnStateEnter();
            var currentFrame = FrameNum;
            fsm.fighter.entity.PlayAnim("Air_Special_8Directional_Shoot_Right");
    
            fsm.fighter.entity.SkipCurrentAnimToFrame(currentFrame);
        }
    
        public override void TransitionGround()
        {
            PlatformInvuln = 0;
            SetState(new GroundedFighterSpecial8DirectionalShootState(fsm));
        }
        protected override void WaveBounce()
        {
            //base.WaveBounce();
        }
    }
    
    
    public class GroundedFighterSpecial8DirectionalShootRightState : GroundedFighterSpecial8DirectionalShootState
    {
        public GroundedFighterSpecial8DirectionalShootRightState(FighterSM fsm) : base(fsm)
        {
            lag = true;
        }
    
        public override void OnStateEnter()
        {
            base.OnStateEnter();
            var currentFrame = FrameNum;
            fsm.fighter.entity.PlayAnim("Ground_Special_8Directional_Shoot_Right");
    
            fsm.fighter.entity.SkipCurrentAnimToFrame(currentFrame);
        }
    
        protected override void TransitionAir()
        {
            IndieSpd += ExternalSpd;
            ExternalSpd = Vector2.zero;
            PlatformInvuln = 0.2f;
            SetState(new AirborneFighterSpecial8DirectionalShootRightState(fsm));
        }
    }
    public class AirborneFighterSpecial8DirectionalShootRightState : AirborneFighterSpecial8DirectionalShootState
    {
        public AirborneFighterSpecial8DirectionalShootRightState(FighterSM fsm) : base(fsm)
        {
            lag = true;
        }
        public override void OnStateEnter()
        {
            base.OnStateEnter();
            var currentFrame = FrameNum;
            fsm.fighter.entity.PlayAnim("Air_Special_8Directional_Shoot_Right");
    
            fsm.fighter.entity.SkipCurrentAnimToFrame(currentFrame);
        }
    
        public override void TransitionGround()
        {
            PlatformInvuln = 0;
            SetState(new GroundedFighterSpecial8DirectionalShootRightState(fsm));
        }
    }
    
    public class GroundedFighterSpecial8DirectionalShootUpRightState : GroundedFighterSpecial8DirectionalShootState
    {
        public GroundedFighterSpecial8DirectionalShootUpRightState(FighterSM fsm) : base(fsm)
        {
            lag = true;
        }
    
        public override void OnStateEnter()
        {
            base.OnStateEnter();
            var currentFrame = FrameNum;
            fsm.fighter.entity.PlayAnim("Ground_Special_8Directional_Shoot_UpRight");
    
            fsm.fighter.entity.SkipCurrentAnimToFrame(currentFrame);
        }
    
        protected override void TransitionAir()
        {
            IndieSpd += ExternalSpd;
            ExternalSpd = Vector2.zero;
            PlatformInvuln = 0.2f;
            SetState(new AirborneFighterSpecial8DirectionalShootUpRightState(fsm));
        }
    }
    public class AirborneFighterSpecial8DirectionalShootUpRightState : AirborneFighterSpecial8DirectionalShootState
    {
        public AirborneFighterSpecial8DirectionalShootUpRightState(FighterSM fsm) : base(fsm)
        {
            lag = true;
        }
        public override void OnStateEnter()
        {
            base.OnStateEnter();
            var currentFrame = FrameNum;
            fsm.fighter.entity.PlayAnim("Air_Special_8Directional_Shoot_UpRight");
    
            fsm.fighter.entity.SkipCurrentAnimToFrame(currentFrame);
        }
    
        public override void TransitionGround()
        {
            PlatformInvuln = 0;
            SetState(new GroundedFighterSpecial8DirectionalShootUpRightState(fsm));
        }
    }
    
    public class GroundedFighterSpecial8DirectionalShootUpState : GroundedFighterSpecial8DirectionalShootState
    {
        public GroundedFighterSpecial8DirectionalShootUpState(FighterSM fsm) : base(fsm)
        {
            lag = true;
        }
    
        public override void OnStateEnter()
        {
            base.OnStateEnter();
            var currentFrame = FrameNum;
            fsm.fighter.entity.PlayAnim("Ground_Special_8Directional_Shoot_Up");
    
            fsm.fighter.entity.SkipCurrentAnimToFrame(currentFrame);
        }
    
        protected override void TransitionAir()
        {
            IndieSpd += ExternalSpd;
            ExternalSpd = Vector2.zero;
            PlatformInvuln = 0.2f;
            SetState(new AirborneFighterSpecial8DirectionalShootUpState(fsm));
        }
    }
    public class AirborneFighterSpecial8DirectionalShootUpState : AirborneFighterSpecial8DirectionalShootState
    {
        public AirborneFighterSpecial8DirectionalShootUpState(FighterSM fsm) : base(fsm)
        {
            lag = true;
        }
        public override void OnStateEnter()
        {
            base.OnStateEnter();
            var currentFrame = FrameNum;
            fsm.fighter.entity.PlayAnim("Air_Special_8Directional_Shoot_Up");
    
            fsm.fighter.entity.SkipCurrentAnimToFrame(currentFrame);
        }
    
        public override void TransitionGround()
        {
            PlatformInvuln = 0;
            SetState(new GroundedFighterSpecial8DirectionalShootUpState(fsm));
        }
    }
    
    public class GroundedFighterSpecial8DirectionalShootDownRightState : GroundedFighterSpecial8DirectionalShootState
    {
        public GroundedFighterSpecial8DirectionalShootDownRightState(FighterSM fsm) : base(fsm)
        {
            lag = true;
        }
    
        public override void OnStateEnter()
        {
            base.OnStateEnter();
            var currentFrame = FrameNum;
            fsm.fighter.entity.PlayAnim("Ground_Special_8Directional_Shoot_DownRight");
    
            fsm.fighter.entity.SkipCurrentAnimToFrame(currentFrame);
        }
    
        protected override void TransitionAir()
        {
            IndieSpd += ExternalSpd;
            ExternalSpd = Vector2.zero;
            PlatformInvuln = 0.2f;
            SetState(new AirborneFighterSpecial8DirectionalShootDownRightState(fsm));
        }
    }
    public class AirborneFighterSpecial8DirectionalShootDownRightState : AirborneFighterSpecial8DirectionalShootState
    {
        public AirborneFighterSpecial8DirectionalShootDownRightState(FighterSM fsm) : base(fsm)
        {
            lag = true;
        }
        public override void OnStateEnter()
        {
            base.OnStateEnter();
            var currentFrame = FrameNum;
            fsm.fighter.entity.PlayAnim("Air_Special_8Directional_Shoot_DownRight");
    
            fsm.fighter.entity.SkipCurrentAnimToFrame(currentFrame);
        }
    
        public override void TransitionGround()
        {
            PlatformInvuln = 0;
            SetState(new GroundedFighterSpecial8DirectionalShootDownRightState(fsm));
        }
    }
    
    public class GroundedFighterSpecial8DirectionalShootDownState : GroundedFighterSpecial8DirectionalShootState
    {
        public GroundedFighterSpecial8DirectionalShootDownState(FighterSM fsm) : base(fsm)
        {
            lag = true;
        }
    
        public override void OnStateEnter()
        {
            base.OnStateEnter();
            var currentFrame = FrameNum;
            fsm.fighter.entity.PlayAnim("Ground_Special_8Directional_Shoot_Down");
    
            fsm.fighter.entity.SkipCurrentAnimToFrame(currentFrame);
        }
    
        protected override void TransitionAir()
        {
            IndieSpd += ExternalSpd;
            ExternalSpd = Vector2.zero;
            PlatformInvuln = 0.2f;
            SetState(new AirborneFighterSpecial8DirectionalShootDownState(fsm));
        }
    }
    public class AirborneFighterSpecial8DirectionalShootDownState : AirborneFighterSpecial8DirectionalShootState
    {
        public AirborneFighterSpecial8DirectionalShootDownState(FighterSM fsm) : base(fsm)
        {
            lag = true;
        }
        public override void OnStateEnter()
        {
            base.OnStateEnter();
            var currentFrame = FrameNum;
            fsm.fighter.entity.PlayAnim("Air_Special_8Directional_Shoot_Down");
    
            fsm.fighter.entity.SkipCurrentAnimToFrame(currentFrame);
        }
    
        public override void TransitionGround()
        {
            PlatformInvuln = 0;
            SetState(new GroundedFighterSpecial8DirectionalShootDownState(fsm));
        }
    }
    
    public class GroundedFighterSpecialChargeStartState : GroundedFighterSpecialState
    {
        public GroundedFighterSpecialChargeStartState(FighterSM fsm) : base(fsm)
        {
            lag = true;
        }
    
        public override void OnStateEnter()
        {
            base.OnStateEnter();
            var currentFrame = FrameNum;
            fsm.fighter.entity.PlayAnim("Ground_Special_Charge");
            fsm.fighter.entity.SkipCurrentAnimToFrame(currentFrame);
        }
        public override void OnStateUpdate(float timeScale)
        {
            base.OnStateUpdate(timeScale);
            RecordSpecialAim();
        }
    
        protected override void TransitionAir()
        {
            IndieSpd += ExternalSpd;
            ExternalSpd = Vector2.zero;
            PlatformInvuln = 0.2f;
            SetState(new AirborneFighterSpecialChargeStartState(fsm));
        }
    }
    public class AirborneFighterSpecialChargeStartState : AirborneFighterSpecialState
    {
        public AirborneFighterSpecialChargeStartState(FighterSM fsm) : base(fsm)
        {
            lag = true;
        }
        public override void OnStateEnter()
        {
            base.OnStateEnter();
            var currentFrame = FrameNum;
            fsm.fighter.entity.PlayAnim("Air_Special_Charge");
            fsm.fighter.entity.SkipCurrentAnimToFrame(currentFrame);
        }
        public override void OnStateUpdate(float timeScale)
        {
            base.OnStateUpdate(timeScale);
            RecordSpecialAim();
        }
    
        public override void TransitionGround()
        {
            PlatformInvuln = 0;
            SetState(new GroundedFighterSpecialChargeStartState(fsm));
        }
    }
    public class GroundedFighterSpecialChargeReadyState : GroundedFighterSpecialState
    {
        public GroundedFighterSpecialChargeReadyState(FighterSM fsm) : base(fsm)
        {
            lag = true;
        }
    
        public override void OnStateEnter()
        {
            base.OnStateEnter();
            var currentFrame = FrameNum;
            fsm.fighter.entity.PlayAnim("Ground_Special_Charge");
            fsm.fighter.entity.SkipCurrentAnimToFrame(currentFrame);
        }
        public override void OnStateUpdate(float timeScale)
        {
            base.OnStateUpdate(timeScale);
            RecordSpecialAim();
            SpecialCharge += Time.fixedDeltaTime * timeScale;
            TransitionSpeclialShoot();
        }
        protected virtual void TransitionSpeclialShoot()
        {
            if (SpecialCancelCondition)
                fsm.fighter.extension.TransitionSpeciaShoot();
        }
        protected override void TransitionAir()
        {
            IndieSpd += ExternalSpd;
            ExternalSpd = Vector2.zero;
            PlatformInvuln = 0.2f;
            SetState(new AirborneFighterSpecialChargeReadyState(fsm));
        }
        protected override void WaveBounce()
        {
            //base.WaveBounce();
        }
    }
    public class AirborneFighterSpecialChargeReadyState : AirborneFighterSpecialState
    {
        public AirborneFighterSpecialChargeReadyState(FighterSM fsm) : base(fsm)
        {
            lag = true;
        }
        public override void OnStateEnter()
        {
            base.OnStateEnter();
            var currentFrame = FrameNum;
            fsm.fighter.entity.PlayAnim("Air_Special_Charge");
            fsm.fighter.entity.SkipCurrentAnimToFrame(currentFrame);
        }
    
        public override void OnStateUpdate(float timeScale)
        {
            base.OnStateUpdate(timeScale);
            RecordSpecialAim();
            SpecialCharge += Time.fixedDeltaTime * timeScale;
            TransitionSpecialShoot();
        }
    
        protected virtual void TransitionSpecialShoot()
        {
            if (SpecialCancelCondition)
                fsm.fighter.extension.TransitionSpeciaShoot();
        }
    
        public override void TransitionGround()
        {
            PlatformInvuln = 0;
            SetState(new GroundedFighterSpecialChargeReadyState(fsm));
        }
        protected override void WaveBounce()
        {
            //base.WaveBounce();
        }
    }
    
    public class GroundedFighterSpecialShootState : GroundedFighterSpecialState
    {
        public GroundedFighterSpecialShootState(FighterSM fsm) : base(fsm)
        {
            lag = true;
        }
    
        public override void OnStateEnter()
        {
            base.OnStateEnter();
            var currentFrame = FrameNum;
            fsm.fighter.entity.PlayAnim("Ground_Special_Shoot");
            fsm.fighter.entity.SkipCurrentAnimToFrame(currentFrame);
        }
    
        protected override void TransitionAir()
        {
            IndieSpd += ExternalSpd;
            ExternalSpd = Vector2.zero;
            PlatformInvuln = 0.2f;
            SetState(new AirborneFighterSpecialShootState(fsm));
        }
        protected override void WaveBounce()
        {
            //base.WaveBounce();
        }
    }
    public class AirborneFighterSpecialShootState : AirborneFighterSpecialState
    {
        public AirborneFighterSpecialShootState(FighterSM fsm) : base(fsm)
        {
            lag = true;
        }
        public override void OnStateEnter()
        {
            base.OnStateEnter();
            var currentFrame = FrameNum;
            fsm.fighter.entity.PlayAnim("Air_Special_Shoot");
            fsm.fighter.entity.SkipCurrentAnimToFrame(currentFrame);
        }
    
        public override void TransitionGround()
        {
            PlatformInvuln = 0;
            SetState(new GroundedFighterSpecialShootState(fsm));
        }
        protected override void WaveBounce()
        {
            //base.WaveBounce();
        }
    }
    
    
    public class GroundedFighterSpecialNoMoveShootState : GroundedFighterSpecialState
    {
        public GroundedFighterSpecialNoMoveShootState(FighterSM fsm) : base(fsm)
        {
            lag = true;
        }
        public override void OnStateEnter()
        {
            base.OnStateEnter();
            var currentFrame = FrameNum;
            fsm.fighter.entity.PlayAnim("Air_Special_NoMove_Shoot");
            fsm.fighter.entity.SkipCurrentAnimToFrame(currentFrame);
        }
        protected override void TransitionAir()
        {
            IndieSpd += ExternalSpd;
            ExternalSpd = Vector2.zero;
            PlatformInvuln = 0.2f;
            SetState(new AirborneFighterSpecialNoMoveShootState(fsm));
        }
    }
    public class AirborneFighterSpecialNoMoveShootState : AirborneFighterSpecialState
    {
        public AirborneFighterSpecialNoMoveShootState(FighterSM fsm) : base(fsm)
        {
            lag = true;
            moveable = false;
        }
        public override void OnStateEnter()
        {
            base.OnStateEnter();
            var currentFrame = FrameNum;
            fsm.fighter.entity.PlayAnim("Air_Special_NoMove_Shoot");
            fsm.fighter.entity.SkipCurrentAnimToFrame(currentFrame);
        }
        public override void TransitionGround()
        {
            PlatformInvuln = 0;
            SetState(new GroundedFighterSpecialNoMoveShootState(fsm));
        }
    }
    
    
    public class GroundedFighterSpellcardShootState : GroundedFighterState
    {
        public GroundedFighterSpellcardShootState(FighterSM fsm) : base(fsm)
        {
            lag = true;
        }
    
        public override void OnStateEnter()
        {
            base.OnStateEnter();
            fsm.fighter.entity.PlayAnim("Spellcard_Shoot");
            IndieSpd *= 0;
            SelfSpd *= 0;
            Gravity = false;
        }
    
        public override void OnStateUpdate(float timeScale)
        {
            base.OnStateUpdate(timeScale);
            IndieSpd *= 0;
            SelfSpd *= 0;
        }
    
        public override void OnStateExit()
        {
            base.OnStateExit();
            Gravity = true;
        }
    
        protected override void TransitionAir()
        {
            //base.TransitionAir();
        }
    }
    public class GroundedFighterSpellcardCharge2StartState : GroundedFighterState
    {
        public GroundedFighterSpellcardCharge2StartState(FighterSM fsm) : base(fsm)
        {
            lag = true;
        }
    
        public override void OnStateEnter()
        {
            base.OnStateEnter();
            fsm.fighter.entity.PlayAnim("Spellcard_Charge_2");
            IndieSpd *= 0;
            SelfSpd *= 0;
            Gravity = false;
        }
        public override void OnStateUpdate(float timeScale)
        {
            base.OnStateUpdate(timeScale);
            IndieSpd *= 0;
            SelfSpd *= 0;
        }
    
        public override void OnStateExit()
        {
            base.OnStateExit();
            Gravity = true;
        }
    
        protected override void TransitionAir()
        {
            //base.TransitionAir();
        }
    }
    public class GroundedFighterSpellcardCharge2ReadyState : GroundedFighterState
    {
        public GroundedFighterSpellcardCharge2ReadyState(FighterSM fsm) : base(fsm)
        {
            lag = true;
        }
    
        public override void OnStateEnter()
        {
            base.OnStateEnter();
            Gravity = false;
        }
    
        public override void OnStateUpdate(float timeScale)
        {
            base.OnStateUpdate(timeScale);
            IndieSpd *= 0;
            SelfSpd *= 0;
            SpecialCharge += Time.fixedDeltaTime * timeScale;
            if (SpecialCancelCondition)
                SetState(new GroundedFighterSpellcardShoot2State(fsm));
        }
    
        public override void OnStateExit()
        {
            base.OnStateExit();
            Gravity = true;
        }
    
        protected override void TransitionAir()
        {
            //base.TransitionAir();
        }
    }
    public class GroundedFighterSpellcardShoot2State : GroundedFighterState
    {
        public GroundedFighterSpellcardShoot2State(FighterSM fsm) : base(fsm)
        {
            lag = true;
        }
    
        public override void OnStateEnter()
        {
            base.OnStateEnter();
            fsm.fighter.entity.PlayAnim("Spellcard_Shoot_2");
            IndieSpd *= 0;
            SelfSpd *= 0;
            Gravity = false;
        }
        public override void OnStateUpdate(float timeScale)
        {
            base.OnStateUpdate(timeScale);
            IndieSpd *= 0;
            SelfSpd *= 0;
        }
    
        public override void OnStateExit()
        {
            base.OnStateExit();
            Gravity = true;
        }
    
        protected override void TransitionAir()
        {
            //base.TransitionAir();
        }
    }
    public class GroundedFighterSpellcardAttackState : GroundedFighterState
    {
        public GroundedFighterSpellcardAttackState(FighterSM fsm) : base(fsm)
        {
            lag = true;
        }
    
        public override void OnStateEnter()
        {
            base.OnStateEnter();
            fsm.fighter.entity.PlayAnim("Spellcard_Attack");
            IndieSpd *= 0;
            SelfSpd *= 0;
            Gravity = false;
        }
        public override void OnStateUpdate(float timeScale)
        {
            base.OnStateUpdate(timeScale);
            IndieSpd *= 0;
            SelfSpd *= 0;
        }
    
        public override void OnStateExit()
        {
            base.OnStateExit();
            Gravity = true;
        }
    
        protected override void TransitionAir()
        {
            //base.TransitionAir();
        }
    }
    
    public class GroundedFighterSpecial2DirectionalShootUpState : GroundedFighterSpecial8DirectionalShootState
    {
        public GroundedFighterSpecial2DirectionalShootUpState(FighterSM fsm) : base(fsm)
        {
            lag = true;
        }
    
        public override void OnStateEnter()
        {
            base.OnStateEnter();
            var currentFrame = FrameNum;
            fsm.fighter.entity.PlayAnim("Ground_Special_2Directional_Shoot_Up");
    
            fsm.fighter.entity.SkipCurrentAnimToFrame(currentFrame);
        }
    
        protected override void TransitionAir()
        {
            IndieSpd += ExternalSpd;
            ExternalSpd = Vector2.zero;
            PlatformInvuln = 0.2f;
            SetState(new AirborneFighterSpecial2DirectionalShootUpState(fsm));
        }
    }
    public class AirborneFighterSpecial2DirectionalShootUpState : AirborneFighterSpecial8DirectionalShootState
    {
        public AirborneFighterSpecial2DirectionalShootUpState(FighterSM fsm) : base(fsm)
        {
            lag = true;
        }
        public override void OnStateEnter()
        {
            base.OnStateEnter();
            var currentFrame = FrameNum;
            fsm.fighter.entity.PlayAnim("Air_Special_2Directional_Shoot_Up");
    
            fsm.fighter.entity.SkipCurrentAnimToFrame(currentFrame);
        }
    
        public override void TransitionGround()
        {
            PlatformInvuln = 0;
            SetState(new GroundedFighterSpecial2DirectionalShootUpState(fsm));
        }
    }
    public class GroundedFighterSpecial2DirectionalShootDownState : GroundedFighterSpecial8DirectionalShootState
    {
        public GroundedFighterSpecial2DirectionalShootDownState(FighterSM fsm) : base(fsm)
        {
            lag = true;
        }
    
        public override void OnStateEnter()
        {
            base.OnStateEnter();
            var currentFrame = FrameNum;
            fsm.fighter.entity.PlayAnim("Ground_Special_2Directional_Shoot_Down");
    
            fsm.fighter.entity.SkipCurrentAnimToFrame(currentFrame);
        }
    
        protected override void TransitionAir()
        {
            IndieSpd += ExternalSpd;
            ExternalSpd = Vector2.zero;
            PlatformInvuln = 0.2f;
            SetState(new AirborneFighterSpecial2DirectionalShootDownState(fsm));
        }
    }
    public class AirborneFighterSpecial2DirectionalShootDownState : AirborneFighterSpecial8DirectionalShootState
    {
        public AirborneFighterSpecial2DirectionalShootDownState(FighterSM fsm) : base(fsm)
        {
            lag = true;
        }
        public override void OnStateEnter()
        {
            base.OnStateEnter();
            var currentFrame = FrameNum;
            fsm.fighter.entity.PlayAnim("Air_Special_2Directional_Shoot_Down");
    
            fsm.fighter.entity.SkipCurrentAnimToFrame(currentFrame);
        }
    
        public override void TransitionGround()
        {
            PlatformInvuln = 0;
            SetState(new GroundedFighterSpecial2DirectionalShootDownState(fsm));
        }
    }
    public class AirborneFighterSpecial3DirectionalShootSideState : AirborneFighterSpecial8DirectionalShootState
    {
        public AirborneFighterSpecial3DirectionalShootSideState(FighterSM fsm) : base(fsm)
        {
            lag = true;
        }
        public override void OnStateEnter()
        {
            base.OnStateEnter();
            var currentFrame = FrameNum;
            fsm.fighter.entity.PlayAnim("Air_Special_3Directional_Shoot_Side");
    
            fsm.fighter.entity.SkipCurrentAnimToFrame(currentFrame);
        }
    
        public override void TransitionGround()
        {
            PlatformInvuln = 0;
            SetState(new GroundedFighterSpecial3DirectionalShootSideState(fsm));
        }
    }
    public class GroundedFighterSpecial3DirectionalShootSideState : GroundedFighterSpecial8DirectionalShootState
    {
        public GroundedFighterSpecial3DirectionalShootSideState(FighterSM fsm) : base(fsm)
        {
            lag = true;
        }
    
        public override void OnStateEnter()
        {
            base.OnStateEnter();
            var currentFrame = FrameNum;
            fsm.fighter.entity.PlayAnim("Ground_Special_3Directional_Shoot_Side");
    
            fsm.fighter.entity.SkipCurrentAnimToFrame(currentFrame);
        }
    
        protected override void TransitionAir()
        {
            IndieSpd += ExternalSpd;
            ExternalSpd = Vector2.zero;
            PlatformInvuln = 0.2f;
            SetState(new AirborneFighterSpecial3DirectionalShootSideState(fsm));
        }
    }}
