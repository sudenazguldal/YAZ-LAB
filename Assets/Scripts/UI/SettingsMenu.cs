using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using System.Collections.Generic;
using TMPro;

public class SettingsMenu : MonoBehaviour
{
    [Header("Audio Settings")]
    public AudioMixer audioMixer;
    public Slider masterSlider;
    public Slider musicSlider;
    public Slider sfxSlider;

    [Header("Graphics Settings")]
    public Toggle fullscreenToggle;
    public List<QualityButtonUI> QualityButtons;

    [Header("Control Settings")]
    public Slider aimSensitivitySlider;
    public static float MouseSensitivity = 1f;

    [Header("UI References")]
    public GameObject SettingsPanel;
    public GameObject CanvasMainMenu;

    private int pendingQualityLevel;
    private bool pendingFullscreen;
    private float pendingMouseSens;

    private int savedQualityLevel;
    private bool savedFullscreen;
    private float savedMouseSens;

    public TMP_Dropdown crosshairDropdown;
    public CrosshairController crosshairController;

    private int pendingCrosshairIndex;
    private int savedCrosshairIndex;

    void Start()
    {
        // ----- Load Saved -----
        savedQualityLevel = PlayerPrefs.GetInt("QualityLevel", QualitySettings.GetQualityLevel());
        savedFullscreen = PlayerPrefs.GetInt("Fullscreen", Screen.fullScreen ? 1 : 0) == 1;
        savedMouseSens = PlayerPrefs.GetFloat("MouseSensitivity", 1f);

        // ----- Apply Saved -----
        pendingQualityLevel = savedQualityLevel;
        pendingFullscreen = savedFullscreen;
        pendingMouseSens = savedMouseSens;

        fullscreenToggle.isOn = savedFullscreen;
        aimSensitivitySlider.value = savedMouseSens;
        UpdateQualityButtonVisuals(savedQualityLevel);

        MouseSensitivity = savedMouseSens;

        foreach (var button in QualityButtons)
            button.OnButtonClicked += OnQualityButtonClicked;

        savedCrosshairIndex = PlayerPrefs.GetInt("SelectedCrosshair", 0);
        pendingCrosshairIndex = savedCrosshairIndex;
        crosshairDropdown.value = savedCrosshairIndex;
        crosshairDropdown.onValueChanged.AddListener(OnCrosshairChanged);
    }

    // =====================================================
    // GRAPHICS
    // =====================================================
    private void OnQualityButtonClicked(QualityButtonUI clickedButton)
    {
        foreach (var btn in QualityButtons)
            btn.SetActive(false);
        clickedButton.SetActive(true);
        SetQuality(clickedButton.QualityIndex);
    }

    private void UpdateQualityButtonVisuals(int activeIndex)
    {
        foreach (var btn in QualityButtons)
            btn.SetActive(btn.QualityIndex == activeIndex);
    }

    public void SetQuality(int index) => pendingQualityLevel = index;
    public void SetFullscreen(bool isFullscreen) => pendingFullscreen = isFullscreen;

    // =====================================================
    // SENSITIVITY
    // =====================================================
    public void SetMouseSensitivity(float value)
    {
        pendingMouseSens = value;
        MouseSensitivity = value;
    }

    private void OnCrosshairChanged(int index)
    {
        pendingCrosshairIndex = index;
    }

    // =====================================================
    // APPLY
    // =====================================================
    public void ApplyChanges()
    {
        QualitySettings.SetQualityLevel(pendingQualityLevel);
        Screen.fullScreen = pendingFullscreen;
        MouseSensitivity = pendingMouseSens;

        PlayerPrefs.SetInt("QualityLevel", pendingQualityLevel);
        PlayerPrefs.SetInt("Fullscreen", pendingFullscreen ? 1 : 0);
        PlayerPrefs.SetFloat("MouseSensitivity", MouseSensitivity);
        // 🔹 Crosshair seçimini uygula
        PlayerPrefs.SetInt("SelectedCrosshair", pendingCrosshairIndex);
        PlayerPrefs.Save();

        // CrosshairController sahnede açıksa (örneğin Settings sahnesinde test ediyorsan)
        if (crosshairController != null)
            crosshairController.UpdateCrosshair(pendingCrosshairIndex);

        savedCrosshairIndex = pendingCrosshairIndex;
        PlayerPrefs.Save();

        savedQualityLevel = pendingQualityLevel;
        savedFullscreen = pendingFullscreen;
        savedMouseSens = pendingMouseSens;

        Debug.Log($"Ayarlar kaydedildi! Sensitivity={MouseSensitivity}");
    }

    // =====================================================
    // BACK TO MAIN MENU
    // =====================================================
    public void BackToMainMenu()
    {
        bool changed =
            pendingQualityLevel != savedQualityLevel ||
            pendingFullscreen != savedFullscreen ||
            Mathf.Abs(pendingMouseSens - savedMouseSens) > 0.001f;

        if (changed)
        {
            QualitySettings.SetQualityLevel(savedQualityLevel);
            Screen.fullScreen = savedFullscreen;
            MouseSensitivity = savedMouseSens;

            fullscreenToggle.isOn = savedFullscreen;
            aimSensitivitySlider.value = savedMouseSens;
            UpdateQualityButtonVisuals(savedQualityLevel);

            Debug.Log("🔄 Değişiklikler geri alındı.");
        }

        if (SettingsPanel != null)
            SettingsPanel.SetActive(false);
        if (CanvasMainMenu != null)
            CanvasMainMenu.SetActive(true);

        Debug.Log("↩️ Ana menüye dönüldü.");
    }
}
