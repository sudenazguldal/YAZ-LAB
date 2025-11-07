using UnityEngine;
using UnityEngine.UI;

public class CrosshairController : MonoBehaviour
{
    [Header("References")]
    public Image crosshairImage;

    public Sprite[] crosshairSprites; // Dot, Circle, Cross, X, Knob

    private int currentIndex;

    void Start()
    {
        currentIndex = PlayerPrefs.GetInt("SelectedCrosshair", 0);
        UpdateCrosshair(currentIndex);
    }

    public void UpdateCrosshair(int index)
    {
        if (crosshairSprites.Length == 0 || index >= crosshairSprites.Length) return;

        crosshairImage.sprite = crosshairSprites[index];
        PlayerPrefs.SetInt("SelectedCrosshair", index);

        if (index == 0) // 5. eleman Knob ise
            crosshairImage.rectTransform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        else
        {
            // Diðer crosshair'lar için normal ölçek
            crosshairImage.rectTransform.localScale = new Vector3(0.7f, 0.7f, 0.7f);
        }
    }
}
