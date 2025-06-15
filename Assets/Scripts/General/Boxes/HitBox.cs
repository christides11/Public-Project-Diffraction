namespace TightStuff
{
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Events;
    
    public class HitBox : InteractBox
    {
        public float FreezeMultiplier { get => hitProperties.freezeMultiplier; set => hitProperties.freezeMultiplier = value; }
        public float ShieldFreezeMultiplier { get => hitProperties.shieldFreezeMultiplier; set => hitProperties.shieldFreezeMultiplier = value; }
        public float DamageValue { get => hitProperties.damage; set => hitProperties.damage = value; }
        public float Knockback { get => hitProperties.knockback; set => hitProperties.knockback = value; }
        public float KnockbackGrowth { get => hitProperties.knockbackGrowth; set => hitProperties.knockbackGrowth = value; }
        public float HitCoolDown { get => hitProperties.hitCoolDown; set => hitProperties.hitCoolDown = value; }
        public float KnockbackDir { get => hitProperties.knockbackDir; set => hitProperties.knockbackDir = value; }
    
        public OnHitEvent OnHit;
    
        public HitProperties hitProperties;
    
        protected override void Start()
        {
            base.Start();
            hitProperties.owner = owner;
        }
    
        public override void GUpdate()
        {
            base.GUpdate();
            owner = hitProperties.owner;
        }
        public override void LateGUpdate()
        {
            base.LateGUpdate();
            if (hitProperties.grabTarget != null)
            {
                hitProperties.grabTarget.transform.position = entity.transform.position + (Vector3)(_colliders[0].offset * (Vector2)_colliders[0].transform.lossyScale);
                HurtStunEntity(hitProperties.grabTarget);
                hitProperties.grabTarget.stateVars.freezeTime = Time.fixedDeltaTime; 
                hitProperties.grabTarget.stateVars.flippedLeft = !entity.stateVars.flippedLeft;
    
                if (!_colliders[0].enabled || hitProperties.grabTarget.stateVars.indieSpd.magnitude > 10)
                {
                    if (entity.TryGetComponent(out BaseFighterBehavior fighter))
                    {
                        if (hitProperties.grabTarget.stateVars.indieSpd.magnitude > 10)
                            fighter.entity.actionState.SetState(new GroundedFighterThrowTechState(fighter.entity.actionState as FighterSM));
                    }
                    else if (hitProperties.grabTarget.stateVars.indieSpd.magnitude <= 10)
                        hitProperties.grabTarget.stateVars.indieSpd = SidedKnockback(entity.stateVars.flippedLeft).normalized * hitProperties.knockback;
                    hitProperties.grabTarget = null;
                    return;
                }
                hitProperties.grabTarget.stateVars.indieSpd = entity.stateVars.indieSpd;
            }
        }
    
        public virtual float AdditionalRange()
        {
            return 0;
        }
    
        #region BasicHitProperties
        public void Damage(HitObject hit)
        {
            if (hit.box.entity.stateVars.invulnerable)
                return;
            if (hit.box is HurtBox)
                hit.box.entity.stateVars.percent += hitProperties.damage;
            if (hit.box is ShieldBox)
            {
                if (hit.box.owner.TryGetComponent(out BaseFighterBehavior fighter))
                    if (fighter.ActionState.GetState() is GroundedFighterPerfectShieldState || fighter.ActionState.GetState() is GroundedFighterPerfectShieldEndState)
                    {
                        if (hit.hitbox.owner == hit.box.owner)
                            return;
                        if (fighter.stateVarsF.grazeMeter + GrazeBox.CalculateGraze(hitProperties.damage) >= 30 && fighter.stateVarsF.grazeMeter < 30 && GrazePoint.FullMeter != null)
                            GrazePoint.FullMeter.Invoke(fighter);
                        fighter.stateVarsF.grazeMeter += GrazeBox.CalculateGraze(hitProperties.damage);
                        fighter.PlayGrazeOutlineAnimation();
                        return;
                    }
                hit.box.entity.stateVars.percent += hitProperties.damage + hitProperties.shieldDamage;
            }
        }
        public void DamageProjectile(HitObject hit)
        {
            if (hit.box.entity.stateVars.invulnerable)
                return;
            if (!hit.box.entity.TryGetComponent(out BaseProjectileBehaviour _))
                return;
            if (hit.box is HurtBox)
                hit.box.entity.stateVars.percent += 5;
        }

        public void DamageSpeedBased(HitObject hit)
        {
            if (hit.box.entity.stateVars.invulnerable)
                return;
            if (hit.box is HurtBox)
                hit.box.entity.stateVars.percent += hitProperties.damage + ((int)entity.stateVars.indieSpd.magnitude);
            if (hit.box is ShieldBox)
            {
                if (hit.box.owner.TryGetComponent(out BaseFighterBehavior fighter))
                    if (fighter.ActionState.GetState() is GroundedFighterPerfectShieldState || fighter.ActionState.GetState() is GroundedFighterPerfectShieldEndState)
                    {
                        if (hit.hitbox.owner == hit.box.owner)
                            return;
                        if (fighter.stateVarsF.grazeMeter + GrazeBox.CalculateGraze(hitProperties.damage) >= 30 && fighter.stateVarsF.grazeMeter < 30 && GrazePoint.FullMeter != null)
                            GrazePoint.FullMeter.Invoke(fighter);
                        fighter.stateVarsF.grazeMeter += GrazeBox.CalculateGraze(hitProperties.damage);
                        fighter.PlayGrazeOutlineAnimation();
                        return;
                    }
                hit.box.entity.stateVars.percent += hitProperties.damage + hitProperties.shieldDamage;
            }
        }
    
        public void Freeze(HitObject hit)
        {
            hit.box.entity.stateVars.freezeTime = Mathf.Clamp(CalculateFreeze(hitProperties.damage), hit.box.entity.stateVars.freezeTime, 5);
            if (hit.box is HitBox)
                entity.stateVars.freezeTime = Mathf.Clamp((hit.box as HitBox).CalculateFreeze((hit.box as HitBox).hitProperties.damage), entity.stateVars.freezeTime, 5);
            else
                entity.stateVars.freezeTime = Mathf.Clamp(CalculateSelfFreeze(Mathf.Clamp(hitProperties.damage, 0, hit.box.entity.et.health)), entity.stateVars.freezeTime, 5);

            if (hit.box is ShieldBox)
            {
                hit.box.entity.stateVars.freezeTime *= hitProperties.shieldFreezeMultiplier;
                DetectPerfectShield(hit);
            }

            void DetectPerfectShield(HitObject hit)
            {
                if (hit.box.owner.TryGetComponent(out BaseFighterBehavior fighter))
                {
                    if (fighter.entity.actionState.GetState() is GroundedFighterPerfectShieldState)
                    {
                        entity.stateVars.freezeTime += Time.fixedDeltaTime * 3f;
                        hit.box.entity.stateVars.freezeTime = Mathf.Clamp(hit.box.entity.stateVars.freezeTime, Time.fixedDeltaTime * 3, Time.fixedDeltaTime * 8);
                        Debug.Log(hit.box.entity.stateVars.freezeTime);
                    }

                }
            }
        }

        public float CalculateFreeze(float val)
        {
            return Mathf.Sqrt(val) * hitProperties.freezeMultiplier * 0.05f;
        }
        public float CalculateSelfFreeze(float val)
        {
            return Mathf.Sqrt(val) * 0.05f;
        }

        public void FreezeSpeedBased(HitObject hit)
        {
            hit.box.entity.stateVars.freezeTime = Mathf.Clamp(Mathf.Sqrt(hitProperties.damage + ((int)entity.stateVars.indieSpd.magnitude)) * hitProperties.freezeMultiplier * 0.05f, hit.box.entity.stateVars.freezeTime, 5);


            if (hit.box is HitBox)
                entity.stateVars.freezeTime = Mathf.Clamp((hit.box as HitBox).CalculateFreeze((hit.box as HitBox).hitProperties.damage), entity.stateVars.freezeTime, 5);
            else
                entity.stateVars.freezeTime = Mathf.Clamp(CalculateSelfFreeze(Mathf.Clamp(hitProperties.damage, 0, hit.box.entity.et.health) + ((int)entity.stateVars.indieSpd.magnitude)), entity.stateVars.freezeTime, 5);
    
            if (hit.box is ShieldBox)
            {
                hit.box.entity.stateVars.freezeTime *= hitProperties.shieldFreezeMultiplier;
                DetectPerfectShield(hit);
            }
    
            void DetectPerfectShield(HitObject hit)
            {
                if (hit.box.owner.TryGetComponent(out BaseFighterBehavior fighter))
                {
                    if (fighter.entity.actionState.GetState() is GroundedFighterPerfectShieldState)
                    {
                        entity.stateVars.freezeTime += Time.fixedDeltaTime * 3f;
                        hit.box.entity.stateVars.freezeTime = Mathf.Clamp(hit.box.entity.stateVars.freezeTime, Time.fixedDeltaTime * 3, Time.fixedDeltaTime * 8);
                        Debug.Log(hit.box.entity.stateVars.freezeTime);
                    }
    
                }
            }
        }
        public void FreezeAccumulate(HitObject hit)
        {
            hit.box.entity.stateVars.freezeTime += Mathf.Sqrt(hitProperties.damage) * hitProperties.freezeMultiplier * 0.05f;
            if (hit.box is HitBox)
                entity.stateVars.freezeTime = Mathf.Clamp((hit.box as HitBox).CalculateFreeze((hit.box as HitBox).hitProperties.damage), entity.stateVars.freezeTime, 5);
            else
                entity.stateVars.freezeTime += Mathf.Clamp(CalculateSelfFreeze(Mathf.Clamp(hitProperties.damage, 0, hit.box.entity.et.health)), 0, hit.box.entity.et.health);

            if (hit.box is ShieldBox)
            {
                hit.box.entity.stateVars.freezeTime *= hitProperties.shieldFreezeMultiplier;
                DetectPerfectShield(hit);
            }
    
            void DetectPerfectShield(HitObject hit)
            {
                if (hit.box.owner.TryGetComponent(out BaseFighterBehavior fighter))
                {
                    if (fighter.entity.actionState.GetState() is GroundedFighterPerfectShieldState)
                    {
                        entity.stateVars.freezeTime += Time.fixedDeltaTime * 3f;
                        hit.box.entity.stateVars.freezeTime = Mathf.Clamp(hit.box.entity.stateVars.freezeTime, Time.fixedDeltaTime * 3, Time.fixedDeltaTime * 8);
                        Debug.Log(hit.box.entity.stateVars.freezeTime);
                    }
    
                }
            }
        }
    
        public void KnockbackNormal(HitObject hit)
        {
            Launch(hit, SidedKnockback(entity.stateVars.flippedLeft));
        }
        public void KnockbackNormalWind(HitObject hit)
        {
            LaunchWind(hit, SidedKnockback(entity.stateVars.flippedLeft));
        }
        public void KnockbackNormalCompleteOverride(HitObject hit)
        {
            if (hit.box.entity.gameObject.layer == 16)
                return;
            if (hit.box.entity.stateVars.invulnerable)
                return;
            if (hitProperties.knockbackGrowth <= 0 && hitProperties.knockback <= 0)
                return;
            if (hit.box.entity.stateVars.damageArmor > hit.hitbox.hitProperties.damage || hit.box.entity.stateVars.knockbackArmor > hitProperties.knockback)
                return;
            hit.box.entity.stateVars.indieSpd *= 0;
            Launch(hit, SidedKnockback(entity.stateVars.flippedLeft));
        }
    
        public void KnockbackSideDependent(HitObject hit)
        {
            var leftSide = (hit.box.transform.position - transform.position).x < 0;
            Launch(hit, SidedKnockback(leftSide));
        }
    
        public void KnockbackSuction(HitObject hit)
        {
            if (hit.box is HitBox || hit.box is ShieldBox)
                Launch(hit, -(hit.hitBy.offset * hit.hitbox.transform.lossyScale + (Vector2)hit.hitBy.transform.position - (Vector2)hit.box.transform.position).normalized);
            else
                Launch(hit, (hit.hitBy.offset * hit.hitbox.transform.lossyScale + (Vector2)hit.hitBy.transform.position - (Vector2)hit.box.transform.position).normalized);
        }
        public void KnockbackSuctionInvert(HitObject hit)
        {
            if (hit.box is HitBox || hit.box is ShieldBox)
                Launch(hit, -((Vector2)hit.box.transform.position - hit.hitBy.offset * hit.hitbox.transform.lossyScale + (Vector2)hit.hitBy.transform.position).normalized);
            else
                Launch(hit, ((Vector2)hit.box.transform.position - hit.hitBy.offset * hit.hitbox.transform.lossyScale + (Vector2)hit.hitBy.transform.position).normalized);
        }
        public void KnockbackSpeedDependent(HitObject hit)
        {
            var dir = (hit.hitbox.entity.stateVars.indieSpd + hit.hitbox.entity.stateVars.externalSpd + hit.hitbox.entity.stateVars.windSpd).normalized;
            if ((hit.hitbox.entity.stateVars.indieSpd + hit.hitbox.entity.stateVars.externalSpd + hit.hitbox.entity.stateVars.windSpd).magnitude < 1f)
            {
                KnockbackSideDependent(hit);
                return;
            }
            Launch(hit, dir);
        }
        public void KnockbackRotationDependent(HitObject hit)
        {
            var dir = (hit.hitbox.entity.transform.up).normalized;
            if ((hit.hitbox.entity.stateVars.indieSpd + hit.hitbox.entity.stateVars.externalSpd).magnitude < 1f)
            {
                KnockbackSideDependent(hit);
                return;
            }
            Launch(hit, dir);
        }
        public void KnockbackRotationDependentWithAngleOffset(HitObject hit)
        {
            var angle = Mathf.Atan2(SidedKnockback(entity.stateVars.flippedLeft).x, SidedKnockback(entity.stateVars.flippedLeft).y) * Mathf.Rad2Deg;
            var dir = Quaternion.Euler(0, 0, angle) * hit.hitbox.entity.transform.up.normalized;
            Launch(hit, dir);
        }
    
        public void KnockbackSpeedDependentWithAngleOffset(HitObject hit)
        {
            var dir = (hit.hitbox.entity.stateVars.indieSpd + hit.hitbox.entity.stateVars.externalSpd + hit.hitbox.entity.stateVars.windSpd).normalized;
            if ((hit.hitbox.entity.stateVars.indieSpd + hit.hitbox.entity.stateVars.externalSpd + hit.hitbox.entity.stateVars.windSpd).magnitude < 1f)
            {
                KnockbackSideDependent(hit);
                return;
            }
            Launch(hit, dir);
        }
        
        public void HurtStun(HitObject hit)
        {
            if (hit.box.entity.stateVars.invulnerable)
                return;
            if (hit.box is HitBox || hit.box is ShieldBox)
                return;
            if (hit.box.entity.stateVars.damageArmor > hit.hitbox.hitProperties.damage || hit.box.entity.stateVars.knockbackArmor > CalculateLaunchSpeed(hit))
                return;
            if (hit.box.entity.TryGetComponent(out BaseFighterBehavior fighter))
            {
                if (!fighter.entity.stateVars.aerial && !fighter.BounceCondition)
                    hit.box.entity.actionState.SetState(new GroundedFighterHurtFreezeState(hit.box.entity.actionState as FighterSM));
                else
                    hit.box.entity.actionState.SetState(new AirborneFighterHurtFreezeState(hit.box.entity.actionState as FighterSM));
    
                var currentSpd = hit.box.entity.stateVars.indieSpd.magnitude * 2f;
                float stunTimerHighLaunchSkew = (currentSpd - currentSpd * 0.04f) / (0.04f * currentSpd) * Time.fixedDeltaTime;
                if (currentSpd == 0)
                    stunTimerHighLaunchSkew = 0;
                float stunTimerLowLaunchSkew = currentSpd * 0.02f;
    
                fighter.stateVarsF.stunStopTime = (stunTimerLowLaunchSkew + stunTimerHighLaunchSkew) / 2;
            }
        }
        public void HurtStunForceHeavy(HitObject hit)
        {
            if (hit.box.entity.stateVars.invulnerable)
                return;
            if (hit.box is HitBox || hit.box is ShieldBox)
                return;
            if (hit.box.entity.stateVars.damageArmor > hit.hitbox.hitProperties.damage || hit.box.entity.stateVars.knockbackArmor > CalculateLaunchSpeed(hit))
                return;
            if (hit.box.entity.TryGetComponent(out BaseFighterBehavior fighter))
            {
                if (!fighter.entity.stateVars.aerial && !fighter.BounceCondition)
                    hit.box.entity.actionState.SetState(new GroundedFighterHurtFreezeState(hit.box.entity.actionState as FighterSM));
                else
                    hit.box.entity.actionState.SetState(new AirborneFighterHurtFreezeIntoForcedHurtHeavyState(hit.box.entity.actionState as FighterSM));
    
                var currentSpd = hit.box.entity.stateVars.indieSpd.magnitude * 2f;
                float stunTimerHighLaunchSkew = (currentSpd - currentSpd * 0.04f) / (0.04f * currentSpd) * Time.fixedDeltaTime;
                if (currentSpd == 0)
                    stunTimerHighLaunchSkew = 0;
                float stunTimerLowLaunchSkew = currentSpd * 0.02f;
    
                fighter.stateVarsF.stunStopTime = (stunTimerLowLaunchSkew + stunTimerHighLaunchSkew) / 2;
            }
        }
        public void HurtStunEntity(Entity hit)
        {
            if (hit.stateVars.invulnerable)
                return;
            if (hit.TryGetComponent(out BaseFighterBehavior fighter))
            {
                if (!fighter.entity.stateVars.aerial && !fighter.BounceCondition)
                    hit.actionState.SetState(new GroundedFighterHurtFreezeState(hit.actionState as FighterSM));
                else
                    hit.actionState.SetState(new AirborneFighterHurtFreezeState(hit.actionState as FighterSM));
    
                var currentSpd = hit.stateVars.indieSpd.magnitude;
                float stunTimerHighLaunchSkew = (currentSpd - currentSpd * 0.04f) / (0.04f * currentSpd) * Time.fixedDeltaTime;
                if (currentSpd == 0)
                    stunTimerHighLaunchSkew = 0;
                float stunTimerLowLaunchSkew = currentSpd * 0.02f;
    
                fighter.stateVarsF.stunStopTime = (stunTimerLowLaunchSkew + stunTimerHighLaunchSkew) / 2;
            }
        }
        public void SpeedInherit(HitObject hit)
        {
            if (hit.box.entity.gameObject.layer == 16)
                return;
            if (hit.box.entity.stateVars.invulnerable)
                return;
            Vector2 LaunchSpd = (hit.hitbox.entity.stateVars.indieSpd + hit.hitbox.entity.stateVars.externalSpd + hit.hitbox.entity.stateVars.windSpd);
            hit.box.entity.stateVars.lastLaunchSpd += LaunchSpd;
            if (hit.box is ShieldBox)
            {
                if (hit.box.owner.actionState.GetState() is GroundedFighterPerfectShieldState || hit.box.owner.actionState.GetState() is GroundedFighterPerfectShieldEndState)
                    return;
                LaunchSpd *= 0.25f;
                if (!hit.box.owner.stateVars.aerial)
                    LaunchSpd *= Vector2.right;
            }
            if (hit.box is HitBox)
            {
                LaunchSpd *= 0.25f;
                if (!hit.box.entity.stateVars.aerial)
                    LaunchSpd *= Vector2.right;
            }
            hit.box.entity.stateVars.indieSpd += LaunchSpd;
        }
        public void SpeedInheritAlignKnockbackDir(HitObject hit)
        {
            if (hit.box.entity.gameObject.layer == 16)
                return;
            if (hit.box.entity.stateVars.invulnerable)
                return;
            Vector2 LaunchSpd = (hit.hitbox.entity.stateVars.indieSpd + hit.hitbox.entity.stateVars.externalSpd + hit.hitbox.entity.stateVars.windSpd);
            LaunchSpd = Vector2.Dot(LaunchSpd.normalized, SidedKnockback(entity.stateVars.flippedLeft).normalized) < 0 ? Vector2.zero : Vector3.Project(LaunchSpd, SidedKnockback(entity.stateVars.flippedLeft).normalized);
    
            hit.box.entity.stateVars.lastLaunchSpd += LaunchSpd;
            if (hit.box is ShieldBox)
            {
                if (hit.box.owner.actionState.GetState() is GroundedFighterPerfectShieldState || hit.box.owner.actionState.GetState() is GroundedFighterPerfectShieldEndState)
                    return;
                LaunchSpd *= 0.25f;
                if (!hit.box.owner.stateVars.aerial)
                    LaunchSpd *= Vector2.right;
            }
            if (hit.box is HitBox)
            {
                LaunchSpd *= 0.25f;
                if (!hit.box.entity.stateVars.aerial)
                    LaunchSpd *= Vector2.right;
            }
            hit.box.entity.stateVars.indieSpd += LaunchSpd;
        }
        public void SpeedInheritWeak(HitObject hit)
        {
            if (hit.box.entity.gameObject.layer == 16)
                return;
            if (hit.box.entity.stateVars.invulnerable)
                return;
            Vector2 LaunchSpd = (hit.hitbox.entity.stateVars.indieSpd + hit.hitbox.entity.stateVars.externalSpd + hit.hitbox.entity.stateVars.windSpd) * 0.25f;
            hit.box.entity.stateVars.lastLaunchSpd += LaunchSpd;
            if (hit.box is ShieldBox)
            {
                if (hit.box.owner.actionState.GetState() is GroundedFighterPerfectShieldState || hit.box.owner.actionState.GetState() is GroundedFighterPerfectShieldEndState)
                    return;
                LaunchSpd *= 0.25f;
                if (!hit.box.owner.stateVars.aerial)
                    LaunchSpd *= Vector2.right;
            }
            if (hit.box is HitBox)
            {
                LaunchSpd *= 0.25f;
                if (!hit.box.entity.stateVars.aerial)
                    LaunchSpd *= Vector2.right;
            }
            hit.box.entity.stateVars.indieSpd += LaunchSpd;
        }
    
        public void Grab(HitObject hit)
        {
            if (hitProperties.grabTarget != null)
                return;
            if (hit.box.entity.stateVars.invulnerable)
                return;
            if (hit.box is HitBox || hit.box is ShieldBox)
                return;
            if (hit.box.entity.TryGetComponent(out BaseFighterBehavior fighter))
            {
                if ((fighter.ActionState.GetState() is GroundedFighterGrabState || fighter.ActionState.GetState() is GroundedFighterThrowTechState) && fighter.entity.stateVars.flippedLeft != hit.hitbox.entity.stateVars.flippedLeft)
                {
                    fighter.entity.actionState.SetState(new GroundedFighterThrowTechState(fighter.entity.actionState as FighterSM));
                    return;
                }
                hitProperties.grabTarget = hit.box.entity;
            }
        }
    
        public void FaceEnemy(HitObject hit)
        {
            if (hit.box.entity.stateVars.invulnerable)
                return;
            if (hit.box.entity.TryGetComponent(out BaseFighterBehavior fighter))
                hit.box.entity.stateVars.flippedLeft = !hit.hitbox.entity.stateVars.flippedLeft;
        }
    
        public void Death(HitObject hit)
        {
            if (hit.box is HitBox || hit.box is ShieldBox)
                return;
    
            if (hit.box.entity.TryGetComponent(out BaseFighterBehavior fighter))
            {
                if (fighter.ActionState.GetState() is AirborneFighterRespawnState && hit.box.transform.position.y > 0)
                    return;
    
                hit.box.entity.actionState.SetState(new GroundedFighterDeadState(hit.box.entity.actionState as FighterSM));
            }
            hit.box.entity.SetEntityActive(false);
        }
    
    
        public Vector2 SidedKnockback(bool condition)
        {
            var knockbackDir = (Vector2)(Quaternion.Euler(0, 0, -hitProperties.knockbackDir) * Vector2.up);
            return Vector2.Scale(knockbackDir, (condition ? Vector2.left : Vector2.right) + Vector2.up);
        }
        public void Launch(HitObject hit, Vector2 knockbackDir)
        {
            if (hit.box.entity.gameObject.layer == 16)
                return;
            if (hit.box.entity.stateVars.invulnerable)
                return;
            if (hitProperties.knockbackGrowth <= 0 && hitProperties.knockback <= 0)
                return;
            if (hit.box.entity.stateVars.damageArmor > hit.hitbox.hitProperties.damage || hit.box.entity.stateVars.knockbackArmor > hitProperties.knockback)
                return;
            Vector2 LaunchSpd = CalculateLaunchSpeed(hit) * knockbackDir;
    
            LaunchSpd /= hit.box.entity.transform.lossyScale.y;
            hit.box.entity.stateVars.lastLaunchSpd = LaunchSpd;
            if (hit.box is ShieldBox)
            {
                if (hit.box.owner.actionState.GetState() is GroundedFighterPerfectShieldState || hit.box.owner.actionState.GetState() is GroundedFighterPerfectShieldEndState)
                    return;
                LaunchSpd *= 0.25f;
                if (!hit.box.owner.stateVars.aerial)
                    LaunchSpd *= Vector2.right;
            }
            if (hit.box is HitBox)
            {
                LaunchSpd *= 0.25f;
                if (!hit.box.entity.stateVars.aerial)
                    LaunchSpd *= Vector2.right;
            }
    
            if (hit.box is not ShieldBox && (hit.box.entity.physicsState.GetState() is Launched || hit.box.entity.actionState.GetState() is GroundedFighterHurtFreezeState || hit.box.entity.actionState.GetState() is AirborneFighterHurtFreezeState) 
                && (hit.box.entity.stateVars.indieSpd.magnitude > 20 || hitProperties.stackKnockbackRegardlessOfLaunchSpeed) && hit.box is HurtBox)
            {
                hit.box.entity.stateVars.indieSpd = (LaunchSpd * 2 + hit.box.entity.stateVars.indieSpd).normalized * Mathf.Clamp(LaunchSpd.magnitude + hit.box.entity.stateVars.indieSpd.magnitude * (hit.box.entity.stateVars.freezeTime > 0 ? 0.5f : 1), hit.box.entity.stateVars.indieSpd.magnitude, Mathf.Infinity);
                hit.box.entity.stateVars.lastLaunchSpd = hit.box.entity.stateVars.indieSpd;
            }
            else if (hit.box is HitBox)
                hit.box.entity.stateVars.indieSpd += LaunchSpd;
            else
                hit.box.entity.stateVars.indieSpd = LaunchSpd;
    
    
            if (hit.box.entity.TryGetComponent(out BaseFighterBehavior fighter))
                KnockbackBracing(hit, fighter);
    
    
            void KnockbackBracing(HitObject hit, BaseFighterBehavior fighter)
            {
                float HorizontalKnockbackBracing = hit.box.entity.stateVars.indieSpd.x * (fighter.controlling.moveStick.raw.x * 0.05f);
                hit.box.entity.stateVars.indieSpd = new Vector2(hit.box.entity.stateVars.indieSpd.x + HorizontalKnockbackBracing, hit.box.entity.stateVars.indieSpd.y);
            }
        }
    
        public void LaunchWind(HitObject hit, Vector2 knockbackDir)
        {
            if (hit.box.entity.gameObject.layer == 16)
                return;
            if (hit.box.entity.stateVars.invulnerable)
                return;
            if (hitProperties.knockbackGrowth <= 0 && hitProperties.knockback <= 0)
                return;
            if (hit.box is ShieldBox)
                return;
            if (hit.box is HitBox)
                return;
    
            if (hit.box.entity.actionState.GetState() is AirborneFighterLedgeEndState || hit.box.entity.actionState.GetState() is AirborneFighterLedgeIdleState || hit.box.entity.actionState.GetState() is AirborneFighterLedgeStartState || hit.box.entity.actionState.GetState() is GroundedFighterLedgeEndState)
                return;
            Vector2 LaunchSpd = CalculateLaunchSpeed(hit) * knockbackDir;
    
            LaunchSpd /= hit.box.entity.transform.lossyScale.y;
    
            hit.box.entity.stateVars.windSpd += LaunchSpd;
            hit.box.entity.stateVars.indieSpd += LaunchSpd;
        }
    
        public float CalculateLaunchSpeed(HitObject hit)
        {
            return Mathf.Clamp(((hitProperties.knockbackGrowth * hit.box.entity.stateVars.percent * (3 / hit.box.entity.et.mass)) * (hit.box is HurtBox ? 1 : 0) + hitProperties.knockback * (3 / hit.box.entity.et.mass)) * 0.8f, 0, 150);
        }
        #endregion
    
    }
    
    
    [System.Serializable]
    public class OnHitEvent : UnityEvent<HitObject> { }
    
    [System.Serializable]
    public struct HitProperties
    {
        public float freezeMultiplier;
        public float shieldFreezeMultiplier;
        public float damage;
        public float shieldDamage;
        public float knockback;
        public float knockbackGrowth;
        public float hitCoolDown;
        public float knockbackDir;
    
        public bool stackKnockbackRegardlessOfLaunchSpeed;
        public bool hitGroundedOnly;
        public bool hitOwner;
    
        public Entity owner;
        public Entity grabTarget;
    }}
