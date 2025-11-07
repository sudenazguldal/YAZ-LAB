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
            HealthComponent playerHealth = other.GetComponent<HealthComponent>();

            if (playerHealth != null)
            {
               
                playerHealth.TakeDamage(damage);
            }
        }

        Destroy(gameObject);
    }
}

