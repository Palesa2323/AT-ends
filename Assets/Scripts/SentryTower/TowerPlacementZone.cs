using UnityEngine;

public class TowerPlacementZone : MonoBehaviour
{
    [SerializeField] private Camera PlayerCamera;
    [SerializeField] private GameObject tower;
    [SerializeField] private LayerMask placemenCollideMask;
    [SerializeField] private LayerMask placementcheckMask;

    private GameObject CurrentPlacingTower;
    private RaycastHit hitInfo;

    void Update()
    {
        // Raycast from camera to mouse position to get the current position
        Ray camRay = PlayerCamera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(camRay, out hitInfo, 1000f, placemenCollideMask))
        {
            if (CurrentPlacingTower != null)
            {
                CurrentPlacingTower.transform.position = hitInfo.point;
            }
        }

        // Check for a left mouse button click
        if (Input.GetMouseButtonDown(0))
        {
            // First, check if a tower is being placed and we have a valid hit point
            if (CurrentPlacingTower != null && hitInfo.collider != null)
            {
                // INVALID CHECK 1: Is the clicked spot a restricted area?
                if (hitInfo.collider.CompareTag("Cant Place"))
                {
                    Debug.Log("Cannot place tower here. It's a restricted area.");
                    Destroy(CurrentPlacingTower); // Destroy the ghost tower
                    CurrentPlacingTower = null;
                    return; // Stop the function here
                }

                // INVALID CHECK 2: Is the placement spot blocked by another object?
                BoxCollider towerCollider = CurrentPlacingTower.GetComponentInChildren<BoxCollider>();
                Vector3 boxCenter = CurrentPlacingTower.transform.position + towerCollider.center;
                Vector3 halfExtents = towerCollider.size / 2;

                if (Physics.CheckBox(boxCenter, halfExtents, Quaternion.identity, placementcheckMask, QueryTriggerInteraction.Ignore))
                {
                    Debug.Log("Cannot place tower here. The area is blocked.");
                    Destroy(CurrentPlacingTower); // Destroy the ghost tower
                    CurrentPlacingTower = null;
                    return; // Stop the function here
                }

                // If we've reached this point, all checks have passed!
                // Finalize placement by dropping the tower
                CurrentPlacingTower = null;
                Debug.Log("Tower placed successfully!");
            }
        }
    }

    public void SetTowerToPlace()
    {
        if (CurrentPlacingTower == null)
        {
            CurrentPlacingTower = Instantiate(tower, Vector3.zero, Quaternion.identity);
        }
    }
}