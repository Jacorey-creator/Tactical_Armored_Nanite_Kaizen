using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Unity.AI.Navigation;
using static UnityEditor.Experimental.GraphView.GraphView;
using Assets.Scripts.Enviornment.MapGeneration;
using Assets.Scripts.Player;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class ProceduralWorldGenerator : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField] private WorldGenerationConfig config = new WorldGenerationConfig();

    [Header("Player Settings")]
    [SerializeField] private Transform playerTransform;
    [SerializeField] private GameObject playerPrefab;

    [Header("NavMesh Settings")]
    [SerializeField] private NavMeshSurface navMeshSurface;

    // Dependencies (could be injected)
    private IPlayer player;
    private IChunkManager chunkManager;
    private ITerrainGenerator terrainGenerator;
    private IChunkLoadingStrategy loadingStrategy;
    private WorldGenerationEvents events;

    // State
    private Vector2Int currentPlayerChunk;
    private float navMeshUpdateTimer;
    private bool isInitialized;

    private void Awake()
    {
        InitializeDependencies();
        InitializeNavMesh();
        InitializeSeed();
    }

    private void InitializeDependencies()
    {
        // Initialize dependencies (in real project, could use DI container)
        chunkManager = new ChunkManager();
        terrainGenerator = new TerrainChunkGenerator();
        loadingStrategy = new CircularChunkLoadingStrategy();
        events = new WorldGenerationEvents();

        // Subscribe to events
        events.OnChunkCreated += OnChunkCreated;
        events.OnChunkDestroyed += OnChunkDestroyed;
    }

    private void InitializeNavMesh()
    {
        if (navMeshSurface == null)
        {
            navMeshSurface = GetComponent<NavMeshSurface>();
            if (navMeshSurface == null)
            {
                navMeshSurface = gameObject.AddComponent<NavMeshSurface>();
            }
        }
        navMeshSurface.collectObjects = CollectObjects.All;
    }

    private void InitializeSeed()
    {
        if (config.worldSeed == 0f)
        {
            config.worldSeed = UnityEngine.Random.Range(0f, 10000f);
        }
        UnityEngine.Random.InitState((int)config.worldSeed);
    }

    private void Start()
    {
        InitializePlayer();
        if (player != null && player.IsValid)
        {
            InitializeWorld();
        }
    }

    private void InitializePlayer()
    {
        if (playerTransform == null)
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
            if (playerObject != null)
            {
                playerTransform = playerObject.transform;
            }
        }

        if (playerTransform != null)
        {
            player = new UnityPlayer(playerTransform);
            player.OnPositionChanged += OnPlayerPositionChanged;
        }
    }

    private void InitializeWorld()
    {
        currentPlayerChunk = GetChunkPosition(player.Position);
        GenerateChunksAroundPlayer();
        isInitialized = true;
    }

    private void Update()
    {
        if (!isInitialized || player == null || !player.IsValid)
            return;

        // Update player (for position change detection)
        if (player is UnityPlayer unityPlayer)
        {
            unityPlayer.Update();
        }

        // Update NavMesh periodically
        UpdateNavMeshTimer();
    }

    private void UpdateNavMeshTimer()
    {
        navMeshUpdateTimer += Time.deltaTime;
        if (navMeshUpdateTimer >= config.navMeshUpdateInterval)
        {
            navMeshUpdateTimer = 0f;
            UpdateNavMesh();
        }
    }

    private void OnPlayerPositionChanged(Vector3 newPosition)
    {
        Vector2Int newPlayerChunk = GetChunkPosition(newPosition);
        if (newPlayerChunk != currentPlayerChunk)
        {
            currentPlayerChunk = newPlayerChunk;
            GenerateChunksAroundPlayer();
        }
    }

    private Vector2Int GetChunkPosition(Vector3 worldPosition)
    {
        return new Vector2Int(
            Mathf.FloorToInt(worldPosition.x / config.chunkSize),
            Mathf.FloorToInt(worldPosition.z / config.chunkSize)
        );
    }

    private void GenerateChunksAroundPlayer()
    {
        HashSet<Vector2Int> chunksToKeep = loadingStrategy.GetChunksToLoad(currentPlayerChunk, config.renderDistance);

        // Create new chunks
        foreach (Vector2Int chunkPos in chunksToKeep)
        {
            if (!chunkManager.HasChunk(chunkPos))
            {
                CreateChunk(chunkPos);
            }
        }

        // Remove old chunks
        List<Vector2Int> chunksToRemove = new List<Vector2Int>();
        foreach (Vector2Int activeChunk in chunkManager.GetActiveChunkPositions())
        {
            if (!chunksToKeep.Contains(activeChunk))
            {
                chunksToRemove.Add(activeChunk);
            }
        }

        foreach (Vector2Int chunkPos in chunksToRemove)
        {
            DestroyChunk(chunkPos);
        }
    }

    private void CreateChunk(Vector2Int chunkPosition)
    {
        GameObject chunk = terrainGenerator.GenerateChunk(chunkPosition, config, transform);
        chunkManager.AddChunk(chunkPosition, chunk);
        events.NotifyChunkCreated(chunkPosition, chunk);
    }

    private void DestroyChunk(Vector2Int chunkPosition)
    {
        chunkManager.RemoveChunk(chunkPosition);
        events.NotifyChunkDestroyed(chunkPosition);
    }

    private void UpdateNavMesh()
    {
        if (navMeshSurface != null)
        {
            navMeshSurface.BuildNavMesh();
            events.NotifyNavMeshUpdated();
        }
    }

    // Event handlers
    private void OnChunkCreated(Vector2Int position, GameObject chunk)
    {
        Debug.Log($"Chunk created at {position}");
    }

    private void OnChunkDestroyed(Vector2Int position)
    {
        Debug.Log($"Chunk destroyed at {position}");
    }

    // Public API
    public void SetPlayer(Transform newPlayerTransform)
    {
        playerTransform = newPlayerTransform;
        InitializePlayer();
        if (player != null && player.IsValid)
        {
            InitializeWorld();
        }
    }

    public WorldGenerationConfig GetConfig()
    {
        return config;
    }

    public WorldGenerationEvents GetEvents()
    {
        return events;
    }

    private void OnDestroy()
    {
        // Cleanup
        if (events != null)
        {
            events.OnChunkCreated -= OnChunkCreated;
            events.OnChunkDestroyed -= OnChunkDestroyed;
        }

        if (player != null)
        {
            player.OnPositionChanged -= OnPlayerPositionChanged;
        }

        chunkManager?.Clear();
    }
}