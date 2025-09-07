using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerPlacementZone : MonoBehaviour
{
    [SerializeField] private Camera PlayerCamera;
    [SerializeField] private GameObject tower;
    [SerializeField] private LayerMask placemenCollideMask;
    [SerializeField] private LayerMask placementcheckMask;
    private int TowerCost = 50;

    private GameObject CurrentPlacingTower;
    private RaycastHit hitInfo;

    public void SetTowerToPlace()
    {
        if (CurrentPlacingTower == null)
        {
            CurrentPlacingTower = Instantiate(tower, Vector3.zero, Quaternion.identity);
        }
    }

    void Update()
    {
        Ray camRay = PlayerCamera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(camRay, out hitInfo, 1000f, placemenCollideMask))
        {
            if (CurrentPlacingTower != null)
            {
                CurrentPlacingTower.transform.position = hitInfo.point;
            }
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            Destroy(CurrentPlacingTower);
            CurrentPlacingTower = null;
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            if (CurrentPlacingTower != null && hitInfo.collider != null)
            {
                if (hitInfo.collider.CompareTag("Cant Place"))
                {
                    Debug.Log("Cannot place tower here. It's a restricted area.");
                    Destroy(CurrentPlacingTower);
                    CurrentPlacingTower = null;
                    return;
                }

                BoxCollider towerCollider = CurrentPlacingTower.GetComponentInChildren<BoxCollider>();
                Vector3 boxCenter = CurrentPlacingTower.transform.position + towerCollider.center;
                Vector3 halfExtents = towerCollider.size / 2;

                if (Physics.CheckBox(boxCenter, halfExtents, Quaternion.identity, placementcheckMask, QueryTriggerInteraction.Ignore))
                {
                    Debug.Log("Cannot place tower here. The area is blocked.");
                    Destroy(CurrentPlacingTower); // Destroy the tower if a collision is found
                    CurrentPlacingTower = null;
                    return;
                }

                // If all checks pass, finalize placement and add to the list
                GameLoop.TowersInGame.Add(CurrentPlacingTower.GetComponent<TowerBehaviour>());
                CurrentPlacingTower = null;
                Debug.Log("Tower placed successfully!");
            }
        }
    }
}