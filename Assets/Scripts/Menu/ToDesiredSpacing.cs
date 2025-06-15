namespace TightStuff.Menu
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;
    
    public class ToDesiredSpacing : MonoBehaviour
    {
        [SerializeField]
        private float lerpSpeed = 5f;
    
        private GridLayoutGroup grid;
        private Vector2 desiredSpacing;
    
        // Start is called before the first frame update
        void Start()
        {
            grid = GetComponent<GridLayoutGroup>();
            desiredSpacing = grid.spacing;
        }
    
        // Update is called once per frame
        void Update()
        {
            grid.spacing = Vector2.Lerp(grid.spacing, desiredSpacing, Time.deltaTime);
        }
    
        public void SetSpacingX(int x)
        {
            desiredSpacing.x = x;
        }
    }
}
