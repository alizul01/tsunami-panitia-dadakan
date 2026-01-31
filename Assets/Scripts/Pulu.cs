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
        if (col != null) col.enabled = false;
        
        gameObject.tag = "Pulu";
        if (visual != null)
        {
            visual.DOPunchScale(Vector3.one * 0.3f, 0.3f, 10, 1);
        }
    }
}
