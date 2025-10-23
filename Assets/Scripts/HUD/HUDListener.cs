using UnityEngine;

public class HUDListener : MonoBehaviour
{
    public HUDState hudState;

    public GameObject restartButton;
    public GameObject resumeButton;
    public GameObject gameOverPanel;
    public GameObject menuButton;

    private void OnEnable()
    {
        hudState.onGameStart.AddListener(GameStart);
        hudState.onGameOver.AddListener(GameOver);
        hudState.onPause.AddListener(OnPauseUI);
        hudState.onResume.AddListener(OnResumeUI);
    }

    private void OnDisable()
    {
        hudState.onGameStart.RemoveListener(GameStart);
        hudState.onGameOver.RemoveListener(GameOver);
        hudState.onPause.RemoveListener(OnPauseUI);
        hudState.onResume.RemoveListener(OnResumeUI);
    }

    private void GameStart()
    {
        gameOverPanel.SetActive(false);
        resumeButton.SetActive(false);
        menuButton.SetActive(false);
        restartButton.SetActive(false);
    }

    private void GameOver()
    {
        gameOverPanel.SetActive(true);
        restartButton.SetActive(true);
        menuButton.SetActive(true);
    }

    private void OnPauseUI()
    {
        gameOverPanel.SetActive(true);
        resumeButton.SetActive(true);
        menuButton.SetActive(true);
        restartButton.SetActive(true);
    }

    private void OnResumeUI()
    {
        gameOverPanel.SetActive(false);
        resumeButton.SetActive(false);
        menuButton.SetActive(false);
        restartButton.SetActive(false);
    }
}
