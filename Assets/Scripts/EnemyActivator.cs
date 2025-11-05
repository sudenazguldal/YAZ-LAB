using UnityEngine;

public class EnemyActivator : MonoBehaviour
{
    // Unity Editor'den sürükleyip býrakacaðýmýz düþmanýn script'ine referans.
    public enemy1 targetEnemy;

    // Tetikleyicinin birden fazla kez çalýþmasýný engellemek için bayrak.
    private bool hasBeenActivated = false;

    // Bu obje ile baþka bir objenin Collider'ý çakýþtýðýnda çalýþýr.
    private void OnTriggerEnter(Collider other)
    {
        // Kontrol:
        // 1. Daha önce tetiklenmemiþ mi?
        // 2. Tetikleyen obje "Player" etiketine sahip mi?
        if (!hasBeenActivated && other.CompareTag("Player"))
        {
            if (targetEnemy != null)
            {
                // Düþmanýn hazýrladýðýmýz fonksiyonunu çaðýrýyoruz.
                // other.transform (oyuncunun Transform'u) hedef olarak gönderilir.
                targetEnemy.ActivateChase(other.transform);

                // Tekrar tetiklenmesini engelle.
                hasBeenActivated = true;
            }
        }
    }
}