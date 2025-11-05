using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class HealthUI : MonoBehaviour
{
    public HealthComponent playerHealth;
    public TextMeshProUGUI healthText;
    public Image heartFillImage;

    void Update()
    {
        if (playerHealth == null) return;

        healthText.text = $"{Mathf.CeilToInt(playerHealth.currentHealth)}";
        UpdateHeart((int)playerHealth.currentHealth);
    }

    public void UpdateHeart(int health)
    {
        float fillAmount = (float)health / playerHealth.maxHealth;

        if (heartFillImage != null)
            heartFillImage.fillAmount = fillAmount; // sadece fillAmount ile çalýþýr
    }
}