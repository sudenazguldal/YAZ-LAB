using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "NewInventory", menuName = "Game Data/Inventory Data")]
public class InventoryData : ScriptableObject
{
    // Verileri Event'ler ile sarmalýyoruz ki, veriler deðiþtiðinde UI veya diðer scriptler anýnda haber alabilsin.

    // Mühimmat
    [SerializeField] private int ammoCount = 24;
    public event UnityAction<int> OnAmmoChange;

    // Can Kiti
    [SerializeField] private int healthKits = 0;
    public event UnityAction<int> OnHealthKitChange;

    // Anahtar
    [SerializeField] private bool hasKey = false;
    public event UnityAction OnKeyFound;


    public event UnityAction<bool> OnKeyChange;


    // --- Metotlar ---

    public void AddAmmo(int amount)
    {
        ammoCount += amount;
        OnAmmoChange?.Invoke(ammoCount); // UI'ý güncelle
    }

    public void AddHealthKit()
    {
        healthKits++;
        OnHealthKitChange?.Invoke(healthKits);
    }

    public void SetKey(bool value)
    {
        hasKey = value;
        // Event'i tetikle ve yeni durumu gönder
        OnKeyChange?.Invoke(hasKey);
    }
    public bool RemoveAmmo(int amount)
    {
        if (ammoCount >= amount)
        {
            ammoCount -= amount;
            OnAmmoChange?.Invoke(ammoCount); // UI ve diðer dinleyicileri güncelle
            return true; // Harcama baþarýlý
        }
        return false; // Yeterli mermi yok (Ateþleme engellenmeli)
    }

    // Oyun yeniden baþladýðýnda kolayca sýfýrlama metodu
    public void ResetInventory()
    {
        ammoCount = 24;
        healthKits = 0;
        hasKey = false;
        // Tüm UI'larý sýfýrlamak için event'leri tekrar tetikleyin
        OnAmmoChange?.Invoke(ammoCount);
        OnHealthKitChange?.Invoke(healthKits);
    }

    
    public void RemoveHealthKit(int amount)
    {
        healthKits = Mathf.Max(0, healthKits - amount);
        OnHealthKitChange?.Invoke(healthKits); // UI'ý güncelle
    }

    // Diðer scriptlerin okumasý için getter'lar
    public int Ammo => ammoCount;
    public int HealthKits => healthKits;
    public bool HasKey => hasKey;
}