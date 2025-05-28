using UnityEngine;
using System.Collections;

/// <summary>
/// Integrates the procedural world generation with the GameManager
/// This script bridges between your existing game structure and the world generation system
/// </summary>
public class WorldGenerationIntegrator : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private ProceduralWorldGenerator worldGenerator;
    [SerializeField] private NavMeshManager navMeshManager;

    [Header("Integration Settings")]
    [SerializeField] private float initialGenerationDelay = 0.5f;
    [SerializeField] private bool centerFirstChunkOnPlayer = true;

    public ProceduralWorldGenerator WorldGenerator => worldGenerator;

    private void Awake()
    {
        // Find components if not assigned
        if (worldGenerator == null)
        {
            worldGenerator = GetComponent<ProceduralWorldGenerator>();
            if (worldGenerator == null)
            {
                worldGenerator = GetComponentInChildren<ProceduralWorldGenerator>();
                if (worldGenerator == null)
                {
                    Debug.LogError("WorldGenerationIntegrator: No ProceduralWorldGenerator found!");
                }
            }
        }

        if (navMeshManager == null)
        {
            navMeshManager = GetComponent<NavMeshManager>();
            if (navMeshManager == null)
            {
                navMeshManager = GetComponentInChildren<NavMeshManager>();
            }
        }
    }

    private void Start()
    {
        // Listen for game start events
        GameManager gameManager = GameManager.Instance;
        if (gameManager != null)
        {
            Debug.Log("WorldGenerationIntegrator: Connected to GameManager");

            // Wait a moment for player to be spawned, then initialize world
            StartCoroutine(DelayedWorldInitialization());
        }
        else
        {
            Debug.LogWarning("WorldGenerationIntegrator: No GameManager found. World will generate immediately.");
        }
    }

    private IEnumerator DelayedWorldInitialization()
    {
        // Wait for the player to be spawned by GameManager
        yield return new WaitForSeconds(initialGenerationDelay);

        // Try to find player
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            Debug.Log("WorldGenerationIntegrator: Found player, initializing world generation");

            // Set player as NavMesh tracking target
            if (navMeshManager != null)
            {
                navMeshManager.SetTrackingTarget(player.transform);

                // Initial NavMesh build focused on player
                navMeshManager.RebuildNavMeshAtPosition(player.transform.position);
            }
        }
        else
        {
            Debug.LogWarning("WorldGenerationIntegrator: No player found after delay");
        }
    }

    // Called by GameManager when the game state changes
    public void OnGameStateChanged(IGameState newState)
    {
        if (newState is PlayingState)
        {
            // Enable systems when game starts or resumes
            if (worldGenerator != null)
                worldGenerator.enabled = true;

            if (navMeshManager != null)
                navMeshManager.enabled = true;
        }
        else if (newState is PausedState or MainMenuState or GameOverState)
        {
            // Disable systems when game is paused, over, or in main menu
            if (worldGenerator != null)
                worldGenerator.enabled = false;

            if (navMeshManager != null)
                navMeshManager.enabled = false;
        }
    }

    // If player dies and respawns, call this to update world generation
    public void OnPlayerRespawned(GameObject player)
    {
        if (player != null && navMeshManager != null)
        {
            navMeshManager.SetTrackingTarget(player.transform);
            navMeshManager.RebuildNavMeshAtPosition(player.transform.position);
        }
    }
}