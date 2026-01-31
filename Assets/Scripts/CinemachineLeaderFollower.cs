using Unity.Cinemachine;
using UnityEngine;

public class CinemachineLeaderFollower : MonoBehaviour
{
    private CinemachineCamera vcam;

    private void Start()
    {
        vcam = GetComponent<CinemachineCamera>();
        if (vcam == null) return;

        if (HordeController.Instance != null && HordeController.Instance.Leader != null)
        {
            vcam.Follow = HordeController.Instance.Leader;
        }
    }
}
