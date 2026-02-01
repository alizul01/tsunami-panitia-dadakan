using UnityEngine;
using System.Collections.Generic;

public class GroundData : MonoBehaviour
{
    [Header("Main Settings")]
    public Transform nextSpawnPoint;

    [Header("Spawn Points")]
    public List<Transform> obstacleSpawnPoints;
    public List<Transform> puluSpawnPoints;

    private void OnDrawGizmos()
    {
        // Visualisasi di Editor biar gampang atur posisi
        if (nextSpawnPoint != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(nextSpawnPoint.position, 0.3f);
            Gizmos.DrawLine(transform.position, nextSpawnPoint.position);
        }

        Gizmos.color = Color.red;
        foreach (var p in obstacleSpawnPoints)
        {
            if (p != null) Gizmos.DrawWireCube(p.position, Vector3.one * 0.5f);
        }

        Gizmos.color = Color.green;
        foreach (var p in puluSpawnPoints)
        {
            if (p != null) Gizmos.DrawWireSphere(p.position, 0.3f);
        }
    }
}
