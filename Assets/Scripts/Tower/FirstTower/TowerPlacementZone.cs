using UnityEngine;

public class TowerPlacementZone : MonoBehaviour
{
    public Camera PlayerCamera;

    [Header("Tower Prefabs")]
    public GameObject normalTowerPrefab;
    public GameObject cryoTowerPrefab;
    public GameObject bombTowerPrefab;

    [Header("Placement Settings")]
    public LayerMask placementCollideMask; // Terrain layer
    public LayerMask placementCheckMask;   // Tower layer
    public MeshGenerator meshGen;          // Terrain height helper

    [Header("Costs")]
    public int normalTowerCost = 50;
    public int cryoTowerCost = 70;
    public int bombTowerCost = 100;

    private GameObject currentPlacingTower;
    private int currentTowerCost;
    private GameLoop gameLoop;
    private Material towerMaterial;

    void Start()
    {
        gameLoop = FindFirstObjectByType<GameLoop>();
        if (gameLoop == null)
            Debug.LogError("GameLoop not found in scene!");

        if (PlayerCamera == null)
            Debug.LogError("PlayerCamera not assigned in inspector!");
    }

    void Update()
    {
        // If we’re not placing a tower, don’t do anything
        if (currentPlacingTower == null)
            return;

        Ray ray = PlayerCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitInfo;

        // Only continue if we hit something valid (terrain)
        if (Physics.Raycast(ray, out hitInfo, Mathf.Infinity, placementCollideMask))
        {
            Vector3 placePosition = hitInfo.point;

            // Adjust height to sit nicely above terrain
            if (meshGen != null)
            {
                float terrainHeight = meshGen.GetHeightAtPosition(placePosition.x, placePosition.z);
                placePosition.y = terrainHeight + 0.5f;
            }

            currentPlacingTower.transform.position = placePosition;

            // Find tower collider for placement validation
            BoxCollider towerCollider = currentPlacingTower.GetComponentInChildren<BoxCollider>();
            if (towerCollider == null)
            {
                Debug.LogWarning("Tower prefab missing BoxCollider! Please add one.");
                return;
            }

            Vector3 boxCenter = currentPlacingTower.transform.position + towerCollider.center;
            Vector3 halfExtents = towerCollider.size / 2;

            // Check if placement area overlaps another tower
            bool blocked = Physics.CheckBox(
                boxCenter,
                halfExtents,
                Quaternion.identity,
                placementCheckMask,
                QueryTriggerInteraction.Ignore
            );

            // Show feedback color
            if (towerMaterial != null)
                towerMaterial.color = blocked ? Color.red : Color.green;

            // --- LEFT CLICK: Place tower if valid ---
            if (!blocked && Input.GetMouseButtonDown(0))
            {
                gameLoop.DeductCost(currentTowerCost);
                if (towerMaterial != null)
                    towerMaterial.color = Color.white;

                // Finalize placement
                currentPlacingTower.layer = LayerMask.NameToLayer("Tower");
                Debug.Log($"Placed tower: {currentPlacingTower.name} at {currentPlacingTower.transform.position}");

                currentPlacingTower = null; // stop following mouse
                towerMaterial = null;
                return;
            }

            // --- RIGHT CLICK: Cancel placement ---
            if (Input.GetMouseButtonDown(1))
            {
                Debug.Log("Cancelled tower placement.");
                CancelPlacement();
            }
        }
        else
        {
            // If ray misses terrain, make preview red
            if (towerMaterial != null)
                towerMaterial.color = Color.red;
        }
    }

    // Begins placement for selected tower type
    private void StartPlacement(GameObject prefab, int cost)
    {
        Debug.Log($"Attempting to place: {prefab?.name}");

        if (GameLoop.Resources < cost)
        {
            Debug.LogWarning("Not enough resources to place this tower!");
            return;
        }

        if (prefab == null)
        {
            Debug.LogError("Tower prefab is missing! Check inspector references.");
            return;
        }

        // If we already had a preview, destroy it first
        if (currentPlacingTower != null)
        {
            Destroy(currentPlacingTower);
        }

        // Spawn preview
        currentPlacingTower = Instantiate(prefab, Vector3.zero, Quaternion.identity);
        currentTowerCost = cost;

        // Get material for color feedback
        Renderer rend = currentPlacingTower.GetComponentInChildren<Renderer>();
        if (rend != null)
        {
            towerMaterial = rend.material;
            towerMaterial.color = Color.green;
        }
        else
        {
            Debug.LogWarning("Tower prefab has no Renderer, cannot show placement color.");
        }
    }

    public void PlaceNormalTower() => StartPlacement(normalTowerPrefab, normalTowerCost);
    public void PlaceCryoTower() => StartPlacement(cryoTowerPrefab, cryoTowerCost);
    public void PlaceBombTower() => StartPlacement(bombTowerPrefab, bombTowerCost);

    private void CancelPlacement()
    {
        if (currentPlacingTower != null)
            Destroy(currentPlacingTower);
        currentPlacingTower = null;
        towerMaterial = null;
    }
}
