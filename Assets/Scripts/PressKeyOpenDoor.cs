using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressKeyOpenDoor : MonoBehaviour
{   
    [HideInInspector] public bool isOpened = false;
    [Header("UI & References")]
    public GameObject instructionUI;     
    public Animator doorAnimator;        
    public string animationName; 
    public GameObject triggerZone;       
    public AudioSource DoorOpenSound;

    [Header("Door Sounds")]
    public AudioSource DoorSoundSource; 
    public AudioClip DefaultOpenSound; 
    public AudioClip SideDoorOpenSound; 

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
            TryOpenDoor(); 
        }
        
        if (isWalking && player != null && walkTargetPoint != null)
        {
            player.position = Vector3.MoveTowards(
                player.position,
                walkTargetPoint.position,
                walkSpeed * Time.deltaTime
            );

          
            if (Vector3.Distance(player.position, walkTargetPoint.position) < 0.1f)
            {
                isWalking = false;
            }
        }
    }
    // Gecikmeli diyalog gösterme için coroutine metodu ses biraz geç başlıyo 
    private IEnumerator ShowDialogueDelayed(string message, float delay)
    {
        // Yarım saniye bekler
        yield return new WaitForSeconds(delay);

        // Gecikmeden sonra diyalog metodunu çağırır
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
                // Anahtar VARSA açar
                OpenDoor();
                inventoryData.SetKey(false);
                if (uiManager != null)
                {
                    uiManager.ShowNotification("Kapı anahtarla açıldı!", Color.green);
                }
            }
            else
            {
                

              
                if (lockSound != null && DoorSoundSource != null)
                {
                    // Mevcut sesi durdur  bazen kafayı yiyor diğer seslerle
                    DoorSoundSource.Stop();
                    // Yeni klibi yükler ve çalar
                    DoorSoundSource.clip = lockSound;
                    DoorSoundSource.Play();
                }

                
                // gecikmeli dialogue
                string dialogMessage = "Kilitli... Tabii ki kilitli. Biliyordum... Beni yine buraya getireceğini biliyordum. 'O gece' yarım kalan hesabı bitirmenin vakti geldi, Doktor. O anahtarı bulacağım.";
                float dialogDelay = 0.5f; //  Yarım saniye (500 milisaniye) gecikme

                
                StartCoroutine(ShowDialogueDelayed(dialogMessage, dialogDelay));

                if (uiManager != null)
                {
                    uiManager.SetObjective("Anahtarı bul.");
                }
            }
        }
        else
        {
            // SideDoor değilse direkt aç
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
            //  Zombileri spawn et
            if (spawner != null)
            {
                spawner.SpawnAllEnemies(); 
            }
            
            var saveData = FindObjectOfType<PlayerSaveData>();
            if (saveData != null)
            {
                saveData.enemiesSpawned = true;
               
            }

            // 2. SES GEÇİŞİ
            if (CryingAudioSource != null && CryingAudioSource.isPlaying)
            {
                CryingAudioSource.Stop();
                
            }

            if (TempoMusicSource != null)
            {
                TempoMusicSource.loop = true;
                TempoMusicSource.Play();
                
            }

            if (uiManager != null)
            {
               
                uiManager.ShowDialogue("Yine başlıyor... Olamaz, yine o geceki gibi! Kütüphaneye ulaşmam lazım. Her şeyin cevabı orda olmalı... Yan kapıdan!");

              
                uiManager.SetObjective("Yan kapıdan kütüphaneye ulaş.");
            }
        }

        
        if (isSideDoor) //açılan kapı side door mu kontrolü
        {
            
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
                
                uiManager.ShowDialogue("Sonunda... Kütüphane. O gece olan her şeyin cevabı bu odada olmalı. Biliyorum.");
            }

               
        }
    }
}