using Esper.ESave;
using System.IO;
using UnityEngine;
using static Esper.ESave.SaveFileSetupData;
using Esper.ESave.Encryption;

public class PlayerSaveData : MonoBehaviour
{
    [Header("Default Spawn")]
    public Transform startPoint;

    public HealthComponent playerHealth;
    public WeaponAmmo weaponAmmo;              // 🔹 Silah referansı
    public RevolverAmmoDisplay ammoDisplay;    // 🔹 UI referansı
    private SaveFile saveFile;

    void Start()
    {
        if (SaveStorage.instance == null)
        {
            Debug.LogError("SaveStorage sahnede bulunamadı!");
            return;
        }

        EnsureSaveFileExists();

        saveFile = SaveStorage.instance.GetSaveByFileName("MainSave");

        if (saveFile == null)
            StartCoroutine(BindSaveFileNextFrame());
        else
            LoadGame();
    }

    System.Collections.IEnumerator BindSaveFileNextFrame()
    {
        yield return null;
        saveFile = SaveStorage.instance.GetSaveByFileName("MainSave");
        if (saveFile == null)
            Debug.LogWarning("MainSave hala bulunamadı.");
        else
            LoadGame();
    }

    // ============================================================
    // -------------------------- SAVE -----------------------------
    // ============================================================
    public void SaveGame()
    {
        if (saveFile == null)
        {
            Debug.LogError("SaveGame: saveFile null!");
            return;
        }

        Vector3 pos = transform.position;

        saveFile.AddOrUpdateData("PlayerX", pos.x);
        saveFile.AddOrUpdateData("PlayerY", pos.y);
        saveFile.AddOrUpdateData("PlayerZ", pos.z);

        if (playerHealth != null)
            saveFile.AddOrUpdateData("PlayerHealth", playerHealth.currentHealth);

        // --- WEAPON DATA ---
        if (weaponAmmo != null)
        {
            saveFile.AddOrUpdateData("CurrentAmmo", weaponAmmo.Current);
            saveFile.AddOrUpdateData("ClipSize", weaponAmmo.Clip);

            // --- INVENTORY DATA ---
            if (weaponAmmo.playerInventoryData != null)
            {
                var inv = weaponAmmo.playerInventoryData;
                saveFile.AddOrUpdateData("InventoryAmmo", inv.Ammo);
                saveFile.AddOrUpdateData("InventoryHealthKits", inv.HealthKits);
                saveFile.AddOrUpdateData("InventoryHasKey", inv.HasKey);
            }
        }

        saveFile.AddOrUpdateData("SceneName", UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
        saveFile.Save(true);

        Debug.Log("💾 Saved → " + Application.persistentDataPath + "/YAZ-LAB/MainSave.json");
    }

    // ============================================================
    // -------------------------- LOAD -----------------------------
    // ============================================================
    public void LoadGame()
    {
        EnsureSaveFileExists();
        if (saveFile == null)
        {
            Debug.LogError("LoadGame: saveFile null!");
            return;
        }

        saveFile.Load();

        // --- POSITION ---
        if (saveFile.HasData("PlayerX"))
        {
            float x = saveFile.GetData<float>("PlayerX");
            float y = saveFile.GetData<float>("PlayerY");
            float z = saveFile.GetData<float>("PlayerZ");
            transform.position = new Vector3(x, y, z);
        }

        // --- HEALTH ---
        if (playerHealth != null && saveFile.HasData("PlayerHealth"))
            playerHealth.currentHealth = saveFile.GetData<float>("PlayerHealth");

        // --- WEAPON AMMO ---
        if (weaponAmmo != null)
        {
            if (saveFile.HasData("CurrentAmmo"))
                if (saveFile.HasData("CurrentAmmo"))
                {
                    int savedCurrent = saveFile.GetData<int>("CurrentAmmo");
                    weaponAmmo.MarkAsLoadedFromSave(savedCurrent);
                }

            if (saveFile.HasData("ClipSize"))
                typeof(WeaponAmmo)
                    .GetField("ClipSize", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                    ?.SetValue(weaponAmmo, saveFile.GetData<int>("ClipSize"));
        }

        // --- INVENTORY DATA ---
        if (weaponAmmo != null && weaponAmmo.playerInventoryData != null)
        {
            var inv = weaponAmmo.playerInventoryData;

            if (saveFile.HasData("InventoryAmmo"))
            {
                int savedAmmo = saveFile.GetData<int>("InventoryAmmo");
                typeof(InventoryData)
                    .GetField("ammoCount", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                    ?.SetValue(inv, savedAmmo);
            }

            if (saveFile.HasData("InventoryHealthKits"))
            {
                int savedKits = saveFile.GetData<int>("InventoryHealthKits");
                typeof(InventoryData)
                    .GetField("healthKits", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                    ?.SetValue(inv, savedKits);
            }

            if (saveFile.HasData("InventoryHasKey"))
            {
                bool savedKey = saveFile.GetData<bool>("InventoryHasKey");
                typeof(InventoryData)
                    .GetField("hasKey", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                    ?.SetValue(inv, savedKey);
            }

            // Event'leri tetikle ki UI hemen güncellensin
            inv.ForceUpdateEvents();
        }

        // --- UI UPDATE ---
        if (ammoDisplay != null && weaponAmmo != null)
        {
            ammoDisplay.currentAmmo = weaponAmmo.Current;
            ammoDisplay.maxAmmo = weaponAmmo.Clip;
            ammoDisplay.UpdateAmmoUI();
        }

        Debug.Log("✅ Kayıt başarıyla yüklendi!");
    }

    // ============================================================
    // ----------------------- SAVEFILE OLUŞTURUCU ----------------
    // ============================================================
    private void EnsureSaveFileExists()
    {
        if (saveFile != null && saveFile.fileName == "MainSave")
        {
            if (!SaveStorage.instance.ContainsKey("MainSave"))
                SaveStorage.instance.AddSave(saveFile);
            return;
        }

        var setupData = new SaveFileSetupData
        {
            fileName = "MainSave",
            saveLocation = SaveLocation.DataPath,
            filePath = "YAZ-LAB/MainSave",
            fileType = FileType.Json,
            encryptionMethod = EncryptionMethod.None,
            addToStorage = true
        };

        saveFile = new SaveFile(setupData, false);

        if (!SaveStorage.instance.ContainsKey("MainSave"))
            SaveStorage.instance.AddSave(saveFile);
    }
}
