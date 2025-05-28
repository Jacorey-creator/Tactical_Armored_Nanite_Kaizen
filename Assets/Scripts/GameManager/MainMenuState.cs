using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


    public class MainMenuState : IGameState
    {
        public void Enter(GameManager gameManager)
        {
            Debug.Log("Entered Main Menu State");
            // Show main menu UI
            // Hide game UI
        }

        public void Update(GameManager gameManager)
        {
            // Handle main menu updates
        }

        public void HandleInput(GameManager gameManager)
        {
            // Handle main menu input (start game, quit, etc.)
            if (Input.GetKeyDown(KeyCode.Space))
            {
                gameManager.ChangeState(new PlayingState());
            }
        }

        public void Exit(GameManager gameManager)
        {
            Debug.Log("Exited Main Menu State");
            // Hide main menu UI
        }
}

