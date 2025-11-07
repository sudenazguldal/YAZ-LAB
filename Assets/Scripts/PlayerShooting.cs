using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    [Header("Shooting Config")]
    [SerializeField] private float fireRate = 0.15f;
    public float FireRate => fireRate; // Dışarıdan okumak için
    [SerializeField] private float damage = 25f;
    [SerializeField] private LayerMask aimColliderLayerMask = default; 

    [Header("VFX")]
    [SerializeField] private Transform spawnBulletPosition;
    [SerializeField] private GameObject vfxMuzzleFlash;
    [SerializeField] private GameObject vfxHitTarget;
    [SerializeField] private GameObject vfxHitOthers;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource; 
    [SerializeField] private AudioClip shootSound;     
    [SerializeField] private AudioClip hitTargetSound; 
    [SerializeField] private AudioClip hitOthersSound;
    [SerializeField] private AudioClip emptyGunSound;
    [Header("Cover Shooting")]
    private bool isPeeking = false; // PlayerController'dan gelen Cover Aim durumu
    [SerializeField] private float peekHeightOffset = 0.6f; // Siperden uzanma yüksekliği 

    [Header("Inventory")]
   
    [SerializeField] private InventoryData inventoryData;
    [SerializeField] private int ammoCostPerShot = 1; // Her atışta harcanan mermi miktarı 

    WeaponAmmo Ammo;

    private float nextFireTime = 0.1f;
    private bool isShooting = false;
    private bool currentIsAiming = false; // PlayerController'dan gelecek Aim durumunu tutar

    

    void Start()
    {
        Ammo = GetComponentInChildren<WeaponAmmo>();
    }

    // PlayerController'dan çağrılacak Aim durumunu güncelleme metodu
    public void SetAiming(bool isAiming, bool isCovering)
    {
        currentIsAiming = isAiming;

        // isPeeking durumu Siperdeysen VE nişan alıyorsan TRUE
        isPeeking = isCovering && isAiming;
    }
    public void HandleShootInput(bool isPressed)
    {
        isShooting = isPressed;
    }

    // PlayerController'dan çağrılacak ana metod
    public void Shoot(Vector3 targetPoint)
    {
        // Güvenlik Kontrolleri
        if (currentIsAiming == false) return;
        if (Ammo == null) return; 

        //ammo kontrol
        if (Ammo.CurrentAmmo <= 0)
        {
            // no ammo sound
            if (audioSource != null && emptyGunSound != null)
            {
                audioSource.PlayOneShot(emptyGunSound);
            }
            return; 
        }

        // atış var düşür ammo sayacını
        Ammo.CurrentAmmo--; 
                            

        // pozisyon ve peek hesapları
        Vector3 bulletSpawnPosition = spawnBulletPosition.position;

        if (isPeeking)
        {
            // Namlu pozisyonunu siperden uzanıyormuş gibi yukarı kaydır animasyonda olmuyo ama işlevde oluyo
            bulletSpawnPosition += Vector3.up * peekHeightOffset;
        }

        // vfx ve soundlar
        if (vfxMuzzleFlash != null)
        {
            // Muzzle Flash oluşturma ve yok etme mantığı (Mevcut kodunuz)
            GameObject muzzleFlashInstance = Instantiate(vfxMuzzleFlash, bulletSpawnPosition, spawnBulletPosition.rotation);
            ParticleSystem ps = muzzleFlashInstance.GetComponent<ParticleSystem>();
           
            Destroy(muzzleFlashInstance, 0.5f); 
        }

        if (audioSource != null && shootSound != null)
        {
            audioSource.PlayOneShot(shootSound);
        }

        // hitscan
        Vector3 shootDirection = (targetPoint - bulletSpawnPosition).normalized;
        RaycastHit hitInfo;

        if (Physics.Raycast(bulletSpawnPosition, shootDirection, out hitInfo, 1000f, aimColliderLayerMask))
        {
            HandleHit(hitInfo);
        }
    }

    private void HandleHit(RaycastHit hitInfo)
    {
        HealthEnemy targetHealth = hitInfo.transform.GetComponent<HealthEnemy>();
        Quaternion hitRotation = Quaternion.LookRotation(hitInfo.normal);

        if (targetHealth != null)
        {
            //  Zombi vuruldu
            targetHealth.TakeDamage(damage);

            //  Vuruş efekti
            if (vfxHitTarget != null)
            {
                GameObject hitVFX = Instantiate(vfxHitTarget, hitInfo.point, hitRotation);
                ParticleSystem ps = hitVFX.GetComponent<ParticleSystem>();
                if (ps != null)
                {
                    float totalDuration = ps.main.duration + ps.main.startLifetime.constantMax;
                    ps.Play();
                    Destroy(hitVFX, totalDuration + 0.1f);
                }
            }

            
            if (audioSource != null && hitTargetSound != null)
            {
               
                audioSource.PlayOneShot(hitTargetSound, 1.0f);
            }
        }
        else
        {
           
            if (vfxHitOthers != null)
            {
                GameObject hitVFX = Instantiate(vfxHitOthers, hitInfo.point, hitRotation);
                ParticleSystem ps = hitVFX.GetComponent<ParticleSystem>();
                if (ps != null)
                {
                    float totalDuration = ps.main.duration + ps.main.startLifetime.constantMax;
                    ps.Play();
                    Destroy(hitVFX, totalDuration + 0.1f);
                }
                else
                {
                    Destroy(hitVFX, 0.5f);
                }
            }

            // Diğer yüzey sesi
            if (audioSource != null && hitOthersSound != null)
            {
                AudioSource.PlayClipAtPoint(hitOthersSound, hitInfo.point);
            }
        }
    


     
        if (audioSource != null && hitOthersSound != null)
        {
            AudioSource.PlayClipAtPoint(hitOthersSound, hitInfo.point);
        }
    }


    private Vector3 GetAimTarget()
    {
        
        Vector2 screenCenterPoint = new Vector2(Screen.width / 2f, Screen.height / 2f);
        Ray ray = Camera.main.ScreenPointToRay(screenCenterPoint);

        if (Physics.Raycast(ray, out RaycastHit raycastHit, 999f, aimColliderLayerMask))
        {
            return raycastHit.point;
        }

        return ray.GetPoint(999f); // Hiçbir şeye çarpmazsa uzak noktaya gider
    }

   
    void Update()
    {
        // Nişan alma tespiti (Kameradan hedefi bulma) 
        Vector3 mouseWorldPosition = GetAimTarget();

        //  Ateşleme zamanlaması ve kontrolü
        if (currentIsAiming && isShooting && Time.time >= nextFireTime)
        {
            // Sadece nişan alıyorsak ateş et
            Shoot(GetAimTarget()); // GetAimTarget metodunu da Update içinde çağırabiliriz.
            nextFireTime = Time.time + FireRate;
        }
        



    }


}
