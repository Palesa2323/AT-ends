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
        if (currentPlacingTower == null)
            return; // Nothing to place

        Ray ray = PlayerCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitInfo;

        // Raycast ignoring layers for safety, can add layer mask later
        bool hitTerrain = Physics.Raycast(ray, out hitInfo, Mathf.Infinity, placementCollideMask);

        if (hitTerrain)
        {
            Debug.Log("Raycast hit: " + hitInfo.collider.name + " at " + hitInfo.point);

            // Use procedural mesh height if meshGen exists, otherwise use hit point
            Vector3 placePosition = hitInfo.point;
            if (meshGen != null)
            {
                float terrainHeight = meshGen.GetHeightAtPosition(placePosition.x, placePosition.z);
                placePosition.y = terrainHeight + 0.5f;
            }
            else
            {
                placePosition.y += 0.5f;
            }

            currentPlacingTower.transform.position = placePosition;

            // Check for placement collisions
            BoxCollider towerCollider = currentPlacingTower.GetComponentInChildren<BoxCollider>();
            bool blocked = false;
            if (towerCollider != null)
            {
                Vector3 boxCenter = currentPlacingTower.transform.position + towerCollider.center;
                Vector3 halfExtents = towerCollider.size / 2f;
                blocked = Physics.CheckBox(boxCenter, halfExtents, Quaternion.identity, placementCheckMask, QueryTriggerInteraction.Ignore);

                if (towerMaterial != null)
                    towerMaterial.color = blocked ? Color.red : Color.green;
            }

            // Place tower on left-click if valid
            if (!blocked && Input.GetMouseButtonDown(0))
            {
                gameLoop.DeductCost(currentTowerCost);
                currentPlacingTower.layer = LayerMask.NameToLayer("Tower"); // assign proper layer
                currentPlacingTower = null; // stops following mouse
                towerMaterial = null;
            }
        }
        else
        {
            // Ray missed terrain: keep tower visible in front of camera for debugging
            if (currentPlacingTower != null)
            {
                currentPlacingTower.transform.position = PlayerCamera.transform.position + PlayerCamera.transform.forward * 5f;
                if (towerMaterial != null)
                    towerMaterial.color = Color.red;
            }

            Debug.LogWarning("Raycast Missed Terrain. Tower moved to debug position.");
        }

        // Cancel placement on right-click
        if (Input.GetMouseButtonDown(1))
        {
            if (currentPlacingTower != null)
                Destroy(currentPlacingTower);

            currentPlacingTower = null;
            towerMaterial = null;
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

        // 1. Clean up old preview if necessary
        if (currentPlacingTower != null)
        {
            Destroy(currentPlacingTower);
        }

        // 2. Spawn preview and immediately check for failure
        currentPlacingTower = Instantiate(prefab, Vector3.zero, Quaternion.identity);
        if (currentPlacingTower == null)
        {
            Debug.LogError("FATAL ERROR: Instantiate failed. Check Console for errors in the prefab's Awake/Start methods.");
            return;
        }

        // Set a debug name so you can track it in the Hierarchy
        currentPlacingTower.name = $"Ghost_Tower_{prefab.name}";

        currentTowerCost = cost;

        // 3. Get material for color feedback
        Renderer rend = currentPlacingTower.GetComponentInChildren<Renderer>();

        if (rend != null)
        {
            // Use sharedMaterial for reading, but continue using .material for the unique instance copy
            towerMaterial = rend.material;

            if (towerMaterial != null)
            {
                // Force a specific semi-transparent color for debugging visibility
                Color debugColor = Color.green;
                debugColor.a = 0.5f;
                towerMaterial.color = debugColor;
            }
            else
            {
                Debug.LogError("Tower spawned, but its Renderer has a null material.");
            }
        }
        else
        {
            Debug.LogWarning("Tower prefab has no Renderer, placement color is invisible.");
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
