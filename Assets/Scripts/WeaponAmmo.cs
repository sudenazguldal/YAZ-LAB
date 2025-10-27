using UnityEngine;

public class WeaponAmmo : MonoBehaviour
{
    [Header("Ammo ")]
    [SerializeField] private int ClipSize;
    [SerializeField] private int ExtraAmmo;
    [SerializeField] public int CurrentAmmo;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource; // Atýþ sesini çalacak AudioSource bileþeni
    [SerializeField] private AudioClip ReloadSound;
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
        // Yeniden doldurma iþlemi gerçekleþip gerçekleþmediðini kontrol eden bayrak
        bool reloadOccurred = false;

        // Þu anki klipte kaç mermilik boþluk var?
        int ammoNeeded = ClipSize - CurrentAmmo;

        // Eðer zaten tam doluysa veya ekstra mermi yoksa, ses çalmayacaðýz
        if (ammoNeeded <= 0 || ExtraAmmo <= 0)
        {
            return; // Ýþlem yok, metottan çýk
        }

        // Ekstra mühimmat, klipsi tamamen doldurmaya yetiyor mu?
        if (ExtraAmmo >= ammoNeeded)
        {
            // Klipsi tamamen doldur.
            ExtraAmmo -= ammoNeeded;
            CurrentAmmo = ClipSize;
            reloadOccurred = true;
        }
        // Ekstra mühimmat var ama klipsi tam doldurmaya yetmiyor.
        else // (ExtraAmmo > 0 && ExtraAmmo < ammoNeeded)
        {
            // Tüm ekstra mühimmatý CurrentAmmo'ya ekle.
            CurrentAmmo += ExtraAmmo;
            ExtraAmmo = 0;
            reloadOccurred = true;
        }

        // Eðer baþarýlý bir doldurma iþlemi gerçekleþtiyse sesi çal.
        if (reloadOccurred)
        {
            // Ses çalma iþlemini buraya ekleyin:
            // AudioClip'i AudioSource bileþeninde bir defalýk (OneShot) olarak çalar.
            audioSource.PlayOneShot(ReloadSound);
        }
    }


}
