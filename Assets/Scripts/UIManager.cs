

using UnityEngine;
using TMPro;
using System.Collections;

public class UIManager : MonoBehaviour
{
    // ----------------------------------------------------
    // BÖLGE 1: SAÐ ÜST BÝLDÝRÝM (Kýsa Süreli)
    // ----------------------------------------------------
    [Header("1. Notification (Sað Üst Bildirim)")]
    [SerializeField] private TextMeshProUGUI notificationText;
    [SerializeField] private float displayDuration = 2.0f;

    // ----------------------------------------------------
    // BÖLGE 2: ALT ORTA DÝYALOG/ÝÇ KONUÞMA (Karakter Konuþmasý)
    // ----------------------------------------------------
    [Header("2. Dialogue (Alt Orta Konuþma)")]
    [SerializeField] private TextMeshProUGUI dialogueText; //  YENÝ ALAN: Diyalog Text
    [SerializeField] private float dialogueDuration = 4.0f; // Konuþma daha uzun kalmalý

    [Header("3. Objective (Kalýcý Görev Metni)")]
    [SerializeField] private TextMeshProUGUI objectiveText;



    void Start()
    {
        // ... (NotificationText sýfýrlama mantýðý)

        if (dialogueText != null)
        {
            dialogueText.text = "";
            dialogueText.gameObject.SetActive(false);
        }

        if (objectiveText != null)
        {
            objectiveText.gameObject.SetActive(false); // Baþlangýçta gizli
        }
    }

    // ----------------------------------------------------
    // METOT 1: BÝLDÝRÝM (Mermi/Kit Toplandý)
    // ----------------------------------------------------
    public void ShowNotification(string message, Color color)
    {
        // ... (Mevcut Notification kodu ayný kalýr)
        // ... (CancelInvoke, text ayarý, Invoke("HideNotification", duration) vb.)
        if (notificationText == null) return;
        CancelInvoke("HideNotification");
        notificationText.color = color;
        notificationText.text = message;
        notificationText.gameObject.SetActive(true);
        Invoke("HideNotification", displayDuration);
    }

    // ----------------------------------------------------
    // METOT 2: DÝYALOG/ÝÇ KONUÞMA (Karakter Konuþmasý/Kapý Kilitli)
    // ----------------------------------------------------
    public void ShowDialogue(string message)
    {
        if (dialogueText == null) return;

        CancelInvoke("HideDialogue");

        dialogueText.text = message;
        dialogueText.gameObject.SetActive(true);
        // Diyaloglar beyaz renkte (varsayýlan) kalabilir.

        Invoke("HideDialogue", dialogueDuration);
    }

    private void HideDialogue()
    {
        if (dialogueText != null)
        {
            dialogueText.gameObject.SetActive(false);
        }
    }

    private void HideNotification()
    {
        if (notificationText != null)
        {
            notificationText.gameObject.SetActive(false);
        }
    }

    public void SetObjective(string message)
    {
        if (objectiveText == null) return;

        if (string.IsNullOrEmpty(message))
        {
            // Mesaj boþsa görevi gizle
            objectiveText.gameObject.SetActive(false);
        }
        else
        {
            // Mesaj varsa göster
            objectiveText.text = $"GÖREV: {message}";
            objectiveText.gameObject.SetActive(true);
        }
    }
}