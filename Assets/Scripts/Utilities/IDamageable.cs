using UnityEngine;

public interface IDamageable
{
    void TakeDamage(float damage, Vector2 hitDir, float knockback = 0f, GameObject src = null);
    bool IsDead { get; }
}
