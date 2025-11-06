using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressKeyOpenDoor : MonoBehaviour
{
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
    [SerializeField] private InventoryData inventoryData; // Proje penceresindeki SO dosyası buraya sürüklenecek
    [SerializeField] private UIManager uiManager;        // Canvas'a takılı UIManager buraya sürüklenecek
    
    

    public bool MainDoor = false;




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
                string dialogMessage = "Kilitli. Belki etrafta anahtarını bulabilirim.";
                float dialogDelay = 0.5f; //  Yarım saniye (500 milisaniye) gecikme

                // Coroutine'i başlat
                StartCoroutine(ShowDialogueDelayed(dialogMessage, dialogDelay));
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
        if (doorAnimator != null)
        {
            doorAnimator.Play(animationName);
        }
        if (DoorOpenSound != null)
        {
            AudioClip clipToPlay = null;

            if (isSideDoor && SideDoorOpenSound != null)
            {
                // Side Door ise özel sesi kullan
                clipToPlay = SideDoorOpenSound;
            }
            else if (DefaultOpenSound != null)
            {
                // Normal kapı veya SideDoor'a özel ses atanmamışsa varsayılanı kullan
                clipToPlay = DefaultOpenSound;
            }

            if (clipToPlay != null)
            {
                DoorOpenSound.clip = clipToPlay;
                DoorOpenSound.Play();
            }
        }

        if (instructionUI != null)
            instructionUI.SetActive(false);

        if (triggerZone != null)
            triggerZone.SetActive(false);




        canOpen = false;

        if (MainDoor && spawner != null)
        {
            spawner.SpawnAllEnemies();
        }

        if (isSideDoor && targetEnemy != null)
        {
            Debug.Log($"SideDoor açıldı! {targetEnemy.name} çağırılıyor...");
            targetEnemy.ActivateChase(GameObject.FindGameObjectWithTag("Player").transform);
        }
        else
        {
            Debug.LogWarning(" targetEnemy atanmamış veya SideDoor false!");
        }

    }
}