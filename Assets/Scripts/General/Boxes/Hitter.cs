namespace TightStuff
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    
    public class Hitter : UpdateAbstract
    {
        public List<HitBox> hitboxes;
        public List<HitObject> hitObjects;
        
        public OnHitEvent OnHitRemove;
        public static event Action<HitObject> OnHitEvent;
    
        protected virtual void Start()
        {
            order = 25000;
            lateOrder = 0;
        }
    
        public override void GUpdate()
        {
            base.GUpdate();
            if (!enabled || MatchManager.worldTime <= 0)
                return;
            foreach (var hit in hitboxes)
                DetectBoxes(hit);
        }
    
        public override void LateGUpdate()
        {
            base.LateGUpdate();
            for (int i = 0; i < hitObjects.Count; i++)
                ActOnHit(hitObjects[i]);
            if (hitboxes.Count <= 0)
            {
                Debug.Log(name);
                return;
            }
            UpdateHitObjects(hitboxes[0].entity.TrueTimeScale);
        }
    
        private void UpdateHitObjects(float timeScale)
        {
            var tempHits = new List<HitObject>();
            foreach (var hit in hitObjects)
            {
                if (hit.hitTimer == hit.hitbox.hitProperties.hitCoolDown)
                    tempHits.Add(hit.UpdateHit(1));
                else
                    tempHits.Add(hit.UpdateHit(timeScale));
    
                if (hit.hitTimer <= 0)
                {
                    if (OnHitRemove != null)
                        OnHitRemove.Invoke(hit);
                    tempHits.Remove(hit);
                }
            }
            hitObjects = tempHits;
        }
    
        public void ClearHits()
        {
            if (OnHitRemove != null)
            {
                foreach (var hit in hitObjects)
                    OnHitRemove.Invoke(hit);
            }
            var tempHits = new List<HitObject>();
            hitObjects = tempHits;
        }
    
        private void DetectBoxes(HitBox hit)
        {
            if (hit.entity.NonFreezeTimeScale <= 0)
                return;
            var _colliders = hit.GetColliders();
            for (int i = 0; i < _colliders.Count; i++)
            {
                Collider2D col = _colliders[i];
                if (!col.enabled)
                    continue;
    
                Vector3 prevPos = hit.entity.stateVars.position + hit.transform.rotation * Vector3.Scale((Vector3)hit.stateVar[i].offset, hit.transform.lossyScale);
                Vector3 origin = hit.transform.position + hit.transform.rotation * Vector3.Scale((Vector3)col.offset, hit.transform.lossyScale);
                Vector3 posDifference = prevPos - origin;
                float differenceAngle = Mathf.Atan2(posDifference.y, posDifference.x) * Mathf.Rad2Deg;
    
                if (col is CircleCollider2D)
                {
                    var circle = col as CircleCollider2D;
                    var size = circle.radius * hit.transform.lossyScale.y + hit.AdditionalRange();
                    Collider2D[] hits = Physics2D.OverlapCircleAll(origin, size, hit.HitLayers);
                    Collider2D[] hits2 = Physics2D.OverlapBoxAll((prevPos + origin) / 2, new Vector2(posDifference.magnitude, size * 2), differenceAngle, hit.HitLayers);
                    Collider2D[] hits3 = Physics2D.OverlapCircleAll(prevPos, size, hit.HitLayers);
    
                    RegisterHits(hits, col, hit);
                    RegisterHits(hits2, col, hit);
                    RegisterHits(hits3, col, hit);
    
                    hit.InvokeBoxDraw(new BoxDraw(origin, size * 2, BoxDraw.BoxShape.circle, hit));
                    hit.InvokeBoxDraw(new BoxDraw((prevPos + origin) / 2, new Vector2(posDifference.magnitude, size * 2), BoxDraw.BoxShape.box, hit, differenceAngle));
                    hit.InvokeBoxDraw(new BoxDraw(prevPos, size * 2, BoxDraw.BoxShape.circle, hit));
                }
                else if (col is BoxCollider2D)
                {
                    var box = col as BoxCollider2D;
                    var size = box.size * hit.transform.lossyScale.y + 2 * hit.AdditionalRange() * Vector2.one;
                    var interpolateWidth = (size.x > size.y) ? size.y : size.x;
    
                    Collider2D[] hits = Physics2D.OverlapBoxAll(origin, size, hit.transform.eulerAngles.z, hit.HitLayers);
                    Collider2D[] hits2 = Physics2D.OverlapBoxAll((prevPos + origin) / 2, new Vector2(posDifference.magnitude, interpolateWidth), differenceAngle, hit.HitLayers);
                    Collider2D[] hits3 = Physics2D.OverlapBoxAll(prevPos, size, hit.transform.eulerAngles.z, hit.HitLayers);
    
                    RegisterHits(hits, col, hit);
                    RegisterHits(hits2, col, hit);
                    RegisterHits(hits3, col, hit);
    
                    hit.InvokeBoxDraw(new BoxDraw(origin, size, BoxDraw.BoxShape.box, hit, hit.transform.eulerAngles.z));
                    hit.InvokeBoxDraw(new BoxDraw((prevPos + origin) / 2, new Vector2(posDifference.magnitude, interpolateWidth), BoxDraw.BoxShape.box, hit, differenceAngle));
                    hit.InvokeBoxDraw(new BoxDraw(prevPos, size, BoxDraw.BoxShape.box, hit, hit.transform.eulerAngles.z));
                }
            }
        }
    
        private void RegisterHits(Collider2D[] hits, Collider2D hitcol, HitBox hitbox)
        {
            foreach (var hit in hits)
            {
                if (hit.transform == transform)
                    continue;
                AddHitToList(hit, hitcol, hitbox);
            }
        }
    
        private void AddHitToList(Collider2D hit, Collider2D hitcol, HitBox hitbox)
        {
            InteractBox interactBox;
            if (hit.TryGetComponent(out interactBox) || hit.transform.parent.TryGetComponent(out interactBox))
            {
                if (interactBox.entity == hitbox.entity)
                    return;
                if (interactBox.ignoreHitbox && hitbox is HitBox)
                    return;
                if (hitbox.owner == interactBox.owner && (!hitbox.hitByOwner || hitbox is GrazeBox) && (!interactBox.hitByOwner || hitbox is GrazeBox) && (!hitbox.hitProperties.hitOwner || hitbox is GrazeBox))
                    return;
                if (hitbox.hitProperties.hitGroundedOnly && interactBox.owner.stateVars.aerial)
                    return;
                foreach (HitObject obj in hitObjects)
                {
                    if (obj.box == interactBox)
                        return;
                    //Shield owner from their any hitbox with seperate entity
                    if (obj.box is ShieldBox && obj.box.owner == interactBox.entity)
                        return;
                    if (interactBox is ShieldBox && obj.box.entity == interactBox.owner)
                    {
                        hitObjects.Add(new HitObject(interactBox, hitbox, hitcol, hit, obj.hitTimer));
                        hitObjects.Remove(obj);
                        return;
                    }
                    //Override hitbox in HitObjects if it exists in list
                    if (interactBox is HitBox && obj.box.entity == interactBox.entity && !(obj.box is HitBox))
                    {
                        hitObjects.Add(new HitObject(interactBox, hitbox, hitcol, hit, hitbox.hitProperties.hitCoolDown));
                        hitObjects.Remove(obj);
                        return;
                    }
                    //Shield owner from their own hitbox with seperate entity
                    if (obj.box is HitBox && obj.box.owner == interactBox.entity && interactBox.owner == hitbox.owner)
                        return;
                    if (interactBox is HitBox && obj.box.entity == interactBox.owner && interactBox.owner == hitbox.owner)
                    {
                        hitObjects.Add(new HitObject(interactBox, hitbox, hitcol, hit, obj.hitTimer));
                        hitObjects.Remove(obj);
                        return;
                    }
                }
                hitObjects.Add(new HitObject(interactBox, hitbox, hitcol, hit, hitbox.hitProperties.hitCoolDown));
            }
        }
    
    
        protected virtual void ActOnHit(HitObject obj)
        {
            if (obj.hitTimer == obj.hitbox.hitProperties.hitCoolDown && !obj.box.entity.stateVars.intangible)
            {
                if (obj.hitbox.OnHit != null)
                    obj.hitbox.OnHit.Invoke(obj);
                if (obj.box.OnHitted != null)
                    obj.box.OnHitted.Invoke(obj);
                if (OnHitEvent != null)
                    OnHitEvent(obj);
            }
        }
    }
    
    
    [System.Serializable]
    public struct HitObject
    {
        public InteractBox box;
        public HitBox hitbox;
        public Collider2D hitCol;
        public Collider2D hitBy;
        public float hitTimer;
    
        public HitObject(InteractBox box, HitBox hit, Collider2D hitBy, Collider2D hitcol)
        {
            this.box = box;
            this.hitbox = hit;
            this.hitBy = hitBy;
            this.hitCol = hitcol;
            hitTimer = 0;
        }
        public HitObject(InteractBox box, HitBox hit, Collider2D hitBy, Collider2D hitcol, float time)
        {
            this.box = box;
            this.hitbox = hit;
            this.hitBy = hitBy;
            this.hitCol = hitcol;
            this.hitTimer = time;
        }
    
        public HitObject UpdateHit(float timeScale)
        {
            hitTimer -= timeScale * Time.fixedDeltaTime;
            return this;
        }
    }}
