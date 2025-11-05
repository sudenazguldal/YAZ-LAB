using UnityEngine;

public class BattleDamage : MonoBehaviour
{
    public float damage = 20f;
    public float lifetime = 3f;

    private void Start()
    {
        Destroy(gameObject, lifetime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            HealthComponent playerHealth = other.GetComponent<HealthComponent>();

            if (playerHealth != null)
            {
                Debug.Log($"Kemik, {other.name} (Oyuncu)'ya çarptý ve {damage} hasar verdi!");
                playerHealth.TakeDamage(damage);
            }
        }

        Destroy(gameObject);
    }
}

