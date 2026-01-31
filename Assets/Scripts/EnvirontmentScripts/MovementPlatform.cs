using UnityEngine;

public class MovementPlatform : MonoBehaviour
{
    [SerializeField] private float scrollSpeed = 5f;
    [SerializeField] private float platformWidth;

    private Vector3 startPosition;

    void Start()
    {
        startPosition = transform.localPosition;
    }

    void Update()
    {
        float move = Time.time * scrollSpeed;
        float newPos = Mathf.Repeat(move, platformWidth);
        transform.localPosition = startPosition + Vector3.left * newPos;
    }
}