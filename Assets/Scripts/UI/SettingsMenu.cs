using System.Collections.Generic;
using UnityEditor.ShaderGraph;
using UnityEngine;
using UnityEngine.Audio;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;

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

        // 🎧 ----- Voice -----
        float masterVol = PlayerPrefs.GetFloat("MasterVol", 0f);
        float musicVol = PlayerPrefs.GetFloat("MusicVol", 0f);
        float sfxVol = PlayerPrefs.GetFloat("SFXVol", 0f);

        audioMixer.SetFloat("MasterVol", masterVol);
        audioMixer.SetFloat("MusicVol", musicVol);
        audioMixer.SetFloat("SFXVol", sfxVol);

        if (masterSlider != null) masterSlider.value = Mathf.InverseLerp(-80f, 0f, masterVol);
        if (musicSlider != null) musicSlider.value = Mathf.InverseLerp(-80f, 0f, musicVol);
        if (sfxSlider != null) sfxSlider.value = Mathf.InverseLerp(-80f, 0f, sfxVol);

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
    //  AUDIO
    // =====================================================
    public void SetMasterVolume(float value)
    {
        float volume = Mathf.Lerp(-20f, 0f, value);
        audioMixer.SetFloat("MasterVol", volume);
        PlayerPrefs.SetFloat("MasterVol", volume);
    }

    public void SetMusicVolume(float value)
    {
        float volume = Mathf.Lerp(-20f, 0f, value);
        audioMixer.SetFloat("MusicVol", volume);
        PlayerPrefs.SetFloat("MusicVol", volume);
    }

    public void SetSFXVolume(float value)
    {
        float volume = Mathf.Lerp(-20f, 0f, value);
        audioMixer.SetFloat("SFXVol", volume);
        PlayerPrefs.SetFloat("SFXVol", volume);
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
        // Ses ayarlarını kaydet
        if (masterSlider != null)
        {
            float masterVol = Mathf.Lerp(-30f, 0f, masterSlider.value); // -30 yerine -80 istersen eski sistem
            audioMixer.SetFloat("MasterVol", masterVol);
            PlayerPrefs.SetFloat("MasterVol", masterVol);
        }

        if (musicSlider != null)
        {
            float musicVol = Mathf.Lerp(-30f, 0f, musicSlider.value);
            audioMixer.SetFloat("MusicVol", musicVol);
            PlayerPrefs.SetFloat("MusicVol", musicVol);
        }

        if (sfxSlider != null)
        {
            float sfxVol = Mathf.Lerp(-30f, 0f, sfxSlider.value);
            audioMixer.SetFloat("SFXVol", sfxVol);
            PlayerPrefs.SetFloat("SFXVol", sfxVol);
        }

        // Grafik & kontrol ayarlarını uygula
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

        // 💾 Diske yaz
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

            Debug.Log(" Değişiklikler geri alındı.");
        }

        if (SettingsPanel != null)
            SettingsPanel.SetActive(false);
        if (CanvasMainMenu != null)
            CanvasMainMenu.SetActive(true);

        Debug.Log(" Ana menüye dönüldü.");
    }
}
