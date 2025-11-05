using UnityEngine;

public class HealthComponent : MonoBehaviour
{
    //  GÜNCELLENDÝ: Max Health'i ayarlayabilmek için SerializeField ekleyelim.
    [SerializeField] private float maxHealth = 100f;
    private float currentHealth; // Canýn deðerini bu alanda tutalým.

    // ----------------------------------------------------------------
    //  EKLEME 1: Envanterin okumasý için Gerekli Public Property'ler (CS1061 Fix)
    // -----------------------------------------------------------------
    public float MaxHealth => maxHealth;      // InventoryCollector buradan okuyacak
    public float CurrentHealth => currentHealth; // InventoryCollector buradan okuyacak
    // -----------------------------------------------------------------

    void Awake()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        currentHealth = Mathf.Max(currentHealth, 0); // Canýn sýfýrýn altýna düþmesini engelle
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    // -----------------------------------------------------------------
    //  EKLEME 2: Can Kiti Kullaným Metodu (CS1061 Fix)
    // -----------------------------------------------------------------
    public void Heal(float amount)
    {
        // Caný iyileþtir ve MaxHealth'i aþmasýný engelle
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
        Debug.Log($"Can iyileþtirildi. Yeni Can: {currentHealth}");
        // Buraya ilerde can çubuðu güncelleme mantýðý eklenebilir.
    }
    // -----------------------------------------------------------------

    private void Die()
    {
        
        Destroy(gameObject);
    }
}