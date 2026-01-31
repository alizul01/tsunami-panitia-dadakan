using System;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class HordeController : MonoBehaviour
{
    public static HordeController Instance { get; private set; }
    public event Action<int> OnHordeCountChanged;

    [Header("Settings")]
    [SerializeField] private Transform leader;
    public Transform Leader => leader;
    [SerializeField] private int spacing = 10;
    [SerializeField] private float joinJumpPower = 2f;

    [Header("Visual Settings")]
    [SerializeField] private int minSortingOrder = 1;
    [SerializeField] private int maxSortingOrder = 10;

    [Header("Debug")]
    [SerializeField] private List<Pulu> collectedPulus = new List<Pulu>();

    [Header("Death Settings")]
    [SerializeField] private Color blinkColor = Color.white;
    [SerializeField] private float deathJumpPower = 3f;

    // HashSet untuk melacak Pulu yang sedang melompat agar posisinya tidak ditimpa
    private HashSet<Pulu> jumpingPulus = new HashSet<Pulu>();

    private struct Snapshot
    {
        public Vector3 position;
        public Vector3 scale;

        public Snapshot(Vector3 position, Vector3 scale)
        {
            this.position = position;
            this.scale = scale;
        }
    }

    private List<Snapshot> history = new List<Snapshot>();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        if (leader == null) leader = transform;
    }

    private void LateUpdate()
    {
        RecordSnapshot();
        UpdateHordePositions();
    }

    private void RecordSnapshot()
    {
        history.Add(new Snapshot(leader.position, leader.localScale));
        int maxFramesNeeded = (collectedPulus.Count + 1) * spacing;
        if (history.Count > maxFramesNeeded)
        {
            history.RemoveAt(0);
        }
    }

    private void UpdateHordePositions()
    {
        for (int i = 0; i < collectedPulus.Count; i++)
        {
            Pulu pulu = collectedPulus[i];

            // LOGIC FIX: Jangan timpa posisi jika Pulu sedang dalam proses DOJump
            if (jumpingPulus.Contains(pulu)) continue;

            int frameIndex = history.Count - 1 - ((i + 1) * spacing);

            if (frameIndex >= 0 && frameIndex < history.Count)
            {
                Snapshot snap = history[frameIndex];
                pulu.transform.position = snap.position;
                pulu.transform.localScale = snap.scale;
            }
        }
    }

    public void AddPulu(Pulu pulu)
    {
        if (!collectedPulus.Contains(pulu))
        {
            collectedPulus.Add(pulu);

            // 1. Atur Random Sorting Order agar tumpukan visual bervariasi
            SpriteRenderer sr = pulu.GetComponentInChildren<SpriteRenderer>();
            if (sr != null)
            {
                sr.sortingOrder = UnityEngine.Random.Range(minSortingOrder, maxSortingOrder);
            }

            // 2. Masukkan ke daftar jumping untuk mengunci posisi di Update
            jumpingPulus.Add(pulu);

            // 3. Eksekusi Jump
            pulu.transform.DOJump(pulu.transform.position, joinJumpPower, 1, 0.5f)
                .OnComplete(() => {
                    // 4. Setelah selesai lompat, izinkan UpdateHordePositions mengambil alih
                    jumpingPulus.Remove(pulu);
                });

            pulu.JoinHorde();
            OnHordeCountChanged?.Invoke(collectedPulus.Count);
        }
    }

    public void RemovePulu()
    {
        if (collectedPulus.Count <= 0)
        {
            Debug.Log("Game Over! Tidak ada Pulu tersisa.");
            return;
        }

        int lastIndex = collectedPulus.Count - 1;
        Pulu puluToDie = collectedPulus[lastIndex];
        collectedPulus.RemoveAt(lastIndex);

        OnHordeCountChanged?.Invoke(collectedPulus.Count);
        HandlePuluDeath(puluToDie);
    }

    private void HandlePuluDeath(Pulu pulu)
    {
        pulu.enabled = false;

        Collider2D col = pulu.GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        SpriteRenderer sr = pulu.SpriteRenderer;

        if (sr != null)
        {
            sr.DOColor(blinkColor, 0.05f).SetLoops(2, LoopType.Yoyo);
            sr.DOFade(0, 0.6f).SetEase(Ease.InQuad);
        }

        float randomExitX = UnityEngine.Random.Range(-3f, 3f);
        float randomExitY = UnityEngine.Random.Range(-5f, -8f); 
        Vector3 jumpTarget = pulu.transform.position + new Vector3(randomExitX, randomExitY, 0);

        pulu.transform.DOJump(jumpTarget, deathJumpPower, 1, 0.6f).SetEase(Ease.Linear);

        pulu.transform.DOScale(Vector3.zero, 0.6f).SetEase(Ease.InBack)
            .OnComplete(() => {
                Destroy(pulu.gameObject);
            });
    }
}