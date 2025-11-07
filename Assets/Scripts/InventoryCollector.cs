using UnityEngine;


public class InventoryCollector : MonoBehaviour
{
   

    [Header("Data & References")]
   
    [SerializeField] private InventoryData inventoryData; 

   
    [SerializeField] private PlayerShooting playerShooting;
    [SerializeField] private HealthComponent playerHealth; 

    [Header("Controls")]

    [SerializeField] private KeyCode useHealthKitKey = KeyCode.H;

    [Header("Item Settings")]
    [SerializeField] private float healAmount = 25f; 

    [Header("UI Feedback")]
    [SerializeField] private UIManager uiManager;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource; 
    [SerializeField] private AudioClip ammoPickupSound; 
    [SerializeField] private AudioClip healthKitPickupSound; 
    [SerializeField] private AudioClip keyPickupSound; 
    [SerializeField] private AudioClip healUseSound;

   

    void Awake()
    {
        if (inventoryData != null)
        {
            // Her oyun başlangıcında veriyi sıfırlar ve başlatır
            inventoryData.ResetInventory();
        }
    }

    void OnEnable()
    {
        if (inventoryData != null)
        {
            // Event'i dinlemeye başlar
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
        //  Bu metot UI'daki metin alanını veya slotu güncelleyecektir.
        Debug.Log($"UI Güncellendi: Kalan Can Kiti: {newCount}");

        
    }


    void Update()
    {
        
        if (Input.GetKeyDown(useHealthKitKey))
        {
            UseHealthKit();
        }
    }




   

    private void PlayPickupSound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }

    public void CollectAmmo(int amount)
    {
        if (inventoryData != null)
        {
            inventoryData.AddAmmo(amount);
            PlayPickupSound(ammoPickupSound);

            if (uiManager != null)
            {
                string message = $"+{amount} Kurşun Alındı ";
                uiManager.ShowNotification(message, Color.red);
            }
        }
    }

    public void CollectHealthKit()
    {
        if (inventoryData != null)
        {
            inventoryData.AddHealthKit();
            PlayPickupSound(healthKitPickupSound);

            if (uiManager != null)
            {
               
                string message = $"+1 health Kit Alındı ";
                uiManager.ShowNotification(message, Color.yellow);
            }
        }
    }



    public void CollectKey()
    {
        if (inventoryData != null)
        {
            inventoryData.SetKey(true);
            inventoryData.SetKey(true);

            if (uiManager != null)
            {
                string message = $"Anahtar Alındı";
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

   

    public void UseHealthKit()
    {
        
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

        
        inventoryData.RemoveHealthKit(1);

        // Canı İyileştir
        playerHealth.Heal(healAmount);
        PlayPickupSound(healUseSound);

        
    }
}