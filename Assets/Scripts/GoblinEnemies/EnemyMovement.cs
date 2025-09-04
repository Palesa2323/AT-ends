using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    public float MaxHealth;
    public float Health;
    public float Speed;
    public int ID; // Changed from float to int

    public void Init()
    {
        Health = MaxHealth;
    }
}
