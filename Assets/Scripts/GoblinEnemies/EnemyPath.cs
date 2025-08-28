using UnityEngine;

public class EnemyPath : MonoBehaviour
{
    // The public array to hold all the waypoints for the goblins to follow.
    public Transform[] waypoints;

    // This method will be called to get the path
    public Transform[] GetPath()
    {
        return waypoints;
    }
}