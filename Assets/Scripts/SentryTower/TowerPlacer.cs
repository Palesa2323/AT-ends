using UnityEngine;

public class TowerPlacer : MonoBehaviour
{
    public GameObject towerPrefab;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            PlaceTower();
        }
    }

    void PlaceTower()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        // The raycast will now only look for colliders on a specific layer if you use one
        // or just detect any collider.
        if (Physics.Raycast(ray, out hit))
        {
            // Check if the clicked object is a TowerPlacementZone
            TowerPlacementZone placementZone = hit.collider.GetComponent<TowerPlacementZone>();

            if (placementZone != null && !placementZone.isOccupied)
            {
                // Place the tower at the location of the TowerSpot
                Instantiate(towerPrefab, hit.collider.transform.position, Quaternion.identity);
                placementZone.isOccupied = true;
            }
        }
    }
}
