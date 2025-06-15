namespace TightStuff
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Drawing;
    using UnityEngine;
    using UnityEngine.Events;
    
    public class BaseProjectileBehaviour : Controllable
    {
        [SerializeField]
        private List<InteractBox> _boxes;
        public UnityEvent ShootEvent;
        public UnityEvent DestroyEvent;
        public UnityEvent TimeOutEvent;
        public UnityEvent LifeOutEvevnt;
        public OnCollideCastEvent CollideCastEvent;
        public OnCollidePhysicalEvent CollidePhysicalEvent;
    
        [SerializeField]
        private TrailRenderer _trailRenderer;
    
        public float bounceDamp = 0;
        public int collisionDamage = 3;
        [SerializeField]
        private float _maxLifeTime = 4;
        [SerializeField]
        public bool preventRespawnWhileActive;
        [SerializeField]
        public bool preventRespawnWhileLifetimeNotExpired;
    
        private Pooler _particleShooter;
    
        public float LifeTime { get => entity.stateVars.genericTimer; set => entity.stateVars.genericTimer = value; }
        public float MaxLifeTime => _maxLifeTime;
    
        public LayerMask collisionLayers;
    
        // Start is called before the first frame update
        void Start()
        {
            _particleShooter = GetComponent<Pooler>();
            order = 1500;
            entity = GetComponent<Entity>();
            if (entity != null)
                entity.order += 10;
            var hitboxes = GetComponentsInChildren<Hitter>();
            foreach (var hitbox in hitboxes)
                ShootEvent.AddListener(hitbox.ClearHits);
    
            var sm = new ProjectileSM(this);
            entity.actionState = sm;
            entity.actionState.entity = entity;
            entity.actionState.SetState(new ProjectileAirborneState(sm));
    
            entity.physicsState.SetState(new InAirNoTransition(entity.physicsState));
            _trailRenderer = GetComponent<TrailRenderer>();
        }
    
        public override void GUpdate()
        {
            base.GUpdate();
            if (LifeTime <= MaxLifeTime)
                LifeTime += entity.TrueTimeScale * Time.fixedDeltaTime;
            if (_trailRenderer != null)
                _trailRenderer.emitting = entity.enabled && LifeTime > Time.fixedDeltaTime;
            CheckAlive();
        }
    
        public void OnShoot(Entity owner)
        {
            entity.UpdateTransformState();
            LifeTime = 0;
            entity.stateVars.percent = 0;
            entity.stateVars.freezeTime = 0;
            entity.SkipCurrentAnimToFrame(0);
            if (owner.gameObject.TryGetComponent(out BaseFighterBehavior fighter))
            {
                controlling = fighter.controlling;
                AssignOwnerToBoxes(fighter.entity);
            }
            else if (owner.gameObject.TryGetComponent(out BaseProjectileBehaviour proj))
            {
                if (proj.controlling != null)
                {
                    controlling = proj.controlling;
                    if (controlling.gameObject.TryGetComponent(out BaseFighterBehavior projFighter))
                        AssignOwnerToBoxes(projFighter.entity);
                }
            }
            else
                AssignOwnerToBoxes(owner);
            ShootEvent?.Invoke();
        }
    
        private void AssignOwnerToBoxes(Entity owner)
        {
            if (_boxes != null)
            {
                foreach (var box in _boxes)
                {
                    box.owner = owner;
                    if (box is HitBox)
                        (box as HitBox).hitProperties.owner = owner;
                }
            }
        }
    
        public void DamageProjectile(float dmg)
        {
            entity.stateVars.percent += dmg;
        }
        public void DamageProjectileBasedOnSpeedOnImpact(float multiplier)
        {
            entity.stateVars.percent += entity.stateVars.indieSpd.magnitude * multiplier;
        }
        public void DamageOnHitBasedOnHealth(HitObject hit)
        {
            if (hit.box.gameObject.gameObject.CompareTag("Floating"))
                return;
            if (hit.box is HurtBox || hit.box is ShieldBox)
                entity.stateVars.percent += hit.box.entity.et.health;
        }
        public void DamageBounceProjectile(HitObject hit)
        {
            if (hit.box.gameObject.gameObject.CompareTag("Floating"))
                return;
            if (hit.hitbox.hitProperties.damage <= 0)
                return;
            if (hit.box is HurtBox || hit.box is ShieldBox)
                entity.stateVars.percent += collisionDamage;
            if (hit.box is HitBox)
                entity.stateVars.percent += Mathf.Clamp(collisionDamage - (hit.box as HitBox).hitProperties.damage, 0, collisionDamage);
        }
        public void KillOnHitHurtShield(HitObject hit)
        {
            if (hit.box.gameObject.gameObject.CompareTag("Floating"))
                return;
            if ((hit.box is HurtBox || hit.box is ShieldBox) && hit.box.entity.et.health > entity.et.health)
                entity.stateVars.percent += 999;
        }
        public void DamageBy(HitObject hit)
        {
            if (hit.box.gameObject.gameObject.CompareTag("Floating"))
                return;
            if (hit.hitbox.hitProperties.damage <= 0)
                return;
            entity.stateVars.percent += Mathf.Clamp(collisionDamage - hit.hitbox.hitProperties.damage, 0, collisionDamage);
        }
        public void Bounce(ContactPoint2D[] contacts)
        {
            entity.stateVars.indieSpd = Vector2.Reflect(entity.stateVars.indieSpd, GetTotalContactNormals(contacts).normalized) * (1 - bounceDamp);
        }
        public void BounceOnBoxes(HitObject hit)
        {
            if (hit.box.gameObject.gameObject.CompareTag("Floating"))
                return;
            var colNormals = Vector2.zero;
            RaycastHit2D norm = Physics2D.Raycast(transform.position, (hit.box.transform.position - transform.position).normalized);
            if (Vector2.Dot(entity.stateVars.indieSpd.normalized, norm.normal) < 0)
                colNormals += norm.normal;
            entity.stateVars.indieSpd = Vector2.Reflect(entity.stateVars.indieSpd, colNormals.normalized) * (1 - bounceDamp);
        }
        public void BounceByBoxes(HitObject hit)
        {
            if (hit.box.gameObject.gameObject.CompareTag("Floating"))
                return;
            var colNormals = Vector2.zero;
            RaycastHit2D norm = Physics2D.Raycast(transform.position, (hit.hitbox.transform.position - transform.position).normalized);
            if (Vector2.Dot(entity.stateVars.indieSpd.normalized, norm.normal) < 0)
                colNormals += norm.normal;
            entity.stateVars.indieSpd = Vector2.Reflect(entity.stateVars.indieSpd, colNormals.normalized) * (1 - bounceDamp);
        }
    
        public void FlipBasedOnSpeed()
        {
            entity.stateVars.flippedLeft = entity.stateVars.indieSpd.x < 0;
        }
    
        public void ConditionalSpeedInherit(HitObject hit)
        {
            if (hit.box.entity.gameObject.layer == 16)
                return;
            if (hit.box.entity.stateVars.invulnerable)
                return;
            var LaunchSpd = hit.hitbox.entity.stateVars.indieSpd;
            if (hit.box is ShieldBox)
            {
                if (hit.box.owner.actionState.GetState() is GroundedFighterPerfectShieldState || hit.box.owner.actionState.GetState() is GroundedFighterPerfectShieldEndState)
                    return;
                LaunchSpd *= 0.25f;
                if (!hit.box.owner.stateVars.aerial)
                    LaunchSpd *= Vector2.right;
            }
            hit.box.entity.stateVars.lastLaunchSpd += LaunchSpd;
            if (hit.hitbox.entity.stateVars.indieSpd.magnitude > hit.hitbox.entity.et.airSpd + 2)
                hit.box.entity.stateVars.indieSpd += LaunchSpd;
        }
        public void NoCameraShake(HitObject hit)
        {
            entity.stateVars.indieSpd *= 0;
        }
        public Vector2 GetTotalContactNormals(ContactPoint2D[] contacts)
        {
            var normal = Vector2.zero;
            var count = 0;
            foreach (ContactPoint2D contact in contacts)
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
        public void Home(HitObject hit)
        {
            if (!hit.box.entity.TryGetComponent(out BaseFighterBehavior fighter))
                return;
            if (Vector2.Dot(entity.stateVars.indieSpd.normalized, (hit.box.transform.position - transform.position).normalized) > 0 || entity.stateVars.indieSpd.magnitude < 10)
                entity.stateVars.indieSpd = (entity.stateVars.indieSpd.normalized * 10 + (Vector2)(hit.box.transform.position - transform.position).normalized).normalized * entity.stateVars.indieSpd.magnitude;
        }
    
        public void TransitionPhysicsLaunchedState()
        {
            if (entity.stateVars.indieSpd.magnitude > entity.et.airSpd)
                entity.physicsState.SetState(new ProjectileLaunched(entity.physicsState));
        }
        public void TransitionControllableState()
        {
            entity.actionState.SetState(new ProjectileAirborneControllableState(entity.actionState as ProjectileSM));
        }
        public void TransitionControllableHorizontalState()
        {
            entity.actionState.SetState(new ProjectileAirborneControllableHorizontalState(entity.actionState as ProjectileSM));
        }
        public void TransitionMoveStraightState()
        {
            entity.actionState.SetState(new ProjectileAirborneMoveStraightState(entity.actionState as ProjectileSM));
        }
        public void TransitionRemoteAimState()
        {
            entity.actionState.SetState(new ProjectileAirborneRemoteAimState(entity.actionState as ProjectileSM));
        }
        public void TransitionPropelStraightState()
        {
            entity.actionState.SetState(new ProjectileAirbornePropelForwardState(entity.actionState as ProjectileSM));
        }
        public void TransitionIgnorePlatformCastState()
        {
            entity.actionState.SetState(new ProjectileAirborneCollideCastIgnoresPlatformState(entity.actionState as ProjectileSM));
        }
        public void TransitionDefaultState()
        {
            entity.actionState.SetState(new ProjectileAirborneState(entity.actionState as ProjectileSM));
        }
        public void SetParentToOwner()
        {
            transform.parent = controlling.transform;
        }
        private void CheckAlive()
        {
            if (!entity.stateVars.enabled)
                return;
            if (entity.stateVars.percent >= entity.et.health || LifeTime > _maxLifeTime)
            {
                entity.SetEntityActive(false);
                entity.stateVars.freezeTime = 0;
                DestroyEvent?.Invoke();
                if (LifeTime > _maxLifeTime)
                    TimeOutEvent?.Invoke();
                else
                    LifeOutEvevnt?.Invoke();
            }
        }
    
        public void SpurtSplashParticle(HitObject hit)
        {
            if (_particleShooter == null)
                return;
            if (hit.box.entity.gameObject.CompareTag("Floating"))
                return;
            if (hit.box.entity.stateVars.invulnerable || hit.box is ShieldBox)
                return;
            var damageDealth = Mathf.Clamp(hit.hitbox.hitProperties.damage, 0, hit.box.entity.et.health);
            var point = hit.hitCol.ClosestPoint((Vector2)hit.hitBy.transform.position + hit.hitBy.offset);
            var randInt = UnityEngine.Random.Range(0, 2) * 2 - 1;
            var randInt2 = UnityEngine.Random.Range(0, 2) * 2 - 1;
            var angle = Mathf.Atan2(hit.box.entity.stateVars.indieSpd.normalized.y, hit.box.entity.stateVars.indieSpd.normalized.x) * Mathf.Rad2Deg;
            if (hit.box.entity.stateVars.indieSpd.magnitude < 5)
                angle = Random.Range(0, 360);
    
            var rad = (angle - 25 * randInt2) * Mathf.Deg2Rad;
            var rad2 = (angle + 25 * randInt2) * Mathf.Deg2Rad;
            var rad3 = (angle + 180) * Mathf.Deg2Rad;
    
            var particle = _particleShooter.SpawnEntityFromPool("HitSplash2", point, Quaternion.identity, Vector3.one);
            var particle2 = _particleShooter.SpawnEntityFromPool("HitSplash", point, Quaternion.identity, Vector3.one);
    
            particle.transform.up = new Vector3(-Mathf.Sin(rad), Mathf.Cos(rad), 0).normalized;
            particle.transform.localScale = Vector3.one * Mathf.Clamp(Mathf.Pow(damageDealth, 2) * 0.004f + 0.2f, 0.2f, 2.2f);
            particle.transform.localScale = new Vector3(particle.transform.localScale.x, particle.transform.localScale.y * randInt, 1);
            particle.stateVars.selfTime = 1f;
            particle.PlayParticles();
    
            particle2.transform.up = new Vector3(-Mathf.Sin(rad2), Mathf.Cos(rad2), 0).normalized;
            particle2.transform.localScale = Vector3.one * Mathf.Clamp(Mathf.Pow(damageDealth, 2) * 0.004f + 0.4f, 0.4f, 1.8f);
            particle2.transform.localScale = new Vector3(particle2.transform.localScale.x, particle2.transform.localScale.y * randInt, 1);
            particle2.stateVars.selfTime = 1f;
            particle2.PlayParticles();
    
            if (hit.box.entity.stateVars.lastLaunchSpd.magnitude > 50)
            {
                var particle3 = _particleShooter.SpawnEntityFromPool("HitSplash", point, Quaternion.identity, Vector3.one);
                particle3.transform.up = new Vector3(-Mathf.Sin(rad3), Mathf.Cos(rad3), 0).normalized;
                particle3.transform.localScale = Vector3.one * Mathf.Clamp(Mathf.Pow(damageDealth, 2) * 0.004f + 0.2f, 0.2f, 2.2f);
                particle3.transform.localScale = new Vector3(particle.transform.localScale.x, particle.transform.localScale.y * randInt, 1);
                particle3.stateVars.selfTime = 1f;
                particle3.PlayParticles();
            }
        }
        public void SpurtSplashBigParticle(HitObject hit)
        {
            if (_particleShooter == null)
                return;
            if (hit.box.entity.gameObject.CompareTag("Floating"))
                return;
            if (hit.box.entity.stateVars.invulnerable || hit.box is ShieldBox)
                return;
            var damageDealth = Mathf.Clamp(hit.hitbox.hitProperties.damage, 0, hit.box.entity.et.health);
            var point = hit.hitCol.ClosestPoint((Vector2)hit.hitBy.transform.position + hit.hitBy.offset);
            var randInt = UnityEngine.Random.Range(0, 2) * 2 - 1;
            var randInt2 = UnityEngine.Random.Range(0, 2) * 2 - 1;
            var angle = Mathf.Atan2(hit.box.entity.stateVars.indieSpd.normalized.y, hit.box.entity.stateVars.indieSpd.normalized.x) * Mathf.Rad2Deg;
            if (hit.box.entity.stateVars.lastLaunchSpd.magnitude < 5)
                angle = Random.Range(0, 360);
    
            var rad = (angle - 25 * randInt2) * Mathf.Deg2Rad;
            var rad2 = (angle + 25 * randInt2) * Mathf.Deg2Rad;
            var rad3 = (angle + 180) * Mathf.Deg2Rad;
    
            var particle = _particleShooter.SpawnEntityFromPool("HitSplash2", point, Quaternion.identity, Vector3.one);
            var particle2 = _particleShooter.SpawnEntityFromPool("HitSplash", point, Quaternion.identity, Vector3.one);
    
            particle.transform.up = new Vector3(-Mathf.Sin(rad), Mathf.Cos(rad), 0).normalized;
            particle.transform.localScale = Vector3.one * Mathf.Clamp(Mathf.Pow(damageDealth, 2) * 0.004f + 0.2f, 0.5f, 2.2f);
            particle.transform.localScale = new Vector3(particle.transform.localScale.x, particle.transform.localScale.y * randInt, 1);
            particle.stateVars.selfTime = 1f;
            particle.PlayParticles();
    
            particle2.transform.up = new Vector3(-Mathf.Sin(rad2), Mathf.Cos(rad2), 0).normalized;
            particle2.transform.localScale = Vector3.one * Mathf.Clamp(Mathf.Pow(damageDealth, 2) * 0.004f + 0.4f, 0.7f, 1.8f);
            particle2.transform.localScale = new Vector3(particle2.transform.localScale.x, particle2.transform.localScale.y * randInt, 1);
            particle2.stateVars.selfTime = 1f;
            particle2.PlayParticles();
    
            if (hit.box.entity.stateVars.lastLaunchSpd.magnitude > 50)
            {
                var particle3 = _particleShooter.SpawnEntityFromPool("HitSplash", point, Quaternion.identity, Vector3.one);
                particle3.transform.up = new Vector3(-Mathf.Sin(rad3), Mathf.Cos(rad3), 0).normalized;
                particle3.transform.localScale = Vector3.one * Mathf.Clamp(Mathf.Pow(damageDealth, 2) * 0.004f + 0.2f, 0.6f, 2.2f);
                particle3.transform.localScale = new Vector3(particle.transform.localScale.x, particle.transform.localScale.y * randInt, 1);
                particle3.stateVars.selfTime = 1f;
                particle3.PlayParticles();
            }
        }
        public void SpurtSplashBiggerParticle(HitObject hit)
        {
            if (_particleShooter == null)
                return;
            if (hit.box.entity.gameObject.CompareTag("Floating"))
                return;
            if (hit.box.entity.stateVars.invulnerable || hit.box is ShieldBox)
                return;
            var point = hit.hitCol.ClosestPoint((Vector2)hit.hitBy.transform.position + hit.hitBy.offset);
            var randInt = UnityEngine.Random.Range(0, 2) * 2 - 1;
            var randInt2 = UnityEngine.Random.Range(0, 2) * 2 - 1;
            var angle = Mathf.Atan2(hit.box.entity.stateVars.indieSpd.normalized.y, hit.box.entity.stateVars.indieSpd.normalized.x) * Mathf.Rad2Deg;
            if (hit.box.entity.stateVars.lastLaunchSpd.magnitude < 5)
                angle = Random.Range(0, 360);
    
            var rad = (angle - 25 * randInt2) * Mathf.Deg2Rad;
            var rad2 = (angle + 25 * randInt2) * Mathf.Deg2Rad;
            var rad3 = (angle + 180) * Mathf.Deg2Rad;
    
            var particle = _particleShooter.SpawnEntityFromPool("HitSplash2", point, Quaternion.identity, Vector3.one);
            var particle2 = _particleShooter.SpawnEntityFromPool("HitSplash", point, Quaternion.identity, Vector3.one);
    
            particle.transform.up = new Vector3(-Mathf.Sin(rad), Mathf.Cos(rad), 0).normalized;
            particle.transform.localScale = Vector3.one * 1.5f;
            particle.transform.localScale = new Vector3(particle.transform.localScale.x, particle.transform.localScale.y * randInt, 1);
            particle.stateVars.selfTime = 1f;
            particle.PlayParticles();
    
            particle2.transform.up = new Vector3(-Mathf.Sin(rad2), Mathf.Cos(rad2), 0).normalized;
            particle2.transform.localScale = Vector3.one * 1.5f;
            particle2.transform.localScale = new Vector3(particle2.transform.localScale.x, particle2.transform.localScale.y * randInt, 1);
            particle2.stateVars.selfTime = 1f;
            particle2.PlayParticles();
    
            var particle3 = _particleShooter.SpawnEntityFromPool("HitSplash", point, Quaternion.identity, Vector3.one);
            particle3.transform.up = new Vector3(-Mathf.Sin(rad3), Mathf.Cos(rad3), 0).normalized;
            particle3.transform.localScale = Vector3.one * 1.5f;
            particle3.transform.localScale = new Vector3(particle.transform.localScale.x, particle.transform.localScale.y * randInt, 1);
            particle3.stateVars.selfTime = 1f;
            particle3.PlayParticles();
        }
    
        public void PhyisicalHitSound(HitObject hit)
        {
            if (hit.box.entity.stateVars.invulnerable || hit.box is ShieldBox)
                return;
            if (hit.box.entity.gameObject.CompareTag("Floating"))
                return;
            var damageDealth = Mathf.Clamp(hit.hitbox.hitProperties.damage, 0, hit.box.entity.et.health);
            var hitHitbox = 1f;
            if (hit.box is HitBox)
                hitHitbox = 0.25f;
            var trueDmg = Mathf.Clamp(hit.hitbox.hitProperties.damage, 0, hit.box.entity.et.health);
            var point = hit.hitCol.ClosestPoint((Vector2)hit.hitBy.transform.position + hit.hitBy.offset);
            if (damageDealth > 10)
            {
                var sound = _particleShooter.SpawnFromPool("PhysicalStrongSFX", point, Quaternion.identity, Vector3.one);
                if (sound.TryGetComponent(out AudioSource audio))
                {
                    audio.pitch = 1.75f - Mathf.Clamp(damageDealth / 20f, 0, 1);
                    audio.volume = Mathf.Clamp(damageDealth / 20f, 0.5f, 1) * hitHitbox;
                    var sound2 = _particleShooter.SpawnFromPool("PhysicalLightSFX", point, Quaternion.identity, Vector3.one);
                }
            }
            else if (damageDealth > 5)
            {
                var sound = _particleShooter.SpawnFromPool("PhysicalMediumSFX", point, Quaternion.identity, Vector3.one);
                if (sound.TryGetComponent(out AudioSource audio))
                {
                    audio.pitch = 1.75f - Mathf.Clamp(damageDealth / 10f, 0, 1);
                    audio.volume = Mathf.Clamp(damageDealth / 20f, 0.5f, 1) * hitHitbox;
                }
            }
            else
            {
                var sound = _particleShooter.SpawnFromPool("PhysicalLightSFX", point, Quaternion.identity, Vector3.one);
                if (sound.TryGetComponent(out AudioSource audio))
                {
                    audio.pitch = 1.25f - Mathf.Clamp(damageDealth / 10, 0, 0.5f);
                    audio.volume = 1 * hitHitbox;
                }
            }
        }
    
        public void EnergyHitSound(HitObject hit)
        {
            if (hit.box.entity.stateVars.invulnerable || hit.box is ShieldBox)
                return;
            if (hit.box.entity.gameObject.CompareTag("Floating"))
                return;
            var damageDealth = Mathf.Clamp(hit.hitbox.hitProperties.damage, 0, hit.box.entity.et.health);
            var hitHitbox = 1f;
            if (hit.box is HitBox)
                hitHitbox = 0.25f;
            var point = hit.hitCol.ClosestPoint((Vector2)hit.hitBy.transform.position + hit.hitBy.offset);
            if (damageDealth > 10)
            {
                var sound = _particleShooter.SpawnFromPool("EnergyHitHeavy", point, Quaternion.identity, Vector3.one);
                if (sound.TryGetComponent(out AudioSource audio))
                {
                    audio.pitch = 2f - Mathf.Clamp(damageDealth / 20f, 0, 1);
                    audio.volume = Mathf.Clamp(damageDealth / 20f, 0.5f, 1) * hitHitbox;
                    var sound2 = _particleShooter.SpawnFromPool("EnergyHitLight", point, Quaternion.identity, Vector3.one);
                }
            }
            else if (damageDealth > 5)
            {
                var sound = _particleShooter.SpawnFromPool("EnergyHitLight", point, Quaternion.identity, Vector3.one);
                if (sound.TryGetComponent(out AudioSource audio))
                {
                    audio.pitch = 1.25f - Mathf.Clamp(damageDealth / 40f, 0, 0.25f);
                    audio.volume = Mathf.Clamp(damageDealth / 20f, 0.75f, 1) * hitHitbox;
                }
            }
            else
            {
                var sound = _particleShooter.SpawnFromPool("EnergyHitMedium", point, Quaternion.identity, Vector3.one);
                if (sound.TryGetComponent(out AudioSource audio))
                {
                    audio.pitch = 1.3f - Mathf.Clamp(damageDealth / 20, 0, 0.25f);
                    audio.volume = 0.5f * hitHitbox;
                }
            }
        }
        public void RandomRangeSound(HitObject hit)
        {
            if (hit.box.entity.stateVars.invulnerable || hit.box is ShieldBox)
                return;
            if (hit.box.gameObject.CompareTag("Floating"))
                return;
            var hitHitbox = 1f;
            if (hit.box is HitBox)
                hitHitbox = 1.25f;
            var point = hit.hitCol.ClosestPoint((Vector2)hit.hitBy.transform.position + hit.hitBy.offset);
            var sound = _particleShooter.SpawnFromPool("SoundRandom", point, Quaternion.identity, Vector3.one);
            if (sound.TryGetComponent(out AudioSource audio))
            {
                audio.pitch = Mathf.Clamp(1.25f - Random.value * 0.5f, hitHitbox, 1.25f);
            }
        }
        public void SoundOnHit(HitObject hit)
        {
            if (hit.box.entity.stateVars.invulnerable || hit.box is ShieldBox)
                return;
            if (hit.box.entity.gameObject.CompareTag("Floating"))
                return;
            var point = hit.hitCol.ClosestPoint((Vector2)hit.hitBy.transform.position + hit.hitBy.offset);
            var sound = _particleShooter.SpawnFromPool("HitSound", point, Quaternion.identity, Vector3.one);
        }
        public void SpawnObj(string objName)
        {
            var x = _particleShooter.SpawnEntityFromPool(objName, transform.position, Quaternion.identity, transform.lossyScale);
            if (x != null)
                x.stateVars.flippedLeft = entity.stateVars.flippedLeft;
        }
        public void SpawnObjRandomFlippedLeft(string objName)
        {
            var x = _particleShooter.SpawnEntityFromPool(objName, transform.position, Quaternion.identity, transform.lossyScale);
            if (x != null)
                x.stateVars.flippedLeft = MatchManager.FrameNum % 2 == 0;
        }
        public void SpawnFireworks()
        {
            var variance = (MatchManager.FrameNum % 10f) / 10f - 0.5f;
            _particleShooter.SpawnEntityFromPool("Fireworks", transform.position, Quaternion.identity, transform.lossyScale * 0.15f, 20 * (Vector2.up + Vector2.right * (0.25f - variance)).normalized);
            _particleShooter.SpawnEntityFromPool("Fireworks2", transform.position, Quaternion.identity, transform.lossyScale * 0.1f, 25 * (Vector2.up + Vector2.left * (0.25f + variance)).normalized);
            _particleShooter.SpawnEntityFromPool("Fireworks3", transform.position, Quaternion.identity, transform.lossyScale * 0.075f, 30 * (Vector2.up * (0.4f - variance) + Vector2.left * 0.5f).normalized);
            _particleShooter.SpawnEntityFromPool("Fireworks4", transform.position, Quaternion.identity, transform.lossyScale * 0.125f, 28 * (Vector2.up * (0.4f + variance) + Vector2.right * 0.5f).normalized);
        }
        public void SpawnNoEntity(string objName)
        {
            _particleShooter.SpawnFromPool(objName, transform.position, Quaternion.identity, transform.lossyScale);
        }
        public void SpurtHitStarParticles(HitObject hit)
        {
            if (_particleShooter == null)
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
                    var particle = _particleShooter.SpawnEntityFromPool("HitStar", point, hit.box.transform.rotation, hit.box.transform.lossyScale);
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
        public void SpurtHitSparkleMiniParticles(HitObject hit)
        {
            if (_particleShooter == null)
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
                    var particle = _particleShooter.SpawnEntityFromPool("HitSparkle", point, hit.box.transform.rotation, hit.box.transform.lossyScale);
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
    }
    
    [System.Serializable]
    public class OnCollideCastEvent : UnityEvent<Collider2D> { }
    [System.Serializable]
    public class OnCollidePhysicalEvent : UnityEvent<ContactPoint2D[]> { }}
