using DG.Tweening;
using UnityEngine;

public class PlayerDamageHandler : MonoBehaviour
{
    [SerializeField] private LayerMask obstacleLayer;
    [SerializeField] private float hitCooldown = 0.5f;
    private float lastHitTime;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        CheckAndDamage(collision.gameObject);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        CheckAndDamage(collision.gameObject);
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        CheckAndDamage(other.gameObject);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        CheckAndDamage(other.gameObject);
    }

    private void CheckAndDamage(GameObject obj)
    {
        if (((1 << obj.layer) & obstacleLayer) != 0)
        {
            TryRemovePulu();
        }
    }

    private void TryRemovePulu()
    {
        if (Time.time > lastHitTime + hitCooldown)
        {
            if (HordeController.Instance != null)
            {
                HordeController.Instance.RemovePulu();
                lastHitTime = Time.time;

                transform.DOShakePosition(0.2f, 0.3f);
            }
        }
    }
}