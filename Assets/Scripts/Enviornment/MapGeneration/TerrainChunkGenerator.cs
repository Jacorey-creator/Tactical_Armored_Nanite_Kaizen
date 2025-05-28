using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Enviornment.MapGeneration
{
    public class TerrainChunkGenerator : ITerrainGenerator
    {
        public GameObject GenerateChunk(Vector2Int chunkPosition, WorldGenerationConfig config, Transform parent)
        {
            // Create chunk GameObject
            GameObject chunkObject = new GameObject($"Chunk_{chunkPosition.x}_{chunkPosition.y}");
            chunkObject.transform.position = new Vector3(
                chunkPosition.x * config.chunkSize,
                0,
                chunkPosition.y * config.chunkSize
            );
            chunkObject.transform.parent = parent;

            // Add terrain mesh generator
            TerrainMeshGenerator terrainGenerator = chunkObject.AddComponent<TerrainMeshGenerator>();
            MeshCollider meshCollider = chunkObject.AddComponent<MeshCollider>();

            // Configure terrain generator
            float xOffset = chunkPosition.x * config.chunkSize;
            float zOffset = chunkPosition.y * config.chunkSize;
            terrainGenerator.SetNoiseOffset(xOffset, zOffset);
            terrainGenerator.UpdateTerrainParameters(
                config.chunkSize,
                config.chunkSize,
                config.heightScale,
                config.noiseScale
            );

            // Apply mesh to collider
            meshCollider.sharedMesh = terrainGenerator.GetMesh();

            return chunkObject;
        }
    }

}
