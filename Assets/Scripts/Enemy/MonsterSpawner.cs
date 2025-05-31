using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterSpawner : MonoBehaviour
{
    public GameObject[] enemies;
    public int enemiesPerWave = 5;
    public int totalWaves = 3;
    public float spawnRadius = 5f;
    public float spawnDelay = 1.5f;
    public float waveDelay = 5f;
    public float spawnDelayBetweenWaves = 3f;

    public Room currentRoom;
    private Coroutine spawnCoroutine;
    private bool spawnStarted = false;
    private bool hasSpawnedAllWaves = false;
    private List<GameObject> activeEnemies = new List<GameObject>();

    private void Start()
    {
        //Collider2D col = GetComponent<Collider2D>();
        //if (currentRoom != null && currentRoom.IsStairRoom)
        if (currentRoom != null && (currentRoom.IsStairRoom || (currentRoom.IsBossRoom && enemies.Length == 0)))
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Update()
    {
        if (spawnStarted || !hasSpawnedAllWaves) return;

        if (activeEnemies.Count == 0 && currentRoom != null)
        {
            currentRoom.OpenAllDoors();
            enabled = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !spawnStarted && !hasSpawnedAllWaves)
        {
            if (currentRoom != null && currentRoom.IsStairRoom)
            {
                Debug.Log("Entered special room. No enemy spawning.");
                return;
            }

            spawnStarted = true;
            if (currentRoom != null)
            {
                currentRoom.CloseAllDoors();
            }

            spawnCoroutine = StartCoroutine(SpawnWave());
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") && spawnStarted)
        {
            spawnStarted = false;
            if (spawnCoroutine != null)
            {
                StopCoroutine(spawnCoroutine);
            }
        }
    }

    IEnumerator SpawnWave()
    {
        if (currentRoom != null && currentRoom.IsBossRoom)
        {
            Debug.Log("Spawning Boss in room " + gameObject.name);
            SpawnEnemy();
            yield break;
        }

        for (int wave = 1; wave <= totalWaves; wave++)
        {
            Debug.Log($"Spawning wave {wave} in room {gameObject.name}");

            for (int i = 0; i < enemiesPerWave; i++)
            {
                if (!spawnStarted) yield break;
                SpawnEnemy();
                yield return new WaitForSeconds(spawnDelay);
            }

            Debug.Log($"Wave {wave} completed. Waiting {waveDelay} seconds for the next wave.");
            yield return new WaitForSeconds(waveDelay);
        }

        Debug.Log("All waves cleared in room " + gameObject.name);
        hasSpawnedAllWaves = true;
        spawnStarted = false;
    }

    private void SpawnEnemy()
    {
        if (enemies.Length == 0)
        {
            Debug.LogError("No enemies assigned in MonsterSpawner!");
            return;
        }

        GameObject enemyPrefab = enemies[Random.Range(0, enemies.Length)];
        Vector2 randomPosition = Random.insideUnitCircle * spawnRadius;
        Vector3 spawnPosition = transform.position + new Vector3(randomPosition.x, randomPosition.y, 0);

        GameObject newEnemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
        activeEnemies.Add(newEnemy);

        EnemyHealth enemy = newEnemy.GetComponent<EnemyHealth>();
        if (enemy != null)
        {
            enemy.OnEnemyDied += HandleEnemyDeath;
        }

        Debug.Log($"Spawned {enemyPrefab.name} at {spawnPosition}");
    }

    private void HandleEnemyDeath(GameObject enemy)
    {
        if (activeEnemies.Contains(enemy))
        {
            activeEnemies.Remove(enemy);
        }

        if(hasSpawnedAllWaves && activeEnemies.Count == 0)
        {
            Debug.Log("All enemies defeated in room " + gameObject.name);

            currentRoom.OpenAllDoors();

            enabled = false;
        }
    }
}
