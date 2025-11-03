using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    [Header("Shooting Config")]
    [SerializeField] private float fireRate = 0.15f;
    public float FireRate => fireRate; // Dışarıdan okumak için
    [SerializeField] private float damage = 10f;
    [SerializeField] private LayerMask aimColliderLayerMask = default; 

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
        if (currentIsAiming == false) return; // Sadece nişan alındığında ateşle
        if (Ammo.CurrentAmmo == 0)
        {

            if (audioSource != null && emptyGunSound != null)
            {

                audioSource.PlayOneShot(emptyGunSound);
            }


            return; // Mermi yoksa ateşleme
        }




        Vector3 bulletSpawnPosition = spawnBulletPosition.position;

        if (isPeeking)
        {
            // Namlu pozisyonunu siperden uzanıyormuş gibi yukarı kaydır
            bulletSpawnPosition += Vector3.up * peekHeightOffset;
        }


        // 1. Namlu Alevi Efekti


        if (vfxMuzzleFlash != null)
        {


            //  1. TANIMLAMA ve OLUŞTURMA 
            GameObject muzzleFlashInstance = Instantiate(
        vfxMuzzleFlash,
        bulletSpawnPosition,
      spawnBulletPosition.rotation
      );

            ParticleSystem ps = muzzleFlashInstance.GetComponent<ParticleSystem>();

            if (ps != null)
            {
                // 1. Manuel Hesaplamaya Geri Dönüş
                // Parçacıkların maksimum ömrünü al (eğer ayar sabit değilse .constantMax kullanılır)
                float maxLifetime = ps.main.startLifetime.constantMax;

                // Toplam Süre = Yayma Süresi + Parçacık Ömrü
                float totalDuration = ps.main.duration + maxLifetime;

                ps.Play(); // Manuel olarak başlat

                // 2. Güvenli Marj ile Yok Et
                Destroy(muzzleFlashInstance, totalDuration + 0.1f);
            }
            else
            {
                // Eğer Partikül Sistemi yoksa (veya bulunamazsa), yine de yok et
                Destroy(muzzleFlashInstance, 0.5f);
            }




        }

        if (audioSource != null && shootSound != null)
        {
            // PlayOneShot, AudioSource'un mevcut sesini kesmeden yeni bir ses çalar.
            audioSource.PlayOneShot(shootSound);
        }

        //  Namludan Hedefe Yön Hesabı
        Vector3 shootDirection = (targetPoint - bulletSpawnPosition).normalized;

        // 3. HITSCAN RAYCAST
        RaycastHit hitInfo;
        if (Physics.Raycast(bulletSpawnPosition, shootDirection, out hitInfo, 1000f))
        {
            
            HandleHit(hitInfo);
        }
        else {
            
        }
        

        Ammo.CurrentAmmo--;
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

    // public void Shoot(Vector3 targetPoint) metodu aynı kalır.
    // private void HandleHit(RaycastHit hitInfo) metodu aynı kalır.
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
        if (isShooting && Time.time >= nextFireTime)
        {
            Shoot(mouseWorldPosition);
            nextFireTime = Time.time + FireRate;
        }



    }


}
