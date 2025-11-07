using UnityEngine;
using UnityEngine.UI;

public class CrosshairVisibilityUI : MonoBehaviour
{
    private Image crosshairImage;

    void Start()
    {
        crosshairImage = GetComponent<Image>();
        crosshairImage.enabled = false; // oyun baþýnda gizli
    }

    void Update()
    {
        bool isAiming = Input.GetButton("Fire2"); // sað týk (niþan)
        crosshairImage.enabled = isAiming;
    }
}
