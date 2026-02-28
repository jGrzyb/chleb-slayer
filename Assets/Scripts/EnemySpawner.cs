using UnityEngine;
using System.Collections;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private GameObject enemyPrefab;

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
        Instantiate(enemyPrefab, transform.position, Quaternion.identity);
    }
}
