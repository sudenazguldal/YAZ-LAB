using UnityEngine;

public class WeaponAmmo : MonoBehaviour
{
    [Header("Ammo ")]
    [SerializeField] private int ClipSize;
    
    [SerializeField] public int CurrentAmmo;

    [Header("Management")]
    [SerializeField] private InventoryData playerInventoryData;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource; // Atış sesini çalacak AudioSource bileşeni
    [SerializeField] private AudioClip ReloadSound;
    bool reloadOccurred;
    void Start()
    {
        CurrentAmmo = ClipSize;
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.R)) Reload();
    }
    public void Reload()
    {
        if (playerInventoryData == null) return;

        // Şu anki klipte kaç mermilik boşluk var?
        int ammoNeeded = ClipSize - CurrentAmmo;

        // SO'da ne kadar mermi var?
        int ammoAvailable = playerInventoryData.Ammo;

        // Yükleme olup olmadığını kontrol etmek için bayrak (flag)
        bool reloadOccurred = false; // Tanımlı değilse, bunu metodun başında tanımlayın.

        // 1. Yükleme gereksizse veya mermi yoksa çık
        if (ammoNeeded <= 0 || ammoAvailable <= 0)
        {
            return;
        }

        // 2. Envanterden alınacak maksimum mühimmat miktarını belirle
        int ammoToTake = Mathf.Min(ammoNeeded, ammoAvailable);

        // 3. Mühimmat çekiliyorsa (ammoToTake > 0)
        if (ammoToTake > 0)
        {
            // 🔹 4. KRİTİK DÜZELTME: Mermiyi SADECE BİR KEZ ÇIKAR!
            playerInventoryData.RemoveAmmo(ammoToTake);

            // 5. Silahın şarjörüne ekle
            CurrentAmmo += ammoToTake;

            // Yükleme gerçekleşti bayrağını ayarla
            reloadOccurred = true;
        }

        // 6. Ses çalma mantığı
        if (reloadOccurred)
        {
            if (audioSource != null && ReloadSound != null)
            {
                audioSource.PlayOneShot(ReloadSound);
            }
        }
        // NOT: Eğer reloadOccurred metot içinde tanımlı değilse, public bool reloadOccurred = false;
        // şeklinde metodun başında tanımladığınızdan emin olun.
    }


}
