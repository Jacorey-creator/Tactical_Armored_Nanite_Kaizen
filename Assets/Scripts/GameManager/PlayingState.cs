using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class PlayingState : IGameState
{
    private bool hasSceneLoaded = false;

    public void Enter(GameManager gameManager)
    {
        Debug.Log("Entered Playing State");
        hasSceneLoaded = false;

        // Load the game scene first
        UnityEngine.SceneManagement.SceneManager.LoadScene("SampleScene"); 

        // Subscribe to scene loaded event to spawn player after scene loads
        UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, UnityEngine.SceneManagement.LoadSceneMode mode)
    {
        // Only spawn player if this is the game scene
        if (scene.name == "SampleScene")
        {
            hasSceneLoaded = true;
            GameManager.Instance.SpawnPlayer();
            // Show game UI
            // Resume game systems

            // Unsubscribe from the event
            UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }

    public void Update(GameManager gameManager)
    {
        // Only update game logic after scene has loaded
        if (hasSceneLoaded)
        {
            // Handle game logic updates
            // Check win/lose conditions
        }
    }

    public void HandleInput(GameManager gameManager)
    {
        // Only handle input after scene has loaded
        if (hasSceneLoaded)
        {
            // Handle game input
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                gameManager.ChangeState(new PausedState());
            }
        }
    }

    public void Exit(GameManager gameManager)
    {
        Debug.Log("Exited Playing State");
        // Unsubscribe from scene loaded event in case we exit before scene loads
        UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoaded;
        // Pause game systems if needed
    }
}
