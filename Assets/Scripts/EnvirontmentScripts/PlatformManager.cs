using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct PlatformChunk
{
    public string name;
    public GameObject prefab;
    public float length;
}

public class PlatformManager : MonoBehaviour
{
    [Header("Chunk Settings")]
    [SerializeField] private List<PlatformChunk> availableChunks;
    [SerializeField] private int initialChunksOnScreen = 5;
    [SerializeField] private Transform cameraTransform;
    private class ActiveChunkData
    {
        public GameObject go;
        public float length;
    }

    private Queue<ActiveChunkData> activeChunks = new Queue<ActiveChunkData>();
    private float nextSpawnX = 0f;
    private float safeMargin = 15f;

    void Start()
    {
        nextSpawnX = transform.position.x;

        for (int i = 0; i < initialChunksOnScreen; i++)
        {
            SpawnInitialChunk();
        }
    }

    void Update()
    {
        if (activeChunks.Count > 0)
        {
            ActiveChunkData oldest = activeChunks.Peek();

            if (oldest.go.transform.position.x + oldest.length < cameraTransform.position.x - safeMargin)
            {
                RecycleChunk();
            }
        }
    }

    void SpawnInitialChunk()
    {
        int index = Random.Range(0, availableChunks.Count);
        PlatformChunk data = availableChunks[index];

        GameObject go = Instantiate(data.prefab, new Vector3(nextSpawnX, 0, 0), Quaternion.identity);
        go.transform.SetParent(this.transform);

        ActiveChunkData newChunk = new ActiveChunkData { go = go, length = data.length };

        activeChunks.Enqueue(newChunk);

        nextSpawnX += data.length;
    }

    void RecycleChunk()
    {
        ActiveChunkData chunkToMove = activeChunks.Dequeue();

        chunkToMove.go.transform.position = new Vector3(nextSpawnX, 0, 0);

        nextSpawnX += chunkToMove.length;

        activeChunks.Enqueue(chunkToMove);
    }
}