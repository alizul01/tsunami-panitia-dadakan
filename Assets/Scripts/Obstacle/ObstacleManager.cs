using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;


public class PuluGenerator : MonoBehaviour
{
    [SerializeField] private float spawnIntervalMin;
    [SerializeField] private float spawnIntervalMax;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private List<GameObject> PuluPrefabs;

    private float timer;
    private float currentSpawnInterval = 0;


    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= currentSpawnInterval)
        {
            GenerateRandomObstacle();
            timer = 0;
            currentSpawnInterval = Random.Range(spawnIntervalMin,spawnIntervalMax);
        }
    }

    void GenerateRandomObstacle()
    {
        // Pilih tag secara acak dari list pool yang tersedia
        int randomIndex = Random.Range(0, PuluPrefabs.Count);
        GameObject prefabToSpawn = PuluPrefabs[randomIndex];
        Instantiate(prefabToSpawn, spawnPoint.position, Quaternion.identity);
    }
}