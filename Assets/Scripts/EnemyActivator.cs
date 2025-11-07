using UnityEngine;

public class EnemyActivator : MonoBehaviour
{
 
    public enemy1 targetEnemy;

   
    private bool hasBeenActivated = false;

   
    private void OnTriggerEnter(Collider other)
    {
        // Kontrol:Daha önce tetiklenmemiþ mi  Tetikleyen obje "Player" etiketine sahip mi
       
        if (!hasBeenActivated && other.CompareTag("Player"))
        {
            if (targetEnemy != null)
            {
                // Düþmanýn hazýrladýðýmýz fonksiyonunu çaðýrýyoruz
              
                targetEnemy.ActivateChase(other.transform);

                // Tekrar tetiklenmesini engelle
                hasBeenActivated = true;
            }
        }
    }
}