using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerPlacementZone : MonoBehaviour
{
    public Camera PlayerCamera;

    [Header("Tower Prefabs")]
    public GameObject normalTowerPrefab;
    public GameObject cryoTowerPrefab;
    public GameObject bombTowerPrefab;

    [Header("Placement Settings")]
    public LayerMask placementCollideMask;
    public LayerMask placementCheckMask;
    public MeshGenerator meshGen; // drag your terrain GameObject here

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
    }

    void Update()
    {
        if (currentPlacingTower == null) return;

        Ray ray = PlayerCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitInfo;

        if (Physics.Raycast(ray, out hitInfo, Mathf.Infinity, placementCollideMask))
        {
            Vector3 placePosition = hitInfo.point;

            // Align to terrain height if needed
            if (meshGen != null)
            {
                float terrainHeight = meshGen.GetHeightAtPosition(placePosition.x, placePosition.z);
                placePosition.y = terrainHeight;
            }

            currentPlacingTower.transform.position = placePosition;

            BoxCollider towerCollider = currentPlacingTower.GetComponentInChildren<BoxCollider>();
            Vector3 boxCenter = currentPlacingTower.transform.position + towerCollider.center;
            Vector3 halfExtents = towerCollider.size / 2;

            bool blocked = Physics.CheckBox(boxCenter, halfExtents, Quaternion.identity, placementCheckMask, QueryTriggerInteraction.Ignore);

            // Visual feedback
            if (towerMaterial != null)
            {
                towerMaterial.color = blocked ? Color.red : Color.green;
            }

            // Place tower if clear and player clicks
            if (!blocked && Input.GetMouseButtonDown(0))
            {
                gameLoop.DeductCost(currentTowerCost);
                if (towerMaterial != null) towerMaterial.color = Color.white;
                currentPlacingTower = null;
            }

            // Cancel with right-click
            if (Input.GetMouseButtonDown(1))
            {
                CancelPlacement();
            }
        }
    }

    private void StartPlacement(GameObject prefab, int cost)
    {
        if (GameLoop.Resources < cost)
        {
            Debug.Log("Not enough resources!");
            return;
        }

        if (currentPlacingTower != null)
        {
            Destroy(currentPlacingTower);
        }

        currentPlacingTower = Instantiate(prefab, Vector3.zero, Quaternion.identity);
        currentTowerCost = cost;

        Renderer rend = currentPlacingTower.GetComponent<Renderer>();
        if (rend != null)
        {
            towerMaterial = rend.material;
            Color c = towerMaterial.color;
            c.a = 0.6f;
            towerMaterial.color = c;
        }
    }

    public void PlaceNormalTower()
    {
        StartPlacement(normalTowerPrefab, normalTowerCost);
    }

    public void PlaceCryoTower()
    {
        StartPlacement(cryoTowerPrefab, cryoTowerCost);
    }

    public void PlaceBombTower()
    {
        StartPlacement(bombTowerPrefab, bombTowerCost);
    }

    private void CancelPlacement()
    {
        if (currentPlacingTower != null)
            Destroy(currentPlacingTower);
        currentPlacingTower = null;
    }
}
