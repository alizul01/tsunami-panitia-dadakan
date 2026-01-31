using UnityEngine;

public class SceneLoader : MonoBehaviour
{
    [SerializeField] private string targetSceneName;

    public void LoadTargetScene()
    {
        if (SceneTransitionManager.Instance != null)
        {
            SceneTransitionManager.Instance.LoadScene(targetSceneName);
        }
        else
        {
            Debug.LogError("SceneTransitionManager not found! Make sure it exists in the scene.");
            // Fallback
            UnityEngine.SceneManagement.SceneManager.LoadScene(targetSceneName);
        }
    }
}
