namespace TightStuff.Menu
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.EventSystems;
    
    public class CursorTarget : MonoBehaviour
    {
        public List<CursorTarget> sharedTargets;
    
        public Vector3 target;
    
        public UnityEvent<Vector3> OnDeselectEvent;
        public UnityEvent<Vector3> OnSelectEvent;
        // Start is called before the first frame update
        void Start()
        {
            
        }
    
        // Update is called once per frame
        void Update()
        {
    
        }
    
        public void SharedButtonOffsets(ref Vector3 target)
        {
            var count = 0;
            for (int i = 0; i < sharedTargets.Count; i++)
            {
                float angleInRadians = 0;
                if (sharedTargets[i].target != null)
                {
                    if (count != 0)
                    {
                        angleInRadians = -Mathf.Deg2Rad * ((360 / count) + 180 + (count == 3 ? 30 : 0));
                    }
                    float xComponent = Mathf.Cos(angleInRadians);
                    float yComponent = Mathf.Sin(angleInRadians);
                    Vector2 unitVector = new Vector2(xComponent, yComponent) * (count <= 1 ? 0 : 1);
    
                    target = unitVector * 25;
                    count++;
                }
            }
        }
        public void SharedButtonSliderOffset(ref Vector3 target)
        {
            var count = 0;
            for (int i = 0; i < sharedTargets.Count; i++)
            {
                float angleInRadians = 0;
                if (sharedTargets[i].target != null)
                {
                    if (count != 0)
                    {
                        angleInRadians = -Mathf.Deg2Rad * ((360 / count) + 180 + (count == 3 ? 30 : 0));
                    }
                    float xComponent = Mathf.Cos(angleInRadians);
                    float yComponent = Mathf.Sin(angleInRadians);
                    Vector2 unitVector = new Vector2(xComponent, yComponent) * (count <= 1 ? 0 : 1);
    
                    target = unitVector * 25;
                    count++;
                }
            }
        }
    }
}
