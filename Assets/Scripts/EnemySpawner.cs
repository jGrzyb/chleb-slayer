using UnityEngine;
using System.Collections;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private float minTime;
    [SerializeField] private float maxTime;
    public bool isSpawning = true;

    private void Start()
    {
        StartCoroutine(SpawnRoutine());
    }

    private IEnumerator SpawnRoutine()
    {
        while (true)
        {
            if (!isSpawning)
            {
                yield return null;
                continue;
            }

            float delay = Random.Range(minTime, maxTime);
            yield return new WaitForSeconds(delay);

            if (isSpawning)
            {
                Spawn();
            }
        }
    }

    private void Spawn()
    {
        Instantiate(enemyPrefab, transform.position, Quaternion.identity);
    }


    public void StartSpawner() => isSpawning = true;
    public void StopSpawner() => isSpawning = false;
}
