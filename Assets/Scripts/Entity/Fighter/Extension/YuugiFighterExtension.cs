namespace TightStuff.Extension
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    using static FighterState;
    using Random = UnityEngine.Random;
    
    public class YuugiFighterExtension : BaseFighterExtensions
    {
        private Material _floatMaterial;
    
        [SerializeField]
        private List<Entity> _afterImages;
    
        [SerializeField]
        private BaseProjectileBehaviour _geyser;
    
        [SerializeField]
        private float _drainSpd = 1;
        [SerializeField]
        private float _drinkSpd = 0.75f;
        [SerializeField]
        private float _additionalSmashJumpHeight = 3;
    
        [SerializeField]
        private LayerMask _blastzone;
        protected bool TapSmash { get => _fighter.controlling.moveStick.BufferTap() && _fighter.controlling.attackButton.Buffer() && _fighter.controlling.tapSmash; }
        private bool Smash => TapSmash || _fighter.controlling.smashStick.BufferHold() || _fighter.controlling.smashButton.Buffer();
        private bool Attack => TapSmash || Smash || _fighter.controlling.attackStick.BufferHold() || _fighter.controlling.attackButton.Buffer();
        private bool Shielding { get => _fighter.ActionState.GetState() is GroundedFighterShieldState; }
        public float DrunkPercent => _fighter.entity.stateVars.damageArmor;
        public float DrinkSpeed => _drinkSpd;
        public float DrainSpeed => _drainSpd;

        [SerializeField]
        private Color _blushColor;

        protected override void Start()
        {
            base.Start();
            ActionSM.OnStateEnter += DoubleJump;
            ActionSM.OnStateEnter += AnomalyFocus;
            ActionSM.OnStateEnter += LandShockwave;
    
            order = 0;
            _geyser.controlling = _fighter.controlling;
            _geyser.TransitionControllableState();
            _geyser.transform.parent = null;
            _geyser.entity.SetEntityActive(false);
        }
    
        public override void GUpdate()
        {
            base.GUpdate();
            DrunkRedFaceEffect();
            var alpha = 0.75f;
            foreach (var afterimage in _afterImages)
            {
                (afterimage.AssociatedRenderers[0] as SpriteRenderer).sprite = (_fighter.entity.AssociatedRenderers[0] as SpriteRenderer).sprite;
                State state = _fighter.entity.actionState.GetState();
                if (state is not GroundedFighterRollMoveState)
                    afterimage.stateVars.indieSpd = _fighter.entity.stateVars.indieSpd;
                else
                    afterimage.stateVars.indieSpd *= 0;
    
                afterimage.stateVars.flippedLeft = _fighter.entity.stateVars.flippedLeft;
    
                Color imgColor = (afterimage.AssociatedRenderers[0] as SpriteRenderer).color;
                imgColor.a = Mathf.Lerp(imgColor.a, (state is AirborneFighterFocusAnomalyState || state is GroundedFighterRollMoveState) ? 0.5f : 0, Time.fixedDeltaTime * 10);
                alpha /= 2;
                (afterimage.AssociatedRenderers[0] as SpriteRenderer).color = imgColor;
            }
    
            _fighter.entity.stateVars.damageArmor = Mathf.Clamp(_fighter.entity.stateVars.damageArmor - _fighter.entity.TrueTimeScale * _drainSpd, 0, 30);
        }
    
        protected override void OnDestroy()
        {
            base.OnDestroy();
            ActionSM.OnStateEnter -= DoubleJump;
            ActionSM.OnStateEnter -= AnomalyFocus;
            ActionSM.OnStateEnter -= LandShockwave;
        }
    
        protected override void SpecialDown(Vector2 specialDir)
        {
            base.SpecialDown(specialDir);
            if (!_fighter.entity.stateVars.aerial)
            {
                _fighter.entity.PlayAnim("Ground_Special_Misc_2");
                _fighter.entity.actionState.SetState(new GroundedFighterSpecialDrinkStartYuugiState(_fighter.entity.actionState as FighterSM));
            }
            else
            {
                _fighter.entity.PlayAnim("Air_Special_Misc_2");
                _fighter.entity.actionState.SetState(new AirborneFighterSpecialDrinkStartYuugiState(_fighter.entity.actionState as FighterSM));
            }
            _fighter.stateVarsF.turning = false;
        }
    
        protected override void SpecialHorizontal(FighterState.AttackID specialID)
        {
            base.SpecialHorizontal(specialID);
            if (!_fighter.entity.stateVars.aerial)
            {
                _fighter.entity.PlayAnim("Ground_Special_Shoot");
                _fighter.entity.actionState.SetState(new GroundedFighterSpecialChargeStartYuugiState(_fighter.entity.actionState as FighterSM));
            }
            else
            {
                _fighter.entity.PlayAnim("Air_Special_Shoot");
                _fighter.entity.actionState.SetState(new GroundedFighterSpecialChargeStartYuugiState(_fighter.entity.actionState as FighterSM));
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
            base.SpecialUp(specialDir);
            if (!_fighter.entity.stateVars.aerial)
            {
                _fighter.entity.PlayAnim("Ground_Special_Charge");
                _fighter.entity.actionState.SetState(new GroundedFighterSpecialChargeStartState(_fighter.entity.actionState as FighterSM));
            }
            else
            {
                _fighter.entity.PlayAnim("Air_Special_Charge");
                _fighter.entity.actionState.SetState(new AirborneFighterSpecialChargeStartState(_fighter.entity.actionState as FighterSM));
            }
            _fighter.stateVarsF.turning = false;
        }
    
        protected override void SpellcardUp(Vector2 specialDir)
        {
            base.SpellcardDown(specialDir);
            _fighter.entity.actionState.SetState(new GroundedFighterSpellcardAttackState(_fighter.entity.actionState as FighterSM));
        }
    
        protected override void SpellcardHorizontal(FighterState.AttackID specialID)
        {
            if (specialID == AttackID.Back)
                _fighter.entity.stateVars.flippedLeft = !_fighter.entity.stateVars.flippedLeft;
            _fighter.stateVarsF.grazeMeter -= 10;
            _fighter.entity.actionState.SetState(new GroundedFighterSpellcardShootState(_fighter.entity.actionState as FighterSM));
        }
    
        protected override void SpellcardDown(Vector2 specialDir)
        {
            if (Mathf.Abs(specialDir.x) > 0.175f)
                _fighter.entity.stateVars.flippedLeft = specialDir.x < 0;
            _fighter.stateVarsF.grazeMeter -= 6;
            _fighter.entity.actionState.SetState(new GroundedFighterSpellcardShoot2State(_fighter.entity.actionState as FighterSM));
        }
        private void DoubleJump(State state)
        {
            if (!_fighter.aesthetics.enabled)
                return;
    
            if (state is not AirborneFighterAirdashState)
                return;
    
            if ((state as AirborneFighterAirdashState).fsm.entity != _fighter.entity)
                return;
    
    
            if (state is AirborneFighterAirdashTapState)
                _fighter.entity.actionState.SetState(new AirborneFighterDoublejumpTapState(_fighter.entity.actionState as FighterSM));
            else if (state is AirborneFighterAirdashToggleState)
                _fighter.entity.actionState.SetState(new AirborneFighterDoublejumpToggleState(_fighter.entity.actionState as FighterSM));
            else
                _fighter.entity.actionState.SetState(new AirborneFighterDoublejumpState(_fighter.entity.actionState as FighterSM));
        }
        private void AnomalyFocus(State state)
        {
            if (!_fighter.aesthetics.enabled)
                return;
    
            if (state is not AirborneFighterFocusState || state is AirborneFighterFocusAnomalyState)
                return;
    
            if ((state as AirborneFighterFocusState).fsm.entity != _fighter.entity)
                return;
            _fighter.entity.actionState.SetState(new AirborneFighterFocusAnomalyState(_fighter.entity.actionState as FighterSM));
        }
        private void LandShockwave(State state)
        {
            if (!_fighter.aesthetics.enabled)
                return;
    
            if (state is not GroundedFighterLandState)
                return;
    
            if ((state as GroundedFighterLandState).fsm.entity != _fighter.entity)
                return;
    
            _camShake.ShakeCamera(MathF.Abs(_fighter.entity.stateVars.indieSpd.y) / 2, 0.2f);
        }
        public void SetFocusAfterImagePos()
        {
            foreach (var afterimage in _afterImages)
                afterimage.transform.position = transform.position;
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
        private void DSmashJump()
        {
            _fighter.entity.stateVars.indieSpd += new Vector2(0, _fighter.Ft.jumpHeight / 2 + _fighter.stateVarsF.attackCharge * _additionalSmashJumpHeight);
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
            _fighter.entity.actionState.SetState(new AirborneFighterAttackNoMoveState(_fighter.entity.actionState as FighterSM));
        }
        private void ChangeAttackIDToBack()
        {
            _fighter.stateVarsF.currentAttackID = AttackID.Back;
        }
    
        public void TransitionChargeReadyNoGround()
        {
            _fighter.entity.actionState.SetState(new AirborneFighterSpecialChargeReadyMarisaUpState(_fighter.entity.actionState as FighterSM));
        }
        public void TransitionNotDrinking()
        {
            if (_fighter.entity.stateVars.aerial)
                _fighter.entity.actionState.SetState(new AirborneFighterSpecialDrinkStartYuugiState(_fighter.entity.actionState as FighterSM));
            else
                _fighter.entity.actionState.SetState(new GroundedFighterSpecialDrinkStartYuugiState(_fighter.entity.actionState as FighterSM));
        }
        public void TransitionDrinking()
        {
            if (_fighter.entity.stateVars.aerial)
                _fighter.entity.actionState.SetState(new AirborneFighterSpecialDrinkingYuugiState(_fighter.entity.actionState as FighterSM));
            else
                _fighter.entity.actionState.SetState(new GroundedFighterSpecialDrinkingYuugiState(_fighter.entity.actionState as FighterSM));
        }
    
        public void SkipUpSpecialAnimation()
        {
            if (!_fighter.stateVarsF.usedAirSpecial)
                _fighter.entity.SkipCurrentAnimToFrame(590);
        }
    
        private void RushEffect(int color)
        {
            string col = color == 0 ? "BlueRush" : "YellowRush";
            var x = _fighter.aesthetics.SpawnEntityFromPool(col, _fighter.hurtbox[0].rotation * Vector3.Scale(_fighter.aesthetics.transform.localPosition, _fighter.transform.lossyScale) + _fighter.transform.position, 
                Quaternion.Euler(0, 0, _fighter.entity.AssociatedRenderers[0].transform.rotation.eulerAngles.z + (_fighter.entity.stateVars.flippedLeft ? 90 : -90)), _fighter.transform.localScale, Vector2.zero);
        }
        private void StompEffect()
        {
            var x = _fighter.aesthetics.SpawnEntityFromPool("Stomp", _fighter.aesthetics.transform.position, Quaternion.identity, _fighter.transform.localScale);
        }
        private void ExplosionEffect()
        {
            var x = _fighter.shooter.SpawnEntityFromPool("Explosion", _fighter.aesthetics.transform.position, Quaternion.identity, _fighter.transform.localScale * (1 + _fighter.stateVarsF.attackCharge));
        }
        private void ShoutEffect()
        {
            var x = _fighter.aesthetics.SpawnEntityFromPool("Distortion", _fighter.aesthetics.transform.position, Quaternion.identity, _fighter.transform.localScale);
        }
        private void DiarDescend()
        {
            _fighter.entity.actionState.SetState(new AirborneFighterYuugiDownAirState(_fighter.entity.actionState as FighterSM));
        }
    
        private void MatchFighterPalette(Entity x)
        {
            if (x != null)
                (x.AssociatedRenderers[0] as SpriteRenderer).material = _fighter.entity.AssociatedRenderers[0].material;
        }

        private void DrunkRedFaceEffect()
        {
            var _sprite = _fighter.entity.AssociatedRenderers[0] as SpriteRenderer;

            var ideal = Color.Lerp(Color.white, _blushColor, DrunkPercent / 30);
            _sprite.color = Color.Lerp(_sprite.color, ideal, 0.25f);
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
            if (trueDmg > 15)
            {
                var sound = _fighter.aesthetics.SpawnFromPool("PhysicalSuperStrongSFX", point, Quaternion.identity, Vector3.one);
                if (sound.TryGetComponent(out AudioSource audio))
                {
                    audio.pitch = 1.75f - Mathf.Clamp(trueDmg / 20f, 0, 1);
                    audio.volume = Mathf.Clamp(trueDmg / 20f, 0.5f, 1);
                    var sound2 = _fighter.aesthetics.SpawnFromPool("PhysicalLightSFX", point, Quaternion.identity, Vector3.one);
                }
            }
            else if (trueDmg > 10)
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
            if (trueDmg > 15)
            {
                var sound = _fighter.aesthetics.SpawnFromPool("PhysicalSuperStrongSFX", point, Quaternion.identity, Vector3.one);
                if (sound.TryGetComponent(out AudioSource audio))
                {
                    audio.pitch = 1.75f - Mathf.Clamp(trueDmg / 20f, 0, 1);
                    audio.volume = Mathf.Clamp(trueDmg / 20f, 0.5f, 1);
                    var sound2 = _fighter.aesthetics.SpawnFromPool("PhysicalLightSFX", point, Quaternion.identity, Vector3.one);
                }
            }
            else if (trueDmg > 10)
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
    
        public void ShakeGround(int intensity, float time)
        {
            _camShake.ShakeCamera(intensity, time);
        }
        public void FootstepShake(float intensity)
        {
            _camShake.ShakeCamera(intensity, 0.2f);
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
    
    
        private void ShootWaterBullets()
        {
            //if (_fighter.entity.stateVars.flippedLeft)
            //_fighter.stateVarsF.specialAim *= new Vector2(-1, 1);
    
            var projSpd = 20;
            var OGspecialAim = _fighter.stateVarsF.specialAim.magnitude > 0.2f ? _fighter.stateVarsF.specialAim.normalized : (_fighter.entity.stateVars.flippedLeft ? Vector2.left : Vector2.right);
            OGspecialAim = (OGspecialAim.normalized + MathF.Abs(OGspecialAim.normalized.x) * Vector2.up * 0.5f).normalized;
            var pos = (Vector3)OGspecialAim * 1.1f;
            var angle = Mathf.Atan2(OGspecialAim.y, OGspecialAim.x) * Mathf.Rad2Deg;
            var x = _fighter.shooter.SpawnEntityFromPool("WaterBullet", _fighter.shooter.transform.position + pos, Quaternion.Euler(0, 0, angle - 90), _fighter.transform.localScale / 2, projSpd * OGspecialAim);
    
            var specialAim = (OGspecialAim * 2 + Vector2.Perpendicular(OGspecialAim)).normalized;
            angle = Mathf.Atan2(specialAim.y, specialAim.x) * Mathf.Rad2Deg;
            pos = (Vector3)specialAim * 1.1f;
            var y = _fighter.shooter.SpawnEntityFromPool("WaterBullet", _fighter.shooter.transform.position + pos, Quaternion.Euler(0, 0, angle - 90), _fighter.transform.localScale / 2, projSpd * specialAim);
    
            specialAim = (OGspecialAim * 2 - Vector2.Perpendicular(OGspecialAim)).normalized;
            angle = Mathf.Atan2(specialAim.y, specialAim.x) * Mathf.Rad2Deg;
            pos = (Vector3)specialAim * 1.1f;
            var y2 = _fighter.shooter.SpawnEntityFromPool("WaterBullet", _fighter.shooter.transform.position + pos, Quaternion.Euler(0, 0, angle - 90), _fighter.transform.localScale / 2, projSpd * specialAim);
    
            if (x == null)
                return;
            SpawnSound("ShootWater");
        }
        private void ShootWheel()
        {
            var specialAim = _fighter.stateVarsF.specialAim.magnitude > 0.2f ? _fighter.stateVarsF.specialAim.normalized : (_fighter.entity.stateVars.flippedLeft ? Vector2.left : Vector2.right);
            if (specialAim.x < 0 ^ _fighter.entity.stateVars.flippedLeft)
                specialAim *= new Vector2(-1, 1);
            var additionalSpd = Vector3.Project(_fighter.entity.stateVars.indieSpd, specialAim.normalized) / 2;
            if (Vector2.Dot(specialAim.normalized, _fighter.entity.stateVars.indieSpd.normalized) <= 0)
                additionalSpd *= 0;
    
            var v = _fighter.shooter.SpawnEntityFromPool("Wheel", _fighter.shooter.transform.position, Quaternion.identity, _fighter.transform.localScale, 3f * specialAim + (Vector2)additionalSpd);
            if (v == null)
                return;
            v.stateVars.flippedLeft = _fighter.entity.stateVars.flippedLeft;
        }
        private void ShootGeyserSpot()
        {
            if (_geyser.entity.enabled || _geyser.LifeTime < _geyser.MaxLifeTime)
                return;
    
            _geyser.transform.position = new Vector2(transform.position.x + Mathf.Abs(_fighter.shooter.transform.localPosition.x) * _fighter.controlling.moveStick.raw.x, _fighter.shooter.transform.position.y);
            if (_fighter.controlling.specialStick.raw.magnitude > 0.3f)
            {
                _geyser.transform.position = new Vector2(transform.position.x + Mathf.Abs(_fighter.shooter.transform.localPosition.x) * _fighter.controlling.specialStick.raw.x, _fighter.shooter.transform.position.y);
            }
            _geyser.OnShoot(_fighter.entity);
            _geyser.entity.stateVars.indieSpd = _fighter.entity.stateVars.indieSpd * Vector2.right;
            _geyser.entity.SetEntityActive(true);
        }
        private void ShootGeyser()
        {
            if (!_geyser.entity.enabled)
                return;
            _geyser.entity.SetEntityActive(false);
            var hit = Physics2D.Raycast(_geyser.transform.position, Vector2.down, 50, _blastzone);
    
            var v = _fighter.shooter.SpawnEntityFromPool("Geyser", hit.point, Quaternion.identity, _fighter.transform.localScale, Vector2.zero);
            if (v != null)
            {
                v.stateVars.indieSpd = (_geyser.transform.position.y - hit.point.y) * Vector2.up * v.et.airSpd + _fighter.stateVarsF.specialCharge * 50f * Vector2.up;
    
                _camShake.ShakeCamera(5, 1);
                var f = _fighter.aesthetics.SpawnEntityFromPool("Splash", hit.point, Quaternion.identity, _fighter.transform.localScale * 1.25f, Vector2.zero);
                f.PlayParticles();
            }
        }
        private void ShootSteams()
        {
            var hit = Physics2D.Raycast(_fighter.transform.position + Vector3.right, Vector2.down, 1, _fighter.groundplatLayers);
            Debug.Log(hit.point);
            if (hit)
                _fighter.shooter.SpawnEntityFromPool("Steam", _fighter.transform.position + new Vector3(1, -0.85f), Quaternion.identity, _fighter.transform.localScale, Vector2.zero);
    
            var hit2 = Physics2D.Raycast(_fighter.transform.position + Vector3.left, Vector2.down, 1, _fighter.groundplatLayers);
            if (hit2)
                _fighter.shooter.SpawnEntityFromPool("Steam", _fighter.transform.position + new Vector3(-1, -0.85f), Quaternion.identity, _fighter.transform.localScale, Vector2.zero);
        }
        private void SummonWind()
        {
            var v = _fighter.shooter.SpawnEntityFromPool("Ooe", Vector2.zero, Quaternion.identity, Vector2.one, Vector2.zero);
            v.stateVars.flippedLeft = !_fighter.entity.stateVars.flippedLeft;
        }
        private void ShootCrack()
        {
            var v = _fighter.shooter.SpawnEntityFromPool("InitialCrackEffect", _fighter.shooter.transform.position, Quaternion.identity, _fighter.transform.localScale, Vector2.zero);
            var f = _fighter.aesthetics.SpawnEntityFromPool("CrackEffect", _fighter.aesthetics.transform.position, Quaternion.identity, _fighter.transform.localScale, Vector2.zero);
            f.PlayParticles();
            v.stateVars.flippedLeft = _fighter.entity.stateVars.flippedLeft;
        }
        private void ShootTrueCrack()
        {
            var v = _fighter.shooter.SpawnEntityFromPool("TrueCrack", _fighter.shooter.transform.position, Quaternion.identity, _fighter.transform.localScale, Vector2.zero);
            var f = _fighter.aesthetics.SpawnEntityFromPool("CrackEffect", _fighter.aesthetics.transform.position, Quaternion.identity, _fighter.transform.localScale, Vector2.zero);
            var x = _fighter.aesthetics.SpawnEntityFromPool("BigCrackEffect", _fighter.aesthetics.transform.position, Quaternion.identity, _fighter.transform.localScale, Vector2.zero);
            f.PlayParticles();
            x.PlayParticles();
            v.stateVars.flippedLeft = _fighter.entity.stateVars.flippedLeft;
        }
    }
    
    
    public class AirborneFighterDoublejumpState : AirborneFighterState
    {
    
        public AirborneFighterDoublejumpState(FighterSM fsm) : base(fsm)
        {
            airDashable = false;
        }
    
        public override void OnStateEnter()
        {
            base.OnStateEnter();
            SetAirDashDirection();
            Orientate(AirdashDir);
            var perpendicularSpd = Vector3.Project(IndieSpd, Vector2.Perpendicular(AirdashDir));
            var alignedSpd = Vector3.Project(IndieSpd, AirdashDir.normalized);

            if ((Jumping || JumpingTap || JumpingToggle) && AirdashDir.x != 0 && AirdashDir.y < 0)
                IndieSpd = new Vector2(IndieSpd.x, IndieSpd.y / 3);
            
            AirdashCancelable = false;
            AirdashTimePassed = 0;
            JumptimePassed = 1;
    
    
            var biggerSpd = (Ft.airdashSpd > alignedSpd.magnitude || Vector2.Dot(IndieSpd.normalized, AirdashDir.normalized) < 0)? Ft.airdashSpd : alignedSpd.magnitude;
    
            IndieSpd = AirdashDir * (biggerSpd + (Mathf.Clamp(AirdashDir.y, 0, 1) * 2)) + new Vector2(perpendicularSpd.x, perpendicularSpd.y) * 0.75f;
    
            fsm.entity.PlayAnim("Air_Airdash_Start");
        }



        public override void OnStateUpdate(float timeScale)
        {
            AirdashTimePassed += Time.fixedDeltaTime * timeScale;
    
            CheckStopAirdashing();
            base.OnStateUpdate(timeScale);
        }
    
        public override void OnStateExit()
        {
            Gravity = true;
            Orientate(Vector2.up);
            base.OnStateExit();
        }
    
        private void SetAirDashDirection()
        {
            var dir = GetDirection();
    
            AirdashDir = ConvertVectorTo8Dir(dir);
    
            if (AirdashDir.magnitude == 0)
                AirdashDir = Vector2.up;
        }
    
        protected virtual Vector2 GetDirection()
        {
            return Controlling.jumpButton.stickAccompanimentOnTap;
        }
    
        private void CheckStopAirdashing()
        {
            bool completeCondition = AirdashTimePassed >= Ft.airdashTime;
            if (completeCondition || (PlatformCondition && IsReachingGround(fsm.fighter.platLayers)))
            {
                SetState(new AirborneFighterDoublejumpStopState(fsm));
            }
        }
    
        public void Orientate(Vector2 airdashDir)
        {
            if (Mathf.Abs(airdashDir.x) > 0.1f)
                FlippedLeft = AirdashDir.x < 0;
    
            if (airdashDir == new Vector2(0, -1))
            {
                if (FlippedLeft)
                {
                    fsm.entity.transform.up = new Vector2(-0.01f, -1);
                }
                else
                {
                    fsm.entity.transform.up = new Vector2(0.01f, -1);
                }
            }
            else
            {
                fsm.entity.transform.up = airdashDir;
            }
        }
    
        public bool IsReachingGround(LayerMask layers)
        {
            if (Physics2D.Raycast(new Vector2(fsm.fighter.groundCheckPoint2.position.x, fsm.entity.transform.position.y), Vector2.down, 0.85f, layers))
            {
                RaycastHit2D hit = CheckAndTeleportToGround();
                return hit;
            }
            return false;
        }
    }
    
    public class AirborneFighterDoublejumpTapState : AirborneFighterDoublejumpState
    {
    
        public AirborneFighterDoublejumpTapState(FighterSM fsm) : base(fsm)
        {
            airDashable = false;
        }
    
        protected override Vector2 GetDirection()
        {
            return Controlling.moveStick.tapInitPos;
        }
    }
    public class AirborneFighterDoublejumpToggleState : AirborneFighterDoublejumpState
    {
    
        public AirborneFighterDoublejumpToggleState(FighterSM fsm) : base(fsm)
        {
            airDashable = false;
        }
    
        protected override Vector2 GetDirection()
        {
            return Controlling.airdashToggleButton.stickAccompanimentOnTap;
        }
    }
    
    public class AirborneFighterDoublejumpStopState : AirborneFighterState
    {
        public AirborneFighterDoublejumpStopState(FighterSM fsm) : base(fsm)
        {
            airDashable = false;
        }
        public override void OnStateEnter()
        {
            base.OnStateEnter();
    
            if (!Aerial)
            {
                TransitionGround();
                return;
            }
    
            fsm.entity.PlayAnim("Air_Airdash_End");
        }
    }
    
    public class AirborneFighterFocusAnomalyState : AirborneFighterFocusState
    {
        public AirborneFighterFocusAnomalyState(FighterSM fsm) : base(fsm)
        {
            lag = true;
            moveable = false;
        }
        public override void OnStateEnter()
        {
            base.OnStateEnter();
            (fsm.fighter.extension as YuugiFighterExtension).SetFocusAfterImagePos();
        }
    
        public override void CalculateFocusSpd(float timeScale)
        {
            WindSpd -= WindSpd * 0.3f * timeScale;
    
            var alignedDir = Vector2.Dot(WindSpd.normalized, IndieSpd.normalized);
            var alignedIndieSpd = Vector3.Project(IndieSpd, IndieSpd.normalized).magnitude;
            var alignedWindSpd = Vector3.Project(WindSpd, IndieSpd.normalized).magnitude;
            var ratioSpd = Mathf.Clamp((alignedIndieSpd + alignedWindSpd) / Ft.focusSpd, 0, 1);
    
            var windForce = Ft.focusAcl * Controlling.moveStick.raw;
    
            WindSpd += windForce * Mathf.Clamp((1 - alignedDir) + (1 - ratioSpd), 0, 1);
            IndieSpd += Ft.focusAcl * 0.05f * Controlling.moveStick.raw;
        }
        public override void OnStateExit()
        {
            base.OnStateExit();
            WindSpd *= 0;
        }
    }
    
    public class AirborneFighterYuugiDownAirState : AirborneFighterState
    {
        public AirborneFighterYuugiDownAirState(FighterSM fsm) : base(fsm)
        {
            lag = true;
            moveable = false;
        }
        public override void OnStateEnter()
        {
            base.OnStateEnter();
            var flip = FlippedLeft;
            var direction = new Vector2((flip ? -1 : 1) * Mathf.Sin(Mathf.Deg2Rad * 130), Mathf.Cos(Mathf.Deg2Rad * 130));
    
            var inheritedSpd = Vector2.Dot(direction.normalized, IndieSpd.normalized) < 0 ? Vector2.zero : (Vector2)Vector3.Project(IndieSpd, direction.normalized);
    
            IndieSpd = inheritedSpd.magnitude > 15 ? inheritedSpd : (direction * 15);
            SpecialAim = IndieSpd;
        }
        public override void OnStateUpdate(float timeScale)
        {
            base.OnStateUpdate(timeScale);
            IndieSpd = SpecialAim;
        }
    }
    
    public class GroundedFighterSpecialChargeStartYuugiState : GroundedFighterSpecialChargeStartState
    {
        public GroundedFighterSpecialChargeStartYuugiState(FighterSM fsm) : base(fsm)
        {
            lag = true;
        }
    
        public override void OnStateEnter()
        {
            var currentFrame = FrameNum;
            fsm.fighter.entity.PlayAnim("Ground_Special_Misc_1");
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
            SetState(new AirborneFighterSpecialChargeStartYuugiState(fsm));
        }
    }
    public class AirborneFighterSpecialChargeStartYuugiState : AirborneFighterSpecialState
    {
        public AirborneFighterSpecialChargeStartYuugiState(FighterSM fsm) : base(fsm)
        {
            lag = true;
        }
        public override void OnStateEnter()
        {
            var currentFrame = FrameNum;
            fsm.fighter.entity.PlayAnim("Air_Special_Misc_1");
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
            SetState(new GroundedFighterSpecialChargeStartYuugiState(fsm));
        }
    }
    
    public class GroundedFighterSpecialDrinkStartYuugiState : GroundedFighterSpecialChargeStartState
    {
        public GroundedFighterSpecialDrinkStartYuugiState(FighterSM fsm) : base(fsm)
        {
            lag = true;
        }
    
        public override void OnStateEnter()
        {
            var currentFrame = FrameNum;
            fsm.fighter.entity.PlayAnim("Ground_Special_Misc_2");
            fsm.fighter.entity.SkipCurrentAnimToFrame(currentFrame);
        }
        protected override void TransitionAir()
        {
            IndieSpd += ExternalSpd;
            ExternalSpd = Vector2.zero;
            PlatformInvuln = 0.2f;
            SetState(new AirborneFighterSpecialDrinkStartYuugiState(fsm));
        }
    }
    public class AirborneFighterSpecialDrinkStartYuugiState : AirborneFighterSpecialState
    {
        public AirborneFighterSpecialDrinkStartYuugiState(FighterSM fsm) : base(fsm)
        {
            lag = true;
        }
        public override void OnStateEnter()
        {
            var currentFrame = FrameNum;
            fsm.fighter.entity.PlayAnim("Air_Special_Misc_2");
            fsm.fighter.entity.SkipCurrentAnimToFrame(currentFrame);
        }
        public override void TransitionGround()
        {
            PlatformInvuln = 0;
            SetState(new GroundedFighterSpecialDrinkStartYuugiState(fsm));
        }
    }
    public class GroundedFighterSpecialDrinkingYuugiState : GroundedFighterSpecialChargeStartState
    {
        public GroundedFighterSpecialDrinkingYuugiState(FighterSM fsm) : base(fsm)
        {
            lag = true;
        }
    
        public override void OnStateEnter()
        {
            var currentFrame = FrameNum;
            fsm.fighter.entity.PlayAnim("Ground_Special_Misc_2");
            fsm.fighter.entity.SkipCurrentAnimToFrame(currentFrame);
            fsm.entity.stateVars.damageArmor += 13;
        }
        public override void OnStateUpdate(float timeScale)
        {
            base.OnStateUpdate(timeScale);
            var yuugi = (fsm.fighter.extension as YuugiFighterExtension);
            fsm.entity.stateVars.damageArmor += timeScale * (yuugi.DrinkSpeed + yuugi.DrainSpeed);
            if (SpecialCancelCondition)
            {
                yuugi.TransitionNotDrinking();
                fsm.fighter.entity.SkipCurrentAnimToFrame(490);
            }
        }
        protected override void TransitionAir()
        {
            IndieSpd += ExternalSpd;
            ExternalSpd = Vector2.zero;
            PlatformInvuln = 0.2f;
            SetState(new AirborneFighterSpecialDrinkingYuugiState(fsm));
        }
    }
    public class AirborneFighterSpecialDrinkingYuugiState : AirborneFighterSpecialState
    {
        public AirborneFighterSpecialDrinkingYuugiState(FighterSM fsm) : base(fsm)
        {
            lag = true;
        }
        public override void OnStateEnter()
        {
            var currentFrame = FrameNum;
            fsm.fighter.entity.PlayAnim("Air_Special_Misc_2");
            fsm.fighter.entity.SkipCurrentAnimToFrame(currentFrame);
            fsm.entity.stateVars.damageArmor += 13;
        }
        public override void OnStateUpdate(float timeScale)
        {
            base.OnStateUpdate(timeScale);
            var yuugi = (fsm.fighter.extension as YuugiFighterExtension);
            fsm.entity.stateVars.damageArmor += timeScale * (yuugi.DrinkSpeed + yuugi.DrainSpeed);
            if (SpecialCancelCondition)
            {
                yuugi.TransitionNotDrinking();
                fsm.fighter.entity.SkipCurrentAnimToFrame(490);
            }
        }
        public override void TransitionGround()
        {
            PlatformInvuln = 0;
            SetState(new GroundedFighterSpecialDrinkingYuugiState(fsm));
        }
    }}
