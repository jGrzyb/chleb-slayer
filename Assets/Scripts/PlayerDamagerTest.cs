using UnityEngine;

public class PlayerDamagerTest : MonoBehaviour
{
    [SerializeField] private float damageAmount = 0.1f;

    private void HandleCollision(Collider2D collider)
    {
        if (collider.TryGetComponent(out Player player))
        {
            player.TakeDamage(damageAmount, Vector2.zero);
        }
    }
    private void OnTriggerEnter2D(Collider2D collider)
    {
        HandleCollision(collider);
    }

    private void OnTriggerStay2D(Collider2D collider)
    {
        HandleCollision(collider);
    }
}