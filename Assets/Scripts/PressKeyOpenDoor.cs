using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressKeyOpenDoor : MonoBehaviour
{   
    [HideInInspector] public bool isOpened = false;
    [Header("UI & References")]
    public GameObject instructionUI;     // “Press E to open” yazısı
    public Animator doorAnimator;        // Kapı animatörü (her kapıya özgü)
    public string animationName; // Oynatılacak animasyon ismi
    public GameObject triggerZone;       // Trigger objesi (isteğe bağlı)
    public AudioSource DoorOpenSound;

    [Header("Door Sounds")]
    public AudioSource DoorSoundSource; // Sesin çalınacağı AudioSource bileşeni
    public AudioClip DefaultOpenSound;  // Normal kapılar için ses (Eski DoorOpenSound'un clip'i)
    public AudioClip SideDoorOpenSound; // ⬅️ SideDoor için özel ses

    public AudioClip lockSound;

    private bool canOpen = false;

    [Header("Extra For SideDoor")]
    public bool isSideDoor = false;
    public Transform walkTargetPoint;
    public float walkSpeed = 2.5f;
    public doctor targetEnemy;
    private bool isWalking = false;
    private Transform player;

    [Header("Inventory & UI References")]
    [SerializeField] private InventoryData inventoryData; 
    [SerializeField] private UIManager uiManager;        

    [Header("Audio Transition - Main Door")]
    public AudioSource CryingAudioSource;
    
    public AudioSource TempoMusicSource;
    public AudioSource LibraryTheme;


   
    public bool MainDoor = false;

    private bool isDoorOpen = false;

    public EnemySpawner spawner;

   

    void Start()
    {
        if (instructionUI != null)
            instructionUI.SetActive(false);
    }

    
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if(isOpened) return;
            player = other.transform;

            if (instructionUI != null)
                instructionUI.SetActive(true);
            canOpen = true;

            
        }
    }
   

    void OnTriggerExit(Collider other)
    {
        if (instructionUI != null)
            instructionUI.SetActive(false);
        canOpen = false;
    }

    void Update()
    {
        if (canOpen && Input.GetKeyDown(KeyCode.E))
        {
            TryOpenDoor(); // ⬅️ OpenDoor yerine TryOpenDoor metodunu çağıralım
        }
        //Eğer SideDoor ise player'ı hedef noktaya doğru yürüt
        if (isWalking && player != null && walkTargetPoint != null)
        {
            player.position = Vector3.MoveTowards(
                player.position,
                walkTargetPoint.position,
                walkSpeed * Time.deltaTime
            );

            // Hedefe ulaştıysa durdur
            if (Vector3.Distance(player.position, walkTargetPoint.position) < 0.1f)
            {
                isWalking = false;
            }
        }
    }
    //  YENİ COROUTINE: Diyalogu gecikmeli olarak gösterir
    private IEnumerator ShowDialogueDelayed(string message, float delay)
    {
        // Yarım saniye (veya belirlediğiniz süre) bekler
        yield return new WaitForSeconds(delay);

        // Gecikmeden sonra diyalog metodunu çağırır.
        if (uiManager != null)
        {
            uiManager.ShowDialogue(message);
        }
    }

    void TryOpenDoor()
    {
        if (isSideDoor)
        {
            if (inventoryData != null && inventoryData.HasKey)
            {
                // Anahtar VAR
                OpenDoor();
                inventoryData.SetKey(false);
                if (uiManager != null)
                {
                    uiManager.ShowNotification("Kapı anahtarla açıldı!", Color.green);
                }
            }
            else
            {
                

                // 1. Kilit Sesi Çalma
                if (lockSound != null && DoorSoundSource != null)
                {
                    // Mevcut sesi durdur (gerekirse)
                    DoorSoundSource.Stop();
                    // Yeni klibi yükle ve hemen çal
                    DoorSoundSource.clip = lockSound;
                    DoorSoundSource.Play();
                }

                
                // 2. Diyalog Mesajını Başlatma (Gecikmeli)
                string dialogMessage = "Kilitli... Tabii ki kilitli. Biliyordum... Beni yine buraya getireceğini biliyordum. 'O gece' yarım kalan hesabı bitirmenin vakti geldi, Doktor. O anahtarı bulacağım.";
                float dialogDelay = 0.5f; //  Yarım saniye (500 milisaniye) gecikme

                // Coroutine'i başlat
                StartCoroutine(ShowDialogueDelayed(dialogMessage, dialogDelay));

                if (uiManager != null)
                {
                    uiManager.SetObjective("Anahtarı bul.");
                }
            }
        }
        else
        {
            // SideDoor değilse (Normal Kapı / Main Door) direkt aç
            OpenDoor();
        }
    }

    void OpenDoor()
    {

        if(isOpened )return;
        if (doorAnimator != null)
        {
            doorAnimator.Play(animationName);
        }
        if (DoorOpenSound != null)
        {
            AudioClip clipToPlay = null;

            if (isSideDoor && SideDoorOpenSound != null)
            {
                clipToPlay = SideDoorOpenSound;
            }
            else if (DefaultOpenSound != null)
            {
                clipToPlay = DefaultOpenSound;
            }

            if (clipToPlay != null)
            {
                DoorSoundSource.clip = clipToPlay; 
                DoorSoundSource.Play();
            }
        }

        if (instructionUI != null)
            instructionUI.SetActive(false);

        if (triggerZone != null)
            triggerZone.SetActive(false);

        canOpen = false;

        
        if (MainDoor)
        {
            //  Zombileri Spawn Et
            if (spawner != null)
            {
                spawner.SpawnAllEnemies(); 
            }
            
            var saveData = FindObjectOfType<PlayerSaveData>();
            if (saveData != null)
            {
                saveData.enemiesSpawned = true;
                Debug.Log(" enemiesSpawned TRUE yapıldı!");
            }

            // 2. SES GEÇİŞİ
            if (CryingAudioSource != null && CryingAudioSource.isPlaying)
            {
                CryingAudioSource.Stop();
                Debug.Log("Ana Kapı açıldı: Ağlama sesi durduruldu.");
            }

            if (TempoMusicSource != null)
            {
                TempoMusicSource.loop = true;
                TempoMusicSource.Play();
                Debug.Log("Ana Kapı açıldı: Tempolu müzik başladı.");
            }

            if (uiManager != null)
            {
                // 1. Anlık Panik Diyaloğu (Alt-Orta)
                uiManager.ShowDialogue("Yine başlıyor... Olamaz, yine o geceki gibi! Kütüphaneye ulaşmam lazım. Her şeyin cevabı orda olmalı... Yan kapıdan!");

                // 2. Kalıcı Görev Metni (Sol Üst)
                uiManager.SetObjective("Yan kapıdan kütüphaneye ulaş.");
            }
        }

        // --- SideDoor Kontrolü (Ayrı) ---
        if (isSideDoor) // ⬅️ Eğer açılan kapı SideDoor ise
        {
            //  YENİ EKLENTİ: Tempolu müziği durdur
            if (TempoMusicSource != null && TempoMusicSource.isPlaying)
            {
                TempoMusicSource.Stop();
                

            }

            if (LibraryTheme != null)
            {
                LibraryTheme.loop = true;
                LibraryTheme.Play();
                
            }



            if (uiManager != null)
            {
                // 1. Anlık Panik Diyaloğu (Alt-Orta)
                uiManager.ShowDialogue("Sonunda... Kütüphane. O gece olan her şeyin cevabı bu odada olmalı. Biliyorum.");
            }

                // Kalan SideDoor mantığı (Düşmanı aktive etme)
                if (targetEnemy != null)
            {
                Debug.Log($"SideDoor açıldı! {targetEnemy.name} çağırılıyor...");
                targetEnemy.ActivateChase(GameObject.FindGameObjectWithTag("Player").transform);
            }
            else
            {
                Debug.LogWarning("SideDoor açıldı ama targetEnemy atanmamış!");
            }
        }
    }
}