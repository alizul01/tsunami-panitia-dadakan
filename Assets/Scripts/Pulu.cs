using UnityEngine;
using DG.Tweening;

public class Pulu : MonoBehaviour
{
    [SerializeField] private Collider2D col;
    [SerializeField] private Transform visual;
    private bool isCollected = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isCollected) return;

        // Check for Player or already collected Pulus (since they follow the player, they are safe to touch)
        // Adjust tag check as needed for your project setup
        if (collision.CompareTag("Player") || collision.CompareTag("Pulu"))
        {
            if (HordeController.Instance != null)
            {
                HordeController.Instance.AddPulu(this);
            }
        }
    }

    public void JoinHorde()
    {
        isCollected = true;
        
        // Disable collision to avoid re-triggering or blocking
        if (col != null) col.enabled = false;
        
        // Ensure Tag is updated so this Pulu can collect others too
        gameObject.tag = "Pulu";
        
        // Visual punch effect
        if (visual != null)
        {
            visual.DOPunchScale(Vector3.one * 0.3f, 0.3f, 10, 1);
        }
    }
}
