using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class PausedState : IGameState
{
    public void Enter(GameManager gameManager)
    {
        Debug.Log("Entered Paused State");
        Time.timeScale = 0f;
        // Show pause menu
    }

    public void Update(GameManager gameManager)
    {
        // Pause menu updates (if needed)
    }

    public void HandleInput(GameManager gameManager)
    {
        // Handle pause menu input
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            gameManager.ChangeState(new PlayingState());
        }
        else if (Input.GetKeyDown(KeyCode.Q))
        {
            gameManager.ChangeState(new MainMenuState());
        }
    }

    public void Exit(GameManager gameManager)
    {
        Debug.Log("Exited Paused State");
        Time.timeScale = 1f;
        // Hide pause menu
    }
}

