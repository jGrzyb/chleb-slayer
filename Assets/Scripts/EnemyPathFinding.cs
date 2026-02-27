using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class EnemyBehaviour : MonoBehaviour
{
    Transform currentTarget = null;
    string tagTower = "Tower";
    [SerializeField] Transform player;
    NavMeshAgent agent;

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
            FindClosestTowerFromList();
        }
        if (currentTarget != null)
        {
            agent.SetDestination(currentTarget.position);
        }
    }
    void FindClosestTowerFromList()
    {
        if (Tower.ActiveTowers.Count == 0)
        {
            SetPlayerAsTarget();
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
    public void takeDamage()
    {
        if (currentTarget.CompareTag("Tower") && Vector3.Distance(transform.position, currentTarget.position) < 0.5f)
        {
            Destroy(currentTarget.gameObject);
            Tower.ActiveTowers.Remove(currentTarget);
            currentTarget = null;
        }
        if (currentTarget.CompareTag("Player"))
        {
            //zadaje obra¿enia graczowi
        }
    }
    void OnDestroy()
    {
        RemoveSelfFromList();
    }
    public void SetPlayerAsTarget()
    {
        currentTarget = player;
    }
}
