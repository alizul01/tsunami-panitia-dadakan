using UnityEngine;

public class Spawner : MonoBehaviour
{
    [Header("Ground Settings")]
    public GameObject groundPrefab;
    public Transform nextSpawnPoint;

    [Header("Sub-Spawners Managers")]
    [SerializeField] private ObstacleSpawner obstacleSpawner;
    [SerializeField] private PuluSpawner puluSpawner;

    void Start()
    {
        // Try to find spawners if not assigned
        if (obstacleSpawner == null) obstacleSpawner = GetComponent<ObstacleSpawner>();
        if (puluSpawner == null) puluSpawner = GetComponent<PuluSpawner>();

        SpawnGround();
    }

    public void SpawnGround()
    {
        if (groundPrefab == null)
        {
            Debug.LogError("Ground Prefab is missing in Spawner!");
            return;
        }

        GameObject newGround = Instantiate(groundPrefab, nextSpawnPoint.position, Quaternion.identity);

        // Get Data from the new ground
        GroundData data = newGround.GetComponent<GroundData>();

        if (data != null)
        {
            // 1. Update next spawn point explicitly from data
            if (data.nextSpawnPoint != null)
            {
                nextSpawnPoint = data.nextSpawnPoint;
            }
            else
            {
                Debug.LogWarning("GroundData found but NextSpawnPoint is not assigned on " + newGround.name);
            }

            // 2. Delegate spawning to sub-spawners
            if (obstacleSpawner != null) 
                obstacleSpawner.SpawnObstacles(data.obstacleSpawnPoints, newGround.transform);
            
            if (puluSpawner != null) 
                puluSpawner.SpawnPulus(data.puluSpawnPoints, newGround.transform);
        }
        else
        {
            // Fallback for objects without GroundData (Legacy support)
            // Assumes the 3rd child is the next point (index 2)
            if (newGround.transform.childCount > 2)
            {
                nextSpawnPoint = newGround.transform.GetChild(2).transform; 
            }
        }
    }
}
