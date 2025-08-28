using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer), typeof(MeshCollider))]
public class TerrainGenerator : MonoBehaviour
{
    public int width = 100;
    public int height = 100;
    public float scale = 20f;
    public float heightMultiplier = 5f;

    private Mesh mesh;
    private MeshCollider meshCollider;

    void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        meshCollider = GetComponent<MeshCollider>();

        GenerateTerrainMesh();
        ApplyMeshToCollider();
    }

    void GenerateTerrainMesh()
    {
        Vector3[] vertices = new Vector3[width * height];
        int[] triangles = new int[(width - 1) * (height - 1) * 6];
        Vector2[] uv = new Vector2[width * height];

        for (int z = 0; z < height; z++)
        {
            for (int x = 0; x < width; x++)
            {
                int i = z * width + x;
                float perlinValue = Mathf.PerlinNoise(x / scale, z / scale);
                vertices[i] = new Vector3(x, perlinValue * heightMultiplier, z);
                uv[i] = new Vector2((float)x / width, (float)z / height);

                if (x < width - 1 && z < height - 1)
                {
                    int t = (z * (width - 1) + x) * 6;
                    triangles[t + 0] = i;
                    triangles[t + 1] = i + width;
                    triangles[t + 2] = i + 1;
                    triangles[t + 3] = i + 1;
                    triangles[t + 4] = i + width;
                    triangles[t + 5] = i + width + 1;
                }
            }
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uv;
        mesh.RecalculateNormals();
    }

    void ApplyMeshToCollider()
    {
        meshCollider.sharedMesh = mesh;
    }
}