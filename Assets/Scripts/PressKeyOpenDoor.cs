using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressKeyOpenDoor : MonoBehaviour
{
    [Header("UI & References")]
    public GameObject instructionUI;     // “Press E to open” yazýsý
    public Animator doorAnimator;        // Kapý animatörü (her kapýya özgü)
    public string animationName ; // Oynatýlacak animasyon ismi
    public GameObject triggerZone;       // Trigger objesi (isteðe baðlý)
    public AudioSource DoorOpenSound;

    private bool canOpen = false;

    void Start()
    {
        if (instructionUI != null)
            instructionUI.SetActive(false);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
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
            DoorOpenSound.Play(); 
        }

        if (instructionUI != null)
            instructionUI.SetActive(false);

        if (triggerZone != null)
            triggerZone.SetActive(false);

        
        

        canOpen = false;
    }
}
