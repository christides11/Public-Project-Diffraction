namespace TightStuff
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    
    public abstract class InteractBox : UpdateAbstract
    {
        public static event Action<BoxDraw> OnBoxDraw;
    
        [SerializeField]
        protected LayerMask _hitLayers;
    
        public Entity entity;
        public Entity owner;
    
        [SerializeField]
        protected List<Collider2D> _colliders;
    
        public List<BoxStates> stateVar;
        public OnHitEvent OnHitted;
        public OnHitEvent OnGrazed;
    
        public bool hitByOwner;
        public bool ignoreHitbox;
        public LayerMask HitLayers => _hitLayers; 
        public List<Collider2D> Colliders => _colliders; 
    
        public enum BoxType
        {
            Hurtbox = 11, Shield = 12, Hitbox = 13, Blastzone = 14
        }
    
        // Start is called before the first frame update
        protected virtual void Start()
        {
            order = 20000;
            lateOrder = 25000;
            if (entity == null)
                entity = GetComponent<Entity>();
    
            SaveState();
        }
    
        public override void GUpdate()
        {
            base.GUpdate();
    
            var _colliders = GetColliders();
            for (int i = 0; i < _colliders.Count; i++)
            {
                Collider2D col = _colliders[i];
                if (!col.enabled)
                    continue;
    
                Vector3 origin = transform.position + transform.rotation * Vector3.Scale((Vector3)col.offset, transform.lossyScale);
    
                if (col is CircleCollider2D)
                {
                    var circle = col as CircleCollider2D;
                    var size = circle.radius * transform.lossyScale.y;
    
                    if (col.gameObject.layer != 13)
                        InvokeBoxDraw(new BoxDraw(origin, size * 2, BoxDraw.BoxShape.circle, this));
                }
                else if (col is BoxCollider2D)
                {
                    var box = col as BoxCollider2D;
                    var size = box.size * transform.lossyScale.y * Vector2.one;
    
                    if (col.gameObject.layer != 13)
                        InvokeBoxDraw(new BoxDraw(origin, size, BoxDraw.BoxShape.box, this, transform.eulerAngles.z));
                }
            }
        }
    
        public override void LateGUpdate()
        {
            base.LateGUpdate();
            SaveState();
        }
    
        protected virtual void SaveState()
        {
            stateVar = new List<BoxStates>();
            foreach (Collider2D col in Colliders)
            {
                if (col is CircleCollider2D)
                {
                    var circle = col as CircleCollider2D;
                    stateVar.Add(new BoxStates(circle.offset, circle.radius, col.enabled, hitByOwner));
                }
                else if (col is BoxCollider2D)
                {
                    var box = col as BoxCollider2D;
                    stateVar.Add(new BoxStates(box.offset, box.size, col.enabled, hitByOwner));
                }
            }
        }
    
        public virtual void RollbackState()
        {
            for (int i = 0; i < Colliders.Count; i++)
            {
                Collider2D col = Colliders[i];
                if (col is CircleCollider2D)
                {
                    var circle = col as CircleCollider2D;
                    circle.radius = stateVar[i].radius;
                    circle.offset = stateVar[i].offset;
                    circle.enabled = stateVar[i].enabled;
                    hitByOwner = stateVar[i].hitByOwner;
                }
                else if (col is BoxCollider2D)
                {
                    var box = col as BoxCollider2D;
                    box.offset = stateVar[i].offset;
                    box.size = stateVar[i].size;
                    box.enabled = stateVar[i].enabled;
                    hitByOwner = stateVar[i].hitByOwner;
                }
            }
        }
    
        public void InvokeBoxDraw(BoxDraw properties)
        {
            if (OnBoxDraw != null)
                OnBoxDraw(properties);
        }
    
        public List<Collider2D> GetColliders()
        {
            return Colliders;
        }
    
    }
    
    public struct BoxStates
    {
        public bool enabled;
    
        public Vector2 offset;
        public Vector2 size;
        public float radius;
    
        public bool hitByOwner;
    
        public BoxStates(Vector2 off, Vector2 siz, bool enable, bool hit)
        {
            offset = off;
            size = siz;
            radius = 0;
            enabled = enable;
            hitByOwner = hit;
        }
        public BoxStates(Vector2 off, float rad, bool enable, bool hit)
        {
            offset = off;
            size = Vector2.zero;
            radius = rad;
            enabled = enable;
            hitByOwner = hit;
        }
    }
    
    public class BoxDraw
    {
        public Vector2 scale;
        public float radius;
    
        public Vector2 center;
        public BoxShape boxShape;
        public readonly InteractBox box;
        public float angle;
    
        public BoxDraw(Vector2 pos, Vector2 scal, BoxShape shape, InteractBox type, float an)
        {
            scale = scal;
            center = pos;
            boxShape = shape;
            box = type;
            angle = an;
        }
        public BoxDraw(Vector2 pos, float rad, BoxShape shape, InteractBox type)
        {
            radius = rad;
            center = pos;
            boxShape = shape;
            box = type;
        }
    
        public enum BoxShape { box, circle }
    }}
