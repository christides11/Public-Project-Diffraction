namespace TightStuff.Menu
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    
    public class ColorAndPaletteAssigner : MonoBehaviour
    {
        public static ColorAndPaletteAssigner instance;
        public List<PlayerColor> colors;
        public List<PaletteOccupied> fighterPaletteOccupied;
    
        [System.Serializable]
        public class PlayerColor
        {
            public Material gemColor;
            public int occupied;
            public Color color;
        }
        [System.Serializable]
        public class PaletteOccupied
        {
            public List<int> occupied;
        }
    
        // Start is called before the first frame update
        void OnEnable()
        {
            if (instance == null)
                instance = this;
            else
            {
                DestroyImmediate(gameObject);
                return;
            }
            //DontDestroyOnLoad(gameObject);
            fighterPaletteOccupied = new List<PaletteOccupied>();
            for (int i = 0; i < 10; i++)
            {
                fighterPaletteOccupied.Add(new PaletteOccupied());
                fighterPaletteOccupied[i].occupied = new List<int>();
                fighterPaletteOccupied[i].occupied.Add(-1);
                fighterPaletteOccupied[i].occupied.Add(-1);
                fighterPaletteOccupied[i].occupied.Add(-1);
                fighterPaletteOccupied[i].occupied.Add(-1);
                fighterPaletteOccupied[i].occupied.Add(-1);
                fighterPaletteOccupied[i].occupied.Add(-1);
                fighterPaletteOccupied[i].occupied.Add(-1);
                fighterPaletteOccupied[i].occupied.Add(-1);
            }
        }
    
        public int GetPlayerColor(int id, int colorID1, int colorID2)
        {
            foreach (PlayerColor color in colors)
                if (color.occupied == id)
                    color.occupied = -1;
    
            if (colors[colorID1].occupied == -1)
            {
                colors[colorID1].occupied = id;
                return colorID1;
            }
            if (colors[colorID2].occupied == -1)
            {
                colors[colorID2].occupied = id;
                return colorID2;
            }
            for (int i = 0; i < colors.Count; i++)
            {
                PlayerColor color = colors[i];
                if (color.occupied == -1)
                {
                    color.occupied = id;
                    return i;
                }
            }
    
            return colors.Count - 1;
        }
        public int GetNextPlayerColor(int id, int current, int next)
        {
            foreach (PlayerColor color in colors)
                if (color.occupied == id)
                    color.occupied = -1;
    
            var adjusted2 = current;
            if (current == 0 && next < 0)
                adjusted2 = colors.Count - 1 - next;
    
            for (int i = adjusted2 + next; i < colors.Count && i >= 0; i += next)
            {
                PlayerColor color = colors[i];
                if (color.occupied == -1)
                {
                    color.occupied = id;
                    return i;
                }
            }
    
            return colors.Count - 1;
        }
        public int GetFighterPaletteID(int playerId, int fighterID)
        {
            var adjusted = fighterID - 1;
            if (fighterID == 0)
                adjusted = 0;
            //Debug.Log(adjusted);
            for (int i = 0; i < fighterPaletteOccupied.Count; i++)
                for (int j = 0; j < fighterPaletteOccupied[i].occupied.Count; j++)
                    if (fighterPaletteOccupied[i].occupied[j] == playerId)
                        fighterPaletteOccupied[i].occupied[j] = -1;
    
            for (int i = 0; i < fighterPaletteOccupied[adjusted].occupied.Count; i++)
                if (fighterPaletteOccupied[adjusted].occupied[i] == -1)
                {
                    fighterPaletteOccupied[adjusted].occupied[i] = playerId;
                    return i;
                }
    
            return 0;
        }
        public int GetNextFighterPaletteID(int playerId, int fighterID, int current, int next)
        {
            var adjusted = fighterID - 1;
            if (fighterID == 0)
                adjusted = 0;
    
            for (int i = 0; i < fighterPaletteOccupied.Count; i++)
                for (int j = 0; j < fighterPaletteOccupied[i].occupied.Count; j++)
                    if (fighterPaletteOccupied[i].occupied[j] == playerId)
                        fighterPaletteOccupied[i].occupied[j] = -1;
            var adjusted2 = current;
    
            for (int k = 0; k <= 8; k++)
            {
                adjusted2 += next;
                if (adjusted2 < 0)
                    adjusted2 = fighterPaletteOccupied[adjusted].occupied.Count - 1;
                else if (adjusted2 > fighterPaletteOccupied[adjusted].occupied.Count - 1)
                    adjusted2 = 0;
                Debug.Log(adjusted2);
                if (fighterPaletteOccupied[adjusted].occupied[adjusted2] == -1)
                {
                    fighterPaletteOccupied[adjusted].occupied[adjusted2] = playerId;
                    return adjusted2;
                }
            }
    
            return 0;
        }
        
        public void UnoccupyPlayer(int id)
        {
            foreach (PlayerColor color in colors)
                if (color.occupied == id)
                    color.occupied = -1;
            for (int i = 0; i < fighterPaletteOccupied.Count; i++)
                for (int j = 0; j < fighterPaletteOccupied[i].occupied.Count; j++)
                    if (fighterPaletteOccupied[i].occupied[j] == id)
                        fighterPaletteOccupied[i].occupied[j] = -1;
        }

        public int GetColorIdBasedOnColor(Color color)
        {
            for (int i = 0; i < colors.Count; i++)
            {
                if (colors[i].color == color)
                    return i;
            }
            Debug.Log("No corresponding color!");
            return -1;
        }

        // Update is called once per frame
        void Update()
        {
            
        }

        private void OnDestroy()
        {
            instance = null;
        }
    }
}
