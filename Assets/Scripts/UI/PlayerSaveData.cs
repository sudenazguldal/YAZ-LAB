using Esper.ESave;
using Esper.ESave.Encryption;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq; // LINQ kullanımı için eklendi
using UnityEngine;
using static Esper.ESave.SaveFileSetupData;


public class PlayerSaveData : MonoBehaviour
{
    [HideInInspector] public bool enemiesSpawned = false;


    [Header("Default Spawn")]
    public Transform startPoint;

    [Header("Scene References")]
    public List<PressKeyOpenDoor> doors; // Kapı script referansları
    public List<HealthEnemy> zombies;    // Zombi can referansları (Boss dahil)

    [Header("Optional")]
    public EnemySpawner enemySpawner;

    public HealthComponent playerHealth;
    public WeaponAmmo weaponAmmo;
    public RevolverAmmoDisplay ammoDisplay;
    private SaveFile saveFile;


    void Awake()
    {
        // ... (Awake metodu aynı kalabilir) ...
        string savePath = Path.Combine(Application.persistentDataPath, "YAZ-LAB", "MainSave.json");
        if (File.Exists(savePath))
        {
            Debug.Log("Önceki kayıt bulundu, eski save yükleniyor...");
        }
        else
        {
            Debug.Log("Kayıt bulunamadı, yeni oyun başlatılıyor...");
            enemiesSpawned = false; // sıfırdan başla
        }
    }


    void Start()
    {
        // ... (Start metodu aynı kalabilir) ...
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

        zombies.RemoveAll(z => z == null);

        // Sahnedeki tüm aktif HealthEnemy bileşenlerini bul
        var allEnemiesInScene = FindObjectsOfType<HealthEnemy>();
        foreach (var enemy in allEnemiesInScene)
        {
            // Eğer bu düşman zaten listede yoksa, onu listeye ekle
            if (!zombies.Contains(enemy))
            {
                zombies.Add(enemy);
                Debug.Log($"Kaydedilecek zombi listesine yeni eklendi: {enemy.name}");
            }
        }

        Vector3 pos = transform.position;

        saveFile.AddOrUpdateData("PlayerX", pos.x);
        saveFile.AddOrUpdateData("PlayerY", pos.y);
        saveFile.AddOrUpdateData("PlayerZ", pos.z);

        if (playerHealth != null)
            saveFile.AddOrUpdateData("PlayerHealth", playerHealth.currentHealth);

        // --- WEAPON & INVENTORY DATA ---
        if (weaponAmmo != null)
        {
            saveFile.AddOrUpdateData("CurrentAmmo", weaponAmmo.Current);
            saveFile.AddOrUpdateData("ClipSize", weaponAmmo.Clip);

            if (weaponAmmo.playerInventoryData != null)
            {
                var inv = weaponAmmo.playerInventoryData;
                saveFile.AddOrUpdateData("InventoryAmmo", inv.Ammo);
                saveFile.AddOrUpdateData("InventoryHealthKits", inv.HealthKits);
                saveFile.AddOrUpdateData("InventoryHasKey", inv.HasKey);
            }
        }

        // 🔹 KAPI DURUMLARI
        for (int i = 0; i < doors.Count; i++)
        {
            bool isOpened = false;
            Animator anim = doors[i].doorAnimator;
            if (anim != null)
            {
                AnimatorStateInfo state = anim.GetCurrentAnimatorStateInfo(0);
                // Kapının açıldığı animasyonun adını kontrol et
                isOpened = state.IsName(doors[i].animationName) || doors[i].isOpened;
            }
            saveFile.AddOrUpdateData($"Door_{i}_IsOpen", isOpened);
        }

        // 🔹 ZOMBİLER
        // Sadece hayatta olanları kaydetmek daha verimli olabilir, 
        // ancak mevcut yapıya sadık kalalım.
        for (int i = 0; i < zombies.Count; i++)
        {
            var z = zombies[i];
            if (z == null) continue;

            Vector3 zp = z.transform.position;
            saveFile.AddOrUpdateData($"Zombie_{i}_X", zp.x);
            saveFile.AddOrUpdateData($"Zombie_{i}_Y", zp.y);
            saveFile.AddOrUpdateData($"Zombie_{i}_Z", zp.z);
            saveFile.AddOrUpdateData($"Zombie_{i}_Health", GetPrivateHealth(z));
        }

        saveFile.AddOrUpdateData("EnemiesSpawned", enemiesSpawned);

        saveFile.AddOrUpdateData("SceneName", UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
        saveFile.Save(true);

        Debug.Log(" Saved → " + Application.persistentDataPath + "/YAZ-LAB/MainSave.json");
    }

    // ============================================================
    // -------------------------- LOAD -----------------------------
    // ============================================================
    public void LoadGame()
    {
        if (enemySpawner == null)
        {
            enemySpawner = FindObjectOfType<EnemySpawner>();
            Debug.LogWarning(" enemySpawner null bulundu, sahneden otomatik atandı!");
        }

        EnsureSaveFileExists();
        if (saveFile == null)
        {
            Debug.LogError("LoadGame: saveFile null!");
            return;
        }

        saveFile.Load();

        // --- ENEMIES SPAWN FLAG ---
        if (saveFile.HasData("EnemiesSpawned"))
            enemiesSpawned = saveFile.GetData<bool>("EnemiesSpawned");

        // Zombi listesini temizle
        zombies.RemoveAll(z => z == null);

        // Sahneye bak: Boss hariç düşman var mı?
        bool sceneHasNonBossEnemy = FindObjectsOfType<HealthEnemy>().Any(z => !z.gameObject.CompareTag("Boss"));

        if (enemiesSpawned && !sceneHasNonBossEnemy && enemySpawner != null)
        {
            Debug.Log("Save yükleniyor: Zombiler daha önce spawn olmuş, **spawner yeniden çağrılıyor...**");
            enemySpawner.SpawnAllEnemies();

            // Spawn işlemi Coroutine içinde gerçekleşiyorsa, yüklemeyi Coroutine bitince yapalım
            StartCoroutine(PostSpawnLoad());
            return; // LoadGame'i burada sonlandır
        }
        else
        {
            // Eğer düşmanlar zaten sahnedeyse VEYA hiç spawn edilmemişse, direk yükle
            ApplyLoadedData();
        }

        Debug.Log("Kayıt başarıyla yüklendi!");
    }

    // ============================================================
    // ----------------------- POST-SPAWN YÜKLEME -------------------
    // ============================================================

    // Bu, düşmanlar spawn edildikten sonra (birkaç frame sonra) çağrılır.
    private IEnumerator PostSpawnLoad()
    {
        // SpawnAllEnemies'in Instantiation'ı bitirmesi için 1 frame bekle
        yield return null;

        // Yeni spawn olanları listeye ekle
        foreach (var newZ in FindObjectsOfType<HealthEnemy>())
        {
            if (!zombies.Contains(newZ))
                zombies.Add(newZ);
        }

        // Gerçek yükleme işlemini yap
        ApplyLoadedData();

        Debug.Log("Kayıt başarıyla yüklendi (Spawn sonrası)!");
    }

    // ============================================================
    // ----------------------- MERKEZİ YÜKLEME METODU ----------------
    // ============================================================
    public void ApplyLoadedData()
    {
        // Listeyi null referanslardan temizle (önemli!)
        zombies.RemoveAll(z => z == null);

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
            inv.ForceUpdateEvents();
        }

        // 🔹 KAPI DURUMLARI
        for (int i = 0; i < doors.Count; i++)
        {
            string key = $"Door_{i}_IsOpen";
            if (saveFile.HasData(key))
            {
                bool isOpened = saveFile.GetData<bool>(key);
                if (isOpened)
                {
                    doors[i].doorAnimator.Play(doors[i].animationName);
                    doors[i].isOpened = true;
                }
            }
        }

        // 🔹 ZOMBİLERİN KONUM & CAN YÜKLEMESİ VE AI SIFIRLAMASI
        Transform playerTr = GameObject.FindGameObjectWithTag("Player")?.transform;

        for (int i = 0; i < zombies.Count; i++)
        {
            var z = zombies[i];
            if (z == null) continue;

            string prefix = $"Zombie_{i}_";

            // 1. Canı Yükle
            if (saveFile.HasData(prefix + "Health"))
                SetPrivateHealth(z, saveFile.GetData<float>(prefix + "Health"));

            float h = GetPrivateHealth(z);

            if (h <= 0f)
            {
                // Düşman öldüyse sahneden kaldır
                Destroy(z.gameObject);
                continue;
            }

            // 2. Konumu Yükle
            if (saveFile.HasData(prefix + "X"))
            {
                float zx = saveFile.GetData<float>(prefix + "X");
                float zy = saveFile.GetData<float>(prefix + "Y");
                float zz = saveFile.GetData<float>(prefix + "Z");
                z.transform.position = new Vector3(zx, zy, zz);
            }

            // 3. AI & NavMesh'i SIFIRLA (KRİTİK BÖLÜM)
            var agent = z.GetComponent<UnityEngine.AI.NavMeshAgent>();
            if (agent != null)
            {
                agent.enabled = false;
                agent.Warp(z.transform.position); // Konumu NavMesh'e bildir
                agent.enabled = true;
                agent.isStopped = false;
                agent.velocity = Vector3.zero;
            }

            // AI Scriptlerini (enemy1 / doctor) yenile
            var ai = z.GetComponent<enemy1>();
            var bossAI = z.GetComponent<doctor>(); // Boss'un scripti

            if (ai != null)
            {
                // Player referansını tazele
                var playerField = typeof(enemy1).GetField("player", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                if (playerField != null && playerTr != null)
                    playerField.SetValue(ai, playerTr);
                ai.enabled = false; // Resetle
                ai.enabled = true;
            }

            if (bossAI != null)
            {
                // Doctor (Boss) Player referansını tazele
                var playerField = typeof(doctor).GetField("player", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                if (playerField != null && playerTr != null)
                    playerField.SetValue(bossAI, playerTr);

                // AI'ı sıfırla
                bossAI.enabled = false;
                bossAI.enabled = true;
            }
        }

        // --- UI UPDATE ---
        if (ammoDisplay != null && weaponAmmo != null)
        {
            ammoDisplay.currentAmmo = weaponAmmo.Current;
            ammoDisplay.maxAmmo = weaponAmmo.Clip;
            ammoDisplay.UpdateAmmoUI();
        }
    }


    // ============================================================
    // ----------------------- YARDIMCI METOTLAR -------------------
    // ============================================================

    // ... (GetPrivateHealth ve SetPrivateHealth aynı kalsın) ...
    private float GetPrivateHealth(HealthEnemy enemy)
    {
        var field = typeof(HealthEnemy).GetField("currentHealth", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        return (float)field.GetValue(enemy);
    }

    private void SetPrivateHealth(HealthEnemy enemy, float value)
    {
        var field = typeof(HealthEnemy).GetField("currentHealth", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        field.SetValue(enemy, value);
    }

    // NOT: SpawnEnemiesAfterFrame() metodu artık LoadGame içinde çağrılmıyor, 
    // yerini PostSpawnLoad() ve LoadGame içindeki direkt if kontrolü aldı.
    // Bu metodu silebilirsiniz, veya aşağıdaki gibi bırakabilirsiniz:
    private IEnumerator SpawnEnemiesAfterFrame()
    {
        yield break;
    }


    // ============================================================
    // ----------------------- SAVEFILE OLUŞTURUCU ----------------
    // ============================================================
    private void EnsureSaveFileExists()
    {
        // ... (Bu metot aynı kalabilir) ...
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