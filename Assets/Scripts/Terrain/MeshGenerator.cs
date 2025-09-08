using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(MeshFilter))]
public class MeshGenerator : MonoBehaviour
{
    Mesh mesh;
    public Vector3[] vertices;
    int[] triangles;
    Color[] colors;

    public int xSize = 200; // Change this from 20 to a larger number
    public int zSize = 100; // Change this from 20 to a larger number
    public Gradient gradient;
    public MeshFilter meshFilter;
    private MeshCollider meshCollider;

    // NEW: Path Color
    public Color pathColor = Color.grey; // You can change this in the Inspector
    public float pathWidth = 2f; // Keep this public for the path flattening logic


    float minTerrainHeight;
    float maxTerrainHeight;

    // Random offsets
    float xOffset;
    float zOffset;

    // Enemy path stuff
    public List<EnemyPath> enemyPaths = new List<EnemyPath>();
    public Transform waypointParent; // optional parent for markers in hierarchy

    void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        // Get the existing MeshCollider component
        meshCollider = GetComponent<MeshCollider>();
        if (meshCollider == null)
        {
            Debug.LogError("MeshCollider not found on the GameObject!");
            return;
        }

        // Generate random offsets every new game
        xOffset = Random.Range(0f, 9999f);
        zOffset = Random.Range(0f, 9999f);

        // Randomize path color for each new game
        pathColor = Random.ColorHSV(0f, 1f, 0.5f, 1f, 0.5f, 1f);

        CreateShape();
        UpdateMesh();
        // Check terrain height
        Debug.Log("Terrain min height: " + minTerrainHeight);
        Debug.Log("Terrain max height: " + maxTerrainHeight);

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
        float pathWidth = 5f;

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

        /// Generate colors (gradient for terrain, special color for paths)
        colors = new Color[vertices.Length];
        i = 0;
        for (int z = 0; z <= zSize; z++)
        {
            for (int x = 0; x <= xSize; x++)
            {
                Vector3 currentVertex = vertices[i]; // Get the vertex we're coloring
                bool isPathVertex = false;

                // Check if this vertex is part of any generated path
                foreach (EnemyPath path in enemyPaths)
                {
                    for (int j = 0; j < path.waypoints.Count - 1; j++)
                    {
                        Vector2 start = new Vector2(path.waypoints[j].x, path.waypoints[j].z);
                        Vector2 end = new Vector2(path.waypoints[j + 1].x, path.waypoints[j + 1].z);
                        Vector2 vert2D = new Vector2(currentVertex.x, currentVertex.z);

                        if (IsPointNearLine(start, end, vert2D, pathWidth))
                        {
                            isPathVertex = true;
                            // Also flatten the path here, 
                            // currentVertex.y = 0.5f; // Or whatever flat height i want
                            // vertices[i] = currentVertex; // Update the vertex in the array
                            break; // Found a path, no need to check other segments/paths
                        }
                    }
                    if (isPathVertex) break;
                }

                if (isPathVertex)
                {
                    colors[i] = pathColor; // Apply the path color
                    // Ensure path vertices are flattened consistently
                    currentVertex.y = 0.5f; // Set to a consistent flat height
                    vertices[i] = currentVertex; // Update the vertex in the array
                }
                else
                {
                    // Apply original terrain gradient color
                    float height = Mathf.InverseLerp(minTerrainHeight, maxTerrainHeight, currentVertex.y);
                    colors[i] = gradient.Evaluate(height);
                }
                i++;
            }
        }
        // Create enemy paths
        GenerateWaypoints(pathStarts, center);
    }

    void GenerateWaypoints(Vector2[] pathStarts, Vector2 center)
    {
        enemyPaths.Clear();

        for (int i = 0; i < pathStarts.Length; i++)
        {
            EnemyPath path = new EnemyPath("Enemy Path " + (i + 1));
            int numWaypoints = 20;

            for (int j = 0; j <= numWaypoints; j++)
            {
                float t = j / (float)numWaypoints;
                Vector2 point2D = Vector2.Lerp(pathStarts[i], center, t);

                int x = Mathf.RoundToInt(point2D.x);
                int z = Mathf.RoundToInt(point2D.y);
                int index = Mathf.Clamp(z * (xSize + 1) + x, 0, vertices.Length - 1);

                float y = vertices[index].y;

                Vector3 waypoint = new Vector3(point2D.x, 0.5f + 0.5f, point2D.y);
                path.waypoints.Add(waypoint);

                // Optional visible markers - these will now be above the colored path
                if (waypointParent != null)
                {
                    GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    sphere.transform.position = waypoint;
                    sphere.transform.localScale = Vector3.one * 0.5f;
                    sphere.name = path.pathName + "_WP" + j;
                    sphere.transform.SetParent(waypointParent);

                    // This is the new line you need to add
                    // Get the MeshRenderer component and disable it
                    MeshRenderer renderer = sphere.GetComponent<MeshRenderer>();
                    if (renderer != null)
                    {
                        renderer.enabled = false;
                    }
                }
            }
            enemyPaths.Add(path);
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
    private void UpdateMesh()
    {
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.colors = colors;
        mesh.RecalculateNormals();

        // Add or update MeshCollider
        MeshCollider meshCollider = GetComponent<MeshCollider>();
        if (meshCollider == null)
            meshCollider = gameObject.AddComponent<MeshCollider>();
        meshCollider.sharedMesh = mesh;
    }


    private void OnDrawGizmos()
    {
        if (enemyPaths == null) return;

        Color[] colors = { Color.red, Color.green, Color.blue };

        for (int i = 0; i < enemyPaths.Count; i++)
        {
            Gizmos.color = colors[i % colors.Length];
            var wp = enemyPaths[i].waypoints;
            for (int j = 0; j < wp.Count - 1; j++)
            {
                Gizmos.DrawSphere(wp[j], 0.2f);
                Gizmos.DrawLine(wp[j], wp[j + 1]);
            }
        }
    }

    public float GetHeightAtPosition(float x, float z)
    {
        // Convert world position to nearest vertex in the mesh
        int ix = Mathf.Clamp(Mathf.RoundToInt(x), 0, xSize);
        int iz = Mathf.Clamp(Mathf.RoundToInt(z), 0, zSize);
        int index = iz * (xSize + 1) + ix;

        if (vertices != null && index >= 0 && index < vertices.Length)
            return vertices[index].y;

        return 0f; // fallback
    }

}
[System.Serializable]
public class EnemyPath
{
    public string pathName;
    public List<Vector3> waypoints;

    public EnemyPath(string name)
    {
        pathName = name;
        waypoints = new List<Vector3>();
    }
}