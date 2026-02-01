using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject groundPrefab;
    public Transform nextSpawnPoint;

    void Start()
    {
        SpawnGround();
    }

    public void SpawnGround()
    {
        if (groundPrefab == null)
        {
            Debug.LogError("Ground Prefab tidak ditemukan atau telah didestroy. Pastikan assign Prefab dari Project window (aset), bukan dari Hierarchy (scene).");
            return;
        }

        GameObject temp = Instantiate(groundPrefab, nextSpawnPoint.position, Quaternion.identity);
        nextSpawnPoint = temp.transform.GetChild(2).transform;
    }
}
