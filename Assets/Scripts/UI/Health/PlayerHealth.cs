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
        
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);
        
        FindObjectOfType<HealthUI>().UpdateHeart((int)currentHealth);
        UpdateHealthUI();

        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    [System.Obsolete]
    public void Heal(float healAmount)
    {
        currentHealth += healAmount;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);
        
        FindObjectOfType<HealthUI>().UpdateHeart((int)currentHealth);
        UpdateHealthUI();
    }

    void UpdateHealthUI()
    {
        if (healthText != null)
            healthText.text = Mathf.CeilToInt(currentHealth).ToString();  // sayýyý yuvarlayarak gösterir
    }

    void Die()
    {
        Debug.Log("Player died!");
        // Ölüm animasyonu, ekran, sahne reset gibi vs
    }
}
