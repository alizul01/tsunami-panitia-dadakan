using UnityEngine;
using TMPro;

public class HordeUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI countText;

    private void Start()
    {
        if (HordeController.Instance != null)
        {
            HordeController.Instance.OnHordeCountChanged += UpdateCount;
            UpdateCount(0); 
        }
    }

    private void OnDestroy()
    {
        if (HordeController.Instance != null)
        {
            HordeController.Instance.OnHordeCountChanged -= UpdateCount;
        }
    }

    private void UpdateCount(int count)
    {
        if (countText != null)
        {
            countText.text = "Horde: " + count;
        }
    }
}
