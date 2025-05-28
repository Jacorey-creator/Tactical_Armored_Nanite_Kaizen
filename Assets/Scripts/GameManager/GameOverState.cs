using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class GameOverState : IGameState
{
    public void Enter(GameManager gameManager)
    {
        Debug.Log("Entered Game Over State");
        // Show game over screen
        // Display final score
    }

    public void Update(GameManager gameManager)
    {
        // Game over screen updates
    }

    public void HandleInput(GameManager gameManager)
    {
        // Handle game over input
        if (Input.GetKeyDown(KeyCode.R))
        {
            gameManager.ChangeState(new PlayingState());
        }
        else if (Input.GetKeyDown(KeyCode.M))
        {
            gameManager.ChangeState(new MainMenuState());
        }
    }

    public void Exit(GameManager gameManager)
    {
        Debug.Log("Exited Game Over State");
        // Hide game over screen
    }
}

