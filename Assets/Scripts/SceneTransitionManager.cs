using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections;

public class SceneTransitionManager : MonoBehaviour
{
    public static SceneTransitionManager Instance { get; private set; }

    [Header("Setup")]
    [SerializeField] private CanvasGroup fadePanel;
    [SerializeField] private float transitionDuration = 0.5f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        if (fadePanel != null)
        {
            fadePanel.alpha = 0;
            fadePanel.blocksRaycasts = false;
        }
    }

    public void LoadScene(string sceneName)
    {
        StartCoroutine(TransitionRoutine(sceneName));
    }

    private IEnumerator TransitionRoutine(string sceneName)
    {
        fadePanel.blocksRaycasts = true;
        
        Tween fadeOut = fadePanel.DOFade(1f, transitionDuration);
        yield return fadeOut.WaitForCompletion();

        // 2. Load Scene
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        asyncLoad.allowSceneActivation = false;

        // Wait until loaded (progress >= 0.9f)
        while (asyncLoad.progress < 0.9f)
        {
            yield return null;
        }

        // Small delay if needed or just activate
        asyncLoad.allowSceneActivation = true;

        // Wait for scene to fully activate
        yield return null; 

        // 3. Fade In (Screen reveals new scene)
        Tween fadeIn = fadePanel.DOFade(0f, transitionDuration);
        yield return fadeIn.WaitForCompletion();

        // 4. Restore input
        fadePanel.blocksRaycasts = false;
    }
}
