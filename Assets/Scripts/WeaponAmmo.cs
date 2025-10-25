using UnityEngine;

public class WeaponAmmo : MonoBehaviour
{
    [Header("Ammo ")]
    [SerializeField] private int ClipSize;
    [SerializeField] private int ExtraAmmo;
    [SerializeField] public int CurrentAmmo;
    void Start()
    {
        CurrentAmmo = ClipSize;
    }

    public void Update()
    {
      if (Input.GetKeyDown(KeyCode.R)) Reload();
    }
    public void Reload()
    {
        
        if (ExtraAmmo >= ClipSize)
        {
           int AmmoToReload = ClipSize-CurrentAmmo;
            ExtraAmmo -= AmmoToReload;
            CurrentAmmo+= AmmoToReload;
        }
        else if ( ExtraAmmo >0 ) 
        {
           if (ExtraAmmo + CurrentAmmo > ClipSize)
            {
                int leftOverAmmo = (ExtraAmmo + CurrentAmmo) - ClipSize;
                ExtraAmmo = leftOverAmmo;
                CurrentAmmo = ClipSize;
            }
            else
            {
                CurrentAmmo += ExtraAmmo;
                ExtraAmmo = 0;
            }

        }
    }


}
