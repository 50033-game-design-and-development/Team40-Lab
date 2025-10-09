using System;
using UnityEngine;

public static class GameEvents
{
    public static event Action<int> OnEnemyStomped;
    public static void RaiseEnemyStomped(int points) => OnEnemyStomped?.Invoke(points);

    public static event Action<Collider2D> OnStompDetected;
    public static void RaiseStompDetected(Collider2D stompedCollider) => OnStompDetected?.Invoke(stompedCollider);
}