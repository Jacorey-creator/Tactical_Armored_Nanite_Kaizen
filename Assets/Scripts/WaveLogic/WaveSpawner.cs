using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;
using UnityEngine.Events;

public class WaveSpawner : MonoBehaviour
{
    [Header("Wave Configuration")]
    public List<WaveController> waves = new();

    public bool loopWaves = false;

    public float loopDelay = 10f;

    [Header("Spawn Areas")]
    public List<SpawnArea> spawnAreas = new();

    public LayerMask groundLayer;

    [Header("Spawn Settings")]
    public float timeBetweenSpawns = 0.2f;

    public bool autoStart = true;

    [Header("Pool Settings")]
    public bool useObjectPooling = true;

    public int initialPoolSize = 20;

    [Header("Events")]
    public UnityEvent onAllWavesComplete;

    // Object pools for each prefab type
    private Dictionary<GameObject, Queue<GameObject>> objectPools = new Dictionary<GameObject, Queue<GameObject>>();

    // Current wave index
    private int currentWaveIndex = 0;

    // Whether waves are currently spawning
    private bool isSpawning = false;

    // List of active entities for tracking
    private List<GameObject> activeEntities = new();

    // Parent transform for organizing pooled objects
    private Transform poolParent;

    private void Start()
    {
        if (useObjectPooling)
        {
            InitializeObjectPools();
        }

        if (autoStart)
        {
            StartWaves();
        }
    }
    private void InitializeObjectPools()
    {
        // Create a parent for all pooled objects
        GameObject poolParentObj = new GameObject("Spawn_Pools");
        poolParent = poolParentObj.transform;

        // Identify all unique prefabs across all waves
        HashSet<GameObject> uniquePrefabs = new HashSet<GameObject>();
        foreach (var wave in waves)
        {
            foreach (var prefabCount in wave.prefabsToSpawn)
            {
                if (prefabCount.prefab != null)
                {
                    uniquePrefabs.Add(prefabCount.prefab);
                }
            }
        }

        // Initialize a pool for each unique prefab
        foreach (var prefab in uniquePrefabs)
        {
            // Create a parent for this prefab's pool
            GameObject prefabPoolParent = new GameObject(prefab.name + "_Pool");
            prefabPoolParent.transform.SetParent(poolParent);

            Queue<GameObject> pool = new Queue<GameObject>();

            // Pre-instantiate pool objects
            for (int i = 0; i < initialPoolSize; i++)
            {
                GameObject obj = InstantiatePoolObject(prefab, prefabPoolParent.transform);
                pool.Enqueue(obj);
            }

            objectPools.Add(prefab, pool);
        }
    }

    private GameObject InstantiatePoolObject(GameObject prefab, Transform parent)
    {
        GameObject obj = Instantiate(prefab, parent);
        obj.SetActive(false);
        return obj;
    }
    private GameObject GetPooledObject(GameObject prefab)
    {
        if (!useObjectPooling)
        {
            return Instantiate(prefab);
        }

        if (objectPools.TryGetValue(prefab, out Queue<GameObject> pool) && pool.Count > 0)
        {
            GameObject obj = pool.Dequeue();
            obj.SetActive(true);
            return obj;
        }
        else
        {
            // If pool is empty, instantiate a new object
            GameObject objParent = poolParent.Find(prefab.name + "_Pool")?.gameObject;
            if (objParent == null)
            {
                objParent = new GameObject(prefab.name + "_Pool");
                objParent.transform.SetParent(poolParent);
            }

            return Instantiate(prefab);
        }
    }
    public void ReturnToPool(GameObject obj, GameObject originalPrefab)
    {
        if (!useObjectPooling)
        {
            Destroy(obj);
            return;
        }

        if (objectPools.TryGetValue(originalPrefab, out Queue<GameObject> pool))
        {
            obj.SetActive(false);

            // Find the correct parent for this object
            Transform poolParentForPrefab = poolParent.Find(originalPrefab.name + "_Pool");
            if (poolParentForPrefab != null)
            {
                obj.transform.SetParent(poolParentForPrefab);
            }
            else
            {
                obj.transform.SetParent(poolParent);
            }

            pool.Enqueue(obj);

            // Remove from active entities
            activeEntities.Remove(obj);
        }
        else
        {
            Destroy(obj);
        }
    }

    public void StartWaves()
    {
        if (isSpawning)
        {
            StopAllCoroutines();
        }

        currentWaveIndex = 0;
        isSpawning = true;
        StartCoroutine(SpawnWavesCoroutine());
    }

    public void StartWavesFromIndex(int index)
    {
        if (isSpawning)
        {
            StopAllCoroutines();
        }

        currentWaveIndex = Mathf.Clamp(index, 0, waves.Count - 1);
        isSpawning = true;
        StartCoroutine(SpawnWavesCoroutine());
    }

    public void StopWaves()
    {
        isSpawning = false;
        StopAllCoroutines();
    }

    public void SkipToNextWave()
    {
        if (isSpawning)
        {
            StopAllCoroutines();
            currentWaveIndex++;

            if (currentWaveIndex >= waves.Count)
            {
                if (loopWaves)
                {
                    currentWaveIndex = 0;
                }
                else
                {
                    isSpawning = false;
                    onAllWavesComplete?.Invoke();
                    return;
                }
            }
            StartCoroutine(SpawnWavesCoroutine());
        }
    }

    private IEnumerator SpawnWavesCoroutine()
    {
        while (currentWaveIndex < waves.Count)
        {
            WaveController currentWave = waves[currentWaveIndex];

            // Wait for the delay before starting this wave
            yield return new WaitForSeconds(currentWave.delayBeforeWave);

            // Trigger wave start event
            currentWave.onWaveStart?.Invoke();

            yield return StartCoroutine(SpawnWaveEntitiesCoroutine(currentWave));

            // Trigger wave complete event
            currentWave.onWaveComplete?.Invoke();

            // Move to the next wave
            currentWaveIndex++;

            // Check if we've completed all waves
            if (currentWaveIndex >= waves.Count)
            {
                if (loopWaves)
                {
                    currentWaveIndex = 0;
                    yield return new WaitForSeconds(loopDelay);
                }
                else
                {
                    isSpawning = false;
                    onAllWavesComplete?.Invoke();
                    break;
                }
            }
        }
    }

    private IEnumerator SpawnWaveEntitiesCoroutine(WaveController wave)
    {
        foreach (var prefabCount in wave.prefabsToSpawn)
        {
            if (prefabCount.prefab == null || prefabCount.count <= 0)
                continue;

            for (int i = 0; i < prefabCount.count; i++)
            {
                // Get spawn position
                Vector3 spawnPosition;
                Quaternion spawnRotation = Quaternion.identity;

                if (wave.useSpecificSpawnPoints && wave.spawnPoints.Count > 0)
                {
                    // Use a specific spawn point
                    int pointIndex = Random.Range(0, wave.spawnPoints.Count);
                    Transform spawnPoint = wave.spawnPoints[pointIndex];

                    if (spawnPoint != null)
                    {
                        spawnPosition = spawnPoint.position;
                        spawnRotation = spawnPoint.rotation;
                    }
                    else
                    {
                        // Fallback to a random position if spawn point is null
                        spawnPosition = GetRandomSpawnPosition();
                    }
                }
                else
                {
                    // Use a random position in spawn areas
                    spawnPosition = GetRandomSpawnPosition();
                }

                // Spawn the entity
                GameObject entity = GetPooledObject(prefabCount.prefab);
                entity.transform.position = spawnPosition;
                entity.transform.rotation = spawnRotation;

                // Add to active entities list for tracking
                activeEntities.Add(entity);

                // Wait before spawning the next entity
                yield return new WaitForSeconds(timeBetweenSpawns);
            }
        }
    }

    private Vector3 GetRandomSpawnPosition()
    {
        if (spawnAreas.Count == 0)
        {
            Debug.LogWarning("No spawn areas defined!");
            return transform.position;
        }

        // Select a random spawn area
        SpawnArea area = spawnAreas[Random.Range(0, spawnAreas.Count)];

        if (area.areaCenter == null)
        {
            Debug.LogWarning("Spawn area center is null!");
            return transform.position;
        }

        // Get random point within sphere
        Vector3 randomPos = area.areaCenter.position + Random.insideUnitSphere * area.radius;

        // If using Nav Mesh, adjust position to be on nav mesh (would need NavMesh components)
        if (area.useNavMesh)
        {
            // Use NavMesh sampling to find a valid position
            NavMeshHit hit;
            float maxDistance = area.radius;

            // Sample position on NavMesh
            if (NavMesh.SamplePosition(randomPos, out hit, maxDistance, NavMesh.AllAreas))
            {
                // Use the position found on the NavMesh
                randomPos = hit.position;

                randomPos.y = area.minHeightFromGround > 0 ? hit.position.y + area.minHeightFromGround : hit.position.y;
            }
            else
            {
                // If no NavMesh position found, fall back to ground raycasting
                Debug.LogWarning("No valid NavMesh position found, falling back to raycast");
                randomPos = GetGroundPosition(randomPos, area);
            }
        }
        else if (area.minHeightFromGround > 0)
        {
            // Ensure minimum height from ground if specified
            randomPos = GetGroundPosition(randomPos, area);
        }

        return randomPos;
    }

    private Vector3 GetGroundPosition(Vector3 position, SpawnArea area)
    {
        RaycastHit hit;
        if (Physics.Raycast(new Vector3(position.x, position.y + 100f, position.z), Vector3.down, out hit, 200f, groundLayer))
        {
            return new Vector3(position.x, hit.point.y + area.minHeightFromGround, position.z);
        }
        return position;
    }

    public int GetActiveEntityCount() { return activeEntities.Count; }

    public int GetCurrentWaveIndex() { return currentWaveIndex; }

    public int GetTotalWaveCount() { return waves.Count; }

    private void OnDrawGizmos()
    {
        if (spawnAreas == null)
            return;

        foreach (var area in spawnAreas)
        {
            if (area.areaCenter != null)
            {
                Gizmos.color = area.gizmoColor;
                Gizmos.DrawWireSphere(area.areaCenter.position, area.radius);
            }
        }
    }
}