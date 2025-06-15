namespace TightStuff
{
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.SceneManagement;
    using UnityEngine.Events;
    using TightStuff.Rollback;
    using UnityEngine.InputSystem;

    public class MatchManager : UpdateAbstract
    {
        public List<Entity> entities;
        public List<BaseFighterBehavior> fighters;
        public List<Controller> controllers;
        public List<InteractBox> boxes;
        public List<Hitter> hitters;
        public List<HitBox> hitboxes;
        public List<GrazeBox> grazeboxes;
        public List<Pooler> pools;
    
        public Replayer replayer;
        public Runner runner;
        public SharedGame.IGameRunner gameRunner;
        public MusicPlayer musicPlayer;
    
        public Entity entity;
        public UnityEvent OnGameEnd;
        public UnityEvent OnGameTrueEnd;
    
        public static int FrameNum;
        public static float currentTime = 60 * 7;
        public static bool paused = false;
        public static bool training;
        public static bool local;
        public static bool endGame = false;
        public static bool trueEndGame = false;
        public static int rollingBack;

        public bool IsTraining;

        public static float worldTime = 1;
        public static float initTime = 60 * 7;
        public float DeathFreezeDuration { get => entity.stateVars.genericTimer; set => entity.stateVars.genericTimer = value; }
        public float DeathFreezeCooldown { get => entity.stateVars.percent; set => entity.stateVars.percent = value; }
        public bool SpellcardDarken { get => entity.stateVars.intangible; set => entity.stateVars.intangible = value; }
    
        // Start is called before the first frame update
        void Start()
        {
            training = IsTraining;
            FrameNum = 0;
            endGame = false;
            trueEndGame = false;
            replayer = GetComponent<Replayer>();
            runner = GetComponent<Runner>();
            entity = GetComponent<Entity>();
            GameObject[] rootGameObjects = SceneManager.GetActiveScene().GetRootGameObjects();
    
            foreach (var rootGameObject in rootGameObjects)
            {
                Entity[] es = rootGameObject.GetComponentsInChildren<Entity>();
                for (int i = 0; i < es.Length; i++)
                {
                    entities.Add(es[i]);
                }
                BaseFighterBehavior[] fs = rootGameObject.GetComponentsInChildren<BaseFighterBehavior>();
                foreach (BaseFighterBehavior e in fs)
                {
                    fighters.Add(e);
                }
                Controller[] cs = rootGameObject.GetComponentsInChildren<Controller>();
                foreach (Controller c in cs)
                {
                    controllers.Add(c);
                }
                InteractBox[] box = rootGameObject.GetComponentsInChildren<InteractBox>();
                foreach (InteractBox b in box)
                {
                    boxes.Add(b);
                    if (b is HitBox)
                        hitboxes.Add((HitBox)b);
                    if (b is GrazeBox)
                        grazeboxes.Add((GrazeBox)b);
                }
                Hitter[] hs = rootGameObject.GetComponentsInChildren<Hitter>();
                foreach (Hitter h in hs)
                {
                    hitters.Add(h);
                }
                Pooler[] ps = rootGameObject.GetComponentsInChildren<Pooler>();
                foreach (Pooler p in ps)
                {
                    pools.Add(p);
                }
            }
            ActionSM.OnStateEnter += CheckGameEnded;
            controllers.Sort((a, b) => a.id.CompareTo(b.id));
            fighters.Sort((a, b) => a.controlling.id.CompareTo(b.controlling.id));
        }
    
        private void OnDisable()
        {
            ActionSM.OnStateEnter -= CheckGameEnded;
        }
    
        public void UpdateFrame(long[] inputs)
        {
            if (FrameNum == 0) //FRAME START=======================================
            {
                musicPlayer.StartMusic(musicPlayer.currentMusic);
                currentTime = initTime + 3;
                for (int i = 0; i < entities.Count; i++)
                    entities[i].stateVars.entityID = i;
    
                //Had to be put here instead of Start to prevent crashes for some reason
                foreach (var fighter in fighters)
                    (fighter.entity.AssociatedRenderers[0] as SpriteRenderer).material = fighter.Ft.palette[fighter.colorID].colorMat;
            } //FRAME START=======================================
    
            UpdateFrameIndependentButtons();
            if (paused && local)
            {
                AudioListener.pause = true;
                return;
            }
            AudioListener.pause = false;
    
            if (endGame)
            {
                entity.stateVars.selfTime = Mathf.Clamp(entity.stateVars.selfTime - Time.fixedDeltaTime / 3, 0, 1000);
                if (!trueEndGame && entity.stateVars.selfTime <= 0)
                {
                    //if (!Replayer.replaying)
                        //replayer.SaveReplay();
                    OnGameTrueEnd?.Invoke();
                    trueEndGame = true;
                }
            }
    
            worldTime = entity.stateVars.selfTime;
    
            if (DeathFreezeDuration > 0)
            {
                worldTime = 0;
                DeathFreezeDuration -= Time.fixedDeltaTime;
                musicPlayer.musicSource.volume = (0.25f - DeathFreezeDuration) * (musicPlayer.currentMusic.defaultVolume * musicPlayer.currentVolume) / 0.5f;
                if (DeathFreezeDuration <= 0)
                {
                    musicPlayer.musicSource.volume = musicPlayer.currentMusic.defaultVolume * musicPlayer.currentVolume;
                    worldTime = 1;
                }
            }
    
            if (DeathFreezeCooldown > 0)
                DeathFreezeCooldown -= Time.fixedDeltaTime;
            if ((currentTime > 0 && !endGame && ! training) || currentTime > initTime)
                currentTime -= Time.fixedDeltaTime;
    
            if (currentTime <= 0 && !endGame)
            {
                currentTime = 0;
                EndGame();
            }
    
            FrameNum++;
    
            ParseInputForEachFighter(inputs);
            runner.AdvanceFrame();
            entity.stateVars.selfTime = worldTime;
            if (replayer != null || training)
                replayer.RecordFrame(inputs);
    
            return;
            if (rollingBack > 0)
            {
                Time.timeScale = 2;
                rollingBack--;
            }
            else
            {
                Time.timeScale = 1;
            }
        }
    
        private void EndGame()
        {
            OnGameEnd?.Invoke();
            endGame = true;
            musicPlayer.SetTargetVolume(0);
        }
    
        public void UpdateFrameIndependentButtons()
        {
            foreach(Controller controller in controllers)
            {
                if (controller.input.enabled)
                {
                    controller.submenuButton.raw = controller.submenuButtonRaw;
                    controller.menuButton.raw = controller.menuButtonRaw;
    
                    controller.submenuButton.UpdateButton(Time.fixedDeltaTime, controller.moveStickRaw);
                    controller.menuButton.UpdateButton(Time.fixedDeltaTime, controller.moveStickRaw);


                    if (controller.menuButton.Tapped())
                    {
                        //if (!Replayer.replaying)
                            //replayer.SaveReplay();
                        //paused = !paused;
                        controller.menuButton.ConsumeBuffer();
                    }
                    if (controller.submenuButton.Tapped())
                    {
                        Time.timeScale = 1f;
                        controller.submenuButton.ConsumeBuffer();
                    }
                }
            }
            if (!local)
                return;
            if (!paused)
            {
                Time.timeScale = 1f;
                InputSystem.settings.updateMode = InputSettings.UpdateMode.ProcessEventsManually;
            }
            else
            {
                Time.timeScale = 0;
                InputSystem.settings.updateMode = InputSettings.UpdateMode.ProcessEventsInDynamicUpdate;
            }
        }
    
        private void ParseInputForEachFighter(long[] inputs)
        {
            if (Replayer.replaying)
            {
                replayer.ReplayInputOnly();
                return;
            }
    
            for (int i = 0; i < inputs.Length; i++)
            {
                parseInputs(inputs[i], i);
            }
        }
    
        public void Rollback(Frame frame)
        {
            rollingBack = FrameNum - frame.frameNum;
            for (int i = 0; i < frame.states.Count; i++)
            {
                entities[frame.states[i].entityID].stateVars = frame.states[i];
            }
    
            for (int i = 0; i < fighters.Count; i++)
                fighters[i].stateVarsF = frame.fighterstates[i];
    
            for (int i = 0; i < controllers.Count; i++)
                controllers[i].state = frame.controllerstates[i];
    
            for (int i = 0; i < boxes.Count; i++)
                boxes[i].stateVar = frame.boxstates[i];
    
            for (int i = 0; i < hitboxes.Count; i++)
                hitboxes[i].hitProperties = frame.hitboxproperties[i];
    
            for (int i = 0; i < hitters.Count; i++)
                hitters[i].hitObjects = frame.hitobjects[i];
    
            for (int i = 0; i < grazeboxes.Count; i++)
                grazeboxes[i].grazePoints = frame.grazepoints[i];
    
            for (int i = 0; i < pools.Count; i++)
                pools[i].queue = frame.poolqueues[i];
    
            FrameNum = frame.frameNum;
            currentTime = frame.time;
            RollbackStates();
        }
    
        private void RollbackStates()
        {
            foreach (Entity e in entities)
                e.RollbackState();
            foreach (Controller c in controllers)
                c.RollbackState();
            foreach (InteractBox b in boxes)
                b.RollbackState();
            Physics2D.Simulate(0);
            Physics2D.Simulate(0);
        }
    
        public Frame GetFrame()
        {
            var states = new List<EntityState>();
            var fighterstates = new List<FighterStateVars>();
            var controllerstates = new List<Controller.ControllerState>();
            var boxstates = new List<List<BoxStates>>();
            var boxhitstates = new List<List<HitObject>>();
            var hitproperties = new List<HitProperties>();
            var grazepoints = new List<List<GrazePoint>>();
            var poolqueues = new List<int[]>();
    
            for (int i = 0; i < entities.Count; i++)
            {
                if (entities[i].enabled || entities[i].alwaysSaveState)
                    states.Add(entities[i].stateVars);
            }
    
            foreach (BaseFighterBehavior f in fighters)
                fighterstates.Add(f.stateVarsF);
    
            foreach (Controller c in controllers)
                controllerstates.Add(c.state);
    
            foreach (InteractBox b in boxes)
                boxstates.Add(b.stateVar);
    
            foreach (HitBox h in hitboxes)
                hitproperties.Add(h.hitProperties);
    
            foreach (Hitter hi in hitters)
                boxhitstates.Add(RecreateNewListOfStruct(hi.hitObjects));
    
            foreach (GrazeBox g in grazeboxes)
                grazepoints.Add(RecreateNewListOfStruct(g.grazePoints));
    
            foreach (Pooler p in pools)
                poolqueues.Add(RecreateNewArray(p.queue));
    
            return new Frame(states, fighterstates, controllerstates, boxstates, boxhitstates, hitproperties, grazepoints, poolqueues, currentTime, FrameNum);
        }
    
        public List<T> RecreateNewListOfStruct<T>(List<T> list)
        {
            var tempList = new List<T>();
            foreach (T t in list)
                tempList.Add(t);
            return tempList;
        }
        public T[] RecreateNewArray<T>(T[] array)
        {
            var tempArray = new T[array.Length];
            for (int i = 0; i < tempArray.Length; i++)
                tempArray[i] = array[i];
            return tempArray;
        }
    
        public void parseInputs(long input, int id)
        {
            if (currentTime > initTime)
                return;
            if (controllers.Count <= id)
                return;
            controllers[id].jumpButton.raw = (input & DIFConstants.JUMP) != 0;
            controllers[id].shieldButton.raw = (input & DIFConstants.SHIELD) != 0;
            controllers[id].grabButton.raw = (input & DIFConstants.GRAB) != 0;
            controllers[id].neutralLock.raw = (input & DIFConstants.NEUTRAL) != 0;
            controllers[id].dashToggleButton.raw = (input & DIFConstants.DASHTOGGLE) != 0;
            controllers[id].airdashToggleButton.raw = (input & DIFConstants.AIRDASHTOGGLE) != 0;
            controllers[id].jumpToggleButton.raw = (input & DIFConstants.JUMPTOGGLE) != 0;
            controllers[id].smashToggleButton.raw = (input & DIFConstants.SMASHTOGGLE) != 0;
            controllers[id].specialToggle.raw = (input & DIFConstants.SPECTOGGLE) != 0;
            controllers[id].attackButton.raw = (input & DIFConstants.ATTACK) != 0 && !controllers[id].specialToggle.raw;
            controllers[id].specialButton.raw = ((input & DIFConstants.SPECIAL) != 0) || (((input & DIFConstants.ATTACK) != 0) && controllers[id].specialToggle.raw);
            controllers[id].assist.raw = (input & DIFConstants.ASSIST) != 0;
            controllers[id].upButton.raw = (input & DIFConstants.UP) != 0;
            controllers[id].downButton.raw = (input & DIFConstants.DOWN) != 0;
            controllers[id].leftButton.raw = (input & DIFConstants.LEFT) != 0;
            controllers[id].rightButton.raw = (input & DIFConstants.RIGHT) != 0;
            controllers[id].tauntButton.raw = (input & DIFConstants.TAUNT) != 0;
    
            controllers[id].moveIntX = (int)((input >> 21) & ((1u << 10) - 1)) * (((input & DIFConstants.STICK_MOVE_SIGN_X) != 0) ? -1 : 1);
            controllers[id].moveIntY = (int)((input >> 31) & ((1u << 10) - 1)) * (((input & DIFConstants.STICK_MOVE_SIGN_Y) != 0) ? -1 : 1);
            controllers[id].moveStick.raw = new Vector2(controllers[id].moveIntX / 1000f, controllers[id].moveIntY / 1000f);
    
            controllers[id].cIntX = (int)((input >> 41) & ((1u << 10) - 1)) * (((input & DIFConstants.STICK_C_SIGN_X) != 0) ? -1 : 1);
            controllers[id].cIntY = (int)((input >> 51) & ((1u << 10) - 1)) * (((input & DIFConstants.STICK_C_SIGN_Y) != 0) ? -1 : 1);
            controllers[id].cStick.raw = new Vector2(controllers[id].cIntX / 1000f, controllers[id].cIntY / 1000f);
    
            controllers[id].attackStick.raw = (!controllers[id].specialToggle.raw && !controllers[id].smashToggleButton.raw) ? controllers[id].cStick.raw : Vector2.zero;
            controllers[id].specialStick.raw = controllers[id].specialToggle.raw ? controllers[id].cStick.raw : Vector2.zero;
            controllers[id].smashStick.raw = (controllers[id].smashToggleButton.raw && !controllers[id].specialToggle.raw) ? controllers[id].cStick.raw : Vector2.zero;
        }
    
        public void CheckGameEnded(State state)
        {
            if (state is not GroundedFighterDeadState)
                return;
            var total = fighters.Count;
            if (fighters.Count == 1)
                total++;
            for (int i = 0; i < fighters.Count; i++)
            {
                if (fighters[i].stateVarsF.stocks == 0)
                    total--;
            }
            if (total <= 1 && !endGame)
            {
                EndGame();
            }
        }
        public void ReturnToCharacterSelect()
        {
            if (gameRunner != null)
                gameRunner.Shutdown();
            SceneManager.LoadScene("PreBattle", LoadSceneMode.Single);
        }
    }
    
    public struct Frame
    {
        public List<EntityState> states;
        public List<FighterStateVars> fighterstates;
        public List<Controller.ControllerState> controllerstates;
        public List<List<BoxStates>> boxstates;
        public List<List<HitObject>> hitobjects;
        public List<HitProperties> hitboxproperties;
        public List<List<GrazePoint>> grazepoints;
        public List<int[]> poolqueues;
        public float time;
        public int frameNum;
    
        public Frame(List<EntityState> s, List<FighterStateVars> fs, List<Controller.ControllerState> cs, List<List<BoxStates>> bs, List<List<HitObject>> hs, List<HitProperties> hp, List<List<GrazePoint>> gz, List<int[]> ps, float t, int f)
        {
            states = s;
            fighterstates = fs;
            controllerstates = cs;
            boxstates = bs;
            hitobjects = hs;
            hitboxproperties = hp;
            grazepoints = gz;
            poolqueues = ps;
            frameNum = f;
            time = t;
        }
    }}
