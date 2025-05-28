using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Enviornment.MapGeneration
{
    [System.Serializable]
    public class WorldGenerationConfig
    {
        [Header("World Generation")]
        public int renderDistance = 3;
        public int chunkSize = 20;
        public float heightScale = 1.5f;
        public float noiseScale = 0.3f;
        public float worldSeed = 0f;

        [Header("NavMesh Settings")]
        public float navMeshUpdateInterval = 1.0f;

        [Header("Player Settings")]
        public Vector3 initialPlayerPosition = Vector3.zero;
    }

    // Abstraction for player
    public interface IPlayer
    {
        Vector3 Position { get; }
        bool IsValid { get; }
        event Action<Vector3> OnPositionChanged;
    }
    // Abstraction for chunk management
    public interface IChunkManager
    {
        void AddChunk(Vector2Int position, GameObject chunk);
        void RemoveChunk(Vector2Int position);
        bool HasChunk(Vector2Int position);
        GameObject GetChunk(Vector2Int position);
        IEnumerable<Vector2Int> GetActiveChunkPositions();
        void Clear();
    }

    // Abstraction for terrain generation
    public interface ITerrainGenerator
    {
        GameObject GenerateChunk(Vector2Int chunkPosition, WorldGenerationConfig config, Transform parent);
    }
}
