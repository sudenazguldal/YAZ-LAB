// InventoryHUDDisplay.cs (Sadece Can Kiti Sayacý için)

using UnityEngine;
using TMPro;

public class InventoryHUDDisplay : MonoBehaviour
{
    //  Health Kit Sayacý için Kalýcý Referans
    [SerializeField] private InventoryData inventoryData;
    private TextMeshProUGUI textComponent; // Bu script'in takýlý olduðu Text bileþeni

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
            // KRÝTÝK ABONELÝK
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
            //  KALICI GÖSTERÝM: Yeni sayý 0 bile olsa gösterimde kalýr.
            textComponent.text = $"Health Kit = {newCount}";

            // Eðer isterseniz, 0 olunca gizleyebilirsiniz
          //  textComponent.gameObject.SetActive(newCount > 0);
        }
    }
}