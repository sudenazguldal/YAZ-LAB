using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    [Header("Gereken Bileþenler")]
    [SerializeField] private UIManager uiManager;

    [Header("Diyalog Ayarlarý")]
    [TextArea(3, 5)] // Metin kutusunu Inspector'da büyütür
    [SerializeField] private string dialogueMessage = "Bu aðlama sesi de ";

    private bool hasTriggered = false; // Diyalogun sadece bir kez tetiklenmesi için

    void Start()
    {
        // UIManager'ý sahnede bulmaya çalýþ (Eðer Inspector'dan atanmadýysa)
        if (uiManager == null)
        {
            uiManager = FindAnyObjectByType<UIManager>();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Eðer giren obje "Player" ise VE bu diyalog daha önce tetiklenmediyse
        if (other.CompareTag("Player") && !hasTriggered)
        {
            // 1. Tekrar tetiklenmemesi için iþaretle
            hasTriggered = true;

            // 2. UIManager üzerinden diyaloðu göster
            if (uiManager != null)
            {
                uiManager.ShowDialogue(dialogueMessage);
            }

            // 3. (Ýsteðe baðlý) Bu trigger artýk iþe yaramaz, kendini yok edebilir
            // Destroy(this.gameObject); 
        }
    }
}