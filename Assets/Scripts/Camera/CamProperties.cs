namespace TightStuff
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.SceneManagement;
    
    public class CamProperties : UpdateAbstract
    {
        private List<Entity> _followTarget;
        [SerializeField]
        private Transform _camCenter;
    
        private Entity _entity;
    
    
        private float _XDamping;
        private float _YDamping;
    
        [SerializeField]
        private float initDamp;
        [SerializeField]
        private float dampAlt;
    
        [SerializeField]
        private float initXBound = 4;
        [SerializeField]
        private float initYMaxBound = 5;
        [SerializeField]
        private float initYMinBound = 0;
    
        [SerializeField]
        private float boundStepDistance = 2;
    
        private bool XMoved { get => _entity.stateVars.frameNum > 0; set => _entity.stateVars.frameNum = value? 1 : 0; }
        private bool YMoved { get => _entity.stateVars.gravity; set => _entity.stateVars.gravity = value; }
    
        // Start is called before the first frame update
        void Start()
        {
            _entity = GetComponent<Entity>();
    
            _camCenter.parent = null;
            _camCenter.position = Vector3.zero;
    
    
            GameObject[] rootGameObjects = SceneManager.GetActiveScene().GetRootGameObjects();
    
            _followTarget = new List<Entity>();
            foreach (var rootGameObject in rootGameObjects)
            {
                Entity[] es = rootGameObject.GetComponentsInChildren<Entity>();
                foreach (Entity e in es)
                {
                    if (e.followTarget)
                        _followTarget.Add(e);
                }
            }
        }
    
        // Update is called once per frame
        public override void GUpdate()
        {
    
            var x = 0f;
            var y = 0f;
            var midPoint = Vector2.zero;
    
            var followCount = 0;
    
            CalculateMidpoint(ref x, ref y, ref midPoint, ref followCount);
    
            SnapToStageCenter(ref x, ref y, ref midPoint);
    
            CalculateDesiredCameraPosition(x, y, midPoint, followCount);
    
            FuzzyCameraRecenter();
    
            InterpolateCameraPosition();
        }
    
        private void InterpolateCameraPosition()
        {
            var camX = Mathf.Lerp(transform.position.x, _camCenter.position.x, _XDamping / 30);
            var camY = Mathf.Lerp(transform.position.y, _camCenter.position.y, _YDamping / 30);
            transform.position = new Vector3(camX, camY, -10);
        }
    
        private void FuzzyCameraRecenter()
        {
            if (Mathf.Abs(_camCenter.position.x) <= initXBound - (XMoved ? boundStepDistance : 0))
            {
                _camCenter.position *= Vector2.up;
                _XDamping = dampAlt;
                XMoved = false;
            }
            else
            {
                _XDamping = initDamp;
                XMoved = true;
            }
            if (_camCenter.position.y <= initYMaxBound - (YMoved ? boundStepDistance : 0) && _camCenter.position.y >= initYMinBound + (YMoved ? boundStepDistance : 0))
            {
                _camCenter.position = new Vector2(_camCenter.position.x, 2.5f);
                _YDamping = dampAlt;
                YMoved = false;
            }
            else
            {
                _YDamping = initDamp;
                YMoved = true;
            }
        }
    
        private void CalculateDesiredCameraPosition(float x, float y, Vector2 midPoint, int followCount)
        {
            _camCenter.position = (new Vector2(x, y) * 0.5f + midPoint / followCount) / 2f;
        }
    
        private void SnapToStageCenter(ref float x, ref float y, ref Vector2 midPoint)
        {
            if (Mathf.Abs(midPoint.x) <= initXBound - (XMoved ? boundStepDistance : 0))
            {
                midPoint *= Vector2.up;
                x = 0;
            }
            if (midPoint.y <= initYMaxBound - (YMoved ? boundStepDistance : 0) && midPoint.y >= initYMinBound + (YMoved ? boundStepDistance : 0))
            {
                midPoint = new Vector2(midPoint.x, 2.5f);
                y = 2.5f;
            }
        }
    
        private void CalculateMidpoint(ref float x, ref float y, ref Vector2 midPoint, ref int followCount)
        {
            foreach (Entity en in _followTarget)
            {
                followCount++;
                /*
                if (!en.enabled)
                {
                    midPoint += Vector2.up;
                    if (Mathf.Abs(x) < 0)
                        x = en.transform.position.x;
                    if (Mathf.Abs(y) < 1)
                        y = en.transform.position.y;
                    continue;
                }*/
                midPoint += (Vector2)en.transform.position;
                if (Mathf.Abs(x) < Mathf.Abs(en.transform.position.x))
                    x = en.transform.position.x;
                if (Mathf.Abs(y) < Mathf.Abs(en.transform.position.y))
                    y = en.transform.position.y;
            }
            if (followCount == 0)
                followCount = 1;
        }
    }
}
