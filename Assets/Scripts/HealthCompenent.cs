using UnityEngine;
using TMPro; // UI için
using UnityEngine.UI; // Gerekli olabilir
using System.Collections;
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

    private string IsDieTrigger = "isdie";
    public GameManager gameManager;
    [SerializeField]
    private Animator animator;

    void Awake()
    {
        currentHealth = maxHealth;
    }

    // PlayerHealth.cs'ten ta??nd?
    void Start()
    {
        if (animator == null)
            animator = GetComponentInChildren<Animator>();
        if (gameManager == null)
            gameManager = FindObjectOfType<GameManager>();
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