namespace TightStuff.Extension
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using UnityEngine;
    using UnityEngine.SocialPlatforms;
    using UnityEngine.UIElements;
    using static FighterState;
    using static UnityEngine.EventSystems.EventTrigger;
    using static UnityEngine.ParticleSystem;
    using Random = UnityEngine.Random;
    
    public class MarisaFighterExtension : BaseFighterExtensions
    {
        [SerializeField]
        private List<BaseProjectileBehaviour> _dSmashBombs;
        [SerializeField]
        private List<Entity> _dSpecialVisuals;
        private List<HitBox> _dSmashBombHits = new List<HitBox>();
        [SerializeField]
        private List<BaseProjectileBehaviour> _nonDirLasers;
        private HitProperties _initDSmashBombProperties;
    
        [SerializeField]
        private Transform _bombHang;
    
        [SerializeField]
        private Entity _nonDirLaserCircle;
        [SerializeField]
        private float _nonDirLaserSpinSpd = 10;
        [SerializeField]
        private float _bombCooldown = 2;
    
        [SerializeField]
        private Color _targetPotionColor;
        private Color _initPotionColor;
        [SerializeField]
        private Color _targetPotionLightColor;
        private Color _initPotionLightColor;
    
        [SerializeField]
        private AudioSource _rocket;
        [SerializeField]
        private AudioSource _rocketCharge;
    
        protected bool TapSmash { get => _fighter.controlling.moveStick.BufferTap() && _fighter.controlling.attackButton.Buffer() && _fighter.controlling.tapSmash; }
        private bool Smash => TapSmash || _fighter.controlling.smashStick.BufferHold() || _fighter.controlling.smashButton.Buffer();
        private bool Attack => TapSmash || Smash || _fighter.controlling.attackStick.BufferHold() || _fighter.controlling.attackButton.Buffer();
        private bool Shielding { get => _fighter.ActionState.GetState() is GroundedFighterShieldState; }
        public bool DropBomb { get => _fighter.controlling.grabButton.Buffer() || (Attack && Shielding) || (Attack && _fighter.controlling.shieldButton.raw); }
        public float CurrentPotionCD => _dSpecialVisuals[1].stateVars.damageArmor;
        public float PotionCD => _bombCooldown;
    
        protected override void Start()
        {
            base.Start();
            //ActionSM.OnStateEnter += ReplaceAttackWithThrow;
            ActionSM.OnStateEnter += StardustReverieSound;
            ActionSM.OnStateUpdate += MasterSpark;
            GameObject[] masterSparks = GameObject.FindGameObjectsWithTag("MasterSpark");
            foreach (GameObject masterSpark in masterSparks)
                masterSpark.GetComponentInChildren<HitBox>().OnHit.AddListener(RandomRangeSound);
            foreach (BaseProjectileBehaviour bomb in _dSmashBombs)
            {
                bomb.transform.parent = null;
                bomb.entity.actionState.SetState(new ProjectileAirborneNoPlatformOverrideState(bomb.entity.actionState as ProjectileSM));
                _dSmashBombHits.Add(bomb.GetComponent<Pooler>().poolDict["Explosion"][0].GetComponentInChildren<HitBox>());
            }
            foreach (Entity bombVisual in _dSpecialVisuals)
            {
                bombVisual.transform.GetChild(0).parent = null;
                bombVisual.transform.parent = _bombHang;
                bombVisual.transform.localPosition *= 0;
                bombVisual.stateVars.genericTimer = 0;
            }
            _initDSmashBombProperties = _dSmashBombHits[0].hitProperties;
            _nonDirLaserCircle.transform.parent = null;
            order = 0;
        }
    
        public override void GUpdate()
        {
            base.GUpdate();
            _dSpecialVisuals[0].SetEntityActive(_fighter.stateVarsF.specialTimer >= (1 + (_dSpecialVisuals[0].stateVars.intangible ? 1 : 0)));
            _dSpecialVisuals[1].SetEntityActive(_fighter.stateVarsF.specialTimer >= (2 + (_dSpecialVisuals[0].stateVars.intangible ? 1 : 0)));
    
            if (_dSpecialVisuals[1].stateVars.genericTimer > _dSpecialVisuals[0].stateVars.genericTimer)
            {
                var temp = _dSpecialVisuals[1].stateVars.genericTimer;
                _dSpecialVisuals[1].stateVars.genericTimer = _dSpecialVisuals[0].stateVars.genericTimer;
                _dSpecialVisuals[0].stateVars.genericTimer = temp;
            }
            foreach (Entity bombVisual in _dSpecialVisuals)
            {
                if (bombVisual.stateVars.enabled)
                {
                    bombVisual.stateVars.genericTimer += Time.fixedDeltaTime * bombVisual.TrueTimeScale * 0.33f;
                    if (bombVisual.stateVars.genericTimer >= 1)
                        DSpecialPotionDrop();
                }
                if (bombVisual.stateVars.damageArmor > 0)
                    bombVisual.stateVars.damageArmor -= Time.fixedDeltaTime * bombVisual.TrueTimeScale;
            }
    
            if (DropBomb && _fighter.stateVarsF.specialTimer >= 1 && !(_fighter.ActionState.GetState() as FighterState).Lag)
            {
                if (Shielding)
                {
                    DSpecialPotionDrop();
                    ConsumeDropBombBuffer();
                }
                else
                {
                    var attackDir = (_fighter.ActionState.GetState() as FighterState).GetAttackDir(_fighter.controlling.grabButton, _fighter.controlling.cStick);
                    var command = AttackID.Neutral;
                    if (attackDir.y > 0.6f)
                        command = AttackID.Up; //Up
    
                    else if (attackDir.y < -0.6f)
                        command = AttackID.Down; //Down
    
                    else if (Mathf.Abs(attackDir.x) > 0.3f)
                    {
                        command = AttackID.Forward; //Forward
                        if ((_fighter.ActionState.GetState() as FighterState).MoveStickAgainstFlip(attackDir))
                            command = AttackID.Back; //Back
                    }
                    _fighter.stateVarsF.currentAttackID = command;
                    BombThrow();
                }
            }
            if (DropBomb && _fighter.stateVarsF.specialTimer >= 1 && _fighter.ActionState.GetState() is AirborneFighterFocusSpinState)
            {
                DSpecialPotionDrop();
                ConsumeDropBombBuffer();
            }
    
            _fighter.stateVarsF.specialTimer = Mathf.Clamp(_fighter.stateVarsF.specialTimer, 0, 2);
            _nonDirLaserCircle.transform.position = transform.position - Vector3.up * 0.175f;
            _nonDirLaserCircle.transform.rotation = Quaternion.Euler(0, 0, _nonDirLaserCircle.transform.rotation.eulerAngles.z + 
                Mathf.Sign(_fighter.stateVarsF.specialCooldown) * (_nonDirLasers[0].LifeTime / _nonDirLasers[0].MaxLifeTime) * _fighter.entity.TrueTimeScale * Time.fixedDeltaTime * _nonDirLaserSpinSpd);
    
            if (_fighter.ActionState.GetState() is AirborneFighterSpecialMarisaRushState)
                _rocket.volume = Mathf.Lerp(_rocket.volume, 0.6f, Time.fixedDeltaTime * _fighter.entity.TrueTimeScale * 10);
            else
                _rocket.volume = Mathf.Lerp(_rocket.volume, 0, Time.fixedDeltaTime * _fighter.entity.TrueTimeScale * 5f);
    
            if (_fighter.ActionState.GetState() is AirborneFighterSpecialChargeReadyMarisaUpState || _fighter.ActionState.GetState() is AirborneFighterSpecialChargeStartMarisaUpState)
                _rocketCharge.volume = Mathf.Lerp(_rocketCharge.volume, 0.25f, Time.fixedDeltaTime * _fighter.entity.TrueTimeScale * 10);
            else
                _rocketCharge.volume = Mathf.Lerp(_rocketCharge.volume, 0, Time.fixedDeltaTime * _fighter.entity.TrueTimeScale * 5f);
        }
    
        public void ConsumeDropBombBuffer()
        {
            _fighter.controlling.grabButton.ConsumeBuffer();
            _fighter.controlling.attackButton.ConsumeBuffer();
            _fighter.controlling.smashButton.ConsumeBuffer();
            _fighter.controlling.smashStick.ConsumeBufferHold();
            _fighter.controlling.attackStick.ConsumeBufferHold();
            _fighter.controlling.smashStick.ConsumeBufferTap();
            _fighter.controlling.attackStick.ConsumeBufferTap();
        }
    
        protected override void OnDestroy()
        {
            base.OnDestroy();
            //ActionSM.OnStateEnter -= ReplaceAttackWithThrow;
            ActionSM.OnStateEnter -= StardustReverieSound;
            ActionSM.OnStateUpdate -= MasterSpark;
        }
    
        protected override void SpecialDown(Vector2 specialDir)
        {
            if (_fighter.stateVarsF.specialTimer >= 2 || _dSpecialVisuals[1].stateVars.damageArmor > _bombCooldown)
                return;
            if (Mathf.Abs(specialDir.x) > 0.175f)
                _fighter.entity.stateVars.flippedLeft = specialDir.x < 0;
            _fighter.stateVarsF.specialCharge = 0;
            if (Mathf.Abs(specialDir.x) > 0.175f)
                _fighter.entity.stateVars.flippedLeft = specialDir.x < 0;
            if (!_fighter.entity.stateVars.aerial)
            {
                _fighter.entity.PlayAnim("Ground_Special_Charge");
                _fighter.entity.actionState.SetState(new GroundedFighterSpecialChargeStartNoWavebounceState(_fighter.entity.actionState as FighterSM));
            }
            else
            {
                _fighter.entity.PlayAnim("Air_Special_Charge");
                _fighter.entity.actionState.SetState(new AirborneFighterSpecialChargeStartNoWavebounceState(_fighter.entity.actionState as FighterSM));
            }
            _fighter.stateVarsF.turning = false;
        }
    
        protected override void SpecialHorizontal(FighterState.AttackID specialID)
        {
            base.SpecialHorizontal(specialID);
            if (!_fighter.entity.stateVars.aerial)
            {
                _fighter.entity.PlayAnim("Ground_Special_Shoot");
                _fighter.entity.actionState.SetState(new GroundedFighterSpecialShootWaveBounceState(_fighter.entity.actionState as FighterSM));
            }
            else
            {
                _fighter.entity.PlayAnim("Air_Special_Shoot");
                _fighter.entity.actionState.SetState(new GroundedFighterSpecialShootWaveBounceState(_fighter.entity.actionState as FighterSM));
            }
            _fighter.stateVarsF.turning = false;
        }
    
        protected override void SpecialNeutral(Vector2 specialDir)
        {
            base.SpecialNeutral(specialDir);
            if (!_fighter.entity.stateVars.aerial)
            {
                _fighter.entity.PlayAnim("Ground_Special_8Directional_Charge");
                _fighter.entity.actionState.SetState(new GroundedFighterSpecial8DirectionalChargeStartState(_fighter.entity.actionState as FighterSM));
            }
            else
            {
                _fighter.entity.PlayAnim("Air_Special_8Directional_Charge");
                _fighter.entity.actionState.SetState(new AirborneFighterSpecial8DirectionalChargeStartState(_fighter.entity.actionState as FighterSM));
            }
            _fighter.stateVarsF.turning = false;
        }
    
        protected override void SpecialUp(Vector2 specialDir)
        {
            if (_fighter.stateVarsF.stamina < 1.5f || (_fighter.stateVarsF.stamina < 0.9f && _fighter.stateVarsF.maxStamina <= 1))
                return;
            base.SpecialUp(specialDir);
            _fighter.entity.actionState.SetState(new AirborneFighterSpecialChargeStartMarisaUpState(_fighter.entity.actionState as FighterSM));
        }
    
        protected override void SpellcardUp(Vector2 specialDir)
        {
            base.SpellcardDown(specialDir);
            _fighter.entity.actionState.SetState(new GroundedFighterSpellcardAttackState(_fighter.entity.actionState as FighterSM));
        }
    
        protected override void SpellcardHorizontal(FighterState.AttackID specialID)
        {
            base.SpellcardHorizontal(specialID);
            _fighter.entity.actionState.SetState(new GroundedFighterSpellcardShootState(_fighter.entity.actionState as FighterSM));
        }
    
        protected override void SpellcardDown(Vector2 specialDir)
        {
            if (Mathf.Abs(specialDir.x) > 0.175f)
                _fighter.entity.stateVars.flippedLeft = specialDir.x < 0;
            _fighter.stateVarsF.grazeMeter -= 6;
            _fighter.entity.actionState.SetState(new GroundedFighterSpellcardShoot2State(_fighter.entity.actionState as FighterSM));
        }
        private void AirBounce()
        {
            _fighter.entity.stateVars.indieSpd *= new Vector2(0.5f, 1);
            var ascending = _fighter.entity.stateVars.indieSpd.y > 0 ? 0.75f : 1;
            _fighter.entity.stateVars.indieSpd += new Vector2(0, _fighter.Ft.airdashSpd - Mathf.Clamp(_fighter.entity.stateVars.indieSpd.y, 0, _fighter.Ft.airdashSpd)) * ascending;
        }
        private void RushBounce()
        {
            _fighter.entity.stateVars.indieSpd *= new Vector2(0.8f, 1);
            var ascending = _fighter.entity.stateVars.indieSpd.y > 0 ? 0.75f : 1;
            _fighter.entity.stateVars.indieSpd += new Vector2(0, _fighter.Ft.airdashSpd - Mathf.Clamp(_fighter.entity.stateVars.indieSpd.y, 0, _fighter.Ft.airdashSpd)) * ascending;
        }
        private void SpecialNeutralStrongBounce()
        {
            if (_fighter.stateVarsF.specialAim.normalized.magnitude < 0.1)
                _fighter.entity.stateVars.indieSpd += (_fighter.entity.stateVars.flippedLeft ? Vector2.right : Vector2.left) * 8;
            else
                _fighter.entity.stateVars.indieSpd += -_fighter.stateVarsF.specialAim.normalized * 8;
        }
        private void AirBounceWeak()
        {
            _fighter.entity.stateVars.indieSpd *= new Vector2(0.5f, 1);
            var ascending = _fighter.entity.stateVars.indieSpd.y >= 0 ? 0.5f : 0.75f;
            _fighter.entity.stateVars.indieSpd += new Vector2(0, _fighter.Ft.airdashSpd - Mathf.Clamp(_fighter.entity.stateVars.indieSpd.y, 0, _fighter.Ft.airdashSpd)) * ascending;
        }
        private void BackAirRecoil()
        {
            var flip = _fighter.entity.stateVars.flippedLeft ? -0.75f : 0.75f;
            var refSpd = _fighter.entity.stateVars.flippedLeft ^ _fighter.entity.stateVars.indieSpd.x < 0 ? 0 : _fighter.entity.stateVars.indieSpd.x;
            _fighter.entity.stateVars.indieSpd += flip * new Vector2(_fighter.Ft.airdashSpd * 0.75f - Mathf.Clamp(Mathf.Abs(refSpd), 0, _fighter.Ft.airdashSpd) * 0.4f, 0);
            _fighter.entity.actionState.SetState(new AirborneFighterAttackNoMoveState(_fighter.entity.actionState as FighterSM));
        }
        private void TransitionAirLagMoveable()
        {
            _fighter.entity.actionState.SetState(new AirborneFighterAttackMoveState(_fighter.entity.actionState as FighterSM));
        }
        private void TransitionAirLagUnmoveable()
        {
            _fighter.entity.actionState.SetState(new AirborneFighterSpecialMarisaRushLagState(_fighter.entity.actionState as FighterSM));
        }
        private void ChangeAttackIDToBack()
        {
            _fighter.stateVarsF.currentAttackID = AttackID.Back;
        }
        private void FTiltCharge()
        {
            _fighter.entity.actionState.SetState(new GroundedFighterAttackMarisaChargeState(_fighter.entity.actionState as FighterSM));
        }
        private void FTiltRelease()
        {
            _fighter.entity.actionState.SetState(new GroundedFighterAttackMarisaReleaseState(_fighter.entity.actionState as FighterSM));
        }
    
        public void TransitionChargeReadyNoGround()
        {
            _fighter.entity.actionState.SetState(new AirborneFighterSpecialChargeReadyMarisaUpState(_fighter.entity.actionState as FighterSM));
        }
        public void TransitionMasterSparkAim()
        {
            _fighter.entity.actionState.SetState(new GroundedFighterSpellcardAimState(_fighter.entity.actionState as FighterSM));
        }
        public void TransitionMasterSpark()
        {
            _fighter.entity.actionState.SetState(new GroundedFighterSpellcardRemainAimState(_fighter.entity.actionState as FighterSM));
        }
        public void TransitionMasterSparkRecovery()
        {
            _fighter.entity.actionState.SetState(new GroundedFighterSpellcardReturnFromAimState(_fighter.entity.actionState as FighterSM));
        }
        public void TransitionMasterSparkStaticAim()
        {
            _fighter.entity.actionState.SetState(new GroundedFighterSpellcardStaticAimState(_fighter.entity.actionState as FighterSM));
        }
        public void TransitionMarisaUpSpecial()
        {
            _fighter.entity.actionState.SetState(new AirborneFighterSpecialMarisaRushState(_fighter.entity.actionState as FighterSM));
        }
        private void StardustReverieSound(State state)
        {
            if (!(state is AirborneFighterSpecialMarisaRushState))
                return;
            if ((state as FighterState).fsm.entity != _fighter.entity)
                return;
            SpawnSound("StardustReverie");
        }
        public void ReplaceAttackWithThrow(State state)
        {
            if (state is not GroundedFighterGrabState)
                return;
            if ((state as FighterState).fsm.entity != _fighter.entity)
                return;
            BombThrow();
        }
    
        private void BombThrow()
        {
            if (_fighter.stateVarsF.specialTimer <= 0)
                return;
            _fighter.stateVarsF.specialCharge = 1;
            if (Smash)
                _fighter.stateVarsF.specialCharge = 1.5f;
            if (!_fighter.entity.stateVars.aerial)
            {
                _fighter.entity.PlayAnim("Ground_Special_3Directional_Shoot_Side");
                _fighter.stateVarsF.specialAim = (Vector2.down + Vector2.right).normalized;
                _fighter.entity.actionState.SetState(new GroundedFighterSpecial3DirectionalShootSideState(_fighter.entity.actionState as FighterSM));
                if (_fighter.stateVarsF.currentAttackID == AttackID.Up)
                {
                    _fighter.entity.PlayAnim("Ground_Special_2Directional_Shoot_Up");
                    _fighter.stateVarsF.specialAim = Vector2.up;
                    _fighter.entity.actionState.SetState(new GroundedFighterSpecial2DirectionalShootUpState(_fighter.entity.actionState as FighterSM));
                    return;
                }
                if (_fighter.stateVarsF.currentAttackID == AttackID.Down)
                {
                    _fighter.entity.PlayAnim("Ground_Special_2Directional_Shoot_Down");
                    _fighter.stateVarsF.specialAim = Vector2.down;
                    _fighter.entity.actionState.SetState(new GroundedFighterSpecial2DirectionalShootDownState(_fighter.entity.actionState as FighterSM));
                    return;
                }
                return;
            }
            else
            {
                _fighter.entity.PlayAnim("Air_Special_3Directional_Shoot_Side");
                _fighter.stateVarsF.specialAim = Vector2.right;
                _fighter.entity.actionState.SetState(new AirborneFighterSpecial3DirectionalShootSideState(_fighter.entity.actionState as FighterSM));
    
                if (_fighter.stateVarsF.currentAttackID == AttackID.Back)
                    _fighter.entity.stateVars.flippedLeft = !_fighter.entity.stateVars.flippedLeft;
    
                if (_fighter.stateVarsF.currentAttackID == AttackID.Up)
                {
                    _fighter.entity.PlayAnim("Air_Special_2Directional_Shoot_Up");
                    _fighter.stateVarsF.specialAim = Vector2.up;
                    _fighter.entity.actionState.SetState(new AirborneFighterSpecial2DirectionalShootUpState(_fighter.entity.actionState as FighterSM));
                    return;
                }
                if (_fighter.stateVarsF.currentAttackID == AttackID.Down)
                {
                    _fighter.entity.PlayAnim("Air_Special_2Directional_Shoot_Down");
                    _fighter.stateVarsF.specialAim = Vector2.down;
                    _fighter.entity.actionState.SetState(new AirborneFighterSpecial2DirectionalShootDownState(_fighter.entity.actionState as FighterSM));
                    return;
                }
            }
        }
    
        public void MasterSpark(State state)
        {
            if (state is not GroundedFighterSpellcardRemainAimState)
                return;
            if ((state as FighterState).fsm.entity != _fighter.entity)
                return;
            var count = _fighter.stateVarsF.specialCount % 10;
            if (_fighter.stateVarsF.specialCount - count == _fighter.entity.stateVars.frameNum)
                return;
            _camShake.ShakeCamera(2, 0.5f);
            var shootDir = (_fighter.entity.stateVars.flippedLeft ? 1 : -1) * Vector2.Perpendicular(_fighter.stateVarsF.hurtboxRotation);
            var angle = Mathf.Atan2(shootDir.y, shootDir.x) * Mathf.Rad2Deg;
            var color = "Red";
            if (_fighter.stateVarsF.specialCount % 10 == 1)
                color = "Red";
            if (_fighter.stateVarsF.specialCount % 10 == 2)
                color = "Orange";
            if (_fighter.stateVarsF.specialCount % 10 == 3)
                color = "Yellow";
            if (_fighter.stateVarsF.specialCount % 10 == 4)
                color = "Green";
            if (_fighter.stateVarsF.specialCount % 10 == 5)
                color = "Blue";
            if (_fighter.stateVarsF.specialCount % 10 == 6)
                color = "Purple";
    
            _fighter.stateVarsF.specialCount = _fighter.entity.stateVars.frameNum + _fighter.stateVarsF.specialCount % 10 + 1;
            if (count > 6)
                _fighter.stateVarsF.specialCount = _fighter.entity.stateVars.frameNum;
    
    
            if (_fighter.entity.stateVars.frameNum % 30 == 0 || _fighter.entity.stateVars.frameNum % 50 == 0)
                _fighter.shooter.SpawnEntityFromPool("MasterSparkParticle" + color, _fighter.hurtbox[0].rotation * Vector3.Scale(_fighter.shooter.transform.localPosition, _fighter.transform.lossyScale) + _fighter.transform.position, Quaternion.Euler(0, 0, angle - 90), _fighter.transform.localScale * 2.5f, 10 * shootDir );
            else if (_fighter.entity.stateVars.frameNum % 20 == 0)
                _fighter.shooter.SpawnEntityFromPool("MasterSparkParticle" + color, _fighter.hurtbox[0].rotation * Vector3.Scale(_fighter.shooter.transform.localPosition, _fighter.transform.lossyScale) + _fighter.transform.position, Quaternion.Euler(0, 0, angle - 90), _fighter.transform.localScale * 2.5f, (10 + count) * -Vector2.Perpendicular(shootDir));
            else
                _fighter.shooter.SpawnEntityFromPool("MasterSparkParticle" + color, _fighter.hurtbox[0].rotation * Vector3.Scale(_fighter.shooter.transform.localPosition, _fighter.transform.lossyScale) + _fighter.transform.position, Quaternion.Euler(0, 0, angle - 90), _fighter.transform.localScale * 2.5f, (10 + count) * Vector2.Perpendicular(shootDir));
        }
        public void SkipUpSpecialAnimation()
        {
            if (!_fighter.stateVarsF.usedAirSpecial)
                _fighter.entity.SkipCurrentAnimToFrame(590);
        }
    
        public void AddBomb(int i)
        {
            _fighter.stateVarsF.specialTimer += i;
            if (i < 0)
                return;
            if (!_dSpecialVisuals[0].stateVars.enabled)
                _dSpecialVisuals[0].stateVars.genericTimer = 0;
            else
                _dSpecialVisuals[1].stateVars.genericTimer = 0;
    
            _dSpecialVisuals[1].stateVars.damageArmor += _bombCooldown;
        }
        private void RushEffect(int color)
        {
            string col = color == 0 ? "BlueRush" : "YellowRush";
            var x = _fighter.aesthetics.SpawnEntityFromPool(col, _fighter.hurtbox[0].rotation * Vector3.Scale(_fighter.aesthetics.transform.localPosition, _fighter.transform.lossyScale) + _fighter.transform.position, 
                Quaternion.Euler(0, 0, _fighter.entity.AssociatedRenderers[0].transform.rotation.eulerAngles.z + (_fighter.entity.stateVars.flippedLeft ? 90 : -90)), _fighter.transform.localScale, Vector2.zero);
        }
        private void ShootAmulet()
        {
            var specialAim = _fighter.stateVarsF.specialAim.magnitude > 0.2f ? _fighter.stateVarsF.specialAim.normalized : (_fighter.entity.stateVars.flippedLeft? Vector2.left : Vector2.right);
            var angle = Mathf.Atan2(specialAim.y, specialAim.x) * Mathf.Rad2Deg;
    
            var x = _fighter.shooter.SpawnEntityFromPool("QuickDart", _fighter.hurtbox[0].rotation * Vector3.Scale(_fighter.shooter.transform.localPosition, _fighter.transform.lossyScale) + _fighter.transform.position, Quaternion.Euler(0, 0, angle - 90), _fighter.transform.localScale, 10f * specialAim);
        }
        private void ShootStrongOrb()
        {
            var specialAim = _fighter.stateVarsF.specialAim.magnitude > 0.2f ? _fighter.stateVarsF.specialAim.normalized : (_fighter.entity.stateVars.flippedLeft ? Vector2.left : Vector2.right);
            var angle = Mathf.Atan2(specialAim.y, specialAim.x) * Mathf.Rad2Deg;
    
            var x = _fighter.shooter.SpawnEntityFromPool("StrongOrb", _fighter.hurtbox[0].rotation * Vector3.Scale(_fighter.shooter.transform.localPosition, _fighter.transform.lossyScale) + _fighter.transform.position, Quaternion.Euler(0, 0, angle - 90), _fighter.transform.localScale, 15f * specialAim);
        }
        private void ShootMissile(float up)
        {
            var specialAim = _fighter.entity.stateVars.flippedLeft ? Vector2.left : Vector2.right;
            var angle = Mathf.Atan2(specialAim.y, specialAim.x) * Mathf.Rad2Deg;
            specialAim = (specialAim * 0.1f + Vector2.up * Mathf.Sign(up)).normalized * Mathf.Abs(up);
    
            var x = _fighter.shooter.SpawnEntityFromPool("Missile", _fighter.hurtbox[0].rotation * Vector3.Scale(_fighter.shooter.transform.localPosition, _fighter.transform.lossyScale) + _fighter.transform.position, Quaternion.Euler(0, 0, angle - 90), _fighter.transform.localScale, 4f * specialAim);
        }
        private void ShootFireworks()
        {
            var x = _fighter.shooter.SpawnEntityFromPool("USmash", _fighter.hurtbox[0].rotation * Vector3.Scale(_fighter.shooter.transform.localPosition, _fighter.transform.lossyScale) + _fighter.transform.position, Quaternion.identity, _fighter.transform.localScale, (1 + _fighter.stateVarsF.attackCharge * 0.75f) * 8f * Vector2.up);
        }
        private void ShootFTilt()
        {
            var x = _fighter.aesthetics.SpawnEntityFromPool("FTilt", _fighter.aesthetics.transform.position, Quaternion.Euler(0, 0, _fighter.entity.stateVars.flippedLeft ? 90 : -90), _fighter.transform.localScale * 0.3f);
            var y = _fighter.aesthetics.SpawnEntityFromPool("FTiltMain", _fighter.aesthetics.transform.position, Quaternion.Euler(0, 0, _fighter.entity.stateVars.flippedLeft ? 180 : 0), _fighter.transform.localScale * 1.5f);
            x.stateVars.selfTime = 2;
            y.PlayParticles();
        }
        private void ShootBAir()
        {
            var x = _fighter.aesthetics.SpawnEntityFromPool("BAir", _fighter.aesthetics.transform.position, Quaternion.Euler(0, 0, _fighter.entity.stateVars.flippedLeft ? -90 : 90), _fighter.transform.localScale * 0.5f);
    
            var y = _fighter.aesthetics.SpawnEntityFromPool("BAirBurst", _fighter.aesthetics.transform.position, Quaternion.Euler(0, 0, _fighter.entity.stateVars.flippedLeft ? -90 : 90), _fighter.transform.localScale);
        }
        private void ShootDAir()
        {
            var x = _fighter.aesthetics.SpawnEntityFromPool("BAir", _fighter.aesthetics.transform.position, Quaternion.identity, _fighter.transform.localScale * 0.5f);
    
            var y = _fighter.aesthetics.SpawnEntityFromPool("BAirBurst", _fighter.aesthetics.transform.position, Quaternion.Euler(0, 0, _fighter.entity.stateVars.flippedLeft ? -90 : 90), _fighter.transform.localScale);
        }
        private void ShootUTilt()
        {
            var x = _fighter.aesthetics.SpawnEntityFromPool("UTiltStar", _fighter.aesthetics.transform.position, Quaternion.identity, _fighter.transform.localScale);
            var y = _fighter.aesthetics.SpawnEntityFromPool("UTiltCircle", _fighter.aesthetics.transform.position, Quaternion.identity, _fighter.transform.localScale * 0.25f);
        }
        private void ShootJab()
        {
            var x = _fighter.aesthetics.SpawnEntityFromPool("UTiltStar", _fighter.aesthetics.transform.position, Quaternion.identity, _fighter.transform.localScale * 0.75f);
            var y = _fighter.aesthetics.SpawnEntityFromPool("UTiltCircle", _fighter.aesthetics.transform.position, Quaternion.identity, _fighter.transform.localScale * 0.175f);
        }
    
        private void MatchFighterPalette(Entity x)
        {
            if (x != null)
                (x.AssociatedRenderers[0] as SpriteRenderer).material = _fighter.entity.AssociatedRenderers[0].material;
        }
    
        public void PhyisicalHitSound(HitObject hit)
        {
            if (!_fighter.aesthetics.enabled)
                return;
            if (hit.box.entity.stateVars.invulnerable || hit.box is ShieldBox)
                return;
            if (hit.box.entity.gameObject.CompareTag("Floating"))
                return;
            var point = hit.hitCol.ClosestPoint((Vector2)hit.hitBy.transform.position + hit.hitBy.offset);
            var trueDmg = Mathf.Clamp(hit.hitbox.hitProperties.damage, 0, hit.box.entity.et.health);
            if (trueDmg > 10)
            {
                var sound = _fighter.aesthetics.SpawnFromPool("PhysicalStrongSFX", point, Quaternion.identity, Vector3.one);
                if (sound.TryGetComponent(out AudioSource audio))
                {
                    audio.pitch = 1.75f - Mathf.Clamp(trueDmg / 20f, 0, 1);
                    audio.volume = Mathf.Clamp(trueDmg / 20f, 0.5f, 1);
                    var sound2 = _fighter.aesthetics.SpawnFromPool("PhysicalLightSFX", point, Quaternion.identity, Vector3.one);
                }
            }
            else if (trueDmg > 5)
            {
                var sound = _fighter.aesthetics.SpawnFromPool("PhysicalMediumSFX", point, Quaternion.identity, Vector3.one);
                if (sound.TryGetComponent(out AudioSource audio))
                {
                    audio.pitch = 1.75f - Mathf.Clamp(trueDmg / 10f, 0, 1);
                    audio.volume = Mathf.Clamp(trueDmg / 20f, 0.5f, 1);
                }
            }
            else
            {
                var sound = _fighter.aesthetics.SpawnFromPool("PhysicalLightSFX", point, Quaternion.identity, Vector3.one);
                if (sound.TryGetComponent(out AudioSource audio))
                {
                    audio.pitch = 1.25f - Mathf.Clamp(trueDmg / 10, 0, 0.5f);
                    audio.volume = 1;
                }
            }
        }
        public void PhyisicalHitSoundSpeedDependdent(HitObject hit)
        {
            if (!_fighter.aesthetics.enabled)
                return;
            if (hit.box.entity.stateVars.invulnerable || hit.box is ShieldBox)
                return;
            if (hit.box.entity.gameObject.CompareTag("Floating"))
                return;
            var point = hit.hitCol.ClosestPoint((Vector2)hit.hitBy.transform.position + hit.hitBy.offset);
            var trueDmg = Mathf.Clamp(hit.hitbox.hitProperties.damage + _fighter.entity.stateVars.indieSpd.magnitude, 0, hit.box.entity.et.health);
            if (trueDmg > 10)
            {
                var sound = _fighter.aesthetics.SpawnFromPool("PhysicalStrongSFX", point, Quaternion.identity, Vector3.one);
                if (sound.TryGetComponent(out AudioSource audio))
                {
                    audio.pitch = 1.75f - Mathf.Clamp(trueDmg / 20f, 0, 1);
                    audio.volume = Mathf.Clamp(trueDmg / 20f, 0.5f, 1);
                    var sound2 = _fighter.aesthetics.SpawnFromPool("PhysicalLightSFX", point, Quaternion.identity, Vector3.one);
                }
            }
            else if (trueDmg > 5)
            {
                var sound = _fighter.aesthetics.SpawnFromPool("PhysicalMediumSFX", point, Quaternion.identity, Vector3.one);
                if (sound.TryGetComponent(out AudioSource audio))
                {
                    audio.pitch = 1.75f - Mathf.Clamp(trueDmg / 10f, 0, 1);
                    audio.volume = Mathf.Clamp(trueDmg / 20f, 0.5f, 1);
                }
            }
            else
            {
                var sound = _fighter.aesthetics.SpawnFromPool("PhysicalLightSFX", point, Quaternion.identity, Vector3.one);
                if (sound.TryGetComponent(out AudioSource audio))
                {
                    audio.pitch = 1.25f - Mathf.Clamp(trueDmg / 10, 0, 0.5f);
                    audio.volume = 1;
                }
            }
        }
        public void BroomHitSound(HitObject hit)
        {
            if (!_fighter.aesthetics.enabled)
                return;
            if (hit.box.entity.stateVars.invulnerable || hit.box is ShieldBox)
                return;
            if (hit.box.entity.gameObject.CompareTag("Floating"))
                return;
            var point = hit.hitCol.ClosestPoint((Vector2)hit.hitBy.transform.position + hit.hitBy.offset);
            var trueDmg = Mathf.Clamp(hit.hitbox.hitProperties.damage, 0, hit.box.entity.et.health);
            var sound = _fighter.aesthetics.SpawnFromPool("BroomHit", point, Quaternion.identity, Vector3.one);
            if (sound.TryGetComponent(out AudioSource audio))
            {
                audio.pitch = 1.25f - Mathf.Clamp(trueDmg / 40f, 0, 0.5f);
            }
        }
        public void SparkleHitSound(HitObject hit)
        {
            if (!_fighter.aesthetics.enabled)
                return;
            if (hit.box.entity.stateVars.invulnerable || hit.box is ShieldBox)
                return;
            if (hit.box.entity.gameObject.CompareTag("Floating"))
                return;
            var point = hit.hitCol.ClosestPoint((Vector2)hit.hitBy.transform.position + hit.hitBy.offset);
            var trueDmg = Mathf.Clamp(hit.hitbox.hitProperties.damage, 0, hit.box.entity.et.health);
            var sound = _fighter.aesthetics.SpawnFromPool("SparkleSFX", point, Quaternion.identity, Vector3.one);
            if (sound.TryGetComponent(out AudioSource audio))
            {
                audio.pitch = 1.25f - Mathf.Clamp(trueDmg / 40f, 0, 0.5f);
            }
        }
    
        public void EnergyHitSound(HitObject hit)
        {
            if (!_fighter.aesthetics.enabled)
                return;
            if (hit.box.entity.stateVars.invulnerable || hit.box is ShieldBox)
                return;
            if (hit.box.entity.gameObject.CompareTag("Floating"))
                return;
            var trueDmg = Mathf.Clamp(hit.hitbox.hitProperties.damage, 0, hit.box.entity.et.health);
            var point = hit.hitCol.ClosestPoint((Vector2)hit.hitBy.transform.position + hit.hitBy.offset);
            if (trueDmg > 10)
            {
                var sound = _fighter.aesthetics.SpawnFromPool("EnergyHitHeavy", point, Quaternion.identity, Vector3.one);
                if (sound.TryGetComponent(out AudioSource audio))
                {
                    audio.pitch = 2f - Mathf.Clamp(trueDmg / 20f, 0, 1);
                    audio.volume = Mathf.Clamp(trueDmg / 20f, 0.5f, 1);
                    var sound2 = _fighter.aesthetics.SpawnFromPool("EnergyHitLight", point, Quaternion.identity, Vector3.one);
                }
            }
            else if (trueDmg > 5)
            {
                var sound = _fighter.aesthetics.SpawnFromPool("EnergyHitLight", point, Quaternion.identity, Vector3.one);
                if (sound.TryGetComponent(out AudioSource audio))
                {
                    audio.pitch = 1.25f - Mathf.Clamp(trueDmg / 40f, 0, 0.25f);
                    audio.volume = Mathf.Clamp(trueDmg / 20f, 0.75f, 1);
                }
            }
            else
            {
                var sound = _fighter.aesthetics.SpawnFromPool("EnergyHitMedium", point, Quaternion.identity, Vector3.one);
                if (sound.TryGetComponent(out AudioSource audio))
                {
                    audio.pitch = 1.55f - Mathf.Clamp(trueDmg / 20, 0, 0.25f);
                    audio.volume = 0.5f;
                }
            }
        }
    
        public void SpurtShockwaveParticle(HitObject hit)
        {
            if (!_fighter.aesthetics.enabled)
                return;
            if (hit.box.entity.stateVars.invulnerable || hit.box is ShieldBox)
                return;
            if (hit.box.entity.gameObject.CompareTag("Floating"))
                return;
            var trueDmg = Mathf.Clamp(hit.hitbox.hitProperties.damage, 0, hit.box.entity.et.health);
            var point = hit.hitCol.ClosestPoint((Vector2)hit.hitBy.transform.position + hit.hitBy.offset);
            var angle = Mathf.Atan2(hit.box.entity.stateVars.indieSpd.normalized.y, hit.box.entity.stateVars.indieSpd.normalized.x) * Mathf.Rad2Deg + 90;
            var rad = angle * Mathf.Deg2Rad;
            var particle = _fighter.aesthetics.SpawnEntityFromPool("Shockwave", point, Quaternion.identity, Vector3.one);
    
            particle.transform.up = new Vector3(-Mathf.Sin(rad), Mathf.Cos(rad), 0).normalized;
            particle.transform.localScale = Vector3.one * Mathf.Clamp(Mathf.Pow(trueDmg, 2) * 0.004f + 0.25f, 0.25f, 2f);
            particle.stateVars.selfTime = 1f;
            particle.PlayParticles();
        }
    
        public void SpurtSlashParticle(HitObject hit)
        {
            if (!_fighter.aesthetics.enabled)
                return;
            if (hit.box.entity.stateVars.invulnerable || hit.box is ShieldBox)
                return;
            if (hit.box.entity.gameObject.CompareTag("Floating"))
                return;
            var point = hit.hitCol.ClosestPoint((Vector2)hit.hitBy.transform.position + hit.hitBy.offset);
            var particle = _fighter.aesthetics.SpawnEntityFromPool("Slash", point, Quaternion.identity, Vector3.one);
    
            particle.stateVars.selfTime = 1f;
            particle.PlayParticles();
        }
        public void UAirShockwaveParticle()
        {
            if (!_fighter.aesthetics.enabled)
                return;
            var point = transform.position;
            var particle = _fighter.aesthetics.SpawnEntityFromPool("Shockwave", point, Quaternion.Euler(0, 0, transform.lossyScale.x < 0? 25 : -25), Vector3.one * 0.2f);
    
            particle.stateVars.selfTime = 1f;
            particle.PlayParticles();
        }
        public void DashAttackParticle()
        {
            if (!_fighter.aesthetics.enabled)
                return;
            var forwardDisplace = _fighter.entity.stateVars.flippedLeft ? Vector3.right : Vector3.left;
            var ad = _fighter.aesthetics.SpawnEntityFromPool("Dash", transform.position - Vector3.up * 0.8f - forwardDisplace, transform.rotation, Vector3.one);
            ad.transform.position -= 0.4f * (_fighter.entity.stateVars.flippedLeft ? Vector3.right : Vector3.left);
            ad.stateVars.flippedLeft = _fighter.entity.stateVars.flippedLeft;
        }
        public void USmashParticle()
        {
            if (!_fighter.aesthetics.enabled)
                return;
            var forwardDisplace = 0.5f * (_fighter.entity.stateVars.flippedLeft ? Vector3.right : Vector3.left);
            var ad = _fighter.aesthetics.SpawnEntityFromPool("Dash", transform.position - Vector3.up * 0.8f - forwardDisplace, transform.rotation, Vector3.one);
            ad.transform.position -= 0.4f * (_fighter.entity.stateVars.flippedLeft ? Vector3.right : Vector3.left);
            ad.stateVars.flippedLeft = _fighter.entity.stateVars.flippedLeft;
        }
        public void FSmashParticle()
        {
            if (!_fighter.aesthetics.enabled)
                return;
            var fighter = _fighter.entity;
            var ad = _fighter.aesthetics.SpawnEntityFromPool("FSmashDust", fighter.transform.position - (Vector3.up * 0.95f) * fighter.transform.lossyScale.y, fighter.transform.rotation, Vector3.one);
            ad.transform.position -= 0.4f * (fighter.stateVars.flippedLeft ? Vector3.right : Vector3.left);
            ad.stateVars.flippedLeft = fighter.stateVars.flippedLeft;
        }
        public void FtiltParticle()
        {
            if (!_fighter.aesthetics.enabled)
                return;
            var fighter = _fighter.entity;
            var ad = _fighter.aesthetics.SpawnEntityFromPool("FSmashDust", fighter.transform.position - (Vector3.up * 0.95f) * fighter.transform.lossyScale.y, fighter.transform.rotation, Vector3.one * 0.75f);
            ad.transform.position -= 0.4f * (fighter.stateVars.flippedLeft ? Vector3.right : Vector3.left) * fighter.transform.lossyScale.y;
            ad.stateVars.flippedLeft = fighter.stateVars.flippedLeft;
        }
        public void FSmash2Particle()
        {
            if (!_fighter.aesthetics.enabled)
                return;
            var forwardDisplace = 1.75f * (_fighter.entity.stateVars.flippedLeft ? Vector3.right : Vector3.left);
            var ad = _fighter.aesthetics.SpawnEntityFromPool("Dash", transform.position - Vector3.up * 0.8f - forwardDisplace, transform.rotation, Vector3.one);
            ad.transform.position -= 0.4f * (_fighter.entity.stateVars.flippedLeft ? Vector3.right : Vector3.left);
            ad.stateVars.flippedLeft = _fighter.entity.stateVars.flippedLeft;
        }
        public void FSmashProjectileGreen(float angle)
        {
            MeteonicShower(angle, "Green");
        }
        public void FSmashProjectileRed(float angle)
        {
            MeteonicShower(angle, "Red");
        }
        public void FSmashProjectileOrange(float angle)
        {
            MeteonicShower(angle, "Orange");
        }
        public void FSmashProjectileBlue(float angle)
        {
            MeteonicShower(angle, "Blue");
        }
        public void FSmashProjectileYellow(float angle)
        {
            MeteonicShower(angle, "Yellow");
        }
        public void FSmashProjectilePurple(float angle)
        {
            MeteonicShower(angle, "Purple");
        }
    
        public void MeteonicShower(float angle, string color)
        {
            var specialAim = _fighter.entity.stateVars.flippedLeft ? Vector2.left : Vector2.right;
            specialAim = (specialAim + Vector2.up * angle * 0.5f).normalized;
            specialAim *= (1 + _fighter.stateVarsF.attackCharge);
    
            var x = _fighter.shooter.SpawnEntityFromPool("Stardust" + color, _fighter.hurtbox[0].rotation * Vector3.Scale(_fighter.shooter.transform.localPosition, _fighter.transform.lossyScale) + _fighter.transform.position, Quaternion.identity, _fighter.transform.localScale, 12f * specialAim);
        }
        public void DSpecialPotion()
        {
            var aim = _fighter.stateVarsF.specialCharge * Vector2.right + Vector2.up * 0.55f;
            AddBomb(-1);
            if (_fighter.ActionState.GetState() is AirborneFighterSpecialMarisaRushState)
                aim = -Mathf.Sign(aim.x) * 0.25f * Vector2.right + Vector2.up * 0.2f;
            else if (_fighter.stateVarsF.currentAttackID == AttackID.Up)
                aim = Vector2.up * 1.45f * Mathf.Clamp(_fighter.stateVarsF.specialCharge, 1, 1.3f);
            else if (_fighter.stateVarsF.currentAttackID == AttackID.Down)
            {
                if (!_fighter.entity.stateVars.aerial)
                    aim = Mathf.Sign(aim.x) * (_fighter.stateVarsF.specialCharge <= 1? 0.25f : 1) * Vector2.right + Vector2.up * 0.6f;
                else
                    aim = Vector2.down * 1.25f * _fighter.stateVarsF.specialCharge;
            }
    
            aim *= Vector2.up + (_fighter.entity.stateVars.flippedLeft ? Vector2.left : Vector2.right);
            var x = _fighter.shooter.SpawnEntityFromPool("Potion", _fighter.hurtbox[0].rotation * Vector3.Scale(_fighter.shooter.transform.localPosition, _fighter.transform.lossyScale) + _fighter.transform.position, Quaternion.identity, _fighter.transform.localScale, 8f * aim + _fighter.entity.stateVars.indieSpd * 0.5f);
            var y = x.GetComponent<BaseProjectileBehaviour>();
            var timer = 0f;
            foreach (Entity bombV in _dSpecialVisuals)
            {
                timer = bombV.stateVars.genericTimer > timer ? bombV.stateVars.genericTimer : timer;
            }
            _dSpecialVisuals[0].stateVars.genericTimer = _dSpecialVisuals[1].stateVars.genericTimer;
            _dSpecialVisuals[1].stateVars.genericTimer = 0;
            y.LifeTime = y.MaxLifeTime * timer;
    
        }
        public void DSpecialPotionDrop()
        {
            AddBomb(-1);
            var x = _fighter.shooter.SpawnEntityFromPool("Potion", _bombHang.position, Quaternion.identity, _fighter.transform.localScale, _fighter.entity.stateVars.indieSpd * 0.5f);
            var y = x.GetComponent<BaseProjectileBehaviour>();
            var timer = 0f;
            foreach (Entity bombV in _dSpecialVisuals)
            {
                timer = bombV.stateVars.genericTimer > timer ? bombV.stateVars.genericTimer : timer;
            }
            _dSpecialVisuals[0].stateVars.genericTimer = _dSpecialVisuals[1].stateVars.genericTimer;
            _dSpecialVisuals[1].stateVars.genericTimer = 0;
            y.LifeTime = y.MaxLifeTime * timer;
        }
        public void DsmashPotion()
        {
            var specialAim = _fighter.entity.stateVars.flippedLeft ? Vector2.left : Vector2.right;
            var dropBelowPlatform = (_fighter.controlling.moveStick.raw.y <= -0.5f || _fighter.controlling.attackStick.raw.y <= -0.5f);
    
            var bomb1 = 0;
            var bomb2 = 1;
            foreach (BaseProjectileBehaviour bomb in _dSmashBombs)
            {
                if (_dSmashBombs[bomb1].entity.stateVars.enabled)
                    bomb1 += 2;
                if (_dSmashBombs[bomb2].entity.stateVars.enabled)
                    bomb2 += 2;
                if (bomb1 >= _dSmashBombs.Count)
                    bomb1 = 0;
                if (bomb2 >= _dSmashBombs.Count)
                    bomb1 = 1;
            }
    
            _dSmashBombs[bomb1].transform.position = _fighter.shooter.transform.position;
            _dSmashBombs[bomb1].entity.stateVars.indieSpd = specialAim;
            _dSmashBombs[bomb1].OnShoot(_fighter.entity);
            _dSmashBombs[bomb1].entity.AssociatedRenderers[0].transform.rotation = Quaternion.identity;
            _dSmashBombs[bomb1].entity.stateVars.flippedLeft = _fighter.entity.stateVars.flippedLeft;
            _dSmashBombs[bomb1].entity.stateVars.platformCondition = !dropBelowPlatform;
            _dSmashBombs[bomb1].entity.SetEntityActive(true);
            _dSmashBombHits[bomb1].hitProperties = _initDSmashBombProperties;
            _dSmashBombHits[bomb1].hitProperties.damage *= 1 + _fighter.stateVarsF.attackCharge * 0.5f;
            _dSmashBombHits[bomb1].hitProperties.knockback *= 1 + _fighter.stateVarsF.attackCharge * 0.5f;
    
            _dSmashBombs[bomb2].transform.position = _fighter.aesthetics.transform.position;
            _dSmashBombs[bomb2].entity.stateVars.indieSpd = -specialAim;
            _dSmashBombs[bomb2].OnShoot(_fighter.entity);
            _dSmashBombs[bomb2].entity.AssociatedRenderers[0].transform.rotation = Quaternion.identity;
            _dSmashBombs[bomb2].entity.stateVars.flippedLeft = _fighter.entity.stateVars.flippedLeft;
            _dSmashBombs[bomb2].entity.stateVars.platformCondition = !dropBelowPlatform;
            _dSmashBombs[bomb2].entity.SetEntityActive(true);
            _dSmashBombHits[bomb2].hitProperties = _initDSmashBombProperties;
            _dSmashBombHits[bomb2].hitProperties.damage *= 1 + _fighter.stateVarsF.attackCharge * 0.5f;
            _dSmashBombHits[bomb2].hitProperties.knockback *= 1 + _fighter.stateVarsF.attackCharge * 0.5f;
        }
        private void NonDirectionalLaserActivate()
        {
            foreach (BaseProjectileBehaviour laser in _nonDirLasers)
            {
                _fighter.stateVarsF.specialCooldown = (_fighter.controlling.shieldButton.raw ^ _fighter.entity.stateVars.flippedLeft) ? -1 : 1;
                laser.entity.SetEntityActive(true);
                laser.entity.OnSpawn.Invoke();
                laser.OnShoot(_fighter.entity);
            }
        }
    
        public void SpurtShockwaveHorizontalParticle(HitObject hit)
        {
            if (!_fighter.aesthetics.enabled)
                return;
            if (hit.box.entity.stateVars.invulnerable || hit.box is ShieldBox)
                return;
            if (hit.box.entity.gameObject.CompareTag("Floating"))
                return;
            var point = hit.hitCol.ClosestPoint((Vector2)hit.hitBy.transform.position + hit.hitBy.offset);
            var angle = (hit.hitbox.entity.stateVars.flippedLeft ? 180 : 0) + 90;
            var rad = angle * Mathf.Deg2Rad;
            var particle = _fighter.aesthetics.SpawnEntityFromPool("Shockwave", point, Quaternion.identity, Vector3.one);
    
            var trueDmg = Mathf.Clamp(hit.hitbox.hitProperties.damage, 0, hit.box.entity.et.health);
            particle.transform.up = new Vector3(-Mathf.Sin(rad), Mathf.Cos(rad), 0).normalized;
            particle.transform.localScale = Vector3.one * Mathf.Clamp(Mathf.Pow(trueDmg, 2) * 0.004f + 0.25f, 0.25f, 2f);
            particle.stateVars.selfTime = 1f;
            particle.PlayParticles();
        }
        public void SpurtSplashInvertParticle(HitObject hit)
        {
            if (!_fighter.aesthetics.enabled)
                return;
            if (hit.box.entity.stateVars.invulnerable || hit.box is ShieldBox)
                return;
            if (hit.box.entity.gameObject.CompareTag("Floating"))
                return;
            var point = hit.hitCol.ClosestPoint((Vector2)hit.hitBy.transform.position + hit.hitBy.offset);
            var randInt = UnityEngine.Random.Range(0, 2) * 2 - 1;
            var randInt2 = UnityEngine.Random.Range(0, 2) * 2 - 1;
            var angle = Mathf.Atan2(hit.box.entity.stateVars.indieSpd.normalized.y, hit.box.entity.stateVars.indieSpd.normalized.x) * Mathf.Rad2Deg;
            angle += 180;
            if (hit.box.entity.stateVars.lastLaunchSpd.magnitude < 5)
                angle = UnityEngine.Random.Range(0, 360);
            var rad = (angle - 25 * randInt2) * Mathf.Deg2Rad;
            var rad2 = (angle + 25 * randInt2) * Mathf.Deg2Rad;
            var rad3 = (angle + 180) * Mathf.Deg2Rad;
    
            var splash1 = "HitSplash2";
            var splash2 = "HitSplash";
    
            var scale = 1;
            if (_fighter.ActionState.GetState() is GroundedFighterAttackState)
            {
                if (_fighter.stateVarsF.currentAttackID == FighterState.AttackID.Neutral || _fighter.stateVarsF.currentAttackID == FighterState.AttackID.Up)
                {
                    splash1 = "EnergyBlue";
                    splash2 = "EnergyDarkBluePurple";
                }
                if (_fighter.stateVarsF.currentAttackID == FighterState.AttackID.Forward)
                {
                    splash1 = "EnergyGreen";
                    splash2 = "EnergyGreen2";
                    scale = 2;
                }
            }
    
            var particle = _fighter.aesthetics.SpawnEntityFromPool(splash1, point, Quaternion.identity, Vector3.one);
            var particle2 = _fighter.aesthetics.SpawnEntityFromPool(splash2, point, Quaternion.identity, Vector3.one);
            var trueDmg = Mathf.Clamp(hit.hitbox.hitProperties.damage, 0, hit.box.entity.et.health);
    
            particle.transform.up = new Vector3(-Mathf.Sin(rad), Mathf.Cos(rad), 0).normalized;
            particle.transform.localScale = Vector3.one * Mathf.Clamp(Mathf.Pow(trueDmg, 2) * 0.004f + 0.4f, 0.4f, 2.2f);
            particle.transform.localScale = new Vector3(particle.transform.localScale.x, particle.transform.localScale.y * randInt, 1) * scale;
            particle.stateVars.selfTime = 1f;
            particle.PlayParticles();
    
            particle2.transform.up = new Vector3(-Mathf.Sin(rad2), Mathf.Cos(rad2), 0).normalized;
            particle2.transform.localScale = Vector3.one * Mathf.Clamp(Mathf.Pow(trueDmg, 2) * 0.004f + 0.6f, 0.6f, 1.8f);
            particle2.transform.localScale = new Vector3(particle2.transform.localScale.x, particle2.transform.localScale.y * randInt, 1) * scale;
            particle2.stateVars.selfTime = 1f;
            particle2.PlayParticles();
    
            if (hit.box.entity.stateVars.lastLaunchSpd.magnitude > 50)
            {
                var particle3 = _fighter.aesthetics.SpawnEntityFromPool(splash2, point, Quaternion.identity, Vector3.one);
                particle3.transform.up = new Vector3(-Mathf.Sin(rad3), Mathf.Cos(rad3), 0).normalized;
                particle3.transform.localScale = Vector3.one * Mathf.Clamp(Mathf.Pow(trueDmg, 2) * 0.004f + 0.2f, 0.2f, 2.2f);
                particle3.transform.localScale = new Vector3(particle.transform.localScale.x, particle.transform.localScale.y * randInt, 1) * scale;
                particle3.stateVars.selfTime = 1f;
                particle3.PlayParticles();
            }
        }
        public void SpurtHitStarParticles(HitObject hit)
        {
            if (_fighter.aesthetics == null)
                return;
            if (hit.box.entity.gameObject.CompareTag("Floating"))
                return;
            if (hit.box.entity.stateVars.invulnerable || hit.box is ShieldBox)
                return;
    
            var trueDmg = Mathf.Clamp(hit.hitbox.hitProperties.damage, 0, hit.box.entity.et.health);
            if (trueDmg > 0)
            {
                for (int i = 0; i < Mathf.Clamp(Mathf.CeilToInt(trueDmg / 4), 0, 4); i++)
                {
                    var point = hit.hitCol.ClosestPoint((Vector2)hit.hitBy.transform.position + hit.hitBy.offset);
                    var particle = _fighter.aesthetics.SpawnEntityFromPool("HitStar", point, hit.box.transform.rotation, hit.box.transform.lossyScale);
                    var angle = Mathf.Atan2(hit.box.entity.stateVars.lastLaunchSpd.normalized.y, hit.box.entity.stateVars.lastLaunchSpd.normalized.x) * Mathf.Rad2Deg;
    
                    if (hit.hitbox.hitProperties.knockback <= 2)
                        angle = Random.Range(0, 360);
    
                    var rad = angle * Mathf.Deg2Rad;
    
                    particle.transform.up = new Vector3(-Mathf.Sin(rad), Mathf.Cos(rad), 0).normalized;
                    particle.transform.localScale = Vector3.one * Mathf.Clamp(Mathf.Pow(trueDmg, 2) * 0.005f + 0.75f, 0.75f, 3);
                    particle.PlayParticles();
                }
            }
        }
        public void SpurtSplashParticle(HitObject hit)
        {
            if (!_fighter.aesthetics.enabled)
                return;
            if (hit.box.entity.stateVars.invulnerable || hit.box is ShieldBox)
                return;
            if (hit.box.entity.gameObject.CompareTag("Floating"))
                return;
            var point = hit.hitCol.ClosestPoint((Vector2)hit.hitBy.transform.position + hit.hitBy.offset);
            var randInt = UnityEngine.Random.Range(0, 2) * 2 - 1;
            var randInt2 = UnityEngine.Random.Range(0, 2) * 2 - 1;
            var angle = Mathf.Atan2(hit.box.entity.stateVars.indieSpd.normalized.y, hit.box.entity.stateVars.indieSpd.normalized.x) * Mathf.Rad2Deg;
            if (hit.box.entity.stateVars.lastLaunchSpd.magnitude < 5)
                angle = UnityEngine.Random.Range(0, 360);
            var rad = (angle - 25 * randInt2) * Mathf.Deg2Rad;
            var rad2 = (angle + 25 * randInt2) * Mathf.Deg2Rad;
            var rad3 = (angle + 180) * Mathf.Deg2Rad;
    
            var splash1 = "HitSplash2";
            var splash2 = "HitSplash";
    
            if (_fighter.ActionState.GetState() is GroundedFighterAttackState)
            {
                if (_fighter.stateVarsF.currentAttackID == FighterState.AttackID.Neutral)
                {
                    splash1 = "EnergyBlue";
                    splash2 = "EnergyDarkBluePurple";
                }
            }
            var trueDmg = Mathf.Clamp(hit.hitbox.hitProperties.damage, 0, hit.box.entity.et.health);
    
            var particle = _fighter.aesthetics.SpawnEntityFromPool(splash1, point, Quaternion.identity, Vector3.one);
            var particle2 = _fighter.aesthetics.SpawnEntityFromPool(splash2, point, Quaternion.identity, Vector3.one);
    
            particle.transform.up = new Vector3(-Mathf.Sin(rad), Mathf.Cos(rad), 0).normalized;
            particle.transform.localScale = Vector3.one * Mathf.Clamp(Mathf.Pow(trueDmg, 2) * 0.004f + 0.4f, 0.4f, 2.2f);
            particle.transform.localScale = new Vector3(particle.transform.localScale.x, particle.transform.localScale.y * randInt, 1);
            particle.stateVars.selfTime = 1f;
            particle.PlayParticles();
    
            particle2.transform.up = new Vector3(-Mathf.Sin(rad2), Mathf.Cos(rad2), 0).normalized;
            particle2.transform.localScale = Vector3.one * Mathf.Clamp(Mathf.Pow(trueDmg, 2) * 0.004f + 0.6f, 0.6f, 1.8f);
            particle2.transform.localScale = new Vector3(particle2.transform.localScale.x, particle2.transform.localScale.y * randInt, 1);
            particle2.stateVars.selfTime = 1f;
            particle2.PlayParticles();
    
            if (hit.box.entity.stateVars.lastLaunchSpd.magnitude > 50)
            {
                var particle3 = _fighter.aesthetics.SpawnEntityFromPool(splash2, point, Quaternion.identity, Vector3.one);
                particle3.transform.up = new Vector3(-Mathf.Sin(rad3), Mathf.Cos(rad3), 0).normalized;
                particle3.transform.localScale = Vector3.one * Mathf.Clamp(Mathf.Pow(trueDmg, 2) * 0.004f + 0.2f, 0.2f, 2.2f);
                particle3.transform.localScale = new Vector3(particle.transform.localScale.x, particle.transform.localScale.y * randInt, 1);
                particle3.stateVars.selfTime = 1f;
                particle3.PlayParticles();
            }
        }
    
        public void ComboSpecial2Dir()
        {
            if (_fighter.controlling.specialButton.Buffer() || (_fighter.controlling.cStick.BufferHold() && _fighter.controlling.specialToggle.raw))
            {
                Vector2 dir = GetAttackSpecialDir();
                TransitionSpecialCombo(dir);
            }
        }
    
        private Vector2 GetAttackSpecialDir()
        {
            var dir = Vector2.zero;
            if (_fighter.controlling.cStick.BufferHold() || _fighter.controlling.attackButton.Buffer())
                dir = (_fighter.entity.actionState.GetState() as FighterState).GetAttackDir(_fighter.controlling.attackButton, _fighter.controlling.cStick);
            if (_fighter.controlling.cStick.BufferHold() || _fighter.controlling.specialButton.Buffer())
                dir = (_fighter.entity.actionState.GetState() as FighterState).GetAttackDir(_fighter.controlling.specialButton, _fighter.controlling.cStick);
            return dir;
        }
    
        public void TransitionSpecialCombo(Vector2 dir)
        {
    
            //if (Mathf.Abs(dir.x) > 0.35f)
                //_fighter.entity.stateVars.flippedLeft = dir.x < 0;
    
            if (!_fighter.entity.stateVars.aerial)
            {
                _fighter.entity.PlayAnim("Ground_Special_2Directional_Shoot_Down");
                _fighter.stateVarsF.specialAim = Vector2.right;
                _fighter.entity.actionState.SetState(new GroundedFighterSpecial2DirectionalShootDownState(_fighter.entity.actionState as FighterSM));
    
                if (dir.y > 0.9f)
                {
                    _fighter.entity.PlayAnim("Ground_Special_2Directional_Shoot_Up");
                    _fighter.stateVarsF.specialAim = Vector2.up;
                    _fighter.entity.actionState.SetState(new GroundedFighterSpecial2DirectionalShootUpState(_fighter.entity.actionState as FighterSM));
                    return;
                }
                if (dir.y < -0.9f)
                {
                    _fighter.entity.PlayAnim("Ground_Special_2Directional_Shoot_Down");
                    _fighter.stateVarsF.specialAim = Vector2.down;
                    _fighter.entity.actionState.SetState(new GroundedFighterSpecial2DirectionalShootDownState(_fighter.entity.actionState as FighterSM));
                    return;
                }
                if (dir.y > 0.35f && Mathf.Abs(dir.x) > 0.35f)
                {
                    _fighter.entity.PlayAnim("Ground_Special_2Directional_Shoot_Up");
                    _fighter.stateVarsF.specialAim = Vector2.one.normalized;
                    _fighter.entity.actionState.SetState(new GroundedFighterSpecial2DirectionalShootUpState(_fighter.entity.actionState as FighterSM));
                    return;
                }
                if (dir.y < -0.35f && Mathf.Abs(dir.x) > 0.35f)
                {
                    _fighter.entity.PlayAnim("Ground_Special_2Directional_Shoot_Down");
                    _fighter.stateVarsF.specialAim = (Vector2.down + Vector2.right).normalized;
                    _fighter.entity.actionState.SetState(new GroundedFighterSpecial2DirectionalShootDownState(_fighter.entity.actionState as FighterSM));
                    return;
                }
            }
            else
            {
                _fighter.entity.PlayAnim("Air_Special_2Directional_Shoot_Down");
                _fighter.stateVarsF.specialAim = Vector2.right;
                _fighter.entity.actionState.SetState(new AirborneFighterSpecial2DirectionalShootDownState(_fighter.entity.actionState as FighterSM));
    
                if (dir.y > 0.9f)
                {
                    _fighter.entity.PlayAnim("Air_Special_2Directional_Shoot_Up");
                    _fighter.stateVarsF.specialAim = Vector2.up;
                    _fighter.entity.actionState.SetState(new AirborneFighterSpecial2DirectionalShootUpState(_fighter.entity.actionState as FighterSM));
                    return;
                }
                if (dir.y < -0.9f)
                {
                    _fighter.entity.PlayAnim("Air_Special_2Directional_Shoot_Down");
                    _fighter.stateVarsF.specialAim = Vector2.down;
                    _fighter.entity.actionState.SetState(new AirborneFighterSpecial2DirectionalShootDownState(_fighter.entity.actionState as FighterSM));
                    return;
                }
                if (dir.y > 0.35f && Mathf.Abs(dir.x) > 0.35f)
                {
                    _fighter.entity.PlayAnim("Air_Special_2Directional_Shoot_Up");
                    _fighter.stateVarsF.specialAim = Vector2.one.normalized;
                    _fighter.entity.actionState.SetState(new AirborneFighterSpecial2DirectionalShootUpState(_fighter.entity.actionState as FighterSM));
                    return;
                }
                if (dir.y < -0.35f && Mathf.Abs(dir.x) > 0.35f)
                {
                    _fighter.entity.PlayAnim("Air_Special_2Directional_Shoot_Down");
                    _fighter.stateVarsF.specialAim = (Vector2.down + Vector2.right).normalized;
                    _fighter.entity.actionState.SetState(new AirborneFighterSpecial2DirectionalShootDownState(_fighter.entity.actionState as FighterSM));
                    return;
                }
            }
        }
    }
    public class GroundedFighterAttackMarisaChargeState : GroundedFighterState
    {
        public GroundedFighterAttackMarisaChargeState(FighterSM fsm) : base(fsm)
        {
            lag = true;
        }
        public override void OnStateEnter()
        {
            AttackCharge = 0;
        }
    
        public override void OnStateUpdate(float timeScale)
        {
            base.OnStateUpdate(timeScale);
            AttackCharge += timeScale * Time.fixedDeltaTime * 6;
            if (AttackCancelCondition)
                SetState(new GroundedFighterAttackMarisaReleaseState(fsm));
        }
    }
    public class GroundedFighterAttackMarisaReleaseState : GroundedFighterState
    {
        public GroundedFighterAttackMarisaReleaseState(FighterSM fsm) : base(fsm)
        {
            lag = true;
        }
        public override void OnStateEnter()
        {
            base.OnStateEnter();
            fsm.fighter.ClearAttackHits();
            fsm.entity.SkipCurrentAnimToFrame(360);
        }
    }
    public class AirborneFighterAttackNoMoveState : AirborneFighterState
    {
        public AirborneFighterAttackNoMoveState(FighterSM fsm) : base(fsm)
        {
            lag = true;
            moveable = false;
        }
    
        public override void OnStateEnter()
        {
        }
    }
    public class AirborneFighterAttackMoveState : AirborneFighterState
    {
        public AirborneFighterAttackMoveState(FighterSM fsm) : base(fsm)
        {
            lag = true;
        }
    
        public override void OnStateEnter()
        {
        }
    }
    public class AirborneFighterSpecialChargeStartNoWavebounceState : AirborneFighterSpecialChargeStartState
    {
        public AirborneFighterSpecialChargeStartNoWavebounceState(FighterSM fsm) : base(fsm)
        {
            lag = true;
        }
        protected override void WaveBounce()
        {
        }
    }
    public class GroundedFighterSpecialChargeStartNoWavebounceState : GroundedFighterSpecialChargeStartState
    {
        public GroundedFighterSpecialChargeStartNoWavebounceState(FighterSM fsm) : base(fsm)
        {
            lag = true;
        }
        protected override void WaveBounce()
        {
        }
    }
    public class AirborneFighterSpecialChargeStartMarisaUpState : AirborneFighterSpecialChargeStartState
    {
        public AirborneFighterSpecialChargeStartMarisaUpState(FighterSM fsm) : base(fsm)
        {
            lag = true;
            moveable = false;
        }
    
        public override void TransitionGround()
        {
        }
        public override void OnStateEnter()
        {
            fsm.fighter.entity.PlayAnim("Air_Special_Misc_1");
    
            SpecialAim = Vector2.up;
            AttackCharge = 0;
            ExternalSpd *= 0;
            fsm.entity.physicsState.SetState(new MarisaAirRush(fsm.entity.physicsState));
        }
        public override void OnStateUpdate(float timeScale)
        {
            base.OnStateUpdate(timeScale);
            var specialAim = SpecialAim.magnitude > 0.2f ? SpecialAim.normalized : (FlippedLeft ? Vector2.left : Vector2.right);
            if (specialAim.x < 0 ^ FlippedLeft)
                specialAim *= new Vector2(-1, 1);
            Vector3 FlyingDir = (Quaternion.AngleAxis(FlippedLeft ? 90 : -90, Vector3.forward) * specialAim).normalized;
            RotateHurtbox(Vector3.Slerp(HurtboxRotation, -FlyingDir, Time.fixedDeltaTime * timeScale * 2));
        }
    }
    public class AirborneFighterSpecialChargeReadyMarisaUpState : AirborneFighterSpecialChargeReadyState
    {
        public AirborneFighterSpecialChargeReadyMarisaUpState(FighterSM fsm) : base(fsm)
        {
            lag = true;
            moveable = false;
        }
        public override void OnStateEnter()
        {
            var currentFrame = FrameNum;
            fsm.fighter.entity.PlayAnim("Air_Special_Misc_1");
            fsm.fighter.entity.SkipCurrentAnimToFrame(currentFrame);
            AttackCharge = 0;
            fsm.entity.physicsState.SetState(new MarisaAirRush(fsm.entity.physicsState));
        }
        public override void OnStateUpdate(float timeScale)
        {
            base.OnStateUpdate(timeScale);
            var specialAim = SpecialAim.magnitude > 0.2f ? SpecialAim.normalized : (FlippedLeft ? Vector2.left : Vector2.right);
            if (specialAim.x < 0 ^ FlippedLeft)
                specialAim *= new Vector2(-1, 1);
            Vector3 FlyingDir = (Quaternion.AngleAxis(FlippedLeft ? 90 : -90, Vector3.forward) * specialAim).normalized;
            RotateHurtbox(Vector3.Slerp(HurtboxRotation, -FlyingDir, Time.fixedDeltaTime * timeScale * 2));
    
            AttackCharge += timeScale * Time.fixedDeltaTime;
            CalculateFocusSpd(timeScale);
            fsm.fighter.ExpendStamina(timeScale * Time.fixedDeltaTime * (2f/3f));
            IndieSpd -= IndieSpd * Ft.airRes * timeScale;
        }
        public override void OnStateExit()
        {
            base.OnStateEnter();
            RotateHurtbox(Vector2.up);
            if (Aerial)
                fsm.entity.physicsState.SetState(new InAir(fsm.entity.physicsState));
            else
                fsm.entity.physicsState.SetState(new OnGround(fsm.entity.physicsState));
        }
    
        public override void TransitionGround()
        {
        }
    
        protected override void TransitionSpecialShoot()
        {
            if (SpecialCancelCondition)
                SetState(new AirborneFighterSpecialMarisaRushState(fsm));
        }
    
        protected override void WaveBounce()
        {
            //base.WaveBounce();
        }
        private void CalculateFocusSpd(float timeScale)
        {
            if (IndieSpd.magnitude > Ft.focusSpd)
            {
                var moveDir = IndieSpd.normalized;
                IndieSpd += (moveDir - Controlling.moveStick.raw).sqrMagnitude / 2
                    * Ft.focusAcl * Controlling.moveStick.raw * timeScale;
                return;
            }
            IndieSpd += Ft.focusAcl * Controlling.moveStick.raw;
        }
    }
    
    public class AirborneFighterSpecialMarisaRushState : AirborneFighterState
    {
        public AirborneFighterSpecialMarisaRushState(FighterSM fsm) : base(fsm)
        {
            lag = true;
            moveable = false;
            staminaRegenable = false;
        }
        public override void OnStateEnter()
        {
            base.OnStateEnter();
            var specialAim = SpecialAim.magnitude > 0.2f ? SpecialAim.normalized : (FlippedLeft ? Vector2.left : Vector2.right);
            if (specialAim.x < 0 ^ FlippedLeft)
                specialAim *= new Vector2(-1, 1);
            CurrentAttackID = AttackID.Down;
            fsm.fighter.stateVarsF.usedAirSpecial = false;
            fsm.fighter.ExpendStamina(2f);
            fsm.entity.PlayAnim("Air_Special_Attack");
            IndieSpd = specialAim.normalized * Ft.airdashSpd * 1.5f + Vector2.Dot(specialAim.normalized, IndieSpd.normalized) * IndieSpd.magnitude * specialAim.normalized;
            fsm.entity.physicsState.SetState(new MarisaAirRush(fsm.entity.physicsState));
    
            Vector3 FlyingDir = (Quaternion.AngleAxis(FlippedLeft ? 90 : -90, Vector3.forward) * IndieSpd).normalized;
            RotateHurtbox(-FlyingDir);
        }
    
        public override void OnStateUpdate(float timeScale)
        {
            base.OnStateUpdate(timeScale);
            RecordSpecialAim();
            
            Vector3 FlyingDir = (Quaternion.AngleAxis(FlippedLeft ? 90 : -90, Vector3.forward) * IndieSpd).normalized;
            RotateHurtbox(-FlyingDir);
            IndieSpd += SpecialAim * Ft.airAcl * 2f * timeScale;
            IndieSpd -= IndieSpd * Ft.airRes * timeScale;
            if (FrameNum > 180)
                fsm.fighter.ExpendStamina(timeScale * 5 * Time.fixedDeltaTime);
            if (((AttackHoldCondition || SpecialHoldCondition || Controlling.shieldButton.raw || fsm.entity.rb.velocity.magnitude < Ft.airSpd || Stamina <= 0) && FrameNum > 110) || (Vector2.Dot(fsm.entity.rb.velocity.normalized, IndieSpd.normalized) < 0.85f && timeScale >= 0.5f))
            {
                fsm.entity.SkipCurrentAnimToFrame(380);
                if (AttackHoldCondition || SpecialHoldCondition)
                {
                    fsm.fighter.stateVarsF.usedAirSpecial = true;
                }
                SetState(new AirborneFighterSpecialMarisaRushLagState(fsm));
            }
            if (((fsm.fighter.extension as MarisaFighterExtension).DropBomb && FrameNum > 110 && SpecialTimer > 0))
            {
                (fsm.fighter.extension as MarisaFighterExtension).DSpecialPotion();
                (fsm.fighter.extension as MarisaFighterExtension).ConsumeDropBombBuffer();
            }
    
        }
        public override void OnStateExit()
        {
            base.OnStateEnter();
            if (Aerial)
                fsm.entity.physicsState.SetState(new InAir(fsm.entity.physicsState));
            else
                fsm.entity.physicsState.SetState(new OnGround(fsm.entity.physicsState));
        }
    
    
        public override void TransitionGround()
        {
        }
    }
    public class AirborneFighterSpecialMarisaRushLagState : AirborneFighterState
    {
        public AirborneFighterSpecialMarisaRushLagState(FighterSM fsm) : base(fsm)
        {
            lag = true;
            moveable = false;
        }
        public override void OnStateUpdate(float timeScale)
        {
            base.OnStateUpdate(timeScale);
            RotateHurtbox(Vector3.Slerp(HurtboxRotation, Vector3.up, Time.fixedDeltaTime * timeScale * 2));
        }
        public override void OnStateExit()
        {
            base.OnStateEnter();
            RotateHurtbox(Vector2.up);
        }
    }
    public class AirborneFighterSpecialShootWaveBounceState : AirborneFighterSpecialShootState
    {
        public AirborneFighterSpecialShootWaveBounceState(FighterSM fsm) : base(fsm)
        {
            lag = true;
        }
        protected override void WaveBounce()
        {
            base.WaveBounce();
        }
    }
    public class GroundedFighterSpecialShootWaveBounceState : GroundedFighterSpecialShootState
    {
        public GroundedFighterSpecialShootWaveBounceState(FighterSM fsm) : base(fsm)
        {
            lag = true;
        }
    
        protected override void WaveBounce()
        {
            base.WaveBounce();
        }
    }
    public class GroundedFighterSpecial8DirectionalChargeReady2State : GroundedFighterSpecialState
    {
        public GroundedFighterSpecial8DirectionalChargeReady2State(FighterSM fsm) : base(fsm)
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
                fsm.fighter.extension.TransitionSpecia8DirShoot2();
        }
    
        protected override void TransitionAir()
        {
            IndieSpd += ExternalSpd;
            ExternalSpd = Vector2.zero;
            PlatformInvuln = 0.2f;
            SetState(new AirborneFighterSpecial8DirectionalChargeReady2State(fsm));
        }
        protected override void WaveBounce()
        {
            //base.WaveBounce();
        }
    }
    
    public class AirborneFighterSpecial8DirectionalChargeReady2State : AirborneFighterSpecialState
    {
        public AirborneFighterSpecial8DirectionalChargeReady2State(FighterSM fsm) : base(fsm)
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
                fsm.fighter.extension.TransitionSpecia8DirShoot2();
        }
    
        public override void TransitionGround()
        {
            PlatformInvuln = 0;
            SetState(new GroundedFighterSpecial8DirectionalChargeReady2State(fsm));
        }
        protected override void WaveBounce()
        {
            //base.WaveBounce();
        }
    }
    
    public class GroundedFighterSpecial8DirectionalShootRight2State : GroundedFighterSpecial8DirectionalShootState
    {
        public GroundedFighterSpecial8DirectionalShootRight2State(FighterSM fsm) : base(fsm)
        {
            lag = true;
        }
    
        public override void OnStateEnter()
        {
            base.OnStateEnter();
            var currentFrame = FrameNum;
            fsm.fighter.entity.PlayAnim("Ground_Special_8Directional_Shoot_Right2");
    
            fsm.fighter.entity.SkipCurrentAnimToFrame(currentFrame);
        }
    
        protected override void TransitionAir()
        {
            IndieSpd += ExternalSpd;
            ExternalSpd = Vector2.zero;
            PlatformInvuln = 0.2f;
            SetState(new AirborneFighterSpecial8DirectionalShootRight2State(fsm));
        }
    }
    public class AirborneFighterSpecial8DirectionalShootRight2State : AirborneFighterSpecial8DirectionalShootState
    {
        public AirborneFighterSpecial8DirectionalShootRight2State(FighterSM fsm) : base(fsm)
        {
            lag = true;
        }
        public override void OnStateEnter()
        {
            base.OnStateEnter();
            var currentFrame = FrameNum;
            fsm.fighter.entity.PlayAnim("Air_Special_8Directional_Shoot_Right2");
    
            fsm.fighter.entity.SkipCurrentAnimToFrame(currentFrame);
        }
    
        public override void TransitionGround()
        {
            PlatformInvuln = 0;
            SetState(new GroundedFighterSpecial8DirectionalShootRight2State(fsm));
        }
    }
    
    public class GroundedFighterSpecial8DirectionalShootUpRight2State : GroundedFighterSpecial8DirectionalShootState
    {
        public GroundedFighterSpecial8DirectionalShootUpRight2State(FighterSM fsm) : base(fsm)
        {
            lag = true;
        }
    
        public override void OnStateEnter()
        {
            base.OnStateEnter();
            var currentFrame = FrameNum;
            fsm.fighter.entity.PlayAnim("Ground_Special_8Directional_Shoot_UpRight2");
    
            fsm.fighter.entity.SkipCurrentAnimToFrame(currentFrame);
        }
    
        protected override void TransitionAir()
        {
            IndieSpd += ExternalSpd;
            ExternalSpd = Vector2.zero;
            PlatformInvuln = 0.2f;
            SetState(new AirborneFighterSpecial8DirectionalShootUpRight2State(fsm));
        }
    }
    public class AirborneFighterSpecial8DirectionalShootUpRight2State : AirborneFighterSpecial8DirectionalShootState
    {
        public AirborneFighterSpecial8DirectionalShootUpRight2State(FighterSM fsm) : base(fsm)
        {
            lag = true;
        }
        public override void OnStateEnter()
        {
            base.OnStateEnter();
            var currentFrame = FrameNum;
            fsm.fighter.entity.PlayAnim("Air_Special_8Directional_Shoot_UpRight2");
    
            fsm.fighter.entity.SkipCurrentAnimToFrame(currentFrame);
        }
    
        public override void TransitionGround()
        {
            PlatformInvuln = 0;
            SetState(new GroundedFighterSpecial8DirectionalShootUpRight2State(fsm));
        }
    }
    
    public class GroundedFighterSpecial8DirectionalShootUp2State : GroundedFighterSpecial8DirectionalShootState
    {
        public GroundedFighterSpecial8DirectionalShootUp2State(FighterSM fsm) : base(fsm)
        {
            lag = true;
        }
    
        public override void OnStateEnter()
        {
            base.OnStateEnter();
            var currentFrame = FrameNum;
            fsm.fighter.entity.PlayAnim("Ground_Special_8Directional_Shoot_Up2");
    
            fsm.fighter.entity.SkipCurrentAnimToFrame(currentFrame);
        }
    
        protected override void TransitionAir()
        {
            IndieSpd += ExternalSpd;
            ExternalSpd = Vector2.zero;
            PlatformInvuln = 0.2f;
            SetState(new AirborneFighterSpecial8DirectionalShootUp2State(fsm));
        }
    }
    public class AirborneFighterSpecial8DirectionalShootUp2State : AirborneFighterSpecial8DirectionalShootState
    {
        public AirborneFighterSpecial8DirectionalShootUp2State(FighterSM fsm) : base(fsm)
        {
            lag = true;
        }
        public override void OnStateEnter()
        {
            base.OnStateEnter();
            var currentFrame = FrameNum;
            fsm.fighter.entity.PlayAnim("Air_Special_8Directional_Shoot_Up2");
    
            fsm.fighter.entity.SkipCurrentAnimToFrame(currentFrame);
        }
    
        public override void TransitionGround()
        {
            PlatformInvuln = 0;
            SetState(new GroundedFighterSpecial8DirectionalShootUp2State(fsm));
        }
    }
    
    public class GroundedFighterSpecial8DirectionalShootDownRight2State : GroundedFighterSpecial8DirectionalShootState
    {
        public GroundedFighterSpecial8DirectionalShootDownRight2State(FighterSM fsm) : base(fsm)
        {
            lag = true;
        }
    
        public override void OnStateEnter()
        {
            base.OnStateEnter();
            var currentFrame = FrameNum;
            fsm.fighter.entity.PlayAnim("Ground_Special_8Directional_Shoot_DownRight2");
    
            fsm.fighter.entity.SkipCurrentAnimToFrame(currentFrame);
        }
    
        protected override void TransitionAir()
        {
            IndieSpd += ExternalSpd;
            ExternalSpd = Vector2.zero;
            PlatformInvuln = 0.2f;
            SetState(new AirborneFighterSpecial8DirectionalShootDownRight2State(fsm));
        }
    }
    public class AirborneFighterSpecial8DirectionalShootDownRight2State : AirborneFighterSpecial8DirectionalShootState
    {
        public AirborneFighterSpecial8DirectionalShootDownRight2State(FighterSM fsm) : base(fsm)
        {
            lag = true;
        }
        public override void OnStateEnter()
        {
            base.OnStateEnter();
            var currentFrame = FrameNum;
            fsm.fighter.entity.PlayAnim("Air_Special_8Directional_Shoot_DownRight2");
    
            fsm.fighter.entity.SkipCurrentAnimToFrame(currentFrame);
        }
    
        public override void TransitionGround()
        {
            PlatformInvuln = 0;
            SetState(new GroundedFighterSpecial8DirectionalShootDownRight2State(fsm));
        }
    }
    
    public class GroundedFighterSpecial8DirectionalShootDown2State : GroundedFighterSpecial8DirectionalShootState
    {
        public GroundedFighterSpecial8DirectionalShootDown2State(FighterSM fsm) : base(fsm)
        {
            lag = true;
        }
    
        public override void OnStateEnter()
        {
            base.OnStateEnter();
            var currentFrame = FrameNum;
            fsm.fighter.entity.PlayAnim("Ground_Special_8Directional_Shoot_Down2");
    
            fsm.fighter.entity.SkipCurrentAnimToFrame(currentFrame);
        }
    
        protected override void TransitionAir()
        {
            IndieSpd += ExternalSpd;
            ExternalSpd = Vector2.zero;
            PlatformInvuln = 0.2f;
            SetState(new AirborneFighterSpecial8DirectionalShootDown2State(fsm));
        }
    }
    public class AirborneFighterSpecial8DirectionalShootDown2State : AirborneFighterSpecial8DirectionalShootState
    {
        public AirborneFighterSpecial8DirectionalShootDown2State(FighterSM fsm) : base(fsm)
        {
            lag = true;
        }
        public override void OnStateEnter()
        {
            base.OnStateEnter();
            var currentFrame = FrameNum;
            fsm.fighter.entity.PlayAnim("Air_Special_8Directional_Shoot_Down2");
    
            fsm.fighter.entity.SkipCurrentAnimToFrame(currentFrame);
        }
    
        public override void TransitionGround()
        {
            PlatformInvuln = 0;
            SetState(new GroundedFighterSpecial8DirectionalShootDown2State(fsm));
        }
    }
    
    
    public class GroundedFighterSpellcardAimState : GroundedFighterState
    {
        public GroundedFighterSpellcardAimState(FighterSM fsm) : base(fsm)
        {
            lag = true;
        }
    
        public override void OnStateEnter()
        {
            base.OnStateEnter();
            SpecialAim = FlippedLeft? Vector2.left : Vector2.right;
            IndieSpd *= 0;
            SelfSpd *= 0;
            Gravity = false;
        }
        public override void OnStateUpdate(float timeScale)
        {
            base.OnStateUpdate(timeScale);
            RecordSpecialAim();
            var specialAim = SpecialAim.normalized;
            Vector3 FlyingDir = -(Quaternion.AngleAxis(FlippedLeft ? 90 : -90, Vector3.forward) * specialAim).normalized;
            if (Vector2.Dot(FlyingDir, HurtboxRotation.normalized) < 0)
                FlyingDir = Vector2.Reflect(-FlyingDir, Vector2.Perpendicular(HurtboxRotation));
            RotateHurtbox(Vector3.Slerp(HurtboxRotation, FlyingDir, Time.fixedDeltaTime * timeScale * 1f));
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
    public class GroundedFighterSpellcardRemainAimState : GroundedFighterState
    {
        public GroundedFighterSpellcardRemainAimState(FighterSM fsm) : base(fsm)
        {
            lag = true;
        }
    
        public override void OnStateEnter()
        {
            base.OnStateEnter();
            IndieSpd *= 0;
            SelfSpd *= 0;
            Gravity = false;
        }
        public override void OnStateUpdate(float timeScale)
        {
            base.OnStateUpdate(timeScale);
            RecordSpecialAim();
            var specialAim = SpecialAim.normalized;
            Vector3 FlyingDir = -(Quaternion.AngleAxis(FlippedLeft ? 90 : -90, Vector3.forward) * specialAim).normalized;
            if (Vector2.Dot(FlyingDir, HurtboxRotation.normalized) < 0)
                FlyingDir = Vector2.Reflect(-FlyingDir, Vector2.Perpendicular(HurtboxRotation));
            RotateHurtbox(Vector3.Slerp(HurtboxRotation, FlyingDir, Time.fixedDeltaTime * timeScale * 0.5f));
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
    public class GroundedFighterSpellcardStaticAimState : GroundedFighterState
    {
        public GroundedFighterSpellcardStaticAimState(FighterSM fsm) : base(fsm)
        {
            lag = true;
        }
    
        public override void OnStateEnter()
        {
            base.OnStateEnter();
            IndieSpd *= 0;
            SelfSpd *= 0;
            Gravity = false;
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
    public class GroundedFighterSpellcardReturnFromAimState : GroundedFighterState
    {
        public GroundedFighterSpellcardReturnFromAimState(FighterSM fsm) : base(fsm)
        {
            lag = true;
        }
    
        public override void OnStateEnter()
        {
            base.OnStateEnter();
            IndieSpd *= 0;
            SelfSpd *= 0;
            Gravity = false;
        }
        public override void OnStateUpdate(float timeScale)
        {
            base.OnStateUpdate(timeScale);
            RotateHurtbox(Vector3.Slerp(HurtboxRotation, Vector2.up, Time.fixedDeltaTime * timeScale * 3));
        }
    
        public override void OnStateExit()
        {
            base.OnStateExit();
            RotateHurtbox(Vector2.up);
            Gravity = true;
        }
    
        protected override void TransitionAir()
        {
            //base.TransitionAir();
        }
    }}
