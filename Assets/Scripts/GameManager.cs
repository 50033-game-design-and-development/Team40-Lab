using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    // events
    public UnityEvent gameStart;
    public UnityEvent gameRestart;
    public UnityEvent<int> scoreChange;
    public UnityEvent gameOver;

    [Header("Goomba Spawn Settings")]
    public GameObject goombaPrefab;
    public Transform spawnPoint;
    public int noOfGoombas = 5;
    public float spacingX = 10f;

    private readonly List<GameObject> spawnedGoombas = new List<GameObject>();

    private int score = 0;

    void Start()
    {
        SpawnGoombas();
        gameStart?.Invoke();
        Time.timeScale = 1.0f;
    }

    private void OnEnable()
    {
        GameEvents.OnEnemyStomped += IncreaseScore;
    }

    private void OnDisable()
    {
        GameEvents.OnEnemyStomped -= IncreaseScore;
    }

    public void GameRestart()
    {
        Time.timeScale = 1.0f;

        score = 0;
        SetScore(score);

        ClearGoombas();
        SpawnGoombas();

        gameRestart?.Invoke();
    }

    public void IncreaseScore(int increment)
    {
        score += increment;
        SetScore(score);
    }

    public void SetScore(int score)
    {
        scoreChange?.Invoke(score);
    }

    public void GameOver()
    {
        Time.timeScale = 0.0f;
        gameOver?.Invoke();
    }

    private void SpawnGoombas()
    {
        if (goombaPrefab == null || spawnPoint == null) return;

        for (int i = 0; i < noOfGoombas; i++)
        {
            Vector3 pos = spawnPoint.position + Vector3.right * (i * spacingX);
            GameObject goomba = Instantiate(goombaPrefab, pos, spawnPoint.rotation);
            spawnedGoombas.Add(goomba);

            // var evt = goomba.GetComponent<AnimationEventIntTool>();
            // if (evt != null) evt.useInt.AddListener(IncreaseScore);
        }
    }

    private void ClearGoombas()
    {
        for (int i = 0; i < spawnedGoombas.Count; i++)
        {
            if (spawnedGoombas[i] != null) Destroy(spawnedGoombas[i]);
        }
        spawnedGoombas.Clear();
    }
}
