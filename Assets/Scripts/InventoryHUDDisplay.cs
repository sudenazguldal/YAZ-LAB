// InventoryHUDDisplay.cs
using UnityEngine;
using TMPro;

public class InventoryHUDDisplay : MonoBehaviour
{
    [SerializeField] private InventoryData inventoryData;

    //  HEALTH KIT GÖSTERÝMÝ ÝÇÝN ALAN
    [Header("Health Kit Display")]
    [SerializeField] private TextMeshProUGUI healthKitTextComponent;

    //  YENÝ ALAN: ANAHTAR GÖSTERÝMÝ ÝÇÝN
    [Header("Key Status Display")]
    [SerializeField] private TextMeshProUGUI keyStatusTextComponent;

    void Awake()
    {
        // Baþlangýç deðerlerini al
        if (inventoryData != null)
        {
            UpdateHealthKitDisplay(inventoryData.HealthKits);
            UpdateKeyDisplay(inventoryData.HasKey); // Baþlangýç Anahtar durumunu göster
        }
    }

    void OnEnable()
    {
        if (inventoryData != null)
        {
            inventoryData.OnHealthKitChange += UpdateHealthKitDisplay;
            inventoryData.OnKeyChange += UpdateKeyDisplay; //  Anahtar Event'ine abone ol
        }
    }

    void OnDisable()
    {
        if (inventoryData != null)
        {
            inventoryData.OnHealthKitChange -= UpdateHealthKitDisplay;
            inventoryData.OnKeyChange -= UpdateKeyDisplay; //  Aboneliði iptal et
        }
    }

    private void UpdateHealthKitDisplay(int newCount)
    {
        if (healthKitTextComponent != null)
        {
            healthKitTextComponent.text = $"Health Kits: {newCount}";
        }
    }

   
    private void UpdateKeyDisplay(bool hasKey)
    {
        if (keyStatusTextComponent != null)
        {
            string status = hasKey ? "VAR" : "YOK";
            keyStatusTextComponent.text = $"Key: {status}";

            // Eðer isterseniz, anahtar toplanýnca metin rengini deðiþtirebilirsiniz.
            keyStatusTextComponent.color = hasKey ? Color.yellow : Color.white;
        }
    }
}