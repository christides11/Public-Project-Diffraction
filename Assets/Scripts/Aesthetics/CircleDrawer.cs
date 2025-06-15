namespace TightStuff.Aesthetics
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    
    public class CircleDrawer : MonoBehaviour
    {
        private LineRenderer _line;
    
        [SerializeField]
        private int step = 10;
        [SerializeField]
        private float radius = 5;
        [SerializeField]
        private float width = 1;
    
    
        // Start is called before the first frame update
        void Start()
        {
            _line = GetComponent<LineRenderer>();
            _line.useWorldSpace = false;
            _line.loop = true;
        }
    
        // Update is called once per frame
        void Update()
        {
            DrawCircle(step, radius, width);
        }
    
        private void DrawCircle(int steps, float radius, float width)
        {
            _line.positionCount = steps;
            _line.widthMultiplier = width;
    
            for (int currentStep = 0; currentStep < steps; currentStep++)
            {
                float circumferenceProgress = (float)currentStep / (steps - 1);
                float currentRadian = circumferenceProgress * 2 * Mathf.PI;
    
                float xScaled = Mathf.Cos(currentRadian);
                float yScaled = Mathf.Sin(currentRadian);
    
                float x = xScaled * (radius - _line.widthMultiplier / 2);
                float y = yScaled * (radius - _line.widthMultiplier / 2);
    
                Vector3 currentPosition = new Vector3(x, y, 0);
    
                _line.SetPosition(currentStep, currentPosition);
            }
        }
    }
}
