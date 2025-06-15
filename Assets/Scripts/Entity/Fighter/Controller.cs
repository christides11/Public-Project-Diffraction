namespace TightStuff
{
    ﻿using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using TightStuff.Menu;
    using UnityEngine;
    using UnityEngine.InputSystem;
    
    public class Controller : UpdateAbstract
    {
        public PlayerInput input;

        public TextAsset defaultProfile;
    
        public bool rollbackToggle;
        public float timeScale = 1;
    
        public Button jumpButton;
        public Button shieldButton;
        public Button attackButton;
        public Button specialButton;
        public Button smashButton;
        public Button grabButton;
        public Button tauntButton;
        public Button menuButton;
        public Button submenuButton;
        public Button neutralLock;
        public Button assist;
        public Button specialToggle;
        public Button smashToggleButton;
        public StickToggle dashToggleButton;
        public StickToggle jumpToggleButton;
        public StickToggle airdashToggleButton;
        public Stick moveStick;
        public Stick cStick;
        public Stick attackStick;
        public Stick specialStick;
        public Stick smashStick;
        public Button upButton;
        public Button downButton;
        public Button leftButton;
        public Button rightButton;
    
        public int moveIntX;
        public int moveIntY;
        public int cIntX;
        public int cIntY;
    
        public bool jumpButtonRaw;
        public bool shieldButtonRaw;
        public bool attackButtonRaw;
        public bool specialButtonRaw;
        public bool smashButtonRaw;
        public bool grabButtonRaw;
        public bool tauntButtonRaw;
        public bool menuButtonRaw;
        public bool submenuButtonRaw;
        public bool neutralLockRaw;
        public bool assistRaw;
        public bool specialToggleRaw;
        public bool upButtonRaw;
        public bool downButtonRaw;
        public bool leftButtonRaw;
        public bool rightButtonRaw;
        public bool smashToggleButtonRaw;
        public bool dashToggleButtonRaw;
        public bool jumpToggleButtonRaw;
        public bool airdashToggleButtonRaw;
        public Vector2 moveStickRaw;
        public Vector2 cStickRaw;
        public int moveIntXRaw;
        public int moveIntYRaw;
        public int cIntXRaw;
        public int cIntYRaw;
    
        [System.Serializable]
        public struct ControllerState
        {
            public Button jumpButton;
            public Button shieldButton;
            public Button attackButton;
            public Button specialButton;
            public Button smashButton;
            public Button grabButton;
            public Button tauntButton;
            public Button menuButton;
            public Button submenuButton;
            public Button frame;
            public Button neutralLock;
            public Button assist;
            public Button specialToggle;
            public Button smashToggleButton;
            public StickToggle dashToggleButton;
            public StickToggle jumpToggleButton;
            public StickToggle airdashToggleButton;
            public Stick moveStick;
            public Stick cStick;
            public Stick attackStick;
            public Stick specialStick;
            public Stick smashStick;
            public Button upButton;
            public Button downButton;
            public Button leftButton;
            public Button rightButton;
            public int moveIntX;
            public int moveIntY;
            public int cIntX;
            public int cIntY;
        }
        public ControllerState state;
    
        public bool tapDash;
        public bool tapAirdash;
        public bool tapSmash;
        public bool tapJump;
        public bool tapDrop;
    
        public bool autoLedgeGrab;
    
        public int id;
        public string playerTag;
        public Color playerColor = Color.white;
    
        public float bufferWindow;
        public float stickThreshold = 0.75f;
        public int stickSensitivity = 4;
        public bool doubleTap;
        public bool stickDirectionChangeResetsTrigger;

        public string rebindFileName;

        [System.Serializable]
        public struct Button
        {
            public bool raw;
            public float timer;
            public float bufferWindow;
    
            public Vector2 stickAccompanimentOnTap;
    
            public bool Buffer()
            {
                return raw && timer < bufferWindow;
            }
            public void ConsumeBuffer()
            {
                timer = bufferWindow;
            }
            public void ResetBuffer()
            {
                timer = 0;
            }
    
            public bool Tapped()
            {
                return raw && timer <= Time.fixedDeltaTime;
            }
    
            public void UpdateButton(float deltatime, Vector2 stick)
            {
                if (!raw)
                {
                    timer = 0;
                    return;
                }
                if (timer <= 0)
                    stickAccompanimentOnTap = stick;
    
                if (timer < bufferWindow)
                    timer += deltatime;
            }
        }
        [System.Serializable]
        public struct StickToggle
        {
            public bool raw;
            public float timer;
    
            public float bufferWindow;
            public float stickThreshold;
            public bool stickDirectionChangeResetsTrigger;
    
            public Vector2 stickAccompanimentOnTap;
            public Vector2 stickAccompaniment;
    
            public bool Buffer()
            {
                return raw && timer < bufferWindow && stickAccompaniment.magnitude > stickThreshold;
            }
            public void ConsumeBuffer()
            {
                timer = bufferWindow;
            }
    
            public bool Tapped()
            {
                return raw && timer <= Time.fixedDeltaTime;
            }
    
            public void UpdateButton(float deltatime, Vector2 stick)
            {
                stickAccompaniment = stick;
                if (!raw || stickAccompaniment.magnitude <= stickThreshold)
                {
                    timer = 0;
                    return;
                }
    
                if (CheckDifferentDirection())
                    timer = 0;
    
                if (timer <= 0)
                    stickAccompanimentOnTap = stick;
    
                if (timer < bufferWindow)
                    timer += deltatime;
            }
    
            private bool CheckDifferentDirection()
            {
                return (Vector2.Dot(stickAccompaniment.normalized, stickAccompanimentOnTap.normalized) < 0.75f && timer >= bufferWindow) && stickDirectionChangeResetsTrigger;
            }
        }
    
        [System.Serializable]
        public struct Stick
        {
            public Vector2 raw;
            public Vector2 holdInitPos;
            public Vector2 tapInitPos;
    
            public List<Vector2> prevPos;
            public List<Vector2> prevPos2;
            public float timerHold;
            public float timerTap;
            public float timerTap2;
            public float bufferWindow;
            public int stickSensitivity;
    
            public bool holding;
            public bool doubleTap;
    
            public bool BufferTap()
            {
                if (!doubleTap)
                    return timerTap > 0;
                else
                    return timerTap2 > 0;
            }
            public bool BufferHold()
            {
                return timerHold > 0;
            }
    
            public void ConsumeBufferTap()
            {
                timerTap = 0;
                timerTap2 = 0;
            }
            public void ConsumeBufferHold()
            {
                timerHold = 0;
            }
            public void ResetBuffer()
            {
                timerTap = bufferWindow;
                timerHold = bufferWindow;
            }
    
            public void UpdateButton(float deltatime)
            {
                if (prevPos == null)
                    prevPos = new List<Vector2>();
                prevPos.Add(raw);
    
                if (prevPos.Count > stickSensitivity)
                    prevPos.RemoveAt(0);
    
                if (prevPos2 == null)
                    prevPos2 = new List<Vector2>();
                prevPos2.Add(raw);
    
                if (prevPos2.Count > stickSensitivity)
                    prevPos2.RemoveAt(0);
    
                CheckHold(deltatime);
                CheckTap(deltatime);
            }
    
            private void CheckHold(float deltatime) //Only checks if stick reaches a certain threshold and is held
            {
                if (raw.magnitude <= 0.5f)
                    holding = false;
    
                if (raw.magnitude > 0.5f && (!holding || CheckDifferentDirection()))
                {
                    timerHold = bufferWindow;
                    holdInitPos = raw;
                    holding = true;
                    return;
                }
    
                if (timerHold > 0)
                    timerHold -= deltatime;
            }
    
            private void CheckTap(float deltatime) //Checks if the stick is tapped in a certain speed
            {
                if (!doubleTap)
                {
                    foreach (Vector2 p in prevPos)
                    {
                        if ((raw - p).magnitude > 0.6f && raw.magnitude > 0.9f)
                        {
                            timerTap = bufferWindow;
                            prevPos.Clear();
                            tapInitPos = raw;
                            break;
                        }
                    }
                }
                else
                {
                    if (timerTap > 0)
                    {
                        var returned = false;
                        foreach (Vector2 p in prevPos2)
                        { 
                            if (!returned)
                            {
                                if (p.magnitude < 0.3f)
                                    returned = true;
                            }
                            else if ((raw - p).magnitude > 0.6f && raw.magnitude > 0.9f && Vector2.Dot(tapInitPos, raw) > 0.5f)
                            {
                                timerTap2 = bufferWindow;
                                timerTap = 0;
                                prevPos2.Clear();
                                tapInitPos = raw;
                                break;
                            }
                        }
                    }
                    else
                    {
                        foreach (Vector2 p in prevPos)
                        {
                            if ((raw - p).magnitude > 0.6f && raw.magnitude > 0.9f)
                            {
                                timerTap = bufferWindow + 5 * Time.fixedDeltaTime;
                                prevPos.Clear();
                                prevPos2.Clear();
                                tapInitPos = raw;
                                break;
                            }
                        }
                    }
                }
                if (timerTap > 0)
                    timerTap -= deltatime;
                if (timerTap2 > 0)
                    timerTap2 -= deltatime;
            }
    
            private bool CheckDifferentDirection()
            {
                return Vector2.Dot(raw.normalized, holdInitPos.normalized) < 0.5f;
            }
        }
        private void OnDisable()
        {
            string actionMapName = "Default";
            InputActionAsset actionAsset = LoadInputProfile("ActionMaps/" + actionMapName + ".json");
            input.actions = actionAsset;
            if (!string.IsNullOrEmpty(rebindFileName))
            {

                var prof = PlayerTag.FromJson(defaultProfile.text);
                if (prof != null)
                {
                    actionAsset = prof.rebind;
                    actionAsset.LoadBindingOverridesFromJson(prof.rebindOverrides);
                    input.actions = actionAsset;
                    Debug.Log("Rebind Loaded!");
                }
            }

            actionAsset.FindActionMap("Gameplay").FindAction("Move").performed -= OnMove;
            actionAsset.FindActionMap("Gameplay").FindAction("Jump").performed -= OnJump;
            actionAsset.FindActionMap("Gameplay").FindAction("Shield").performed -= OnShield;
            actionAsset.FindActionMap("Gameplay").FindAction("DashToggle").performed -= OnDashToggle;
            actionAsset.FindActionMap("Gameplay").FindAction("Grab").performed -= OnGrab;
            actionAsset.FindActionMap("Gameplay").FindAction("Attack").performed -= OnAttack;
            actionAsset.FindActionMap("Gameplay").FindAction("AttackDir").performed -= OnCStick;
            actionAsset.FindActionMap("Gameplay").FindAction("Menu").performed -= OnPause;
            actionAsset.FindActionMap("Gameplay").FindAction("SubMenu").performed -= OnSubmenu;
            actionAsset.FindActionMap("Gameplay").FindAction("Special").performed -= OnSpecial;
            actionAsset.FindActionMap("Gameplay").FindAction("SpecialToggle").performed -= OnSpecToggle;
            actionAsset.FindActionMap("Gameplay").FindAction("NeutralLock").performed -= OnNeutral;
            actionAsset.FindActionMap("Gameplay").FindAction("SmashToggle").performed -= OnSmashToggle;
            actionAsset.FindActionMap("Gameplay").FindAction("Smash").performed -= OnSmash;
            actionAsset.FindActionMap("Gameplay").FindAction("Spellcard").performed -= OnSpellcard;
            actionAsset.FindActionMap("Gameplay").FindAction("Assist").performed -= OnAssist;
            actionAsset.FindActionMap("Gameplay").FindAction("JumpToggle").performed -= OnJumpToggle;
            actionAsset.FindActionMap("Gameplay").FindAction("AirdashToggle").performed -= OnAirdashToggle;
            actionAsset.FindActionMap("Gameplay").FindAction("DPadUp").performed -= OnUp;
            actionAsset.FindActionMap("Gameplay").FindAction("DPadDown").performed -= OnDown;
            actionAsset.FindActionMap("Gameplay").FindAction("DPadLeft").performed -= OnLeft;
            actionAsset.FindActionMap("Gameplay").FindAction("DPadRight").performed -= OnRight;
            actionAsset.FindActionMap("Gameplay").FindAction("Taunt").performed -= OnTaunt;
    
            //foreach (var map in actionAsset.actionMaps)
                //map.RemoveAllBindingOverrides();

            actionAsset.FindActionMap("Gameplay").Disable();
        }
    
        // Start is called before the first frame update
        void Start()
        {
            order = -1;
            input = GetComponent<PlayerInput>();

            if (playerTag == "")
                playerTag = "P" + (id + 1);

            string actionMapName = "Default";

            InputActionAsset actionAsset = LoadInputProfile("ActionMaps/" + actionMapName + ".json");

            input.actions = actionAsset;
            if (!string.IsNullOrEmpty(rebindFileName))
            {
                var prof = PlayerTag.FromJson(defaultProfile.text);
                if (File.Exists("PlayerProfiles/" + rebindFileName + ".json"))
                {
                    prof = PlayerTag.LoadFromJson("PlayerProfiles/" + rebindFileName + ".json");
                }
                if (prof != null)
                {
                    actionAsset = prof.rebind;
                    actionAsset.LoadBindingOverridesFromJson(prof.rebindOverrides);
                    input.actions = actionAsset;
                    Debug.Log("Rebind Loaded!");
                }
            }

            input.SwitchCurrentActionMap("Gameplay");

            actionAsset.FindActionMap("Gameplay").FindAction("Move").performed += OnMove;
            actionAsset.FindActionMap("Gameplay").FindAction("Jump").performed += OnJump;
            actionAsset.FindActionMap("Gameplay").FindAction("Shield").performed += OnShield;
            actionAsset.FindActionMap("Gameplay").FindAction("DashToggle").performed += OnDashToggle;
            actionAsset.FindActionMap("Gameplay").FindAction("Grab").performed += OnGrab;
            actionAsset.FindActionMap("Gameplay").FindAction("Attack").performed += OnAttack;
            actionAsset.FindActionMap("Gameplay").FindAction("AttackDir").performed += OnCStick;
            actionAsset.FindActionMap("Gameplay").FindAction("Menu").performed += OnPause;
            actionAsset.FindActionMap("Gameplay").FindAction("SubMenu").performed += OnSubmenu;
            actionAsset.FindActionMap("Gameplay").FindAction("Special").performed += OnSpecial;
            actionAsset.FindActionMap("Gameplay").FindAction("SpecialToggle").performed += OnSpecToggle;
            actionAsset.FindActionMap("Gameplay").FindAction("NeutralLock").performed += OnNeutral;
            actionAsset.FindActionMap("Gameplay").FindAction("SmashToggle").performed += OnSmashToggle;
            actionAsset.FindActionMap("Gameplay").FindAction("Smash").performed += OnSmash;
            actionAsset.FindActionMap("Gameplay").FindAction("Spellcard").performed += OnSpellcard;
            actionAsset.FindActionMap("Gameplay").FindAction("Assist").performed += OnAssist;
            actionAsset.FindActionMap("Gameplay").FindAction("JumpToggle").performed += OnJumpToggle;
            actionAsset.FindActionMap("Gameplay").FindAction("AirdashToggle").performed += OnAirdashToggle;
            actionAsset.FindActionMap("Gameplay").FindAction("DPadUp").performed += OnUp;
            actionAsset.FindActionMap("Gameplay").FindAction("DPadDown").performed += OnDown;
            actionAsset.FindActionMap("Gameplay").FindAction("DPadLeft").performed += OnLeft;
            actionAsset.FindActionMap("Gameplay").FindAction("DPadRight").performed += OnRight;
            actionAsset.FindActionMap("Gameplay").FindAction("Taunt").performed += OnTaunt;
    
    
            jumpButton.bufferWindow = bufferWindow * Time.fixedDeltaTime;
            shieldButton.bufferWindow = bufferWindow * Time.fixedDeltaTime;
            attackButton.bufferWindow = bufferWindow * Time.fixedDeltaTime;
            specialButton.bufferWindow = bufferWindow * Time.fixedDeltaTime;
            smashButton.bufferWindow = bufferWindow * Time.fixedDeltaTime;
            grabButton.bufferWindow = bufferWindow * Time.fixedDeltaTime;
            tauntButton.bufferWindow = bufferWindow * Time.fixedDeltaTime;
            menuButton.bufferWindow = bufferWindow * Time.fixedDeltaTime;
            submenuButton.bufferWindow = bufferWindow * Time.fixedDeltaTime;
            neutralLock.bufferWindow = bufferWindow * Time.fixedDeltaTime;
            assist.bufferWindow = bufferWindow * Time.fixedDeltaTime;
            specialToggle.bufferWindow = bufferWindow * Time.fixedDeltaTime;
            smashToggleButton.bufferWindow = bufferWindow * Time.fixedDeltaTime;
            dashToggleButton.bufferWindow = bufferWindow * Time.fixedDeltaTime;
            airdashToggleButton.bufferWindow = bufferWindow * Time.fixedDeltaTime;
            jumpToggleButton.bufferWindow = bufferWindow * Time.fixedDeltaTime;
            upButton.bufferWindow = bufferWindow * Time.fixedDeltaTime;
            downButton.bufferWindow = bufferWindow * Time.fixedDeltaTime;
            leftButton.bufferWindow = bufferWindow * Time.fixedDeltaTime;
            rightButton.bufferWindow = bufferWindow * Time.fixedDeltaTime;
    
            dashToggleButton.stickDirectionChangeResetsTrigger = stickDirectionChangeResetsTrigger;
            airdashToggleButton.stickDirectionChangeResetsTrigger = stickDirectionChangeResetsTrigger;
            jumpToggleButton.stickDirectionChangeResetsTrigger = stickDirectionChangeResetsTrigger;
    
            airdashToggleButton.stickThreshold = stickThreshold;
            dashToggleButton.stickThreshold = stickThreshold;
            jumpToggleButton.stickThreshold = stickThreshold;
    
            moveStick.bufferWindow = bufferWindow * Time.fixedDeltaTime;
            moveStick.prevPos = new List<Vector2>();
            moveStick.stickSensitivity = stickSensitivity;
            moveStick.doubleTap = doubleTap;
    
            cStick.bufferWindow = bufferWindow * Time.fixedDeltaTime;
            cStick.prevPos = new List<Vector2>();
            cStick.stickSensitivity = stickSensitivity;
            cStick.doubleTap = doubleTap;

            attackStick.bufferWindow = bufferWindow * Time.fixedDeltaTime;
            attackStick.prevPos = new List<Vector2>();
            attackStick.stickSensitivity = stickSensitivity;
            attackStick.doubleTap = doubleTap;

            specialStick.bufferWindow = bufferWindow * Time.fixedDeltaTime;
            specialStick.prevPos = new List<Vector2>();
            specialStick.stickSensitivity = stickSensitivity;
            specialStick.doubleTap = doubleTap;

            smashStick.bufferWindow = bufferWindow * Time.fixedDeltaTime;
            smashStick.prevPos = new List<Vector2>();
            smashStick.stickSensitivity = stickSensitivity;
            smashStick.doubleTap = doubleTap;

            state = new ControllerState();
            UpdateState();
        }
        public InputActionAsset LoadInputProfile(string path)
        {
            string jsonContent = File.ReadAllText(path);
            return InputActionAsset.FromJson(jsonContent);
        }

        public override void GUpdate()
        {
            base.GUpdate();
            ButtonUpdates();
        }
    
        public override void LateGUpdate()
        {
            base.LateGUpdate();
            UpdateState();
        }
    
        private void UpdateState()
        {
            state.jumpButton = jumpButton;
            state.shieldButton = shieldButton;
            state.attackButton = attackButton;
            state.specialButton = specialButton;
            state.smashButton = smashButton;
            state.grabButton = grabButton;
            state.neutralLock = neutralLock;
            state.dashToggleButton = dashToggleButton;
            state.airdashToggleButton = airdashToggleButton;
            state.jumpToggleButton = jumpToggleButton;
            state.smashToggleButton = smashToggleButton;
            state.specialToggle = specialToggle;
            state.assist = assist;
            state.upButton = upButton;
            state.downButton = downButton;
            state.leftButton = leftButton;
            state.rightButton = rightButton;
            state.tauntButton = tauntButton;
    
            state.moveStick = moveStick;
            state.cStick = cStick;
            state.attackStick = attackStick;
            state.specialStick = specialStick;
            state.smashStick = smashStick;
            state.moveIntX = moveIntX;
            state.moveIntY = moveIntY;
            state.cIntX = cIntX;
            state.cIntY = cIntY;
        }
    
        public void RollbackState()
        {
            jumpButton = state.jumpButton;
            shieldButton = state.shieldButton;
            attackButton = state.attackButton;
            specialButton = state.specialButton;
            smashButton = state.smashButton;
            grabButton = state.grabButton;
            tauntButton = state.tauntButton;
            neutralLock = state.neutralLock;
            dashToggleButton = state.dashToggleButton;
            airdashToggleButton = state.airdashToggleButton;
            jumpToggleButton = state.jumpToggleButton;
            smashToggleButton = state.smashToggleButton;
            specialToggle = state.specialToggle;
            assist = state.assist;
            upButton = state.upButton;
            downButton = state.downButton;
            leftButton = state.leftButton;
            rightButton = state.rightButton;
            moveStick = state.moveStick;
            cStick = state.cStick;
            attackStick = state.attackStick;
            specialStick = state.specialStick;
            smashStick = state.smashStick;
            moveIntX = state.moveIntX;
            moveIntY = state.moveIntY;
            cIntX = state.cIntX;
            cIntY = state.cIntY;
        }
    
        private void ButtonUpdates()
        {
            jumpButton.UpdateButton(Time.fixedDeltaTime * timeScale, moveStick.raw);
            shieldButton.UpdateButton(Time.fixedDeltaTime * timeScale, moveStick.raw);
            attackButton.UpdateButton(Time.fixedDeltaTime * timeScale, moveStick.raw);
            specialButton.UpdateButton(Time.fixedDeltaTime * timeScale, moveStick.raw);
            smashButton.UpdateButton(Time.fixedDeltaTime * timeScale, moveStick.raw);
            grabButton.UpdateButton(Time.fixedDeltaTime * timeScale, moveStick.raw);
            tauntButton.UpdateButton(Time.fixedDeltaTime * timeScale, moveStick.raw);
            menuButton.UpdateButton(Time.fixedDeltaTime * timeScale, moveStick.raw);
            submenuButton.UpdateButton(Time.fixedDeltaTime * timeScale, moveStick.raw);
            neutralLock.UpdateButton(Time.fixedDeltaTime * timeScale, moveStick.raw);
            dashToggleButton.UpdateButton(Time.fixedDeltaTime * timeScale, moveStick.raw);
            airdashToggleButton.UpdateButton(Time.fixedDeltaTime * timeScale, moveStick.raw);
            jumpToggleButton.UpdateButton(Time.fixedDeltaTime * timeScale, moveStick.raw);
            smashToggleButton.UpdateButton(Time.fixedDeltaTime * timeScale, moveStick.raw);
            specialToggle.UpdateButton(Time.fixedDeltaTime * timeScale, moveStick.raw);
            assist.UpdateButton(Time.fixedDeltaTime * timeScale, moveStick.raw);
            upButton.UpdateButton(Time.fixedDeltaTime * timeScale, moveStick.raw);
            downButton.UpdateButton(Time.fixedDeltaTime * timeScale, moveStick.raw);
            leftButton.UpdateButton(Time.fixedDeltaTime * timeScale, moveStick.raw);
            rightButton.UpdateButton(Time.fixedDeltaTime * timeScale, moveStick.raw);
    
            moveStick.UpdateButton(Time.fixedDeltaTime * timeScale);
            cStick.UpdateButton(Time.fixedDeltaTime * timeScale);
            attackStick.UpdateButton(Time.fixedDeltaTime * timeScale);
            specialStick.UpdateButton(Time.fixedDeltaTime * timeScale);
            smashStick.UpdateButton(Time.fixedDeltaTime * timeScale);
        }
    
        public void OnMove(InputAction.CallbackContext ctx)
        {
            var input = Vector2.ClampMagnitude(ctx.ReadValue<Vector2>(), 1);
    
            moveIntXRaw = (int)(input.x * 1000);
            moveIntYRaw = (int)(input.y * 1000);
            moveStickRaw = new Vector2((float)moveIntX / 1000f, (float)moveIntY / 1000f);
        }
        public void OnCStick(InputAction.CallbackContext ctx)
        {
            var input = Vector2.ClampMagnitude(ctx.ReadValue<Vector2>(), 1);
    
            cIntXRaw = (int)(input.x * 1000);
            cIntYRaw = (int)(input.y * 1000);
            cStickRaw = new Vector2((float)cIntX / 1000f, (float)cIntY / 1000f);
        }
        public void OnJump(InputAction.CallbackContext ctx)
        {
            jumpButtonRaw =  (ctx.ReadValueAsObject() is Vector2) ? ctx.ReadValue<Vector2>().magnitude > 0.25f : ctx.ReadValueAsButton();
        }
        public void OnAttack(InputAction.CallbackContext ctx)
        {
            attackButtonRaw =  (ctx.ReadValueAsObject() is Vector2) ? ctx.ReadValue<Vector2>().magnitude > 0.25f : ctx.ReadValueAsButton();
        }
        public void OnSpecial(InputAction.CallbackContext ctx)
        {
            specialButtonRaw =  (ctx.ReadValueAsObject() is Vector2) ? ctx.ReadValue<Vector2>().magnitude > 0.25f : ctx.ReadValueAsButton();
        }
        public void OnGrab(InputAction.CallbackContext ctx)
        {
            grabButtonRaw =  (ctx.ReadValueAsObject() is Vector2) ? ctx.ReadValue<Vector2>().magnitude > 0.25f : ctx.ReadValueAsButton();
        }
        public void OnShield(InputAction.CallbackContext ctx)
        {
            shieldButtonRaw =  (ctx.ReadValueAsObject() is Vector2) ? ctx.ReadValue<Vector2>().magnitude > 0.25f : ctx.ReadValueAsButton();
        }
        public void OnNeutral(InputAction.CallbackContext ctx)
        {
            neutralLockRaw =  (ctx.ReadValueAsObject() is Vector2) ? ctx.ReadValue<Vector2>().magnitude > 0.25f : ctx.ReadValueAsButton();
        }
        public void OnSubmenu(InputAction.CallbackContext ctx)
        {
            submenuButtonRaw =  (ctx.ReadValueAsObject() is Vector2) ? ctx.ReadValue<Vector2>().magnitude > 0.25f : ctx.ReadValueAsButton();
        }
        public void OnPause(InputAction.CallbackContext ctx)
        {
            menuButtonRaw =  (ctx.ReadValueAsObject() is Vector2) ? ctx.ReadValue<Vector2>().magnitude > 0.25f : ctx.ReadValueAsButton();
        }
        public void OnSmash(InputAction.CallbackContext ctx)
        {
            smashButtonRaw =  (ctx.ReadValueAsObject() is Vector2) ? ctx.ReadValue<Vector2>().magnitude > 0.25f : ctx.ReadValueAsButton();
            attackButtonRaw =  (ctx.ReadValueAsObject() is Vector2) ? ctx.ReadValue<Vector2>().magnitude > 0.25f : ctx.ReadValueAsButton();
        }
        public void OnSpellcard(InputAction.CallbackContext ctx)
        {
            specialButtonRaw =  (ctx.ReadValueAsObject() is Vector2) ? ctx.ReadValue<Vector2>().magnitude > 0.25f : ctx.ReadValueAsButton();
            attackButtonRaw =  (ctx.ReadValueAsObject() is Vector2) ? ctx.ReadValue<Vector2>().magnitude > 0.25f : ctx.ReadValueAsButton();
        }
        public void OnUp(InputAction.CallbackContext ctx)
        {
            upButtonRaw =  (ctx.ReadValueAsObject() is Vector2) ? ctx.ReadValue<Vector2>().magnitude > 0.25f : ctx.ReadValueAsButton();
        }
        public void OnDown(InputAction.CallbackContext ctx)
        {
            downButtonRaw =  (ctx.ReadValueAsObject() is Vector2) ? ctx.ReadValue<Vector2>().magnitude > 0.25f : ctx.ReadValueAsButton();
        }
        public void OnLeft(InputAction.CallbackContext ctx)
        {
            leftButtonRaw =  (ctx.ReadValueAsObject() is Vector2) ? ctx.ReadValue<Vector2>().magnitude > 0.25f : ctx.ReadValueAsButton();
        }
        public void OnRight(InputAction.CallbackContext ctx)
        {
            rightButtonRaw =  (ctx.ReadValueAsObject() is Vector2) ? ctx.ReadValue<Vector2>().magnitude > 0.25f : ctx.ReadValueAsButton();
        }
        public void OnTaunt(InputAction.CallbackContext ctx)
        {
            tauntButtonRaw =  (ctx.ReadValueAsObject() is Vector2) ? ctx.ReadValue<Vector2>().magnitude > 0.25f : ctx.ReadValueAsButton();
        }
        public void OnDashToggle(InputAction.CallbackContext ctx)
        {
            dashToggleButtonRaw =  (ctx.ReadValueAsObject() is Vector2) ? ctx.ReadValue<Vector2>().magnitude > 0.25f : ctx.ReadValueAsButton();
        }
        public void OnJumpToggle(InputAction.CallbackContext ctx)
        {
            airdashToggleButtonRaw =  (ctx.ReadValueAsObject() is Vector2) ? ctx.ReadValue<Vector2>().magnitude > 0.25f : ctx.ReadValueAsButton();
            jumpToggleButtonRaw =  (ctx.ReadValueAsObject() is Vector2) ? ctx.ReadValue<Vector2>().magnitude > 0.25f : ctx.ReadValueAsButton();
        }
        public void OnAirdashToggle(InputAction.CallbackContext ctx)
        {
            airdashToggleButtonRaw =  (ctx.ReadValueAsObject() is Vector2) ? ctx.ReadValue<Vector2>().magnitude > 0.25f : ctx.ReadValueAsButton();
        }
        public void OnSmashToggle(InputAction.CallbackContext ctx)
        {
            smashToggleButtonRaw =  (ctx.ReadValueAsObject() is Vector2) ? ctx.ReadValue<Vector2>().magnitude > 0.25f : ctx.ReadValueAsButton();
        }
        public void OnSpecToggle(InputAction.CallbackContext ctx)
        {
            specialToggleRaw =  (ctx.ReadValueAsObject() is Vector2) ? ctx.ReadValue<Vector2>().magnitude > 0.25f : ctx.ReadValueAsButton();
        }
    
        public void OnAssist(InputAction.CallbackContext ctx)
        {
            assistRaw =  (ctx.ReadValueAsObject() is Vector2) ? ctx.ReadValue<Vector2>().magnitude > 0.25f : ctx.ReadValueAsButton();
        }
    }
}
