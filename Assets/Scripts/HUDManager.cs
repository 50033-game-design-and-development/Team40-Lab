using UnityEngine;
using TMPro;

public class HUDManager : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] TextMeshProUGUI scoreText;
    [SerializeField] RectTransform restartButton;
    [SerializeField] GameObject gameOverPanel;

    [Header("Positions (Anchored)")]
    [SerializeField] Vector2 scorePos_InGame;
    [SerializeField] Vector2 restartPos_InGame;

    void Awake()
    {
        if (!scoreText) Debug.LogError("[HUDManager] Score Text is not assigned.");
        if (!restartButton) Debug.LogError("[HUDManager] Restart Button is not assigned.");
        if (!gameOverPanel) Debug.LogError("[HUDManager] Game Over Panel is not assigned.");
    }

    void Start()
    {
        GameStart();
    }

    public void GameStart()
    {
        if (gameOverPanel) gameOverPanel.SetActive(false);

        if (scoreText)
            scoreText.rectTransform.anchoredPosition = scorePos_InGame;

        if (restartButton)
            restartButton.anchoredPosition = restartPos_InGame;
    }

    public void GameOver()
    {
        if (gameOverPanel) gameOverPanel.SetActive(true);

    }

    public void SetScore(int score)
    {
        if (scoreText)
            scoreText.text = $"Score: {score}";
    }
}
