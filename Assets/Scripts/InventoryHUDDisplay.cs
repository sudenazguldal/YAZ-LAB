// InventoryHUDDisplay.cs (Sadece Can Kiti Sayacý için)

using UnityEngine;
using TMPro;

public class InventoryHUDDisplay : MonoBehaviour
{
   // helath kit sayacý için
    [SerializeField] private InventoryData inventoryData;
    private TextMeshProUGUI textComponent; 

    void Awake()
    {
        textComponent = GetComponent<TextMeshProUGUI>();
        if (inventoryData != null && textComponent != null)
        {
            // Baþlangýçta güncel veriyi çek
            UpdateHealthKitDisplay(inventoryData.HealthKits);
        }
    }

    void OnEnable()
    {
        if (inventoryData != null)
        {
           
            inventoryData.OnHealthKitChange += UpdateHealthKitDisplay;
        }
    }

    void OnDisable()
    {
        if (inventoryData != null)
        {
            inventoryData.OnHealthKitChange -= UpdateHealthKitDisplay;
        }
    }

    private void UpdateHealthKitDisplay(int newCount)
    {
        if (textComponent != null)
        {
            // Yeni sayý 0 bile olsa gösterimde kalýr.
            textComponent.text = $"Health Kit = {newCount}";

          
        }
    }
}