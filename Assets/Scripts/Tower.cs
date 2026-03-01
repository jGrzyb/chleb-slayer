using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour, IDamageable
{
    [SerializeField] private float invincibilityDuration = 0.5f;
    [SerializeField] private Bullet bulletPrefab;
    private float currentHealth;
    private GameManager.PlayerStats Stats => GameManager.I.playerStats;
    public static List<Transform> ActiveTowers = new List<Transform>();
    private float invincibilityRemainingTime = 0f;
    
    void Start()
    {
        GameManager.I.endStats.towersBuilt++;
        currentHealth = Stats.towerMaxHealth;
        UpdateCooldown();
        SoundManager.I.PlayOverlap(SoundManager.I.PlaceTowerClip);
    }

    void FixedUpdate()
    {
        invincibilityRemainingTime -= Time.fixedDeltaTime;
    }

    public void TakeDamage(float damage, Vector2 knockBackDirection)
    {
        if (invincibilityRemainingTime > 0f) return;
        Debug.Log($"Tower takes {damage} damage.");
        invincibilityRemainingTime = invincibilityDuration;
        float previousHealth = currentHealth;
        currentHealth -= damage * (1f - Stats.towerResistance);
        if (currentHealth <= 0f && previousHealth > 0f)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("Tower has died.");
        Destroy(gameObject);
    }

    private void Attack()
    {
        EnemyBehaviour closestEnemy = null;
        float closestDistance = float.MaxValue;
        foreach (var enemy in EnemyBehaviour.AllEnemies)
        {
            float distance = Vector2.Distance(transform.position, enemy.transform.position);
            if (distance < closestDistance)
            {
            closestDistance = distance;
            closestEnemy = enemy;
            }
        }
        if (closestEnemy == null) return;

        Vector2 direction = (closestEnemy.transform.position - transform.position).normalized;
        Bullet bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
        bullet.Initialize(direction);
        SoundManager.I.PlayShoot(SoundManager.I.ShootClip);
    }

    private void UpdateCooldown()
    {
        CancelInvoke(nameof(Attack));
        InvokeRepeating(nameof(Attack), Stats.towerAttackCooldown, Stats.towerAttackCooldown);
    }

    void OnEnable()
    {
        ActiveTowers.Add(transform);
    }

    void OnDisable()
    {
        ActiveTowers.Remove(transform);
    }
}