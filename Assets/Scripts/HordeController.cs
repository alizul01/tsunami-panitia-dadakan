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

    [Header("Debug")]
    [SerializeField] private List<Pulu> collectedPulus = new List<Pulu>();

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
            int frameIndex = history.Count - 1 - ((i + 1) * spacing);

            if (frameIndex >= 0 && frameIndex < history.Count)
            {
                Pulu pulu = collectedPulus[i];
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
            pulu.transform.DOJump(pulu.transform.position, joinJumpPower, 1, 0.5f).onComplete += () =>
            {
                collectedPulus.Add(pulu);
                pulu.JoinHorde();
                OnHordeCountChanged?.Invoke(collectedPulus.Count);
            };
        }
    }
}
