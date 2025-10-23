using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Game/HUD State")]
public class HUDState : ScriptableObject
{
    public UnityEvent onGameStart;
    public UnityEvent onGameOver;
    public UnityEvent onPause;
    public UnityEvent onResume;

    public void GameStart() => onGameStart?.Invoke();
    public void GameOver() => onGameOver?.Invoke();
    public void Pause() => onPause?.Invoke();
    public void Resume() => onResume?.Invoke();
}
