using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using System;

public class SettingsMenu : MonoBehaviour
{
    [Header("Audio Settings")]
    public AudioMixer audioMixer;
    public Slider masterSlider;
    public Slider musicSlider;
    public Slider sfxSlider;

    [Header("Graphics Settings")]
    public Dropdown qualityDropdown;
    public Toggle fullscreenToggle;
    
    [Header("Control Settings")]
    public Slider sensitivitySlider;

    public static float MouseSensitivity = 1f;

    void Start()
    {

        qualityDropdown.ClearOptions();
        qualityDropdown.AddOptions(new System.Collections.Generic.List<string>(QualitySettings.names));
        int q = PlayerPrefs.GetInt("QualityLevel", QualitySettings.GetQualityLevel());
        qualityDropdown.value = q;
        SetQuality(q);

        // Kaydedilmiþ ayar varsa yükle
        if (PlayerPrefs.HasKey("MasterVol"))
        {
            LoadVolume();
        }
        else
        {
            SetMasterVolume(masterSlider.value);
            SetMusicVolume(musicSlider.value);
            SetSFXVolume(sfxSlider.value);
        }

        if (PlayerPrefs.HasKey("QualityLevel"))
        {
            qualityDropdown.value = PlayerPrefs.GetInt("QualityLevel");
            SetQuality(qualityDropdown.value);
        }
        if (PlayerPrefs.HasKey("Fullscreen"))
        {
            bool fs = PlayerPrefs.GetInt("Fullscreen") == 1;
            fullscreenToggle.isOn = fs;
            SetFullscreen(fs);
        }

        if (PlayerPrefs.HasKey("MouseSensitivity"))
        {
            float sens = PlayerPrefs.GetFloat("MouseSensitivity");
            sensitivitySlider.value = sens;
            SetMouseSensitivity(sens);
        }
    }

    public void SetMasterVolume(float value)
    {
        audioMixer.SetFloat("MasterVol", Mathf.Log10(value) * 20);
        PlayerPrefs.SetFloat("MasterVol", value);
    }

    public void SetMusicVolume(float value)
    {
        audioMixer.SetFloat("MusicVol", Mathf.Log10(value) * 20);
        PlayerPrefs.SetFloat("MusicVol", value);
    }

    public void SetSFXVolume(float value)
    {
        audioMixer.SetFloat("SFXVol", Mathf.Log10(value) * 20);
        PlayerPrefs.SetFloat("SFXVol", value);
    }

    void LoadVolume()
    {
        float master = PlayerPrefs.GetFloat("MasterVol");
        float music = PlayerPrefs.GetFloat("MusicVol");
        float sfx = PlayerPrefs.GetFloat("SFXVol");

        masterSlider.value = master;
        musicSlider.value = music;
        sfxSlider.value = sfx;

        SetMasterVolume(master);
        SetMusicVolume(music);
        SetSFXVolume(sfx);
    }

    public void SetQuality(int index)
    {
        QualitySettings.SetQualityLevel(index);
        PlayerPrefs.SetInt("QualityLevel", index);
    }

    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
        PlayerPrefs.SetInt("Fullscreen", isFullscreen ? 1 : 0);
    }

    public void SetMouseSensitivity(float value)
    {
        MouseSensitivity = value;
        PlayerPrefs.SetFloat("MouseSensitivity", value);
    }

    public void ApplyChanges()
    {
        PlayerPrefs.Save();
        Debug.Log("Settings Applied!");
    }

    public void BackToMainMenu()
    {
        Debug.Log("Back to main menu");
        // SceneManager.LoadScene("MainMenu"); // veya paneli kapat
    }

}
