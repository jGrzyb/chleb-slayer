using System;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager I { get; private set; }
    public PlayerStats playerStats = new PlayerStats();

    void Awake()
    {
        if (I != null && I != this)
        {
            Destroy(gameObject);
            return;
        }
        I = this;
        DontDestroyOnLoad(gameObject);
    }

    [Serializable]
    public class PlayerStats
    {
        public float attackDamage = 0.1f;
        public float attackCooldown = 0.3f;
        public float attackKnockback = 1f;
        public float critChance = 0f;
        public float attackRange = 1f;
        public float damageResistance = 0f;
        public float damageInvincibilityDuration = 0.5f;
        public float movementSpeed = 5f;
        public float dashCooldown = 1f;
        public float towerDamage = 0.1f;
        public float towerResistance = 0f;
    }
}