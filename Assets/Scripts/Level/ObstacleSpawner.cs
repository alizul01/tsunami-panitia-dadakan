using UnityEngine;
using System.Collections.Generic;

public class ObstacleSpawner : MonoBehaviour
{
    [Header("Configuration")]
    public List<GameObject> obstaclePrefabs;
    [Range(0f, 100f)] public float spawnChance = 50f;

    public void SpawnObstacles(List<Transform> spawnPoints, Transform groundParent)
    {
        if (obstaclePrefabs == null || obstaclePrefabs.Count == 0) return;

        foreach (Transform point in spawnPoints)
        {
            if (point == null) continue;

            // Random chance check
            if (Random.Range(0f, 100f) <= spawnChance)
            {
                GameObject randomPrefab = obstaclePrefabs[Random.Range(0, obstaclePrefabs.Count)];
                
                // Spawn as child of Ground so it gets destroyed with it
                GameObject obs = Instantiate(randomPrefab, point.position, Quaternion.identity);
                obs.transform.SetParent(groundParent); 
            }
        }
    }
}
