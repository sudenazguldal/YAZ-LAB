using UnityEngine;
using TMPro;
using UnityEngine.UI; 
using System.Collections;

public class HealthComponent : MonoBehaviour
{
    [Header("Data and UI")]
    
    [SerializeField] public float maxHealth = 100f;
    public float currentHealth;

    
    public TextMeshProUGUI healthText;

    [Header("Damage Feedback")]
    [SerializeField] private Image damageVignetteImage; 
    [SerializeField] private float flashDuration = 2.5f; 
    
  
    [SerializeField] private float maxAlpha = 1f;     // max şeffaflık


    [SerializeField] private Color damageColor = Color.red;

    private string IsDieTrigger = "isdie";
    public GameManager gameManager;
    [SerializeField]
    private Animator animator;

    
    public float MaxHealth => maxHealth;
    public float CurrentHealth => currentHealth;
    

    void Awake()
    {
        currentHealth = maxHealth;
    }

    
    void Start()
    {
        if (animator == null)
            animator = GetComponentInChildren<Animator>();
        if (gameManager == null)
            gameManager = FindObjectOfType<GameManager>();
        UpdateHealthUI();
    }

   
    void Update()
    {
    }

    

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

        //  damage fx inin başlatılması -bir anda-
        Color flashColor = damageColor;
        flashColor.a = targetAlpha;
        damageVignetteImage.color = flashColor;

        // fade out kısmı daha yavaş olacak şekilde ayarladık daha sinematik olsun diye
        float timer = 0f;
        
        while (timer < flashDuration)
        {
            timer += Time.deltaTime;

            // Alpha değerini targetAlpha'dan maxa 0'a doğru yavaşça Lerple
            // Timer'ı toplam süreye oranlayarak yavaşlatır
            float currentAlpha = Mathf.Lerp(targetAlpha, 0f, timer / flashDuration);

            // Alpha'yı uygular
            flashColor.a = currentAlpha;
            damageVignetteImage.color = flashColor;

            yield return null;
        }

        // Bitiş tamamen şeffaf yapar
        flashColor.a = 0f;
        damageVignetteImage.color = flashColor;
    }

    public void Heal(float amount)
    {
        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
        

        UpdateHealthUI(); // UI'ı güncellee
        FindAnyObjectByType<HealthUI>().UpdateHeart((int)currentHealth); // Dış UI'ı güncellee
    }

    void UpdateHealthUI()
    {
        if (healthText != null)
            healthText.text = Mathf.CeilToInt(currentHealth).ToString();
    }

    public void Die()
    {
        

        //  Ölüm animasyonunu tetikler
        if (animator != null)
            animator.SetTrigger(IsDieTrigger);

        // Ölüm animasyonu bittikten sonra Lose ekranını çağırır
        StartCoroutine(DeathSequence());
    }

    private IEnumerator DeathSequence()
    {
        // Ölüm animasyonu süresi kadar bekletir yoksa ölemiyo karakter
        yield return new WaitForSeconds(3.5f);

        // GameManager'ı bul
        if (gameManager == null)
            gameManager = FindObjectOfType<GameManager>();

        // Lose paneli göster
        if (gameManager != null)
            gameManager.PlayerLose();
        
            
    }
}