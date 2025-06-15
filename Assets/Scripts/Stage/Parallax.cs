namespace TightStuff.Stage
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    
    public class Parallax : MonoBehaviour
    {
        private Transform camPos;
        private Vector2 lastCamPos;
    
        [SerializeField]
        private float paraEff;
        [SerializeField]
        private float yMultiplier;
    
        // Start is called before the first frame update
        void Start()
        {
            camPos = Camera.main.transform;
            lastCamPos = camPos.position;
        }
    
        // Update is called once per frame
        void Update()
        {
            Vector2 deltaMov = (Vector2)camPos.position - lastCamPos;
            transform.localPosition += (Vector3)(deltaMov * paraEff * new Vector2(1, 1 + yMultiplier));
            lastCamPos = camPos.position;
        }
    }
}
