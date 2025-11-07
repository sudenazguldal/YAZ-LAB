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

    private bool canOpen = false;

    [Header("Extra For SideDoor")]
    public bool isSideDoor = false;
    public Transform walkTargetPoint;
    public float walkSpeed = 2.5f;
    public doctor targetEnemy;
    private bool isWalking = false;
    private Transform player;

    public bool MainDoor = false;

    private bool isDoorOpen = false;

    public EnemySpawner spawner;

    
    void Start()
    {
        if (instructionUI != null)
            instructionUI.SetActive(false);
    }

    /*private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player")) 
        {
            if (!isWalking) 
            {
                isWalking=true;
            }
        }
        
    }*/

    /*void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (instructionUI != null)
                instructionUI.SetActive(true);
            canOpen = true;
        }
    }*/
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Eğer kapı zaten açıksa hiç işlem yapma
            if (isOpened) return;

            player = other.transform;

            if (instructionUI != null)
                instructionUI.SetActive(true);
            canOpen = true;
        }
    }
  /*  private void OnTriggerStay(Collider other)
    {

        if (other.CompareTag("Player"))
        {
            if (!isWalking)
            {
                isWalking = true;
            }
        }
    }*/

    void OnTriggerExit(Collider other)
    {
        if (instructionUI != null)
            instructionUI.SetActive(false);
        canOpen = false;
    }

    void Update()
    {
        if (canOpen && !isOpened && Input.GetKeyDown(KeyCode.E))
        {
            OpenDoor();
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

    void OpenDoor()
    {
        if (isOpened) return; // Kapı zaten açık, hiçbir şey yapma

        if (doorAnimator != null)
        {
            doorAnimator.Play(animationName);
        }
        if (DoorOpenSound != null)
        {
            DoorOpenSound.Play();
        }

        if (instructionUI != null)
            instructionUI.SetActive(false);

        if (triggerZone != null)
            triggerZone.SetActive(false);

        canOpen = false;

        if (MainDoor && spawner != null)
        {
            spawner.SpawnAllEnemies();

            //  enemiesSpawned true yap
            var saveData = FindObjectOfType<PlayerSaveData>();
            if (saveData != null)
            {
                saveData.enemiesSpawned = true;
                Debug.Log(" enemiesSpawned TRUE yapıldı!");
            }
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

        isOpened = true; 


    }
}