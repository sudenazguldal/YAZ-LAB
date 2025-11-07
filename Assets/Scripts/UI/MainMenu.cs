using Esper.ESave;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [Header("UI References")]
    public GameObject CanvasMainMenu;    // CanvasMainMenu
    public GameObject CanvasSettings;    // Canvas Settings
    public GameObject SettingsPanel;     // SettingsPanel

    void Start()
    {
        //  Oyun her zaman ana menüden başlasın
        if (CanvasMainMenu != null) CanvasMainMenu.SetActive(true);
        if (CanvasSettings != null) CanvasSettings.SetActive(false);
        
    }

    public void ContinueGame()
    {
        if (SaveStorage.instance == null)
        {
            SceneManager.LoadScene("MainScene");
            return;
        }

        var saveFile = SaveStorage.instance.GetSaveByFileName("MainSave");
        string savePath = Path.Combine(Application.persistentDataPath, "YAZ-LAB", "MainSave.json");

        if (saveFile == null || !File.Exists(savePath))
        {
            Debug.Log("MainMenu: Kayıt bulunamadı. Yeni oyun başlatılıyor.");
            SceneManager.LoadScene("MainScene");
            return;
        }

        if (saveFile.HasData("SceneName"))
        {
            saveFile.Load();
            string lastScene = saveFile.GetData<string>("SceneName");
            Debug.Log("MainMenu: Kayıtlı sahneye yükleniyor: " + lastScene);
            SceneManager.LoadScene(lastScene);
        }
        else
        {
            Debug.Log("MainMenu: SceneName eksik. Yeni oyun başlatılıyor.");
            SceneManager.LoadScene("MainScene");
        }
    }

    public void NewGame()
    {
        FileDeleter.DeleteMainSave();

        if (SaveStorage.instance != null)
        {
            var residual = SaveStorage.instance.GetSaveByFileName("MainSave");
            if (residual != null)
                residual.EmptyFile();
        }

        SceneManager.LoadScene("MainScene");
    }

    public void OpenSettings()
    {
        Debug.Log("Settings opened");

        //  Ana menüyü gizle
        if (CanvasMainMenu != null)
            CanvasMainMenu.SetActive(false);

        //  Settings menüsünü göster
        if (CanvasSettings != null)
            CanvasSettings.SetActive(true);

        if (SettingsPanel != null)
            SettingsPanel.SetActive(true);
    }

    public void ExitGame()
    {
        Debug.Log("The game is closing...");
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
