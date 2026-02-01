using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;
using TMPro;

public class UIGameOver : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private CanvasGroup backgroundFade;
    [SerializeField] private RectTransform gameOverPanel;
    [SerializeField] private TextMeshProUGUI gameOverText;
    [SerializeField] private Button restartButton;

    [Header("Settings")]
    [SerializeField] private Color blinkColor = Color.red;
    private Color originalColor;

    private void Awake()
    {
        gameObject.SetActive(false);
        backgroundFade.alpha = 0;
        restartButton.onClick.AddListener(RestartGame);
        if (gameOverText != null) originalColor = gameOverText.color;
    }

    public void ShowGameOver()
    {
        gameObject.SetActive(true);

        backgroundFade.DOFade(1f, 0.5f).SetUpdate(true);

        gameOverText.transform.localScale = Vector3.zero;
        gameOverText.transform.DOScale(Vector3.one, 0.5f)
            .SetEase(Ease.OutBack)
            .OnComplete(() => {
                gameOverText.transform.DOPunchScale(Vector3.one * 0.2f, 0.5f, 10, 1)
                    .SetLoops(-1)
                    .SetUpdate(true);

                gameOverText.DOColor(blinkColor, 0.3f)
                    .SetLoops(-1, LoopType.Yoyo)
                    .SetUpdate(true);
            });

        restartButton.transform.localScale = Vector3.zero;
        restartButton.transform.DOScale(Vector3.one, 0.5f)
            .SetDelay(0.5f)
            .SetEase(Ease.OutBack)
            .SetUpdate(true);
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;

        DOTween.KillAll();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}