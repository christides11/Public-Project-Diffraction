namespace TightStuff
{
    ﻿using System.Collections.Generic;
    using TightStuff.Extension;
    using UnityEngine;
    
    public abstract class FighterState : ActionState
    {
        protected bool lag = false;
        public readonly new FighterSM fsm;
    
        public enum AttackID { Neutral, Forward, Up, Down, Back, Dash, Grab }
    
        public FighterState(FighterSM fsm) : base(fsm)
        {
            this.fsm = fsm;
        }
        protected LayerMask GroundLayers { get => fsm.fighter.groundLayers; }
        protected LayerMask GroundPlatLayers { get => fsm.fighter.groundplatLayers; }
    
        protected Vector2 HurtboxRotation { get => fsm.fighter.stateVarsF.hurtboxRotation; set => fsm.fighter.stateVarsF.hurtboxRotation = value; }
        protected float SpawnInvulnTimer { get => fsm.fighter.stateVarsF.spawnInvulnTimer; set => fsm.fighter.stateVarsF.spawnInvulnTimer = value; }
        protected bool PlayerCollision { get => fsm.fighter.stateVarsF.playerCollision; set => fsm.fighter.stateVarsF.playerCollision = value; }
    
        protected float WalktimePassed { get => fsm.fighter.stateVarsF.walktimePassed; set => fsm.fighter.stateVarsF.walktimePassed = value; }
        protected bool Turning { get => fsm.fighter.stateVarsF.turning; set => fsm.fighter.stateVarsF.turning = value; }
        protected bool LastLeftDir { get => fsm.fighter.stateVarsF.lastLeftDir; set => fsm.fighter.stateVarsF.lastLeftDir = value; }
        protected bool CrouchLag { get => fsm.fighter.stateVarsF.crouchLag; set => fsm.fighter.stateVarsF.crouchLag = value; }
        protected float JumptimePassed { get => fsm.fighter.stateVarsF.jumptimePassed; set => fsm.fighter.stateVarsF.jumptimePassed = value; }
        protected float PlatformInvuln { get => fsm.fighter.stateVarsF.platformInvuln; set => fsm.fighter.stateVarsF.platformInvuln = value; }
        protected float PlatformDropTimer { get => fsm.fighter.stateVarsF.platformDropTimer; set => fsm.fighter.stateVarsF.platformDropTimer = value; }
        protected bool StopDash { get => fsm.fighter.stateVarsF.stopDash; set => fsm.fighter.stateVarsF.stopDash = value; }
        protected bool DashJump { get => fsm.fighter.stateVarsF.dashJump; set => fsm.fighter.stateVarsF.dashJump = value; }
        protected bool Jumping { get => fsm.fighter.stateVarsF.jumping; set => fsm.fighter.stateVarsF.jumping = value; }
        protected bool JumpingTap { get => fsm.fighter.stateVarsF.jumpingTap; set => fsm.fighter.stateVarsF.jumpingTap = value; }
        protected bool JumpingToggle { get => fsm.fighter.stateVarsF.jumpingToggle; set => fsm.fighter.stateVarsF.jumpingToggle = value; }
    
        protected Vector2 AirdashDir { get => fsm.fighter.stateVarsF.airdashDir; set => fsm.fighter.stateVarsF.airdashDir = value; }
        protected bool AirdashCancelable { get => fsm.fighter.stateVarsF.airdashCancelable; set => fsm.fighter.stateVarsF.airdashCancelable = value; }
        protected float AirdashTimePassed { get => fsm.fighter.stateVarsF.airdashTimePassed; set => fsm.fighter.stateVarsF.airdashTimePassed = value; }
        protected float AirdashDownSpeed { get => fsm.fighter.stateVarsF.airdashDownSpeed; set => fsm.fighter.stateVarsF.airdashDownSpeed = value; }
    
        protected float StunStopTime { get => fsm.fighter.stateVarsF.stunStopTime; set => fsm.fighter.stateVarsF.stunStopTime = value; }
        protected float TechCooldown { get => fsm.fighter.stateVarsF.techCooldown; set => fsm.fighter.stateVarsF.techCooldown = value; }
        protected float FocusTimePassed { get => fsm.fighter.stateVarsF.focusTimePassed; set => fsm.fighter.stateVarsF.focusTimePassed = value; }
    
        protected Vector2 Displace { get => fsm.fighter.stateVarsF.displace; set => fsm.fighter.stateVarsF.displace = value; }
    
        protected bool SpellcardState { get => fsm.fighter.stateVarsF.spellcardState; set => fsm.fighter.stateVarsF.spellcardState = value; }
        protected int SpellcardUpCount { get => fsm.fighter.stateVarsF.spellcardUpCount; set => fsm.fighter.stateVarsF.spellcardUpCount = value; }
        protected int SpellcardSideCount { get => fsm.fighter.stateVarsF.spellcardSideCount; set => fsm.fighter.stateVarsF.spellcardSideCount = value; }
        protected int SpellcardDownCount { get => fsm.fighter.stateVarsF.spellcardDownCount; set => fsm.fighter.stateVarsF.spellcardDownCount = value; }
    
        protected int RollDir { get => fsm.fighter.stateVarsF.rollDir; set => fsm.fighter.stateVarsF.rollDir = value; }
    
        protected int Stocks { get => fsm.fighter.stateVarsF.stocks; set => fsm.fighter.stateVarsF.stocks = value; }
        protected float Stamina { get => fsm.fighter.stateVarsF.stamina; set => fsm.fighter.stateVarsF.stamina = Mathf.Clamp(value, 0, fsm.fighter.stateVarsF.maxStamina); }
        protected float MaxStamina { get => fsm.fighter.stateVarsF.maxStamina; set => fsm.fighter.stateVarsF.maxStamina = value; }
        protected float GrazeMeter { get => fsm.fighter.stateVarsF.grazeMeter; set => fsm.fighter.stateVarsF.grazeMeter = value; }
    
    
        protected AttackID CurrentAttackID { get => fsm.fighter.stateVarsF.currentAttackID; set => fsm.fighter.stateVarsF.currentAttackID = value; }
        protected AttackID CurrentSpecialID { get => fsm.fighter.stateVarsF.currentSpecialID; set => fsm.fighter.stateVarsF.currentSpecialID = value; }
        protected AttackID CurrentSpellcardID { get => fsm.fighter.stateVarsF.currentSpellcardID; set => fsm.fighter.stateVarsF.currentSpellcardID = value; }
        protected bool AutoCancel { get => fsm.fighter.stateVarsF.autoCancel; set => fsm.fighter.stateVarsF.autoCancel = value; }
        protected bool SmashAutoCancel { get => fsm.fighter.stateVarsF.smashAutoCancel; set => fsm.fighter.stateVarsF.smashAutoCancel = value; }
        protected Vector2 SpecialAim { get => fsm.fighter.stateVarsF.specialAim; set => fsm.fighter.stateVarsF.specialAim = value; }
        protected float SpecialCharge { get => fsm.fighter.stateVarsF.specialCharge; set => fsm.fighter.stateVarsF.specialCharge = value; }
        protected float SpecialTimer { get => fsm.fighter.stateVarsF.specialTimer; set => fsm.fighter.stateVarsF.specialTimer = value; }
        protected float AttackCharge { get => fsm.fighter.stateVarsF.attackCharge; set => fsm.fighter.stateVarsF.attackCharge = value; }
    
        protected bool TapDash { get => Controlling.moveStick.BufferTap() && Controlling.tapDash; }
        protected bool TapAirDash { get => Controlling.moveStick.BufferTap() && Controlling.tapAirdash; }
        protected bool TapSmash { get => ((Controlling.moveStick.BufferTap() && Controlling.attackButton.Buffer()) || Controlling.attackStick.BufferTap()) && Controlling.tapSmash; }
        protected bool TapJump { get => Controlling.moveStick.BufferTap() && Controlling.tapJump & Controlling.moveStick.tapInitPos.y > 0.4f; }
        protected bool TapDrop { get => Controlling.moveStick.BufferTap() && Controlling.tapDrop & Controlling.moveStick.tapInitPos.y < -0.5f; }
    
        protected bool ToggleDash { get => Mathf.Abs(Controlling.moveStick.raw.magnitude) > 0.75f && Controlling.dashToggleButton.Buffer(); }
        protected bool ToggleAirDash { get => Mathf.Abs(Controlling.moveStick.raw.magnitude) > 0.75f && Controlling.airdashToggleButton.Buffer(); }
        protected bool ToggleJump { get => Controlling.moveStick.raw.y > 0.4f && Controlling.jumpToggleButton.Buffer(); }
        protected bool ToggleDrop { get => Controlling.moveStick.raw.y < -0.5f && Controlling.dashToggleButton.Buffer(); }
        protected bool StickSmash { get => Controlling.smashStick.BufferHold(); }
        protected bool StickSpecial { get => Controlling.specialStick.BufferHold(); }
    
        protected bool AssistCondition => (Controlling.assist.Buffer()) && fsm.fighter.stateVarsF.assistCD <= 0;
    
        public bool AttackCondition { get => Controlling.smashButton.Buffer() || Controlling.attackButton.Buffer() || StickSmash || Controlling.attackStick.BufferHold(); }
        public bool SpecialCondition { get => Controlling.specialButton.Buffer() || StickSpecial; }
        public bool AttackHoldCondition { get => Controlling.smashButton.raw || Controlling.attackButton.raw || Controlling.attackStick.raw.magnitude > 0.75f; }
        public bool SpecialHoldCondition { get => Controlling.specialButton.raw || Controlling.specialStick.raw.magnitude > 0.75f; }
        protected bool SpellcardCondition { get => SpecialCondition && (Controlling.smashToggleButton.raw || Controlling.smashButton.raw || (Controlling.tapSmash && Controlling.specialStick.BufferTap())); }
    
        protected bool SpellcardTriggerCondition => SpellcardState || GrazeMeter >= 30;
    
        protected bool SpecialCancelCondition => !Controlling.specialButton.raw && Controlling.specialStick.raw.magnitude <= 0.5f;
        protected bool AttackCancelCondition => !Controlling.attackButton.raw && Controlling.attackStick.raw.magnitude <= 0.5f && Controlling.smashStick.raw.magnitude <= 0.5f;
    
        protected ContactPoint2D[] Collision { get => fsm.entity.stateVars.currentCollision; }
    
        protected Controller Controlling { get => fsm.fighter.controlling; }
        protected FighterProperties Ft { get => fsm.fighter.Ft; }
        public bool Lag => lag;
    
        public bool MoveStickAgainstFlip(Vector2 moveStick)
        {
            return FlippedLeft != moveStick.x < 0 && Mathf.Abs(moveStick.x) > 0.1;
        }
        protected Vector2 ConvertVectorTo8Dir(Vector2 dir)
        {
            if (dir.x > 0.4f || dir.x < -0.4f)
            {
                dir = new Vector2(Mathf.Sign(dir.x), dir.y);
            }
            else
            {
                dir = Vector2.up * dir.y;
            }
            if (dir.y > 0.4f || dir.y < -0.4f)
            {
                dir = new Vector2(dir.x, Mathf.Sign(dir.y));
            }
            else
            {
                dir = Vector2.right * dir.x;
            }
    
            return Vector2.ClampMagnitude(dir, 1);
        }
    
        protected void CheckTurns()
        {
            if (InputNotZero())
                Turning = MoveStickAgainstFlip(Controlling.moveStick.raw);
        }
        protected bool InputNotZero()
        {
            return Mathf.Abs(Controlling.moveStick.raw.x) > 0.1f;
        }
    
        protected void ToggleConsume()
        {
            Controlling.airdashToggleButton.ConsumeBuffer();
            Controlling.dashToggleButton.ConsumeBuffer();
            Controlling.jumpToggleButton.ConsumeBuffer();
        }
    
        protected AttackID GetAttackID(Vector2 attackDir)
        {
            var command = AttackID.Neutral;
            if (attackDir.y > 0.6f)
                command = AttackID.Up; //Up
    
            else if (attackDir.y < -0.6f)
                command = AttackID.Down; //Down
    
            else if (Mathf.Abs(attackDir.x) > 0.3f)
            {
                command = AttackID.Forward; //Forward
                if (MoveStickAgainstFlip(attackDir))
                    command = AttackID.Back; //Back
            }
            return command;
        }
    
        public Vector2 GetAttackDir(Controller.Button button, Controller.Stick stick)
        {
            if (Controlling.neutralLock.raw)
                return Vector2.zero;
            if (stick.BufferHold())
                return stick.holdInitPos.normalized;
    
            if (button.stickAccompanimentOnTap.magnitude > 0.3 && button.Buffer())
                return button.stickAccompanimentOnTap.normalized;
    
            return Vector2.zero;
        }
    
        protected void ResetStateVars()
        {
            WalktimePassed = 0;
            SelfSpd *= 0;
            JumptimePassed = 1;
            PlatformDropTimer = 0;
    
            if (Turning)
                FlippedLeft = !FlippedLeft;
    
            Turning = false;
        }
        protected void RotateHurtbox(Vector2 up)
        {
            HurtboxRotation = up;
            fsm.entity.SetFlipped();
            foreach (Transform t in fsm.fighter.hurtbox)
            {
                t.up = HurtboxRotation;
            }
        }
        protected virtual void RecordSpecialAim()
        {
            if (Controlling.cStick.raw.magnitude > 0.8f)
            {
                SpecialAim = Controlling.cStick.raw;
            }
            else if (Controlling.moveStick.raw.magnitude > 0.8f)
            {
                SpecialAim = Controlling.moveStick.raw;
            }
        }
    
        protected void ResetPhysicsStates()
        {
            if (Aerial)
                fsm.entity.physicsState.SetState(new InAir(fsm.entity.physicsState));
            else
                fsm.entity.physicsState.SetState(new OnGround(fsm.entity.physicsState));
        }
    }
    
    public abstract class GroundedFighterState : FighterState
    {
        protected bool walkable = true;
        protected bool crouchable = true;
        protected bool attackable = true;
        protected bool dashable = true;
        protected bool shieldable = true;
        protected bool rollable = true;
        protected bool jumpable = true;
        protected bool ledgedroppable = true;
        protected bool specialable = true;
    
        #region TransitionConditions
        protected bool DashCondition { get => (TapDash || ToggleDash) && Mathf.Abs(Controlling.moveStick.raw.x) > 0.5f; }
        protected bool DropCondition { get => PlatformDropTimer >= Controlling.moveStick.stickSensitivity * Time.fixedDeltaTime && Controlling.moveStick.raw.y < -0.5f && Collision[0].collider.gameObject.layer != 0 && (TapDrop || ToggleDrop); }
        protected bool WalkCondition { get => Mathf.Abs(Controlling.moveStick.raw.x) > 0.1; }
        protected bool CrouchCondition { get => Controlling.moveStick.raw.y < -0.5; }
        protected bool JumpCondition { get => Controlling.jumpButton.Buffer(); }
        protected bool TauntCondition { get => Controlling.tauntButton.Buffer(); }
        protected bool GrabCondition { get => Controlling.grabButton.Buffer(); }
        protected bool JumpTapCondition { get => TapJump && FreezeTime <= 0; }
        protected bool JumpToggleCondition { get => ToggleJump; }
        protected bool ShieldCondition { get => Controlling.shieldButton.Buffer(); }
        #endregion
    
        public GroundedFighterState(FighterSM fsm) : base(fsm) { }
    
        public override void OnStateUpdate(float timeScale)
        {
            base.OnStateUpdate(timeScale);
            FocusTimePassed = 0;
            StaminaRegen(timeScale);
    
            CalculatePlatformSpd();
            CheckPlatdropCondition(timeScale);
    
            if (InputNotZero())
                LastLeftDir = Controlling.moveStick.raw.x < 0;
    
            CalculateWalkSpd(fsm.entity);
            DeclerateWalkSpeedProgress(timeScale);
    
            if (Aerial)
            {
                TransitionAir();
                return;
            }
    
            CheckFighterCollision(timeScale);
    
            if (lag || FreezeTime > 0 || fsm.entity.TrueTimeScale <= 0)
                return;
            CheckAssisting();
            if (CheckSpellcarding())
                return;
            if (CheckDashing())
                return;
            if (CheckJumping())
                return;
            if (CheckJumpingToggle())
                return;
            if (CheckJumpingTap())
                return;
            if (CheckDropping())
                return;
            if (CheckSpecialing())
                return;
            if (CheckAttacking())
                return;
            if (CheckTaunting())
                return;
            if (CheckShielding())
                return;
            if (CheckCrouching())
                return;
            if (CheckWalking())
                return;
    
            #region StateCheckFunctions
            bool CheckSpellcarding()
            {
                if (SpellcardCondition && SpellcardTriggerCondition && attackable)
                {
                    if (SpellcardState)
                    {
                        if (GrazeMeter > 30)
                            return false;
                        var specialDir = GetAttackDir(StickSpecial ? Controlling.attackButton : Controlling.specialButton, Controlling.cStick);
                        if (GetAttackID(specialDir) == AttackID.Back && (CurrentSpellcardID == AttackID.Neutral || CurrentSpellcardID == AttackID.Back || CurrentSpellcardID == AttackID.Forward))
                            fsm.fighter.extension.SpellcardTrigger(specialDir, AttackID.Back);
                        else
                            fsm.fighter.extension.SpellcardTrigger(specialDir, CurrentSpellcardID);
                    }
                    else
                    {
                        var specialDir = GetAttackDir(StickSpecial ? Controlling.attackButton : Controlling.specialButton, Controlling.cStick);
                        CurrentSpellcardID = GetAttackID(specialDir);
                        if (CurrentSpellcardID == AttackID.Down) //TEMPORARY
                            CurrentSpellcardID = AttackID.Up;
    
                        if (CurrentSpellcardID == AttackID.Up && SpellcardUpCount <= 0)
                            return false;
                        if ((CurrentSpellcardID == AttackID.Neutral || CurrentSpellcardID == AttackID.Back || CurrentSpellcardID == AttackID.Forward) && SpellcardSideCount <= 0)
                            return false;
                        if (CurrentSpellcardID == AttackID.Down && SpellcardDownCount <= 0)
                            return false;
    
                        SetState(new GroundedFighterSpellcardActivateState(fsm));
                    }
                    Turning = false;
                    Controlling.specialButton.ConsumeBuffer();
                    Controlling.cStick.ConsumeBufferHold();
                    Controlling.specialStick.ConsumeBufferHold();
                    Controlling.attackStick.ConsumeBufferHold();
                    Controlling.smashStick.ConsumeBufferHold();
                    Controlling.cStick.ConsumeBufferTap();
                    Controlling.specialStick.ConsumeBufferTap();
                    Controlling.attackStick.ConsumeBufferTap();
                    Controlling.smashStick.ConsumeBufferTap();
                    return true;
                }
                return false;
            }
            bool CheckWalking()
            {
                if (WalkCondition && walkable)
                {
                    SetState(new GroundedFighterWalkState(fsm));
                    return true;
                }
                return false;
            }
            bool CheckCrouching()
            {
                if (CrouchCondition && crouchable)
                {
                    SetState(new GroundedFighterCrouchState(fsm));
                    Turning = false;
                    return true;
                }
                return false;
            }
            bool CheckJumping()
            {
                if (JumpCondition && jumpable)
                {
                    Controlling.jumpButton.ConsumeBuffer();
                    if (Controlling.jumpButton.stickAccompanimentOnTap.y < -0.5f && Collision[0].collider.gameObject.layer != 0 && shieldable)
                    {
                        SetState(new AirborneFighterDropState(fsm));
                        return true;
                    }
                    SetState(new GroundedFighterJumpState(fsm));
                    Jumping = true;
                    return true;
                }
                return false;
            }
            bool CheckTaunting()
            {
                if (TauntCondition && attackable)
                {
                    SetState(new GroundedFighterTauntState(fsm));
                    Turning = false;
                    Controlling.tauntButton.ConsumeBuffer();
                    return true;
                }
                return false;
            }
            bool CheckAttacking()
            {
                if ((AttackCondition || GrabCondition) && attackable)
                {
                    SetState(new GroundedFighterAttackState(fsm));
                    Turning = false;
                    Controlling.attackButton.ConsumeBuffer();
                    Controlling.cStick.ConsumeBufferHold();
                    Controlling.attackStick.ConsumeBufferHold();
                    Controlling.smashStick.ConsumeBufferHold();
                    Controlling.cStick.ConsumeBufferTap();
                    Controlling.attackStick.ConsumeBufferTap();
                    Controlling.smashStick.ConsumeBufferTap();
                    if (GrabCondition)
                        Controlling.grabButton.ConsumeBuffer();
                    return true;
                }
                return false;
            }
            bool CheckSpecialing()
            {
                if (SpecialCondition && specialable)
                {
                    var specialDir = GetAttackDir(StickSpecial ? Controlling.attackButton : Controlling.specialButton, Controlling.cStick);
                    fsm.fighter.extension.SpecialTrigger(specialDir, GetAttackID(specialDir));
                    Controlling.cStick.ConsumeBufferHold();
                    Controlling.specialStick.ConsumeBufferHold();
                    Controlling.cStick.ConsumeBufferTap();
                    Controlling.specialStick.ConsumeBufferTap();
                    Controlling.specialButton.ConsumeBuffer();
                    return true;
                }
                return false;
            }
            bool CheckAssisting()
            {
                if (AssistCondition && specialable)
                {
                    var assist = fsm.fighter.assist.SpawnEntityFromPool("Assist", fsm.entity.transform.position, Quaternion.identity, new Vector3(FlippedLeft ? -1 : 1, 1, 1));
                    if (assist != null)
                    {
                        assist.stateVars.flippedLeft = FlippedLeft;
                        fsm.fighter.stateVarsF.assistCD = fsm.fighter.AssistObj.MaxLifeTime;
                    }
                    Controlling.assist.ConsumeBuffer();
                    return true;
                }
                return false;
            }
            bool CheckJumpingTap()
            {
                if (JumpTapCondition && jumpable)
                {
                    SetState(new GroundedFighterJumpState(fsm));
                    JumpingTap = true;
                    Controlling.moveStick.ConsumeBufferTap();
                    return true;
                }
                return false;
            }
            bool CheckJumpingToggle()
            {
                if (JumpToggleCondition && jumpable)
                {
                    SetState(new GroundedFighterJumpState(fsm));
                    JumpingToggle = true;
                    Controlling.moveStick.ConsumeBufferTap();
                    ToggleConsume();
                    return true;
                }
                return false;
            }
            bool CheckDashing()
            {
                if (DashCondition && dashable)
                {
                    SetState(new GroundedFighterDashStartState(fsm));
                    Controlling.moveStick.ConsumeBufferTap();
                    if (ToggleDash)
                        ToggleConsume();
                    return true;
                }
                return false;
            }
            bool CheckDropping()
            {
                if (DropCondition && shieldable)
                {
                    SetState(new AirborneFighterDropState(fsm));
                    Controlling.moveStick.ConsumeBufferTap();
                    if (ToggleDash)
                        ToggleConsume();
                    return true;
                }
                return false;
            }
            bool CheckShielding()
            {
                if (ShieldCondition && shieldable)
                {
                    SetState(new GroundedFighterPerfectShieldState(fsm));
                    Turning = false;
                    Controlling.shieldButton.ConsumeBuffer();
                    return true;
                }
                return false;
            }
            #endregion
        }
    
        private void StaminaRegen(float timeScale)
        {
            Stamina += timeScale * Time.fixedDeltaTime * fsm.entity.NonFreezeTimeScale * 4;
        }
    
        private void CheckFighterCollision(float timeScale)
        {
            var fighters = CheckPlayerCollision();
    
            if (fighters.Count <= 1)
                return;
    
            foreach (var fighter in fighters)
            {
                if (fighter == fsm.fighter || fighter.entity.stateVars.aerial)
                    continue;
    
                var weightRatio = fsm.fighter.Ft.mass / (fsm.fighter.Ft.mass + fighter.Ft.mass);
                var spdForce = (IndieSpd.x + SelfSpd.x) * weightRatio * weightRatio * timeScale * Time.fixedDeltaTime;
    
                if (fighter.transform.position.x > fsm.entity.transform.position.x)
                {
                    fighter.stateVarsF.displace += Mathf.Clamp(spdForce, 0.02f, Mathf.Infinity) * Vector2.right;
                    Displace -= spdForce * Vector2.right;
                }
                else if (fighter.transform.position.x < fsm.entity.transform.position.x)
                {
                    fighter.stateVarsF.displace += Mathf.Clamp(spdForce, -Mathf.Infinity, -0.02f) * Vector2.right;
                    Displace -= spdForce * Vector2.right;
                }
            }
        }
    
        protected virtual bool CheckLedge()
        {
            var midHit = Physics2D.Raycast(fsm.entity.transform.position, Vector2.down, (fsm.fighter.boxcastCol as BoxCollider2D).size.y, GroundPlatLayers);
            var leftHit = Physics2D.Raycast(fsm.entity.transform.position + (fsm.fighter.boxcastCol as BoxCollider2D).size.x * 0.5f * Vector3.left * fsm.entity.transform.lossyScale.y, Vector2.down, (fsm.fighter.boxcastCol as BoxCollider2D).size.y * fsm.entity.transform.lossyScale.y, GroundPlatLayers);
            var rightHit = Physics2D.Raycast(fsm.entity.transform.position + (fsm.fighter.boxcastCol as BoxCollider2D).size.x * 0.5f * Vector3.right * fsm.entity.transform.lossyScale.y, Vector2.down, (fsm.fighter.boxcastCol as BoxCollider2D).size.y * fsm.entity.transform.lossyScale.y, GroundPlatLayers);
    
            if ((!rightHit && (SelfSpd + IndieSpd).x > 0) || (!leftHit && (SelfSpd + IndieSpd).x < 0))
            {
                SelfSpd *= 0;
                IndieSpd *= 0;
    
                if (midHit)
                    return false;
    
                if (!rightHit)
                    fsm.entity.transform.position -= 0.1f * Vector3.right;
                else if (!leftHit)
                    fsm.entity.transform.position += 0.1f * Vector3.right;
                return false;
            }
            return true;
        }
    
        protected List<BaseFighterBehavior> CheckPlayerCollision()
        {
            var fighters = new List<BaseFighterBehavior>();
            if (!PlayerCollision)
                return fighters;
            var fighterCols = Physics2D.OverlapBoxAll(fsm.entity.transform.position + (Vector3)fsm.fighter.boxcastCol.offset * fsm.entity.transform.lossyScale.y, (fsm.fighter.boxcastCol as BoxCollider2D).size * fsm.entity.transform.lossyScale.y, fsm.entity.transform.eulerAngles.z, fsm.fighter.playerLayers);
            foreach (var fighterCol in fighterCols)
            {
                if (fighterCol.attachedRigidbody.TryGetComponent(out BaseFighterBehavior f))
                {
                    if (f.stateVarsF.playerCollision)
                        fighters.Add(f);
                }
            }
            return fighters;
        }
    
        private void CheckPlatdropCondition(float timeScale)
        {
            if (TapDrop || ToggleDrop)
                PlatformDropTimer += Time.fixedDeltaTime * timeScale;
            else
                PlatformDropTimer = 0;
        }
    
        private void CalculatePlatformSpd()
        {
            if (Collision != null)
            {
                if (Collision[0].collider.gameObject.TryGetComponent(out Entity plat))
                    ExternalSpd = plat.stateVars.indieSpd;
                else
                    ExternalSpd = Vector2.zero;
            }
            else
                ExternalSpd = Vector2.zero;
        }
    
        private void DeclerateWalkSpeedProgress(float timeScale)
        {
            WalktimePassed = Mathf.Lerp(WalktimePassed, 0, Et.groundRes * 2 * timeScale);
        }
    
        protected virtual void TransitionAir()
        {
            IndieSpd += ExternalSpd;
            ExternalSpd = Vector2.zero;
            PlatformInvuln = 0.3f;
            SetState(new AirborneFighterGroundToAirState(fsm));
        }
    
        private void CalculateWalkSpd(Entity entity)
        {
            WalktimePassed = ((int)(WalktimePassed * 1000)) / 1000f;
            SelfSpd = Vector2.right * WalktimePassed * Et.runSpd;
        }
    
        protected void PlayTurnToIdleAnimation(int animFrame)
        {
            fsm.entity.PlayAnim("Ground_Walk_Turn_End");
    
            if (FlippedLeft == LastLeftDir)
                fsm.entity.SkipCurrentAnimToFrame(animFrame);
    
            FlippedLeft = LastLeftDir;
            Turning = true;
        }
    }
    
    public abstract class AirborneFighterState : FighterState
    {
        protected bool moveable = true;
        protected bool moveableVertical = true;
        protected bool airDashable = true;
        protected bool focusable = true;
        protected bool staminaRegenable = true;
    
        protected bool LedgeGrabCondition { get => Controlling.grabButton.Buffer() || (Controlling.autoLedgeGrab && Controlling.moveStick.raw.y >= -0.5f); }
        protected bool AirdashCondition { get => Controlling.jumpButton.Buffer(); }
        protected bool AirdashTapCondition { get => TapAirDash; }
        protected bool AirdashToggleCondition { get => ToggleAirDash; }
        protected bool FocusCondition { get => Controlling.shieldButton.Buffer(); }
        protected bool LedgeCondition { get => Controlling.grabButton.Buffer() || (Controlling.autoLedgeGrab && Controlling.moveStick.raw.y >= -0.5f); }
    
        public AirborneFighterState(FighterSM fsm) : base(fsm) { }
        public override void OnStateUpdate(float timeScale)
        {
            base.OnStateUpdate(timeScale);
            SelfSpd *= 0;
            WalktimePassed = 0;
    
            JumptimePassed += Time.fixedDeltaTime * timeScale;
            PlatformInvuln -= Time.fixedDeltaTime * timeScale;
    
            CheckJumping();
            CheckJumpingToggle();
            CheckJumpingTap();
    
            if (staminaRegenable)
                StaminaRegen(timeScale);
    
            if (!Aerial)
            {
                TransitionGround();
                return;
            }
            CheckPlatformCondition();
    
            if (moveable)
                CalculateAirMoveSpeed(timeScale);
            if (lag || FreezeTime > 0 || fsm.entity.TrueTimeScale <= 0)
                return;
            CheckAssisting();
            if (CheckSpellcarding())
                return;
            if (CheckAirdashing())
                return;
            if (CheckAirdashingToggle())
                return;
            if (CheckAirdashingTap())
                return;
            if (CheckFocusing())
                return;
            if (CheckSpecialing())
                return;
            if (CheckAttacking())
                return;
            if (CheckLedgeGrabbing())
                return;
    
            #region StateCheckFunctions
            bool CheckSpellcarding()
            {
                if (SpellcardCondition && SpellcardTriggerCondition)
                {
                    if (SpellcardState)
                    {
                        if (GrazeMeter > 30)
                            return false;
                        var specialDir = GetAttackDir(StickSpecial ? Controlling.attackButton : Controlling.specialButton, Controlling.cStick);
                        fsm.fighter.extension.SpellcardTrigger(specialDir, CurrentSpellcardID);
                    }
                    else
                    {
                        var specialDir = GetAttackDir(StickSpecial ? Controlling.attackButton : Controlling.specialButton, Controlling.cStick);
                        CurrentSpellcardID = GetAttackID(specialDir);
                        if (CurrentSpellcardID == AttackID.Down) //TEMPORARY
                            CurrentSpellcardID = AttackID.Up;
    
                        if (CurrentSpellcardID == AttackID.Up && SpellcardUpCount <= 0)
                            return false;
                        if ((CurrentSpellcardID == AttackID.Neutral || CurrentSpellcardID == AttackID.Back || CurrentSpellcardID == AttackID.Forward) && SpellcardSideCount <= 0)
                            return false;
                        if (CurrentSpellcardID == AttackID.Down && SpellcardDownCount <= 0)
                            return false;
    
                        SetState(new GroundedFighterSpellcardActivateState(fsm));
                    }
                    Controlling.cStick.ConsumeBufferHold();
                    Controlling.specialStick.ConsumeBufferHold();
                    Controlling.attackStick.ConsumeBufferHold();
                    Controlling.smashStick.ConsumeBufferHold();
                    Controlling.cStick.ConsumeBufferTap();
                    Controlling.specialStick.ConsumeBufferTap();
                    Controlling.attackStick.ConsumeBufferTap();
                    Controlling.smashStick.ConsumeBufferTap();
                    Controlling.specialButton.ConsumeBuffer();
                    return true;
                }
                return false;
            }
            bool CheckAttacking()
            {
                if (AttackCondition)
                {
                    SetState(new AirborneFighterAttackState(fsm));
                    Controlling.cStick.ConsumeBufferHold();
                    Controlling.attackStick.ConsumeBufferHold();
                    Controlling.smashStick.ConsumeBufferHold();
                    Controlling.cStick.ConsumeBufferTap();
                    Controlling.attackStick.ConsumeBufferTap();
                    Controlling.smashStick.ConsumeBufferTap();
                    Controlling.attackButton.ConsumeBuffer();
                    return true;
                }
                return false;
            }
            bool CheckSpecialing()
            {
                if (SpecialCondition)
                {
                    var specialDir = GetAttackDir(StickSpecial ? Controlling.attackButton : Controlling.specialButton, Controlling.cStick);
                    fsm.fighter.extension.SpecialTrigger(specialDir, GetAttackID(specialDir));
                    Controlling.cStick.ConsumeBufferHold();
                    Controlling.specialStick.ConsumeBufferHold();
                    Controlling.cStick.ConsumeBufferTap();
                    Controlling.specialStick.ConsumeBufferTap();
                    Controlling.specialButton.ConsumeBuffer();
                    return true;
                }
                return false;
            }
            bool CheckAssisting()
            {
                if (AssistCondition)
                {
                    var assist = fsm.fighter.assist.SpawnEntityFromPool("Assist", fsm.entity.transform.position, Quaternion.identity, new Vector3(FlippedLeft ? -1 : 1, 1, 1));
                    if (assist != null)
                    {
                        assist.stateVars.flippedLeft = FlippedLeft;
                        fsm.fighter.stateVarsF.assistCD = assist.GetComponent<BaseProjectileBehaviour>().MaxLifeTime;
                    }
                    Controlling.assist.ConsumeBuffer();
                    return true;
                }
                return false;
            }
            bool CheckAirdashing()
            {
                if (AirdashCondition && airDashable && Stamina >= 0.9f)
                {
                    SetState(new AirborneFighterAirdashState(fsm));
                    Controlling.jumpButton.ConsumeBuffer();
                    return true;
                }
                return false;
            }
            bool CheckAirdashingTap()
            {
                if (AirdashTapCondition && airDashable && Stamina >= 0.9f)
                {
                    SetState(new AirborneFighterAirdashTapState(fsm));
                    Controlling.moveStick.ConsumeBufferTap();
                    return true;
                }
                return false;
            }
            bool CheckAirdashingToggle()
            {
                if (AirdashToggleCondition && airDashable && Stamina >= 0.9f)
                {
                    SetState(new AirborneFighterAirdashToggleState(fsm));
                    Controlling.moveStick.ConsumeBufferHold();
                    ToggleConsume();
                    return true;
                }
                return false;
            }
            bool CheckFocusing()
            {
                if (FocusCondition && focusable && Stamina > 0f)
                {
                    SetState(new AirborneFighterFocusSpinState(fsm));
                    return true;
                }
                return false;
            }
            bool CheckLedgeGrabbing()
            {
                var ledgeCol = fsm.fighter.ledgecastCol;
                var ledge = GetLedge(ledgeCol);
    
                var aboveGround = Physics2D.Raycast(fsm.entity.transform.position, Vector2.down, 10, GroundPlatLayers);
    
                if (!ledge || !LedgeCondition || aboveGround)
                    return false;
    
                SetLedgePosition(ledgeCol, ledge);
                Controlling.grabButton.ConsumeBuffer();
                SetState(new AirborneFighterLedgeStartState(fsm));
    
                return true;
            }
            void CheckJumping()
            {
                if (!Jumping)
                    return;
    
                if (IndieSpd.y < 0 || (!Controlling.jumpButton.raw && JumptimePassed >= Ft.shorthopTime) || JumptimePassed >= Ft.jumpTime)
                    Decelerate();
            }
            void CheckJumpingTap()
            {
                if (!JumpingTap)
                    return;
    
                if (IndieSpd.y < 0 || (Controlling.moveStick.raw.y <= 0.4f && JumptimePassed >= Ft.shorthopTime) || JumptimePassed >= Ft.jumpTime)
                    Decelerate();
            }
            void CheckJumpingToggle()
            {
                if (!JumpingToggle)
                    return;
    
                if (IndieSpd.y < 0 || ((!Controlling.jumpToggleButton.raw || Controlling.moveStick.raw.y <= 0.4f) && JumptimePassed >= Ft.shorthopTime) || JumptimePassed >= Ft.jumpTime)
                    Decelerate();
            }
            #endregion
        }
    
        protected Collider2D GetLedge(CircleCollider2D ledgeCol)
        {
            return Physics2D.OverlapCircle((Vector2)fsm.fighter.transform.position + ledgeCol.offset * fsm.fighter.transform.localScale, ledgeCol.radius, fsm.fighter.ledgeLayer);
        }
    
        protected void SetLedgePosition(CircleCollider2D ledgeCol, Collider2D ledge)
        {
            if (!ledge)
                return;
            var rightLedge = ledge.transform.localScale.x < 0;
            var ledgePos = (Vector2)ledge.transform.position + ledge.offset * ledge.transform.localScale;
    
            fsm.fighter.transform.position = ledgePos + new Vector2(rightLedge ? 0.75f : -0.75f, -0.65f);
            FlippedLeft = rightLedge;
        }
        protected virtual void StaminaRegen(float timeScale)
        {
            Stamina += timeScale * Time.fixedDeltaTime * fsm.entity.NonFreezeTimeScale * 0.25f;
        }
    
        protected virtual void CheckPlatformCondition()
        {
    
            if ((IndieSpd + SelfSpd).y >= 0 || Controlling.moveStick.raw.y < -0.5f || PlatformInvuln > 0 || CheckInPlat())
                PlatformCondition = false;
            else
                PlatformCondition = true;
        }
    
        protected Collider2D CheckInPlat()
        {
            return Physics2D.OverlapBox(fsm.entity.transform.position, (fsm.fighter.boxcastCol as BoxCollider2D).size * fsm.entity.transform.lossyScale.y, 0, fsm.fighter.platLayers);
        }
    
        private void CalculateAirMoveSpeed(float timeScale)
        {
            if ((IndieSpd.x > Ft.airSpd || IndieSpd.x < -Ft.airSpd) && Mathf.Sign(Controlling.moveStick.raw.x) == Mathf.Sign(IndieSpd.x))
            {
                IndieSpd += Vector2.right * IndieSpd.x * Ft.airRes * Mathf.Abs(Controlling.moveStick.raw.x) * timeScale;
            }
            else
            {
                IndieSpd += Vector2.right * Ft.airAcl * Controlling.moveStick.raw.x * timeScale;
            }
    
            if (!moveableVertical)
                return;
            IndieSpd += Vector2.up * Ft.upAcl * Mathf.Clamp(Controlling.moveStick.raw.y, 0, 1) * timeScale;
            if (IndieSpd.y > -Ft.termVelo)
            {
                IndieSpd += Vector2.up * Ft.downAcl * Mathf.Clamp(Controlling.moveStick.raw.y, -1, 0) * timeScale;
            }
        }
    
        public virtual void TransitionGround()
        {
            PlatformInvuln = 0;
            SetState(new GroundedFighterLandState(fsm));
            CheckAndTeleportToGround();
        }
    
        protected void Decelerate()
        {
            if (Aerial && IndieSpd.y > 0)
                IndieSpd -= Ft.decelerate * ((Ft.jumpTime - Ft.shorthopTime) / (JumptimePassed - Ft.shorthopTime + (Ft.jumpTime - Ft.shorthopTime) / 2)) * Vector2.up;
            Jumping = false;
            JumpingTap = false;
            JumpingToggle = false;
        }
    
        protected virtual RaycastHit2D CheckAndTeleportToGround()
        {
            var midHit = Physics2D.Raycast(fsm.entity.transform.position, Vector2.down, (fsm.fighter.boxcastCol as BoxCollider2D).size.y, GroundPlatLayers);
            var leftHit = Physics2D.Raycast(fsm.entity.transform.position + (fsm.fighter.boxcastCol as BoxCollider2D).size.x * 0.5f * Vector3.left * fsm.entity.transform.lossyScale.y, Vector2.down, (fsm.fighter.boxcastCol as BoxCollider2D).size.y * fsm.entity.transform.lossyScale.y, GroundPlatLayers);
            var rightHit = Physics2D.Raycast(fsm.entity.transform.position + (fsm.fighter.boxcastCol as BoxCollider2D).size.x * 0.5f * Vector3.right * fsm.entity.transform.lossyScale.y, Vector2.down, (fsm.fighter.boxcastCol as BoxCollider2D).size.y * fsm.entity.transform.lossyScale.y, GroundPlatLayers);
    
            if (midHit)
            {
                fsm.entity.transform.position = new Vector3(fsm.entity.transform.position.x, midHit.point.y + 0.85f);
                return midHit;
            }
            if (rightHit)
            {
                fsm.entity.transform.position = new Vector3(fsm.entity.transform.position.x + 0.1f, rightHit.point.y + 0.85f);
                return rightHit;
            }
            if (leftHit)
            {
                fsm.entity.transform.position = new Vector3(fsm.entity.transform.position.x - 0.1f, leftHit.point.y + 0.85f);
                return leftHit;
            }
            return new RaycastHit2D();
        }
    }
    
    //====================================================================================================================================================
    
    public class GroundedFighterIdleState : GroundedFighterState
    {
        public GroundedFighterIdleState(FighterSM fsm) : base(fsm) { }
    
        public override void OnStateEnter()
        {
            base.OnStateEnter();
            //OnStateUpdate(0);
        }
    }
    
    public class GroundedFighterLandState : GroundedFighterState
    {
        public GroundedFighterLandState(FighterSM fsm) : base(fsm)
        {
            lag = true;
        }
    
        public override void OnStateEnter()
        {
            base.OnStateEnter();
            Turning = false;
            DashJump = false;
            CheckLedge();
    
            if (AutoCancel)
                fsm.entity.PlayAnim("Ground_Land_NoAC_" + CurrentAttackID);
            else if (SmashAutoCancel)
                fsm.entity.PlayAnim("Ground_Land_Smash_NoAC_" + CurrentAttackID);
            else
                fsm.entity.PlayAnim("Ground_Land");
    
            if (ActState is AirborneFighterHurtHeavyState && !(ActState is AirborneFighterHurtNormalState))
                fsm.entity.PlayAnim("Ground_Land_Bad");
            StunStopTime = 0;
        }
    
        public override void OnStateUpdate(float timeScale)
        {
            base.OnStateUpdate(timeScale);
            if (FrameNum >= 20)
                SetGroundPosition();
        }
    
        private void SetGroundPosition()
        {
            if (Collision == null)
                return;
            if (Collision.Length == 0)
                return;
            foreach (var c in Collision)
            {
                if (c.normal.y > 0.9f && c.collider.gameObject.layer == 10)
                {
                    fsm.entity.transform.position = ((Vector2)c.collider.transform.position + c.collider.offset) * Vector2.up + fsm.entity.transform.position * Vector2.right + 0.83f * Vector2.up;
                    return;
                }
            }
        }
        protected override bool CheckLedge()
        {
            var midHit = Physics2D.Raycast(fsm.entity.transform.position, Vector2.down, (fsm.fighter.boxcastCol as BoxCollider2D).size.y, GroundPlatLayers);
            var leftHit = Physics2D.Raycast(fsm.entity.transform.position + (fsm.fighter.boxcastCol as BoxCollider2D).size.x * 0.5f * Vector3.left * fsm.entity.transform.lossyScale.y, Vector2.down, (fsm.fighter.boxcastCol as BoxCollider2D).size.y * fsm.entity.transform.lossyScale.y, GroundPlatLayers);
            var rightHit = Physics2D.Raycast(fsm.entity.transform.position + (fsm.fighter.boxcastCol as BoxCollider2D).size.x * 0.5f * Vector3.right * fsm.entity.transform.lossyScale.y, Vector2.down, (fsm.fighter.boxcastCol as BoxCollider2D).size.y * fsm.entity.transform.lossyScale.y, GroundPlatLayers);
    
            if ((!rightHit && (SelfSpd + IndieSpd).x > 0) || (!leftHit && (SelfSpd + IndieSpd).x < 0))
            {
                if (midHit)
                    return false;
    
                if (!rightHit)
                    fsm.entity.transform.position -= 0.1f * Vector3.right;
                else if (!leftHit)
                    fsm.entity.transform.position += 0.1f * Vector3.right;
                return false;
            }
            return true;
        }
    }
    
    public class GroundedFighterCrouchState : GroundedFighterState
    {
        public GroundedFighterCrouchState(FighterSM fsm) : base(fsm)
        {
            walkable = false;
            crouchable = false;
        }
    
        public override void OnStateEnter()
        {
            base.OnStateEnter();
            CrouchLag = true;
    
            if ((ActState is GroundedFighterAttackState || (Ft.name == "Marisa" && ActState is Extension.GroundedFighterSpecial2DirectionalShootDownState)) && CurrentAttackID == AttackID.Down)
                return;
    
            if (fsm.entity.CheckPrevAnimIsName("Ground_Land"))
            {
                CrouchLag = false;
                fsm.entity.PlayAnim("Ground_Crouch_Start");
                fsm.entity.SkipCurrentAnimToFrame(80);
            }
            else
                fsm.entity.PlayAnim("Ground_Crouch_Start");
        }
    
        public override void OnStateUpdate(float timeScale)
        {
            base.OnStateUpdate(timeScale);
            CheckNotCrouching();
        }
    
        public override void OnStateExit()
        {
            base.OnStateExit();
            CrouchLag = true;
            fsm.entity.PlayAnim("Ground_Crouch_End");
        }
    
        private void CheckNotCrouching()
        {
            if (Controlling.moveStick.raw.y >= -0.5f && !CrouchLag)
                SetState(new GroundedFighterIdleState(fsm));
        }
    }
    
    public class AirborneFighterIdleState : AirborneFighterState
    {
        public AirborneFighterIdleState(FighterSM fsm) : base(fsm) { }
    
        public override void OnStateEnter()
        {
            base.OnStateEnter();
            Turning = false;
            //OnStateUpdate(0);
        }
    }
    
    public class GroundedFighterWalkState : GroundedFighterState
    {
        public GroundedFighterWalkState(FighterSM fsm) : base(fsm)
        {
            walkable = false;
        }
        public override void OnStateEnter()
        {
            base.OnStateEnter();
    
            var animFrame = FrameNum;
            if (Turning)
            {
                fsm.entity.PlayAnim("Ground_Walk_Turn");
                fsm.entity.SkipCurrentAnimToFrame(animFrame);
                return;
            }
            fsm.entity.PlayAnim("Ground_Walk_Start");
            CheckTurn();
        }
        public override void OnStateExit()
        {
            var animFrame = FrameNum;
            fsm.entity.PlayAnim("Ground_Walk_End");
    
            if (Turning)
                PlayTurnToIdleAnimation(animFrame);
        }
    
        public override void OnStateUpdate(float timeScale)
        {
            CalculateWalkSpeedProgress(timeScale);
    
            CheckTurn();
            CheckNotWalking();
    
            base.OnStateUpdate(timeScale);
        }
    
        private void CheckNotWalking()
        {
            if (!WalkCondition)
                SetState(new GroundedFighterIdleState(fsm));
    
            LastLeftDir = Controlling.moveStick.raw.x < 0;
        }
    
        protected override void TransitionAir()
        {
            IndieSpd += SelfSpd;
            base.TransitionAir();
        }
    
        private void CheckTurn()
        {
            if (MoveStickAgainstFlip(Controlling.moveStick.raw) && !Turning)
            {
                FlippedLeft = Controlling.moveStick.raw.x < 0;
                fsm.entity.PlayAnim("Ground_Walk_Turn");
                Turning = true;
                return;
            }
        }
    
        private void CalculateWalkSpeedProgress(float timeScale)
        {
            var horiInput = Controlling.moveStick.raw.x;
    
            if (Mathf.Abs(WalktimePassed) < Mathf.Abs(horiInput))
                WalktimePassed += Time.fixedDeltaTime * Et.runAcl * Mathf.Sign(horiInput) * timeScale; //Determines how far into the walk animation its in and affects its speed
            WalktimePassed = Mathf.Clamp(WalktimePassed, -1, 1);
    
        }
    }
    
    public class GroundedFighterDashStartState : GroundedFighterState
    {
        public GroundedFighterDashStartState(FighterSM fsm) : base(fsm)
        {
            walkable = false;
            crouchable = false;
            dashable = false;
        }
    
        public override void OnStateEnter()
        {
            base.OnStateEnter();
            fsm.entity.PlayAnim("Ground_Dash_Start");
    
            var stickRecord = Controlling.moveStick.tapInitPos;
            if (ToggleDash)
                stickRecord = Controlling.dashToggleButton.stickAccompanimentOnTap;
    
            if (MoveStickAgainstFlip(Controlling.moveStick.raw) || Turning)
            {
                fsm.entity.PlayAnim("Ground_Dash_Start_Back");
                FlippedLeft = stickRecord.x < 0;
            }
            StopDash = false;
        }
    
        public override void OnStateUpdate(float timeScale)
        {
            WalktimePassed = 0;
            CalculateDashSpd(timeScale);
    
            CheckTurns();
    
            CheckStickNotHeld();
    
            base.OnStateUpdate(timeScale);
        }
        private void CheckStickNotHeld()
        {
            if (Mathf.Abs(Controlling.moveStick.raw.x) <= 0.5f || MoveStickAgainstFlip(Controlling.moveStick.raw))
                StopDash = true;
        }
    
        private void CalculateDashSpd(float timeScale)
        {
            IndieSpd = new Vector2((FlippedLeft ? -1 : 1) * Ft.dashSpd, IndieSpd.y);
        }
    }
    
    public class GroundedFighterDashIdleState : GroundedFighterState
    {
        public GroundedFighterDashIdleState(FighterSM fsm) : base(fsm)
        {
            walkable = false;
            crouchable = false;
            dashable = false;
        }
    
        public override void OnStateUpdate(float timeScale)
        {
            WalktimePassed = 0;
    
            CalculateDashSpeed(timeScale);
            CheckTurns();
            CheckDashStop();
    
            base.OnStateUpdate(timeScale);
        }
    
        private void CheckDashStop()
        {
            if (Mathf.Abs(Controlling.moveStick.raw.x) < 0.5f || MoveStickAgainstFlip(Controlling.moveStick.raw))
                SetState(new GroundedFighterDashStopState(fsm));
        }
    
        private void CalculateDashSpeed(float timeScale)
        {
            IndieSpd = new Vector2((FlippedLeft ? -1 : 1) * (Ft.dashSpd - 1), IndieSpd.y);
        }
    }
    
    public class GroundedFighterDashStopState : GroundedFighterState
    {
        public GroundedFighterDashStopState(FighterSM fsm) : base(fsm)
        {
            walkable = false;
            crouchable = false;
            dashable = false;
        }
        public override void OnStateEnter()
        {
            base.OnStateEnter();
            fsm.entity.PlayAnim("Ground_Dash_End");
        }
        public override void OnStateUpdate(float timeScale)
        {
            CheckTurns();
    
            base.OnStateUpdate(timeScale);
        }
    
        public override void OnStateExit()
        {
            base.OnStateExit();
            if (Turning)
                PlayTurnToIdleAnimation(0);
        }
    }
    
    public class GroundedFighterJumpState : GroundedFighterState
    {
        public GroundedFighterJumpState(FighterSM fsm) : base(fsm)
        {
            lag = true;
        }
    
        public override void OnStateEnter()
        {
            base.OnStateEnter();
            CheckDashJump();
            if (Turning && ActState is GroundedFighterDashStartState)
            {
                FlippedLeft = !FlippedLeft;
                Turning = false;
            }
            JumptimePassed = 0;
    
            fsm.entity.PlayAnim("Ground_Jump_Start");
        }
    
        private void CheckDashJump()
        {
            DashJump = false;
    
            if (ActState is GroundedFighterDashStartState)
                IndieSpd -= Vector2.right * Mathf.Sign(IndieSpd.x) * 2;
            if (ActState is GroundedFighterDashIdleState || ActState is GroundedFighterDashStartState || (ActState is GroundedFighterDashStopState && FrameNum <= 10))
                DashJump = true;
        }
    
        protected override void TransitionAir()
        {
            SetState(new AirborneFighterJumpState(fsm));
            if (IndieSpd.y <= 1)
                fsm.fighter.Jump();
        }
    }
    
    public class AirborneFighterJumpState : AirborneFighterState
    {
        public AirborneFighterJumpState(FighterSM fsm) : base(fsm) { }
    
        public override void OnStateEnter()
        {
            base.OnStateEnter();
            JumptimePassed = 0;
            fsm.entity.PlayAnim("Air_Jump_Forward");
            if (MoveStickAgainstFlip(Controlling.moveStick.raw) || (IndieSpd.x < 0 != FlippedLeft && Mathf.Abs(IndieSpd.x) > 1))
                fsm.entity.PlayAnim("Air_Jump_Backward");
    
            Turning = false;
    
            SetState(new AirborneFighterIdleState(fsm));
            IndieSpd += ExternalSpd;
            ExternalSpd = Vector2.zero;
        }
    }
    
    public class AirborneFighterAirdashState : AirborneFighterState
    {
        protected virtual bool AirdashStopInputCondition { get => !Controlling.jumpButton.raw; }
    
        public AirborneFighterAirdashState(FighterSM fsm) : base(fsm)
        {
            staminaRegenable = false;
            airDashable = false;
        }
    
        public override void OnStateEnter()
        {
            base.OnStateEnter();
            Stamina -= 1;
            SetAirDashDirection();
            Orientate(AirdashDir);
    
            SetAirdashDownSpeed();
    
            Gravity = false;
            AirdashCancelable = false;
            AirdashTimePassed = 0;
            JumptimePassed = 1;
    
            if (fsm.fighter.extension is not YuugiFighterExtension)
                IndieSpd = AirdashDir * (Ft.airdashSpd + AirdashDownSpeed * AirdashDir.y);
            fsm.entity.PlayAnim("Air_Airdash_Start");
        }
    
        public override void OnStateUpdate(float timeScale)
        {
            IndieSpd = AirdashDir * (Ft.airdashSpd + AirdashDownSpeed * AirdashDir.y);
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
    
        private void SetAirdashDownSpeed()
        {
            AirdashDownSpeed = 0;
            if (IndieSpd.y < 0 && AirdashDir.y < 0)
            {
                AirdashDownSpeed = IndieSpd.y / 2;
            }
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
            bool completeCondition = (AirdashCancelable && AirdashStopInputCondition) || AirdashTimePassed >= Ft.airdashTime;
            if (completeCondition || (PlatformCondition && IsReachingGround(fsm.fighter.platLayers)))
            {
                SetState(new AirborneFighterAirdashStopState(fsm));
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
    
    public class AirborneFighterAirdashTapState : AirborneFighterAirdashState
    {
        protected override bool AirdashStopInputCondition { get => Controlling.moveStick.raw.magnitude <= 0.75f || Vector2.Dot(Controlling.moveStick.raw, AirdashDir) < 0.5f; }
    
        public AirborneFighterAirdashTapState(FighterSM fsm) : base(fsm)
        {
            airDashable = false;
        }
    
        protected override Vector2 GetDirection()
        {
            return Controlling.moveStick.tapInitPos;
        }
    }
    public class AirborneFighterAirdashToggleState : AirborneFighterAirdashState
    {
        protected override bool AirdashStopInputCondition { get => Controlling.moveStick.raw.magnitude <= 0.75f || !Controlling.airdashToggleButton.raw || Vector2.Dot(Controlling.moveStick.raw, AirdashDir) < 0.5f; }
    
        public AirborneFighterAirdashToggleState(FighterSM fsm) : base(fsm)
        {
            airDashable = false;
        }
    
        protected override Vector2 GetDirection()
        {
            return Controlling.airdashToggleButton.stickAccompanimentOnTap;
        }
    }
    
    public class AirborneFighterAirdashStopState : AirborneFighterState
    {
        public AirborneFighterAirdashStopState(FighterSM fsm) : base(fsm)
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
    
    public class AirborneFighterGroundToAirState : AirborneFighterState
    {
        public AirborneFighterGroundToAirState(FighterSM fsm) : base(fsm)
        {
            airDashable = false;
        }
        public override void OnStateEnter()
        {
            base.OnStateEnter();
            fsm.entity.PlayAnim("Air_Transition");
            SetState(new AirborneFighterIdleState(fsm));
        }
    }
    
    public class AirborneFighterFocusSpinState : AirborneFighterState
    {
        public AirborneFighterFocusSpinState(FighterSM fsm) : base(fsm)
        {
            lag = true;
            moveable = false;
        }
    
        public override void OnStateEnter()
        {
            base.OnStateEnter();
            fsm.entity.PlayAnim("Air_Focus_Spin");
        }
    
        public override void OnStateUpdate(float timeScale)
        {
            base.OnStateUpdate(timeScale);
            if (!Turning)
                CheckTurns();
            IndieSpd += (IndieSpd.x * Et.airRes * Vector2.right) * timeScale;
        }
        public override void OnStateExit()
        {
            base.OnStateEnter();
            if (Turning)
                FlippedLeft = !FlippedLeft;
            Turning = false;
        }
    }
    
    public class AirborneFighterFocusSpinEndState : AirborneFighterState
    {
        public AirborneFighterFocusSpinEndState(FighterSM fsm) : base(fsm)
        {
            airDashable = false;
            focusable = false;
        }
    }
    
    public class AirborneFighterFocusState : AirborneFighterState
    {
        public AirborneFighterFocusState(FighterSM fsm) : base(fsm)
        {
            lag = true;
            moveable = false;
            staminaRegenable = false;
        }
    
        public override void OnStateEnter()
        {
            base.OnStateEnter();
            fsm.entity.PlayAnim("Air_Focus_Start");
            FocusTimePassed = 0;
            fsm.entity.physicsState.SetState(new Focus(fsm.entity.physicsState));
        }
    
        public override void OnStateUpdate(float timeScale)
        {
            base.OnStateUpdate(timeScale);
            FocusTimePassed += Time.fixedDeltaTime * timeScale;
            Stamina -= Time.fixedDeltaTime * timeScale * Ft.focusTime;
            CalculateFocusSpd(timeScale);
            CheckNotFocusing();
        }
    
        public override void OnStateExit()
        {
            base.OnStateExit();
            FocusTimePassed = 0;
            fsm.entity.PlayAnim("Air_Focus_End");
            if (Aerial)
                fsm.entity.physicsState.SetState(new InAir(fsm.entity.physicsState));
        }
    
        private void CheckNotFocusing()
        {
            if (FreezeTime > 0 || fsm.entity.TrueTimeScale <= 0)
                return;
            if (!Controlling.shieldButton.raw || Stamina <= 0)
                SetState(new AirborneFighterFocusSpinEndState(fsm));
        }
    
        public virtual void CalculateFocusSpd(float timeScale)
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
    
    public class GroundedFighterShieldBreakState : GroundedFighterState
    {
        public GroundedFighterShieldBreakState(FighterSM fsm) : base(fsm)
        {
            lag = true;
        }
    
        public override void OnStateEnter()
        {
            base.OnStateEnter();
            IndieSpd *= 0;
            fsm.entity.PlayAnim("Ground_Shield_Break_Start");
            if (MaxStamina > 1)
                MaxStamina -= 1;
            fsm.fighter.shield.stateVars.percent = fsm.fighter.shield.et.health;
        }
    
        public override void OnStateUpdate(float timeScale)
        {
            base.OnStateUpdate(timeScale);
            CheckLedge();
            fsm.fighter.shield.stateVars.percent -= Time.fixedDeltaTime * timeScale * 5f;
            if (fsm.fighter.shield.stateVars.percent <= 0)
                SetState(new GroundedFighterShieldBreakEndState(fsm));
        }
    }
    public class GroundedFighterShieldBreakEndState : GroundedFighterState
    {
        public GroundedFighterShieldBreakEndState(FighterSM fsm) : base(fsm)
        {
            lag = true;
        }
    
        public override void OnStateEnter()
        {
            base.OnStateEnter();
            fsm.entity.PlayAnim("Ground_Shield_Break_End");
            fsm.fighter.shield.stateVars.percent = 0;
        }
    }
    
    public class GroundedFighterShieldState : GroundedFighterState
    {
        protected bool RollCondition { get => TapDash || ToggleDash; }
    
        public GroundedFighterShieldState(FighterSM fsm) : base(fsm)
        {
            walkable = false;
            crouchable = false;
            shieldable = false;
            specialable = false;
            jumpable = false;
            dashable = false;
        }
    
        public override void OnStateEnter()
        {
            base.OnStateEnter();
            WalktimePassed = 0;
            CheckRolling();
        }
    
        public override void OnStateUpdate(float timeScale)
        {
            base.OnStateUpdate(timeScale);
            fsm.fighter.shield.stateVars.percent += Time.fixedDeltaTime * timeScale * 5f * 2;
            CheckLedge();
            ApplyGroundFriction(timeScale);
            CheckRolling();
            CheckNotShielding();
            if (fsm.fighter.shield.stateVars.percent >= fsm.fighter.shield.et.health)
                SetState(new GroundedFighterShieldBreakState(fsm));
        }
    
        protected void ApplyGroundFriction(float timeScale)
        {
            IndieSpd -= timeScale * (IndieSpd.x * Et.groundRes * 1.5f * Vector2.right);
        }
    
        protected virtual void CheckNotShielding()
        {
            if (FreezeTime > 0 || fsm.entity.TrueTimeScale <= 0)
                return;
            if (!Controlling.shieldButton.raw && FreezeTime <= 0)
            {
                SetState(new GroundedFighterShieldEndState(fsm));
            }
        }
        protected bool CheckRolling()
        {
            if (RollCondition && rollable && FreezeTime <= 0)
            {
                Turning = false;
                RollDir = 0;
    
                var rollStick = ToggleDash ? Controlling.dashToggleButton.stickAccompanimentOnTap : Controlling.moveStick.tapInitPos;
    
                if (Mathf.Abs(rollStick.x) > 0.5f)
                    RollDir = (int)Mathf.Sign(rollStick.x);
    
                SetState(new GroundedFighterRollStartState(fsm));
                ToggleConsume();
                Controlling.moveStick.ConsumeBufferTap();
                Controlling.moveStick.ConsumeBufferHold();
                return true;
            }
            return false;
        }
    }
    
    public class GroundedFighterPerfectShieldState : GroundedFighterShieldState
    {
        public GroundedFighterPerfectShieldState(FighterSM fsm) : base(fsm)
        {
            walkable = false;
            crouchable = false;
            shieldable = false;
            specialable = false;
            jumpable = false;
            dashable = false;
        }
        public override void OnStateEnter()
        {
            fsm.entity.PlayAnim("Ground_Shield_Start");
            base.OnStateEnter();
        }
    
        protected override void CheckNotShielding()
        {
            if (!Controlling.shieldButton.raw)
            {
                if (FreezeTime <= 0)
                {
                    SetState(new GroundedFighterShieldEndState(fsm));
                    return;
                }
                SetState(new GroundedFighterPerfectShieldEndState(fsm));
                FreezeTime = 0;
            }
        }
    }
    
    public class GroundedFighterShieldEndState : GroundedFighterState
    {
        public GroundedFighterShieldEndState(FighterSM fsm) : base(fsm)
        {
            lag = true;
        }
    
        public override void OnStateEnter()
        {
            base.OnStateEnter();
            fsm.entity.PlayAnim("Ground_Shield_End");
        }
    
        public override void OnStateUpdate(float timeScale)
        {
            base.OnStateUpdate(timeScale);
            ApplyGroundFriction(timeScale);
        }
    
        protected void ApplyGroundFriction(float timeScale)
        {
            IndieSpd -= timeScale * (IndieSpd.x * Et.groundRes * 1.5f * Vector2.right);
        }
    }
    public class GroundedFighterThrowTechState : GroundedFighterState
    {
        public GroundedFighterThrowTechState(FighterSM fsm) : base(fsm)
        {
            lag = true;
        }
    
        public override void OnStateEnter()
        {
            base.OnStateEnter();
            fsm.entity.PlayAnim("Ground_Shield_End");
            IndieSpd = (FlippedLeft ? 5f : -5f) * Vector2.right;
        }
    }
    
    public class GroundedFighterPerfectShieldEndState : GroundedFighterShieldEndState
    {
        public GroundedFighterPerfectShieldEndState(FighterSM fsm) : base(fsm)
        {
            lag = true;
        }
    
        public override void OnStateEnter()
        {
            base.OnStateEnter();
            fsm.entity.PlayAnim("Ground_Shield_End_Perfect");
        }
    }
    
    public class GroundedFighterTechStartState : GroundedFighterState
    {
        public GroundedFighterTechStartState(FighterSM fsm) : base(fsm)
        {
            lag = true;
        }
    
        public override void OnStateEnter()
        {
            base.OnStateEnter();
            WalktimePassed = 0;
            WindSpd *= 0;
            fsm.entity.PlayAnim("Ground_Tech");
            CheckRollDir();
        }
        public override void OnStateUpdate(float timeScale)
        {
            base.OnStateUpdate(timeScale);
            CheckLedge();
        }
    
        private void CheckRollDir()
        {
            if (RollDir != 0)
                fsm.entity.PlayAnim("Ground_Tech_Forward");
            if (MoveStickAgainstFlip(Vector2.right * RollDir))
                fsm.entity.PlayAnim("Ground_Tech_Back");
        }
    }
    
    public class GroundedFighterRollStartState : GroundedFighterState
    {
        public GroundedFighterRollStartState(FighterSM fsm) : base(fsm)
        {
            lag = true;
        }
    
        public override void OnStateEnter()
        {
            base.OnStateEnter();
            WalktimePassed = 0;
            WindSpd *= 0;
            fsm.entity.PlayAnim("Ground_Roll");
            CheckRollDir();
        }
        public override void OnStateUpdate(float timeScale)
        {
            base.OnStateUpdate(timeScale);
            CheckLedge();
        }
    
        private void CheckRollDir()
        {
            if (RollDir != 0)
                fsm.entity.PlayAnim("Ground_Roll_Forward");
            if (MoveStickAgainstFlip(Vector2.right * RollDir))
                fsm.entity.PlayAnim("Ground_Roll_Backward");
        }
    }
    
    public class GroundedFighterRollLagState : GroundedFighterState
    {
        public GroundedFighterRollLagState(FighterSM fsm) : base(fsm)
        {
            lag = true;
        }
    }
    
    public class GroundedFighterRollMoveState : GroundedFighterState
    {
        public GroundedFighterRollMoveState(FighterSM fsm) : base(fsm)
        {
            lag = true;
        }
        public override void OnStateUpdate(float timeScale)
        {
            base.OnStateUpdate(timeScale);
            IndieSpd = new Vector2(Ft.rollSpd * RollDir, IndieSpd.y);
    
            CheckLedge();
        }
    }
    public class GroundedFighterAttackState : GroundedFighterState
    {
        private bool DashAttackCondition { get => (DashCondition || ActState is GroundedFighterDashIdleState || ActState is GroundedFighterDashStartState) && !StopDash; }
        private bool Grab { get => ShieldCondition || ActState is GroundedFighterShieldState || Controlling.grabButton.Buffer(); }
        private bool Smash => (TapSmash || StickSmash || Controlling.smashButton.Buffer() || Controlling.smashToggleButton.raw) && GrazeMeter >= 5;
    
        public GroundedFighterAttackState(FighterSM fsm) : base(fsm)
        {
            lag = true;
        }
    
        public override void OnStateEnter()
        {
            base.OnStateEnter();
            fsm.fighter.ClearAttackHits();
            WalktimePassed = 0;
    
            var attackDir = GetAttackDir(Controlling.attackButton, Controlling.cStick);
            if (Grab)
            {
                SetState(new GroundedFighterGrabState(fsm));
                return;
            }
    
            if (DashAttackCondition && !Smash)
            {
                fsm.entity.PlayAnim("Ground_Attack_Tilt_Dash");
                return;
            }
    
            if (!Smash)
                fsm.entity.PlayAnim("Ground_Attack_Tilt_" + ConvertAttackID(attackDir).ToString());
            else
                fsm.entity.PlayAnim("Ground_Attack_Smash_" + ConvertAttackID(attackDir).ToString() + "_Charge");
        }
    
        public override void OnStateUpdate(float timeScale)
        {
            base.OnStateUpdate(timeScale);
            CheckLedge();
        }
    
        private AttackID ConvertAttackID(Vector2 attackDir)
        {
            if (MoveStickAgainstFlip(attackDir))
                FlippedLeft = !FlippedLeft;
    
            var id = GetAttackID(attackDir);
            if (id == AttackID.Back || (Smash && id == AttackID.Neutral))
                id = AttackID.Forward;
    
            CurrentAttackID = id;
    
            return id;
        }
    }
    
    public class GroundedFighterAttackThrowState : GroundedFighterState
    {
        public GroundedFighterAttackThrowState(FighterSM fsm) : base(fsm)
        {
            lag = true;
        }
    
        public override void OnStateEnter()
        {
            base.OnStateEnter();
            fsm.entity.PlayAnim("Ground_Attack_Throw");
        }
    }
    public class GroundedFighterAttackComboState : GroundedFighterState
    {
        private readonly string attack;
        public GroundedFighterAttackComboState(FighterSM fsm, string attack) : base(fsm)
        {
            lag = true;
            this.attack = attack;
        }
    
        public override void OnStateUpdate(float timeScale)
        {
            base.OnStateEnter();
            if (Controlling.attackButton.raw)
            {
                fsm.entity.PlayAnim(attack);
                SetState(new GroundedFighterAttackEndState(fsm));
            }
        }
    }
    public class GroundedFighterAttackEndState : GroundedFighterState
    {
        public GroundedFighterAttackEndState(FighterSM fsm) : base(fsm)
        {
            lag = true;
        }
    }
    
    public class GroundedFighterAttackSmashChargeState : GroundedFighterState
    {
        public GroundedFighterAttackSmashChargeState(FighterSM fsm) : base(fsm)
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
            AttackCharge += timeScale * Time.fixedDeltaTime;
            if (AttackCancelCondition)
                SetState(new GroundedFighterAttackSmashState(fsm));
        }
    }
    public class GroundedFighterAttackSmashState : GroundedFighterState
    {
        public GroundedFighterAttackSmashState(FighterSM fsm) : base(fsm)
        {
            lag = true;
        }
        public override void OnStateEnter()
        {
            base.OnStateEnter();
            fsm.fighter.ClearAttackHits();
            fsm.entity.PlayAnim("Ground_Attack_Smash_" + CurrentAttackID.ToString());
        }
    }
    
    public class GroundedFighterGrabState : GroundedFighterState
    {
        public GroundedFighterGrabState(FighterSM fsm) : base(fsm)
        {
            lag = true;
        }
    
        public override void OnStateEnter()
        {
            base.OnStateEnter();
            fsm.fighter.ClearAttackHits();
            WalktimePassed = 0;
    
            fsm.entity.PlayAnim("Ground_Attack_Grab");
        }
        public override void OnStateUpdate(float timeScale)
        {
            base.OnStateUpdate(timeScale);
            CheckLedge();
        }
    }
    
    public class AirborneFighterAttackState : AirborneFighterState
    {
        public AirborneFighterAttackState(FighterSM fsm) : base(fsm)
        {
            lag = true;
        }
    
        public override void OnStateEnter()
        {
            base.OnStateEnter();
            fsm.fighter.ClearAttackHits();
    
            var attackDir = GetAttackDir(Controlling.attackButton, Controlling.cStick);
    
            CurrentAttackID = GetAttackID(attackDir);
    
            fsm.entity.PlayAnim("Air_Attack_Tilt_" + GetAttackID(attackDir).ToString());
        }
        public override void OnStateExit()
        {
            base.OnStateExit();
            fsm.fighter.ClearAttackHits();
        }
    }
    
    public class AirborneFighterRespawnState : AirborneFighterState
    {
        public AirborneFighterRespawnState(FighterSM fsm) : base(fsm)
        {
            lag = true;
            moveable = false;
        }
    
        public override void OnStateEnter()
        {
            base.OnStateEnter();
            Percent = 0;
            SpawnInvulnTimer = 3;
            MaxStamina = 4;
            fsm.entity.PlayAnim("Air_Respawning");
        }
    
        public override void OnStateUpdate(float timeScale)
        {
            base.OnStateUpdate(timeScale);
            SpawnInvulnTimer = 3;
            IndieSpd = new Vector2(Controlling.moveStick.raw.x * 3, -35 + Controlling.moveStick.raw.y * 10);
        }
    
        public override void TransitionGround()
        {
            if (ActState is AirborneFighterRespawnState)
                SetState(new GroundedFighterRespawnLandState(fsm));
        }
    
        protected override void CheckPlatformCondition()
        {
            PlatformCondition = false;
        }
    }
    
    public class GroundedFighterRespawnLandState : GroundedFighterState
    {
        public GroundedFighterRespawnLandState(FighterSM fsm) : base(fsm)
        {
            lag = true;
        }
        public override void OnStateEnter()
        {
            base.OnStateEnter();
            Stamina = MaxStamina;
            SpawnInvulnTimer = 3;
            IndieSpd *= Vector2.up;
            fsm.entity.PlayAnim("Ground_Respawn_Land");
        }
    }
    
    public class GroundedFighterDeadState : AirborneFighterState
    {
        public GroundedFighterDeadState(FighterSM fsm) : base(fsm)
        {
            lag = true;
            moveable = false;
        }
        public override void OnStateEnter()
        {
            base.OnStateEnter();
            fsm.entity.PlayAnim("Air_Dead");
            ResetPhysicsStates();
            StunStopTime = 0;
            if (!MatchManager.training)
                Stocks--;
            Stamina = 0;
            RotateHurtbox(Vector2.up);
            SpecialAim *= Vector2.zero;
            SpecialCharge = 0;
            SpecialTimer = 0;
            AttackCharge = 0;
            if (SpellcardState)
                GrazeMeter = 0;
            SpellcardState = false;
            IndieSpd *= 0;
            WindSpd *= 0;
            if (Stocks > 0)
                fsm.fighter._respawnWarning.entity.actionState.SetState(new StandbyState(fsm.fighter._respawnWarning.entity.actionState as ControllableSM));
        }
    
        public override void TransitionGround() { }
    }
    public class GroundedFighterTauntState : GroundedFighterState
    {
        public GroundedFighterTauntState(FighterSM fsm) : base(fsm)
        {
            lag = true;
        }
    
        public override void OnStateEnter()
        {
            base.OnStateEnter();
            WalktimePassed = 0;
    
            var attackDir = GetAttackDir(Controlling.tauntButton, Controlling.cStick);
    
            fsm.entity.PlayAnim("Ground_Taunt_" + ConvertAttackID(attackDir).ToString());
        }
    
        private AttackID ConvertAttackID(Vector2 attackDir)
        {
            if (MoveStickAgainstFlip(attackDir))
                FlippedLeft = !FlippedLeft;
    
            var id = GetAttackID(attackDir);
            if (id == AttackID.Back || id == AttackID.Neutral)
                id = AttackID.Forward;
    
            CurrentAttackID = id;
    
            return id;
        }
    }
    
    public class AirborneFighterDropState : AirborneFighterIdleState
    {
        public AirborneFighterDropState(FighterSM fsm) : base(fsm) { }
    
        public override void OnStateEnter()
        {
            base.OnStateEnter();
            PlatformInvuln = 0.3f;
            PlatformCondition = false;
            IndieSpd += ExternalSpd;
            ExternalSpd = Vector2.zero;
            fsm.entity.PlayAnim("Air_Drop");
        }
        public override void TransitionGround() { }
    }
    
    public class AirborneFighterHurtFreezeState : AirborneFighterState
    {
        public AirborneFighterHurtFreezeState(FighterSM fsm) : base(fsm)
        {
            lag = true;
            moveable = false;
        }
    
        public override void OnStateEnter()
        {
            base.OnStateEnter();
            RotateHurtbox(Vector2.up);
            fsm.entity.physicsState.SetState(new Freeze(fsm.entity.physicsState));
            CheckPlatformCondition();
            ResetStateVars();
            FaceAgainstKnockbackDirection();
    
            fsm.entity.PlayAnim("Air_Hurt_Normal_Start");
        }
    
        private void FaceAgainstKnockbackDirection()
        {
            if (Mathf.Abs(IndieSpd.x) > 3)
                FlippedLeft = IndieSpd.x > 0;
        }
    
        public override void OnStateUpdate(float timeScale)
        {
            base.OnStateEnter();
            CheckExitFreezeState();
        }
    
        protected virtual void CheckExitFreezeState()
        {
            if (FreezeTime <= 0)
            {
                fsm.entity.stateVars.lastLaunchSpd = Vector2.zero;
                if (IndieSpd.magnitude > 20)
                    SetState(new AirborneFighterHurtHeavyState(fsm));
                else
                    SetState(new AirborneFighterHurtNormalState(fsm));
            }
        }
    
        public override void TransitionGround()
        { }
    
        protected override void CheckPlatformCondition()
        {
            if ((IndieSpd + SelfSpd).y >= 0)
                PlatformCondition = false;
            else
                PlatformCondition = true;
        }
    }
    
    public class AirborneFighterHurtFreezeIntoForcedHurtHeavyState : AirborneFighterHurtFreezeState
    {
        public AirborneFighterHurtFreezeIntoForcedHurtHeavyState(FighterSM fsm) : base(fsm)
        {
            lag = true;
            moveable = false;
        }
        protected override void CheckExitFreezeState()
        {
            if (FreezeTime <= 0)
            {
                fsm.entity.stateVars.lastLaunchSpd = Vector2.zero;
                SetState(new AirborneFighterHurtHeavyState(fsm));
            }
        }
    }
    
    public class AirborneFighterHurtHeavyState : AirborneFighterState
    {
        protected bool TechCondition => TechCooldown < 0 && IndieSpd.magnitude < 50 && FrameNum > 40;
    
        public AirborneFighterHurtHeavyState(FighterSM fsm) : base(fsm)
        {
            lag = true;
            moveable = false;
        }
    
        public override void OnStateEnter()
        {
            base.OnStateEnter();
            TechCooldown = 1f;
            ExternalSpd *= 0;
            CheckPlatformCondition();
            CalculateDirectionalInfluence();
            fsm.entity.physicsState.SetState(new Launched(fsm.entity.physicsState));
    
            PlayHurtHeavyAnimation();
            if (ContinueCollision && Vector2.Dot(IndieSpd.normalized, fsm.entity.GetTotalContactNormals()) < 0)
                PlayCrashAnimation();
        }
    
        public override void OnStateUpdate(float timeScale)
        {
            TechCooldownUpdate(timeScale);
            if (CheckStopStun())
                return;
            LaunchSpeedInfluence();
            StunStopTime -= timeScale * Time.fixedDeltaTime * 0.8f;
            if (EnteredCollision)
            {
                PlayCrashAnimation();
                CheckTech();
            }
            if (TechCooldown >= 0)
                AlignHurtboxOrientationToLaunchAngle();
    
            base.OnStateUpdate(timeScale);
        }
    
        public override void OnStateExit()
        {
            base.OnStateExit();
            RotateHurtbox(Vector2.up);
        }
    
        private void TechCooldownUpdate(float timeScale)
        {
            if (TechCooldown < 0.3f)
                TechCooldown += timeScale * Time.fixedDeltaTime;
            if (Controlling.shieldButton.Buffer() && TechCooldown >= 0.3f && IndieSpd.magnitude < 50 && FrameNum > 40)
                TechCooldown = -0.2f;
            if (Controlling.shieldButton.Buffer())
                Controlling.shieldButton.ConsumeBuffer();
        }
    
        private void CheckTech()
        {
            if (TechCondition)
            {
                RollDir = 0;
                IndieSpd *= 0;
                StunStopTime = 0;
                if (Mathf.Abs(Controlling.moveStick.raw.x) > 0.5f)
                    RollDir = (int)Mathf.Sign(Controlling.moveStick.raw.x);
    
                if (Aerial)
                {
                    Vector2 normal = fsm.entity.GetTotalContactNormals();
                    fsm.entity.PlayAnim("Air_Hurt_Heavy_End");
                    if (Mathf.Abs(normal.x) > 0.9f && Vector2.Dot(normal, Controlling.moveStick.raw) > 0.3f && Controlling.moveStick.raw.magnitude > 0.5f && Controlling.moveStick.raw.y > -0.3f)
                    {
                        IndieSpd = new Vector2((normal.x > 0) ? 6 : -6, fsm.fighter.Ft.jumpHeight / 2);
                        fsm.entity.PlayAnim("Air_Jump_Forward");
                    }
                    SetState(new AirborneFighterIdleState(fsm));
                }
                else
                    SetState(new GroundedFighterTechStartState(fsm));
    
                ResetPhysicsStates();
            }
        }
    
        private void AlignHurtboxOrientationToLaunchAngle()
        {
            bool flyingHorizontally = fsm.entity.CheckPrevAnimIsName("Air_Hurt_Heavy_Idle") || fsm.entity.CheckPrevAnimIsName("Air_Hurt_Heavy_Start");
            bool crashedHorizontally = fsm.entity.CheckPrevAnimIsName("Air_Hurt_Crash_Idle") || fsm.entity.CheckPrevAnimIsName("Air_Hurt_Crash_Start");
            if (flyingHorizontally || crashedHorizontally)
            {
                Vector3 FlyingDir = (Quaternion.AngleAxis(FlippedLeft ^ crashedHorizontally ? 90 : -90, Vector3.forward) * IndieSpd).normalized;
                if (ContinueCollision && Vector2.Dot(IndieSpd.normalized, fsm.entity.GetTotalContactNormals()) < 0)
                    FlyingDir = (Quaternion.AngleAxis(FlippedLeft ? 90 : -90, Vector3.forward) * -fsm.entity.GetTotalContactNormals()).normalized;
                RotateHurtbox(FlyingDir);
            }
            else
                RotateHurtbox(Vector2.up);
        }
    
        protected virtual void PlayHurtHeavyAnimation()
        {
            fsm.entity.PlayAnim("Air_Hurt_Heavy_Start");
            if (IndieSpd.normalized.y > 0.95f)
                fsm.entity.PlayAnim("Air_Hurt_Heavy_Start_Up");
            if (IndieSpd.normalized.y < -0.95f)
                fsm.entity.PlayAnim("Air_Hurt_Heavy_Start_Down");
        }
    
        public static System.Action<Vector2, Vector2, float> CrashEvent;
        public static System.Action<Vector2, Vector2, float> TechEvent;
        protected virtual void PlayCrashAnimation()
        {
            var normal = fsm.entity.GetTotalContactNormals();
            bool checkNotPlatformSides = !(Collision[0].collider.gameObject.layer == 10 && normal.y < 0.9f);
            if (checkNotPlatformSides)
            {
                FreezeTime += Time.fixedDeltaTime * 2f;
                if (TechCondition)
                {
                    if (TechEvent != null)
                        TechEvent.Invoke(normal, fsm.entity.GetAverageContactPoint(), IndieSpd.magnitude);
                }
                else if (CrashEvent != null)
                    CrashEvent.Invoke(normal, fsm.entity.GetAverageContactPoint(), IndieSpd.magnitude);
                fsm.entity.PlayAnim("Air_Hurt_Crash_Start");
                normal = fsm.entity.GetTotalContactNormals().normalized;
    
                if (Mathf.Abs(normal.x) > 0.1f)
                    FlippedLeft = normal.x < 0;
    
                if (normal.y >= 0.9f)
                    fsm.entity.PlayAnim("Air_Hurt_Crash_Start_Up");
                if (normal.y <= -0.9f)
                    fsm.entity.PlayAnim("Air_Hurt_Crash_Start_Down");
            }
        }
    
        private void LaunchSpeedInfluence()
        {
            IndieSpd = IndieSpd.magnitude * (IndieSpd.normalized + Controlling.moveStick.raw * Mathf.Abs(Controlling.moveStick.raw.y * IndieSpd.normalized.x - Controlling.moveStick.raw.x * IndieSpd.normalized.y) * 0.01f).normalized;
        }
    
        protected virtual bool CheckStopStun()
        {
            if (StunStopTime <= 0)
            {
                ResetPhysicsStates();
                StunStopTime = 0;
                fsm.entity.PlayAnim("Air_Hurt_Heavy_End");
                SetState(new AirborneFighterIdleState(fsm));
                return true;
            }
            return false;
        }
    
        private void CalculateDirectionalInfluence()
        {
            if (IndieSpd.y < -10 && !Aerial)
                return;
    
            float DI = Mathf.Abs(Controlling.moveStick.raw.y * IndieSpd.normalized.x - Controlling.moveStick.raw.x * IndieSpd.normalized.y);
            IndieSpd = (IndieSpd.normalized + DI * Controlling.moveStick.raw * 0.25f).normalized * IndieSpd.magnitude;
        }
    
        public override void TransitionGround()
        {
            if (!fsm.fighter.BounceCondition && TechCooldown > 0)
                base.TransitionGround();
        }
    
        protected override void CheckPlatformCondition()
        {
            if ((IndieSpd + SelfSpd).y >= 0)
                PlatformCondition = false;
            else
                PlatformCondition = true;
        }
    }
    
    public class AirborneFighterHurtNormalState : AirborneFighterHurtHeavyState
    {
        public AirborneFighterHurtNormalState(FighterSM fsm) : base(fsm)
        {
            lag = true;
            moveable = false;
        }
    
        public override void OnStateEnter()
        {
            base.OnStateEnter();
            fsm.entity.physicsState.SetState(new LaunchedMini(fsm.entity.physicsState));
        }
        protected override void PlayHurtHeavyAnimation() { }
        protected override void PlayCrashAnimation() { }
    
        protected override bool CheckStopStun()
        {
            if (StunStopTime <= 0)
            {
                ResetPhysicsStates();
                StunStopTime = 0;
                fsm.entity.PlayAnim("Air_Hurt_Normal_End");
                SetState(new AirborneFighterIdleState(fsm));
                return true;
            }
            return false;
        }
        public override void TransitionGround()
        {
            PlatformInvuln = 0;
            SetState(new GroundedFighterHurtState(fsm));
            fsm.entity.PlayAnim("Ground_Hurt_Normal_Idle");
        }
    }
    
    public class GroundedFighterHurtFreezeState : GroundedFighterState
    {
        public GroundedFighterHurtFreezeState(FighterSM fsm) : base(fsm)
        {
            lag = true;
        }
    
        public override void OnStateEnter()
        {
            base.OnStateEnter();
            ResetStateVars();
            RotateHurtbox(Vector2.up);
            IndieSpd *= Vector2.right;
            FlippedLeft = IndieSpd.x > 0;
    
            fsm.entity.PlayAnim("Ground_Hurt_Normal_Start");
            if (IndieSpd.magnitude <= 5)
                fsm.entity.PlayAnim("Ground_Hurt_Mini");
        }
        public override void OnStateUpdate(float timeScale)
        {
            base.OnStateEnter();
            if (FreezeTime <= 0)
            {
                fsm.entity.stateVars.lastLaunchSpd = Vector2.zero;
                if (IndieSpd.magnitude > 5)
                    SetState(new GroundedFighterHurtState(fsm));
                else
                    SetState(new GroundedFighterHurtMiniState(fsm));
            }
        }
    
        protected override void TransitionAir()
        {
            //base.TransitionAir();
        }
    }
    
    public class GroundedFighterHurtState : GroundedFighterState
    {
        public GroundedFighterHurtState(FighterSM fsm) : base(fsm)
        {
            lag = true;
        }
    
        public override void OnStateEnter()
        {
            base.OnStateEnter();
            IndieSpd *= Vector2.right;
            fsm.entity.physicsState.SetState(new LaunchedMini(fsm.entity.physicsState));
        }
    
        public override void OnStateUpdate(float timeScale)
        {
            CheckStopStun();
            StunStopTime -= timeScale * Time.fixedDeltaTime * 1.1f;
            base.OnStateUpdate(timeScale);
        }
    
        protected virtual void CheckStopStun()
        {
            if (StunStopTime <= 0)
            {
                ResetPhysicsStates();
                StunStopTime = 0;
                fsm.entity.PlayAnim("Ground_Hurt_Normal_End");
                SetState(new GroundedFighterIdleState(fsm));
            }
        }
    }
    
    public class GroundedFighterHurtMiniState : GroundedFighterHurtState
    {
        public GroundedFighterHurtMiniState(FighterSM fsm) : base(fsm)
        {
            lag = true;
        }
        public override void OnStateExit()
        {
            base.OnStateExit();
            if (fsm.entity.CheckPrevAnimIsName("Ground_Hurt_Normal_Idle"))
                fsm.entity.PlayAnim("Ground_Hurt_Normal_End");
        }
        protected override void CheckStopStun()
        {
            if (StunStopTime <= 0)
            {
                ResetPhysicsStates();
                StunStopTime = 0;
                SetState(new GroundedFighterIdleState(fsm));
            }
        }
    }
    
    public class AirborneFighterLedgeStartState : AirborneFighterState
    {
        public AirborneFighterLedgeStartState(FighterSM fsm) : base(fsm)
        {
            lag = true;
            moveable = false;
        }
        public override void OnStateEnter()
        {
            base.OnStateEnter();
            Gravity = false;
            WindSpd *= 0;
            fsm.entity.PlayAnim("Air_Ledge_Start");
            IndieSpd *= 0;
            SelfSpd *= 0;
        }
        public override void OnStateUpdate(float timeScale)
        {
            Stamina += timeScale * Time.fixedDeltaTime * fsm.entity.NonFreezeTimeScale * 2;
            WindSpd *= 0;
            base.OnStateUpdate(timeScale);
        }
        public override void OnStateExit()
        {
            base.OnStateExit();
            Gravity = true;
        }
    }
    public class AirborneFighterLedgeIdleState : AirborneFighterState
    {
        protected bool LedgeGetupCondition => Controlling.moveStick.raw.y > 0.45f && (Mathf.Sign(Controlling.moveStick.raw.x) == (FlippedLeft ? -1 : 1) || Mathf.Abs(Controlling.moveStick.raw.x) < 0.25f);
    
        public AirborneFighterLedgeIdleState(FighterSM fsm) : base(fsm)
        {
            lag = true;
            moveable = false;
        }
        public override void OnStateEnter()
        {
            base.OnStateEnter();
            Gravity = false;
        }
        public override void OnStateUpdate(float timeScale)
        {
            Stamina += timeScale * Time.fixedDeltaTime * fsm.entity.NonFreezeTimeScale * 2;
            WindSpd *= 0;
            base.OnStateUpdate(timeScale);
            if (!Controlling.grabButton.raw && (!Controlling.autoLedgeGrab || Controlling.moveStick.raw.y < -0.5f))
            {
                fsm.entity.PlayAnim("Air_Idle");
                SetState(new GroundedFighterIdleState(fsm));
            }
            if (LedgeGetupCondition)
            {
                fsm.entity.PlayAnim("Ground_Ledge_End_Getup");
                SetState(new GroundedFighterLedgeEndState(fsm));
            }
            if (AirdashCondition || AirdashToggleCondition || AirdashTapCondition)
            {
                fsm.entity.PlayAnim("Air_Ledge_End_Jump");
                SetState(new GroundedFighterLedgeEndState(fsm));
    
                if (AirdashCondition)
                    Controlling.jumpButton.ConsumeBuffer();
                if (AirdashToggleCondition)
                {
                    Controlling.moveStick.ConsumeBufferHold();
                    ToggleConsume();
                }
                if (AirdashTapCondition)
                    Controlling.moveStick.ConsumeBufferTap();
            }
            if (AttackCondition)
            {
                fsm.entity.PlayAnim("Ground_Ledge_End_Attack");
                SetState(new GroundedFighterLedgeEndState(fsm));
                Controlling.attackButton.ConsumeBuffer();
            }
            if (FocusCondition)
            {
                fsm.entity.PlayAnim("Ground_Ledge_End_Roll");
                SetState(new GroundedFighterLedgeEndState(fsm));
                Controlling.shieldButton.ConsumeBuffer();
            }
    
        }
        public override void OnStateExit()
        {
            base.OnStateExit();
            Gravity = true;
        }
    }
    public class GroundedFighterLedgeEndState : GroundedFighterState
    {
        public GroundedFighterLedgeEndState(FighterSM fsm) : base(fsm)
        {
            lag = true;
        }
        public override void OnStateEnter()
        {
            base.OnStateEnter();
            Gravity = false;
            WindSpd *= 0;
        }
        public override void OnStateUpdate(float timeScale)
        {
            base.OnStateUpdate(timeScale);
            WindSpd *= 0;
        }
        public override void OnStateExit()
        {
            base.OnStateExit();
            fsm.fighter.MatchTransformWithHurtbox();
            Gravity = true;
        }
    
        protected override void TransitionAir()
        {
            //base.TransitionAir();
        }
    }
    
    public class AirborneFighterLedgeEndState : AirborneFighterState
    {
        public AirborneFighterLedgeEndState(FighterSM fsm) : base(fsm)
        {
            lag = true;
            moveable = true;
        }
    
        public override void OnStateEnter()
        {
            base.OnStateEnter();
            IndieSpd += (FlippedLeft ? Vector2.left : Vector2.right) * 2f;
        }
        public override void OnStateUpdate(float timeScale)
        {
            base.OnStateUpdate(timeScale);
            WindSpd *= 0;
        }
    
        public override void OnStateExit()
        {
            base.OnStateExit();
            IndieSpd -= Ft.decelerate * 0.5f * Vector2.up;
        }
    
        public override void TransitionGround()
        {
            //base.TransitionGround();
        }
    }
    public class GroundedFighterSpellcardActivateState : GroundedFighterState
    {
        public GroundedFighterSpellcardActivateState(FighterSM fsm) : base(fsm)
        {
            lag = true;
        }
        public override void OnStateEnter()
        {
            base.OnStateEnter();
            IndieSpd *= 0;
            SelfSpd *= 0;
            GrazeMeter = 33;
            fsm.entity.PlayAnim("Spellcard_Activate");
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
    
    public class GroundedFighterIntroState : GroundedFighterState
    {
        public GroundedFighterIntroState(FighterSM fsm) : base(fsm)
        {
            lag = true;
        }
    
        public override void OnStateEnter()
        {
            base.OnStateEnter();
            fsm.entity.PlayAnim("Ground_Intro");
        }
    
        protected override void TransitionAir()
        {
            //base.TransitionAir();
        }
    }
}
