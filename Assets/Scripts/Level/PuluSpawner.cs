using UnityEngine;
using System.Collections.Generic;

public class PuluSpawner : MonoBehaviour
{
    [Header("Configuration")]
    public GameObject puluPrefab;
    [Range(0f, 100f)] public float spawnChance = 30f;

    public void SpawnPulus(List<Transform> spawnPoints, Transform groundParent)
    {
        if (puluPrefab == null) return;

        foreach (Transform point in spawnPoints)
        {
            if (point == null) continue;

            // Random chance check
            if (Random.Range(0f, 100f) <= spawnChance)
            {
                // Spawn as independent object (no parent)
                Instantiate(puluPrefab, point.position, Quaternion.identity);
            }
        }
    }
}
