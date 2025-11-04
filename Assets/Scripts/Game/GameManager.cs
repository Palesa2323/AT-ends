using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public int money;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject); // Ensure only one instance exists
        }
    }
}
