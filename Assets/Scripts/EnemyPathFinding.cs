using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class EnemyBehaviour : MonoBehaviour
{
    Transform currentTarget = null;
    [SerializeField] Transform player;
    NavMeshAgent agent;
    public enum EnemyState
    {
        Searching,
        Chasing,
        Attacking,
        Retaliating
    }
    [Header("Enemy Retaliating Settings")]
    public float time = 3f;
    public float distance = 10f;

    private EnemyState currentState = EnemyState.Searching;

    private float currentRetaliationTime;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
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

                if  (Vector3.Distance(transform.position, currentTarget.position) < 1.0f)
                {
                    currentState = EnemyState.Attacking;
                    //agent.isStopped = true; // Zatrzymaj Agenta
                } 
                if (currentTarget.CompareTag("Player"))
                {
                    checkTower();
                }
                break;
            case EnemyState.Attacking:
                checkTower();
                if (currentTarget == null) break;
                takeDamage();
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
                if (Vector3.Distance(transform.position, player.position) < 1.0f)
                {
                    DamagePlayer();
                }
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

    public void RemoveSelfFromList()
    {
        if (Tower.ActiveTowers.Contains(currentTarget))
        {
            Tower.ActiveTowers.Remove(currentTarget);
        }
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

    //warunek te¿ pewnie do zmiany na razie odleg³oœæ jest, ale mo¿e byæ np. sprawdzanie czy jest w zasiêgu ataku
    public void takeDamage()
    {
        if (currentTarget.CompareTag("Tower") && Vector3.Distance(transform.position, currentTarget.position) < 1.0f)
        {
            DamageTower();
        }
        else if (currentTarget.CompareTag("Player") && Vector3.Distance(transform.position, currentTarget.position) < 1.0f)
        {
            DamagePlayer();
        }
    }

    //tu zmieniaæ system zadawania obra¿eñ graczowi i wie¿y
    public void DamagePlayer()
    {

    }
    public void DamageTower()
    {
        Destroy(currentTarget.gameObject);
        Tower.ActiveTowers.Remove(currentTarget);
        currentTarget = null;
    }
    void OnDestroy()
    {
        RemoveSelfFromList();
    }
    public void SetPlayerAsTarget()
    {
        currentRetaliationTime = time;
        currentState = EnemyState.Retaliating;
        currentTarget = player;
    }
}
