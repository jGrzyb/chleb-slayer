using UnityEngine;

public interface IDamageable
{
    public void TakeDamage(float damage, Vector2 knockBackDirection);
}