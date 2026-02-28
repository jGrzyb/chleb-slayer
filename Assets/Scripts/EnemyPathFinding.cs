using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Collider2D))]
public class EnemyBehaviour : MonoBehaviour
{
    Transform currentTarget = null;
    [SerializeField]private float health = 5f;
    [SerializeField] private float damage = 1f;
    [SerializeField] Transform player;
    NavMeshAgent agent;
    private Collider2D collider2d;
    public enum EnemyState
    {
        Searching,
        Chasing,
        // Attacking,
        Retaliating
    }
    [Header("Enemy Retaliating Settings")]
    [SerializeField] private float time = 3f;
    [SerializeField] private float distance = 10f;

    private EnemyState currentState = EnemyState.Searching;

    private float currentRetaliationTime;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        Player playerObj = FindAnyObjectByType<Player>();
        player = playerObj?.transform;
        collider2d = GetComponent<Collider2D>();
    }

    void Update()
    {
        if (currentTarget == null)
        {
            currentState = EnemyState.Searching;
        }
            switch (currentState)
        {
            case EnemyState.Searching:
                if (currentTarget == null)
                {
                    FindClosestTowerFromList();
                    //agent.isStopped = false;
                    currentState = EnemyState.Chasing;
                }
                break;

            case EnemyState.Chasing:
                if (currentTarget == null) break;
                agent.SetDestination(currentTarget.position);
                if (currentTarget.CompareTag("Player"))
                {
                    checkTower();
                }
                break;
            case EnemyState.Retaliating:
                currentRetaliationTime -= Time.deltaTime;
                if (currentRetaliationTime <= 0f || Vector3.Distance(transform.position, player.position) > distance)
                {
                    currentState = EnemyState.Searching;
                    currentTarget = null;
                    break;
                }
                agent.SetDestination(player.position);
                break;
        }

    }
    void FindClosestTowerFromList()
    {
        if (Tower.ActiveTowers.Count == 0)
        {
            currentTarget = player;
            return;
        }

        float closestDistance = Mathf.Infinity;

        Transform closestTower = null;
        foreach (Transform tower in Tower.ActiveTowers)
        {
            if (tower == null) continue;

            float distanceToTower = (tower.position - transform.position).sqrMagnitude;

            if (distanceToTower < closestDistance)
            {
                closestDistance = distanceToTower;
                closestTower = tower;
            }
        }

        currentTarget = closestTower;
    }

    public void checkTower()
    {
        if (currentTarget.CompareTag("Player"))
        {
            if (Tower.ActiveTowers.Count > 0)
            {
                currentState = EnemyState.Searching;
                currentTarget = null;
            }
        }
    }

    public void SetPlayerAsTarget()
    {
        currentRetaliationTime = time;
        currentState = EnemyState.Retaliating;
        currentTarget = player;
    }

    public void TakeDamage(float damage, GameObject source)
    {
        float previousHealth = health;
        health -= damage;
        if (source.TryGetComponent(out Player _))
        {
            SetPlayerAsTarget();
        }
        if (health <= 0f && previousHealth > 0f)
        {
            Die();
        }
    }

    public void Die()
    {
        Destroy(gameObject);
    }

    private void HandleAttack(Collider2D collision)
    {
        if (collision.TryGetComponent(out IDamageable damageable))
        {
            damageable.TakeDamage(damage);
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        HandleAttack(collision);
    }

    void OnTriggerStay2D(Collider2D collision)
    {
        HandleAttack(collision);
    }
}
