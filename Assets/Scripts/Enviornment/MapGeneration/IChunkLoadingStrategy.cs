using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Enviornment.MapGeneration
{
    public interface IChunkLoadingStrategy
    {
        HashSet<Vector2Int> GetChunksToLoad(Vector2Int playerChunk, int renderDistance);
    }
}
