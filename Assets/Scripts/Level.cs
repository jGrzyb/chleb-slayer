using System;
using System.Collections;
using System.Linq;
using UnityEngine;

public class Level : MonoBehaviour
{
    [SerializeField] private WaveData[] waveDataList;
    public event Action<int> OnWaveChanged = delegate { };
    public int WaveCount => waveDataList.Length;
    private int _waveIndex = 0;
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
        }
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
