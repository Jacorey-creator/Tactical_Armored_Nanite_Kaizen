using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class TerrainMeshGenerator : MonoBehaviour
{
    [Header("Terrain Configuration")]
    [SerializeField] private int xSize = 20;
    [SerializeField] private int zSize = 20;
    [SerializeField] private float heightScale = 1.5f;
    [SerializeField] private float noiseScale = 0.3f;
    [SerializeField] private Vector2 noiseOffset = Vector2.zero;

    [Header("Debug Settings")]
    [SerializeField] private bool showVerticesGizmos = true;
    [SerializeField] private float gizmoSize = 0.1f;
    [SerializeField] private Color gizmoColor = Color.yellow;

    private Mesh mesh;
    private MeshFilter meshFilter;

    #region Unity Lifecycle Methods

    private void Awake()
    {
        meshFilter = GetComponent<MeshFilter>();

        // Add a MeshRenderer if one doesn't exist
        if (GetComponent<MeshRenderer>() == null)
        {
            MeshRenderer renderer = gameObject.AddComponent<MeshRenderer>();
            renderer.material = Resources.Load<Material>("DefaultTerrainMaterial");

            // If the default material couldn't be loaded, create a basic material
            if (renderer.material == null)
            {
                renderer.material = new Material(Shader.Find("Standard"));
                renderer.material.color = new Color(0.3f, 0.5f, 0.2f); // Green-brown terrain color
            }
        }
    }

    private void Start()
    {
        GenerateTerrain();
    }

    #endregion

    #region Mesh Generation

    /// <summary>
    /// Generates a terrain mesh with the specified dimensions
    /// </summary>
    public void GenerateTerrain()
    {
        mesh = new Mesh();
        mesh.name = "Procedural Terrain";
        meshFilter.mesh = mesh;

        Vector3[] vertices = GenerateVertices();
        int[] triangles = GenerateTriangles();
        Vector2[] uvs = GenerateUVs(vertices);

        UpdateMesh(vertices, triangles, uvs);
    }

    /// <summary>
    /// Creates vertex positions with Perlin noise for height
    /// </summary>
    private Vector3[] GenerateVertices()
    {
        Vector3[] vertices = new Vector3[(xSize + 1) * (zSize + 1)];

        for (int z = 0, index = 0; z <= zSize; z++)
        {
            for (int x = 0; x <= xSize; x++, index++)
            {
                float y = CalculateHeight(x, z);
                vertices[index] = new Vector3(x, y, z);
            }
        }

        return vertices;
    }

    /// <summary>
    /// Calculates the height at a given x, z coordinate using Perlin noise
    /// </summary>
    private float CalculateHeight(int x, int z)
    {
        float xCoord = (x + noiseOffset.x) * noiseScale;
        float zCoord = (z + noiseOffset.y) * noiseScale;
        return Mathf.PerlinNoise(xCoord, zCoord) * heightScale;
    }

    /// <summary>
    /// Creates triangles to form the terrain mesh
    /// </summary>
    private int[] GenerateTriangles()
    {
        int[] triangles = new int[xSize * zSize * 6];
        int vertexIndex = 0;
        int triangleIndex = 0;

        for (int z = 0; z < zSize; z++)
        {
            for (int x = 0; x < xSize; x++)
            {
                // First triangle
                triangles[triangleIndex++] = vertexIndex;
                triangles[triangleIndex++] = vertexIndex + xSize + 1;
                triangles[triangleIndex++] = vertexIndex + 1;

                // Second triangle
                triangles[triangleIndex++] = vertexIndex + 1;
                triangles[triangleIndex++] = vertexIndex + xSize + 1;
                triangles[triangleIndex++] = vertexIndex + xSize + 2;

                vertexIndex++;
            }

            vertexIndex++;
        }

        return triangles;
    }

    /// <summary>
    /// Creates UV coordinates for texture mapping
    /// </summary>
    private Vector2[] GenerateUVs(Vector3[] vertices)
    {
        Vector2[] uvs = new Vector2[vertices.Length];

        for (int z = 0, index = 0; z <= zSize; z++)
        {
            for (int x = 0; x <= xSize; x++, index++)
            {
                uvs[index] = new Vector2((float)x / xSize, (float)z / zSize);
            }
        }

        return uvs;
    }

    /// <summary>
    /// Updates the mesh with the generated data
    /// </summary>
    private void UpdateMesh(Vector3[] vertices, int[] triangles, Vector2[] uvs)
    {
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;
        mesh.RecalculateNormals();
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Allows external modification of the terrain parameters and regeneration
    /// </summary>
    public void UpdateTerrainParameters(int newXSize, int newZSize, float newHeightScale, float newNoiseScale)
    {
        xSize = newXSize;
        zSize = newZSize;
        heightScale = newHeightScale;
        noiseScale = newNoiseScale;

        GenerateTerrain();
    }

    /// <summary>
    /// Set the noise offset for chunk-based world generation
    /// </summary>
    public void SetNoiseOffset(float xOffset, float zOffset)
    {
        // This allows different chunks to have continuous terrain
        noiseOffset = new Vector2(xOffset, zOffset);
        GenerateTerrain();
    }

    /// <summary>
    /// Gets the current terrain mesh
    /// </summary>
    public Mesh GetMesh()
    {
        return mesh;
    }

    #endregion

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (!showVerticesGizmos || mesh == null || mesh.vertices == null)
            return;

        Gizmos.color = gizmoColor;
        Vector3[] vertices = mesh.vertices;

        for (int i = 0; i < vertices.Length; i++)
        {
            // Transform the position to world space
            Vector3 worldPosition = transform.TransformPoint(vertices[i]);
            Gizmos.DrawSphere(worldPosition, gizmoSize);
        }
    }
#endif
}