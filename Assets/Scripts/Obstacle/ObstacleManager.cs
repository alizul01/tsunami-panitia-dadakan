using UnityEngine;
using UnityEngine.Rendering;

public class ObstacleGenerator : MonoBehaviour
{
    [SerializeField] private float spawnIntervalMin;
    [SerializeField] private float spawnIntervalMax;
    [SerializeField] private Transform spawnPoint;    

    private float timer;
    private ObstaclePooler pooler;
    private float currentSpawnInterval = 0;

    void Start()
    {
        pooler = ObstaclePooler.Instance;
    }

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
        int randomIndex = Random.Range(0, pooler.pools.Count);
        string randomTag = pooler.pools[randomIndex].tag;

        pooler.SpawnFromPool(randomTag, spawnPoint.position, Quaternion.identity);
    }
}