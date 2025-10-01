using System.Collections;
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
    private GameObject currentGoomba;

    private int score = 0;

    void Start()
    {
        SpawnGoomba();
        gameStart.Invoke();
        Time.timeScale = 1.0f;
    }
    void Update()
    {

    }

    public void GameRestart()
    {
        Time.timeScale = 1.0f;
        // reset score
        score = 0;
        SetScore(score);

        // respawn Goomba
        if (currentGoomba != null)
            Destroy(currentGoomba);
        SpawnGoomba();

        gameRestart.Invoke();

    }

    public void IncreaseScore(int increment)
    {
        score += increment;
        SetScore(score);
    }

    public void SetScore(int score)
    {
        scoreChange.Invoke(score);
    }


    public void GameOver()
    {
        Time.timeScale = 0.0f;
        gameOver.Invoke();
    }

    private void SpawnGoomba()
    {
        if (goombaPrefab != null && spawnPoint != null)
        {
            currentGoomba = Instantiate(goombaPrefab, spawnPoint.position, spawnPoint.rotation);

            var evt = currentGoomba.GetComponent<AnimationEventIntTool>();
            if (evt != null) evt.useInt.AddListener(IncreaseScore);
        }
    }
}