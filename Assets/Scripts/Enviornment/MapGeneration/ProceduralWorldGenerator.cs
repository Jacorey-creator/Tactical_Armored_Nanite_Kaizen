using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Unity.AI.Navigation;


#if UNITY_EDITOR
using UnityEditor;
#endif

public class ProceduralWorldGenerator : MonoBehaviour
{
    [Header("Player Settings")]
    [SerializeField] private Transform playerTransform;
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private Vector3 initialPlayerPosition = new Vector3(0, 0, 0);

    [Header("World Generation")]
    [SerializeField] private int renderDistance = 3;
    [SerializeField] private int chunkSize = 20;
    [SerializeField] private float heightScale = 1.5f;
    [SerializeField] private float noiseScale = 0.3f;
    [SerializeField] private float worldSeed = 0f;

    [Header("NavMesh Settings")]
    [SerializeField] private NavMeshSurface navMeshSurface;
    [SerializeField] private float navMeshUpdateInterval = 1.0f;

    // Dictionary to track chunks by their grid position
    private Dictionary<Vector2Int, GameObject> activeChunks = new Dictionary<Vector2Int, GameObject>();
    private Vector2Int currentPlayerChunk = new Vector2Int(0, 0);
    private float navMeshUpdateTimer = 0f;
    private bool chunksInitialized = false;


    private void Awake()
    {
        // Ensure we have a NavMeshSurface component
        if (navMeshSurface == null)
        {
            navMeshSurface = GetComponent<NavMeshSurface>();
            if (navMeshSurface == null)
            {
                navMeshSurface = gameObject.AddComponent<NavMeshSurface>();
            }
        }

        // Set a random seed if not specified
        if (worldSeed == 0f)
        {
            worldSeed = Random.Range(0f, 10000f);
        }
        
        navMeshSurface.collectObjects = CollectObjects.All;
        Random.InitState((int)worldSeed);
    }

    private void Start()
    {
        // If no player transform is assigned, try to find one
        if (playerTransform == null)
        {
            playerTransform = GameObject.FindGameObjectWithTag("Player")?.transform;
        }

        // Generate initial chunks around player
        UpdatePlayerChunkPosition();
        GenerateChunksAroundPlayer();
    }

    private void Update()
    {
        if (playerTransform == null) return;

        // Check if player has moved to a new chunk
        Vector2Int newPlayerChunk = GetChunkPosition(playerTransform.position);
        if (newPlayerChunk != currentPlayerChunk)
        {
            currentPlayerChunk = newPlayerChunk;
            GenerateChunksAroundPlayer();
        }

        // Update NavMesh periodically
        navMeshUpdateTimer += Time.deltaTime;
        if (navMeshUpdateTimer >= navMeshUpdateInterval)
        {
            navMeshUpdateTimer = 0f;
            UpdateNavMesh();
        }
    }

    private void UpdatePlayerChunkPosition()
    {
        if (playerTransform != null)
        {
            currentPlayerChunk = GetChunkPosition(playerTransform.position);
        }
    }

    private Vector2Int GetChunkPosition(Vector3 worldPosition)
    {
        return new Vector2Int(
            Mathf.FloorToInt(worldPosition.x / chunkSize),
            Mathf.FloorToInt(worldPosition.z / chunkSize)
        );
    }

    private void GenerateChunksAroundPlayer()
    {
        // Track which chunks should remain active
        HashSet<Vector2Int> chunksToKeep = new HashSet<Vector2Int>();

        // Generate chunks in render distance radius around player
        for (int x = -renderDistance; x <= renderDistance; x++)
        {
            for (int z = -renderDistance; z <= renderDistance; z++)
            {
                Vector2Int chunkPos = new Vector2Int(currentPlayerChunk.x + x, currentPlayerChunk.y + z);

                // Only generate if within our render distance circle
                if (Vector2Int.Distance(currentPlayerChunk, chunkPos) <= renderDistance)
                {
                    chunksToKeep.Add(chunkPos);

                    // Create chunk if it doesn't exist
                    if (!activeChunks.ContainsKey(chunkPos))
                    {
                        CreateChunk(chunkPos);
                    }
                }
            }
        }

        // Remove chunks outside render distance
        List<Vector2Int> chunksToRemove = new List<Vector2Int>();
        foreach (KeyValuePair<Vector2Int, GameObject> chunk in activeChunks)
        {
            if (!chunksToKeep.Contains(chunk.Key))
            {
                chunksToRemove.Add(chunk.Key);
            }
        }

        foreach (Vector2Int chunkPos in chunksToRemove)
        {
            DestroyChunk(chunkPos);
        }
    }
    private void CreateChunk(Vector2Int chunkPos)
    {
        // Create a new chunk GameObject
        GameObject chunkObject = new GameObject($"Chunk_{chunkPos.x}_{chunkPos.y}");
        chunkObject.transform.position = new Vector3(chunkPos.x * chunkSize, 0, chunkPos.y * chunkSize);
        chunkObject.transform.parent = transform;

        // Add necessary components
        TerrainMeshGenerator terrainGenerator = chunkObject.AddComponent<TerrainMeshGenerator>();
        MeshCollider meshCollider = chunkObject.AddComponent<MeshCollider>();

        // Set noise offset for this chunk to create continuous terrain
        float xOffset = chunkPos.x * chunkSize;
        float zOffset = chunkPos.y * chunkSize;
        terrainGenerator.SetNoiseOffset(xOffset, zOffset);

        // Configure terrain generator
        terrainGenerator.UpdateTerrainParameters(
            chunkSize,
            chunkSize,
            heightScale,
            noiseScale
        );

        // Apply the terrain mesh to the collider
        meshCollider.sharedMesh = terrainGenerator.GetMesh();

        // Add to active chunks
        activeChunks.Add(chunkPos, chunkObject);
    }

    private void DestroyChunk(Vector2Int chunkPos)
    {
        if (activeChunks.TryGetValue(chunkPos, out GameObject chunk))
        {
            Destroy(chunk);
            activeChunks.Remove(chunkPos);
        }
    }

    private void UpdateNavMesh()
    {
        if (navMeshSurface != null)
        {
            navMeshSurface.BuildNavMesh();
        }
    }
    public void SetPlayer(Transform player)
    {
        playerTransform = player;
        InitializeChunks();
    }
    private void InitializeChunks()
    {
        UpdatePlayerChunkPosition();
        GenerateChunksAroundPlayer();
        chunksInitialized = true;
    }

    // Editor utility class for setting static flags
    private static class GameObjectUtility
    {
        public static void SetStaticEditorFlags(GameObject gameObject, StaticEditorFlags flags)
        {
#if UNITY_EDITOR
            UnityEditor.GameObjectUtility.SetStaticEditorFlags(gameObject, flags);
#endif
        }
    }
}