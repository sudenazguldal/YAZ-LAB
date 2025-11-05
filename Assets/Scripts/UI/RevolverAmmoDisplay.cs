using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class RevolverAmmoDisplay : MonoBehaviour
{
    [Header("References")]
    public WeaponAmmo weaponAmmo;
    public InventoryData playerInventoryData;

    [Header("UI References")]
    public TMP_Text ammoText;
    public Image reloadRing;

    [Header("Ammo Settings")]
    public int currentAmmo = 6;   // 🔹 eklendi
    public int maxAmmo = 6;       // 🔹 eklendi
    public int currentMagazine = 4;

    [Header("Ring Rotation Settings")]
    public float rotationPerReload = 90f;
    public float rotationSpeed = 2f;
    private float baseRotation;
    private Coroutine rotationCoroutine;

    void Start()
    {
        if (reloadRing != null)
            baseRotation = reloadRing.rectTransform.localEulerAngles.z;

        UpdateAmmoUI();
    }
    void Update()
    {
        //  Her frame WeaponAmmo verisini kontrol et
        if (weaponAmmo != null)
        {
            currentAmmo = weaponAmmo.Current;   // WeaponAmmo’dan oku
            maxAmmo = weaponAmmo.Clip;
        }

        UpdateAmmoUI();
    }

    public void UpdateAmmoUI()
    {
        if (ammoText == null) return;

        int totalAmmo = playerInventoryData != null ? playerInventoryData.Ammo : 0;

        // 🔹 Şarjör / Toplam
        ammoText.text = $"{currentAmmo} / {totalAmmo}";

    }

    public void Reload()
    {
        if (rotationCoroutine != null)
            StopCoroutine(rotationCoroutine);
        rotationCoroutine = StartCoroutine(RotateRingSmooth());
    }

    private IEnumerator RotateRingSmooth()
    {
        float targetRotation = baseRotation - rotationPerReload;
        float elapsed = 0f;
        while (elapsed < 0.3f)
        {
            elapsed += Time.deltaTime * rotationSpeed;
            float t = Mathf.SmoothStep(0, 1, elapsed / 0.3f);
            float z = Mathf.Lerp(baseRotation, targetRotation, t);
            reloadRing.rectTransform.localRotation = Quaternion.Euler(0, 0, z);
            yield return null;
        }
        yield return new WaitForSeconds(0.15f);

        elapsed = 0f;
        while (elapsed < 0.4f)
        {
            elapsed += Time.deltaTime * rotationSpeed;
            float t = Mathf.SmoothStep(0, 1, elapsed / 0.4f);
            float z = Mathf.Lerp(targetRotation, baseRotation, t);
            reloadRing.rectTransform.localRotation = Quaternion.Euler(0, 0, z);
            yield return null;
        }
        reloadRing.rectTransform.localRotation = Quaternion.Euler(0, 0, baseRotation);
    }

    // UI kendi ammo değerlerini saklamaya devam ediyor ama
    // artık WeaponAmmo tarafından sürekli güncelleniyor
   
}
