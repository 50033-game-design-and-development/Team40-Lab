using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public void GameRestart()
    {
        var enemies = GetComponentsInChildren<EnemyMovement>(true);
        foreach (var enemy in enemies)
        {
            enemy.GameRestart();
        }
    }
}
