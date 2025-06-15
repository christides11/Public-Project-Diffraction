namespace TightStuff
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.InputSystem;
    
    public class Trainer : MonoBehaviour
    {
        MatchManager match;
    
        public bool frameByFrame;
    
        // Start is called before the first frame update
        void Start()
        {
            match = GetComponent<MatchManager>();
        }
    
        private void Update()
        {
            MatchManager.paused = frameByFrame;
    
            SetPauseInputUpdateMode();
    
            if (match.controllers[0].submenuButton.Tapped())
                MatchManager.paused = false;
        }
    
        private void SetPauseInputUpdateMode()
        {
            if (MatchManager.FrameNum <= 0)
                return;
    
            if (!MatchManager.paused)
                InputSystem.settings.updateMode = InputSettings.UpdateMode.ProcessEventsManually;
            else
                InputSystem.settings.updateMode = InputSettings.UpdateMode.ProcessEventsInDynamicUpdate;
        }
    
    }
}
