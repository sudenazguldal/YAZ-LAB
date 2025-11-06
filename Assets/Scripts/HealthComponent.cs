

using UnityEngine;
using TMPro; // UI için
using UnityEngine.UI; // Gerekli olabilir

public class HealthComponent : MonoBehaviour
{
    // --- VERÝ VE UI ---
    [SerializeField] public float maxHealth = 100f;
    public float currentHealth;

    // UI Referansý (PlayerHealth'ten taþýndý)
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

    // PlayerHealth.cs'ten taþýndý
    void Start()
    {
        UpdateHealthUI();
    }

    // PlayerHealth.cs'ten taþýndý (Test Input'larý)
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
        Debug.Log($"Hasar Aldý: -{amount} | Mevcut Can: {currentHealth}");

        UpdateHealthUI(); //  UI'ý güncelle
        FindAnyObjectByType<HealthUI>().UpdateHeart((int)currentHealth); //  Dýþ UI'ý güncelle (Varsayýlan olarak)

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Heal(float amount)
    {
        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
        Debug.Log($"Ýyileþti: +{amount} | Mevcut Can: {currentHealth}");

        UpdateHealthUI(); // UI'ý güncelle
        FindAnyObjectByType<HealthUI>().UpdateHeart((int)currentHealth); // Dýþ UI'ý güncelle
    }

    void UpdateHealthUI()
    {
        if (healthText != null)
            healthText.text = Mathf.CeilToInt(currentHealth).ToString();
    }

    private void Die()
    {
        Debug.Log("Player öldü!");
        // ... (Ölüm mantýðý)
        // Destroy(gameObject); // Eðer player ölünce yok edilecekse
    }
}