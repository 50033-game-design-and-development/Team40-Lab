using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class HUDManager : Singleton<HUDManager>
{
    [Header("UI Elements")]
    public GameObject restartButton;
    public GameObject resumeButton;
    public GameObject gameOverPanel;
    public GameObject menuButton;

    [Header("Game Events")]
    [SerializeField] private GameEvent gameRestartEvent;
    
    void Start()
    {
    }

    public void GameStart()
    {
        gameOverPanel.SetActive(false);
        resumeButton.SetActive(false);
        menuButton.SetActive(false);
        restartButton.SetActive(false);
    }

    public void GameOver()
    {
        gameOverPanel.SetActive(true);
        restartButton.SetActive(true);
        menuButton.SetActive(true);
    }

    public void OnPauseUI()
    {
        gameOverPanel.SetActive(true);
        resumeButton.SetActive(true);
        menuButton.SetActive(true);
        restartButton.SetActive(true);
    }

    public void OnResumeUI()
    {
        gameOverPanel.SetActive(false);
        resumeButton.SetActive(false);
        menuButton.SetActive(false);
        restartButton.SetActive(false);
    }

    public void OnRestartButtonPressed()
    {
        gameRestartEvent?.Raise();
    }

}
