using System.Collections.Generic;
using UnityEngine;


public class ObstacleGenerator: MonoBehaviour
{
    [SerializeField] private float spawnIntervalMin;
    [SerializeField] private float spawnIntervalMax;
    [SerializeField] private Transform spawnPoint;

    private float timer;
    private ObstaclePooler pooler;
    private float currentSpawnInterval = 0;

    private void Start()
    {
        pooler = GetComponent<ObstaclePooler>();
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
        int randomIndex = Random.Range(0, pooler.pools.Count);
        string randomTag = pooler.pools[randomIndex].tag;

        pooler.SpawnFromPool(randomTag, spawnPoint.position, Quaternion.identity);
    }
}

