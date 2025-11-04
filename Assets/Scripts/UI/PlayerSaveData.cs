using Esper.ESave;
using System.IO;
using UnityEngine;
using static Esper.ESave.SaveFileSetupData;
using Esper.ESave.Encryption;   


public class PlayerSaveData : MonoBehaviour
{
    [Header("Default Spawn")]
    public Transform startPoint;

    public PlayerHealth playerHealth;
    public AmmoDisplay ammoDisplay;
    private SaveFile saveFile;

    void Start()
    {
        // SaveStorage hazır mı? (Aynı kalır)
        if (SaveStorage.instance == null)
        {
            Debug.LogError("SaveStorage sahnede bulunamadı!");
            return;
        }

        // 🎯 1. ADIM: SaveFile objesinin bellekte (storage'da) olduğundan emin ol
        // Bu, New Game sonrası ilk save'in çalışması için kritiktir.
        EnsureSaveFileExists();

        // MainSave’i çek
        saveFile = SaveStorage.instance.GetSaveByFileName("MainSave");

        // Eğer SaveFile hala null ise, BindSaveFileNextFrame'i çağır.
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
            Debug.LogWarning("MainSave hala bulunamadı. SaveFile objesinde 'Add to Storage' açık mı?");
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

        if (ammoDisplay != null)
        {
            saveFile.AddOrUpdateData("CurrentAmmo", ammoDisplay.currentAmmo);
            saveFile.AddOrUpdateData("Magazines", ammoDisplay.magazines);
        }

        saveFile.AddOrUpdateData("SceneName", UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);

        saveFile.Save(true); // diske yaz
        Debug.Log("Saved → " + Application.persistentDataPath + "/MainSave.json");
    }

    // ============================================================
    // -------------------------- LOAD -----------------------------
    // ============================================================
    public void LoadGame()
    {
        // Dosya yolunu kontrol etmek için kullandığınız yol.
        string savePath = Path.Combine(Application.persistentDataPath, "YAZ-LAB", "MainSave.json");

        Debug.Log(" Dosya yolu kontrol ediliyor: " + savePath);

        // 1️⃣ SaveFile objesinin bellekte var olduğundan emin ol (New Game veya ilk başlangıç için)
        EnsureSaveFileExists();

        // 2️⃣ SaveFile'ı yeniden bağla (önceki instance null olabilir)
        if (saveFile == null)
        {
            Debug.Log(" SaveFile null, disktekini yeniden yüklüyorum...");

            var setupData = new SaveFileSetupData
            {
                fileName = "MainSave",
                saveLocation = SaveLocation.DataPath,
                filePath = "YAZ-LAB/MainSave", // 🎯 KRİTİK DÜZELTME: Dosya kontrolü ile eşleşmeli
                fileType = FileType.Json,
                encryptionMethod = EncryptionMethod.None,
                addToStorage = true
            };

            saveFile = new SaveFile(setupData, true);

            if (!SaveStorage.instance.ContainsKey("MainSave"))
                SaveStorage.instance.AddSave(saveFile);
        }

        // 3️⃣ Verileri gerçekten disktekinden oku (Dosya var olduğu için)
        saveFile.Load();

        // 4️⃣ Pozisyon, can, mermi vs uygula
        if (saveFile.HasData("PlayerX") && saveFile.HasData("PlayerY") && saveFile.HasData("PlayerZ"))
        {
            float x = saveFile.GetData<float>("PlayerX");
            float y = saveFile.GetData<float>("PlayerY");
            float z = saveFile.GetData<float>("PlayerZ");
            transform.position = new Vector3(x, y, z);
        }
        else
        {
            // Dosya var ama veri eksik/bozuk. Yine de sıfırla.
            Debug.LogWarning("Kayıt dosyası bulundu ancak pozisyon verileri eksik. Sıfırlanıyor.");
            ResetToStartPoint();
            return;
        }

        // Diğer verileri yükle (Aynı kalır)
        if (playerHealth != null && saveFile.HasData("PlayerHealth"))
            playerHealth.currentHealth = saveFile.GetData<float>("PlayerHealth");

        if (ammoDisplay != null)
        {
            if (saveFile.HasData("CurrentAmmo"))
                ammoDisplay.currentAmmo = saveFile.GetData<int>("CurrentAmmo");
            if (saveFile.HasData("Magazines"))
                ammoDisplay.magazines = saveFile.GetData<int>("Magazines");

            ammoDisplay.UpdateAmmoUI();
        }

        Debug.Log("📂 Continue ile kayıt başarıyla yüklendi!");
    }


    // Yeni eklenecek metot!
    // ============================================================
    // ----------------------- SAVEFILE OLUŞTURUCU ------------------
    // ============================================================
    private void EnsureSaveFileExists()
    {
        // SaveFile nesnesi zaten varsa yeniden oluşturmaya gerek yok
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
            filePath = "YAZ-LAB/MainSave", // 🎯 KRİTİK AYAR: Doğru klasör yolu
            fileType = FileType.Json,
            encryptionMethod = EncryptionMethod.None,
            addToStorage = true
        };

        // false ile yeni bir dosya oluşturur (diske yazmadan önce hafızada tutar)
        saveFile = new SaveFile(setupData, false);

        if (!SaveStorage.instance.ContainsKey("MainSave"))
            SaveStorage.instance.AddSave(saveFile);

        Debug.Log("✅ SaveFile objesi hafızada oluşturuldu ve SaveStorage'a eklendi.");
    }
    // ============================================================
    // ----------------------- UTILITIES ---------------------------
    // ============================================================
    private void ResetToStartPoint()
    {
        if (startPoint != null)
        {
            transform.position = startPoint.position;
            Debug.Log("↩Oyuncu başlangıç noktasına döndü.");
        }

        if (playerHealth != null)
            playerHealth.currentHealth = playerHealth.maxHealth;

        if (ammoDisplay != null)
        {
            ammoDisplay.currentAmmo = ammoDisplay.maxAmmo;
            ammoDisplay.magazines = 3;
            ammoDisplay.UpdateAmmoUI();
        }
    }
}
