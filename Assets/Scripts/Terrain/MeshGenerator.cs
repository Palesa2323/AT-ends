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

        // Generate vertices
        for (int z = 0; z <= zSize; z++)
        {
            for (int x = 0; x <= xSize; x++)
            {
                float y = Mathf.PerlinNoise((x * .3f) + xOffset, (z * .3f) + zOffset) * 2f;
                vertices[i] = new Vector3(x, y, z);
                // Track min and max terrain height
                minTerrainHeight = float.MaxValue;
                maxTerrainHeight = float.MinValue;


                i++;
            }
        }

        // Generate triangles
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
            vert++; // Skip to next row
        }

        // Generate UVs
        colors = new Color[vertices.Length];
        i = 0; // reset i to 0
        for (int z = 0; z <= zSize; z++)
        {
            for (int x = 0; x <= xSize; x++)
            {
                float height = Mathf.InverseLerp(minTerrainHeight, maxTerrainHeight, vertices[i].y);
                Debug.Log(height);
                colors[i] = gradient.Evaluate(height);

                i++;
            }
        }


    }

    void UpdateMesh()
    {
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.colors =colors;

        mesh.RecalculateNormals();
    }

    private void OnDrawGizmos()
    {
        if (vertices == null)
            return;

        for (int i = 0; i < vertices.Length; i++)
        {
            Gizmos.DrawSphere(vertices[i], 0.1f);
        }
    }
}
