
using UnityEngine.UI;
using UnityEngine;
using TMPro;
using System.Collections;

public class UIManager : MonoBehaviour
{
   
    [Header("1. Notification (Sað Üst Bildirim)")]
    [SerializeField] private TextMeshProUGUI notificationText;
    [SerializeField] private float displayDuration = 2.0f;

    [Header("2. Dialogue (Alt Orta Konuþma)")]
    [SerializeField] private TextMeshProUGUI dialogueText; 
    [SerializeField] private float dialogueDuration = 6.0f; 

    [Header("3. Objective (Kalýcý Görev Metni)")]
    [SerializeField] private TextMeshProUGUI objectiveText;

    [Header("4. Delirium Effect (Psychotic Sprite)")]
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
        if (dialogueText == null) return;

        CancelInvoke("HideDialogue");

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
            // ----- FLAÞ (YANIP SÖNME) -----
            flashColor.a = Random.Range(0.1f, 0.8f); // Alpha'yý rastgele yap
            deliriumEffectImage.color = flashColor;

            // ----- TÝTREÞÝM (JITTER) -----
            float jitterX = originalImagePosition.x + Random.Range(-deliriumJitterAmount, deliriumJitterAmount);
            float jitterY = originalImagePosition.y + Random.Range(-deliriumJitterAmount, deliriumJitterAmount);

            deliriumEffectImage.rectTransform.anchoredPosition = new Vector2(jitterX, jitterY);

            yield return new WaitForSeconds(deliriumFlashSpeed); // Çok hýzlý bekle
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