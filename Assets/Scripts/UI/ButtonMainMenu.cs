using UnityEngine;
using UnityEngine.UI;      
using UnityEngine.EventSystems;
using TMPro;

[RequireComponent(typeof(Button))]
public class ButtonMainMenu : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Hover Settings")]
    public float scaleAmount = 1.2f;
    public float scaleSpeed = 6f;
    public Color normalColor = new Color(0.5f, 0.5f, 0.5f); // #808080
    public Color hoverColor = Color.white;

    private Vector3 originalScale;
    private TMP_Text textMesh;
    private bool isHovered = false;
    private bool initialized = false;

    void Start()
    {
        originalScale = transform.localScale;
        textMesh = GetComponentInChildren<TMP_Text>();

        if (textMesh != null)
            textMesh.color = normalColor;

        initialized = true;
    }

    void Update()
    {
        if (!initialized) return;

        Vector3 targetScale = isHovered ? originalScale * scaleAmount : originalScale;
        transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.unscaledDeltaTime * scaleSpeed);

        if (textMesh != null)
        {
            Color targetColor = isHovered ? hoverColor : normalColor;
            textMesh.color = Color.Lerp(textMesh.color, targetColor, Time.unscaledDeltaTime * scaleSpeed);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovered = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovered = false;
    }
}
