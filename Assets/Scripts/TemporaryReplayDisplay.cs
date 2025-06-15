namespace TightStuff
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;
    
    public class TemporaryReplayDisplay : MonoBehaviour
    {
        public static TemporaryReplayDisplay instance;
    
        public Text text;
    
        // Start is called before the first frame update
        void Start()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
                return;
            }
            Destroy(gameObject);
        }
    
        public void ChangeText(string t)
        {
            text.text = t;
        }
    }
}
