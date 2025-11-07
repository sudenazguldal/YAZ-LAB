
using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class FinalConfrontation : MonoBehaviour
{
    [Header("Referanslar")]
    [SerializeField] private UIManager uiManager;
    [SerializeField] private HealthEnemy doctorHealth;
    [SerializeField] private PlayerController playerController; // Hareketi kilitlemek için hala gerekli
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
            // Player üzerindeki tüm gerekli script'leri al
            playerController = other.GetComponent<PlayerController>();
            playerAnimator = other.GetComponentInChildren<Animator>();
            playerShooting = other.GetComponent<PlayerShooting>();

            
            StartCoroutine(ConfrontationSequence());
        }
    }

    private IEnumerator ConfrontationSequence()
    {
        playerController.isInCutscene = true;

        // 2. ZORLA NÝÞAN ALDIR (CameraSwitcher aracýlýðýyla)
        if (cameraSwitcher != null)
        {
            cameraSwitcher.ForceAim(true);
        }

        yield return new WaitForSeconds(1.0f); // Kameranýn geçiþ yapmasý için

        // 3. DÝYALOGLAR (Doktor)
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

        // 5. "DELÝRME" ANI (Flaþ Efekti)
        uiManager.ShowDialogue(null); // Diyalog metnini temizle
        uiManager.ShowDeliriumEffect(true); //  YENÝ FLAÞ EFEKTÝNÝ BAÞLAT
        if (heartbeatSource != null)
        {
            heartbeatSource.Play(); // Kalp atýþý baþlasýn
        }
        if (mainAudioFilter != null)
        {
            mainAudioFilter.enabled = true; // Sesi boðuklaþtýr
        }

        // 6. DOKTOR'UN ÖLÜMÜNÜ DÝNLE
        if (doctorHealth != null)
        {
            //doctorHealth.onDeath.AddListener(EndGame);
        }
    }

    
    private void EndGame()
    {
        Debug.Log("DOKTOR ÖLDÜ. OYUN BÝTÝYOR.");
        // Buraya oyun bitiþ ekranýný (Credits) yükleme kodunu ekleyebilirsiniz.
        // Örn: UnityEngine.SceneManagement.SceneManager.LoadScene("CreditsScene");

        // Þimdilik zamaný durduralým
        Time.timeScale = 0f;
    }
}