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

            if (centerFirstChunkOnPlayer && worldGenerator != null)
            {
                // Offset the world generation so a chunk center is at the player's position
                // This makes the initial chunk more centered on the player spawn point
                AdjustGenerationToPlayerPosition(player.transform.position);
            }

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

    private void AdjustGenerationToPlayerPosition(Vector3 playerPosition)
    {
        // Logic to adjust world generation based on player position
        // This could adjust noise offsets or generation boundaries
        // to make the environment look better around the spawn point

        // Example: You might want to ensure the player spawns in a flat area
        // or in the center of a chunk rather than near a chunk boundary
    }

    // Called when the game state changes
    public void OnGameStateChanged(GameManager.GameState newState)
    {
        if (newState == GameManager.GameState.Playing)
        {
            // Game started or resumed - ensure world generation is active
            if (worldGenerator != null)
            {
                worldGenerator.enabled = true;
            }

            if (navMeshManager != null)
            {
                navMeshManager.enabled = true;
            }
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