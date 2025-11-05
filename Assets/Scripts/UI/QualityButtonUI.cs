using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

[RequireComponent(typeof(UnityEngine.UI.Button))]
public class QualityButtonUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [Header("Quality Index")]
    public int QualityIndex = 0;

    [Header("Color Settings")]
    public Color NormalColor = new Color(0.7f, 0.7f, 0.7f); // Hafif gri
    public Color HoverColor = Color.white; // Beyaz

    [Header("Scale Settings")]
    public float NormalScale = 1f;
    public float HoverScale = 1.2f;
    public float ActiveScale = 1.2f; // Týklanýnca bu ölçekte kal
    public float ScaleSpeed = 10f;

    private TMP_Text textMesh;
    private Vector3 baseScale;
    private bool isHovered = false;
    private bool isActive = false;

    public delegate void QualityButtonClicked(QualityButtonUI button);
    public event QualityButtonClicked OnButtonClicked;

    void Awake()
    {
        baseScale = transform.localScale;
        textMesh = GetComponentInChildren<TMP_Text>();
        if (textMesh != null)
            textMesh.color = NormalColor;
    }

    void Update()
    {
        // Ölçek animasyonu
        float targetScale = NormalScale;

        if (isHovered && !isActive)
            targetScale = HoverScale;
        else if (isActive)
            targetScale = ActiveScale;

        transform.localScale = Vector3.Lerp(
            transform.localScale,
            baseScale * targetScale,
            Time.unscaledDeltaTime * ScaleSpeed
        );

        // Renk animasyonu
        if (textMesh != null)
        {
            Color targetColor = NormalColor;

            if (isHovered || isActive)
                targetColor = HoverColor;

            textMesh.color = Color.Lerp(textMesh.color, targetColor, Time.unscaledDeltaTime * ScaleSpeed);
        }
    }

    public void SetActive(bool active)
    {
        isActive = active;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovered = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovered = false;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        OnButtonClicked?.Invoke(this);
    }
}
