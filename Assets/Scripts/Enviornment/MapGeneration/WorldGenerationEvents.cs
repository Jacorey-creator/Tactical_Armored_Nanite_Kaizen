using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Enviornment.MapGeneration
{
    public class WorldGenerationEvents
    {
        public event Action<Vector2Int, GameObject> OnChunkCreated;
        public event Action<Vector2Int> OnChunkDestroyed;
        public event Action OnNavMeshUpdated;

        public void NotifyChunkCreated(Vector2Int position, GameObject chunk)
        {
            OnChunkCreated?.Invoke(position, chunk);
        }

        public void NotifyChunkDestroyed(Vector2Int position)
        {
            OnChunkDestroyed?.Invoke(position);
        }

        public void NotifyNavMeshUpdated()
        {
            OnNavMeshUpdated?.Invoke();
        }
    }
}
