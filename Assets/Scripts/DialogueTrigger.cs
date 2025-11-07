using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    [Header("Gereken Bileþenler")]
    [SerializeField] private UIManager uiManager;

    [Header("Diyalog Ayarlarý")]
    [TextArea(3, 5)] 
    [SerializeField] private string dialogueMessage = " ";

    private bool hasTriggered = false; // Diyalogun sadece bir kez tetiklenmesi için

    void Start()
    {
        
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
            //  Tekrar tetiklenmemesi için iþaretle
            hasTriggered = true;

            //  UIManager üzerinden diyaloðu gösterir
            if (uiManager != null)
            {
                uiManager.ShowDialogue(dialogueMessage);
            }

            
        }
    }
}