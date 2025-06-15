using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TightStuff
{
    public class TankXTimesSceneLoadDestroy : MonoBehaviour
    {
        public int tanked = 1;
        // Start is called before the first frame update
        void Start()
        {
            DontDestroyOnLoad(this.gameObject);
            SceneManager.sceneLoaded += CheckAndDestroy;
        }

        private void OnDestroy()
        {
            SceneManager.sceneLoaded -= CheckAndDestroy;
        }

        public void CheckAndDestroy(Scene scene, LoadSceneMode mode)
        {
            if (tanked <= 0)
            {
                Destroy(gameObject);
            }
            tanked--;
        }
    }
}
