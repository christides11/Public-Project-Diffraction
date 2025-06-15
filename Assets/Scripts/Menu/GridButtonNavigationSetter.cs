using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TightStuff.Menu
{
    public class GridButtonNavigationSetter : MonoBehaviour
    {
        public List<Selectable> buttons;
        public GridLayoutGroup grid;

        public Selectable bottomSelect;
        public Selectable topSelect;

        public bool reverseHorizontal;

        // Start is called before the first frame update
        void Start()
        {
            grid = GetComponent<GridLayoutGroup>();
        }

        public void FetchButtons()
        {
            Debug.Log("fuckshit");
            buttons = new List<Selectable>();
            foreach (Transform child in transform)
                buttons.Add(child.GetComponent<Selectable>());

            int rows = grid.constraintCount; // Fixed number of rows
            int cols = Mathf.CeilToInt((float)buttons.Count / rows);


            for (int i = 0; i < buttons.Count; i++)
            {
                Navigation nav = new Navigation
                {
                    mode = Navigation.Mode.Explicit
                };

                // Calculate grid position
                int row = i % rows;
                int col = i / rows;

                // Assign navigation directions
                nav.selectOnUp = row > 0 ? buttons[i - 1] : (topSelect != null ? topSelect : null);
                nav.selectOnDown = row < rows - 1 && i + 1 < buttons.Count ? buttons[i + 1] : (bottomSelect != null ? bottomSelect : null);
                nav.selectOnLeft = col > 0 ? buttons[i - rows] : null;
                nav.selectOnRight = col < cols - 1 && i + rows < buttons.Count? buttons[i + rows] : null;

                if (reverseHorizontal)
                {
                    var temp = nav.selectOnRight;
                    nav.selectOnRight = nav.selectOnLeft;
                    nav.selectOnLeft = temp;
                }

                buttons[i].navigation = nav;
            }
        }
    }
}
