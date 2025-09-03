using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(MeshFilter))]
public class MeshGenerator : MonoBehaviour
{
    Mesh mesh;
    Vector3[] vertices;
    int[] triangles;
    Color[] colors;

    public int xSize = 20;
    public int zSize = 20;
    public Gradient gradient;

    float minTerrainHeight;
    float maxTerrainHeight;

    // Random offsets
    float xOffset;
    float zOffset;

    void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        // Generate random offsets every new game
        xOffset = Random.Range(0f, 9999f);
        zOffset = Random.Range(0f, 9999f);

        CreateShape();
        UpdateMesh();
    }

    void CreateShape()
    {
        vertices = new Vector3[(xSize + 1) * (zSize + 1)];
        int i = 0;

        // Initialize min/max heights
        minTerrainHeight = float.MaxValue;
        maxTerrainHeight = float.MinValue;

        // Define paths: three start points to center
        Vector2 center = new Vector2(xSize / 2f, zSize / 2f);
        Vector2[] pathStarts = new Vector2[3]
        {
        new Vector2(0, 0),          // top-left
        new Vector2(xSize, 0),      // top-right
        new Vector2(xSize/2f, zSize) // bottom-center
        };
        float pathWidth = 2f;

        // Generate vertices
        for (int z = 0; z <= zSize; z++)
        {
            for (int x = 0; x <= xSize; x++)
            {
                float y = Mathf.PerlinNoise((x * .3f) + xOffset, (z * .3f) + zOffset) * 2f;
                Vector3 vertex = new Vector3(x, y, z);

                // Flatten vertex if it’s near any path
                Vector2 vert2D = new Vector2(x, z);
                foreach (Vector2 start in pathStarts)
                {
                    if (IsPointNearLine(start, center, vert2D, pathWidth))
                    {
                        vertex.y = 0.5f; // flatten for path
                    }
                }

                vertices[i] = vertex;

                // Track min and max terrain height
                if (vertex.y > maxTerrainHeight) maxTerrainHeight = vertex.y;
                if (vertex.y < minTerrainHeight) minTerrainHeight = vertex.y;

                i++;
            }
        }

        // Generate triangles (same as before)
        triangles = new int[xSize * zSize * 6];
        int vert = 0;
        int tria = 0;
        for (int z = 0; z < zSize; z++)
        {
            for (int x = 0; x < xSize; x++)
            {
                triangles[tria + 0] = vert + 0;
                triangles[tria + 1] = vert + xSize + 1;
                triangles[tria + 2] = vert + 1;
                triangles[tria + 3] = vert + 1;
                triangles[tria + 4] = vert + xSize + 1;
                triangles[tria + 5] = vert + xSize + 2;

                vert++;
                tria += 6;
            }
            vert++;
        }

        // Generate colors (gradient)
        colors = new Color[vertices.Length];
        i = 0;
        for (int z = 0; z <= zSize; z++)
        {
            for (int x = 0; x <= xSize; x++)
            {
                float height = Mathf.InverseLerp(minTerrainHeight, maxTerrainHeight, vertices[i].y);
                colors[i] = gradient.Evaluate(height);
                i++;
            }
        }
    }

    // Helper function for path proximity
    bool IsPointNearLine(Vector2 start, Vector2 end, Vector2 point, float width)
    {
        float dist = Vector2.Distance(PointOnLineClosest(start, end, point), point);
        return dist < width;
    }

    Vector2 PointOnLineClosest(Vector2 a, Vector2 b, Vector2 p)
    {
        Vector2 ap = p - a;
        Vector2 ab = b - a;
        float t = Vector2.Dot(ap, ab) / ab.sqrMagnitude;
        t = Mathf.Clamp01(t);
        return a + ab * t;
    }
    void UpdateMesh() { mesh.Clear(); mesh.vertices = vertices; mesh.triangles = triangles; mesh.colors = colors; mesh.RecalculateNormals(); }
    private void OnDrawGizmos() { if (vertices == null) return; for (int i = 0; i < vertices.Length; i++) { Gizmos.DrawSphere(vertices[i], 0.1f); } }
}
