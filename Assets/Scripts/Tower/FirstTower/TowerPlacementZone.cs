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
        {
            // If we’re not placing a tower, don’t do anything
            if (currentPlacingTower == null)
                return;

            Ray ray = PlayerCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hitInfo;

            // --- CRITICAL RAYCAST CHECK ---
            bool hitTerrain = Physics.Raycast(ray, out hitInfo, Mathf.Infinity, placementCollideMask);

            if (hitTerrain)
            {
                Debug.Log("Raycast Hit Terrain. Tower should be placed at mouse cursor."); // ADDED LOG

                Vector3 placePosition = hitInfo.point;

                if (meshGen != null)
                {
                    float terrainHeight = meshGen.GetHeightAtPosition(placePosition.x, placePosition.z);
                    placePosition.y = terrainHeight + 0.5f;
                }

                // --- TOWER POSITION UPDATE ---
                currentPlacingTower.transform.position = placePosition;

                
                BoxCollider towerCollider = currentPlacingTower.GetComponentInChildren<BoxCollider>();
                if (towerCollider != null)
                {
                    Vector3 boxCenter = currentPlacingTower.transform.position + towerCollider.center;
                    Vector3 halfExtents = towerCollider.size / 2f;

                    // Check for overlap using the correct layer mask
                    bool blocked = Physics.CheckBox(boxCenter, halfExtents, Quaternion.identity, placementCheckMask, QueryTriggerInteraction.Ignore);

                    if (towerMaterial != null)
                        towerMaterial.color = blocked ? Color.red : Color.green;

                    // --- PLACEMENT ---
                    if (!blocked && Input.GetMouseButtonDown(0))
                    {
                        gameLoop.DeductCost(currentTowerCost);
                        // Ensure the layer is correct upon final placement
                        currentPlacingTower.layer = LayerMask.NameToLayer("Tower");
                        currentPlacingTower = null;
                        towerMaterial = null;
                    }
                }
            }
            else 
            {
                Debug.LogWarning("Raycast Missed Terrain. Tower moved to debug position."); // ADDED LOG

                // --- DEBUG VISIBILITY FALLBACK ---
                // Move the tower 5 units in front of the camera, regardless of terrain height.
                if (currentPlacingTower != null)
                {
                    currentPlacingTower.transform.position = PlayerCamera.transform.position + PlayerCamera.transform.forward * 5f;
                    if (towerMaterial != null)
                        towerMaterial.color = Color.red;
                }
            }

            // Cancel logic: if the user right-clicks, cancel placement.
            if (Input.GetMouseButtonDown(1))
            {
                CancelPlacement();
            }

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
