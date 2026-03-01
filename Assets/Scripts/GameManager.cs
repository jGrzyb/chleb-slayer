using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager I { get; private set; }
    public PlayerStats playerStats = new PlayerStats();
    public EndStats endStats = new EndStats();

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
    void Update()
    {
        endStats.timePlayed += Time.deltaTime;
    }

    [Serializable]
    public class PlayerStats
    {
        public float maxHealth = 100f;
        public float attackDamage = 1f;
        public float attackCooldown = 0.3f;
        public float attackKnockback = 1f;
        public float critChance = 0f;
        public float attackRange = 1f;
        public float damageResistance = 0f;
        public float damageInvincibilityDuration = 0.5f;
        public float movementSpeed = 5f;
        public float dashCooldown = 1f;
        public float towerDamage = 1f;
        private float _towerAttackCooldown = 1f;
        public float towerAttackCooldown
        {
            get { return _towerAttackCooldown; }
            set
            {
                _towerAttackCooldown = value;
                OnTowerAttackCooldownChanged?.Invoke();
            }
        }
        public event Action OnTowerAttackCooldownChanged;
        public float towerMaxHealth = 20f;
        public float towerResistance = 0f;
        public float towerInvincibilityDuration = 0.5f;
        public int towerCost = 3;
    }

    public class EndStats
    {
        public float timePlayed = 0f;
        public int towersBuilt = 0;
        public int enemiesKilledByPlayer = 0;
        public int enemiesKilledByTowers = 0;
        public int TotalEnemiesKilled => enemiesKilledByPlayer + enemiesKilledByTowers;
        public int woodCollected = 0;   
        public int stoneCollected = 0;  
        public int goldCollected = 0;
        public bool win = false;

        public string GetFormattedTime()
        {
            int minutes = Mathf.FloorToInt(timePlayed / 60F);
            int seconds = Mathf.FloorToInt(timePlayed - minutes * 60);
            return string.Format("{0:00}:{1:00}", minutes, seconds);
        }
        public void ResetRun()
        {
            timePlayed = 0f;
            towersBuilt = 0;
            enemiesKilledByPlayer = 0;
            enemiesKilledByTowers = 0;
            woodCollected = 0;
            stoneCollected = 0;
            goldCollected = 0;
        }
    }
}