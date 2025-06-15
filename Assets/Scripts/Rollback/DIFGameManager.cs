namespace TightStuff.Rollback
{
    ﻿using UnityGGPO;
    using SharedGame;
    using System.Collections.Generic;
    using UnityEngine.InputSystem;
    
    public class DIFGameManager : GameManager
    {
        public MatchManager matchManager;
        public Runner runner;
    
        public override void StartLocalGame()
        {
            InputSystem.settings.updateMode = InputSettings.UpdateMode.ProcessEventsManually;
            StartGame(new LocalRunner(new DIFGame(matchManager, 0)));
            MatchManager.local = true;
            matchManager.controllers[0].input.enabled = true;
            matchManager.runner = runner;
            runner.Start();
        }
    
        public override void StartGGPOGame(IPerfUpdate perfPanel, IList<Connections> connections, int playerIndex)
        {
            InputSystem.settings.updateMode = InputSettings.UpdateMode.ProcessEventsManually;
            var game = new GGPORunner("difraction", new DIFGame(matchManager, playerIndex), perfPanel);
            game.Init(connections, playerIndex);
            MatchManager.local = false;
            matchManager.controllers[playerIndex].input.enabled = true;
            matchManager.runner = runner;
            runner.Start();
    
            StartGame(game);
        }
    }
}
