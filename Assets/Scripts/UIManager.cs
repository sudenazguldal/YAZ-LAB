using UnityEngine;
using TMPro; 

public class UIManager : MonoBehaviour
{
    //  UNITY EDITOR'DA BURAYA BÝR TMP TEXT ALANI BAÐLAYIN
    [Header("Toplama Mesajlarý")]
    [SerializeField] private TextMeshProUGUI notificationText;
    [SerializeField] private float displayDuration = 2.0f;

    // Mesajlarý sýrayla göstermek için bir Kuyruk (Queue) kullanýlabilir
    // Ancak basitlik için sadece tek mesaj gösterelim.

    void Start()
    {
        if (notificationText != null)
        {
            notificationText.text = ""; // Baþlangýçta boþ olsun
            notificationText.gameObject.SetActive(false); // Baþlangýçta gizli olsun
        }
    }

   
    public void ShowNotification(string message, Color color)
    {
        if (notificationText == null) return;

        // Eski mesajý sýfýrla (eðer hala gösteriliyorsa)
        CancelInvoke("HideNotification");

        notificationText.color = color;
        notificationText.text = message;
        notificationText.gameObject.SetActive(true);

        // Belirtilen süre sonunda mesajý gizle
        Invoke("HideNotification", displayDuration);
    }

    private void HideNotification()
    {
        if (notificationText != null)
        {
            notificationText.gameObject.SetActive(false);
        }
    }
}