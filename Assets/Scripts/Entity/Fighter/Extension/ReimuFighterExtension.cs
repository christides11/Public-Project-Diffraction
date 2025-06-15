namespace TightStuff.Extension
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;
    using TightStuff.Aesthetics;

    public class ReimuFighterExtension : BaseFighterExtensions
    {
        [SerializeField]
        private GoheiBase _gohei;
        [SerializeField]
        private float _floatTime = 2;
        [SerializeField]
        private float _floatCD = 4;
        [SerializeField]
        private Material _floatMaterial;
        private Material _defaultMaterial;
    
        [SerializeField]
        private BaseProjectileBehaviour _mark;
        [SerializeField]
        private HurtBox _floatingHurtbox;
        private List<Collider2D> hurtboxCols;
    
        [SerializeField]
        private List<AfterImages> _afterImages;
        [SerializeField]
        private AudioSource _auraNoise;
    
        private float currentAlpha;
    
        [System.Serializable]
        private class AfterImages
        {
            [SerializeField]
            public SpriteRenderer sprite;
            public float initialAlpha;
        }
    
        public void FlickGohei(string input)
        {
            if (_gohei == null)
                return;
            if (input != null)
            {
                var vals = input.Split(',').Select(s => s.Trim()).ToArray();
                if (vals.Length == 2)
                {
                    float v1 = 0;
                    float v2 = 0;
    
                    if (float.TryParse(vals[0], out v1) &&
                        float.TryParse(vals[1], out v2))
                        _gohei.FlickGohei(v1, v2);
                }
            }
        }
        protected override void Start()
        {
            base.Start();
            InitializeTeleportMark();
            InitializeFloatHurtbox();
            InitializeFloatVisuals();
        }
    
        public override void GUpdate()
        {
            base.GUpdate();
            CheckDownSpecialInvuln();
            MatchFloatboxWithHurtbox();
            DownSpecialCooldown();
            FloatVisualEffect();
            _mark.entity.stateVars.givenTime = _fighter.entity.stateVars.givenTime;
        }
    
        private void InitializeFloatVisuals()
        {
            foreach (var afterimage in _afterImages)
                afterimage.initialAlpha = afterimage.sprite.color.a;
            _defaultMaterial = _fighter.Ft.palette[_fighter.colorID].colorMat;
        }
    
        private void InitializeTeleportMark()
        {
            _mark.controlling = _fighter.controlling;
            _mark.TransitionControllableState();
            _mark.transform.parent = null;
        }
    
        private void InitializeFloatHurtbox()
        {
            var hurtbox = GetComponentInChildren<HurtBox>();
            var empty = new GameObject();
            hurtboxCols = new List<Collider2D>();
            foreach (var col in hurtbox.Colliders)
            {
                var boxObj = Instantiate(empty);
                boxObj.layer = 11;
                boxObj.transform.parent = _floatingHurtbox.transform;
                boxObj.transform.localPosition *= 0;
                if (col.GetType() == typeof(BoxCollider2D))
                {
                    var boxcol = col as BoxCollider2D;
                    var tempcol = boxObj.AddComponent<BoxCollider2D>();
                    tempcol.offset = boxcol.offset;
                    tempcol.size = boxcol.size;
                    tempcol.isTrigger = true;
                    _floatingHurtbox.Colliders.Add(tempcol);
                }
                if (col.GetType() == typeof(CircleCollider2D))
                {
                    var circol = col as CircleCollider2D;
                    var tempcol = boxObj.AddComponent<CircleCollider2D>();
                    tempcol.offset = circol.offset;
                    tempcol.radius = circol.radius;
                    tempcol.isTrigger = true;
                    _floatingHurtbox.Colliders.Add(tempcol);
                }
                hurtboxCols.Add(col);
            }
        }
    
        private void FloatVisualEffect()
        {
            _auraNoise.volume = currentAlpha * 0.5f;
            if (_fighter.stateVarsF.specialTimer > Time.fixedDeltaTime * 4 || _fighter.entity.actionState.GetState() is GroundedFighterRollMoveState || _fighter.entity.actionState.GetState() is GroundedFighterIntroState)
                currentAlpha = Mathf.Clamp(currentAlpha + Time.fixedDeltaTime * 10, 0, 1);
            else
                currentAlpha = Mathf.Clamp(currentAlpha - Time.fixedDeltaTime * 10, 0, 1);
    
            var _sprite = _fighter.entity.AssociatedRenderers[0] as SpriteRenderer;
    
            foreach (var image in _afterImages)
            {
                image.sprite.enabled = true;
                image.sprite.sprite = _sprite.sprite;
                image.sprite.transform.localScale = transform.lossyScale;
                image.sprite.transform.rotation = transform.rotation;
    
                if (currentAlpha <= 0)
                    image.sprite.transform.position = transform.position;
    
                var alphaI = image.sprite.color;
                alphaI.a = currentAlpha * image.initialAlpha;
                image.sprite.color = alphaI;
            }
    
            if (currentAlpha <= 0)
                return;
    
            var alpha = _sprite.color;
            alpha.a = 1 - currentAlpha * 0.5f;
            _sprite.color = alpha;
    
            if (currentAlpha >= 0.9f)
                _sprite.material = _floatMaterial;
            else
                _sprite.material = _defaultMaterial;
        }
    
        private void MatchFloatboxWithHurtbox()
        {
            for (int i = 0; i < hurtboxCols.Count; i++)
            {
                if (hurtboxCols[i].GetType() == typeof(BoxCollider2D) && _floatingHurtbox.Colliders[i].GetType() == typeof(BoxCollider2D))
                {
                    var boxcol = hurtboxCols[i] as BoxCollider2D;
                    var tempcol = _floatingHurtbox.Colliders[i] as BoxCollider2D;
                    tempcol.offset = boxcol.offset;
                    tempcol.size = boxcol.size;
                }
                if (hurtboxCols[i].GetType() == typeof(CircleCollider2D) && _floatingHurtbox.Colliders[i].GetType() == typeof(CircleCollider2D))
                {
                    var boxcol = hurtboxCols[i] as CircleCollider2D;
                    var tempcol = _floatingHurtbox.Colliders[i] as CircleCollider2D;
                    tempcol.offset = boxcol.offset;
                    tempcol.radius = boxcol.radius;
                }
                _floatingHurtbox.Colliders[i].enabled = _fighter.entity.stateVars.intangible;
            }
            foreach(var hitter in _hitters)
            {
                hitter.enabled = !_fighter.entity.stateVars.intangible;
            }
        }
    
        private void DownSpecialCooldown()
        {
            if (_fighter.stateVarsF.specialCooldown > 0)
            {
                _fighter.stateVarsF.specialCooldown -= Time.fixedDeltaTime * _fighter.entity.NonFreezeTimeScale;
                if (_fighter.stateVarsF.specialCooldown <= 0)
                    _fighter.stateVarsF.maxStamina += 1;
            }
        }
    
        private void CheckDownSpecialInvuln()
        {
            var floatStartup = Time.fixedDeltaTime * 4f;
            var floating = _fighter.stateVarsF.specialTimer > 0 && _fighter.stateVarsF.specialTimer < _floatTime - floatStartup;
            _fighter.entity.stateVars.intangible = floating;
    
            if (_fighter.stateVarsF.specialTimer > 0)
                _fighter.stateVarsF.specialTimer -= Time.fixedDeltaTime * _fighter.entity.NonFreezeTimeScale;
        }
    
        protected override void SpecialDown(Vector2 specialDir)
        {
            //base.SpecialDown(specialDir);
            if (_fighter.stateVarsF.stamina < 0.9f || _fighter.stateVarsF.specialCooldown > 0)
                return;
    
            _fighter.stateVarsF.specialTimer = _floatTime;
            _fighter.stateVarsF.specialCooldown = _floatCD;
            _fighter.stateVarsF.stamina -= 1;
            _fighter.stateVarsF.maxStamina -= 1;
        }
    
        protected override void SpecialHorizontal(FighterState.AttackID specialID)
        {
            if (_fighter.stateVarsF.specialTimer > 0)
                return;
            base.SpecialHorizontal(specialID);
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
    
        protected override void SpecialNeutral(Vector2 specialDir)
        {
            if (_fighter.stateVarsF.specialTimer > 0)
                return;
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
            if (_fighter.stateVarsF.usedAirSpecial || _fighter.stateVarsF.specialTimer > 0)
                return;
            base.SpecialUp(specialDir);
            TransitionSpeciaNoMoveShoot();
            _fighter.stateVarsF.usedAirSpecial = true;
        }
    
        protected override void SpellcardDown(Vector2 specialDir)
        {
            if (_fighter.stateVarsF.specialTimer > 0)
                return;
            if (Mathf.Abs(specialDir.x) > 0.175f)
                _fighter.entity.stateVars.flippedLeft = specialDir.x < 0;
            _fighter.stateVarsF.grazeMeter -= 6;
            _fighter.entity.actionState.SetState(new GroundedFighterSpellcardCharge2StartState(_fighter.entity.actionState as FighterSM));
        }
    
        protected override void SpellcardHorizontal(FighterState.AttackID specialID)
        {
            base.SpellcardHorizontal(specialID);
            _fighter.entity.actionState.SetState(new GroundedFighterSpellcardShootState(_fighter.entity.actionState as FighterSM));
        }
    
        protected override void SpellcardUp(Vector2 specialDir)
        {
            base.SpellcardUp(specialDir);
            _fighter.entity.actionState.SetState(new GroundedFighterSpellcardAttackState(_fighter.entity.actionState as FighterSM));
        }
    
        private void TransitionSmashUp()
        {
            _fighter.entity.actionState.SetState(new AirborneFighterAttackSmashUpState(_fighter.entity.actionState as FighterSM));
        }
        private void TransitionSmashUpMoveable()
        {
            _fighter.entity.actionState.SetState(new AirborneFighterLagMoveableState(_fighter.entity.actionState as FighterSM));
        }
        private void UpAirBoost()
        {
            _fighter.entity.stateVars.indieSpd *= new Vector2(0.5f, 1);
            _fighter.entity.stateVars.indieSpd += new Vector2(_fighter.entity.stateVars.flippedLeft ? -1.75f : 1.75f, _fighter.Ft.airdashSpd - Mathf.Clamp(_fighter.entity.stateVars.indieSpd.y, 0, _fighter.Ft.airdashSpd));
        }
        private void Neutral2Boost()
        {
            _fighter.entity.stateVars.indieSpd *= new Vector2(0.5f, 1);
            _fighter.entity.stateVars.indieSpd += new Vector2(0, _fighter.Ft.airdashSpd - Mathf.Clamp(_fighter.entity.stateVars.indieSpd.y, 0, _fighter.Ft.airdashSpd));
        }
        private void TransitionReimuUpSpecialFloat()
        {
            _fighter.entity.actionState.SetState(new AirborneFighterSpecialReimuUpState(_fighter.entity.actionState as FighterSM));
        }
        private void TransitionReimuUpSpecialLag()
        {
            _fighter.entity.actionState.SetState(new AirborneFighterSpecialReimuUpReappearState(_fighter.entity.actionState as FighterSM));
            var sound = _fighter.aesthetics.SpawnFromPool("ReappearSFX", transform.position, Quaternion.identity, Vector3.one);
            _fighter.entity.stateVars.indieSpd = _mark.entity.stateVars.indieSpd / 5 + Vector2.up * 5;
            _fighter.entity.transform.position = _mark.transform.position;
            _mark.entity.SetEntityActive(false);
        }
    
        private void ShootDTilt()
        {
            if (_fighter.stateVarsF.specialTimer > 0)
                return;
            var aimDir = _fighter.entity.stateVars.flippedLeft ? Vector2.left : Vector2.right;
            var proj = _fighter.shooter.SpawnEntityFromPool("DTilt", _fighter.shooter.transform.position, Quaternion.identity, _fighter.transform.localScale, 6f * aimDir);
            proj.stateVars.flippedLeft = _fighter.entity.stateVars.flippedLeft;
        }
    
        private void ShootAmulet()
        {
            var specialAim = _fighter.stateVarsF.specialAim.magnitude > 0.2f ? _fighter.stateVarsF.specialAim.normalized : (_fighter.entity.stateVars.flippedLeft? Vector2.left : Vector2.right);
            var angle = Mathf.Atan2(specialAim.y, specialAim.x) * Mathf.Rad2Deg;
            var additionalSpd = Vector3.Project(_fighter.entity.stateVars.indieSpd, specialAim.normalized) / 2;
            if (Vector2.Dot(specialAim.normalized, _fighter.entity.stateVars.indieSpd.normalized) <= 0)
                additionalSpd *= 0;
    
            if (_fighter.stateVarsF.specialCharge > Time.fixedDeltaTime * 15)
            {
                _fighter.shooter.SpawnEntityFromPool("Needle", _fighter.shooter.transform.position, Quaternion.Euler(0, 0, angle - 90), _fighter.transform.localScale, 17.5f * specialAim + (Vector2)additionalSpd);
                return;
            }
            var x = _fighter.shooter.SpawnEntityFromPool("Amulet", _fighter.shooter.transform.position, Quaternion.Euler(0, 0, angle - 90), _fighter.transform.localScale, 7f * specialAim + (Vector2)additionalSpd);
            MatchFighterPalette(x);
        }
        private void ShootBuster()
        {
            if (_fighter.entity.stateVars.flippedLeft)
                _fighter.stateVarsF.specialAim *= new Vector2(-1, 1);
    
            var OGspecialAim = _fighter.stateVarsF.specialAim.magnitude > 0.2f ? _fighter.stateVarsF.specialAim.normalized : (_fighter.entity.stateVars.flippedLeft ? Vector2.left : Vector2.right);
            var pos = (Vector3)OGspecialAim * 0.5f - Vector3.up * 0.05f;
            var angle = Mathf.Atan2(OGspecialAim.y, OGspecialAim.x) * Mathf.Rad2Deg;
            var x = _fighter.shooter.SpawnEntityFromPool("Buster", _fighter.shooter.transform.position + pos, Quaternion.Euler(0, 0, angle - 90), _fighter.transform.localScale, 15f * OGspecialAim);
    
            var specialAim = (OGspecialAim * 2 - Vector2.Perpendicular(OGspecialAim)).normalized;
            angle = Mathf.Atan2(specialAim.y, specialAim.x) * Mathf.Rad2Deg;
            var y = _fighter.shooter.SpawnEntityFromPool("Buster", _fighter.shooter.transform.position + pos, Quaternion.Euler(0, 0, angle - 90), _fighter.transform.localScale, 15f * specialAim);
    
            specialAim = (OGspecialAim * 4 - Vector2.Perpendicular(OGspecialAim)).normalized;
            angle = Mathf.Atan2(specialAim.y, specialAim.x) * Mathf.Rad2Deg;
            var x2 = _fighter.shooter.SpawnEntityFromPool("Buster", _fighter.shooter.transform.position + pos, Quaternion.Euler(0, 0, angle - 90), _fighter.transform.localScale, 15f * specialAim);
    
            specialAim = (OGspecialAim * 2 + Vector2.Perpendicular(OGspecialAim)).normalized;
            angle = Mathf.Atan2(specialAim.y, specialAim.x) * Mathf.Rad2Deg;
            var y2 = _fighter.shooter.SpawnEntityFromPool("Buster", _fighter.shooter.transform.position + pos, Quaternion.Euler(0, 0, angle - 90), _fighter.transform.localScale, 15f * specialAim);
    
            specialAim = (OGspecialAim * 4 + Vector2.Perpendicular(OGspecialAim)).normalized;
            angle = Mathf.Atan2(specialAim.y, specialAim.x) * Mathf.Rad2Deg;
            var z = _fighter.shooter.SpawnEntityFromPool("Buster", _fighter.shooter.transform.position + pos, Quaternion.Euler(0, 0, angle - 90), _fighter.transform.localScale, 15f * specialAim);
        }
        private void ShootOrb()
        {
            var specialAim = _fighter.stateVarsF.specialAim.magnitude > 0.2f ? _fighter.stateVarsF.specialAim.normalized : (_fighter.entity.stateVars.flippedLeft ? Vector2.left : Vector2.right);
            if (specialAim.x < 0 ^ _fighter.entity.stateVars.flippedLeft)
                specialAim *= new Vector2(-1, 1);
    
            if (_fighter.stateVarsF.specialCharge > Time.fixedDeltaTime * 15)
            {
                var v = _fighter.shooter.SpawnEntityFromPool("BigOrb", _fighter.shooter.transform.position, Quaternion.identity, _fighter.transform.localScale, 3f * specialAim);
                MatchFighterPalette(v);
                return;
            }
            var x = _fighter.shooter.SpawnEntityFromPool("Orb", _fighter.shooter.transform.position, Quaternion.identity, _fighter.transform.localScale, 4f * specialAim);
            var z = _fighter.shooter.SpawnEntityFromPool("Orb", _fighter.shooter.transform.position, Quaternion.identity, _fighter.transform.localScale, 4f * (specialAim - Vector2.Perpendicular(specialAim)).normalized);
            var y = _fighter.shooter.SpawnEntityFromPool("Orb", _fighter.shooter.transform.position, Quaternion.identity, _fighter.transform.localScale, 4f * (Vector2.Perpendicular(specialAim) + specialAim).normalized);
            MatchFighterPalette(x);
            MatchFighterPalette(y);
            MatchFighterPalette(z);
        }
    
        private void MatchFighterPalette(Entity x)
        {
            if (x != null)
                (x.AssociatedRenderers[0] as SpriteRenderer).material = _fighter.entity.AssociatedRenderers[0].material;
        }
    
        private void ShootMark()
        {
            _mark.transform.position = _fighter.shooter.transform.position;
            _mark.OnShoot(_fighter.entity);
            _mark.entity.SetEntityActive(true);
            _mark.entity.stateVars.indieSpd *= 0;
        }
        private void ShootFantasyOrb(float angle, string orbcolor)
        {
            var angleWithOffset = angle;
            if (_fighter.entity.stateVars.flippedLeft)
                angleWithOffset += 180;
            if ((_fighter.entity.stateVars.flippedLeft && _fighter.stateVarsF.specialCharge == -1) || (!_fighter.entity.stateVars.flippedLeft && _fighter.stateVarsF.specialCharge == 1))
                angleWithOffset *= -1;
            if (_fighter.stateVarsF.specialAim.magnitude > 0.3f)
            {
                angleWithOffset += Mathf.Atan2(_fighter.stateVarsF.specialAim.y, _fighter.stateVarsF.specialAim.x) * Mathf.Rad2Deg;
                if (_fighter.entity.stateVars.flippedLeft)
                    angleWithOffset += 180;
            }
            var specialAim = new Vector2(Mathf.Cos(angleWithOffset * Mathf.Deg2Rad), Mathf.Sin(angleWithOffset * Mathf.Deg2Rad));
            var x = _fighter.shooter.SpawnEntityFromPool(orbcolor, _fighter.shooter.transform.position, Quaternion.identity, _fighter.transform.localScale, 12.5f * specialAim);
        }
    
        private void ShootFantasyOrbGreen(float angle)
        {
            ShootFantasyOrb(angle, "FantasyOrbGreen");
        }
    
        private void ShootFantasyOrbRed(float angle)
        {
            ShootFantasyOrb(angle, "FantasyOrbRed");
        }
        private void ShootFantasyOrbBlue(float angle)
        {
            ShootFantasyOrb(angle, "FantasyOrbBlue");
        }
    
        public void DairParticles()
        {
            if (!_fighter.aesthetics.enabled)
                return;
            var point = _fighter.aesthetics.transform.position;
            var particle = _fighter.aesthetics.SpawnEntityFromPool("DAir", point, Quaternion.identity, _fighter.transform.lossyScale);
            particle.stateVars.flippedLeft = _fighter.transform.lossyScale.x < 0;
            particle.transform.localScale = new Vector3(_fighter.transform.lossyScale.x, _fighter.transform.lossyScale.y, 1);
            particle.SkipCurrentAnimToFrame(0);
        }
        public void GoheiHitSound(HitObject hit)
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
                var sound = _fighter.aesthetics.SpawnFromPool("GoheiWhipHeavy", point, Quaternion.identity, Vector3.one);
                if (sound.TryGetComponent(out AudioSource audio))
                {
                    audio.pitch = 1.5f - Mathf.Clamp(trueDmg / 20f, 0, 0.5f);
                    audio.volume = 1;
                }
            }
            else if (trueDmg > 5)
            {
                var sound = _fighter.aesthetics.SpawnFromPool("GoheiWhipMedium", point, Quaternion.identity, Vector3.one);
                if (sound.TryGetComponent(out AudioSource audio))
                {
                    audio.pitch = 1.25f - Mathf.Clamp(trueDmg / 10f, 0, 0.5f);
                    audio.volume = Mathf.Clamp(trueDmg / 20f, 0.75f, 1);
                }
            }
            else
            {
                var sound = _fighter.aesthetics.SpawnFromPool("GoheiWhipLight", point, Quaternion.identity, Vector3.one);
                if (sound.TryGetComponent(out AudioSource audio))
                {
                    audio.pitch = 1.25f - Mathf.Clamp(trueDmg / 5, 0, 0.5f);
                    audio.volume = 0.5f;
                }
            }
        }
        public void PhyisicalHitSound(HitObject hit)
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
                    audio.pitch = 1.25f - Mathf.Clamp(trueDmg / 20, 0, 0.25f);
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
        public void SpurtGoheiParticles(HitObject hit)
        {
            if (!_fighter.aesthetics.enabled)
                return;
            if (hit.box.entity.stateVars.invulnerable || hit.box is ShieldBox)
                return;
            if (hit.box.entity.gameObject.CompareTag("Floating"))
                return;
            var trueDmg = Mathf.Clamp(hit.hitbox.hitProperties.damage, 0, hit.box.entity.et.health);
            if (trueDmg > 0)
            {
                var point = hit.hitCol.ClosestPoint((Vector2)hit.hitBy.transform.position + hit.hitBy.offset);
                var particle = _fighter.aesthetics.SpawnEntityFromPool("HitGohei", point, hit.box.transform.rotation, hit.box.transform.lossyScale);
                var angle = Mathf.Atan2(hit.box.entity.stateVars.indieSpd.normalized.y, hit.box.entity.stateVars.indieSpd.normalized.x) * Mathf.Rad2Deg;
    
                var rad = angle * Mathf.Deg2Rad;
    
                particle.transform.up = new Vector3(-Mathf.Sin(rad), Mathf.Cos(rad), 0).normalized;
                particle.SkipCurrentAnimToFrame(0);
            }
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
            var ad = _fighter.aesthetics.SpawnEntityFromPool("FSmashDust", fighter.transform.position - Vector3.up * 0.95f, fighter.transform.rotation, Vector3.one);
            ad.transform.position -= 0.4f * (fighter.stateVars.flippedLeft ? Vector3.right : Vector3.left);
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
        public void DSmashParticle()
        {
            if (!_fighter.aesthetics.enabled)
                return;
            _camShake.ShakeCamera(5, 0.2f);
            var fighter = _fighter.entity;
            var ad = _fighter.aesthetics.SpawnEntityFromPool("DSmashDust", fighter.transform.position - Vector3.up * 0.95f, fighter.transform.rotation, Vector3.one * 0.75f);
            ad.stateVars.flippedLeft = fighter.stateVars.flippedLeft;
        }
        public void SpurtShockwaveHorizontalParticle(HitObject hit)
        {
            if (!_fighter.aesthetics.enabled)
                return;
            if (hit.box.entity.stateVars.invulnerable || hit.box is ShieldBox)
                return;
            var point = hit.hitCol.ClosestPoint((Vector2)hit.hitBy.transform.position + hit.hitBy.offset);
            var angle = (hit.hitbox.entity.stateVars.flippedLeft? 180 : 0) + 90;
            var rad = angle * Mathf.Deg2Rad;
            var particle = _fighter.aesthetics.SpawnEntityFromPool("Shockwave", point, Quaternion.identity, Vector3.one);
    
            particle.transform.up = new Vector3(-Mathf.Sin(rad), Mathf.Cos(rad), 0).normalized;
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
    
            if (_fighter.ActionState.GetState() is GroundedFighterAttackSmashState)
            {
                splash1 = "HitFSmash2";
                splash2 = "HitFSmash";
                if (_fighter.stateVarsF.currentAttackID == FighterState.AttackID.Down)
                {
                    splash1 = "HitDSmash2";
                    splash2 = "HitDSmash";
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
    
            if (_fighter.ActionState.GetState() is GroundedFighterAttackSmashState)
            {
                splash1 = "HitFSmash2";
                splash2 = "HitFSmash";
                if (_fighter.stateVarsF.currentAttackID == FighterState.AttackID.Down)
                {
                    splash1 = "HitDSmash2";
                    splash2 = "HitDSmash";
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
    
        public void MatchFreeze(HitObject hit)
        {
            if (hit.hitbox is GrazeBox)
                return;
            hit.box.entity.stateVars.indieSpd *= 0;
            hit.box.owner.stateVars.freezeTime = hit.box.entity.stateVars.freezeTime;
        }
        public void DecreaseFloatTime(HitObject hit)
        {
            if (hit.hitbox is GrazeBox)
                return;
            _fighter.stateVarsF.specialTimer -= hit.hitbox.hitProperties.damage / 10f;
        }
        public void ComboSpecial2Dir()
        {
            if (_fighter.controlling.specialButton.raw || (_fighter.controlling.specialStick.raw.magnitude > 0.75f))
            {
                Vector2 dir = GetAttackSpecialDir();
                TransitionSpecialCombo(dir);
            }
        }
    
        private void SetFantasyOrbAim()
        {
            if (_fighter.controlling.cStick.raw.magnitude > 0.3f)
                _fighter.stateVarsF.specialAim = _fighter.controlling.cStick.raw;
            else if (_fighter.controlling.moveStick.raw.magnitude > 0.3f)
                _fighter.stateVarsF.specialAim = _fighter.controlling.moveStick.raw;
            _fighter.stateVarsF.specialCharge = (_fighter.controlling.attackButton.raw || _fighter.controlling.smashButton.raw || _fighter.controlling.specialButton.raw || _fighter.controlling.smashToggleButton.raw || _fighter.controlling.specialToggle.raw || _fighter.controlling.dashToggleButton.raw) ? 1 : -1;
        }
        private void ResetFantasyOrbAim()
        {
            _fighter.stateVarsF.specialAim = Vector2.zero;
            _fighter.stateVarsF.specialCharge = 0;
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
    
    public class AirborneFighterAttackSmashUpState : AirborneFighterState
    {
        public AirborneFighterAttackSmashUpState(FighterSM fsm) : base(fsm)
        {
            lag = true;
            moveable = false;
        }
    
        public override void OnStateEnter()
        {
            base.OnStateEnter();
            fsm.fighter.MovePlayer(new Vector2(1f + AttackCharge * 1.25f, 4f + AttackCharge * 4f));
        }
    }
    public class AirborneFighterLagMoveableState : AirborneFighterState
    {
        public AirborneFighterLagMoveableState(FighterSM fsm) : base(fsm)
        {
            lag = true;
        }
    }
    
    public class AirborneFighterSpecialReimuUpState : AirborneFighterState
    {
        public AirborneFighterSpecialReimuUpState(FighterSM fsm) : base(fsm)
        {
            lag = true;
            moveable = false;
        }
        public override void OnStateEnter()
        {
            base.OnStateEnter();
            IndieSpd = new Vector2(FlippedLeft? 2 : -2, 1);
            ExternalSpd *= 0;
            SelfSpd *= 0;
            Gravity = false;
        }
        public override void OnStateExit()
        {
            base.OnStateExit();
            Gravity = true;
        }
    
        public override void TransitionGround()
        {
            //base.TransitionGround();
        }
    }
    public class AirborneFighterSpecialReimuUpReappearState : AirborneFighterState
    {
        public AirborneFighterSpecialReimuUpReappearState(FighterSM fsm) : base(fsm)
        {
            lag = true;
            moveable = false;
        }
        public override void OnStateEnter()
        {
            base.OnStateEnter();
            CurrentAttackID = AttackID.Back;
            fsm.fighter.entity.PlayAnim("Air_Special_Misc_1");
            if (Mathf.Abs(Controlling.moveStick.raw.x) > 0.2f)
                FlippedLeft = Controlling.moveStick.raw.x < 0;
            if (Controlling.shieldButton.raw)
                FlippedLeft = !FlippedLeft;
            if (AttackHoldCondition)
            {
                fsm.fighter.entity.PlayAnim("Air_Special_Attack");
                Controlling.cStick.ConsumeBufferHold();
                Controlling.attackButton.ConsumeBuffer();
            }
            else if (SpecialHoldCondition)
            {
                var specialDir = GetAttackDir(StickSpecial ? Controlling.attackButton : Controlling.specialButton, Controlling.cStick);
                SpecialAim = specialDir;
                SetState(new AirborneFighterSpecialReimuUpBusterReadyState(fsm));
                Controlling.cStick.ConsumeBufferHold();
                Controlling.specialButton.ConsumeBuffer();
            }
        }
        public new Vector2 GetAttackDir(Controller.Button button, Controller.Stick stick)
        {
            if (Controlling.neutralLock.raw)
                return Vector2.zero;
            if (stick.BufferHold())
                return stick.holdInitPos.normalized;
    
            if (button.stickAccompanimentOnTap.magnitude > 0.3)
                return button.stickAccompanimentOnTap.normalized;
    
            return Vector2.zero;
        }
    }
    public class AirborneFighterSpecialReimuUpBusterReadyState : AirborneFighterState
    {
        public AirborneFighterSpecialReimuUpBusterReadyState(FighterSM fsm) : base(fsm)
        {
            lag = true;
            moveable = false;
        }
    
        public override void OnStateUpdate(float timeScale)
        {
            base.OnStateUpdate(timeScale);
            if (FrameNum == 90)
            {
                (fsm.fighter.extension as ReimuFighterExtension).TransitionSpecialCombo(SpecialAim);
            }
        }
    }}
