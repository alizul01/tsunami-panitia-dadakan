using UnityEngine;

public class ObstacleMovement : MonoBehaviour
{
    public float speed = 5f; // Samakan dengan scrollSpeed platform

    void Update()
    {
        transform.Translate(Vector3.left * speed * Time.deltaTime);

        // Jika sudah jauh di kiri layar, matikan objek agar kembali ke pool
        if (transform.position.x < -15f)
        {
            gameObject.SetActive(false);
        }
    }
}