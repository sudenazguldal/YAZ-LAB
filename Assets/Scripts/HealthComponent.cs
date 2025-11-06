using UnityEngine;
using TMPro;
using UnityEngine.UI; 
using System.Collections;

public class HealthComponent : MonoBehaviour
{
    [Header("VERÝ VE UI")]
    
    [SerializeField] public float maxHealth = 100f;
    public float currentHealth;

    // UI Referansý (PlayerHealth'ten taþýndý)
    public TextMeshProUGUI healthText;

    [Header("Damage Feedback")]
    [SerializeField] private Image damageVignetteImage; //  Yeni: Ekraný kýrmýzý yapan UI resmi
    [SerializeField] private float flashDuration = 2.5f; // Kýrmýzý görüntünün ne kadar süreceði
    
  
    [SerializeField] private float maxAlpha = 1f;     // Maksimum þeffaflýk (Opacity)

    
    [SerializeField] private Color damageColor = Color.red;

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

        if (damageVignetteImage != null)
        {
            StopAllCoroutines();
            StartCoroutine(FlashDamageVignette(amount));
        }
        Debug.Log($"Hasar Aldý: -{amount} | Mevcut Can: {currentHealth}");

        UpdateHealthUI(); 
        FindAnyObjectByType<HealthUI>().UpdateHeart((int)currentHealth); 

        if (currentHealth <= 0)
        {
            Die();
        }
    }
    private IEnumerator FlashDamageVignette(float damageTaken)
    {
        float currentMaxHealth = maxHealth;
        float normalizedDamage = damageTaken / currentMaxHealth;
        float targetAlpha = Mathf.Min(normalizedDamage * 2f, maxAlpha);

        //  FADE-IN KISMI: Alpha deðerini anýnda maksimuma set et
        Color flashColor = damageColor;
        flashColor.a = targetAlpha;
        damageVignetteImage.color = flashColor;

        // ----------------------------------------------------
        // YAVAÞ FADE OUT BAÞLANGICI
        // ----------------------------------------------------

        float timer = 0f;
        // flashDuration artýk SADECE sönme süresini temsil eder (1.5 saniye)
        while (timer < flashDuration)
        {
            timer += Time.deltaTime;

            // Alpha deðerini targetAlpha'dan (maksimum) 0'a doðru yavaþça Lerp et.
            // Timer'ý toplam süreye oranlayarak yavaþlatýrýz.
            float currentAlpha = Mathf.Lerp(targetAlpha, 0f, timer / flashDuration);

            // Alpha'yý uygula
            flashColor.a = currentAlpha;
            damageVignetteImage.color = flashColor;

            yield return null;
        }

        // 3. Bitir: Tamamen þeffaf yap
        flashColor.a = 0f;
        damageVignetteImage.color = flashColor;
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