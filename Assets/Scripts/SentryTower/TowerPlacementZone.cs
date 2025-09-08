using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerPlacementZone : MonoBehaviour
{
    public Camera PlayerCamera;
    public GameObject tower;
    public LayerMask placemenCollideMask;
    public LayerMask placementcheckMask;
    public int TowerCost = 50;

    private GameObject CurrentPlacingTower;
    private GameLoop gameLoop;
    public MeshGenerator meshGen; // drag your terrain GameObject here in the inspector


    void Start()
    {
        gameLoop = FindFirstObjectByType<GameLoop>();
    }

    public void SetTowerToPlace()
    {
        if (GameLoop.Resources >= TowerCost)
        {
            if (CurrentPlacingTower == null)
            {
                CurrentPlacingTower = Instantiate(tower, Vector3.zero, Quaternion.identity);
            }
        }
        else
        {
            Debug.Log("Not enough resources to place a tower!");
        }
    }

    void Update()
    {
        if (CurrentPlacingTower != null)
        {
            Ray ray = PlayerCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hitInfo;

            if (Physics.Raycast(ray, out hitInfo, Mathf.Infinity, placemenCollideMask))
            {
                Vector3 placePosition = hitInfo.point;

                // Get the correct Y (height) from the terrain mesh
                if (meshGen != null)
                {
                    float terrainHeight = meshGen.GetHeightAtPosition(placePosition.x, placePosition.z);
                    placePosition.y = terrainHeight;
                }

                CurrentPlacingTower.transform.position = placePosition;

                BoxCollider towerCollider = CurrentPlacingTower.GetComponentInChildren<BoxCollider>();
                Vector3 boxCenter = CurrentPlacingTower.transform.position + towerCollider.center;
                Vector3 halfExtents = towerCollider.size / 2;

                if (Physics.CheckBox(boxCenter, halfExtents, Quaternion.identity, placementcheckMask, QueryTriggerInteraction.Ignore))
                {
                    CurrentPlacingTower.GetComponent<Renderer>().material.color = Color.red;
                }
                else
                {
                    CurrentPlacingTower.GetComponent<Renderer>().material.color = Color.green;

                    if (Input.GetMouseButtonDown(0))
                    {
                        gameLoop.DeductCost(TowerCost);
                        CurrentPlacingTower.GetComponent<Renderer>().material.color = Color.white;
                        CurrentPlacingTower = null;
                    }
                }
            }
        }
    }


}