using UnityEngine;
using UnityEngine.UI;

public class CrosshairVisibilityUI : MonoBehaviour
{
    private Image crosshairImage;

    void Start()
    {
        crosshairImage = GetComponent<Image>();
        crosshairImage.enabled = false; // 
    }

    void Update()
    {
        bool isAiming = Input.GetButton("Fire2"); // 
        crosshairImage.enabled = isAiming;
    }
}
