using System.Collections.Generic;
using UnityEngine;

public class PuluGenerator : MonoBehaviour
{
    [SerializeField] private float spawnIntervalMin = 1f; 
    [SerializeField] private float spawnIntervalMax = 3f;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private List<GameObject> puluPrefabs; 

    private float timer;
    private float currentSpawnInterval = 0;

    void Start()
    {
        currentSpawnInterval = Random.Range(spawnIntervalMin, spawnIntervalMax);
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= currentSpawnInterval)
        {
            GenerateRandomObstacle();
            timer = 0;
            currentSpawnInterval = Random.Range(spawnIntervalMin, spawnIntervalMax);
        }
    }

    void GenerateRandomObstacle()
    {
        if (puluPrefabs == null || puluPrefabs.Count == 0 || spawnPoint == null)
        {
            Debug.LogWarning("PuluGenerator: List prefab kosong atau SpawnPoint belum diisi!");
            return;
        }

        int randomIndex = Random.Range(0, puluPrefabs.Count);
        GameObject prefabToSpawn = puluPrefabs[randomIndex];

        if (prefabToSpawn != null)
        {
            Instantiate(prefabToSpawn, spawnPoint.position, Quaternion.identity);
        }
    }
}