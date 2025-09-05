using UnityEngine;

public class TowerPlacementZone : MonoBehaviour
{
    [SerializeField] private Camera PlayerCamera;
    [SerializeField] private GameObject tower;
    [SerializeField] private LayerMask placemenCollideMask;
    [SerializeField] private LayerMask placementcheckMask;

    private GameObject CurrentPlacingTower;
    private RaycastHit hitInfo; // Moved hitInfo to the class scope

    void Update()
    {
        // Raycast logic must be in Update to get current mouse position every frame.
        Ray camRay = PlayerCamera.ScreenPointToRay(Input.mousePosition);

        // This check must happen every frame to update the hitInfo variable
        if (Physics.Raycast(camRay, out hitInfo, 100f, placemenCollideMask))
        {
            // Only move the tower if one is being placed
            if (CurrentPlacingTower != null)
            {
                CurrentPlacingTower.transform.position = hitInfo.point;
            }
        }

        // The input check should be separate from the raycast logic.
        if (Input.GetMouseButtonDown(0))
        {
            // Check if we hit something and if it's the right place
            if (hitInfo.collider != null)
            {
                // Check if the area is a valid placement zone
                if (hitInfo.collider.CompareTag("Cant Place"))
                {
                    Debug.Log("Cannot place tower here.");
                    return; // Stop the function here
                }

                // If a tower is being placed and the area is valid, drop it.
                if (CurrentPlacingTower != null)
                {
                    CurrentPlacingTower = null;
                }
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