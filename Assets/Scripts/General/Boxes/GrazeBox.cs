namespace TightStuff
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    
    public class GrazeBox : HitBox
    {
        [SerializeField]
        public HitBox reliant;
        [SerializeField]
        private float _grazeRange = 0.6f;
    
        public List<GrazePoint> grazePoints; 
        public delegate void GrazeAction(HitObject hit, ref GrazePoint gz);
        public static event GrazeAction OnGrazeCreate;
    
        protected override void Start()
        {
            base.Start();
            order += 2;
            OnHit.AddListener(Graze);
            grazePoints = new List<GrazePoint>();
            if (reliant == null)
                reliant = GetComponent<HitBox>();
    
            if (reliant != null)
            {
                owner = reliant.owner;
                entity = reliant.entity;
                hitProperties = reliant.hitProperties;
                _colliders = reliant.GetColliders();
                stateVar = reliant.stateVar;
            }
        }
    
        public override void GUpdate()
        {
            hitProperties = reliant.hitProperties;
            base.GUpdate();
        }
    
        public override void LateGUpdate()
        {
            base.LateGUpdate();
            stateVar = reliant.stateVar;
        }
        public override float AdditionalRange()
        {
            return _grazeRange;
        }
        
        public void Graze(HitObject hit)
        {
            if (hit.box is HitBox || hit.box is ShieldBox || hit.box.entity.stateVars.invulnerable || hit.box.entity.stateVars.intangible)
                return;
    
            if (hit.box.entity.TryGetComponent(out BaseFighterBehavior fighter))
            {
                if (fighter.stateVarsF.spellcardState || fighter.stateVarsF.grazeMeter >= 30 || hitProperties.damage <= 0)
                    return;
                var point = new GrazePoint(CalculateGraze(hitProperties.damage), fighter);
                if (OnGrazeCreate != null)
                    OnGrazeCreate(hit, ref point);
                grazePoints.Add(point);
            }
        }
    
        public static int CalculateGraze(float damage)
        {
            return (int)(damage / 2) + 1;
        }
    
        protected override void SaveState() 
        {
            if (reliant == null)
                base.SaveState();
        }
        public override void RollbackState()
        {
            if (reliant != null)
            {
                stateVar = reliant.stateVar;
                return;
            }
            base.RollbackState();
        }
    }
    
    [System.Serializable]
    public struct GrazePoint
    {
        public int value;
        public BaseFighterBehavior fighter;
        public List<GrazeParticle> gzFX;
    
        public static System.Action<BaseFighterBehavior> FullMeter;
    
        public GrazePoint(int val, BaseFighterBehavior f)
        {
            value = val;
            fighter = f;
            gzFX = new List<GrazeParticle>();
        }
        public void GiftGraze()
        {
            foreach(GrazeParticle p in gzFX)
                p.SetHomeTarget(this);
            if (fighter.stateVarsF.grazeMeter + value >= 30 && fighter.stateVarsF.grazeMeter < 30 && FullMeter != null)
                FullMeter.Invoke(fighter);
            fighter.stateVarsF.grazeMeter += value;
            fighter.PlayGrazeOutlineAnimation();
        }
        public void DestroyGraze()
        {
            foreach(GrazeParticle p in gzFX)
                p.DestroyParticle();
        }
    }}
