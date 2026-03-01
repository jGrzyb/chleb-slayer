using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Level : MonoBehaviour
{
    [SerializeField] private WaveData[] waveDataList;
    [SerializeField] private float winDelay = 3f;
    public event Action<int> OnWaveChanged = delegate { };
    public int WaveCount => waveDataList.Length;
    private int _waveIndex = 0;
    private bool _isLastWaveSpawningFinished = false;
    public int CurrentWave
    {
        get => _waveIndex;
        private set
        {
            _waveIndex = value;
            OnWaveChanged.Invoke(_waveIndex);
        }
    }

    void Start()
    {
        EnemyBehaviour.OnEnemiesListChanged += CheckWinCondition;
        EnemySpawner[] spawners = FindObjectsByType<EnemySpawner>(FindObjectsSortMode.None);
        foreach (WaveData wave in waveDataList)
        {
            for (int i = 0; i < 4; i++)
            {
                wave.spawners[i].spawner = spawners[i];
            }
        }
            StartCoroutine(LevelCoroutine());
    }

    private IEnumerator LevelCoroutine()
    {
        for (int i = 0; i < waveDataList.Length; i++)
        {
            WaveData waveData = waveDataList[i];
            yield return new WaitForSeconds(waveData.waveInterval);
            CurrentWave = i;
            foreach (var spawnerData in waveData.spawners)
            {
                spawnerData.spawner.StartSpawner(spawnerData.enemyCount, spawnerData.spawnInterval);
            }

            if (i == waveDataList.Length - 1)
            {
                float longestSpawnTime = 0f;
                foreach (var spawnerData in waveData.spawners)
                {
                    float timeNeeded = spawnerData.enemyCount * spawnerData.spawnInterval;
                    if (timeNeeded > longestSpawnTime)
                    {
                        longestSpawnTime = timeNeeded;
                    }
                }

                yield return new WaitForSeconds(longestSpawnTime);

                _isLastWaveSpawningFinished = true;

                CheckWinCondition(EnemyBehaviour.AllEnemies.Count);
            }
        }
    }

    private void CheckWinCondition(int currentEnemiesCount)
    {
        if (CurrentWave == waveDataList.Length - 1 && _isLastWaveSpawningFinished && currentEnemiesCount <= 0)
            StartCoroutine(WinSequence());
    }

    private IEnumerator WinSequence()
    {
        EnemyBehaviour.OnEnemiesListChanged -= CheckWinCondition;

        Player player = FindFirstObjectByType<Player>();
        if (player != null) player.Freeze(winDelay);

        yield return new WaitForSeconds(winDelay);

        if (player != null) player.Unfreeze();

        GameManager.I.endStats.win = true;
        SceneManager.LoadScene("StatiscticsScne");
    }
    private void OnDestroy()
    {
        EnemyBehaviour.OnEnemiesListChanged -= CheckWinCondition;
    }

    [Serializable]
    public class WaveData
    {
        public float waveInterval;
        public SpawnerData[] spawners;

        public int totalEnemies => spawners.Aggregate(0, (sum, spawner) => sum + spawner.enemyCount);
    }

    [Serializable]
    public class SpawnerData
    {
        public EnemySpawner spawner;
        public int enemyCount;
        public float spawnInterval;
    }
}
