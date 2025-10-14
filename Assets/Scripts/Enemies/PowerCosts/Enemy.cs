using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private float speed;
    private WaveSpawner waveSpawner;

    private float countdown = 2f;
    void Update()
    {
        transform.Translate(transform.forward * speed * Time.deltaTime);
        countdown -= Time.deltaTime;
        if (countdown <= 0)
        {
            Destroy(gameObject);
        }
    }
}
