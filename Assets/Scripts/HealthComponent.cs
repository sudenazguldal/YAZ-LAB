using UnityEngine;
using TMPro;
using UnityEngine.UI; 
using System.Collections;

public class HealthComponent : MonoBehaviour
{
    [Header("VERİ VE UI")]
    
    [SerializeField] public float maxHealth = 100f;
    public float currentHealth;

    // UI Referansı (PlayerHealth'ten taşındı)
    public TextMeshProUGUI healthText;

    [Header("Damage Feedback")]
    [SerializeField] private Image damageVignetteImage; //  Yeni: Ekranı kırmızı yapan UI resmi
    [SerializeField] private float flashDuration = 2.5f; // Kırmızı görüntünün ne kadar süreceği
    
  
    [SerializeField] private float maxAlpha = 1f;     // Maksimum şeffaflık (Opacity)

    
    [SerializeField] private Color damageColor = Color.red;

    private string IsDieTrigger = "isdie";
    public GameManager gameManager;
    [SerializeField]
    private Animator animator;

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

    // PlayerHealth.cs'ten taşındı
    void Start()
    {
        if (animator == null)
            animator = GetComponentInChildren<Animator>();
        if (gameManager == null)
            gameManager = FindObjectOfType<GameManager>();
        UpdateHealthUI();
    }

    // PlayerHealth.cs'ten taşındı (Test Input'ları)
    void Update()
    {
    }

    // -----------------------------------------------------------------
    // TEMEL METOTLAR (UI Güncellemelerini ekledik)
    // -----------------------------------------------------------------

    public void TakeDamage(float amount)
    {
        if (currentHealth <= 0)
            return; // Zaten ölü ise hasar alma
        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        if (damageVignetteImage != null)
        {
            StopAllCoroutines();
            StartCoroutine(FlashDamageVignette(amount));
        }
        Debug.Log($"Hasar Aldı: -{amount} | Mevcut Can: {currentHealth}");

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

        //  FADE-IN KISMI: Alpha değerini anında maksimuma set et
        Color flashColor = damageColor;
        flashColor.a = targetAlpha;
        damageVignetteImage.color = flashColor;

        // ----------------------------------------------------
        // YAVAŞ FADE OUT BAŞLANGICI
        // ----------------------------------------------------

        float timer = 0f;
        // flashDuration artık SADECE sönme süresini temsil eder (1.5 saniye)
        while (timer < flashDuration)
        {
            timer += Time.deltaTime;

            // Alpha değerini targetAlpha'dan (maksimum) 0'a doğru yavaşça Lerp et.
            // Timer'ı toplam süreye oranlayarak yavaşlatırız.
            float currentAlpha = Mathf.Lerp(targetAlpha, 0f, timer / flashDuration);

            // Alpha'yı uygula
            flashColor.a = currentAlpha;
            damageVignetteImage.color = flashColor;

            yield return null;
        }

        // 3. Bitir: Tamamen şeffaf yap
        flashColor.a = 0f;
        damageVignetteImage.color = flashColor;
    }

    public void Heal(float amount)
    {
        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
        Debug.Log($"İyileşti: +{amount} | Mevcut Can: {currentHealth}");

        UpdateHealthUI(); // UI'ı güncelle
        FindAnyObjectByType<HealthUI>().UpdateHeart((int)currentHealth); // Dış UI'ı güncelle
    }

    void UpdateHealthUI()
    {
        if (healthText != null)
            healthText.text = Mathf.CeilToInt(currentHealth).ToString();
    }

    public void Die()
    {
        Debug.Log("Player öldü!");

        // 1️⃣ Ölüm animasyonunu tetikle
        if (animator != null)
            animator.SetTrigger(IsDieTrigger);

        // 2️⃣ Ölüm animasyonu bittikten sonra Lose çağır
        StartCoroutine(DeathSequence());
    }

    private IEnumerator DeathSequence()
    {
        // Ölüm animasyonu süresi kadar bekle (örneğin 2.5 saniye)
        yield return new WaitForSeconds(3.5f);

        // GameManager'ı bul
        if (gameManager == null)
            gameManager = FindObjectOfType<GameManager>();

        // Lose paneli göster
        if (gameManager != null)
            gameManager.PlayerLose();
        else
            Debug.LogError(" GameManager sahnede bulunamadı!");
    }
}