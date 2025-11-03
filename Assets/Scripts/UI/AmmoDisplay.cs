using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AmmoDisplay : MonoBehaviour
{
    public Image[] bullets;
    public Sprite fullBullet;
    public Sprite emptyBullet;
    public int currentAmmo = 10;
    public int maxAmmo = 10;

    public TMP_Text magText;
    public int magazines = 3;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            UseBullet();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            Reload();
        }
    }

    public void UseBullet()
    {
        if (currentAmmo > 0)
        {
            currentAmmo--;
            UpdateAmmoUI();
        }
    }

    public void Reload()
    {
        // 1. Eğer zaten tam doluysa veya şarjör yoksa → hiçbir şey yapma
        if (currentAmmo == maxAmmo || magazines <= 0)
            return;

        // 2. Aksi halde dolum yap
        currentAmmo = maxAmmo;
        magazines--;

        UpdateAmmoUI();

    }

    public void UpdateAmmoUI()
    {
        for (int i = 0; i < bullets.Length; i++)
        {
            if (i < currentAmmo)
            {
                bullets[i].sprite = fullBullet;
            }
            else
            {
                bullets[i].sprite = emptyBullet;
            }

            if (magText != null)
                magText.text = magazines.ToString();

        }
    }
}