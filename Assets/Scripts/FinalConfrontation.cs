
using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class FinalConfrontation : MonoBehaviour
{
    [Header("Referanslar")]
    [SerializeField] private UIManager uiManager;
    [SerializeField] private HealthEnemy doctorHealth;
    [SerializeField] private PlayerController playerController; 
    [SerializeField] private CameraSwitcher cameraSwitcher;
    [Header("Diyalog ve Zamanlama")]
    [SerializeField] private float dialogueDelay = 6.5f;
    [Header("Delirium Audio")]
    [SerializeField] private AudioSource heartbeatSource;
    [SerializeField] private AudioLowPassFilter mainAudioFilter;
    private doctor doctorScript;

    // Player referanslarý

    private Animator playerAnimator;
    private PlayerShooting playerShooting;

   

    private bool sequenceStarted = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !sequenceStarted)
        {
            sequenceStarted = true;

            if (cameraSwitcher == null)
            {
                cameraSwitcher = other.GetComponent<CameraSwitcher>();
            }
            if (doctorHealth != null)
            {
                doctorScript = doctorHealth.GetComponent<doctor>();
            }
           
            playerController = other.GetComponent<PlayerController>();
            playerAnimator = other.GetComponentInChildren<Animator>();
            playerShooting = other.GetComponent<PlayerShooting>();

            
            StartCoroutine(ConfrontationSequence());
        }
    }

    private IEnumerator ConfrontationSequence()
    {
        playerController.isInCutscene = true;

        // camSwitcher ile aimcam'e geçiþ yap daha sinematik olsun
        if (cameraSwitcher != null)
        {
            cameraSwitcher.ForceAim(true);
        }

        yield return new WaitForSeconds(1.0f); // Kameranýn geçiþ yapmasý için garip gözükmesini önlemek adýna kýsa bir bekleme

        //DÝYALOGLAR 
        if (doctorScript != null) doctorScript.SetTalking(true);
        uiManager.ShowDialogue("Ýnanýlmaz... Onca kusursuz yaratýmým arasýndan yine sen kaldýn. Yýllar geçti, ama hâlâ o gecenin hatasýný telafi edemedim.");
        yield return new WaitForSeconds(dialogueDelay);
        if (doctorScript != null) doctorScript.SetTalking(false);


        if (doctorScript != null) doctorScript.SetTalking(true);
        uiManager.ShowDialogue("Sen… benim tek baþarýsýzlýðým, ama ayný zamanda en büyük merakýmsýn.");
        yield return new WaitForSeconds(dialogueDelay);
        if (doctorScript != null) doctorScript.SetTalking(false);

        
        uiManager.ShowDialogue("Baþarýsýzlýk mý diyorsun? O gece beni parçaladýn, Doktor. Ýnsanlýðýmý senin laboratuvarýnda býraktým.");
        yield return new WaitForSeconds(dialogueDelay);

        uiManager.ShowDialogue("Ama sen hâlâ ayný takýntýdasýn - kusursuzluk. Bu gece, senin deneyin bitiyor.");
        yield return new WaitForSeconds(dialogueDelay);

        if (doctorScript != null) doctorScript.SetTalking(true);
        uiManager.ShowDialogue("Ah, yanýlýyorsun. Sen hâlâ deneyin bir parçasýsýn. Kaçtýðýný sandýn ama her adýmýn, her nefesin...");
        yield return new WaitForSeconds(dialogueDelay);
        if (doctorScript != null) doctorScript.SetTalking(false);

        if (doctorScript != null) doctorScript.SetTalking(true);
        uiManager.ShowDialogue("Hepsi gözlem altýndaydý. Bu gece sadece bir son deðil — sonuç raporu.");
        yield return new WaitForSeconds(dialogueDelay);
        if (doctorScript != null) doctorScript.SetTalking(false);

        // Flaþ Efektleri
        uiManager.ShowDialogue(null); 
        uiManager.ShowDeliriumEffect(true);
        if (heartbeatSource != null)
        {
            heartbeatSource.Play(); 
        }
        if (mainAudioFilter != null)
        {
            mainAudioFilter.enabled = true; // Sesi boðuklaþtýrmasý için
        }

        
    }

   
}