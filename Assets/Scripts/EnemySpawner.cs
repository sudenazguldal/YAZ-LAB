using UnityEngine;
using System.Collections;

[System.Serializable]
public class PatrolRoute
{
    public Transform[] points;
}

public class EnemySpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    public GameObject[] enemyPrefabs;   // 🔹 Doğacak düşmanlar (Warden, Gorgon, Mortis...)
    public Transform[] spawnPoints;     // 🔹 Spawn noktaları
    public float respawnDelay = 10f;

    [Header("Patrol Routes (Her düşmana özel devriye noktaları)")]
    public PatrolRoute[] patrolRoutes;  // 🔹 Her düşmanın kendi rotası (A-B gibi)

    private GameObject[] currentEnemies;

    void Start()
    {
        HealthEnemy.OnEnemyDeath += HandleEnemyDeath;
    }

    void OnDestroy()
    {
        HealthEnemy.OnEnemyDeath -= HandleEnemyDeath;
    }

    public void SpawnAllEnemies()
    {
        if (enemyPrefabs.Length != spawnPoints.Length)
            Debug.LogWarning("Enemy prefab sayısı ile spawn point sayısı eşit değil!");

        currentEnemies = new GameObject[enemyPrefabs.Length];

        for (int i = 0; i < enemyPrefabs.Length; i++)
        {
            if (enemyPrefabs[i] == null || spawnPoints[i] == null)
                continue;

            GameObject newEnemy = Instantiate(enemyPrefabs[i], spawnPoints[i].position, spawnPoints[i].rotation);
            currentEnemies[i] = newEnemy;

            // 🔹 enemy1 scriptini bul ve rotasını ata
            enemy1 script = newEnemy.GetComponent<enemy1>();
            if (script != null && patrolRoutes != null && i < patrolRoutes.Length)
            {
                if (patrolRoutes[i].points != null && patrolRoutes[i].points.Length > 0)
                    script.patrolPoints = patrolRoutes[i].points;
            }
        }
    }

    private void HandleEnemyDeath(HealthEnemy deadEnemy)
    {
        for (int i = 0; i < currentEnemies.Length; i++)
        {
            if (currentEnemies[i] == deadEnemy.gameObject)
            {
                StartCoroutine(RespawnEnemy(i));
                break;
            }
        }
    }

    private IEnumerator RespawnEnemy(int index)
    {
        yield return new WaitForSeconds(respawnDelay);

        Transform point = spawnPoints[index % spawnPoints.Length];
        GameObject newEnemy = Instantiate(enemyPrefabs[index], point.position, point.rotation);
        currentEnemies[index] = newEnemy;

        // Yeni doğan düşmana aynı patrol noktalarını ata
        enemy1 script = newEnemy.GetComponent<enemy1>();
        if (script != null && patrolRoutes != null && index < patrolRoutes.Length)
        {
            if (patrolRoutes[index].points != null && patrolRoutes[index].points.Length > 0)
                script.patrolPoints = patrolRoutes[index].points;
        }
    }
}