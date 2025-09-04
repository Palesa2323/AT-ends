using UnityEngine;
using System.Collections; // Needed for Coroutines

public class PlayerMovement : MonoBehaviour
{
    private Vector3 Velocity;
    private Vector3 PlayerMovementInput;
    private Vector3 PlayerMouseInput;
    private float xRotation;

    [SerializeField] private Transform PlayerCamera;
    [SerializeField] private CharacterController Controller;
    [Space]
    [SerializeField] private float Speed = 5f;
    [SerializeField] private float Sensitivity = 2f;
    [SerializeField] private float spawnOffsetY = 1f; // height above terrain

    private MeshGenerator terrainMesh;

    void Start()
    {
        // Automatically find your procedural terrain
        terrainMesh = Object.FindFirstObjectByType<MeshGenerator>();
        if (terrainMesh == null)
        {
            Debug.LogError("No MeshGenerator found in the scene!");
        }
        else
        {
            StartCoroutine(PlacePlayerOnTerrainCo());
        }
    }

    void Update()
    {
        PlayerMovementInput = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxisRaw("Vertical"));
        PlayerMouseInput = new Vector3(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));

        MovePlayer();
        MovePlayerCamera();
    }

    // === SPAWN PLAYER ON TOP OF TERRAIN ===
    IEnumerator PlacePlayerOnTerrainCo()
    {
        // Wait until the terrain's mesh is created and assigned
        // We check if the vertices array has been populated
        while (terrainMesh.vertices == null || terrainMesh.vertices.Length == 0)
        {
            yield return null; // Wait for one frame
        }

        // Now that the terrain is ready, we can safely place the player
        Vector3 startPos = new Vector3(terrainMesh.xSize / 2f, 50f, terrainMesh.zSize / 2f);
        RaycastHit hit;

        if (Physics.Raycast(startPos, Vector3.down, out hit, 100f))
        {
            transform.position = hit.point + Vector3.up * spawnOffsetY;
        }
        else
        {
            Debug.LogError("No terrain detected below spawn position!");
        }
    }

    // === PLAYER MOVEMENT ===
    private void MovePlayer()
    {
        Vector3 MoveVector = transform.TransformDirection(PlayerMovementInput);

        if (Input.GetKey(KeyCode.Space))
            Velocity.y = 1f;
        else if (Input.GetKey(KeyCode.LeftShift))
            Velocity.y = -1f;

        Controller.Move(MoveVector * Speed * Time.deltaTime);
        Controller.Move(Velocity * Speed * Time.deltaTime);

        Velocity.y = 0f; // reset vertical velocity each frame
    }

    // === CAMERA ROTATION ===
    private void MovePlayerCamera()
    {
        xRotation -= PlayerMouseInput.y * Sensitivity;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f); // prevent flipping
        transform.Rotate(0f, PlayerMouseInput.x * Sensitivity, 0f);
        PlayerCamera.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    }
}