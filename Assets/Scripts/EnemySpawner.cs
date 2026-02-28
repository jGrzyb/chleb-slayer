using UnityEngine;
using System.Collections;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private GameObject enemyPrefab;
    public int SpawnCount { get; private set; } = 0;

    public void StartSpawner(int enemyCount, float spawnInterval)
    {
        StartCoroutine(SpawnRoutine(enemyCount, spawnInterval));
    }

    private IEnumerator SpawnRoutine(int enemyCount, float spawnInterval)
    {
        for (int i = 0; i < enemyCount; i++)
        {
            Spawn();
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    private void Spawn()
    {
        SpawnCount++;
        Instantiate(enemyPrefab, transform.position, Quaternion.identity);
    }
}
