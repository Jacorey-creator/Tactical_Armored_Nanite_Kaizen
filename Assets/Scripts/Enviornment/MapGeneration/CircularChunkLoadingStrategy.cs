using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Enviornment.MapGeneration
{
    public class CircularChunkLoadingStrategy : IChunkLoadingStrategy
    {
        public HashSet<Vector2Int> GetChunksToLoad(Vector2Int playerChunk, int renderDistance)
        {
            HashSet<Vector2Int> chunksToLoad = new HashSet<Vector2Int>();

            for (int x = -renderDistance; x <= renderDistance; x++)
            {
                for (int z = -renderDistance; z <= renderDistance; z++)
                {
                    Vector2Int chunkPos = new Vector2Int(playerChunk.x + x, playerChunk.y + z);

                    if (Vector2Int.Distance(playerChunk, chunkPos) <= renderDistance)
                    {
                        chunksToLoad.Add(chunkPos);
                    }
                }
            }

            return chunksToLoad;
        }
    }
}
