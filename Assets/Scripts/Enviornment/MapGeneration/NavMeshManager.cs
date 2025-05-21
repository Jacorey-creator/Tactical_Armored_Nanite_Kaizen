using System.Collections;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

public class NavMeshManager : MonoBehaviour
{
    [Header("NavMesh Configuration")]
    [SerializeField] private NavMeshSurface navMeshSurface;
    [SerializeField] private float rebuildInterval = 1.0f;
    [SerializeField] private bool continuouslyRebuild = true;
    [SerializeField] private LayerMask navMeshIncludeLayers;

    [Header("Advanced Settings")]
    [SerializeField] private bool useAsyncBuild = true;
    [SerializeField] private float rebuildRadius = 50f;
    [SerializeField] private Transform trackingTarget;

    private bool isRebuildingNavMesh = false;
    private Vector3 lastRebuildPosition;

    private void Awake()
    {
        // If no NavMeshSurface is assigned, try to find one in this GameObject
        if (navMeshSurface == null)
        {
            navMeshSurface = GetComponent<NavMeshSurface>();
            if (navMeshSurface == null)
            {
                navMeshSurface = gameObject.AddComponent<NavMeshSurface>();
                ConfigureDefaultNavMeshSurface();
            }
        }

        // If no tracking target is assigned, try to find the player
        if (trackingTarget == null)
        {
            trackingTarget = GameObject.FindGameObjectWithTag("Player")?.transform;
        }
    }

    private void Start()
    {
        // Initial NavMesh build
        RebuildNavMesh();

        // Start automatic rebuilding if enabled
        if (continuouslyRebuild)
        {
            StartCoroutine(RebuildNavMeshRoutine());
        }
    }

    private void ConfigureDefaultNavMeshSurface()
    {
        navMeshSurface.collectObjects = CollectObjects.All;
        navMeshSurface.layerMask = navMeshIncludeLayers;
        navMeshSurface.useGeometry = NavMeshCollectGeometry.PhysicsColliders;
        navMeshSurface.defaultArea = 0; // Walkable area
    }

    /// <summary>
    /// Rebuilds the NavMesh immediately
    /// </summary>
    public void RebuildNavMesh()
    {
        if (navMeshSurface == null) return;

        // Update the center position to build around if we have a tracking target
        if (trackingTarget != null)
        {
            navMeshSurface.center = trackingTarget.position;
            lastRebuildPosition = trackingTarget.position;
        }

        // Set size if using a limited radius
        if (rebuildRadius > 0)
        {
            navMeshSurface.size = new Vector3(rebuildRadius * 2, rebuildRadius * 2, rebuildRadius * 2);
        }

        navMeshSurface.BuildNavMesh();
    }

    /// <summary>
    /// Rebuilds the NavMesh asynchronously
    /// </summary>
    public IEnumerator RebuildNavMeshAsync()
    {
        if (navMeshSurface == null || isRebuildingNavMesh) yield break;

        isRebuildingNavMesh = true;

        // Update the center position to build around if we have a tracking target
        if (trackingTarget != null)
        {
            navMeshSurface.center = trackingTarget.position;
            lastRebuildPosition = trackingTarget.position;
        }

        // Set size if using a limited radius
        if (rebuildRadius > 0)
        {
            navMeshSurface.size = new Vector3(rebuildRadius * 2, rebuildRadius * 2, rebuildRadius * 2);
        }

        AsyncOperation operation = navMeshSurface.UpdateNavMesh(navMeshSurface.navMeshData);

        while (!operation.isDone)
        {
            yield return null;
        }

        isRebuildingNavMesh = false;
    }

    /// <summary>
    /// Coroutine that periodically rebuilds the NavMesh
    /// </summary>
    private IEnumerator RebuildNavMeshRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(rebuildInterval);

            // Check if player has moved enough to require a rebuild
            if (trackingTarget != null &&
                Vector3.Distance(lastRebuildPosition, trackingTarget.position) > rebuildRadius * 0.5f)
            {
                if (useAsyncBuild)
                {
                    yield return StartCoroutine(RebuildNavMeshAsync());
                }
                else
                {
                    RebuildNavMesh();
                }
            }
            else if (trackingTarget == null)
            {
                // No tracking target, so just rebuild periodically
                if (useAsyncBuild)
                {
                    yield return StartCoroutine(RebuildNavMeshAsync());
                }
                else
                {
                    RebuildNavMesh();
                }
            }
        }
    }

    /// <summary>
    /// Set a new target transform to track for NavMesh rebuilding
    /// </summary>
    public void SetTrackingTarget(Transform target)
    {
        trackingTarget = target;
    }

    /// <summary>
    /// Adjust the NavMesh rebuild radius
    /// </summary>
    public void SetRebuildRadius(float radius)
    {
        rebuildRadius = Mathf.Max(10f, radius); // Minimum 10 units radius
    }

    /// <summary>
    /// Force an immediate NavMesh rebuild centered on a specific position
    /// </summary>
    public void RebuildNavMeshAtPosition(Vector3 position)
    {
        if (navMeshSurface == null) return;

        navMeshSurface.center = position;
        lastRebuildPosition = position;

        if (useAsyncBuild)
        {
            StartCoroutine(RebuildNavMeshAsync());
        }
        else
        {
            RebuildNavMesh();
        }
    }
}