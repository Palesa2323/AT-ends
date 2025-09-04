using UnityEngine;

public class TowerSpotGenerator : MonoBehaviour
{
    public GameObject towerSpotPrefab; // Assign the TowerSpot prefab here
    public MeshGenerator terrainMesh;

    private void Start()
    {
        // Find the generated terrain in the scene
        terrainMesh = Object.FindAnyObjectByType<MeshGenerator>();

        if (terrainMesh != null)
        {
            // Wait a moment to ensure the terrain mesh and waypoints are fully generated
            Invoke("GenerateSpots", 0.1f);
        }
        else
        {
            Debug.LogError("No MeshGenerator found in the scene.");
        }
    }

    void GenerateSpots()
    {
        // Check if enemyPaths list has been populated by the MeshGenerator
        if (terrainMesh.enemyPaths == null || terrainMesh.enemyPaths.Count == 0)
        {
            Debug.LogError("Enemy paths not generated!");
            return;
        }

        // Loop through each enemy path
        foreach (var path in terrainMesh.enemyPaths)
        {
            // Place a spot at a set interval along the path
            for (int i = 0; i < path.waypoints.Count; i += 3) // You can change this interval (e.g., every 3 waypoints)
            {
                Vector3 spotPosition = path.waypoints[i];
                spotPosition.y += 0.5f; // Lift the spot slightly above the path

                GameObject newSpot = Instantiate(towerSpotPrefab, spotPosition, Quaternion.identity, transform);
                newSpot.name = "Tower Spot " + i;
            }
        }
    }
}
