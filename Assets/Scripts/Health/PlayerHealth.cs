using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public float maxHealth = 100f;
    public float currentHealth;

    public TextMeshProUGUI healthText; 

    private void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthUI();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
            TakeDamage(10f);  // hasar 10
        if (Input.GetKeyDown(KeyCode.J))
            Heal(25f);        // iyileþme 25
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);
        Debug.Log($"Took damage: -{damage} | Current Health: {currentHealth}");
        FindObjectOfType<HealthUI>().UpdateHeart((int)currentHealth);
        UpdateHealthUI();

        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    public void Heal(float healAmount)
    {
        currentHealth += healAmount;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);
        Debug.Log($"Healed: +{healAmount} | Current Health: {currentHealth}");
        FindObjectOfType<HealthUI>().UpdateHeart((int)currentHealth);
        UpdateHealthUI();
    }

    void UpdateHealthUI()
    {
        if (healthText != null)
            healthText.text = Mathf.CeilToInt(currentHealth).ToString();  // sayýyý yuvarlayarak göster
    }

    void Die()
    {
        Debug.Log("Player died!");
        // Ölüm animasyonu, ekran, sahne reset gibi iþlemler
    }
}
