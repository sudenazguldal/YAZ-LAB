using UnityEngine;
using TMPro; // UI için
using UnityEngine.UI; // Gerekli olabilir

public class HealthComponent : MonoBehaviour
{
    // --- VER? VE UI ---
    [SerializeField] public float maxHealth = 100f;
    public float currentHealth;

    // UI Referans? (PlayerHealth'ten ta??nd?)
    public TextMeshProUGUI healthText;

    // -----------------------------------------------------------------
    // EKLEME 1: Public Property'ler (InventoryCollector ve Envanter için)
    // -----------------------------------------------------------------
    public float MaxHealth => maxHealth;
    public float CurrentHealth => currentHealth;
    // -----------------------------------------------------------------

    void Awake()
    {
        currentHealth = maxHealth;
    }

    // PlayerHealth.cs'ten ta??nd?
    void Start()
    {
        UpdateHealthUI();
    }

    // PlayerHealth.cs'ten ta??nd? (Test Input'lar?)
    void Update()
    {
    }

    // -----------------------------------------------------------------
    // TEMEL METOTLAR (UI Güncellemelerini ekledik)
    // -----------------------------------------------------------------

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        Debug.Log($"Hasar Ald?: -{amount} | Mevcut Can: {currentHealth}");

        UpdateHealthUI(); //  UI'? güncelle
        FindObjectOfType<HealthUI>().UpdateHeart((int)currentHealth); //  D?? UI'? güncelle (Varsay?lan olarak)

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Heal(float amount)
    {
        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
        Debug.Log($"?yile?ti: +{amount} | Mevcut Can: {currentHealth}");

        UpdateHealthUI(); // UI'? güncelle
        FindObjectOfType<HealthUI>().UpdateHeart((int)currentHealth); // D?? UI'? güncelle
    }

    void UpdateHealthUI()
    {
        if (healthText != null)
            healthText.text = Mathf.CeilToInt(currentHealth).ToString();
    }

    public void Die()
    {
        Debug.Log("Player öldü!");
        // ... (Ölüm mant???)
        // Destroy(gameObject); // E?er player ölünce yok edilecekse
    }
}