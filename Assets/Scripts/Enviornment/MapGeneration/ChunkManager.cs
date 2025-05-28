using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Enviornment.MapGeneration
{
    public class ChunkManager : IChunkManager
    {
        private Dictionary<Vector2Int, GameObject> activeChunks = new Dictionary<Vector2Int, GameObject>();

        public void AddChunk(Vector2Int position, GameObject chunk)
        {
            if (!activeChunks.ContainsKey(position))
            {
                activeChunks.Add(position, chunk);
            }
        }

        public void RemoveChunk(Vector2Int position)
        {
            if (activeChunks.TryGetValue(position, out GameObject chunk))
            {
                if (chunk != null)
                {
                    UnityEngine.Object.Destroy(chunk);
                }
                activeChunks.Remove(position);
            }
        }

        public bool HasChunk(Vector2Int position)
        {
            return activeChunks.ContainsKey(position);
        }

        public GameObject GetChunk(Vector2Int position)
        {
            activeChunks.TryGetValue(position, out GameObject chunk);
            return chunk;
        }

        public IEnumerable<Vector2Int> GetActiveChunkPositions()
        {
            return activeChunks.Keys;
        }

        public void Clear()
        {
            foreach (var chunk in activeChunks.Values)
            {
                if (chunk != null)
                {
                    UnityEngine.Object.Destroy(chunk);
                }
            }
            activeChunks.Clear();
        }
    }
}
