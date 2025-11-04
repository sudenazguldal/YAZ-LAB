using UnityEngine;

public class BoneDamage : MonoBehaviour
{
    public float damage = 10f;
    public float lifetime = 3f;

    private void Start()
    {
        Destroy(gameObject, lifetime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            HealthEnemy playerHealth = other.GetComponent<HealthEnemy>();

            if (playerHealth != null)
            {
                Debug.Log($"Kemik, {other.name} (Oyuncu)'ya çarptý ve {damage} hasar verdi!");
                playerHealth.TakeDamage(damage);
            }
        }

        Destroy(gameObject);
    }
}

