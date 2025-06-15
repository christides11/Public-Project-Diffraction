namespace TightStuff
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.SceneManagement;
    
    public class ToNext : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            Invoke("next", 2f);
        }
    
        // Update is called once per frame
        void Update()
        {
            
        }
    
        void next()
        {
            SceneManager.LoadScene("PreBattle");
            //SceneManager.LoadScene("HakureiShrine");
        }
    }
}
