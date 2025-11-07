using Esper.ESave;
using Esper.ESave.Encryption;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq; 
using UnityEngine;
using static Esper.ESave.SaveFileSetupData;


public class PlayerSaveData : MonoBehaviour
{
    [HideInInspector] public bool enemiesSpawned = false;


    [Header("Default Spawn")]
    public Transform startPoint;

    [Header("Scene References")]
    public List<PressKeyOpenDoor> doors; 
    public List<HealthEnemy> zombies;   

    [Header("Optional")]
    public EnemySpawner enemySpawner;

    public HealthComponent playerHealth;
    public WeaponAmmo weaponAmmo;
    public RevolverAmmoDisplay ammoDisplay;
    private SaveFile saveFile;


    void Awake()
    {
        
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

    
    public void SaveGame()
    {
        if (saveFile == null)
        {
            
            return;
        }

        zombies.RemoveAll(z => z == null);

        // Sahnedeki tüm aktif HealthEnemy bileşenlerini bulur
        var allEnemiesInScene = FindObjectsOfType<HealthEnemy>();
        foreach (var enemy in allEnemiesInScene)
        {
            // Eğer bu düşman zaten listede yoksa, onu listeye ekler aktif olarak haritada bulunanları tutar
            if (!zombies.Contains(enemy))
            {
                zombies.Add(enemy);
                
            }
        }

        Vector3 pos = transform.position;

        saveFile.AddOrUpdateData("PlayerX", pos.x);
        saveFile.AddOrUpdateData("PlayerY", pos.y);
        saveFile.AddOrUpdateData("PlayerZ", pos.z);

        if (playerHealth != null)
            saveFile.AddOrUpdateData("PlayerHealth", playerHealth.currentHealth);

        // --- silah & envanter datası ---
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

        // kapıların durumu
        for (int i = 0; i < doors.Count; i++)
        {
            bool isOpened = false;
            Animator anim = doors[i].doorAnimator;
            if (anim != null)
            {
                AnimatorStateInfo state = anim.GetCurrentAnimatorStateInfo(0);
                // Kapının açıldığı animasyonun adını kontrol eder
                isOpened = state.IsName(doors[i].animationName) || doors[i].isOpened;
            }
            saveFile.AddOrUpdateData($"Door_{i}_IsOpen", isOpened);
        }

        // ZOMBİLER
       
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

       
    }


    public void LoadGame()
    {
        if (enemySpawner == null)
        {
            enemySpawner = FindObjectOfType<EnemySpawner>();
            
        }

        EnsureSaveFileExists();
        if (saveFile == null)
        {
            
            return;
        }

        saveFile.Load();

        
        if (saveFile.HasData("EnemiesSpawned"))
            enemiesSpawned = saveFile.GetData<bool>("EnemiesSpawned");

        // Zombi listesini temizler
        zombies.RemoveAll(z => z == null);

        // Sahne kontorlü Boss hariç düşman var mı?
        bool sceneHasNonBossEnemy = FindObjectsOfType<HealthEnemy>().Any(z => !z.gameObject.CompareTag("Boss"));

        if (enemiesSpawned && !sceneHasNonBossEnemy && enemySpawner != null)
        {
            
            enemySpawner.SpawnAllEnemies();

            
            StartCoroutine(PostSpawnLoad());
            return; 
        }
        else
        {
            // Eğer düşmanlar zaten sahnedeyse VEYA hiç spawn edilmemişse, direk yükle
            ApplyLoadedData();
        }

        Debug.Log("Kayıt başarıyla yüklendi!");
    }

  

    // Bu, düşmanlar spawn edildikten sonra -birkaç frame sonra- çağrılır.
    private IEnumerator PostSpawnLoad()
    {
        
        yield return null;

        
        foreach (var newZ in FindObjectsOfType<HealthEnemy>())
        {
            if (!zombies.Contains(newZ))
                zombies.Add(newZ);
        }

        
        ApplyLoadedData();

       
    }

    
    public void ApplyLoadedData()
    {
        
        zombies.RemoveAll(z => z == null);

        // --- karakter pozisyonu ---
        if (saveFile.HasData("PlayerX"))
        {
            float x = saveFile.GetData<float>("PlayerX");
            float y = saveFile.GetData<float>("PlayerY");
            float z = saveFile.GetData<float>("PlayerZ");
            transform.position = new Vector3(x, y, z);
        }

        // --- sağlık ---
        if (playerHealth != null && saveFile.HasData("PlayerHealth"))
            playerHealth.currentHealth = saveFile.GetData<float>("PlayerHealth");

        // --- mühimmat ---
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

        // --- envanter ---
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

        // kapıların durumu
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

        // zombi load 
        Transform playerTr = GameObject.FindGameObjectWithTag("Player")?.transform;

        for (int i = 0; i < zombies.Count; i++)
        {
            var z = zombies[i];
            if (z == null) continue;

            string prefix = $"Zombie_{i}_";

            // 1- can
            if (saveFile.HasData(prefix + "Health"))
                SetPrivateHealth(z, saveFile.GetData<float>(prefix + "Health"));

            float h = GetPrivateHealth(z);

            if (h <= 0f)
            {
                // Düşman öldüyse sahneden kaldır
                Destroy(z.gameObject);
                continue;
            }

            // 2- pozisyon
            if (saveFile.HasData(prefix + "X"))
            {
                float zx = saveFile.GetData<float>(prefix + "X");
                float zy = saveFile.GetData<float>(prefix + "Y");
                float zz = saveFile.GetData<float>(prefix + "Z");
                z.transform.position = new Vector3(zx, zy, zz);
            }

            // 3. AI & NavMesh'i SIFIRLAr
            var agent = z.GetComponent<UnityEngine.AI.NavMeshAgent>();
            if (agent != null)
            {
                agent.enabled = false;
                agent.Warp(z.transform.position); // Konumu NavMesh'e bildirir
                agent.enabled = true;
                agent.isStopped = false;
                agent.velocity = Vector3.zero;
            }

            // AI Scriptlerini -enemy1 , doctor yenile
            var ai = z.GetComponent<enemy1>();
            var bossAI = z.GetComponent<doctor>(); // Boss un scripti

            if (ai != null)
            {
                // Player referansını yeniler
                var playerField = typeof(enemy1).GetField("player", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                if (playerField != null && playerTr != null)
                    playerField.SetValue(ai, playerTr);
                ai.enabled = false; // Resetle
                ai.enabled = true;
            }

            if (bossAI != null)
            {
                // Doctor (Boss) Player referansını yeniler
                var playerField = typeof(doctor).GetField("player", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                if (playerField != null && playerTr != null)
                    playerField.SetValue(bossAI, playerTr);

                // AI'ı sıfırlar
                bossAI.enabled = false;
                bossAI.enabled = true;
            }
        }

        // --- UI güncelleme ---
        if (ammoDisplay != null && weaponAmmo != null)
        {
            ammoDisplay.currentAmmo = weaponAmmo.Current;
            ammoDisplay.maxAmmo = weaponAmmo.Clip;
            ammoDisplay.UpdateAmmoUI();
        }
    }


 


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

    
    private IEnumerator SpawnEnemiesAfterFrame()
    {
        yield break;
    }


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