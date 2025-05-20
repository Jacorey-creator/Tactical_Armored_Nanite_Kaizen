using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


[System.Serializable]
public class WaveController
{
    public string waveName = "Wave";

    public List<PrefabCount> prefabsToSpawn = new();

    public float delayBeforeWave = 3f;
    
    public bool useSpecificSpawnPoints = false;
    
    public List<Transform> spawnPoints = new();
    
    public UnityEvent onWaveStart;
    
    public UnityEvent onWaveComplete;
}

[System.Serializable]
public class PrefabCount
{
    public GameObject prefab;
    
    public int count = 1;
}

[System.Serializable]
public class SpawnArea
{
    public Transform areaCenter;
    
    public float radius = 5f;
    
    public float minHeightFromGround = 0f;
    
    public bool useNavMesh = false;
    
    public Color gizmoColor = new Color(0f, 1f, 0f, 0.2f);
}
