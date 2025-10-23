using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    [Header("HUD State")]
    public HUDState hudState;

    void Start()
    {
        Time.timeScale = 1.0f;
        hudState.GameStart();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void GameRestart()
    {
        Time.timeScale = 1f;
        hudState.GameStart();
    }

    public void GamePause()
    {
        Time.timeScale = 0f;
        hudState.Pause();
    }
    public void GameResume()
    {
        Time.timeScale = 1f;
        hudState.Resume();
    }

    public void GameOver()
    {
        Time.timeScale = 0f;
        hudState.GameOver();
    }
}