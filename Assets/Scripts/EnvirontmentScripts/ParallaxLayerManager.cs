using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct ParallaxChunkData
{
    public GameObject prefab;
    public float length;
}

public class ParallaxLayerManager : MonoBehaviour
{
    [Header("Layer Settings")]
    [SerializeField] private List<ParallaxChunkData> chunkPrefabs;
    [SerializeField] private float speedMultiplier = 0.5f; // 0 = diam total, 1 = ikut kamera
    [SerializeField] private int initialChunks = 3;

    [Header("References")]
    [SerializeField] private Transform cameraTransform;

    private class ActiveChunk
    {
        public GameObject go;
        public float length;
    }

    private Queue<ActiveChunk> activeChunks = new Queue<ActiveChunk>();
    private float currentEdgeX = 0f;
    private float safeMargin = 20f;
    private Vector3 lastCameraPos;

    private void Awake()
    {
        cameraTransform = Camera.main.transform;
    }

    void Start()
    {
        lastCameraPos = cameraTransform.position;
        currentEdgeX = transform.position.x;

        for (int i = 0; i < initialChunks; i++)
        {
            SpawnChunk();
        }
    }

    void LateUpdate() // Gunakan LateUpdate untuk pergerakan kamera yang mulus
    {
        // 1. Logika Pergerakan Parallax
        float deltaMovement = cameraTransform.position.x - lastCameraPos.x;
        transform.position += Vector3.right * (deltaMovement * speedMultiplier);
        lastCameraPos = cameraTransform.position;

        // 2. Logika Recycling Chunk
        if (activeChunks.Count > 0)
        {
            ActiveChunk oldest = activeChunks.Peek();

            // Cek apakah chunk sudah keluar dari jangkauan kamera (kiri)
            // Karena parent-nya bergerak (parallax), kita cek posisi relatif terhadap kamera
            if (oldest.go.transform.position.x + (oldest.length / 2f) < cameraTransform.position.x - safeMargin)
            {
                RecycleChunk();
            }
        }
    }

    void SpawnChunk()
    {
        int index = Random.Range(0, chunkPrefabs.Count);
        ParallaxChunkData data = chunkPrefabs[index];

        float spawnPosX = currentEdgeX + (data.length / 2f);
        GameObject go = Instantiate(data.prefab, new Vector3(spawnPosX, transform.position.y, 0), Quaternion.identity);

        go.transform.SetParent(this.transform); // Menjadi anak dari layer ini agar ikut tergeser

        activeChunks.Enqueue(new ActiveChunk { go = go, length = data.length });
        currentEdgeX += data.length;
    }

    void RecycleChunk()
    {
        ActiveChunk chunkToMove = activeChunks.Dequeue();

        float newSpawnPosX = currentEdgeX + (chunkToMove.length / 2f);
        chunkToMove.go.transform.position = new Vector3(newSpawnPosX, transform.position.y, 0);

        currentEdgeX += chunkToMove.length;
        activeChunks.Enqueue(chunkToMove);
    }
}