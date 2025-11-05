using UnityEngine;
// Not: ItemType enum'unun (Ammo, HealthKit, Key) tanımlı olduğunu varsayıyoruz.

public class InventoryCollector : MonoBehaviour
{
    // ----------------------------------------------------
    // ALANLAR
    // ----------------------------------------------------

    [Header("Data & References")]
    [Tooltip("Bağlanılacak ScriptableObject InventoryData dosyası.")]
    [SerializeField] private InventoryData inventoryData; // ⬅️ Inventory Data SO Referansı

    // Diğer bileşen referansları (Kullanım mantığı için gerekli)
    [SerializeField] private PlayerShooting playerShooting;
    [SerializeField] private HealthComponent playerHealth; // ⬅️ HealthComponent Referansı

    [Header("Controls")]
    [Tooltip("Can kiti kullanmak için tuş.")]
    [SerializeField] private KeyCode useHealthKitKey = KeyCode.H;

    [Header("Item Settings")]
    [SerializeField] private float healAmount = 25f; // Kullanılan kitin iyileştirme miktarı

    [Header("UI Feedback")]
    [SerializeField] private UIManager uiManager; 

    // ----------------------------------------------------
    // UNITY YAŞAM DÖNGÜSÜ
    // ----------------------------------------------------

    void Awake()
    {
        if (inventoryData != null)
        {
            // KRİTİK: Her oyun başlangıcında veriyi sıfırla ve başlat
            inventoryData.ResetInventory();
        }
    }

    void OnEnable()
    {
        if (inventoryData != null)
        {
            // Event'i dinlemeye başla
            inventoryData.OnHealthKitChange += UpdateHealthKitUI;
        }
    }

    void OnDisable()
    {
        if (inventoryData != null)
        {
            // Event'i dinlemeyi bırak
            inventoryData.OnHealthKitChange -= UpdateHealthKitUI;
        }
    }

    private void UpdateHealthKitUI(int newCount)
    {
        //  Bu metot, UI'daki metin alanını veya slotu güncelleyecektir.
        Debug.Log($"UI Güncellendi: Kalan Can Kiti: {newCount}");

        // (Örn: healthText.text = newCount.ToString();)
        // (Örn: healthKitSlot.gameObject.SetActive(newCount > 0);)
    }


    void Update()
    {
        // Can Kiti Kullanım Input'u
        if (Input.GetKeyDown(useHealthKitKey))
        {
            UseHealthKit();
        }
    }


    

    // ----------------------------------------------------
    // 1. ÖĞE TOPLAMA METOTLARI (PickupItem'ın çağırdığı metotlar)
    // ----------------------------------------------------

    

    public void CollectAmmo(int amount)
    {
        if (inventoryData != null)
        {
            inventoryData.AddAmmo(amount);

            if (uiManager != null)
            {
                string message = $"+{amount} AMMO ({inventoryData.Ammo} Total)";
                uiManager.ShowNotification(message, Color.yellow);
            }
        }
    }

    public void CollectHealthKit()
    {
        if (inventoryData != null)
        {
            inventoryData.AddHealthKit();

            if (uiManager != null)
            {
                // Mesajı InventoryData'dan okunan yeni sayı ile göster
                string message = $"+1 HEALTH KIT ({inventoryData.HealthKits} Total)";
                uiManager.ShowNotification(message, Color.green);
            }
        }
    }



    public void CollectKey()
    {
        if (inventoryData != null)
        {
            inventoryData.SetKey(true);

            if (uiManager != null)
            {
                string message = $"key picked)";
                uiManager.ShowNotification(message, Color.yellow);
            }
        }
    }

    public void CollectItem(PickupItem item)
    {
        if (inventoryData == null) return;

        switch (item.type)
        {
            case ItemType.Ammo:
                CollectAmmo(item.value);
                break;

            case ItemType.HealthKit:
                CollectHealthKit();
                break;

            case ItemType.Key:
                CollectKey();
                break;
        }
    }

    // ----------------------------------------------------
    // 2. ÖĞE KULLANMA MANTIKLARI
    // ----------------------------------------------------

    public void UseHealthKit()
    {
        // Güvenlik Kontrolleri
        if (inventoryData == null || playerHealth == null) return;

        
        if (playerHealth.CurrentHealth >= playerHealth.MaxHealth)
        {
            Debug.Log("Canın zaten dolu.");
            return;
        }

        if (inventoryData.HealthKits <= 0)
        {
            Debug.Log("Can kiti yok.");
            return;
        }

        // 1. Envanterden kiti çıkar (InventoryData'ya Remove metodu olmalı)
        inventoryData.RemoveHealthKit(1);

        // 2. Canı İyileştir
        playerHealth.Heal(healAmount); // HealthComponent'te Heal metodu olmalı

        Debug.Log($"Can kiti kullanıldı. İyileşen: {healAmount}");
    }
}