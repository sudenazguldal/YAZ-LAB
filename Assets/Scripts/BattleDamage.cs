using UnityEngine;

public class BattleDamage : MonoBehaviour
{
    public float damage = 5f;
    public float lifetime = 3f;

    private void Start()
    {
        Destroy(gameObject, lifetime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();

            if (playerHealth != null)
            {
                Debug.Log($"Kemik, {other.name} (Oyuncu)'ya çarptý ve {damage} hasar verdi!");
                playerHealth.TakeDamage(damage);
            }
        }

        Destroy(gameObject);
    }
}

