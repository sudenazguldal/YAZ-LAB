
using UnityEngine.UI;
using UnityEngine;
using TMPro;
using System.Collections;

public class UIManager : MonoBehaviour
{
   
    [Header("1. Notification ")]
    [SerializeField] private TextMeshProUGUI notificationText;
    [SerializeField] private float displayDuration = 2.0f;

    [Header("2. Dialogue ")]
    [SerializeField] private TextMeshProUGUI dialogueText; 
    [SerializeField] private float dialogueDuration = 6.0f;
    [SerializeField] private Color playerDialogueColor = Color.white;

    [Header("3. Objective ")]
    [SerializeField] private TextMeshProUGUI objectiveText;

    [Header("4. Delirium Effect ")]
    [SerializeField] private Image deliriumEffectImage; 
    [SerializeField] private float deliriumFlashSpeed = 0.1f; 
    [SerializeField] private Color deliriumColor = Color.red; 
   
 

    [SerializeField] private float deliriumJitterAmount = 50f;

    private Coroutine deliriumCoroutine;
    private Vector2 originalImagePosition;

    void Start()
    {
        

        if (dialogueText != null)
        {
            dialogueText.text = "";
            dialogueText.gameObject.SetActive(false);
        }

        if (objectiveText != null)
        {
            objectiveText.gameObject.SetActive(false);
        }

        if (deliriumEffectImage != null)
        {
            
            originalImagePosition = deliriumEffectImage.rectTransform.anchoredPosition;
            deliriumEffectImage.gameObject.SetActive(false);
        }

    }

    
   
    public void ShowNotification(string message, Color color)
    {
       
        if (notificationText == null) return;
        CancelInvoke("HideNotification");
        notificationText.color = color;
        notificationText.text = message;
        notificationText.gameObject.SetActive(true);
        Invoke("HideNotification", displayDuration);
    }

 
    public void ShowDialogue(string message)
    {
        ShowDialogue(message, playerDialogueColor);
    }

    public void ShowDialogue(string message, Color color)
    {
        if (dialogueText == null) return;

        
        CancelInvoke("HideDialogue");

        // Eðer mesaj boþsa (null), metni hemen gizle ve çýk
        if (string.IsNullOrEmpty(message))
        {
            HideDialogue();
            return;
        }

        // Renk ve Metni Ayarla
        dialogueText.color = color;
        dialogueText.text = message;
        dialogueText.gameObject.SetActive(true);

        
        Invoke("HideDialogue", dialogueDuration);
    }

    private void HideDialogue()
    {
        if (dialogueText != null)
        {
            dialogueText.gameObject.SetActive(false);
        }
    }


    public void ShowDeliriumEffect(bool show)
    {
        if (deliriumEffectImage == null) return;

        if (show)
        {
            // Eðer zaten çalýþýyorsa durdur ve yeniden baþlat
            if (deliriumCoroutine != null)
            {
                StopCoroutine(deliriumCoroutine);
            }
            deliriumCoroutine = StartCoroutine(FlashDeliriumEffect());
        }
        else
        {
            // Efekti durdur
            if (deliriumCoroutine != null)
            {
                StopCoroutine(deliriumCoroutine);
            }
            deliriumCoroutine = null;
            deliriumEffectImage.gameObject.SetActive(false);
        }
    }


    private IEnumerator FlashDeliriumEffect()
    {
        deliriumEffectImage.gameObject.SetActive(true);
        Color flashColor = deliriumColor;

        while (true) // Doktor ölene kadar
        {
            // ----- FLAÞ yanýp sönme -----
            flashColor.a = Random.Range(0.1f, 0.8f); // Alpha'yý rastgele yap
            deliriumEffectImage.color = flashColor;

            // ----- TÝTREÞÝM jitter olan -----
            float jitterX = originalImagePosition.x + Random.Range(-deliriumJitterAmount, deliriumJitterAmount);
            float jitterY = originalImagePosition.y + Random.Range(-deliriumJitterAmount, deliriumJitterAmount);

            deliriumEffectImage.rectTransform.anchoredPosition = new Vector2(jitterX, jitterY);

            yield return new WaitForSeconds(deliriumFlashSpeed);
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