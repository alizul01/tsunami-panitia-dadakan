using UnityEngine;
using DG.Tweening;

public class Pulu : MonoBehaviour
{
    [SerializeField] private Collider2D col;
    [SerializeField] private Transform visual;
    [SerializeField] private SpriteRenderer spriteRenderer; // Saya ganti namanya agar tidak bentrok dengan keyword 'renderer'

    private bool isCollected = false;

    private void Start()
    {
        // Berikan sprite default saat pertama kali spawn di map
        SetDefaultVisual();
    }

    private void SetDefaultVisual()
    {
        if (PuluVisualManager.Instance != null && spriteRenderer != null)
        {
            spriteRenderer.sprite = PuluVisualManager.Instance.GetPuluDefaultRandom;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isCollected) return;

        // Cek jika menabrak Player atau Pulu lain yang sudah bergabung di Horde
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
        if (isCollected) return;

        isCollected = true;
        if (col != null) col.enabled = false;

        // Ganti tag agar Pulu lain bisa mendeteksi objek ini sebagai bagian dari Horde
        gameObject.tag = "Pulu";

        // Ganti visual menjadi versi pakai masker
        if (PuluVisualManager.Instance != null && spriteRenderer != null)
        {
            spriteRenderer.sprite = PuluVisualManager.Instance.GetPuluWithMaskRandom;
        }

        // Efek animasi saat bergabung
        if (visual != null)
        {
            visual.DOPunchScale(Vector3.one * 0.3f, 0.3f, 10, 1);
        }
    }
}