using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    [Header("Shooting Config")]
    [SerializeField] private float fireRate = 0.15f;
    public float FireRate => fireRate; // Dýþarýdan okumak için
    [SerializeField] private float damage = 10f;
    [SerializeField] private LayerMask aimColliderLayerMask = default; // Gerekirse

    [Header("VFX")]
    [SerializeField] private Transform spawnBulletPosition;
    [SerializeField] private GameObject vfxMuzzleFlash;
    [SerializeField] private GameObject vfxHitTarget;
    [SerializeField] private GameObject vfxHitOthers;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource; // Atýþ sesini çalacak AudioSource bileþeni
    [SerializeField] private AudioClip shootSound;     // Ana atýþ sesi klibi
    [SerializeField] private AudioClip hitTargetSound; // Zombiye vuruþ sesi (Opsiyonel)
    [SerializeField] private AudioClip hitOthersSound; // Duvara vuruþ sesi (Opsiyonel)
    [SerializeField] private AudioClip emptyGunSound;
    [Header("Cover Shooting")]
    private bool isPeeking = false; // PlayerController'dan gelen Cover Aim durumu
    [SerializeField] private float peekHeightOffset = 0.6f; // Siperden uzanma yüksekliði (Örn: 0.6m)

    WeaponAmmo Ammo;

    private float nextFireTime = 0.1f;
    private bool isShooting = false;
    private bool currentIsAiming = false; // PlayerController'dan gelecek Aim durumunu tutar

    // PlayerController'dan gelecek input'u yönetir

    void Start()
    {
        Ammo = GetComponentInChildren<WeaponAmmo>();
    }

    // PlayerController'dan çaðrýlacak Aim durumunu güncelleme metodu
    public void SetAiming(bool isAiming, bool isCovering)
    {
        currentIsAiming = isAiming;

        // isPeeking durumu: Siperdeysen VE niþan alýyorsan TRUE
        isPeeking = isCovering && isAiming;
    }
    public void HandleShootInput(bool isPressed)
    {
        isShooting = isPressed;
    }

    // PlayerController'dan çaðrýlacak ana metod
    public void Shoot(Vector3 targetPoint)
    {
        if (currentIsAiming == false) return; // Sadece niþan alýndýðýnda ateþle
        if (Ammo.CurrentAmmo == 0) {

            if (audioSource != null && emptyGunSound != null)
            {

                audioSource.PlayOneShot(emptyGunSound);
            }


            return; // Mermi yoksa ateþleme
         }
            
            
            

        Vector3 bulletSpawnPosition = spawnBulletPosition.position;

        if (isPeeking)
        {
            // Namlu pozisyonunu siperden uzanýyormuþ gibi yukarý kaydýr
            bulletSpawnPosition += Vector3.up * peekHeightOffset;
        }


        // 1. Namlu Alevi Efekti


        if (vfxMuzzleFlash != null)
        {


            //  1. TANIMLAMA ve OLUÞTURMA (Scope için kritik)
            GameObject muzzleFlashInstance = Instantiate(
                vfxMuzzleFlash,
                bulletSpawnPosition,
            spawnBulletPosition.rotation
            );

            ParticleSystem ps = muzzleFlashInstance.GetComponent<ParticleSystem>();

            if (ps != null)
            {
                // 1. Manuel Hesaplamaya Geri Dönüþ
                // Parçacýklarýn maksimum ömrünü al (eðer ayar sabit deðilse .constantMax kullanýlýr)
                float maxLifetime = ps.main.startLifetime.constantMax;

                // Toplam Süre = Yayma Süresi + Parçacýk Ömrü
                float totalDuration = ps.main.duration + maxLifetime;

                ps.Play(); // Manuel olarak baþlat

                // 2. Güvenli Marj ile Yok Et
                Destroy(muzzleFlashInstance, totalDuration + 0.1f);
            }
            else
            {
                // Eðer Partikül Sistemi yoksa (veya bulunamazsa), yine de yok et
                Destroy(muzzleFlashInstance, 0.5f);
            }




        }

        if (audioSource != null && shootSound != null)
        {
            // PlayOneShot, AudioSource'un mevcut sesini kesmeden yeni bir ses çalar.
            audioSource.PlayOneShot(shootSound);
        }

        //  Namludan Hedefe Yön Hesabý
        Vector3 shootDirection = (targetPoint - bulletSpawnPosition).normalized;

        // 3. HITSCAN RAYCAST
        RaycastHit hitInfo;
        if (Physics.Raycast(bulletSpawnPosition, shootDirection, out hitInfo, 1000f, aimColliderLayerMask))
        {
            HandleHit(hitInfo);
        }

        Ammo.CurrentAmmo--;
    }

    private void HandleHit(RaycastHit hitInfo)
    {
        // ... (Hasar ve VFX kodunuz buraya gelir)
        // HealthComponent'e hasar verme, efektleri oynatma vb.

        // Vurulacak nesnenin Zombi mi yoksa Duvar mý olduðunu anlama
        HealthComponent targetHealth = hitInfo.transform.GetComponent<HealthComponent>();

        // Vuruþ efekti ve sesin doðru yöne bakmasý için dönüþ açýsý
        // hitInfo.normal: Vurulan yüzeyin normali (yüzeye dik olan yön)
        Quaternion hitRotation = Quaternion.LookRotation(hitInfo.normal);

        if (targetHealth != null)
        {
            // ZOMBÝ VURULDU
            // ... (VFX kodlarý)

            // Zombi Vuruþ Sesi
            if (audioSource != null && hitTargetSound != null)
            {
                // Vuruþ sesini, merminin çarptýðý yerde çalmak daha gerçekçi olabilir.
                // Bu yüzden PlayClipAtPoint kullanmak daha iyi olabilir.
                AudioSource.PlayClipAtPoint(hitTargetSound, hitInfo.point);
            }
        }
        else
        {

            // Duvar/Zemin Vuruldu
            if (vfxHitOthers != null)
            {
                GameObject HitOthersInstance = Instantiate(
                    vfxHitOthers,
                    hitInfo.point,
                    hitRotation
                );

                ParticleSystem ps = HitOthersInstance.GetComponent<ParticleSystem>();

                if (ps != null)
                {
                    // 1. Manuel Hesaplamaya Geri Dönüþ
                    // Parçacýklarýn maksimum ömrünü al
                    float maxLifetime = ps.main.startLifetime.constantMax;

                    // Total Süre = Yayma Süresi + Parçacýk Ömrü
                    float totalDuration = ps.main.duration + maxLifetime;

                    ps.Play();

                    // 2. Güvenli Marj ile Yok Et
                    Destroy(HitOthersInstance, totalDuration + 0.1f);
                }
                else
                {
                    // KRÝTÝK DÜZELTME: Yanlýþ deðiþkeni yok etme hatasý düzeltildi.
                    // Eðer Partikül Sistemi yoksa, yine de objeyi yok et.
                    Destroy(HitOthersInstance, 0.5f);
                }
            }




        }

        // Diðer Vuruþ Sesi
        if (audioSource != null && hitOthersSound != null)
        {
            AudioSource.PlayClipAtPoint(hitOthersSound, hitInfo.point);
        }
    }


    private Vector3 GetAimTarget()
    {
        //  Kamera Raycast Mantýðý buraya taþýndý!
        Vector2 screenCenterPoint = new Vector2(Screen.width / 2f, Screen.height / 2f);
        Ray ray = Camera.main.ScreenPointToRay(screenCenterPoint);

        if (Physics.Raycast(ray, out RaycastHit raycastHit, 999f, aimColliderLayerMask))
        {
            return raycastHit.point;
        }

        return ray.GetPoint(999f); // Hiçbir þeye çarpmazsa uzak nokta
    }

    // public void Shoot(Vector3 targetPoint) metodu ayný kalýr.
    // private void HandleHit(RaycastHit hitInfo) metodu ayný kalýr.
    void Update()
    {
        // 1. Niþan Alma Tespiti (Kameradan hedefi bulma) - Artýk bu da burada!
        Vector3 mouseWorldPosition = GetAimTarget();

        // 2. Ateþleme Zamanlamasý ve Kontrolü
        if (currentIsAiming && isShooting && Time.time >= nextFireTime)
        {
            // YENÝ KONTROL: Sadece niþan alýyorsak ateþ et.
            Shoot(GetAimTarget()); // GetAimTarget metodunu da Update içinde çaðýrabiliriz.
            nextFireTime = Time.time + FireRate;
        }
        if (isShooting && Time.time >= nextFireTime)
        {
            Shoot(mouseWorldPosition);
            nextFireTime = Time.time + FireRate;
        }



    }


}