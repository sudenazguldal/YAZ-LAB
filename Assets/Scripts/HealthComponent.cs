// Örnek bir Zombi Saðlýk Komponenti
using UnityEngine;

public class HealthComponent : MonoBehaviour
{
    public float health = 100f;

    public void TakeDamage(float amount)
    {
        health -= amount;
        if (health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        // Zombi ölme mantýðý buraya gelir
        Destroy(gameObject);
    }
}