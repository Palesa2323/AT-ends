using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(MeshFilter))]
public class MeshGenerator : MonoBehaviour
{

    Mesh mesh;
    Vector3[] vertices;
    int[] triangles;

    public int xSize = 20;
    public int zSize = 20;
    void Start()
    {
       mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

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
                float y = Mathf.PerlinNoise(x*  .3f, z * .3f) * 2f; //  Perlin noise
                vertices[i] = new Vector3(x, y, z);
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
    }

    void UpdateMesh()
    {
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;

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
