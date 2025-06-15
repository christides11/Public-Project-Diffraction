namespace TightStuff
{
    using Game2DWaterKit;
    using System.Collections.Generic;
    using UnityEngine;
    using TightStuff.Aesthetics;
    using Extension;
    using static UnityEngine.ParticleSystem;
    
    public class AestheticsObserver : UpdateAbstract
    {
        private CamShaker _camShake;
        private Camera _cam;
        [SerializeField]
        private bool boxView;
    
        [SerializeField]
        private GameObject _boxGraphicHit;
        [SerializeField]
        private GameObject _circleGraphicHit;
        [SerializeField]
        private GameObject _boxGraphicGraze;
        [SerializeField]
        private GameObject _circleGraphicGraze;
        [SerializeField]
        private GameObject _boxGraphicHurt;
        [SerializeField]
        private GameObject _circleGraphicHurt;
        [SerializeField]
        private GameObject _boxGraphicHurtInvuln;
        [SerializeField]
        private GameObject _circleGraphicHurtInvuln;
        [SerializeField]
        private GameObject _boxGraphicHurtIntangible;
        [SerializeField]
        private GameObject _circleGraphicHurtIntangible;
        [SerializeField]
        private GameObject _boxGraphicShield;
        [SerializeField]
        private GameObject _circleGraphicShield;
    
    
        private Pooler _pooler;
        private MatchManager _matchManager;
    
        [SerializeField]
        private SpriteRenderer _backgroundDim;
    
        private List<GameObject> _boxDrawGraphics;
    
        [SerializeField]
        private InterpOutOfBW _deathPredictionEffect;
    
        private void Start()
        {
            _camShake = FindObjectOfType<CamShaker>();
            _cam = Camera.main;
    
            ActionSM.OnStateEnter += OnStateEnter;
            ActionSM.OnStateExit += OnStateExit;
            ActionSM.OnStateUpdate += OnStateUpdate;
            AirborneFighterHurtHeavyState.CrashEvent += OnCrash;
            AirborneFighterHurtHeavyState.TechEvent += OnTech;
    
            GrazePoint.FullMeter += OnFullMeter;
    
            Hitter.OnHitEvent += OnHitEvent;
            GrazeBox.OnGrazeCreate += GenerateGrazeEffect;
    
            InteractBox.OnBoxDraw += OnBoxDraw;
            Runner.OnFrameStart += DestroyAllBoxDraws;
    
            Game2DWaterKit.Ripples.WaterCollisionRipplesModule.OnWaterEnterExit.AddListener(OnWaterEnterExit);
            BaseFighterBehavior.SpellcardTimeStopEvent += DarkenScreen;
    
            _pooler = GetComponent<Pooler>();
            _matchManager = GetComponent<MatchManager>();
            _boxDrawGraphics = new List<GameObject>();
        }
    
        public override void GUpdate()
        {
            base.GUpdate();
            if (_matchManager.SpellcardDarken)
                _backgroundDim.color = Color.Lerp(_backgroundDim.color, new Color(0, 0, 0, 0.5f), Time.fixedDeltaTime * 10f);
            else
                _backgroundDim.color = Color.Lerp(_backgroundDim.color, new Color(0, 0, 0, 0f), Time.fixedDeltaTime * 10f);
        }
    
        private void OnDestroy()
        {
            ActionSM.OnStateEnter -= OnStateEnter;
            ActionSM.OnStateExit -= OnStateExit;
            ActionSM.OnStateUpdate -= OnStateUpdate;
            AirborneFighterHurtHeavyState.CrashEvent -= OnCrash;
            AirborneFighterHurtHeavyState.TechEvent -= OnTech;
    
            GrazePoint.FullMeter -= OnFullMeter;
    
            Hitter.OnHitEvent -= OnHitEvent;
            GrazeBox.OnGrazeCreate -= GenerateGrazeEffect;
    
            InteractBox.OnBoxDraw -= OnBoxDraw;
            Runner.OnFrameStart -= DestroyAllBoxDraws;
            BaseFighterBehavior.SpellcardTimeStopEvent -= DarkenScreen;
        }
    
        private void DarkenScreen()
        {
            if (MatchManager.worldTime <= 0)
                _matchManager.SpellcardDarken = true;
            else
                _matchManager.SpellcardDarken = false;
        }
    
        private void OnStateEnter(State s)
        {
            if (s is GroundedFighterRespawnLandState)
            {
                _camShake.ShakeCamera(40, 0.8f);
                SpawnObjectWithRandomFlippedX(s, "RespawnBlast");
                SpawnObjectWithRandomFlippedX(s, "Shockwave");
                SpawnWind((s as FighterState).fsm.entity.transform.position, "Wind");
            }
            if (s is GroundedFighterDeadState)
            {
                _camShake.ShakeCamera(40, 1.5f);
                SpawnWind((s as FighterState).fsm.entity.transform.position, "Wind");
    
                var point = (s as FighterState).fsm.entity.transform.position;
                var fighterScreenPos = Vector3.Scale(_cam.WorldToViewportPoint(point), new Vector3(1, 1, 0));
                fighterScreenPos -= new Vector3(0.5f, 0.5f, 0);
                fighterScreenPos.Normalize();
    
                var rot = Quaternion.identity;
                if (Mathf.Abs(fighterScreenPos.x) > 0.9f)
                {
                    rot = Quaternion.Euler(0, 0, 90 * Mathf.Sign(fighterScreenPos.x));
                }
                else if (Mathf.Abs(fighterScreenPos.x) > 0.6f)
                {
                    rot = Quaternion.Euler(0, 0, Mathf.Atan2(Mathf.Sign(fighterScreenPos.x), -Mathf.Sign(fighterScreenPos.y)) * Mathf.Rad2Deg);
                }
                else if (point.normalized.y > 0)
                {
                    rot = Quaternion.Euler(0, 0, 180);
                }
                var particle = _pooler.SpawnEntityFromPool("Droplet", point, Quaternion.identity, Vector3.one * 5);
                particle.stateVars.selfTime = 1f;
                particle.PlayParticles();
                var particle2 = _pooler.SpawnEntityFromPool("Droplet", point, Quaternion.identity, Vector3.one * 2.5f);
                particle2.stateVars.selfTime = 0.5f;
                particle2.PlayParticles();
                var particle3 = _pooler.SpawnEntityFromPool("DeathInvert", point, Quaternion.identity, Vector3.one);
                particle3.PlayParticles();
                var particle4 = _pooler.SpawnEntityFromPool("DeathShock", point, Quaternion.identity, Vector3.one * 3);
                particle4.PlayParticles();
                var particle5 = _pooler.SpawnEntityFromPool("DeathPetals", point, rot, Vector3.one * 1.5f);
                particle5.PlayParticles();
                var particle6 = _pooler.SpawnEntityFromPool("InvisibleBallShockwave", point, rot, Vector3.one, 30 * new Vector2(Mathf.Cos((rot.eulerAngles.z + 90) * Mathf.Deg2Rad), Mathf.Sin((rot.eulerAngles.z + 90) * Mathf.Deg2Rad)));
                var particle7 = _pooler.SpawnEntityFromPool("DeathSpirits", point, rot, Vector3.one * 1.5f);
                particle7.PlayParticles();
                var particle8 = _pooler.SpawnEntityFromPool("DeathSplash", point, rot, Vector3.one);
                particle8.PlayParticles();
                var sound = _pooler.SpawnFromPool("DeathSFX", point, Quaternion.identity, Vector3.one);
                _pooler.SpawnFromPool("DeathSplashSFX", point, Quaternion.identity, Vector3.one);
            }
            if (s is GroundedFighterPerfectShieldEndState)
            {
                var point = (s as FighterState).fsm.fighter.shield.transform.position;
                var particle = _pooler.SpawnEntityFromPool("Droplet", (s as FighterState).fsm.fighter.shield.transform.position, Quaternion.identity, Vector3.one * 1.5f);
                particle.stateVars.selfTime = 1f;
                particle.PlayParticles();
    
                var particle2 = _pooler.SpawnEntityFromPool("PerfectShieldRelease", point, Quaternion.identity, Vector3.one * 1.5f);
                particle2.PlayParticles();
                var sound = _pooler.SpawnFromPool("PerfectShieldReleaseSFX", point, Quaternion.identity, Vector3.one);
            }
            if (s is AirborneFighterAirdashState)
            {
                var transform = (s as FighterState).fsm.fighter.hurtbox[0];
                var obj = _pooler.GetOGPrefabInfo("Airdash");
                var ad = _pooler.SpawnEntityFromPool("Airdash", transform.position + Quaternion.AngleAxis(transform.eulerAngles.z, Vector3.forward) * obj.transform.position, transform.rotation, MatchManager.FrameNum % 2 == 0 ? new Vector3(1, 1, 1) : new Vector3(-1, 1, 1));
                var shockwave = SpawnObjectWithRandomFlippedX(s, "Shockwave");
                ad.stateVars.givenTime = (s as FighterState).fsm.entity.stateVars.givenTime;
                shockwave.stateVars.givenTime = (s as FighterState).fsm.entity.stateVars.givenTime;
                shockwave.transform.position = transform.position + Quaternion.AngleAxis(transform.eulerAngles.z, Vector3.forward) * new Vector2(0, -0.35f);
                shockwave.transform.rotation = transform.rotation;
                shockwave.transform.localScale = Vector3.one * 0.3f;
                if ((s as FighterState).fsm.fighter.extension is YuugiFighterExtension)
                {
                    var a= _pooler.SpawnFromPool("DoubleJump", transform.position, Quaternion.identity, Vector3.one);
                    GiveSoundFreezeEntity(s, a);
                    shockwave.transform.localScale = Vector3.one * 0.5f;
                }
                var sound = _pooler.SpawnFromPool("AirdashSFX", transform.position, Quaternion.identity, Vector3.one);
                GiveSoundFreezeEntity(s, sound);
            }
            if (s is AirborneFighterDoublejumpState)
            {
                var transform = (s as FighterState).fsm.fighter.hurtbox[0];
                var a = _pooler.SpawnFromPool("JumpSFX", transform.position, Quaternion.identity, Vector3.one);
    
                GiveSoundFreezeEntity(s, a);
            }
            if (s is GroundedFighterDashStartState)
            {
                var fighter = (s as FighterState).fsm.entity;
                var ad = _pooler.SpawnEntityFromPool("Dash", fighter.transform.position - Vector3.up * 0.95f, fighter.transform.rotation, Vector3.one * 0.75f);
                ad.transform.position -= 0.4f * (fighter.stateVars.flippedLeft ? Vector3.right : Vector3.left);
                ad.stateVars.flippedLeft = fighter.stateVars.flippedLeft;
                ad.stateVars.givenTime = (s as FighterState).fsm.entity.stateVars.givenTime;
                var sound = _pooler.SpawnFromPool("DashSFX", fighter.transform.position, Quaternion.identity, Vector3.one);
                GiveSoundFreezeEntity(s, sound);
            }
            if (s is GroundedFighterDashStopState)
            {
                var fighter = (s as FighterState).fsm.entity;
                var raycast = Physics2D.Raycast(fighter.transform.position, Vector2.down, 1, LayerMask.GetMask("Default", "Platform"));
                if (raycast.collider == null)
                    return;
                var ad = _pooler.SpawnEntityFromPool("DashStop", fighter.transform.position - Vector3.up * 0.8f, fighter.transform.rotation, Vector3.one);
                ad.transform.position -= 0.8f * (fighter.stateVars.flippedLeft ? Vector3.right : Vector3.left);
                ad.stateVars.flippedLeft = fighter.stateVars.flippedLeft;
                ad.stateVars.givenTime = (s as FighterState).fsm.entity.stateVars.givenTime;
                var sound = _pooler.SpawnFromPool("Brake", transform.position, Quaternion.identity, Vector3.one);
                GiveSoundFreezeEntity(s, sound);
            }
            if (s is GroundedFighterLandState)
            {
                var fighter = (s as FighterState).fsm.entity;
                var ad = _pooler.SpawnEntityFromPool("Land", fighter.transform.position - Vector3.up * 0.75f, fighter.transform.rotation, Vector3.one);
                ad.stateVars.givenTime = (s as FighterState).fsm.entity.stateVars.givenTime;
                ad.stateVars.flippedLeft = fighter.stateVars.flippedLeft;
            }
            if (s is GroundedFighterShieldBreakState)
            {
                var point = (s as FighterState).fsm.fighter.shield.transform.position;
                var particle1 = _pooler.SpawnEntityFromPool("ShieldBreak", point, Quaternion.identity, Vector3.one);
                _camShake.ShakeCamera(20, 0.5f);
                particle1.PlayParticles();
                SpawnWind((s as FighterState).fsm.entity.transform.position, "Wind");
                var particle = _pooler.SpawnEntityFromPool("Distortion", point, Quaternion.identity, Vector3.one);
                particle.PlayParticles();
                var particle2 = _pooler.SpawnEntityFromPool("HitCircular", point, Quaternion.identity, Vector3.one);
                particle2.PlayParticles();
                var particle3 = _pooler.SpawnEntityFromPool("ShieldShatter", point, Quaternion.identity, Vector3.one);
                particle3.PlayParticles();
            }
            if (s is GroundedFighterJumpState)
            {
                var point = (s as FighterState).fsm.fighter.shield.transform.position;
                var sound = _pooler.SpawnFromPool("JumpSFX", point, Quaternion.identity, Vector3.one);
                GiveSoundFreezeEntity(s, sound);
                if ((s as FighterState).fsm.fighter.extension is YuugiFighterExtension)
                {
                    var a = _pooler.SpawnFromPool("DoubleJump", point, Quaternion.identity, Vector3.one);
                    GiveSoundFreezeEntity(s, a);
                }
            }
            if (s is GroundedFighterLandState)
            {
                var point = (s as FighterState).fsm.fighter.shield.transform.position;
                if ((s as FighterState).fsm.fighter.extension is YuugiFighterExtension)
                {
                    var sound = _pooler.SpawnFromPool("LandHeavy", point, Quaternion.identity, Vector3.one);
                    GiveSoundFreezeEntity(s, sound);
                }
                else
                {
                    var sound = _pooler.SpawnFromPool("LandSFX", point, Quaternion.identity, Vector3.one);
                    GiveSoundFreezeEntity(s, sound);
                }
            }
            if (s is AirborneFighterHurtHeavyState)
            {
                SpawnLaunchSFX(s);
            }
            if (s is AirborneFighterFocusSpinState)
            {
                var point = (s as FighterState).fsm.fighter.shield.transform.position;
                var sound = _pooler.SpawnFromPool("Twirl", point, Quaternion.identity, Vector3.one);
                GiveSoundFreezeEntity(s, sound);
            }
            if (s is GroundedFighterPerfectShieldState)
            {
                var point = (s as FighterState).fsm.fighter.shield.transform.position;
                var sound = _pooler.SpawnFromPool("ShieldUp", point, Quaternion.identity, Vector3.one);
                GiveSoundFreezeEntity(s, sound);
            }
            if (s is AirborneFighterLedgeStartState)
            {
                var point = (s as FighterState).fsm.fighter.shield.transform.position;
                var sound = _pooler.SpawnFromPool("GrabSFX", point, Quaternion.identity, Vector3.one);
                GiveSoundFreezeEntity(s, sound);
            }
            if (s is GroundedFighterAttackThrowState)
            {
                var point = (s as FighterState).fsm.fighter.shield.transform.position;
                var sound = _pooler.SpawnFromPool("GrabSFX", point, Quaternion.identity, Vector3.one);
                GiveSoundFreezeEntity(s, sound);
            }
        }
    
        private static void GiveSoundFreezeEntity(State s, GameObject sound)
        {
            if (sound.TryGetComponent(out PauseAudioOnFreeze soundFreeze))
            {
                soundFreeze.entity = (s as FighterState).fsm.entity;
            }
        }
    
        private void OnStateUpdate(State s)
        {
            if (s is AirborneFighterHurtHeavyState && !(s is AirborneFighterHurtNormalState))
            {
                var fighter = (s as FighterState).fsm.fighter;
                if (fighter.entity.stateVars.frameNum == 80)
                {
                    var dir = (s as FighterState).fsm.entity.stateVars.indieSpd.normalized;
                    var shockwave = SpawnObjectWithRandomFlippedX(s, "Shockwave", Quaternion.Euler(0, 0, Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg + 90));
                    var size = Mathf.Clamp(fighter.stateVarsF.stunStopTime * 1.5f, 0.1f, 0.75f);
                    shockwave.transform.localScale = new Vector3(size, size * 1.5f, 1);
                }
            }
        }
    
        private void OnStateExit(State s)
        {
            if (s is AirborneFighterHurtFreezeState || s is GroundedFighterHurtFreezeState)
            {
                var fighter = (s as FighterState).fsm.fighter;
                fighter.hurtbox[1].transform.localPosition *= 0;
            }
        }
    
        private void OnCrash(Vector2 normal, Vector2 point, float spd)
        {
            var blast = _pooler.SpawnEntityFromPool(spd >= 50? "CrashUntechable" : "Crash", point, Quaternion.identity, Vector3.one * 0.8f);
            blast.transform.up = normal;
            _camShake.ShakeCamera(Mathf.Clamp(spd / 2, 1, 40), Mathf.Clamp(spd / 100, 0.1f, 0.3f));
            var sound = _pooler.SpawnFromPool("KnockbackBounce", point, Quaternion.identity, Vector3.one);
        }
        private void OnTech(Vector2 normal, Vector2 point, float spd)
        {
            var blast = _pooler.SpawnEntityFromPool("HitCircular", point, Quaternion.identity, Vector3.one * 0.25f);
            var sound = _pooler.SpawnFromPool("GrabSFX", point, Quaternion.identity, Vector3.one);
        }
    
        private void OnHitEvent(HitObject hit)
        {
            if (hit.hitbox is GrazeBox)
                return;
            HitFlash(hit);
            ShakeCamera(hit);
            SpurtHitDustParticles(hit);
            SpurtHitCircleParticles(hit);
            SpurtHitShieldParticles(hit);
            SpurtHitDistortParticles(hit);
            SpurtHitDropletParticles(hit);
            SpurtHitInkParticles(hit);
            ShieldImpactGroundShockwave(hit);
            SpawnLaunchWind(hit);
            InvulnHitSFX(hit);
        }
    
        private void SpawnLaunchSFX(State s)
        {
            var entity = (s as FighterState).fsm.fighter.entity;
            var point = entity.transform.position;
            if ((s as FighterState).fsm.entity.stateVars.indieSpd.magnitude > 50)
            {
                var sound = _pooler.SpawnFromPool("LaunchMedium", point, Quaternion.identity, Vector3.one);
                if (sound.TryGetComponent(out AudioSource audio))
                {
                    audio.pitch = 1.25f - Mathf.Clamp((entity.stateVars.indieSpd.magnitude - 50) / 40, 0, 0.5f);
                }
            }
            else if ((s as FighterState).fsm.entity.stateVars.indieSpd.magnitude > 15)
            {
                var sound = _pooler.SpawnFromPool("LaunchLight", point, Quaternion.identity, Vector3.one);
                if (sound.TryGetComponent(out AudioSource audio))
                {
                    audio.pitch = 1.5f - Mathf.Clamp((entity.stateVars.indieSpd.magnitude - 15) / 70, 0, 0.5f);
                }
            }
        }
    
        private void SpawnLaunchWind(HitObject hit)
        {
            if (hit.hitbox.hitProperties.knockback > 10)
                SpawnWind(hit.box.transform.position, "WindMedium");
            else if (hit.hitbox.hitProperties.knockback > 3)
                SpawnWind(hit.box.transform.position, "WindSmall");
        }
    
        private void OnWaterEnterExit(Game2DWater waterObj, Collider2D col, bool isEnteringWater)
        {
            if (col.attachedRigidbody == null)
                return;
            var posViewport = Vector3.Scale(_cam.WorldToViewportPoint(col.transform.position), new Vector3(1, 1, 0)) - new Vector3(1, 1, 0) * 0.5f;
            if (!col.attachedRigidbody.TryGetComponent(out BaseFighterBehavior fighter) && Mathf.Abs(posViewport.magnitude) > 0.5f)
                return;
    
            var size = 1f;
            var rad = (col as CircleCollider2D).radius;
            if (col is CircleCollider2D)
                size = (rad + 0.2f) * 2;
            Entity splash;
            if (isEnteringWater)
            {
                splash = _pooler.SpawnEntityFromPool("Splash", col.transform.position - rad * 2 * waterObj.MainModule.UpDirection, Quaternion.Euler(waterObj.transform.eulerAngles), new Vector3(size, size, 1));
                if (fighter != null)
                    _pooler.SpawnFromPool("WaterEnterHeavy", col.transform.position, Quaternion.identity, Vector3.one);
                else
                    _pooler.SpawnFromPool("WaterEnterLight", col.transform.position, Quaternion.identity, Vector3.one);
            }
            else
            {
                splash = _pooler.SpawnEntityFromPool("SplashOut", col.transform.position - rad * 2 * waterObj.MainModule.UpDirection, Quaternion.Euler(waterObj.transform.eulerAngles), new Vector3(size, size, 1));
                _pooler.SpawnFromPool("WaterResurface", col.transform.position, Quaternion.identity, Vector3.one);
            }
    
            if (splash.particles.Count >= 1)
            {
                if (splash.stateVars.damageArmor == 0)
                    splash.stateVars.damageArmor = splash.particles[0].main.startSpeedMultiplier;
                var main = splash.particles[0].main;
                main.startSpeedMultiplier = Mathf.Clamp(splash.stateVars.damageArmor * col.attachedRigidbody.velocity.magnitude / 2, 3, 20f);
                if (!isEnteringWater)
                    main.startSpeedMultiplier = Mathf.Clamp(splash.stateVars.damageArmor * col.attachedRigidbody.velocity.magnitude / 5, 3, 20f);
            }
    
            if (splash.particles.Count >= 2)
            {
                if (splash.stateVars.knockbackArmor == 0)
                    splash.stateVars.damageArmor = splash.particles[1].main.startSpeedMultiplier;
                var main2 = splash.particles[1].main;
                main2.startSpeedMultiplier = Mathf.Clamp(splash.stateVars.knockbackArmor * col.attachedRigidbody.velocity.magnitude / 2, 3, 20f);
            }
            if (splash.particles.Count >= 3)
            {
                if (splash.stateVars.genericTimer == 0)
                    splash.stateVars.genericTimer = splash.particles[1].main.startSpeedMultiplier;
                var main2 = splash.particles[1].main;
                main2.startSpeedMultiplier = Mathf.Clamp(splash.stateVars.genericTimer * col.attachedRigidbody.velocity.magnitude / 2, 3, 20f);
            }
    
            splash.PlayParticles();
        }
    
        private void ShieldImpactGroundShockwave(HitObject hit)
        {
            if (hit.box.owner.stateVars.aerial)
                return;
            var launchDir = hit.box.entity.stateVars.lastLaunchSpd.normalized;
            if (hit.box is ShieldBox && hit.hitbox.hitProperties.knockback > 5)
            {
                if (hit.box.owner.TryGetComponent(out BaseFighterBehavior fighter))
                {
                    float size = Mathf.Clamp(Mathf.Pow(hit.hitbox.hitProperties.knockback, 2) * 0.002f + 0.5f, 0.5f, 1.5f);
                    if (Mathf.Abs(launchDir.x) < 0.5f)
                    {
                        var shock = _pooler.SpawnEntityFromPool("ShieldShock", fighter.transform.position - Vector3.up * 1.1f, fighter.transform.rotation, size * fighter.transform.lossyScale);
                    }
                    else
                    {
                        var ad = _pooler.SpawnEntityFromPool("Dash", fighter.transform.position - Vector3.up * 0.95f, fighter.transform.rotation, size * fighter.transform.lossyScale);
                        var left = launchDir.x > 0;
                        ad.transform.position -= 0.4f * (left ? Vector3.right : Vector3.left);
                        ad.stateVars.flippedLeft = left;
                    }
                }
            }
        }
    
        private void HitFlash(HitObject hit)
        {
            if (hit.box.entity.stateVars.lastLaunchSpd.magnitude > 50)
                return;
            
            if (hit.box.entity.stateVars.freezeTime > 0 && !hit.box.entity.stateVars.invulnerable)
            {
                if (hit.box.entity.AssociatedRenderers.Count <= 0)
                    return;
                if (!(hit.box.entity.AssociatedRenderers[0] is SpriteRenderer))
                    return;
                var refImage = hit.box.entity.AssociatedRenderers[0] as SpriteRenderer;
                var flash = _pooler.SpawnEntityFromPool("HitFlash", refImage.transform.position, refImage.transform.rotation, refImage.transform.lossyScale);
                if (!(flash.AssociatedRenderers[0] is SpriteRenderer))
                    return;
                var flashSprite = flash.AssociatedRenderers[0] as SpriteRenderer;
                flash.transform.SetParent(refImage.transform);
                flash.stateVars.freezeTime = Time.fixedDeltaTime * 2;
                flashSprite.sprite = refImage.sprite;
                flashSprite.sortingLayerID = refImage.sortingLayerID;
                flashSprite.sortingOrder = refImage.sortingOrder + 1;
            }
        }
    
        private void SpurtHitDustParticles(HitObject hit)
        {
            var perfect = false;
            if (hit.box is ShieldBox)
                perfect = hit.box.owner.actionState.GetState() is GroundedFighterPerfectShieldState || hit.box.owner.actionState.GetState() is GroundedFighterPerfectShieldEndState;
    
            var trueDmg = Mathf.Clamp(hit.hitbox.hitProperties.damage, 0, hit.box.entity.et.health);
            if (trueDmg > 0)
            {
                for (int i = 0; i < Mathf.Clamp(Mathf.CeilToInt(trueDmg / 4), 0, 4); i++)
                {
                    var point = hit.hitCol.ClosestPoint((Vector2)hit.hitBy.transform.position + hit.hitBy.offset);
                    var particle = _pooler.SpawnEntityFromPool("HitDust", point, hit.box.transform.rotation, hit.box.transform.lossyScale);
                    var angle = Mathf.Atan2(hit.box.entity.stateVars.lastLaunchSpd.normalized.y, hit.box.entity.stateVars.lastLaunchSpd.normalized.x) * Mathf.Rad2Deg;
    
                    if (hit.hitbox.hitProperties.knockback <= 2 || perfect)
                        angle = Random.Range(0, 360);
    
                    var rad = angle * Mathf.Deg2Rad;
    
                    particle.transform.up = new Vector3(-Mathf.Sin(rad), Mathf.Cos(rad), 0).normalized;
                    particle.transform.localScale = Vector3.one * Mathf.Clamp(Mathf.Pow(trueDmg, 2) * 0.005f + 0.75f, 0.75f, 3);
                    if (perfect)
                    {
                        particle.transform.position = hit.box.entity.transform.position;
                        particle.transform.localScale *= 2;
                    }
                    particle.PlayParticles();
                    particle.stateVars.givenTime = 1;
                }
            }
        }
        private void SpurtHitInkParticles(HitObject hit)
        {
            if (hit.box.entity.stateVars.invulnerable || hit.box is ShieldBox || hit.box is HitBox)
                return;
            if (hit.box.entity.gameObject.CompareTag("Floating"))
                return;
            if (hit.box.entity.stateVars.lastLaunchSpd.magnitude > 15)
            {
                var randInt = Random.Range(1, 3);
                var randInt2 = Random.Range(1, 3);
                var point = hit.hitCol.ClosestPoint((Vector2)hit.hitBy.transform.position + hit.hitBy.offset);
                var particle = _pooler.SpawnEntityFromPool("Ink" + randInt, point, hit.box.transform.rotation, hit.box.transform.lossyScale);
                var particle2 = _pooler.SpawnEntityFromPool("Ink" + randInt2, point, hit.box.transform.rotation, hit.box.transform.lossyScale);
                var particle3 = _pooler.SpawnEntityFromPool("InkRound", point, hit.box.transform.rotation, hit.box.transform.lossyScale);
                var angle = Mathf.Atan2(hit.box.entity.stateVars.lastLaunchSpd.normalized.y, hit.box.entity.stateVars.lastLaunchSpd.normalized.x) * Mathf.Rad2Deg + 90;
    
                var rad = (angle + 30) * Mathf.Deg2Rad;
                var rad2 = (angle - 30) * Mathf.Deg2Rad;
    
                particle.transform.up = new Vector3(-Mathf.Sin(rad), Mathf.Cos(rad), 0).normalized;
                particle.transform.localScale = Vector3.one * Mathf.Clamp(hit.box.entity.stateVars.lastLaunchSpd.magnitude * 0.01f, 0.25f, 1);
                particle2.transform.up = new Vector3(-Mathf.Sin(rad2), Mathf.Cos(rad2), 0).normalized;
                particle2.transform.localScale = Vector3.one * Mathf.Clamp(hit.box.entity.stateVars.lastLaunchSpd.magnitude * 0.0075f, 0.25f, 1);
                particle3.transform.up = new Vector3(-Mathf.Sin(rad2), Mathf.Cos(rad2), 0).normalized;
                particle3.transform.localScale = Vector3.one * Mathf.Clamp(hit.box.entity.stateVars.lastLaunchSpd.magnitude * 0.01f, 0.25f, 1);
    
                particle.stateVars.givenTime = 1;
                particle2.stateVars.givenTime = 1;
                particle3.stateVars.givenTime = 1;
    
                if (hit.box.entity.stateVars.lastLaunchSpd.magnitude > 50)
                {
                    var randInt3 = Random.Range(1, 3);
                    var particle4 = _pooler.SpawnEntityFromPool("Ink" + randInt3, point, hit.box.transform.rotation, hit.box.transform.lossyScale);
                    var rad3 = (angle - 180) * Mathf.Deg2Rad;
                    particle4.transform.up = new Vector3(-Mathf.Sin(rad3), Mathf.Cos(rad3), 0).normalized;
                    particle4.transform.localScale = Vector3.one * Mathf.Clamp(hit.box.entity.stateVars.lastLaunchSpd.magnitude * 0.0125f, 0.25f, 1);
                    particle4.stateVars.givenTime = 1;
                    if (_matchManager.DeathFreezeCooldown <= 0)//For death prediction purposes in the future
                    {
                        _matchManager.DeathFreezeDuration = 0.5f;
                        _matchManager.DeathFreezeCooldown = 2f;
                        _deathPredictionEffect.StartBW();
                        _deathPredictionEffect.transform.position = point;
                        var sound = _pooler.SpawnFromPool("DeathPrediction", point, Quaternion.identity, Vector2.one);
    
                    }
                    (particle.AssociatedRenderers[0] as SpriteRenderer).color = Color.red;
                    (particle2.AssociatedRenderers[0] as SpriteRenderer).color = Color.red;
                    (particle3.AssociatedRenderers[0] as SpriteRenderer).color = Color.red;
                    (particle4.AssociatedRenderers[0] as SpriteRenderer).color = Color.red;
                }
            }
        }
    
        private void SpurtHitCircleParticles(HitObject hit)
        {
            if (hit.box.entity.stateVars.invulnerable || hit.box is ShieldBox)
                return;
            if (hit.box.entity.gameObject.CompareTag("Floating"))
                return;
            var trueDmg = Mathf.Clamp(hit.hitbox.hitProperties.damage, 0, hit.box.entity.et.health);
            if (trueDmg > 4)
            {
                var point = hit.hitCol.ClosestPoint((Vector2)hit.hitBy.transform.position + hit.hitBy.offset);
                var particle = _pooler.SpawnEntityFromPool("HitCircular", point, Quaternion.identity, hit.box.transform.lossyScale);
    
                particle.transform.localScale = Vector3.one * Mathf.Clamp(Mathf.Pow(trueDmg, 2) * 0.004f + 0.25f, 0.25f, 2f);
                particle.stateVars.selfTime = 1f;
                particle.stateVars.givenTime = 1;
                particle.PlayParticles();
            }
        }
        private void SpurtHitDistortParticles(HitObject hit)
        {
            if (hit.box.entity.stateVars.invulnerable || hit.box is ShieldBox)
                return;
            if (hit.box.entity.gameObject.CompareTag("Floating"))
                return;
            var trueDmg = Mathf.Clamp(hit.hitbox.hitProperties.damage, 0, hit.box.entity.et.health);
            if (trueDmg > 10)
            {
                var point = hit.hitCol.ClosestPoint((Vector2)hit.hitBy.transform.position + hit.hitBy.offset);
                var particle = _pooler.SpawnEntityFromPool("Distortion", point, Quaternion.identity, hit.box.transform.lossyScale);
    
                particle.transform.localScale = Vector3.one * Mathf.Clamp(Mathf.Pow(trueDmg, 2) * 0.01f, 0.5f, 5);
                particle.stateVars.givenTime = 1;
                particle.PlayParticles();
            }
        }
        private void InvulnHitSFX(HitObject hit)
        {
            if (hit.box.entity.gameObject.CompareTag("Floating"))
                return;
            var trueDmg = Mathf.Clamp(hit.hitbox.hitProperties.damage, 0, hit.box.entity.et.health);
            if (hit.box.entity.stateVars.invulnerable && !(hit.box is ShieldBox) && trueDmg > 0)
            {
                var point = hit.hitCol.transform.position;
                var sound = _pooler.SpawnFromPool("InvulnHit", point, Quaternion.identity, Vector2.one);
                if (sound.TryGetComponent(out AudioSource audio))
                {
                    audio.pitch = 1.25f - Mathf.Clamp(hit.box.entity.stateVars.freezeTime / 2, 0, 0.5f);
                }
            }
        }
        private void SpurtHitDropletParticles(HitObject hit)
        {
            var perfect = false;
            if (hit.box is ShieldBox)
                perfect = hit.box.owner.actionState.GetState() is GroundedFighterPerfectShieldState || hit.box.owner.actionState.GetState() is GroundedFighterPerfectShieldEndState;
            if (perfect)
            {
                var point = hit.hitCol.transform.position;
                var particle = _pooler.SpawnEntityFromPool("PerfectShield", point, Quaternion.identity, Vector2.one * 0.75f);
                particle.stateVars.givenTime = 1;
                particle.stateVars.selfTime = 1f;
                var sound = _pooler.SpawnFromPool("PerfectShieldImpactSFX", point, Quaternion.identity, Vector2.one * 0.5f);
                if (sound.TryGetComponent(out AudioSource audio))
                {
                    audio.pitch = 1.25f - Mathf.Clamp(hit.box.owner.stateVars.freezeTime / 2, 0, 0.5f);
                }
            }
        }
        private void SpurtHitShieldParticles(HitObject hit)
        {
            if (!(hit.box is ShieldBox))
                return;
            var trueDmg = Mathf.Clamp(hit.hitbox.hitProperties.damage, 0, hit.box.entity.et.health);
            if (trueDmg > 0)
            {
                var point = (Vector2)hit.hitCol.transform.position + hit.hitCol.offset * new Vector2(hit.box.owner.stateVars.flippedLeft ? -1 : 1, 1);
                var particle = _pooler.SpawnEntityFromPool("HitShield", point, Quaternion.identity, hit.box.transform.lossyScale);
                var angle = point - (Vector2)hit.hitBy.transform.position - hit.hitBy.offset;
    
                particle.transform.GetChild(0).localScale = Vector3.Scale(new Vector3(0.2f, 1, 1), Vector3.one) * Mathf.Clamp(Mathf.Pow(trueDmg, 2) * 0.0015f + 0.4f, 0.4f, 1f);
                particle.transform.up = -Vector2.Perpendicular(angle.normalized);
                particle.stateVars.givenTime = 1;
                particle.PlayParticles();
    
                var sound = _pooler.SpawnFromPool("ShieldImpactSFX", point, Quaternion.identity, Vector2.one);
                if (sound.TryGetComponent(out AudioSource audio))
                {
                    audio.pitch = 1.25f - Mathf.Clamp(trueDmg * 0.025f, 0, 0.5f);
                }
            }
        }
    
        private void ShakeCamera(HitObject hit)
        {
            if (hit.box is ShieldBox)
                if (hit.box.owner.TryGetComponent(out BaseFighterBehavior fighter))
                    if (fighter.ActionState.GetState() is GroundedFighterPerfectShieldState || fighter.ActionState.GetState() is GroundedFighterPerfectShieldEndState)
                        return;
            var trueDmg = Mathf.Clamp(hit.hitbox.hitProperties.damage, 0, hit.box.entity.et.health);
            _camShake.ShakeCamera(Mathf.Clamp((trueDmg - 5f) * (trueDmg - 5f) * 0.25f, 0, 30), Mathf.Clamp((trueDmg - 5f) / 25f, 0, 1f));
        }
    
        private Entity SpawnObjectWithRandomFlippedX(State s, string tag, Quaternion quaternion = default(Quaternion))
        {
            var obj = _pooler.GetOGPrefabInfo(tag);
            var pooled = _pooler.SpawnEntityFromPool(tag, (s as FighterState).fsm.entity.transform.position + obj.transform.position, quaternion, Vector3.Scale(obj.transform.localScale, Vector3.one));
            pooled.stateVars.flippedLeft = MatchManager.FrameNum % 2 == 0;
            return pooled;
        }
        private void SpawnWind(Vector3 pos, string tag)
        {
            var obj = _pooler.GetOGPrefabInfo(tag);
            _pooler.SpawnFromPool(tag, pos + obj.transform.position, Quaternion.identity, obj.transform.localScale);
        }
    
        private void OnBoxDraw(BoxDraw b)
        {
            if (!boxView)
                return;
    
            if (b.boxShape == BoxDraw.BoxShape.circle)
            {
                var circleView = InstantiateCircleGraphic();
                circleView.transform.localScale = Vector2.one * b.radius;
                _boxDrawGraphics.Add(circleView);
            }
            if (b.boxShape == BoxDraw.BoxShape.box)
            {
                var boxView = InstantiateBoxGraphic();
                boxView.transform.localScale = b.scale;
                boxView.transform.eulerAngles = new Vector3(0, 0, b.angle);
                _boxDrawGraphics.Add(boxView);
            }
    
            GameObject InstantiateBoxGraphic()
            {
                if (b.box is HitBox && !(b.box is GrazeBox))
                {
                    return Instantiate(_boxGraphicHit, b.center, Quaternion.Euler(0, 0, transform.eulerAngles.z));
                }
                if (b.box is GrazeBox)
                {
                    return Instantiate(_boxGraphicGraze, b.center, Quaternion.Euler(0, 0, transform.eulerAngles.z));
                }
                if (b.box is HurtBox)
                {
                    if (b.box.entity.stateVars.intangible)
                        return Instantiate(_boxGraphicHurtIntangible, b.center, Quaternion.Euler(0, 0, transform.eulerAngles.z));
    
                    if (b.box.entity.stateVars.invulnerable)
                        return Instantiate(_boxGraphicHurtInvuln, b.center, Quaternion.Euler(0, 0, transform.eulerAngles.z));
    
                    return Instantiate(_boxGraphicHurt, b.center, Quaternion.Euler(0, 0, transform.eulerAngles.z));
                }
                if (b.box is ShieldBox)
                {
                    return Instantiate(_boxGraphicShield, b.center, Quaternion.Euler(0, 0, transform.eulerAngles.z));
                }
                return null;
            }
            GameObject InstantiateCircleGraphic()
            {
                if (b.box is HitBox && !(b.box is GrazeBox))
                {
                    return Instantiate(_circleGraphicHit, b.center, Quaternion.Euler(0, 0, transform.eulerAngles.z));
                }
                if (b.box is GrazeBox)
                {
                    return Instantiate(_circleGraphicGraze, b.center, Quaternion.Euler(0, 0, transform.eulerAngles.z));
                }
                if (b.box is HurtBox)
                {
                    if (b.box.entity.stateVars.intangible)
                        return Instantiate(_circleGraphicHurtIntangible, b.center, Quaternion.Euler(0, 0, transform.eulerAngles.z));
    
                    if (b.box.entity.stateVars.invulnerable)
                        return Instantiate(_circleGraphicHurtInvuln, b.center, Quaternion.Euler(0, 0, transform.eulerAngles.z));
    
                    return Instantiate(_circleGraphicHurt, b.center, Quaternion.Euler(0, 0, transform.eulerAngles.z));
                }
                if (b.box is ShieldBox)
                {
                    return Instantiate(_circleGraphicShield, b.center, Quaternion.Euler(0, 0, transform.eulerAngles.z));
                }
                return null;
            }
        }
    
        private void GenerateGrazeEffect(HitObject gz, ref GrazePoint gzpoint)
        {
            Entity gzFX = null;
            if (!gz.box.entity.TryGetComponent(out BaseFighterBehavior fighter))
                return;
            if (fighter.stateVarsF.spellcardState || fighter.stateVarsF.grazeMeter >= 30)
                return;
    
            var point = gz.hitCol.ClosestPoint((Vector2)gz.hitBy.transform.position + gz.hitBy.offset);
            var dir = (point - (Vector2)gz.box.entity.transform.position).normalized;
    
            for (int i = 0; i < GrazeBox.CalculateGraze(gz.hitbox.hitProperties.damage); i++)
            {
                if (gz.hitbox.hitProperties.damage > 0)
                {
                    gzFX = _pooler.SpawnEntityFromPool("Graze", (point + (Vector2)gz.hitBy.bounds.ClosestPoint(point)) / 2, Quaternion.identity, Vector3.one * 0.4f, (Vector2.Perpendicular(dir) * UnityEngine.Random.Range(-1.0f, 1.0f) + dir * 2).normalized * 50f);
                }
                if (gzFX.TryGetComponent(out GrazeParticle gzProj))
                {
                    gzProj.ResetHomeTarget();
                    gzpoint.gzFX.Add(gzProj);
                }
            }
        }
    
        private void DestroyAllBoxDraws()
        {
            foreach(var draws in _boxDrawGraphics)
                Destroy(draws);
            _boxDrawGraphics.Clear();
        }
    
        private void OnFullMeter(BaseFighterBehavior fighter)
        {
            var point = fighter.transform.position;
            var particle2 = _pooler.SpawnEntityFromPool("HitCircular", point, Quaternion.identity, Vector3.one);
            particle2.PlayParticles();
            var sound = _pooler.SpawnFromPool("SpellcardGet", point, Quaternion.identity, Vector2.one);
        }
    }
}
