using UnityEngine;

public class WeaponAmmo : MonoBehaviour
{
    [Header("Ammo")]
    [SerializeField] private int ClipSize = 6;
    [SerializeField] public int CurrentAmmo = 6;

    [Header("Inventory")]
    [SerializeField] public InventoryData playerInventoryData;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip ReloadSound;

    public int Clip => ClipSize;          
    public int Current => CurrentAmmo;   

    private bool loadedFromSave = false;

    void Start()
    {
        /// ilk defa oyun başladıysa
        if (!loadedFromSave)
        {
            CurrentAmmo = ClipSize;
            
        }
       
    }
    public void MarkAsLoadedFromSave(int savedAmmo)
    {
        CurrentAmmo = savedAmmo;
        loadedFromSave = true;
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
            Reload();
    }

    public void Reload()
    {
        if (playerInventoryData == null) return;

        int ammoNeeded = ClipSize - CurrentAmmo;
        int ammoAvailable = playerInventoryData.Ammo;
        if (ammoNeeded <= 0 || ammoAvailable <= 0)
            return;

        int ammoToTake = Mathf.Min(ammoNeeded, ammoAvailable);
        if (ammoToTake > 0)
        {
            playerInventoryData.RemoveAmmo(ammoToTake);
            CurrentAmmo += ammoToTake;

            if (audioSource != null && ReloadSound != null)
                audioSource.PlayOneShot(ReloadSound);
        }
    }
}
