using UnityEngine;

public class PooledEntity : MonoBehaviour
{
    [HideInInspector]
    public GameObject originalPrefab;

    [HideInInspector]
    public WaveSpawner spawner;

    public void ReturnToPool()
    {
        if (spawner != null && originalPrefab != null)
        {
            spawner.ReturnToPool(gameObject, originalPrefab);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}