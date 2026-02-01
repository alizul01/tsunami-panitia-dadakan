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
    
    [Tooltip("Jarak (dalam Unity Unit) antara setiap Pulu dalam barisan. Lebih kecil = lebih rapat.")]
    [SerializeField] private float followSpacing = 0.8f; 
    
    [Tooltip("Sensitivitas rekaman path. Makin kecil makin akurat kurvanya.")]
    [SerializeField] private float recordDistanceThreshold = 0.05f;

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
        if (leader == null) return;
        RecordSnapshot();
        UpdateHordePositions();
    }

    private void RecordSnapshot()
    {
        // 1. Initial add
        if (history.Count == 0)
        {
            history.Add(new Snapshot(leader.position, leader.localScale));
            return;
        }

        // 2. Distance check: Hanya rekam jika leader sudah bergerak cukup jauh dari titik terakhir
        // Ini membuat spacing tidak terpengaruh oleh FPS atau kecepatan lari
        float dist = Vector3.Distance(leader.position, history[history.Count - 1].position);
        if (dist >= recordDistanceThreshold)
        {
            history.Add(new Snapshot(leader.position, leader.localScale));
            
            // 3. Pruning: Hapus history lama yang sudah tidak dipakai
            // Kita hitung total jarak yang dibutuhkan horde
            float requiredHistoryLength = (collectedPulus.Count + 10) * followSpacing; 
            
            // Estimasi jumlah point yang dibutuhkan (safety margin x2)
            int maxPoints = Mathf.CeilToInt(requiredHistoryLength / recordDistanceThreshold);
            
            if (history.Count > maxPoints)
            {
                // Remove yang paling tua (index 0)
                history.RemoveAt(0);
            }
        }
    }

    private void UpdateHordePositions()
    {
        if (history.Count < 2) return;

        // Path logic: Leader -> History[Last] -> History[Last-1] ... -> History[0]
        
        for (int i = 0; i < collectedPulus.Count; i++)
        {
            Pulu pulu = collectedPulus[i];

            // Jangan update posisi jika sedang animasi jump (join/death)
            if (jumpingPulus.Contains(pulu)) continue;

            // Target jarak untuk pulu ke-i
            float targetDist = (i + 1) * followSpacing;
            
            SetPuluPositionOnPath(pulu, targetDist);
        }
    }

    private void SetPuluPositionOnPath(Pulu pulu, float targetDist)
    {
        float currentDist = 0f;
        
        // Poin pertama adalah Leader sendiri
        Vector3 prevPos = leader.position;
        Vector3 prevScale = leader.localScale;

        // Cek segmen dari Leader ke History terakhir
        // Ingat history paling baru ada di index TERAKHIR (Count-1)
        
        // Kita iterasi mundur dari yang paling baru
        for (int i = history.Count - 1; i >= -1; i--)
        {
            Vector3 pointPos;
            Vector3 pointScale;

            if (i == -1) // Handling edge case kalau history sudah habis tapi jarak belum ketemu (jarang)
            {
                 // Fallback ke point terlama
                 pointPos = history[0].position;
                 pointScale = history[0].scale;
            }
            else
            {
                 pointPos = history[i].position;
                 pointScale = history[i].scale;
            }

            // Hitung jarak segmen ini
            float segmentDist = Vector3.Distance(prevPos, pointPos);

            // Apakah target ada di dalam segmen ini?
            if (currentDist + segmentDist >= targetDist)
            {
                // Ketemu! Kita lerp di antara prevPos dan pointPos
                float remaining = targetDist - currentDist;
                float t = remaining / segmentDist;

                pulu.transform.position = Vector3.Lerp(prevPos, pointPos, t);
                pulu.transform.localScale = Vector3.Lerp(prevScale, pointScale, t);
                
                // Rotasi polish: Hadapkan pulu ke arah gerakan
                // Vector3 dir = (prevPos - pointPos).normalized; // Arah lari
                // if (dir.x != 0) pulu.transform.localScale = new Vector3(Mathf.Sign(dir.x) * Mathf.Abs(pulu.transform.localScale.x), pulu.transform.localScale.y, pulu.transform.localScale.z);
                
                return;
            }

            currentDist += segmentDist;
            prevPos = pointPos;
            prevScale = pointScale;
            
            if (i == -1) break; // Break loop
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
            if (UIManager.Instance != null)
            {
                UIManager.Instance.OnGameOver?.Invoke();
            }
            else
            {
                Debug.LogWarning("UIManager Instance is null! Cannot invoke OnGameOver.");
            }
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