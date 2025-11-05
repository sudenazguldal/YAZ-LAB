using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    [Header("Shooting Config")]
    [SerializeField] private float fireRate = 0.15f;
    public float FireRate => fireRate; // Dışarıdan okumak için
    [SerializeField] private float damage = 25f;
    [SerializeField] private LayerMask aimColliderLayerMask = default; // Gerekirse


    [Header("VFX")]
    [SerializeField] private Transform spawnBulletPosition;
    [SerializeField] private GameObject vfxMuzzleFlash;
    [SerializeField] private GameObject vfxHitTarget;
    [SerializeField] private GameObject vfxHitOthers;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource; // Atış sesini çalacak AudioSource bileşeni
    [SerializeField] private AudioClip shootSound;     // Ana atış sesi klibi
    [SerializeField] private AudioClip hitTargetSound; // Zombiye vuruş sesi (Opsiyonel)
    [SerializeField] private AudioClip hitOthersSound; // Duvara vuruş sesi (Opsiyonel)
    [SerializeField] private AudioClip emptyGunSound;
    [Header("Cover Shooting")]
    private bool isPeeking = false; // PlayerController'dan gelen Cover Aim durumu
    [SerializeField] private float peekHeightOffset = 0.6f; // Siperden uzanma yüksekliği (Örn: 0.6m)

    [Header("Inventory")]
    [Tooltip("Envanter veri ScriptableObject'ini buraya sürükleyin.")]
    [SerializeField] private InventoryData inventoryData;
    [SerializeField] private int ammoCostPerShot = 1; // Her atışta harcanan mermi miktarı (çoğu zaman 1'dir)

    WeaponAmmo Ammo;

    private float nextFireTime = 0.1f;
    private bool isShooting = false;
    private bool currentIsAiming = false; // PlayerController'dan gelecek Aim durumunu tutar

    // PlayerController'dan gelecek input'u yönetir

    void Start()
    {
        Ammo = GetComponentInChildren<WeaponAmmo>();
    }

    // PlayerController'dan çağrılacak Aim durumunu güncelleme metodu
    public void SetAiming(bool isAiming, bool isCovering)
    {
        currentIsAiming = isAiming;

        // isPeeking durumu: Siperdeysen VE nişan alıyorsan TRUE
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
        if (Ammo == null) { Debug.LogError("WeaponAmmo referansı eksik!"); return; }

        // ----------------------------------------------------------------------
        // 1. MÜHİMMAT KONTROLÜ
        // ----------------------------------------------------------------------
        if (Ammo.CurrentAmmo <= 0)
        {
            // Mermi bittiğinde tık sesi çal
            if (audioSource != null && emptyGunSound != null)
            {
                audioSource.PlayOneShot(emptyGunSound);
            }
            return; // Mermi yoksa ateşleme
        }

        // ----------------------------------------------------------------------
        // 2. MÜHİMMAT HARCAMA (SADECE ŞARJÖRDEN DÜŞ)
        // ----------------------------------------------------------------------
        Ammo.CurrentAmmo--; // Şarjörden 1 mermi düş
                            // NOT: isabet veya harcama başarısız olsa bile mermi atıldığı için düşülür.

        // ----------------------------------------------------------------------
        // 3. POZİSYON VE PEEK HESAPLAMASI
        // ----------------------------------------------------------------------
        Vector3 bulletSpawnPosition = spawnBulletPosition.position;

        if (isPeeking)
        {
            // Namlu pozisyonunu siperden uzanıyormuş gibi yukarı kaydır
            bulletSpawnPosition += Vector3.up * peekHeightOffset;
        }

        // ----------------------------------------------------------------------
        // 4. VFX VE SES
        // ----------------------------------------------------------------------
        if (vfxMuzzleFlash != null)
        {
            // Muzzle Flash oluşturma ve yok etme mantığı (Mevcut kodunuz)
            GameObject muzzleFlashInstance = Instantiate(vfxMuzzleFlash, bulletSpawnPosition, spawnBulletPosition.rotation);
            ParticleSystem ps = muzzleFlashInstance.GetComponent<ParticleSystem>();
            // ... (Kalan VFX mantığı)
            Destroy(muzzleFlashInstance, 0.5f); // Basitleştirilmiş yok etme
        }

        if (audioSource != null && shootSound != null)
        {
            audioSource.PlayOneShot(shootSound);
        }

        // ----------------------------------------------------------------------
        // 5. HITSCAN
        // ----------------------------------------------------------------------
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

            //  Zombi Vuruş Sesi
            if (audioSource != null && hitTargetSound != null)
            {
                Debug.Log("Playing hit target sound");
                audioSource.PlayOneShot(hitTargetSound, 1.0f);
            }
        }
        else
        {
            // Duvar/Zemin vuruldu
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
    


        // Diğer Vuruş Sesi
        if (audioSource != null && hitOthersSound != null)
        {
            AudioSource.PlayClipAtPoint(hitOthersSound, hitInfo.point);
        }
    }


    private Vector3 GetAimTarget()
    {
        //  Kamera Raycast Mantığı buraya taşındı!
        Vector2 screenCenterPoint = new Vector2(Screen.width / 2f, Screen.height / 2f);
        Ray ray = Camera.main.ScreenPointToRay(screenCenterPoint);

        if (Physics.Raycast(ray, out RaycastHit raycastHit, 999f, aimColliderLayerMask))
        {
            return raycastHit.point;
        }

        return ray.GetPoint(999f); // Hiçbir şeye çarpmazsa uzak nokta
    }

   
    void Update()
    {
        // 1. Nişan Alma Tespiti (Kameradan hedefi bulma) - Artık bu da burada!
        Vector3 mouseWorldPosition = GetAimTarget();

        // 2. Ateşleme Zamanlaması ve Kontrolü
        if (currentIsAiming && isShooting && Time.time >= nextFireTime)
        {
            // YENİ KONTROL: Sadece nişan alıyorsak ateş et.
            Shoot(GetAimTarget()); // GetAimTarget metodunu da Update içinde çağırabiliriz.
            nextFireTime = Time.time + FireRate;
        }
        



    }


}
