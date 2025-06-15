using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TightStuff
{
    public class Description : MonoBehaviour
    {
        public static Description instance;

        public Text descriptionText;

        // Start is called before the first frame update
        void Start()
        {
            if (instance != null)
            {
                Destroy(gameObject);
                return;
            }
            descriptionText = GetComponent<Text>();
            instance = this;
        }

    }
}
