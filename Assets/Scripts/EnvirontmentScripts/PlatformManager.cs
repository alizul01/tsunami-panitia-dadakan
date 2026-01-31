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
    [Header("Settings")]
    [SerializeField] private List<PlatformChunk> availableChunks;
    [SerializeField] private int initialChunksOnScreen = 5;
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private float safeMargin = 15f;

    private class ActiveChunkData
    {
        public GameObject go;
        public float length;
    }

    private Queue<ActiveChunkData> activeChunks = new Queue<ActiveChunkData>();
    private float currentEdgeX = 0f; // Menandai koordinat ujung paling kanan dari chunk terakhir

    void Start()
    {
        currentEdgeX = transform.position.x;

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

            // Jarak pengecekan: Jika posisi tengah + setengah panjang < batas kamera
            if (oldest.go.transform.position.x + (oldest.length / 2f) < cameraTransform.position.x - safeMargin)
            {
                RecycleChunk();
            }
        }
    }

    void SpawnInitialChunk()
    {
        int index = Random.Range(0, availableChunks.Count);
        PlatformChunk data = availableChunks[index];

        // Rumus: Posisi Tengah = Ujung Terakhir + (Setengah Panjang Chunk Baru)
        float spawnPosX = currentEdgeX + (data.length / 2f);

        GameObject go = Instantiate(data.prefab, new Vector3(spawnPosX, 0, 0), Quaternion.identity);
        go.transform.SetParent(this.transform);

        activeChunks.Enqueue(new ActiveChunkData { go = go, length = data.length });

        // Update ujung terakhir: Tambahkan seluruh panjang chunk baru
        currentEdgeX += data.length;
    }

    void RecycleChunk()
    {
        ActiveChunkData chunkToMove = activeChunks.Dequeue();

        // Sama seperti spawn: Pindahkan titik tengah ke (Ujung + Setengah Panjangnya)
        float newSpawnPosX = currentEdgeX + (chunkToMove.length / 2f);
        chunkToMove.go.transform.position = new Vector3(newSpawnPosX, 0, 0);

        // Update ujung terakhir
        currentEdgeX += chunkToMove.length;

        activeChunks.Enqueue(chunkToMove);
    }
}