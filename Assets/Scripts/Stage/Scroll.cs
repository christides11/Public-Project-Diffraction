namespace TightStuff.Stage
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    
    public class Scroll : MonoBehaviour
    {
        [SerializeField]
        private float scrollSpd = 2;
        [SerializeField]
        private float endPos;
        [SerializeField]
        private float endPosY;
        [SerializeField]
        private float startPos;
        [SerializeField]
        private float startPosY;
        [SerializeField]
        private Vector2 scrollDir = Vector2.left;
    
        private float travelledDir;
        private float posDiff;
    
    
        // Start is called before the first frame update
        void Start()
        {
            posDiff = Vector2.Distance(new Vector2(startPos, startPosY), new Vector2(endPos, endPosY));
            travelledDir = Vector2.Distance(transform.localPosition, new Vector2(startPos, startPosY));
        }
    
        // Update is called once per frame
        void FixedUpdate()
        {
            if (MatchManager.paused)
                return;
            transform.localPosition += scrollSpd * (Vector3)scrollDir.normalized * MatchManager.worldTime;
            travelledDir += Mathf.Abs(scrollSpd) * MatchManager.worldTime;
            if (travelledDir >= posDiff)
            {
                transform.localPosition -= travelledDir * (Vector3)scrollDir.normalized;
                travelledDir = 0;
            }
        }
    }
}
