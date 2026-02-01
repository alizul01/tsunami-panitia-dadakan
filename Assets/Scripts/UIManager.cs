using System;
using UnityEngine;

public class UIManager : SingletonMonoBehaviour<UIManager>
{
    public Action OnGameOver;


    [SerializeField] private UIGameOver uiGameOver;


    private void OnEnable()
    {
        OnGameOver += ShowGameOverUI;
    }
    private void OnDisable()
    {
        OnGameOver -= ShowGameOverUI;
    }

    public void ShowGameOverUI()
    {
        if (uiGameOver != null)
        {
            uiGameOver.ShowGameOver();
        }
    }
}
