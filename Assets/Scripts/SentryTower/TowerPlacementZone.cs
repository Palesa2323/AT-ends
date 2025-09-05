using UnityEngine;

public class TowerPlacementZone : MonoBehaviour
{
    [SerializeField] private Camera PlayerCamera;
    [SerializeField] private GameObject tower; // <-- this was missing!

    private GameObject CurrentPlacingTower;

    void Update()
    {
        if (CurrentPlacingTower != null)
        {
            Ray camray = PlayerCamera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(camray, out RaycastHit hitInfo, 100))
            {
                CurrentPlacingTower.transform.position = hitInfo.point;
            }
        }

        if (Input.GetMouseButtonDown(0) && CurrentPlacingTower != null)
        {
            // Drop the tower in place
            CurrentPlacingTower = null;
        }
    }

    public void SetTowerToPlace()
    {
        CurrentPlacingTower = Instantiate(tower, Vector3.zero, Quaternion.identity);
    }
}
