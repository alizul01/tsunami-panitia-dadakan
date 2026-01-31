using UnityEngine;

public class FollowPlayerX : MonoBehaviour
{
    [SerializeField] private Transform target; 
    [SerializeField] private float xOffset = -2f; 

    void LateUpdate()
    {
        if (target == null) return;
        transform.position = new Vector3(target.position.x + xOffset, transform.position.y, transform.position.z);
    }
}