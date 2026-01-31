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
    [SerializeField] private float speedMultiplier = 0.5f;
    [SerializeField] private int initialChunks = 3;

    [Header("References")]
    [SerializeField] private Transform cameraTransform;

    private class ActiveChunk
    {
        public GameObject go;
        public float length;
    }

    private Queue<ActiveChunk> activeChunks = new Queue<ActiveChunk>();

    // Gunakan posisi lokal untuk menjaga jarak antar chunk tetap presisi
    private float nextLocalEdgeX = 0f;

    [SerializeField] private float safeMargin = 20f;
    private Vector3 lastCameraPos;

    private void Awake()
    {
        if (cameraTransform == null) cameraTransform = Camera.main.transform;
    }

    void Start()
    {
        lastCameraPos = cameraTransform.position;

        // Mulai dari 0 relatif terhadap posisi parent
        nextLocalEdgeX = 0f;

        for (int i = 0; i < initialChunks; i++)
        {
            SpawnChunk();
        }
    }

    void LateUpdate()
    {
        // 1. Pergerakan Parallax (Sumbu X)
        float deltaMovement = cameraTransform.position.x - lastCameraPos.x;
        transform.position += Vector3.right * (deltaMovement * speedMultiplier);
        lastCameraPos = cameraTransform.position;

        // 2. Logika Recycling
        if (activeChunks.Count > 0)
        {
            ActiveChunk oldest = activeChunks.Peek();

            // Cek posisi WORLD objek terhadap batas WORLD kamera
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

        // Hitung posisi lokal (Center Pivot)
        float spawnLocalX = nextLocalEdgeX + (data.length / 2f);

        GameObject go = Instantiate(data.prefab);
        go.transform.SetParent(this.transform);

        // Set localPosition agar relatif terhadap layer ini
        go.transform.localPosition = new Vector3(spawnLocalX, 0, 0);

        activeChunks.Enqueue(new ActiveChunk { go = go, length = data.length });

        // Update batas lokal berikutnya
        nextLocalEdgeX += data.length;
    }

    void RecycleChunk()
    {
        ActiveChunk chunkToMove = activeChunks.Dequeue();

        // Pindahkan posisi LOKAL ke ujung antrean lokal
        float newLocalPosX = nextLocalEdgeX + (chunkToMove.length / 2f);
        chunkToMove.go.transform.localPosition = new Vector3(newLocalPosX, 0, 0);

        // Update batas lokal
        nextLocalEdgeX += chunkToMove.length;

        activeChunks.Enqueue(chunkToMove);
    }
}