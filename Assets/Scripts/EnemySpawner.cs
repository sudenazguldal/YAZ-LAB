using UnityEngine;
using System.Collections;

public class EnemySpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    public GameObject[] enemyPrefabs;   // 🔹 Doğacak düşmanlar (Warden, Gorgon, Mortis...)
    public Transform[] spawnPoints;     // 🔹 Belirlediğin spawn noktaları (boş objeler)
    public float respawnDelay;     // Kaç saniye sonra yeniden doğacaklar

    private GameObject[] currentEnemies; // 🔹 Her düşmanı ayrı takip edeceğiz

    void Start()
    {
        HealthEnemy.OnEnemyDeath += HandleEnemyDeath;
       SpawnAllEnemies();
    }

    void OnDestroy()
    {
        HealthEnemy.OnEnemyDeath -= HandleEnemyDeath;
    }

    // 🔹 Tüm düşmanları belirlenen noktalarda oluşturur
     private void SpawnAllEnemies()
    {
        // Eğer listedeki eleman sayıları eşleşmiyorsa uyarı ver
        if (enemyPrefabs.Length != spawnPoints.Length)
        {
            Debug.LogWarning("Enemy prefab sayısı ile spawn point sayısı eşit değil!");
        }

        currentEnemies = new GameObject[enemyPrefabs.Length];

        // Her prefab için bir spawn noktası bul ve oluştur
        for (int i = 0; i < enemyPrefabs.Length; i++)
        {
            if (enemyPrefabs[i] == null)
            {
                Debug.LogError("Enemy prefab not assigned at index " + i);
                continue;
            }

            // Eğer spawnPoints kısa ise, mod (%) kullanarak döngüye sok
            Transform point = spawnPoints[i % spawnPoints.Length];

            currentEnemies[i] = Instantiate(enemyPrefabs[i], point.position, point.rotation);
        }
    }

    // 🔹 Bir düşman öldüğünde çağrılır
    private void HandleEnemyDeath(HealthEnemy deadEnemy)
    {
        // Hangi düşmanın öldüğünü bul
        for (int i = 0; i < currentEnemies.Length; i++)
        {
            if (currentEnemies[i] == deadEnemy.gameObject)
            {
                StartCoroutine(RespawnEnemy(i)); // o düşmanı yeniden doğur
                break;
            }
        }
    }

    // 🔹 Ölen düşmanı belirli süre sonra yeniden doğur
    private IEnumerator RespawnEnemy(int index)
    {
        yield return new WaitForSeconds(respawnDelay);

        Transform point = spawnPoints[index % spawnPoints.Length];
        currentEnemies[index] = Instantiate(enemyPrefabs[index], point.position, point.rotation);
    }
}
