using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

    public class PlayingState : IGameState
    {
        public void Enter(GameManager gameManager)
        {
            Debug.Log("Entered Playing State");
            gameManager.SpawnPlayer();
            // Show game UI
            // Resume game systems
        }

        public void Update(GameManager gameManager)
        {
            // Handle game logic updates
            // Check win/lose conditions
        }

        public void HandleInput(GameManager gameManager)
        {
            // Handle game input
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                gameManager.ChangeState(new PausedState());
            }
        }

        public void Exit(GameManager gameManager)
        {
            Debug.Log("Exited Playing State");
            // Pause game systems if needed
        }
    }
