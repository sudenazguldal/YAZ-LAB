using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class RevolverAmmoDisplay : MonoBehaviour
{
    [Header("UI References")]
    public TMP_Text ammoText;
    public Image reloadRing;
    public Image gunIcon;

    [Header("Ammo Settings")]
    public int currentAmmo = 6;
    public int maxAmmo = 6;
    public int currentMagazine = 4;

    [Header("Ring Rotation Settings")]
    [Range(0f, 1f)] public float ringFillAmount = 0.75f;
    public float rotationPerReload = 90f;   // 90° dönüş
    public float rotationSpeed = 2f;        // 🔽 biraz daha yavaş
    private float baseRotation = 0f;
    private Coroutine rotationCoroutine;

    void Start()
    {
        if (reloadRing != null)
            reloadRing.fillAmount = ringFillAmount;

        baseRotation = reloadRing.rectTransform.localEulerAngles.z;
        UpdateAmmoUI();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
            Fire();

        if (Input.GetKeyDown(KeyCode.R))
            Reload();
    }

    public void Fire()
    {
        if (currentAmmo > 0)
        {
            currentAmmo--;
            UpdateAmmoUI();
        }
    }

    public void Reload()
    {
        if (currentAmmo == maxAmmo || currentMagazine <= 0)
            return;

        currentMagazine--;
        currentAmmo = maxAmmo;

        if (rotationCoroutine != null)
            StopCoroutine(rotationCoroutine);

        rotationCoroutine = StartCoroutine(RotateRingSmooth());
        UpdateAmmoUI();
    }

    private IEnumerator RotateRingSmooth()
    {
        float targetRotation = baseRotation - rotationPerReload;
        float startRotation = baseRotation;
        float elapsed = 0f;

        // 1️⃣ Daha yavaş ileri dönüş (yaklaşık 0.3 sn)
        while (elapsed < 0.3f)
        {
            elapsed += Time.deltaTime * rotationSpeed;
            float t = Mathf.SmoothStep(0, 1, elapsed / 0.3f);
            float z = Mathf.Lerp(startRotation, targetRotation, t);
            reloadRing.rectTransform.localRotation = Quaternion.Euler(0, 0, z);
            yield return null;
        }

        yield return new WaitForSeconds(0.15f); // biraz bekleme

        // 2️⃣ Daha yumuşak geri dönüş (yaklaşık 0.4 sn)
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
        rotationCoroutine = null;
    }

    public void UpdateAmmoUI()
    {
        if (ammoText != null)
            ammoText.text = $"{currentMagazine} / {currentAmmo}";
    }
}
